# Gurux.Terminal.Avalonia

## Why use this component?

Use this optional package to add a ready-made settings editor for `GXTerminal` to an existing Avalonia desktop application. It supplies localized fields, input validation and a staged Save/Cancel workflow so the host does not have to implement the media settings form itself.

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
| `Gurux.Terminal.Avalonia` | Settings controls and editing transactions for this framework and media type. |
| Host application | Owns the media, opens/closes connections, sends/receives data, persists accepted settings and supplies platform permissions. |
| [Gurux.Terminal.Example](../../Gurux.Terminal.Example/Gurux.Terminal.Example.csproj) | A complete example application with connection/data controls and activity views. Its application screens are separate from this reusable settings adapter. |

This package is for configuration forms. Adding it does not supply a complete communication application, automatically connect a device or persist settings.

## Editing and ownership

Keep the supplied media closed while editing. The editor stages changes in a private, unopened draft; Save validates before applying settings to the caller-owned media. Cancel discards pending edits. Open media and conflicting external changes are rejected.

The host retains ownership of the media and disposes it when appropriate. Dispose host-owned embedded views or drafts when their containing form ends. A successful Save updates the media configuration; the application decides whether to persist it and when to open a connection.

Use AvaloniaMediaSettingsEditor to show this media-specific editor from the host window.

Serial/Bluetooth discovery and explicit GATT inspection can access native device services; the host remains responsible for permission requests and supported platform transports. Displaying settings does not grant hardware access. Terminal and SMS adapters reuse Serial settings controls where applicable.

## Integration details

Optional Avalonia 12 settings editor for `GXTerminal` on .NET 10. Register `new Gurux.Terminal.Avalonia.AvaloniaMediaSettingsEditor(owner)` as the media settings editor and show the owner before calling `ShowAsync`. An optional parent uses `Gurux.Terminal.Avalonia.AvaloniaMediaWindow`.

Edits are staged on an unopened private media instance. Save validates all fields before applying settings, rejects an open transport and detects settings changed externally. Cancel, owner close and cancellation discard the draft. Cancellation completes with `OperationCanceledException`. Calls are dispatched to the UI thread and overlapping dialogs on an editor are rejected.

All labels and validation messages are in `Properties/Resources.resx`. No transport connection is opened by this library. Terminal and SMS reuse the Serial Avalonia port controls.
