# Hello World DG: SharePoint and Workflow TODO

This guide expands the TODO section saved in [HelloWorldDG.Chat.md](HelloWorldDG.Chat.md). It is written for a first-time user of Microsoft Entra app registrations and the CLI for Microsoft 365.

## Before you start

- You need permission to create an app registration in the `bartco1.onmicrosoft.com` tenant, or an administrator who can create one for you.
- You need at least edit permission on `https://bartco1.sharepoint.com/sites/bartco1`.
- You need permission to create cloud flows and use the existing SharePoint and Office 365 Outlook connections in the Power Platform default environment.
- The sample contacts below must be fictional. Do not place real personal contact data in a training list.

## 1. Register or identify an Entra application

The CLI for Microsoft 365 needs an Entra application (also called an app registration) to identify the client requesting a token. The application ID is public identification, not a password. Device-code authentication signs in as you, so do not create or save a client secret for this task.

1. Go to [Microsoft Entra admin center](https://entra.microsoft.com/).
2. Open **Identity** > **Applications** > **App registrations**.
3. Select **New registration**.
4. Enter a name such as `M365 CLI SharePoint Training`.
5. Select **Accounts in this organizational directory only**.
6. Leave **Redirect URI** empty and select **Register**.
7. On the Overview page, copy **Application (client) ID**. This is the value needed at the CLI `appId` prompt.

If you already have a suitable organization-approved public client app, use its Application (client) ID instead of registering a duplicate.

## 2. Configure the app for delegated access

The CLI requests delegated permissions. That means the commands act with your user permissions after you sign in; permissions do not bypass your SharePoint access.

1. In the app registration, open **Authentication**.
2. Under **Advanced settings**, set **Allow public client flows** to **Yes**, then save.
3. Open **API permissions** > **Add a permission** > **Microsoft Graph** > **Delegated permissions**.
4. Add `Sites.ReadWrite.All` to create the list, fields, and items.
5. Add only additional permissions required by the commands you intend to use. Do not add application permissions or broad directory permissions for this task.
6. If your organization requires it, ask a tenant administrator to select **Grant admin consent**. Without consent, the device-code sign-in may show a consent or authorization error.

`Sites.ReadWrite.All` permits the signed-in user to read and write SharePoint sites to which that user has access. It is necessary because the task creates a list, adds columns, and creates list items.

## 3. Install and sign in to CLI for Microsoft 365

The CLI for Microsoft 365 is a Node.js command-line tool. In this workspace, it was run temporarily with `npx`, which downloads the version for that command without a global install.

1. Confirm Node.js is installed:

   ```powershell
   node --version
   npm --version
   ```

2. Start sign-in. Replace the placeholder with the Application (client) ID from step 1:

   ```powershell
   npx --yes --package @pnp/cli-microsoft365@11.11.0 m365 login --authType deviceCode --appId <APPLICATION-CLIENT-ID>
   ```

3. The command displays a Microsoft sign-in URL and a device code. Open the URL, enter the code, and sign in as `bartco1@bartco1.onmicrosoft.com`.
4. Confirm the sign-in state:

   ```powershell
   npx --yes --package @pnp/cli-microsoft365@11.11.0 m365 status
   ```

To install the CLI globally instead, run `npm install -g @pnp/cli-microsoft365`, then use `m365` in place of the longer `npx` command. Details are in [CLI.Explain.md](CLI.Explain.md).

## 4. Create the SharePoint list

Create `MyAddressListSept2026DG` as a generic list at the requested site.

```powershell
npx --yes --package @pnp/cli-microsoft365@11.11.0 m365 spo list add `
  --title "MyAddressListSept2026DG" `
  --baseTemplate GenericList `
  --webUrl "https://bartco1.sharepoint.com/sites/bartco1" `
  --description "Fictional address contacts for HelloWorkflowSept2026DG" `
  --output json
```

After creation, confirm it is present:

```powershell
npx --yes --package @pnp/cli-microsoft365@11.11.0 m365 spo list get `
  --title "MyAddressListSept2026DG" `
  --webUrl "https://bartco1.sharepoint.com/sites/bartco1"
```

Do not rerun the create command after it succeeds; it will fail because the list title already exists.

## 5. Add columns

Use SharePoint's existing `Title` field for the display name. Add the following columns using **List settings** > **Create column** in the SharePoint site, or use the equivalent `m365 spo field add` commands after checking `m365 spo field add --help`.

| Display name | Recommended type | Internal-name suggestion | Example |
| --- | --- | --- | --- |
| Email Address | Single line of text | `EmailAddress` | `ava.chen@example.test` |
| Position | Single line of text | `Position` | `Facilities Coordinator` |
| Time Zone | Choice | `TimeZone` | `Pacific Standard Time` |
| Street Address | Multiple lines of text | `StreetAddress` | `410 Pine Avenue` |
| City | Single line of text | `City` | `Seattle` |
| State/Province | Single line of text | `StateProvince` | `WA` |
| Postal Code | Single line of text | `PostalCode` | `98101` |
| Country/Region | Single line of text | `CountryRegion` | `United States` |
| Phone Number | Single line of text | `PhoneNumber` | `+1 206 555 0147` |

For the **Time Zone** choice field, add a sensible set of values such as `Pacific Standard Time`, `Mountain Standard Time`, `Central Standard Time`, `Eastern Standard Time`, `GMT Standard Time`, and `W. Europe Standard Time`.

## 6. Populate fictional sample data

Add at least five fictional contacts. Use clearly fictional email domains, such as `example.test`, so messages are never directed to real people.

| Display name | Email address | Position | Time zone | Address |
| --- | --- | --- | --- | --- |
| Ava Chen | `ava.chen@example.test` | Facilities Coordinator | Pacific Standard Time | 410 Pine Avenue, Seattle, WA 98101, United States |
| Marcus Reed | `marcus.reed@example.test` | Program Analyst | Mountain Standard Time | 85 Canyon Road, Denver, CO 80202, United States |
| Priya Nair | `priya.nair@example.test` | Operations Manager | Central Standard Time | 220 Lake Street, Chicago, IL 60601, United States |
| Elena Rossi | `elena.rossi@example.test` | Customer Support Lead | Eastern Standard Time | 16 Harbor Way, Boston, MA 02110, United States |
| Jonas Weber | `jonas.weber@example.test` | Regional Coordinator | W. Europe Standard Time | 72 Lindenstrasse, Berlin, 10115, Germany |

In the SharePoint site, open the list and choose **New** to enter the records. Fill the `Title` field with the display name. Add the data before creating the workflow if you do not want notification emails for the sample seed records.

## 7. Create HelloWorkflowSept2026DG

CLI for Microsoft 365 can list, export, enable, disable, and remove existing cloud flows, but it cannot create a new Power Automate flow. Create this one in the Power Automate web designer.

1. Go to [Power Automate](https://make.powerautomate.com/).
2. Select the `bartco1 (default)` environment.
3. Select **Create** > **Automated cloud flow**.
4. Name the flow `HelloWorkflowSept2026DG`.
5. Select the SharePoint trigger **When an item is created**.
6. Set **Site Address** to `https://bartco1.sharepoint.com/sites/bartco1`.
7. Set **List Name** to `MyAddressListSept2026DG`.
8. Select **New step** and add the Office 365 Outlook action **Send an email (V2)**.
9. Choose the existing connected Outlook account for `bartco1@bartco1.onmicrosoft.com`.
10. Set **To** to `bartco1@bartco1.onmicrosoft.com`.
11. Set a subject such as `New address: @{triggerOutputs()?['body/Title']}`.
12. In the email body, switch to HTML/code view where available and use the template in the next section.
13. Save the flow and turn it on.

## 8. Use an HTML email body

Insert dynamic content for each list column. The exact internal field names depend on how the columns were created. Use the dynamic-content picker rather than typing uncertain internal names.

```html
<div style="font-family:Segoe UI,Arial,sans-serif;color:#172235;max-width:640px;">
  <h2 style="margin:0 0 16px;color:#103a5b;">New Address List Entry</h2>
  <table style="border-collapse:collapse;width:100%;">
    <tr><td style="padding:8px;font-weight:600;border-bottom:1px solid #d7e0dc;">Display name</td><td style="padding:8px;border-bottom:1px solid #d7e0dc;">[Title dynamic content]</td></tr>
    <tr><td style="padding:8px;font-weight:600;border-bottom:1px solid #d7e0dc;">Position</td><td style="padding:8px;border-bottom:1px solid #d7e0dc;">[Position dynamic content]</td></tr>
    <tr><td style="padding:8px;font-weight:600;border-bottom:1px solid #d7e0dc;">Email</td><td style="padding:8px;border-bottom:1px solid #d7e0dc;">[Email Address dynamic content]</td></tr>
    <tr><td style="padding:8px;font-weight:600;border-bottom:1px solid #d7e0dc;">Time zone</td><td style="padding:8px;border-bottom:1px solid #d7e0dc;">[Time Zone dynamic content]</td></tr>
    <tr><td style="padding:8px;font-weight:600;border-bottom:1px solid #d7e0dc;">Address</td><td style="padding:8px;border-bottom:1px solid #d7e0dc;">[Street Address], [City], [State/Province] [Postal Code], [Country/Region]</td></tr>
  </table>
</div>
```

The Outlook **Send an email (V2)** action sends HTML when the body contains HTML markup. Do not include untrusted raw HTML from a list field in the body.

## 9. Test and verify

1. In SharePoint, add one new fictional list item after the flow is turned on.
2. In Power Automate, open `HelloWorkflowSept2026DG` > **Run history**.
3. Confirm the run succeeded and the trigger supplied the expected fields.
4. Check the mailbox for `bartco1@bartco1.onmicrosoft.com`.
5. Confirm the email contains the display name, position, email, time zone, and formatted address.
6. If a run fails, open the failed action to inspect the connector error and confirm the selected SharePoint list and Outlook connection.

## Original TODO

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