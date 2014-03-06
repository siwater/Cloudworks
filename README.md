=======
Cloudworks
==========

On-guest processing support for CloudStack Windows instances. Contains:

1. A Windows Service to process <script> and <powershell> fragments passed in user-data to the instance
  (see http://docs.aws.amazon.com/AWSEC2/latest/WindowsGuide/UsingConfig_WinAMI.html for AWS equivalent)

2. A port of the AWS "cfn bootstrap" tools to work in the CloudStack enviroment
  (see http://aws.amazon.com/developertools/4026240853893296)

To use it, pull down the source (requires Visual Studio 2012) and open the Cloudworks.sln solution.

The Citrix.Cloudworks.Agent.Setup project will build an MSI that installs both components.
