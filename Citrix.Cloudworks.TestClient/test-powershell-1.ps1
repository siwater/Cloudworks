# Test powershell script for UserData upload
New-Item -ItemType directory -Path powershell-folder

Write-Output "Hello world"  

# This will produce an error
Get-Date -Format yyyy-MMM-d hh:mm             		
      
Get-Date -Format 'yyyy-MMM-d hh:mm' > powershell.log	