# Markdown and PAC CLI Format Guide

## Two meanings of format

This workspace uses two related, but different, ideas of format:

- **Markdown format codes** are the characters you write to structure a document. A Markdown renderer turns them into headings, lists, links, code, and other formatted content.
- **PAC CLI output formats** are representations returned by Power Platform CLI commands. A person may prefer readable output; an automation script usually needs structured output such as JSON or CSV.

The Markdown examples below use **GitHub Flavored Markdown (GFM)**. Each editable sample is followed immediately by its rendered result.

## Markdown format codes

### Headings

Use one to six `#` characters followed by a space. Use one `#` for the document title and increase the number for lower levels.

**Source**

```markdown
# Project title
## Main section
### Detail
```

**Rendered result**

# Project title

## Main section

### Detail

### Paragraphs and line breaks

Separate paragraphs with a blank line. To force a new line within a paragraph, end the preceding line with two spaces or use an HTML `<br>` tag.

**Source**

```markdown
This is the first paragraph.

This is the second paragraph.

First line with two trailing spaces.  
Second line.
```

**Rendered result**

This is the first paragraph.

This is the second paragraph.

First line with two trailing spaces.  
Second line.

### Emphasis and deletion

Wrap text in `*` or `_` for italics, `**` or `__` for bold, and `~~` for strikethrough.

**Source**

```markdown
*Italic text*, **bold text**, and ~~removed text~~.
```

**Rendered result**

*Italic text*, **bold text**, and ~~removed text~~.

### Quotes

Start each quoted line with `>`.

**Source**

```markdown
> Important: verify the target environment before running a command.
>
> A blank quoted line separates quoted paragraphs.
```

**Rendered result**

> Important: verify the target environment before running a command.
>
> A blank quoted line separates quoted paragraphs.

### Lists

Use `-`, `*`, or `+` for unordered lists. Use `1.` for ordered lists; most renderers number the items automatically. Indent nested items by two or four spaces consistently.

**Source**

```markdown
- Check the current environment.
- Export the solution.
  - Include dependencies when needed.

1. Authenticate.
1. Select an environment.
1. Run the command.
```

**Rendered result**

- Check the current environment.
- Export the solution.
  - Include dependencies when needed.

1. Authenticate.
1. Select an environment.
1. Run the command.

### Task lists

GFM renders `- [ ]` and `- [x]` as checkboxes.

**Source**

```markdown
- [x] Sign in to PAC CLI.
- [ ] Select the correct environment.
- [ ] Export the solution.
```

**Rendered result**

- [x] Sign in to PAC CLI.
- [ ] Select the correct environment.
- [ ] Export the solution.

### Links and images

Links use `[label](URL)`. Images use `![alternative text](URL-or-path)`. Alternative text describes the image when it cannot be displayed.

**Source**

```markdown
[Power Platform CLI documentation](https://learn.microsoft.com/power-platform/developer/cli/introduction)

![Environment selector screenshot](images/environment-selector.png)
```

**Rendered result**

[Power Platform CLI documentation](https://learn.microsoft.com/power-platform/developer/cli/introduction)

The image source would render an image from `images/environment-selector.png`; its alternative text is "Environment selector screenshot". This guide does not include that image file.

### Horizontal rules

Put three or more hyphens, asterisks, or underscores on their own line.

**Source**

```markdown
---
```

**Rendered result**

---

### Inline code and code blocks

Use single backticks for inline code. Use triple backticks before and after a multi-line code block. Add a language identifier after the opening fence for syntax highlighting where supported.

**Source**

````markdown
Run `pac auth list` to inspect saved profiles.

```powershell
pac auth list
pac org who
```
````

**Rendered result**

Run `pac auth list` to inspect saved profiles.

```powershell
pac auth list
pac org who
```

### Tables

Use pipes (`|`) to separate columns and a row of hyphens below the header. Colons in the separator row control alignment in many renderers.

**Source**

```markdown
| Command purpose | Preferred output | Use case |
| :--- | :---: | ---: |
| Quick inspection | Table or text | Human review |
| Automation | JSON | Scripts and APIs |
| Spreadsheet import | CSV | Reporting |
```

**Rendered result**

| Command purpose | Preferred output | Use case |
| :--- | :---: | ---: |
| Quick inspection | Table or text | Human review |
| Automation | JSON | Scripts and APIs |
| Spreadsheet import | CSV | Reporting |

### Escaping literal characters

Prefix a Markdown control character with a backslash (`\`) when it should appear as literal text.

**Source**

```markdown
\*This is not italic.\*

\# This is not a heading.
```

**Rendered result**

\*This is not italic.\*

\# This is not a heading.

### Collapsible details

GFM supports selected HTML elements. `<details>` and `<summary>` can create a collapsible section in GitHub renderers.

**Source**

````markdown
<details>
<summary>Show command output notes</summary>

Keep secrets and access tokens out of saved output.

</details>
````

**Rendered result**

<details>
<summary>Show command output notes</summary>

Keep secrets and access tokens out of saved output.

</details>

## Writing reliable Markdown

- Preview the file in the target platform. VS Code, GitHub, Azure DevOps, and documentation sites can support different extensions.
- Use blank lines around headings, lists, tables, blockquotes, and fenced code blocks for predictable rendering.
- Keep table cell content short. Complex content, multi-line code, and nested lists are often clearer outside a table.
- Use descriptive link labels rather than raw URLs when the destination matters to readers.
- Use language names on code fences, such as `powershell`, `json`, `yaml`, or `markdown`, when supported.
- Keep the source of a screenshot or diagram close to the document so a relative image path continues to work after a repository clone.
- HTML support is renderer-dependent. Use it only when standard Markdown cannot express the required result.

## PAC CLI output formats

PAC CLI commands may display information in a human-friendly layout, or return it in a structured representation intended for tools. These output formats are not Markdown codes: they are command results that can be copied into Markdown code fences, parsed by scripts, imported into spreadsheets, or inspected by a person.

The available output option and its spelling vary by command and installed PAC CLI version. Check the local command help before relying on a switch:

```powershell
pac <area> <command> --help
```

### Human-readable output

Use the default or table/text-oriented output for a quick interactive inspection. It is convenient for people, but column spacing and labels may change between versions, which makes it fragile for automation.

**Illustrative command**

```powershell
pac org who
```

**Example formatted output**

```text
Environment: Contoso Development
Environment ID: 00000000-0000-0000-0000-000000000000
```

### JSON output

JSON stores named fields and values. Prefer it for PowerShell, JavaScript, CI/CD, and any workflow that needs to select a field without parsing display spacing. The actual format switch is command-specific.

**Illustrative pattern**

```powershell
pac <area> <command> <format-option-for-json>
```

**Example formatted output**

```json
{
  "environmentName": "Contoso Development",
  "environmentId": "00000000-0000-0000-0000-000000000000"
}
```

**PowerShell parsing example**

```powershell
$result = '<command output in JSON>' | ConvertFrom-Json
$result.environmentId
```

### CSV output

CSV stores rows and columns for spreadsheet or reporting workflows. It is useful for flat records; nested objects are generally better represented in JSON. As with JSON, confirm whether the specific PAC CLI command offers CSV and which switch enables it.

**Illustrative output**

```csv
environmentName,environmentId
Contoso Development,00000000-0000-0000-0000-000000000000
```

### Choosing an output format

| Need | Prefer | Why |
| --- | --- | --- |
| Read a result at the terminal | Human-readable output | Easy to scan interactively |
| Use one field in a script | JSON | Named fields are stable to access |
| Import flat rows into Excel | CSV | Spreadsheet-friendly columns |
| Record a command in documentation | Markdown code fence | Preserves commands and output literally |

Avoid scraping table alignment or descriptive labels in a script. Use a documented structured output option when the command provides one; otherwise, check whether the PAC CLI command offers a supported API-oriented alternative.

## Standards and resources

### Markdown standards

- [CommonMark Specification](https://spec.commonmark.org/): a precise, widely implemented Markdown specification.
- [GitHub Flavored Markdown Specification](https://github.github.com/gfm/): GitHub's CommonMark-based extension, including tables, task lists, strikethrough, and autolinks.
- [GitHub Docs: Basic writing and formatting syntax](https://docs.github.com/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax): practical syntax guidance for GitHub rendering.
- [Markdown Guide](https://www.markdownguide.org/): a readable tutorial and reference with notes on processor differences.

CommonMark defines a standardized Markdown core. GFM extends CommonMark with additional features that are popular in repositories, including this guide's tables, task lists, and strikethrough. A renderer may implement only a subset or introduce its own extensions, so its documentation is the final authority for its display behavior.

### PAC CLI resources

- [Power Platform CLI introduction](https://learn.microsoft.com/power-platform/developer/cli/introduction)
- [Power Platform CLI command reference](https://learn.microsoft.com/power-platform/developer/cli/reference)
- Local installed help: `pac --help` and `pac <area> <command> --help`

Microsoft's CLI reference and the help exposed by the installed `pac` executable are authoritative for a command's supported output switches, option names, and behavior. Verify those sources before building automation around a particular format.