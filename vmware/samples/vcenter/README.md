This directory contains samples for managing vSphere infrastructure and virtual machines:

### Virtual machine Create/List/Delete operations
Sample                | Description
----------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
ListVMs.cs            | Demonstrates how to get list of VMs present in vCenter.
CreateDefaultVM.cs    | Demonstrates how to create a VM with system provided defaults
CreateBasicVM.cs      | Demonstrates how to create a basic VM with following configuration - 2 disks, 1 nic
CreateExhaustiveVM.cs | Demonstrates how to create a exhaustive VM with the following configuration - 3 disks, 2 nics, 2 vcpu, 2 GB, memory, boot=BIOS, 1 cdrom, 1 serial port, 1 parallel port, 1 floppy, boot_device=[CDROM, DISK, ETHERNET])

### Virtual machine hardware configuration
Sample                      | Description
----------------------------|----------------------------------------------------------------------------------------------------------
SataAdapterConfiguration.cs | Demonstrates how to configure virtual SATA adapters of a virtual machine.
ScsiAdapterConfiguration.cs | Demonstrates how to configure virtual SCSI adapters of a virtual machine.
BootConfiguration.cs        | Demonstrates how to configure the settings used when booting a virtual machine.
BootDeviceConfiguration.cs  | Demonstrates how to modify the boot devices used by a virtual machine, and in what order they are tried.
CdromConfiguration.cs       | Demonstrates how to configure a CD-ROM device for a VM.
CpuConfiguration.cs         | Demonstrates how to configure a CPU for a VM.
EthernetConfiguration.cs    | Demonstrates how to configure virtual ethernet adapters of a virtual machine.
MemoryConfiguration.cs      | Demonstrates how to configure the memory related settings of a virtual machine.


### Testbed Requirement:
    - 1 vCenter Server
    - 2 ESX hosts
    - 1 datastore
    - Some samples need additional configuration like a cluster, vm folder, standard portgroup, iso file on a datastore and distributed portgroup