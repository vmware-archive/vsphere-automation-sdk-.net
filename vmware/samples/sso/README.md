This directory contains samples for SSO and Lookup Service APIs:

The vSphere Automation SDK for .NET samples use the vCenter Lookup Service to obtain the URLs for other vSphere Automation services (SSO, vAPI, VIM, SPBM, etc.).
The SDK contains the Lookup Service WSDL files. The Lookup Service WSDL files are located in wsdl/ directory.

Sample                    | Description
--------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------
EmbeddedPscSsoWorkflow.cs | Demonstrates how to create a SSO connection using a SAML Bearer token when we have a vCenter Server and embedded Platform Services Controller
ExternalPscSsoWorkflow.cs | Demonstrates how to create a SSO connection using a SAML Bearer token when we have a vCenter Server and external Platform Services Controller

### Testbed Requirement:
    - 1 vCenter Server
    - 1 Platform Services Controller