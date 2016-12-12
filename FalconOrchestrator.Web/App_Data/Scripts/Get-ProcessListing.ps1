$owners = @{}
$ppid = @{}
$cmdline = @{}
Get-WmiObject win32_process |ForEach-Object {$owners[$_.Handle] = $_.GetOwner().User; $ppid[$_.handle] = $_.ParentProcessId; $cmdline[$_.handle] = $_.CommandLine}
Get-Process | Select-Object Id,  @{n="ParentProcessId";e={$ppid[$_.Id.ToString()]}}, Company,Name,@{n="Owner";e={$owners[$_.Id.ToString()]}},Path, @{n="CommandLine";e={$cmdline[$_.Id.ToString()]}},ProductVersion,StartTime,CPU,HandleCount,WorkingSet,PagedMemorySize,PrivateMemorySize,VirtualMemorySize,NonpagedSystemMemorySize,PagedSystemMemorySize,PeakPagedMemorySize,PeakWorkingSet,PeakVirtualMemorySize, Description
