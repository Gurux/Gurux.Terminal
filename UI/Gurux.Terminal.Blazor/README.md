# Gurux.Terminal.Blazor

## Why use this component?

Use this optional package to add a ready-made settings editor for `GXTerminal` to an interactive Blazor application or a Razor-based settings surface. It supplies localized fields, input validation and a staged Save/Cancel workflow so the host does not have to implement the media settings form itself.

This adapter edits serial modem settings, initialization commands, optional dial destination and command timeout.

The media library can be used without this package in services, command-line tools, automated tests or applications that provide their own settings UI. Reference this adapter when users need to edit those settings interactively.

## Choose the UI for your application

| Host application | Settings adapter |
| --- | --- |
| Avalonia desktop | [Gurux.Terminal.Avalonia](../Gurux.Terminal.Avalonia/README.md) |
| Interactive Blazor / Razor UI | [Gurux.Terminal.Blazor](../Gurux.Terminal.Blazor/README.md) |
| Native .NET MAUI | [Gurux.Terminal.Maui](../Gurux.Terminal.Maui/README.md) |

Choose the adapter matching the host's UI framework; installing all three is unnecessary. A native MAUI host can use the MAUI adapter directly without a BlazorWebView. The Blazor adapter provides web components rather than native MAUI or Avalonia controls.

## How the pieces fit together

| Layer | Responsibility |
| --- | --- |
| [Gurux.Terminal transport](../../README.md) | Connection lifecycle, protocol operations and media settings. It remains independent of UI frameworks. |
| `Gurux.Terminal.Blazor` | Settings controls and editing transactions for this framework and media type. |
| Host application | Owns the media, opens/closes connections, sends/receives data, persists accepted settings and supplies platform permissions. |
| [Gurux.Terminal.Example](../../Gurux.Terminal.Example/Gurux.Terminal.Example.csproj) | A complete example application with connection/data controls and activity views. Its application screens are separate from this reusable settings adapter. |

This package is for configuration forms. Adding it does not supply a complete communication application, automatically connect a device or persist settings.

## Editing and ownership

Keep the supplied media closed while editing. The editor stages changes in a private, unopened draft; Save validates before applying settings to the caller-owned media. Cancel discards pending edits. Open media and conflicting external changes are rejected.

The host retains ownership of the media and disposes it when appropriate. Dispose host-owned embedded views or drafts when their containing form ends. A successful Save updates the media configuration; the application decides whether to persist it and when to open a connection.

Render MediaSettingsEditor for a standalone editing session. For custom composition, create and dispose a MediaSettingsDraft and render MediaSettingsForm inside the host container; this form provides its own Save/Cancel actions.

Blazor Server edits media on the server: device discovery and transport access refer to that server, not the browser's computer. Browser WebAssembly cannot use these adapters to gain arbitrary serial-port or TCP-socket access. The host must select an interactive render mode and serve the required static web assets.

Serial/Bluetooth discovery and explicit GATT inspection can access native device services; the host remains responsible for permission requests and supported platform transports. Displaying settings does not grant hardware access. Terminal and SMS adapters reuse Serial settings controls where applicable.

## Integration details

Optional .NET 10 Razor settings components for Terminal.
References the Gurux.UI.Components NuGet package, version 4.0.2611.1201.
MAUI keeps its native UI; Linux keeps Avalonia. Neither depends on this library.

## Host setup

Register appearance/input services in the Blazor application's Program.cs:

```csharp
builder.Services.AddGuruxMediaComponents(); // using Gurux.Terminal.Blazor;
```

Add the namespace to `_Imports.razor`:

```razor
@using Gurux.Terminal.Blazor
```

Load the package's local Bootstrap and adaptive layout CSS in the host HTML/head:

```html
<link data-gx-bootstrap rel="stylesheet" href="_content/Gurux.UI.Components/Appearance/themes/default.min.css" />
<link rel="stylesheet" href="_content/Gurux.UI.Components/Appearance/layout.css" />
```

The host must serve Razor static web assets (standard Blazor templates do).
No CDN or additional JavaScript script tag is needed: GXThemeProvider imports its
appearance module from the package. Keep the standard Blazor host script.

```razor
@if (editing)
{
    <MediaSettingsEditor Media="media" Completed="SettingsClosed" />
}
@code {
    private readonly Gurux.Terminal.GXTerminal media = new();
    private bool editing = true;
    private void SettingsClosed(bool saved) { editing = false; }
}
```

The parent owns and disposes `media`. The editor owns a private unopened draft.
Save validates and commits; Cancel, cancellation and component disposal discard
changes. Saving detects concurrent settings changes and rejects open media.
Remove/recreate the editor to start another editing session. Pass an optional
CancellationToken to close an editor when its owning operation ends.

MediaAppearance uses GXThemeProvider and GXAdaptiveLayout Auto: Windows Fluent,
Android Material, iOS Cupertino, other platforms default. Detection follows the
client WebView/browser, not the server OS. Color mode follows the client setting.

For custom hosts, MediaSettingsDraft and MediaSettingsForm are available separately;
the host must dispose its draft and may wrap the form with MediaAppearance.

## Hosting and transport limits

Use an interactive Blazor render mode. Blazor Server runs media operations on the
server; it cannot use serial ports attached to the browser device. Browser
WebAssembly cannot open System.IO.Ports or arbitrary TCP sockets. This library
provides settings UI and does not add browser transport adapters. Native MAUI
applications continue using Gurux.Terminal.Maui without BlazorWebView.

## Packaging

Build this optional project independently. This package references the Terminal transport, shared components and the Serial Blazor controls used by its modem settings. Terminal and SMS reuse the Serial Blazor port controls. No aggregate media UI package is required.

## Headless verification

From the GXMedias workspace, run `dotnet run --project Gurux.Net/UI/Gurux.Net.Blazor/Tests/Tests.csproj -p:GeneratePackageOnBuild=false -p:BuildInParallel=false`. The combined harness references all four libraries; production network dependencies remain isolated. These tests do not open physical transports.
