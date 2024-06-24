<powershell>
# Wait added to give Windows extra time to finish bootup
start-sleep -s 10
# Variables
[string]$token = Invoke-RestMethod -Headers @{"X-aws-ec2-metadata-token-ttl-seconds" = "21600"} -Method PUT -Uri http://169.254.169.254/latest/api/token
$awsRegion = (Invoke-RestMethod -Headers @{"X-aws-ec2-metadata-token" = $token} -Method GET -Uri http://169.254.169.254/latest/dynamic/instance-identity/document).region
$InstanceId = Invoke-RestMethod -Headers @{"X-aws-ec2-metadata-token" = $token} -Method GET -Uri http://169.254.169.254/latest/meta-data/instance-id
 
$directoryId = "${directory_id}"
$windowsVersion = "${windows_version}"
 
$pamready = "True"
 
$Path= 'HKLM:\Software\UserData'
 
# Start SSM Agent on every boot
Set-Service -Name AmazonSSMAgent -StartupType Automatic
Start-Service -Name AmazonSSMAgent
Get-Service -Name AmazonSSMAgent
 
if (! (Get-Item $Path -ErrorAction SilentlyContinue)) {
  New-Item $Path
  New-ItemProperty -Path $Path -Name RunCount -Value 0 -PropertyType dword
}
 
$runCount = Get-ItemProperty -Path $Path -Name Runcount -ErrorAction SilentlyContinue | Select-Object -ExpandProperty RunCount
 
# Start Build Loop
if ($runCount -ge 0) {
  switch($runCount) {
    0 {
      # Increment the RunCount
      Set-ItemProperty -Path $Path -Name RunCount -Value 1
 
      # Rename Server and Restart
      Set-ExecutionPolicy Unrestricted -Scope LocalMachine -force
      $hostname = "AWS"+$(hostname).Substring($(hostname).IndexOf('-') + 1)
 
      # Add Hostname Tag
      aws ec2 create-tags --resources $InstanceId --tags Key="Hostname",Value=$hostname
 
      # Add FQDN Tag
      $fqdn = "$hostname.deluxe.com".ToLower()
      aws ec2 create-tags --resources $InstanceId --tags Key="FQDN",Value=$fqdn
 
      # Rename
      Rename-Computer -NewName $hostname -Restart
    }
    1 {
      # Increment the RunCount
      Set-ItemProperty -Path $Path -Name RunCount -Value 2
      #Wait-SSMRegister
 
      # Join the correct Domain OU through SSM using a TF variable directoryId
      $DomainJoin=Send-SSMCommand `
      -DocumentName "AWS-JoinDirectoryServiceDomain" `
      -Parameters @{ directoryOU="OU=AWS,OU=$windowsVersion,OU=Servers,DC=deluxe,DC=com";directoryId=$directoryId;directoryName='deluxe.com'} `
      -InstanceId $InstanceId `
      -Region $awsRegion `
      -TimeoutSecond 2000
      Write-Output $DomainJoin
 
    }
    2 {
      # Increment the RunCount
      Set-ItemProperty -Path $Path -Name RunCount -Value 3
 
      # Run pre-installed post-build script
      C:\ServerBuild\Install-DlxWindowsCustomizationAWS.ps1 -DeployDomain 'deluxe.com'
 
      # Clean-up
      Remove-Item 'C:\ServerBuild' -Recurse -Force
      Restart-Computer
    }
    3 {
      # Increment the RunCount
      Set-ItemProperty -Path $Path -Name RunCount -Value 4
 
      # Remove HTTPS listener
      Get-ChildItem wsman:\localhost\Listener\ | Where-Object -Property Keys -like 'Transport=HTTPS' | Remove-Item -Recurse
 
      # Remove all self-signed certs
      Get-ChildItem Cert:\LocalMachine\My | Remove-Item
 
      $fqdn = [System.Net.Dns]::GetHostByName($env:computerName)
 
      # Generate self-signed cert
      $srvCert = New-SelfSignedCertificate -DnsName $fqdn.HostName -CertStoreLocation Cert:\LocalMachine\My -NotAfter (Get-Date).AddMonths(120)
 
      # Add WinRM HTTPS
      New-Item -Path WSMan:\localhost\Listener\ -Transport HTTPS -Address * -CertificateThumbPrint $srvCert.Thumbprint -HostName $fqdn.HostName -Force
 
      [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12; Invoke-WebRequest -Uri 'https://esn28329.live.dynatrace.com/api/v1/deployment/installer/agent/windows/default/latest?arch=x86&flavor=default' -Headers @{ 'Authorization' = "$dynatraceApiToken" } -OutFile $dynatraceOneagentFile
      Start-Process "$dynatraceOneagentFile" -argumentlist "--set-infra-only=false --set-app-log-content-access=true --set-host-group=$dynatraceHostGrp /quiet /qn" -wait
 
      # Clean up
      Remove-Item "$dynatraceOneagentFile" -Recurse -Force
 
      Restart-Computer
    }
    5 {
      # Increment the RunCount
      Set-ItemProperty -Path $Path -Name RunCount -Value 6
 
      # Add PAMReady Tag
      aws ec2 create-tags --resources $InstanceId --tags Key="PAMReady",Value=$pamready
    }
  }
}
 
# Windows licencing activation
slmgr.vbs /skms kms.deluxe.com:1688
slmgr.vbs /ato
 
</powershell>
<persist>true</persist>