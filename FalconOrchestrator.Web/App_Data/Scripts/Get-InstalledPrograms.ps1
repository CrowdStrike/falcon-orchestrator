[cmdletbinding()]            
param(            
 [parameter(ValueFromPipeline=$true,ValueFromPipelineByPropertyName=$true)]            
 [string[]]$ComputerName = $env:computername            
)            
            
begin {            
 $UninstallRegKeys=@("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall",            
     "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall")            
}            
            
process {            
 foreach($Computer in $ComputerName) {            
  Write-Verbose "Working on $Computer"            
 if(Test-Connection -ComputerName $Computer -Count 1 -ea 0) {            
  foreach($UninstallRegKey in $UninstallRegKeys) {            
   try {            
    $HKLM   = [microsoft.win32.registrykey]::OpenRemoteBaseKey('LocalMachine',$computer)            
    $UninstallRef  = $HKLM.OpenSubKey($UninstallRegKey)            
    $Applications = $UninstallRef.GetSubKeyNames()            
   } catch {            
    Write-Verbose "Failed to read $UninstallRegKey"            
    Continue            
   }            
            
   foreach ($App in $Applications) {            
   $AppRegistryKey  = $UninstallRegKey + "\\" + $App            
   $AppDetails   = $HKLM.OpenSubKey($AppRegistryKey)            
   $AppGUID   = $App            
   $AppDisplayName  = $($AppDetails.GetValue("DisplayName"))            
   $AppVersion   = $($AppDetails.GetValue("DisplayVersion"))            
   $AppPublisher  = $($AppDetails.GetValue("Publisher"))            
   $AppInstalledDate = $($AppDetails.GetValue("InstallDate"))            
   $AppUninstall  = $($AppDetails.GetValue("UninstallString"))            
   if($UninstallRegKey -match "Wow6432Node") {            
    $Softwarearchitecture = "x86"            
   } else {            
    $Softwarearchitecture = "x64"            
   }            
   if(!$AppDisplayName) { continue }            
   $OutputObj = New-Object -TypeName PSobject             
   $OutputObj | Add-Member -MemberType NoteProperty -Name ComputerName -Value $Computer.ToUpper()            
   $OutputObj | Add-Member -MemberType NoteProperty -Name AppName -Value $AppDisplayName            
   $OutputObj | Add-Member -MemberType NoteProperty -Name AppVersion -Value $AppVersion            
   $OutputObj | Add-Member -MemberType NoteProperty -Name AppVendor -Value $AppPublisher            
   $OutputObj | Add-Member -MemberType NoteProperty -Name InstalledDate -Value $AppInstalledDate            
   $OutputObj | Add-Member -MemberType NoteProperty -Name UninstallKey -Value $AppUninstall            
   $OutputObj | Add-Member -MemberType NoteProperty -Name AppGUID -Value $AppGUID            
   $OutputObj | Add-Member -MemberType NoteProperty -Name SoftwareArchitecture -Value $Softwarearchitecture            
   $OutputObj            
   }            
  }             
 }            
 }            
}            
            
end {}