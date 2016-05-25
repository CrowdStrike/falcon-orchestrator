$client = New-Object System.Net.WebClient
$file = Get-Item {{FilePath}}
$uri = {{URL}}
$client.UploadFile($uri,$file)


