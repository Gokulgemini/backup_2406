<powershell>
# Wait added to give Windows extra time to finish bootup
start-sleep -s 10

# Variables
[string]$token = Invoke-RestMethod -Headers @{"X-aws-ec2-metadata-token-ttl-seconds" = "21600"} -Method PUT -Uri http://169.254.169.254/latest/api/token
$awsRegion = (Invoke-RestMethod -Headers @{"X-aws-ec2-metadata-token" = $token} -Method GET -Uri http://169.254.169.254/latest/dynamic/instance-identity/document).region
$InstanceId = Invoke-RestMethod -Headers @{"X-aws-ec2-metadata-token" = $token} -Method GET -Uri http://169.254.169.254/latest/meta-data/instance-id
$Path= 'HKLM:\Software\UserData'
$directoryId = "${directory_id}"
$windowsVersion = "${windows_version}"

# Start SSM Agent on every boot
Set-Service -Name AmazonSSMAgent -StartupType Automatic
Start-Service -Name AmazonSSMAgent
Get-Service -Name AmazonSSMAgent

#Function to store log data
function Write-Log {
  [CmdletBinding()]
  param(
      [Parameter()]
      [ValidateNotNullOrEmpty()]
      [string]$Message
  )
  [pscustomobject]@{
      Time = (Get-Date -f g)
      Message = $Message
  } | Export-Csv -Path "c:\UserDataLog\UserDataLogFile.log" -Append -NoTypeInformation
}
#----------------------------------
#Create log file location
if (-not(Test-Path "C:\UserDataLog")) {
  New-Item -ItemType directory -Path "C:\UserDataLog"
  Write-Log -Message "Created folder to store log file."
} else {
  Write-Log -Message "Userdata Log Dir already exists."
}
#----------------------------------
#Userdata location

if (! (Get-Item $Path -ErrorAction SilentlyContinue)) {
  New-Item $Path
  New-ItemProperty -Path $Path -Name RunCount -Value 0 -PropertyType dword
}

$runCount = Get-ItemProperty -Path $Path -Name Runcount -ErrorAction SilentlyContinue | Select-Object -ExpandProperty RunCount

# Start Build Loop
if ($runCount -ge 0) {
  switch($runCount) {
    0 {
      Write-Log -Message "-------------------------"
      Write-Log -Message "Userdata script is stored at : $PSScriptRoot"
      Write-Log -Message "Execution of step $runcount Beginning now..."

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
      Write-Log -Message "-------------------------"
      Write-Log -Message "Userdata script is stored at : $PSScriptRoot"
      Write-Log -Message "Execution of step $runcount Beginning now..."

      # Increment the RunCount
      Set-ItemProperty -Path $Path -Name RunCount -Value 2
      #Wait-SSMRegister

      # Join the correct Domain OU through SSM using a TF variable directoryId
      Write-Log -Message "Attempting domain join."
      $DomainJoin=Send-SSMCommand `
      -DocumentName "AWS-JoinDirectoryServiceDomain" `
      -Parameters @{ directoryOU="OU=AWS,OU=$windowsVersion,OU=Servers,DC=deluxe,DC=com";directoryId=$directoryId;directoryName='deluxe.com'} `
      -InstanceId $InstanceId `
      -Region $awsRegion `
      -TimeoutSecond 2000
      Write-Output $DomainJoin

    }
    2 {
      Write-Log -Message "-------------------------"
      Write-Log -Message "Userdata script is stored at : $PSScriptRoot"
      Write-Log -Message "Execution of step $runcount Beginning now..."

      # Increment the RunCount
      Set-ItemProperty -Path $Path -Name RunCount -Value 3

      # Run pre-installed post-build script
      Write-Log -Message "Executing GCO server-build script."
      C:\ServerBuild\Install-DlxWindowsCustomizationAWS.ps1 -DeployDomain 'deluxe.com'

      # Clean-up
      Remove-Item 'C:\ServerBuild' -Recurse -Force
      Write-Log -Message "GCO server-build script Finished."
      Restart-Computer
    }
    3 {
      Write-Log -Message "-------------------------"
      Write-Log -Message "Userdata script is stored at : $PSScriptRoot"
      Write-Log -Message "Execution of step $runcount Beginning now..."

      # Checking Domain Join
      $checkDomain = Get-CimInstance Win32_ComputerSystem
      $domainJoined = $checkDomain.Domain
      if ($domainJoined -eq "deluxe.com") {
        Write-Log -Message "$dnsRecord has successfully joined $domainJoined."
      }
      else {
        Write-Log -Message "Error Joining deluxe.com domain: $dnsRecord has joined $domainJoined."
      }

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

      # Install Dynatrace agent
      $dynatraceHostGrp = "bpo_app_uat"
      $dynatraceOneagentFile = ".\Dynatrace-OneAgent-Windows-latest.exe"

      Write-Log -Message "Downloading Dynatrace OneAgent."
      [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12; Invoke-WebRequest -Uri 'https://artifacts.deluxe.com/artifactory/MonitoringTeam/Dynatrace-OneAgent-Windows-1.275.146.20231002-095820.exe' -OutFile $dynatraceOneagentFile
      Write-Log -Message "Installing Dynatrace OneAgent."
      Start-Process "$dynatraceOneagentFile" -argumentlist "--set-infra-only=false --set-app-log-content-access=true --set-host-group=$dynatraceHostGrp /quiet /qn" -wait
      Write-Log -Message "Installed Dynatrace OneAgent and restarting the instance."

      # Clean up
      Remove-Item "$dynatraceOneagentFile" -Recurse -Force

      Restart-Computer
    }
    4 {
      Write-Log -Message "-------------------------"
      Write-Log -Message "Userdata script is stored at : $PSScriptRoot"
      Write-Log -Message "Execution of step $runcount Beginning now..."

      # Increment the RunCount
      Set-ItemProperty -Path $Path -Name RunCount -Value 5
      
      #Install IIS
      Write-Log -Message "Installing IIS."
      Install-WindowsFeature -name Web-Server -IncludeManagementTools
      Write-Log -Message "IIS Installed."

      # Download and install .NET 8.0
      Write-Log -Message "Installing .NET 8.0."
      Invoke-WebRequest -Uri https://dot.net/v1/dotnet-install.ps1 -OutFile dotnet-install.ps1
      dotnet-install.ps1 -Channel 8.0
      Write-Log -Message ".NET 8.0 Installed."

      # Clean up
      Remove-Item -Path dotnet-install.ps1

      Restart-Computer
    }
    5 {
      # Increment the RunCount
      Set-ItemProperty -Path $Path -Name RunCount -Value 6
      $CheckDynatraceInstallStatus = Get-WmiObject -Class win32_Product | Where-Object {$_.Name -eq "Dynatrace OneAgent"}
      if ($CheckDynatraceInstallStatus.Name -eq "Dynatrace OneAgent") {
        Write-Log -Message "Dynatrace OneAgent was installed successfully."
      } else {
        Write-Log -Message "Dynatrace OneAgent was not installed successfully."
      }

      $InstanceId = (Invoke-WebRequest -URI http://169.254.169.254/latest/meta-data/instance-id -UseBasicParsing).Content
      Write-Log -Message "Creating PAM ready tag."
      # Add PAMReady Tag
      $pamready = "True"
      aws ec2 create-tags --resources $InstanceId --tags Key="PAMReady",Value=$pamready
    }
  }
}

# Windows licencing activation
slmgr.vbs /skms kms.deluxe.com:1688
slmgr.vbs /ato

</powershell>
<persist>true</persist>