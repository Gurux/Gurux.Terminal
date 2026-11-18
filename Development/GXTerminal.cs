// --------------------------------------------------------------------------
// Gurux Ltd
//
// Copyright (c) Gurux Ltd.
//
// This file is part of Gurux Device Framework.
//
// Commit: $Format:%H$
// Date:   $Format:%cI$
//
// This software is available under either of the following licenses:
//
// 1. GNU General Public License, version 2 only (GPL-2.0-only).
//    See the LICENSE file included with this distribution.
//
// 2. A commercial license granted by Gurux Ltd under a separate written
//    agreement. Use under this option requires a valid commercial license
//    and is governed by the terms of that agreement.
//
// If you have not obtained a commercial license from Gurux Ltd,
// use of this software is governed by GPL-2.0-only.
//
// Commercial licensing: https://www.gurux.fi/
//
// SPDX-License-Identifier: GPL-2.0-only OR LicenseRef-Gurux-Commercial
// --------------------------------------------------------------------------
using System.IO.Ports;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using System.Threading.Channels;
using Gurux.Serial;
using Gurux.Common;

namespace Gurux.Terminal;
/// <summary>AT modem serial media. Commands are serialized and modem errors are surfaced.</summary>
public class GXTerminal : GXMediaBase, IGXMediaIconProvider
{
    /// <summary>Opens the embedded PNG icon for this media without loading a UI framework.</summary>
    /// <returns>A new readable stream positioned at the beginning, or null if the embedded icon is unavailable.</returns>
    /// <remarks>The caller owns the stream and must dispose it. Each call returns an independent stream.</remarks>
    public virtual Stream? OpenIcon()
    {
        return typeof(GXTerminal).Assembly.GetManifestResourceStream("Gurux.Terminal.MediaIcon.png");
    }

    /// <summary>Occurs when the terminal reports a client connection attempt.</summary>
    public event EventHandler<ConnectionEventArgs>? OnClientConnected;
    /// <summary>Occurs when the terminal reports a client disconnection.</summary>
    public event EventHandler<ConnectionEventArgs>? OnClientDisconnected;
    /// <summary>Raises a terminal client connection event and returns its acceptance.</summary>
    /// <param name="info">The connection description supplied to event handlers.</param>
    /// <returns>Whether the connection event handlers accepted the reported client.</returns>
    protected bool NotifyClientConnected(string info)
    {
        var args = new ConnectionEventArgs(info);
        OnClientConnected?.Invoke(this, args);
        return args.Accept;
    }

    /// <summary>Raises a terminal client disconnection event.</summary>
    /// <param name="info">The connection description supplied to event handlers.</param>
    protected void NotifyClientDisconnected(string info) => OnClientDisconnected?.Invoke(this, new ConnectionEventArgs(info));
    /// <summary>Serializes modem command exchanges so their responses cannot overlap.</summary>
    readonly SemaphoreSlim _commands = new(1, 1);
    /// <summary>Buffers modem response bytes for the currently active command exchange.</summary>
    readonly Channel<byte> _responses = Channel.CreateBounded<byte>(new BoundedChannelOptions(65536) { SingleReader = true, SingleWriter = true, FullMode = BoundedChannelFullMode.Wait });
    /// <summary>Owns the serial connection carrying AT commands and modem responses.</summary>
    readonly GXSerial _transport;
    /// <summary>Serializes access to lifecycle transitions and pending cleanup state.</summary>
    readonly System.Threading.Lock _lifecycle = new();
    /// <summary>Identifies callbacks executing within the current asynchronous open operation.</summary>
    readonly AsyncLocal<bool> _inOpening = new();
    /// <summary>Signals completion of the currently pending open operation.</summary>
    TaskCompletionSource? _openingDone;
    /// <summary>Indicates whether incoming bytes belong to an active modem exchange.</summary>
    volatile bool _awaitingResponse;
    /// <summary>Cancels command reads when the modem closes or its response buffer overflows.</summary>
    CancellationTokenSource _commandLifetime = new();
    /// <summary>Creates terminal media that owns an operating system serial port transport.</summary>
    public GXTerminal() : this(new GXSerial())
    {
    }

    /// <summary>Creates terminal media that owns the stream returned by the supplied serial transport factory.</summary>
    /// <param name="streamFactory">Creates an owned readable and writable stream for the supplied serial settings and cancellation token.</param>
    /// <exception cref="ArgumentNullException">The stream factory is null.</exception>
    public GXTerminal(Func<SerialPortSettings, CancellationToken, ValueTask<Stream>> streamFactory) : this(new GXSerial(streamFactory))
    {
    }

    /// <summary>Creates terminal media and forwards the owned serial transport's data, errors and state changes.</summary>
    /// <param name="serial">The serial transport owned by this terminal.</param>
    GXTerminal(GXSerial serial)
    {
        _transport = serial;
        _transport.OnReceived += (_, e) =>
        {
            if (e.Data is byte[] data)
            {
                if (_awaitingResponse)
                {
                    foreach (byte b in data)
                    {
                        if (!_responses.Writer.TryWrite(b))
                        {
                            _commandLifetime.Cancel();
                            ReportError(new IOException(global::Gurux.Terminal.Properties.Resources.Get("ModemCommandResponseExceeds64KiB")));
                            break;
                        }
                    }
                }

                PublishReceived(data, e.SenderInfo);
            }
        };
        _transport.OnError += (_, e) => ReportError(e.Exception);
        _transport.OnMediaStateChange += (_, e) =>
        {
            if (e.State == MediaState.Closed)
            {
                ReceiveBuffer.Close();
                _commandLifetime.Cancel();
            }

            SetState(e.State);
        };
    }

    /// <summary>Gets the framing and device settings used by the underlying serial connection.</summary>
    public SerialPortSettings PortSettings => _transport.PortSettings;
    /// <summary>Gets or sets the operating system port name or platform accessory identifier.</summary>
    public string? PortName { get => _transport.PortName; set => _transport.PortName = value; }
    /// <summary>Gets or sets the requested transfer rate in bits per second.</summary>
    public int BaudRate { get => _transport.BaudRate; set => _transport.BaudRate = value; }
    /// <summary>Gets or sets the number of payload bits in each serial frame.</summary>
    public int DataBits { get => _transport.DataBits; set => _transport.DataBits = value; }
    /// <summary>Gets or sets the parity check used for each serial frame.</summary>
    public Parity Parity { get => _transport.Parity; set => _transport.Parity = value; }
    /// <summary>Gets or sets the stop interval terminating each serial frame.</summary>
    public StopBits StopBits { get => _transport.StopBits; set => _transport.StopBits = value; }
    /// <summary>Gets or sets the hardware or software flow control policy.</summary>
    public Handshake Handshake { get => _transport.Handshake; set => _transport.Handshake = value; }
    /// <summary>Gets or sets whether the data terminal ready signal is asserted.</summary>
    public bool DtrEnable { get => _transport.DtrEnable; set => _transport.DtrEnable = value; }
    /// <summary>Gets or sets whether the request to send signal is asserted.</summary>
    public bool RtsEnable { get => _transport.RtsEnable; set => _transport.RtsEnable = value; }
    /// <inheritdoc/>
    public override string Name => _transport.Name;
    /// <inheritdoc/>
    public override bool IsOpen => _transport.IsOpen;

    /// <summary>Enumerates desktop serial ports on supported operating systems.</summary>
    /// <returns>The available port names, or an empty array on unsupported platforms.</returns>
    public static string[] GetPortNames() => GXSerial.GetPortNames();
    /// <inheritdoc/>
    public override async ValueTask SendAsync(ReadOnlyMemory<byte> data, string? receiver = null, CancellationToken cancellationToken = default)
    {
        await _transport.SendAsync(data, receiver, cancellationToken).ConfigureAwait(false);
        AddBytesSent(data.Length);
    }

    /// <inheritdoc/>
    public override async Task CloseAsync()
    {
        Task? pending;
        lock (_lifecycle)
        {
            _commandLifetime.Cancel();
            pending = _openingDone?.Task;
            ReceiveBuffer.Close();
        }

        await _transport.CloseAsync().ConfigureAwait(false);
        if (pending != null && !_inOpening.Value)
        {
            await pending.ConfigureAwait(false);
        }
    }

    /// <summary>Gets whether opening the terminal should dial the configured phone number.</summary>
    protected virtual bool DialOnOpen => true;
    /// <summary>Gets or sets the modem dial destination or default SMS recipient.</summary>
    public string PhoneNumber { get; set; } = "";
    /// <summary>Gets or sets the single line AT commands sent before dialing or SMS initialization.</summary>
    public string[] InitializeCommands { get; set; } = ["AT", "ATE0"];
    /// <summary>Gets or sets the maximum duration allowed for a modem command response.</summary>
    public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(30);
    /// <inheritdoc/>
    public override string MediaType => "Terminal";

    /// <summary>Gets or replaces terminal settings using legacy serial, Number, Init and ConnectionWaitTime XML elements.</summary>
    /// <remarks>Accepts XML fragments or a Terminal wrapper. Additional command elements preserve semicolons within
    /// initialization commands, and ConnectTimeout preserves submillisecond precision. Null restores defaults.</remarks>
    /// <exception cref="InvalidOperationException">The serial transport is open, opening or awaiting cleanup.</exception>
    /// <exception cref="XmlException">The supplied XML is malformed.</exception>
    /// <exception cref="FormatException">A numeric, timeout or Boolean setting is malformed.</exception>
    /// <exception cref="ArgumentException">A serial framing enumeration cannot be parsed.</exception>
    public override string? Settings
    {
        get
        {
            var xml = new XElement("Terminal",
                new XElement("Number", PhoneNumber),
                new XElement("Init", string.Join(";", InitializeCommands)),
                new XElement("InitializeCommands", InitializeCommands.Select(command => new XElement("Command", command))),
                new XElement("ConnectionWaitTime", (long)ConnectTimeout.TotalMilliseconds),
                new XElement("ConnectTimeout", XmlConvert.ToString(ConnectTimeout)));
            return _transport.Settings + string.Concat(xml.Elements().Select(element => element.ToString(SaveOptions.DisableFormatting)));
        }
        set
        {
            var root = XElement.Parse("<Settings>" + value + "</Settings>");
            var xml = root.Element("Terminal") ?? root.Element("SMS") ?? root;
            string number = (string?)xml.Element("Number") ?? (string?)xml.Element("PhoneNumber") ?? string.Empty;
            string[] initialization = xml.Element("InitializeCommands") is XElement commands
                ? [.. commands.Elements("Command").Select(command => command.Value)]
                : xml.Element("Init") is XElement legacy ? legacy.Value.Split(';') : ["AT", "ATE0"];
            TimeSpan timeout = xml.Element("ConnectTimeout") is XElement duration
                ? XmlConvert.ToTimeSpan(duration.Value)
                : TimeSpan.FromMilliseconds((long?)xml.Element("ConnectionWaitTime") ?? (long?)xml.Element("CommandWaitTime") ?? 30000);
            ApplySettings(() =>
            {
                _transport.Settings = string.Concat(xml.Elements().Select(element => element.ToString(SaveOptions.DisableFormatting)));
                PhoneNumber = number;
                InitializeCommands = initialization;
                ConnectTimeout = timeout;
            });
        }
    }

    /// <summary>Gets or replaces JSON-serialized serial settings, initialization commands and dialing options.</summary>
    /// <remarks>Assigning null restores the default terminal and serial settings. The serial transport must be closed with no pending open or cleanup work.</remarks>
    /// <exception cref="InvalidOperationException">The underlying serial transport is open, opening or awaiting cleanup.</exception>
    /// <exception cref="JsonException">The supplied terminal or nested serial settings are not valid JSON.</exception>
    /// <exception cref="ArgumentException">The terminal or nested serial JSON value is null.</exception>
    public override string? JsonSettings
    {
        get => JsonSerializer.Serialize(new TerminalSettings { Serial = _transport.JsonSettings!, PhoneNumber = PhoneNumber, InitializeCommands = InitializeCommands, ConnectTimeout = ConnectTimeout }, TerminalJsonContext.Default.TerminalSettings);
        set
        {
            var settings = value == null ? new TerminalSettings() : JsonSerializer.Deserialize(value, TerminalJsonContext.Default.TerminalSettings) ?? throw new ArgumentException(global::Gurux.Terminal.Properties.Resources.Get("InvalidTerminalSettings"));
            ApplySettings(() =>
            {
                _transport.JsonSettings = settings.Serial;
                PhoneNumber = settings.PhoneNumber;
                InitializeCommands = settings.InitializeCommands;
                ConnectTimeout = settings.ConnectTimeout;
            });
        }
    }

    /// <summary>Applies a complete terminal or derived SMS settings snapshot while excluding connection startup.</summary>
    /// <param name="apply">The parsed serial and derived settings assignments to execute atomically with respect to opening.</param>
    /// <exception cref="ArgumentNullException">The assignment action is null.</exception>
    /// <exception cref="InvalidOperationException">The terminal is open or its opening operation has started.</exception>
    protected void ApplySettings(Action apply)
    {
        ArgumentNullException.ThrowIfNull(apply);
        lock (_lifecycle)
        {
            if (_openingDone != null || IsOpen)
            {
                throw new InvalidOperationException(global::Gurux.Terminal.Properties.Resources.Get("CloseTheTerminalBeforeChangingSettings"));
            }
            apply();
        }
    }

    /// <summary>Stores serial configuration, initialization commands and dialing options for JSON persistence.</summary>
    public sealed class TerminalSettings
    {
        /// <summary>Gets or sets the JSON-serialized configuration of the underlying serial transport.</summary>
        public string Serial { get; set; } = "{}";
        /// <summary>Gets or sets the modem dial destination or default SMS recipient.</summary>
        public string PhoneNumber { get; set; } = "";
        /// <summary>Gets or sets the single line AT commands sent before dialing or SMS initialization.</summary>
        public string[] InitializeCommands { get; set; } = ["AT", "ATE0"];
        /// <summary>Gets or sets the maximum duration allowed for a modem command response.</summary>
        public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(30);
    }

    /// <summary>Collects serial, command, timeout and dialing errors without changing the connection.</summary>
    /// <returns>All localized configuration errors, or an empty list when the settings are valid.</returns>
    public override IReadOnlyList<GXValidationError> Validate()
    {
        var errors = new List<GXValidationError>(_transport.Validate());
        if (InitializeCommands == null || InitializeCommands.Any(command => string.IsNullOrWhiteSpace(command) || command.Contains('\r') || command.Contains('\n')))
        {
            errors.Add(new(nameof(InitializeCommands), "InvalidValue", global::Gurux.Terminal.Properties.Resources.Get("InitializationCommandsMustBeSingleNonemptyLines")));
        }
        if (ConnectTimeout <= TimeSpan.Zero)
        {
            errors.Add(new(nameof(ConnectTimeout), "OutOfRange", global::Gurux.Terminal.Properties.Resources.Get("CommandTimeoutMustBePositive")));
        }
        if (PhoneNumber.Any(c => !char.IsAsciiDigit(c) && "+*#,PWpw".IndexOf(c) < 0))
        {
            errors.Add(new(nameof(PhoneNumber), "InvalidValue", global::Gurux.Terminal.Properties.Resources.Get("InvalidDialNumber")));
        }
        return errors;
    }

    /// <summary>Checks the serial framing, initialization command lines, command timeout and dial number.</summary>
    /// <exception cref="ArgumentException">The serial settings are invalid, an initialization command is blank or contains a line terminator, or the dial number contains an unsupported character.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The serial baud rate, data bit count or command timeout is outside its supported range.</exception>
    protected virtual void ValidateForOpen()
    {
        _transport.PortSettings.Validate();
        if (InitializeCommands == null || InitializeCommands.Any(command => string.IsNullOrWhiteSpace(command) || command.Contains('\r') || command.Contains('\n')))
        {
            throw new ArgumentException(global::Gurux.Terminal.Properties.Resources.Get("InitializationCommandsMustBeSingleNonemptyLines"));
        }

        if (ConnectTimeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(ConnectTimeout));
        }

        if (PhoneNumber.Any(c => !char.IsAsciiDigit(c) && "+*#,PWpw".IndexOf(c) < 0))
        {
            throw new ArgumentException(global::Gurux.Terminal.Properties.Resources.Get("InvalidDialNumber"));
        }
    }

    /// <inheritdoc/>
    public override async Task OpenAsync(CancellationToken cancellationToken = default)
    {
        TaskCompletionSource done;
        lock (_lifecycle)
        {
            if (_openingDone != null)
            {
                throw new InvalidOperationException(global::Gurux.Terminal.Properties.Resources.Get("TerminalIsAlreadyOpening"));
            }

            if (IsOpen)
            {
                return;
            }

            _openingDone = done = new(TaskCreationOptions.RunContinuationsAsynchronously);
            _commandLifetime.Dispose();
            _commandLifetime = new();
            ReceiveBuffer.Open();
        }

        bool previous = _inOpening.Value;
        _inOpening.Value = true;
        try
        {
            var errors = Validate();
            if (errors.Count != 0)
            {
                ValidateForOpen();
                throw new ArgumentException(errors[0].Message);
            }
            using var cancel = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _commandLifetime.Token);
            await _transport.OpenAsync(cancel.Token).ConfigureAwait(false);
            await InitializeAsync(cancel.Token).ConfigureAwait(false);
        }
        catch
        {
            await _transport.CloseAsync().ConfigureAwait(false);
            throw;
        }
        finally
        {
            lock (_lifecycle)
            {
                _openingDone = null;
            }

            done.TrySetResult();
            _inOpening.Value = previous;
        }
    }

    /// <summary>Sends configured initialization commands and optionally dials the destination.</summary>
    /// <param name="cancellationToken">Cancels the pending operation.</param>
    /// <returns>A task completing when modem initialization finishes.</returns>
    protected virtual async Task InitializeAsync(CancellationToken cancellationToken)
    {
        foreach (string command in InitializeCommands)
        {
            await ExecuteCommandAsync(command, cancellationToken).ConfigureAwait(false);
        }

        if (DialOnOpen && PhoneNumber.Length != 0)
        {
            await ExecuteCommandAsync("ATD" + PhoneNumber, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>Serializes a single AT command exchange and returns the successful modem response.</summary>
    /// <param name="command">The single line AT command without its terminating carriage return.</param>
    /// <param name="cancellationToken">Cancels the pending operation.</param>
    /// <returns>The modem response including its successful terminator.</returns>
    /// <exception cref="ArgumentException">The command is blank or contains a line terminator.</exception>
    /// <exception cref="InvalidOperationException">The serial transport is closed.</exception>
    /// <exception cref="IOException">The modem reports a failure or the response exceeds 64 KiB.</exception>
    /// <exception cref="OperationCanceledException">Cancellation, a command timeout or transport shutdown interrupts the exchange.</exception>
    public Task<string> ExecuteCommandAsync(string command, CancellationToken cancellationToken = default) => ExchangeAsync(Encoding.ASCII.GetBytes(ValidateCommand(command) + "\r"), false, cancellationToken);
    /// <summary>Rejects blank AT commands and embedded line terminators.</summary>
    /// <param name="command">The single line AT command without its terminating carriage return.</param>
    /// <returns>The validated AT command unchanged.</returns>
    /// <exception cref="ArgumentException">The command is blank or contains a line terminator.</exception>
    static string ValidateCommand(string command)
    {
        if (string.IsNullOrWhiteSpace(command) || command.Contains('\r') || command.Contains('\n'))
        {
            throw new ArgumentException(global::Gurux.Terminal.Properties.Resources.Get("AnATCommandMustBeASingleLine"));
        }

        return command;
    }

    /// <summary>Serializes a modem exchange with an optional prompt and payload.</summary>
    /// <param name="bytes">The complete command bytes to send to the modem.</param>
    /// <param name="prompt">Whether to stop when the modem requests a payload with its prompt.</param>
    /// <param name="cancellationToken">Cancels the pending operation.</param>
    /// <returns>The successful response collected from the modem.</returns>
    protected async Task<string> ExchangeAsync(ReadOnlyMemory<byte> bytes, bool prompt, CancellationToken cancellationToken)
    {
        await _commands.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            return await ExchangeCoreAsync(bytes, prompt, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _commands.Release();
        }
    }

    /// <summary>Writes command or payload bytes and reads until a requested prompt, successful terminator, failure or timeout.</summary>
    /// <remarks>The caller must hold the command semaphore throughout this exchange and any related prompt payload exchange.</remarks>
    /// <param name="bytes">The complete command bytes to send to the modem.</param>
    /// <param name="prompt">Whether to stop when the modem requests a payload with its prompt.</param>
    /// <param name="cancellationToken">Cancels the pending operation.</param>
    /// <returns>The collected modem response, including its prompt or successful terminator.</returns>
    /// <exception cref="InvalidOperationException">The serial transport is closed.</exception>
    /// <exception cref="IOException">The modem reports a failure or the response exceeds 64 KiB.</exception>
    /// <exception cref="OperationCanceledException">Cancellation, a command timeout or transport shutdown interrupts the exchange.</exception>
    protected async Task<string> ExchangeCoreAsync(ReadOnlyMemory<byte> bytes, bool prompt, CancellationToken cancellationToken)
    {
        while (_responses.Reader.TryRead(out _))
        {
        }

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _commandLifetime.Token);
        timeout.CancelAfter(ConnectTimeout);
        _awaitingResponse = true;
        try
        {
            await _transport.SendAsync(bytes, cancellationToken: timeout.Token).ConfigureAwait(false);
            AddBytesSent(bytes.Length);
            var result = new StringBuilder();
            var line = new StringBuilder();
            while (true)
            {
                char c = (char)await _responses.Reader.ReadAsync(timeout.Token).ConfigureAwait(false);
                result.Append(c);
                if (result.Length > 65536)
                {
                    throw new IOException(global::Gurux.Terminal.Properties.Resources.Get("ModemCommandResponseExceeds64KiB"));
                }

                if (prompt && c == '>')
                {
                    return result.ToString();
                }

                if (c == '\n')
                {
                    string response = line.ToString().Trim();
                    line.Clear();
                    if (response == "OK" || response.StartsWith("CONNECT", StringComparison.Ordinal))
                    {
                        return result.ToString();
                    }

                    if (response is "ERROR" or "NO CARRIER" or "NO DIALTONE" or "BUSY" or "NO ANSWER" || response.StartsWith("+CME ERROR", StringComparison.Ordinal) || response.StartsWith("+CMS ERROR", StringComparison.Ordinal))
                    {
                        throw new IOException(string.Format(
                            CultureInfo.CurrentCulture,
                            global::Gurux.Terminal.Properties.Resources.Get("ModemReturnedResponse"),
                            response));
                    }
                }
                else
                {
                    line.Append(c);
                }
            }
        }
        catch (OperationCanceledException)
        {
            await _transport.CloseAsync().ConfigureAwait(false);
            throw;
        }
        finally
        {
            _awaitingResponse = false;
        }
    }

    /// <summary>Atomically sends a prompt command followed by its data (for AT+CMGS).</summary>
    /// <param name="command">The single line AT command without its terminating carriage return.</param>
    /// <param name="payload">The encoded message bytes sent after the modem prompt.</param>
    /// <param name="cancellationToken">Cancels the pending operation.</param>
    /// <returns>The successful modem response after the prompt payload is submitted.</returns>
    protected async Task<string> ExecutePromptAsync(string command, ReadOnlyMemory<byte> payload, CancellationToken cancellationToken)
    {
        await _commands.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await ExchangeCoreAsync(Encoding.ASCII.GetBytes(ValidateCommand(command) + "\r"), true, cancellationToken).ConfigureAwait(false);
            return await ExchangeCoreAsync(payload, false, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _commands.Release();
        }
    }
}
