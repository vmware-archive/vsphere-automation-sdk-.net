# VMware vSphere Automation SDK for .NET
## Table of Contents
- [Abstract](#abstract)
- [Supported vCenter Releases](#supported-vcenter-releases)
- [Table of Contents](#table-of-contents)
- [Quick Start Guide](#quick-start-guide)
  - [Setting up build environment](#setting-up-maven)
  - [Setting up a vSphere Test Environment](#setting-up-a-vsphere-test-environment)
  - [Building the Samples](#building-the-samples)
  - [Running the Samples](#running-the-samples)
  - [Importing the samples to eclipse](#importing-the-samples-to-eclipse)
- [API Documentation](#api-documentation)
- [Submitting samples](#submitting-samples)
  - [Required Information](#required-information)
  - [Suggested Information](#suggested-information)
  - [Contribution Process](#contribution-process)
  - [Code Style](#code-style)
- [Resource Maintenance](#resource-maintenance)
  - [Maintenance Ownership](#maintenance-ownership)
  - [Filing Issues](#filing-issues)
  - [Resolving Issues](#resolving-issues)
  - [VMware Sample Exchange](#vmware-sample-exchange)
- [Repository Administrator Resources](#repository-administrator-resources)
  - [Board Members](#board-members)
  - [Approval of Additions](#approval-of-additions)
- [VMware Resources](#vmware-resources)

## Abstract
This document describes the vSphere Automation .NET SDK samples that use the vSphere Automation
.NET client library. The samples have been developed to work with .NET Framework 4.5.

## Supported vCenter Releases:
vCenter 6.0 and 6.5. 
Certain APIs and samples that are introduced in 6.5 release, such as vCenter, Virtual Machine and Appliance Management. 

## Quick Start Guide
This document will walk you through getting up and running with the .NET SDK Samples. Prior to running the samples you will need to setup a vCenter test environment and install maven, the following steps will take you through this process.
Before you can run the SDK samples we'll need to walk you through the following steps:

1. Setting up maven
2. Setting up a vSphere test environment

### Setting up build environment
The SDK samples can be built either through command line or Visual Studio 
1. Ensure that .NET Framework 4.5 is installed on the system
2. Download nuget.exe from https://dist.nuget.org/index.html. (required only if building via command line)
3. Update PATH environment variable to include the .NET Framework directory and the directory where nuget.exe resides.

   SET PATH=%PATH%;"C:\Windows\Microsoft.NET\Framework64\v4.0.30319";"C:\Windows\Microsoft.NET\Framework\v4.0.30319";\<path to nuget.exe directory\>

### Setting up a vSphere Test Environment
**NOTE:** The samples are intended to be run against a freshly installed **non-Production** vSphere setup as the samples may make changes to the test environment and in some cases can destroy items when needed.

To run the samples a vSphere test environment is required with the following minimum configuration
* 1 vCenter Server
* 2 ESX hosts
* 1 NFS Datastore with at least 3GB of free capacity

Apart from the above, each individual sample may require additional setup. Please refer to the sample parameters for more information on that.

### Building the Samples
#### Using CLI
In the root directory of your folder after cloning the repository, run the below commands -

`nuget restore vSphere-Samples-6.5.0.sln`

`msbuild vSphere-Samples-6.5.0.sln /t:Build /p:Configuration=Release`

#### Using Visual Studio (2015 or higher)
1. Open the vSphere-Samples-6.5.0.sln file using Visual Studio
2. Right-click on the solution in Solution Explorer and select "Build Solution"

### Running the Samples
1. Navigate to the bin\ directory where the sample exe resides. 
2. Run the sample without any parameters to display usage information for the sample
````bash
D:\vsphere-automation-sdk-.net\vmware\samples\vcenter\vm\list\ListVMs\bin\Release>ListVMs.exe
ListVMs [options]

  --server                      Required. Hostname of vCenter Server
  --username                    Required. Username to login to vCenter Server
  --password                    Required. Password to login to vCenter Server
  --cleardata                   Specify this option to undo all persistent
                                results of running the sample
  --skip-server-verification    (Default: False) Optional: Specify this option
                                if you do not want to perform SSL certificate
                                verification.
                                NOTE: Circumventing SSL trust
                                in this  manner is unsafe and should not be
                                used with production code. This is ONLY FOR THE
                                PURPOSE OF DEVELOPMENT ENVIRONMENT.
  --help                        Display this help screen.
  ````

Use a command like the following to run a sample by specifying all the required parameters:
```` bash
D:\vsphere-automation-sdk-.net\vmware\samples\vcenter\vm\list\ListVMs\bin\Release>ListVMs.exe --server servername --username administrator@vsphere.local --password password --skip-server-verification
````

## API Documentation
The API documentation can be downloaded from [here](doc/client.zip).

Online version of the API documentation can be found [here](https://code.vmware.com/web/dp/doc/preview?id=4647).

## Submitting samples

### Developer Certificate of Origin

Before you start working with this project, please read our [Developer Certificate of Origin](https://cla.vmware.com/dco). All contributions to this repository must be signed as described on that page. Your signature certifies that you wrote the patch or have the right to pass it on as an open-source patch.

### Required Information
The following information must be included in the README.md for the sample.
* Author Name
  * This can include full name, email address or other identifiable piece of information that would allow interested parties to contact author with questions.
* Date
  * Date the sample was originally written
* Minimal/High Level Description
  * What does the sample do ?
* Any KNOWN limitations or dependencies

### Suggested Information
The following information should be included when possible. Inclusion of information provides valuable information to consumers of the resource.
* vSphere version against which the sample was developed/tested
* SDK version against which the sample was developed/tested
* Java version against which the sample was developed/tested

### Contribution Process

* Follow the [GitHub process](https://help.github.com/articles/fork-a-repo)
  * Please use one branch per sample or change-set
  * Please use one commit and pull request per sample
  * Please post the sample output along with the pull request
  * If you include a license with your sample, use the project license

### Code Style

Please conform to oracle java coding standards.
    http://www.oracle.com/technetwork/articles/javase/codeconvtoc-136057.html

## Resource Maintenance
### Maintenance Ownership
Ownership of any and all submitted samples are maintained by the submitter.
### Filing Issues
Any bugs or other issues should be filed within GitHub by way of the repository’s Issue Tracker.
### Resolving Issues
Any community member can resolve issues within the repository, however only the board member can approve the update. Once approved, assuming the resolution involves a pull request, only a board member will be able to merge and close the request.

### VMware Sample Exchange
It is highly recommended to add any and all submitted samples to the VMware Sample Exchange:  <https://code.vmware.com/samples>

Sample Exchange can be allowed to access your GitHub resources, by way of a linking process, where they can be indexed and searched by the community. There are VMware social media accounts which will advertise resources posted to the site and there's no additional accounts needed, as the VMware Sample Exchange uses MyVMware credentials.     

## Repository Administrator Resources
### Board Members

Board members are volunteers from the SDK community and VMware staff members, board members are not held responsible for any issues which may occur from running of samples from this repository.

Members:
* Vinod Pai (VMware)
* Steve Trefethen (VMware)

### Approval of Additions
Items added to the repository, including items from the Board members, require 2 votes from the board members before being added to the repository. The approving members will have ideally downloaded and tested the item. When two “Approved for Merge” comments are added from board members, the pull can then be committed to the repository.

## VMware Resources

* [vSphere Automation SDK Overview](http://pubs.vmware.com/vsphere-65/index.jsp#com.vmware.vapi.progguide.doc/GUID-AF73991C-FC1C-47DF-8362-184B6544CFDE.html)
* [VMware Code](https://code.vmware.com/home)
* [VMware Developer Community](https://communities.vmware.com/community/vmtn/developer)
* VMware vSphere [REST API Reference documentation](https://code.vmware.com/web/dp/doc/preview?id=4645).
* [VMware Java forum](https://code.vmware.com/forums/7508/vsphere-automation-sdk-for-java)
