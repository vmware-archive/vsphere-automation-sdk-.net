# VMware vSphere Automation SDK for .NET
## Table of Contents
- [Abstract](#abstract)
- [Supported vCenter Releases](#supported-vcenter-releases)
- [Table of Contents](#table-of-contents)
- [Quick Start Guide](#quick-start-guide)
  - [Setting up build environment](#setting-up-build-environment)
  - [Setting up a vSphere Test Environment](#setting-up-a-vsphere-test-environment)
  - [Building the Samples](#building-the-samples)
  - [Running the Samples](#running-the-samples)
  - [Adding a new sample using a template](#adding-a-new-sample-using-a-template)
- [API Documentation](#vSphere API Documentation)
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

## Supported OnPrem vCenter Releases:
vCenter 6.0, 6.5, 6.7 and 6.7 U1.

Please refer to the notes in each sample for detailed compatibility information.

## Quick Start Guide
This document will walk you through getting up and running with the .NET SDK Samples. Prior to running the samples you will need to setup a vCenter test environment and install NuGet CLI. The following steps will take you through this process.

1. Setting up build environment
2. Setting up a vSphere test environment

### Setting up build environment
To build and run the samples, install Microsoft Visual Studio 2015 or higher. 

### Setting up a vSphere Test Environment
**NOTE:** The samples are intended to be run against a freshly installed **non-Production** vSphere setup as the samples may make changes to the test environment and in some cases can destroy items when needed.

To run the samples a vSphere test environment is required with the following minimum configuration
* 1 vCenter Server
* 2 ESX hosts
* 1 NFS Datastore with at least 3GB of free capacity

Apart from the above, each individual sample may require additional setup. Please refer to the sample parameters for more information on that. 

### Building the Samples
* Open the vSphere-Samples.sln file using Visual Studio 2015 or higher
* Right-click on the solution in Solution Explorer and select "Build Solution"

![Build Solution](screenshots/build-solution.jpg?raw=true)

### Running the Samples
* Right click the sample you want to run and select "Set as Startup Project".

![Usage Info](screenshots/set-startup-project.jpg?raw=true)

* To run the sample press "Ctrl + F5" or click "Start without Debugging" from the "Debug" menu.

![Usage Info](screenshots/run-sample.jpg?raw=true)

* If no parameters are specified, the sample usage info is displayed on the console.

![Usage Info](screenshots/usage-info.jpg?raw=true)

* To specify all the required parameters for running the sample, right click on the sample project select "Properties". Navigate to the "Debug" tab. Under the "Start Options" section, specify the parameters required by the sample as mentioned in the sample usage.

![Sample Parameters](screenshots/sample-parameters.jpg?raw=true)

### Adding a new sample using a template

* To import the VMware Project Template, copy the zip file located in "vsphere-automation-sdk-.net\Project Template" to "C:\Users\<username>\Documents\Visual Studio 2017\Templates\ProjectTemplates\Visual C#". 

1. Open Visual Studio
2.  In the menu select File --> New --> Project...
3. Select the template as the image below shows
![New Project](screenshots/newProject.jpg?raw=true)

### vSphere API Documentation

* Current release:       [7.0.0](https://vmware.github.io/vsphere-automation-sdk-.net/vsphere/7.0.0/vapi-client-bindings/index.html).
* Previous releases:    [6.7.1](https://vmware.github.io/vsphere-automation-sdk-.net/vsphere/6.7.1/vapi-client-bindings/index.html).

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
* .NET Framework version against which the sample was developed/tested

### Contribution Process

* Follow the [GitHub process](https://help.github.com/articles/fork-a-repo)
  * Please use one branch per sample or change-set
  * Please use one commit and pull request per sample
  * Please post the sample output along with the pull request
  * If you include a license with your sample, use the project license

### Code Style

Please conform to microsoft design guidelines.
    https://msdn.microsoft.com/en-us/library/ms229042.aspx
    
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
* [vSphere Automation SDK for .NET forum](https://code.vmware.com/forums/7504/vsphere-automation-sdk-for-.net)
