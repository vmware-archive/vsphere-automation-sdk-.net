# VMware Project Template in Visual Studio for .NET Automation SDK
## Table of Contents
- [Abstract](#abstract)
- [Supported vCenter Releases](#supported-vcenter-releases)
- [Table of Contents](#table-of-contents)
- [Quick Start Guide](#quick-start-guide)
  - [Setting up build environment](#setting-up-build-environment)
  - [Setting up a vSphere Test Environment](#setting-up-a-vsphere-test-environment)
  - [Building the Samples](#building-the-samples)

## Abstract
This document describes how to import a VMWare Automation Project Template to Visual Studio 
which has been developed to work with .NET Framework 4.5.
With this template you will be able to have a basic structure to add your custom code and interact with the vAPI.

## Supported vCenter Releases:
vCenter 6.0 and 6.5 and vCenter 6.6.1 for VMware Cloud 1.1.

Please refer to the notes in each sample for detailed compatibility information.

## Quick Start Guide
This document will walk you through getting up and running with the import a VMware Project Template in Visual Studio for .NET Automation SDK.

### Setting up build environment
To import the VMware Project Template, install Microsoft Visual Studio 2015 or higher. 

* Copy the zip file (VMWare-SDK-Automation-Sample-Template.zip) into the the folder C:\Users\<username>\Documents\Visual Studio 2015\Templates\ProjectTemplates

### Setting up a Project Template Test Environment

To run the Project Template, a vSphere test environment is required with the following minimum configuration
* 1 vCenter Server

### Building and running the Project Template 

1. Open Visual Studio
2.  In the menu select File --> New --> Project...
3. Select the template as the image below shows
![Build Solution](screenshots/newProject.jpg?raw=true)
4. Then right  click on the console project on the solution explorer and on the debug options add the "Command line arguments"
                   --server serverIP --username username --password password --skip-server-verification --cleardata

![Build Solution](screenshots/solExplorer.jpg?raw=true)

5. Save
6. Press F5 to debug, or click on Start in Visual Studio
7. Then it should show the output like the image below

![Build Solution](screenshots/output.jpg?raw=true)
