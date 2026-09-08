Created and published the unmanaged Power Platform solution in the authenticated `bartco1 (default)` environment.

- Solution unique/display name: `HelloWorldDGSolution`
- Publisher: `HelloWorldDG`
- Prefix: `hwdg`
- Local solution project: [PacCLI-TestSept2026/HelloWorldDGSolution](PacCLI-TestSept2026/HelloWorldDGSolution)
- Import archive: [PacCLI-TestSept2026/HelloWorldDGSolution/HelloWorldDGSolution.zip](PacCLI-TestSept2026/HelloWorldDGSolution/HelloWorldDGSolution.zip)

Validation confirms it exists online as version `1.0`, unmanaged.

The installed PAC CLI cannot create a blank Canvas App with controls; `pac canvas create` only generates an app from a custom connector. Create **Hello World DG** in Power Apps Studio within `HelloWorldDGSolution`, then add one screen with:
- Two tab controls or buttons that toggle a selected-tab variable and show/hide their content.
- A bottom-aligned status bar, for example a label with `Text = "Ready"` and `Y = Parent.Height - Self.Height`.

The solution is ready for that app to be created and added in Studio.

---

Create sample address list called MyAddressListSept2026DG at 
https://bartco1.sharepoint.com/sites/bartco1
then populate with realistic random data.
Include time zones, postions, email address, display names.

workflow
create a workflow HelloWorkflowSept2026DG that uses above address list and triggers when a new address is added.


https://bartco1.sharepoint.com/sites/bartco1
then send email to bartco1@bartco1.onmicrosoft.com formatted with key address fields using HTML styling.

I’ll first check the locally available Power Platform and SharePoint automation tools, plus the authenticated PAC profile. The key test is whether this environment can create SharePoint lists and Power Automate cloud flows directly; if it can, I’ll build the list, seed it, and create the email workflow.

```text
Index Active Kind      Name         User                            Cloud  Type Environment       Environment Url
[1]   *      UNIVERSAL HelloWorldDG bartco1@bartco1.onmicrosoft.com Public User bartco1 (default) https://org494e45e7.crm.dynamics.com/
```

```text
Microsoft PowerPlatform CLI
Version: 2.8.1+ga4eb71c (.NET Framework 4.8.9337.0)
```

The PAC profile is healthy, but PAC CLI 2.8.1 does not expose a SharePoint-list or cloud-flow command group, and no SharePoint/Graph cmdlets are installed. I’ll check whether the Power Apps management modules are already available and, if not, install the supported PnP.PowerShell module needed to create and populate the SharePoint list.

```text
Name                                   Version          Path
----                                   -------          ----
Microsoft.Online.SharePoint.PowerShell 16.0.26017.12000 C:\Program Files\WindowsPowerShell\Modules\...
PnpDevice                              1.0.0.0          C:\WINDOWS\system32\...
Microsoft.Online.SharePoint.PowerShell 16.0.27515.0     C:\Program Files\SharePoint Online Management Shell\...
```

```text
Name           Version Description
----           ------- -----------
PnP.PowerShell 3.4.1   Microsoft 365 Patterns and Practices PowerShell Cmdlets
```

Only the SharePoint Online admin module is installed, which cannot create site lists or items. PnP.PowerShell 3.4.1 is available from the PowerShell Gallery and is the supported module for the requested SharePoint operations.

```text
Import-Module : The version of Windows PowerShell on this computer is
'5.1.26100.9168'. The module '...\PnP.PowerShell.psd1' requires a minimum
Windows PowerShell version of '7.4.0' to run.
```

PowerShell 7.6.5 is installed, so the PnP module can run there. PnP is usable under PowerShell 7; the list-creation command is `New-PnPList` rather than `Add-PnPList`.

```text
Name        Source
----        ------
New-PnPList PnP.PowerShell
```

The PnP device-login cmdlet cannot emit its device prompt through this terminal host, so it never established a session:

```text
WARNING: The WriteObject and WriteError methods cannot be called from outside the overrides of the BeginProcessing, ProcessRecord, and EndProcessing methods, and they can only be called from within the same thread.
Connect-PnPOnline: The WriteObject and WriteError methods cannot be called from outside the overrides of the BeginProcessing, ProcessRecord, and EndProcessing methods, and they can only be called from within the same thread.
Get-PnPWeb: You are not signed in. Please use Connect-PnPOnline to connect.
```

I’ll switch to Microsoft Graph PowerShell, whose device-code flow is more reliable here, and use its SharePoint list APIs; the workflow creation will still require a Power Automate-specific API or an importable flow package.

```text
Install-Package: Administrator rights are required to install or update. Log on to the computer with an account that has Administrator rights, and then try again, or install by adding "-Scope CurrentUser" to the command.
```

The PowerShell package providers could not install the Microsoft Graph modules because they could not create or extract to the per-user module paths. Alternate tools were checked:

```text
Name     Source                           Version
----     ------                           -------
node.exe C:\Program Files\nodejs\node.exe 24.19.0.0
npm.ps1  C:\Program Files\nodejs\npm.ps1
```

The Power Platform connection inventory confirms usable connections already exist in the target environment:

```text
shared-sharepointonl-a6b215fc-5d67-498e-8170-911e-023b0d5e bartco1@bartco1.onmicrosoft.com /providers/Microsoft.PowerApps/apis/shared_sharepointonline Connected
shared-office365-445ee297-f6ac-4185-a853-a379fae6c1b4      bartco1@bartco1.onmicrosoft.com /providers/Microsoft.PowerApps/apis/shared_office365       Connected
shared-sendmail-1e3150b3-581c-4f57-9515-2f0b0566f612       Mail                            /providers/Microsoft.PowerApps/apis/shared_sendmail        Connected
```

The Microsoft 365 CLI was then installed transiently with Node. It supports SharePoint list creation:

```text
SPO LIST ADD
============

Creates list in the specified site

USAGE

	m365 spo list add [options]

OPTIONS

	-t, --title <title>
		Title of the list to add.

	--baseTemplate [baseTemplate]
		The list definition type on which the list is based. Allowed values Announcements, Contacts, CustomGrid, DataSources,DiscussionBoard, DocumentLibrary, Events, GanttTasks, GenericList, IssuesTracking, Links, NoCodeWorkflows,PictureLibrary, Survey, Tasks, WebPageLibrary, WorkflowHistory, WorkflowProcess, XmlForm. Default value is GenericList.

	-u, --webUrl <webUrl>
		URL of the site where the list should be added.
```

The Microsoft 365 CLI can create the SharePoint list but only lists, exports, enables, disables, or removes existing Power Automate flows. It has no `flow create` command:

```text
Commands:

	flow disable [options]  Disables specified Microsoft Flow
	flow enable [options]   Enables specified Microsoft Flow
	flow export [options]   Exports the specified Microsoft Flow as a file
	flow get [options]      Gets information about the specified Microsoft Flow
	flow list [options]     Lists Power Automate flows in the given environment
	flow remove [options]   Removes the specified Microsoft Flow
```

Microsoft 365 CLI authentication was started:

```text
PS C:\Proj\M365\Power Platform\Vibe> npx --yes --package @pnp/cli-microsoft365@11.11.0 m365 login --authType deviceCode
🌶️  Provide values for the following parameters:
? appId:
```

The Microsoft 365 CLI sign-in is waiting for an **Entra application (client) ID** before it can authenticate to SharePoint.

## TODO

- [ ] Register or identify an Entra application and obtain its **Application (client) ID**. The Microsoft 365 CLI needs this public-client app ID to request delegated access on behalf of the signed-in user; without it, it cannot obtain an OAuth token for the SharePoint site.
- [ ] Configure the app as a public client and grant the delegated permissions needed for list operations, such as `Sites.ReadWrite.All`; grant/admin-consent the permissions when tenant policy requires it.
- [ ] Enter the client ID directly at the active `? appId:` terminal prompt. Do not store an app secret in this workspace; device-code sign-in uses the signed-in user's delegated identity.
- [ ] Complete the device-code sign-in flow when the CLI displays the browser URL and code.
- [ ] Create `MyAddressListSept2026DG` at `https://bartco1.sharepoint.com/sites/bartco1` as a generic SharePoint list.
- [ ] Add fields for display name, email address, position, time zone, street address, city, state/province, postal code, country/region, and phone number.
- [ ] Populate the list with realistic but fictional sample address data. Do not use real personal contact data.
- [ ] In Power Automate, create `HelloWorkflowSept2026DG` using the existing connected SharePoint and Office 365 Outlook connections.
- [ ] Configure the trigger as SharePoint **When an item is created** for `MyAddressListSept2026DG`.
- [ ] Add an **Send an email (V2)** action to `bartco1@bartco1.onmicrosoft.com` with an HTML body containing the key address fields, display name, position, email address, and time zone.
- [ ] Save, turn on, and test the workflow by adding one new list item; confirm the formatted email arrives.