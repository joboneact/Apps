# CLI Reference: Microsoft 365, Power Platform, and PnP PowerShell

This workspace used three command-line tools. They overlap slightly, but each has a different strength.

| Tool | Primary use in this work | Current status |
| --- | --- | --- |
| CLI for Microsoft 365 (`m365`) | Microsoft 365, SharePoint, Entra, Outlook, Teams, and Power Automate administration | Used transiently through `npx`; version `11.11.0` |
| Power Platform CLI (`pac`) | Dataverse environments, solutions, Canvas App files, and Power Platform connections | Installed; version `2.8.1` |
| PnP.PowerShell | SharePoint administration through PowerShell cmdlets | Installed, but requires PowerShell 7.4+; PowerShell `7.6.5` is available |

## CLI for Microsoft 365

### What it is

[CLI for Microsoft 365](https://pnp.github.io/cli-microsoft365/) is an open-source cross-platform command-line tool maintained by the Microsoft 365 developer community. It is designed for scripting and automating Microsoft 365 workloads using a consistent command structure.

It uses the command pattern:

```text
m365 <workload> <resource> <operation> [options]
```

Examples:

```powershell
m365 status
m365 spo list add --title "Example" --webUrl "https://contoso.sharepoint.com/sites/demo"
m365 flow list --environmentName "Default-..."
```

### Where to get it

- Documentation: [pnp.github.io/cli-microsoft365](https://pnp.github.io/cli-microsoft365/)
- npm package: [`@pnp/cli-microsoft365`](https://www.npmjs.com/package/@pnp/cli-microsoft365)
- GitHub source and releases: [pnp/cli-microsoft365](https://github.com/pnp/cli-microsoft365)

For a one-off use without a global install:

```powershell
npx --yes --package @pnp/cli-microsoft365@11.11.0 m365 version
```

For a global installation:

```powershell
npm install -g @pnp/cli-microsoft365
m365 version
```

Node.js and npm are required. Download Node.js from [nodejs.org](https://nodejs.org/).

### First-use setup and help

```powershell
m365 setup
m365 help
m365 <command> --help
m365 version
```

Use the built-in help before running a write operation. It documents required permissions, parameters, examples, and output options for the exact installed version.

### Authentication

Use device-code authentication for interactive local work:

```powershell
m365 login --authType deviceCode --appId <APPLICATION-CLIENT-ID>
m365 status
m365 logout
```

`<APPLICATION-CLIENT-ID>` is the **Application (client) ID** of an Entra app registration configured as a public client. It identifies the CLI to Entra ID. The device-code browser prompt signs in as the user and obtains delegated permissions; it does not use a client secret.

Other sign-in methods may be available for automation, but use the documented method appropriate to the security model and do not put passwords, access tokens, or client secrets in scripts or workspace files.

### Main operation groups

The installed `m365` v11.11.0 lists the following top-level areas. This is a workload-level list, not every individual command.

| Group | Operations it covers |
| --- | --- |
| `app` | Microsoft 365 application administration |
| `booking` | Microsoft Bookings |
| `cli` | CLI configuration, telemetry, and updates |
| `connection` | Connection configuration |
| `context` | Saved command contexts |
| `entra` | App registrations, service principals, groups, users, permissions, and other Entra resources |
| `exo` | Exchange Online operations |
| `external` | External items and connections |
| `file` | Microsoft 365 file operations |
| `flow` | List, get, export, enable, disable, run, and remove existing Power Automate flows |
| `graph` | Microsoft Graph queries and operations |
| `onedrive` | OneDrive files and configuration |
| `onenote` | OneNote notebooks and pages |
| `outlook` | Mail, calendar, contacts, and mailbox operations |
| `pa` | Power Automate administration and connectors |
| `planner` | Plans, buckets, tasks, and labels |
| `pp` | Power Platform environments, apps, connections, and policies |
| `purview` | Microsoft Purview resources |
| `search` | Microsoft Search queries and configuration |
| `spe` | SharePoint Embedded |
| `spfx` | SharePoint Framework project helpers |
| `spo` | SharePoint Online sites, lists, fields, files, pages, permissions, and settings |
| `spp` | SharePoint Premium operations |
| `teams` | Teams apps, teams, channels, tabs, and messages |
| `tenant` | Tenant-wide Microsoft 365 settings |
| `todo` | Microsoft To Do lists and tasks |
| `util` | CLI utility operations |
| `viva` | Viva Connections, Learning, and related resources |

General operations are also available:

| Command | Purpose |
| --- | --- |
| `m365 docs` | Open the documentation page for a command |
| `m365 login` / `logout` / `status` | Manage interactive authentication |
| `m365 request` | Send a supported authenticated web request |
| `m365 search` | Search Microsoft 365 data |
| `m365 setup` | Configure CLI preferences |
| `m365 version` | Show installed version |

### SharePoint list operations for this task

Use the `spo` group. Check exact syntax before modifying a site:

```powershell
m365 spo list add --help
m365 spo list get --help
m365 spo field add --help
m365 spo listitem add --help
```

The key operation used for the planned list is:

```powershell
m365 spo list add `
  --title "MyAddressListSept2026DG" `
  --baseTemplate GenericList `
  --webUrl "https://bartco1.sharepoint.com/sites/bartco1"
```

Prefer `--output json` for scripts and `--output text` for an interactive terminal. Do not parse the spacing of a text table in automation.

### Power Automate flow operations

The installed `flow` group supports existing flows, including:

```text
flow disable
flow enable
flow export
flow get
flow list
flow remove
flow run
```

It does not expose `flow create`. For `HelloWorkflowSept2026DG`, create the new flow in [Power Automate](https://make.powerautomate.com/) first. Once the flow exists, use `m365 flow list`, `m365 flow get`, `m365 flow export`, `m365 flow enable`, and `m365 flow disable` to inspect, back up, and manage it.

## Power Platform CLI (`pac`)

### What it is and where to get it

[Power Platform CLI](https://learn.microsoft.com/power-platform/developer/cli/introduction) is Microsoft’s official CLI for Power Platform development and administration. It works with Dataverse environments, solutions, Canvas Apps, connectors, Power Pages, Power Apps component framework controls, and more.

Installation and command reference:

- [Install Power Platform CLI](https://learn.microsoft.com/power-platform/developer/cli/introduction#install-power-platform-cli)
- [Power Platform CLI command reference](https://learn.microsoft.com/power-platform/developer/cli/reference)

Basic help:

```powershell
pac help
pac <group> help
pac <group> <command> help
```

### Operations used in this workspace

| Command group | Operations used or relevant |
| --- | --- |
| `pac auth` | Create, list, and select Dataverse authentication profiles |
| `pac env` | Inspect, list, select, and query Dataverse environments |
| `pac solution` | Initialize, pack, import, export, list, and manage Dataverse solutions |
| `pac canvas` | List and download Canvas Apps; unpack and pack `.msapp` source files |
| `pac connection` | List and manage Power Platform connector connections |

Examples from this workspace:

```powershell
pac auth list
pac env who
pac solution list
pac canvas list
pac connection list
```

Important Canvas App limitation: the installed PAC CLI can download, unpack, and pack `.msapp` files, but it cannot upload or overwrite a Canvas App in the environment. Use Power Apps Studio to import, save, and publish a packed app.

## PnP.PowerShell

### What it is and where to get it

[PnP.PowerShell](https://pnp.github.io/powershell/) is a PowerShell module for SharePoint, Microsoft Teams, Microsoft 365 Groups, Planner, and related Microsoft 365 services.

- Installation: [Installing PnP PowerShell](https://pnp.github.io/powershell/articles/installation.html)
- Cmdlet reference: [PnP PowerShell cmdlets](https://pnp.github.io/powershell/cmdlets/)

The current version requires PowerShell `7.4` or later. This computer has PowerShell `7.6.5`; start it with:

```powershell
& 'C:\Program Files\PowerShell\7\pwsh.exe'
```

Then install or update PnP.PowerShell in that shell, subject to your organization’s package policy:

```powershell
Install-Module PnP.PowerShell -Scope CurrentUser
Import-Module PnP.PowerShell
```

### Relevant operations

| Cmdlet | Purpose |
| --- | --- |
| `Connect-PnPOnline` | Authenticate to a SharePoint site |
| `Get-PnPWeb` | Verify the active site connection |
| `New-PnPList` | Create a SharePoint list |
| `Add-PnPField` | Add a list or site column |
| `Add-PnPListItem` | Create a list item |
| `Get-PnPList` | Retrieve list details |
| `Get-PnPListItem` | Retrieve list items |

PnP.PowerShell is a good alternative when a PowerShell-native script is preferred. In this session, its interactive device-login prompt did not work through the terminal host, so CLI for Microsoft 365 is the more practical path after an Entra app ID is supplied.

## Which tool should I choose?

| Task | Preferred tool |
| --- | --- |
| Create SharePoint lists, columns, and fictional list data | CLI for Microsoft 365 or PnP.PowerShell |
| Create a new Power Automate cloud flow | Power Automate web designer |
| List, export, enable, or disable an existing flow | CLI for Microsoft 365 |
| Create/import/export a Dataverse solution | Power Platform CLI |
| Download/unpack/edit/pack a Canvas App file | Power Platform CLI |
| Import and publish a Canvas App | Power Apps Studio |

For all tools, start with the narrowest help command, authenticate with least privilege, and use JSON output when a script needs to read command results.