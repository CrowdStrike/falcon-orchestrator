
<#
.SYNOPSIS
 
    Returns the device paths for each volume.
 
    Author: Matthew Graeber (@mattifestation)
    License: BSD 3-Clause
 
.DESCRIPTION
 
    Get-DevicePath returns the corresponding device path for each drive letter. This is useful for converting device paths to drive letters.
 
.EXAMPLE
 
    Get-DevicePath
 
    DevicePath              DriveLetter
    ----------              -----------
    \Device\HarddiskVolume2 D:
    \Device\HarddiskVolume4 C:
 
.OUTPUTS
 
    PSObject[]
 
    For each mount point, a PSObject is returned representing the drive letter and device path.
#>
    # Utilize P/Invoke in order to call QueryDosDevice. I prefer using
    # reflection over Add-Type since it doesn't require compiling C# code.
    $DynAssembly = New-Object System.Reflection.AssemblyName('SysUtils')
    $AssemblyBuilder = [AppDomain]::CurrentDomain.DefineDynamicAssembly($DynAssembly, [Reflection.Emit.AssemblyBuilderAccess]::Run)
    $ModuleBuilder = $AssemblyBuilder.DefineDynamicModule('SysUtils', $False)
 
    # Define [Kernel32]::QueryDosDevice method
    $TypeBuilder = $ModuleBuilder.DefineType('Kernel32', 'Public, Class')
    $PInvokeMethod = $TypeBuilder.DefinePInvokeMethod('QueryDosDevice', 'kernel32.dll', ([Reflection.MethodAttributes]::Public -bor [Reflection.MethodAttributes]::Static), [Reflection.CallingConventions]::Standard, [UInt32], [Type[]]@([String], [Text.StringBuilder], [UInt32]), [Runtime.InteropServices.CallingConvention]::Winapi, [Runtime.InteropServices.CharSet]::Auto)
    $DllImportConstructor = [Runtime.InteropServices.DllImportAttribute].GetConstructor(@([String]))
    $SetLastError = [Runtime.InteropServices.DllImportAttribute].GetField('SetLastError')
    $SetLastErrorCustomAttribute = New-Object Reflection.Emit.CustomAttributeBuilder($DllImportConstructor, @('kernel32.dll'), [Reflection.FieldInfo[]]@($SetLastError), @($true))
    $PInvokeMethod.SetCustomAttribute($SetLastErrorCustomAttribute)
    $Kernel32 = $TypeBuilder.CreateType()
 
    $Max = 65536
    $StringBuilder = New-Object System.Text.StringBuilder($Max)
 
    Get-WmiObject Win32_Volume | ? { $_.DriveLetter } | % {
        $ReturnLength = $Kernel32::QueryDosDevice($_.DriveLetter, $StringBuilder, $Max)
 
        if ($ReturnLength)
        {
            $DriveMapping = @{
                DriveLetter = $_.DriveLetter
                DevicePath = $StringBuilder.ToString()
            }
 
            New-Object PSObject -Property $DriveMapping
        }
    }