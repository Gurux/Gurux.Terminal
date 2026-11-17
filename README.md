# Gurux.Terminal

Gurux.Terminal provides asynchronous AT modem communication and optional data-call dialing for .NET 10 and .NET 11. `GXTerminal` owns a shared `GXSerial` transport and adds serialized command/response exchanges, initialization and modem-error handling.

The transport library has no dependency on MAUI, Avalonia, Blazor or WinForms. Optional settings adapters and the example application provide UI separately. [Gurux.SMS](../Gurux.SMS/README.md) extends the modem command engine with SMS operations and PDU codecs.

## Packages and requirements

| Package | Purpose |
| --- | --- |
| Gurux.Terminal | UI-independent AT command engine and serial transport integration. |
| Gurux.Terminal.Avalonia | Optional Avalonia settings editor. |
| Gurux.Terminal.Blazor | Optional Blazor settings components. |
| Gurux.Terminal.Maui | Optional native MAUI settings editor. |

This README describes the **9.0 API**. Earlier 8.x packages use different APIs; use a matching 9.0 package release or build the current source.

The library targets `net10.0` and `net11.0`. `BuildMobile=true` enables the matching Android targets; `BuildApple=true` also enables iOS targets. Building from source uses the stable .NET 11 SDK selected by [global.json](global.json), including for .NET 10 outputs.

Native mobile builds require the corresponding workloads. iOS builds also require Apple tooling on a Mac and a supported accessory transport.

## Execute an AT command

This example opens an external modem, runs the default initialization commands and queries signal quality. Replace `COM3` with the actual device name, such as `/dev/ttyUSB0` on Linux.

~~~csharp
using Gurux.Terminal;

using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(60));
await using var modem = new GXTerminal
{
    PortName = "COM3",
    BaudRate = 115200,
    ConnectTimeout = TimeSpan.FromSeconds(10)
};

await modem.OpenAsync(timeout.Token);
string response = await modem.ExecuteCommandAsync("AT+CSQ", timeout.Token);
Console.WriteLine(response);
await modem.CloseAsync();
~~~

`OpenAsync()` opens the serial transport, executes `InitializeCommands` in order and optionally dials `PhoneNumber`. The default commands are `AT` and `ATE0`. Leave `PhoneNumber` empty for command-only use.

The returned response includes the modem's successful terminator. Support for a particular AT command depends on the modem and firmware.

## Initialization and dialing

Configure settings while disconnected. Initialization commands must be single nonempty lines without embedded carriage returns or line feeds.

~~~csharp
using Gurux.Terminal;

using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(60));
await using var modem = new GXTerminal
{
    PortName = "COM3",
    BaudRate = 115200,
    InitializeCommands = new[] { "AT", "ATE0", "AT+CMEE=2" },
    PhoneNumber = string.Empty
};

await modem.OpenAsync(timeout.Token);
Console.WriteLine(await modem.ExecuteCommandAsync("ATI", timeout.Token));
await modem.CloseAsync();
~~~

This example requests extended modem errors during startup and then reads modem identification. Use only commands supported by the selected modem.

For a data call, set `PhoneNumber` before opening. After initialization, `GXTerminal` sends `ATD` followed by that value. The configured number accepts digits and the supported dial-control characters `+*#,PWpw`. An empty value disables automatic dialing.

A positive `ConnectTimeout` bounds each active command exchange, including initialization and dialing responses. It is not one overall deadline for all startup commands. Supply a cancellation token when you also need an overall operation deadline.

## Command responses and errors

`ExecuteCommandAsync(command, cancellationToken)`:

- Requires one command line without its terminating carriage return; the library appends `\r` and sends ASCII bytes.
- Serializes command exchanges so their response readers cannot overlap.
- Accumulates fragmented response lines.
- Completes on `OK` or a line beginning with `CONNECT`.
- Throws `IOException` for `ERROR`, `+CME ERROR`, `+CMS ERROR`, `BUSY`, `NO CARRIER`, `NO DIALTONE` or `NO ANSWER`.
- Limits a collected command response to 64 KiB; response-buffer overflow is also reported as an error.

A timeout, caller cancellation or transport shutdown during an active exchange raises `OperationCanceledException`. Cancellation during the exchange closes the underlying transport so a late reply cannot satisfy a later command. Await cleanup and reopen before retrying.

Waiting to enter the command exchange is separately cancellable. `ConnectTimeout` begins when the exchange starts, so use the caller's token to bound queue waiting too.

Protected prompt/payload exchanges hold the command lock across both stages. Gurux.SMS uses this path for the atomic `AT+CMGS` prompt and PDU submission.

## Raw data and buffered receive

`SendAsync(data, receiver, cancellationToken)` sends bytes through the serial transport. It does **not** append an AT terminator or wait for a modem command response. Use `ExecuteCommandAsync` for commands and `SendAsync` for the protocol bytes appropriate to an established data connection.

`OnReceived` publishes incoming transport bytes, including bytes observed during command exchanges. An event is not necessarily a complete line or application packet.

For a buffered data exchange, enter `BeginSynchronous()` before sending, then keep the scope alive until `ReceiveAsync` completes. This example assumes an established data call to a peer whose protocol returns three bytes. Supply the dial destination as the first command-line argument.

~~~csharp
using Gurux.Common;
using Gurux.Terminal;

if (args.Length != 1)
{
    throw new ArgumentException(nameof(args));
}

using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(60));
await using var modem = new GXTerminal
{
    PortName = "COM3",
    BaudRate = 115200,
    PhoneNumber = args[0]
};

await modem.OpenAsync(timeout.Token);

using (modem.BeginSynchronous())
{
    var reply = new ReceiveParameters<byte[]>
    {
        Count = 3,
        WaitTime = 5000
    };

    await modem.SendAsync(new byte[] { 1, 2, 3 }, cancellationToken: timeout.Token);
    if (await modem.ReceiveAsync(reply, timeout.Token) && reply.Reply is byte[] response)
    {
        Console.WriteLine(Convert.ToHexString(response));
    }
}
~~~

Replace the payload and receive conditions with those of your peer's protocol. `ReceiveParameters.Eop` supports terminators across incoming fragments. Timeout or closure returns `false`; cancellation throws `OperationCanceledException`. `AllData` can expose a partial reply even when the receive returns `false`.

Do not run unrelated raw sends alongside AT command exchanges: raw writes do not participate in the command/response lock. Keep data-mode traffic and command-mode operations coordinated with the modem's state.

`BeginSynchronous()` enables buffering without holding a Monitor lock across `await`. Use the legacy `Synchronous` lock only for entirely blocking exchanges.

## Settings

| Setting | Default | Purpose |
| --- | --- | --- |
| `PortName` | Unset | Selected serial port or accessory identity; required before opening. |
| `BaudRate` | `9600` | Requested serial speed. |
| `DataBits` | `8` | Payload bits per serial frame. |
| `Parity` / `StopBits` | `None` / `One` | Serial framing. |
| `Handshake` | `None` | Flow-control policy. |
| `DtrEnable` / `RtsEnable` | `false` | Modem control signals where supported. |
| `InitializeCommands` | `AT`, `ATE0` | Startup command sequence. |
| `PhoneNumber` | Empty | Optional data-call destination. |
| `ConnectTimeout` | 30 seconds | Positive timeout for each active modem command exchange. |

Serial properties forward to the owned `GXSerial` and its `PortSettings`. `GetPortNames()` uses the shared serial discovery implementation. `Validate()` collects localized serial, initialization-command, timeout and dialing errors without opening a device.

`Settings` stores XML using the legacy serial, `Number`, `Init` and `ConnectionWaitTime` elements. Additional elements preserve complete initialization-command entries and timeout precision. `JsonSettings` stores serial settings, initialization commands, dialing options and the command timeout using source-generated JSON metadata.

Replacing serialized settings requires disconnected media with no pending startup or transport cleanup. Configure the terminal's serial settings before opening; `GXTerminal` does not itself expose the `IGXSerialSettings` live-update contract.

## Platforms and custom transports

| Platform | Modem connection |
| --- | --- |
| Windows | Serial COM port or a supported Bluetooth modem stream. |
| Linux | Serial device node or a supported BlueZ Bluetooth modem stream. |
| macOS | Serial port exposed by the operating system. |
| Android | An explicitly supplied native USB, RFCOMM or BLE modem stream factory. |
| iOS | An application-supplied supported accessory/BLE modem stream; arbitrary physical COM/USB ports are unavailable. |

The default constructor creates terminal media backed by `GXSerial`. The overload accepting `Func<SerialPortSettings, CancellationToken, ValueTask<Stream>>` uses the supplied transport factory instead.

A factory must honor cancellation and return a readable, writable stream owned by the media. Closing releases that stream. Android applications obtain device permissions and select the appropriate `AndroidSerialTransports` factory before opening.

Bluetooth or USB serial transport alone does not guarantee that the accessory implements AT modem commands. Use a modem and profile compatible with the commands and data-call behavior required by your application.

See [Gurux.Serial](../Gurux.Serial/README.md) for USB chipset support, Bluetooth profiles, native target requirements, discovery and device permissions.

## Lifecycle and threading

Prefer `OpenAsync`, `ExecuteCommandAsync`, `SendAsync`, `ReceiveAsync` and `CloseAsync` in asynchronous applications. Blocking media APIs remain available for compatibility.

`OpenAsync` completes after serial opening and modem initialization. Failed startup closes the underlying transport. Closing cancels command reads, wakes buffered receives, closes the serial connection and waits for pending opening work unless invoked from that opening operation itself.

Events are not UI-dispatched. Keep handlers short and marshal UI updates to the application dispatcher. Never hold a Monitor lock across `await`.

The media implements `IGXMediaIconProvider`. `OpenIcon()` returns the embedded terminal PNG through a caller-owned stream that must be disposed.

## Example application

[Gurux.Terminal.Example](Gurux.Terminal.Example/Gurux.Terminal.Example.csproj) contains its own Core logic, Avalonia desktop views and native MAUI views. It provides AT command execution, raw/hexadecimal data, optional framed receive, settings and activity history.

Run the Avalonia desktop example from this repository directory:

~~~powershell
dotnet run --project Gurux.Terminal.Example/Gurux.Terminal.Example.csproj -f net11.0 -p:BuildDesktop=true -p:GuruxExamplesBuild=true -p:GeneratePackageOnBuild=false
~~~

Build its Android host:

~~~powershell
dotnet build Gurux.Terminal.Example/Gurux.Terminal.Example.csproj -f net11.0-android -p:BuildMobile=true -p:GuruxExamplesBuild=true -p:GeneratePackageOnBuild=false
~~~

The example targets .NET 10 and .NET 11. Native hosts require matching MAUI workloads and platform tooling; Linux desktop uses Avalonia.

## Build and test

Run from this repository directory:

~~~powershell
dotnet build Development/Gurux.Terminal.csproj -p:GeneratePackageOnBuild=false
dotnet run --project ../Gurux.Serial/Tests/Serial.Tests.csproj -p:GeneratePackageOnBuild=false
~~~

Build the Android library target with:

~~~powershell
dotnet build Development/Gurux.Terminal.csproj -f net10.0-android -p:BuildMobile=true -p:GeneratePackageOnBuild=false
~~~

The shared [Serial/AT/PDU regression suite](../Gurux.Serial/Tests/Serial.Tests.csproj) exercises injected stream transports, fragmented command responses, modem errors, prompt/payload exchanges, timeout/cancellation, shutdown and SMS PDU behavior.

Headless tests and successful native compilation do not validate a physical modem, carrier data call or mobile background operation. Validate those scenarios with the intended devices and platform permissions.

## License

Gurux.Terminal is available under either [GPL-2.0-only](LICENSE) or a commercial license from Gurux Ltd. See [commercial licensing information](LICENSES/LicenseRef-Gurux-Commercial) or contact [Gurux Ltd](https://www.gurux.fi/).
