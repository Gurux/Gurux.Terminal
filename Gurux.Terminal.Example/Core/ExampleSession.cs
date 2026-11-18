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

using System.Diagnostics;
using System.Globalization;
using System.Text;
using Gurux.Common;
using Gurux.Serial;
using Gurux.Terminal;
using Gurux.Terminal.Example.Properties;

namespace Gurux.Terminal.Example;

/// <summary>Serializes modem operations, cancellation and bounded activity reporting for this application.</summary>
public sealed class ExampleSession : IAsyncDisposable
{
    /// <summary>Clears retained activity without changing received payloads, connection state or traffic counters.</summary>
    public void ClearActivity() => _log.Clear();
    /// <summary>Stores received payloads independently of diagnostic activity.</summary>
    private readonly Queue<byte[]> _receivedData = new();
    /// <summary>Tracks retained payload bytes to bound display history to 64 KiB.</summary>
    private int _receivedDataSize;

    /// <summary>Clears received display history without changing the connection, counters or activity log.</summary>
    public void ClearReceived()
    {
        lock (_sync)
        {
            _receivedData.Clear();
            _receivedDataSize = 0;
        }
    }

    /// <summary>Formats the bounded received payload history in arrival order.</summary>
    /// <param name="hex">Whether every payload must be shown as hexadecimal.</param>
    /// <returns>The received frames separated by platform line endings.</returns>
    public string ReceivedData(bool hex = false)
    {
        lock (_sync)
        {
            return string.Join(Environment.NewLine, _receivedData.Select(data => ExampleDataDisplay.Format(data, hex)));
        }
    }

    /// <summary>Copies incoming data into the independently bounded received display history.</summary>
    /// <param name="data">The received bytes or textual payload reported by the media.</param>
    private void RecordReceived(object? data)
    {
        byte[] payload = data is byte[] bytes ? bytes : System.Text.Encoding.UTF8.GetBytes(Convert.ToString(data, CultureInfo.CurrentCulture) ?? "");
        if (payload.Length == 0)
        {
            return;
        }
        byte[] retained = payload.AsSpan(Math.Max(0, payload.Length - 65536)).ToArray();
        lock (_sync)
        {
            _receivedData.Enqueue(retained);
            _receivedDataSize += retained.Length;
            while (_receivedData.Count > 500 || _receivedDataSize > 65536)
            {
                _receivedDataSize -= _receivedData.Dequeue().Length;
            }
        }
    }

    /// <summary>Allows one modem operation at a time.</summary>
    private readonly SemaphoreSlim _operations = new(1, 1);
    /// <summary>Protects cancellation and disposal state.</summary>
    private readonly object _sync = new();
    /// <summary>Stores recent modem activity.</summary>
    private readonly SessionLog _log = new();
    /// <summary>Cancels operations when the connection closes.</summary>
    private CancellationTokenSource _lifetime = new();
    /// <summary>Records whether disposal has started.</summary>
    private bool _disposed;
    /// <summary>Indicates that an operation holds the modem gate.</summary>
    private int _busy;
    /// <summary>Gets the application's modem transport.</summary>
    public GXTerminal Media { get; }
    /// <summary>Gets whether a modem operation is executing.</summary>
    public bool IsBusy => Volatile.Read(ref _busy) != 0;
    /// <summary>Gets a stable copy of recent activity.</summary>
    public IReadOnlyList<string> Log => _log.Snapshot;

    /// <summary>Creates the modem using the native serial port or a supplied platform stream factory.</summary>
    /// <param name="serialFactory">Optional USB, Bluetooth or test stream opener.</param>
    public ExampleSession(Func<SerialPortSettings, CancellationToken, ValueTask<Stream>>? serialFactory = null)
        : this(serialFactory is null ? new GXTerminal() : new GXTerminal(serialFactory))
    {
    }

    /// <summary>Observes an existing modem instance and takes responsibility for its disposal.</summary>
    /// <param name="media">The modem owned by the session.</param>
    /// <exception cref="ArgumentNullException">The modem is null.</exception>
    public ExampleSession(GXTerminal media)
    {
        Media = media ?? throw new ArgumentNullException(nameof(media));
        Media.Trace = TraceLevel.Verbose;
        Media.OnTrace += Received;
        Media.OnError += Error;
    }

    /// <summary>Records raw incoming bytes, including buffered replies, displaying non-ASCII data only as hexadecimal.</summary>
    /// <param name="sender">The transport raising the callback.</param>
    /// <param name="args">The trace category, raw data and sender information.</param>
    private void Received(object? sender, TraceEventArgs args)
    {
        if (args.Type != TraceTypes.Received)
        {
            return;
        }
        RecordReceived(args.Data);
        if (args.Data is byte[] bytes)
        {
            _log.Add(ExampleDataDisplay.IsAscii(bytes)
                ? string.Format(CultureInfo.CurrentCulture, ExampleResources.ReceivedBytes, args.Receiver, Convert.ToHexString(bytes), ExampleDataDisplay.Format(bytes))
                : string.Format(CultureInfo.CurrentCulture, ExampleResources.Received, args.Receiver, ExampleDataDisplay.Format(bytes)));
        }
        else
        {
            _log.Add(string.Format(CultureInfo.CurrentCulture, ExampleResources.Received, args.Receiver, args.Data));
        }
    }
    /// <summary>Records transport failures for display by the application.</summary>
    /// <param name="sender">The transport raising the failure.</param>
    /// <param name="args">The transport exception.</param>
    private void Error(object? sender, MediaErrorEventArgs args) => _log.Add(string.Format(CultureInfo.CurrentCulture, ExampleResources.Error, args.Exception.Message));

    /// <summary>Runs a modem action under the operation gate and linked cancellation.</summary>
    /// <typeparam name="T">The action result type.</typeparam>
    /// <param name="action">The asynchronous modem operation.</param>
    /// <param name="cancellationToken">Cancels waiting or executing the action.</param>
    /// <returns>The result of the modem action.</returns>
    /// <exception cref="ObjectDisposedException">Disposal has begun.</exception>
    /// <exception cref="OperationCanceledException">The caller or connection cancels the operation.</exception>
    private async Task<T> RunAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken)
    {
        CancellationToken token;
        lock (_sync) { ObjectDisposedException.ThrowIf(_disposed, this); token = _lifetime.Token; }
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(token, cancellationToken);
        await _operations.WaitAsync(linked.Token).ConfigureAwait(false);
        Interlocked.Exchange(ref _busy, 1);
        try { linked.Token.ThrowIfCancellationRequested(); return await action(linked.Token).ConfigureAwait(false); }
        finally { Interlocked.Exchange(ref _busy, 0); _operations.Release(); }
    }

    /// <summary>Opens and initializes the modem with cancellation support.</summary>
    /// <param name="cancellationToken">Cancels opening or initialization.</param>
    /// <returns>A task completing after the modem opens.</returns>
    public Task OpenAsync(CancellationToken cancellationToken = default) => RunAsync(async token =>
    {
        await Media.OpenAsync(token).ConfigureAwait(false);
        _log.Add(ExampleResources.Opened);
        return true;
    }, cancellationToken);

    /// <summary>Cancels pending operations, closes the modem and resets connection cancellation.</summary>
    /// <returns>A task completing after close and active operations finish.</returns>
    public async Task CloseAsync()
    {
        lock (_sync) { _lifetime.Cancel(); }
        var closing = Media.CloseAsync();
        await _operations.WaitAsync().ConfigureAwait(false);
        try
        {
            await closing.ConfigureAwait(false);
            lock (_sync) { _lifetime.Dispose(); _lifetime = new(); }
            _log.Add(ExampleResources.Closed);
        }
        finally { _operations.Release(); }
    }

    /// <summary>Executes a single-line terminal command or sends text containing line endings or hexadecimal bytes with an optional framed reply.</summary>
    /// <param name="text">The terminal command or hexadecimal byte string; in text mode, literal \r and \n sequences become carriage returns and line feeds. Text containing line endings is sent unchanged after decoding.</param>
    /// <param name="hex">Whether the input and reply use hexadecimal representation.</param>
    /// <param name="receiveReply">Whether a raw text or hexadecimal exchange waits for a reply.</param>
    /// <param name="eop">Optional end of packet delimiter in the selected representation.</param>
    /// <param name="count">Minimum reply length; zero selects delimiter or available data.</param>
    /// <param name="waitTime">Reply timeout in milliseconds.</param>
    /// <param name="cancellationToken">Cancels sending or receiving.</param>
    /// <returns>The terminal response or raw reply as ASCII text, or hexadecimal when requested or non-ASCII; null when a raw send requests no reply.</returns>
    /// <exception cref="TimeoutException">The requested reply does not arrive before the timeout.</exception>
    /// <exception cref="FormatException">A hexadecimal input is malformed.</exception>
    public Task<string?> SendAsync(string text, bool hex = false, bool receiveReply = false, string? eop = null, int count = 0, int waitTime = 5000, CancellationToken cancellationToken = default)
        => RunAsync<string?>(async token =>
        {
            if (!hex)
            {
                text = text.Replace(@"\r", "\r", StringComparison.Ordinal).Replace(@"\n", "\n", StringComparison.Ordinal);
            }
            if (!hex && !text.Contains('\r') && !text.Contains('\n'))
            {
                var exchange = Media.ExecuteCommandAsync(text, token);
                var sent = Encoding.ASCII.GetBytes(text + "\r");
                _log.Add(string.Format(CultureInfo.CurrentCulture, ExampleResources.Sent, Convert.ToHexString(sent), Encoding.ASCII.GetString(sent)));
                var responseBytes = Encoding.Latin1.GetBytes(await exchange.ConfigureAwait(false));
                var response = ExampleDataDisplay.Format(responseBytes);
                _log.Add(string.Format(CultureInfo.CurrentCulture, ExampleResources.Reply, response));
                return response;
            }
            var data = hex ? Convert.FromHexString(string.Concat(text.Where(c => !char.IsWhiteSpace(c)))) : Encoding.UTF8.GetBytes(text);
            using var scope = receiveReply ? Media.BeginSynchronous() : null;
            if (receiveReply) { Media.ResetSynchronousBuffer(); }
            await Media.SendAsync(data, null, token).ConfigureAwait(false);
            _log.Add(ExampleDataDisplay.IsAscii(data)
                ? string.Format(CultureInfo.CurrentCulture, ExampleResources.Sent, Convert.ToHexString(data), ExampleDataDisplay.Format(data))
                : string.Format(CultureInfo.CurrentCulture, ExampleResources.SentData, ExampleDataDisplay.Format(data)));
            if (!receiveReply) { return null; }
            var parameters = new ReceiveParameters<byte[]> { Count = count, WaitTime = waitTime,
                Eop = string.IsNullOrEmpty(eop) ? null : (hex ? Convert.FromHexString(string.Concat(eop.Where(c => !char.IsWhiteSpace(c)))) : Encoding.UTF8.GetBytes(eop)),
                AllData = count == 0 && string.IsNullOrEmpty(eop) };
            if (!await Media.ReceiveAsync(parameters, token).ConfigureAwait(false)) { throw new TimeoutException(ExampleResources.NoReply); }
            var result = ExampleDataDisplay.Format(parameters.Reply ?? [], hex);
            _log.Add(string.Format(CultureInfo.CurrentCulture, ExampleResources.Reply, result));
            return result;
        }, cancellationToken);

    /// <summary>Stops operations, detaches callbacks and disposes the owned modem once.</summary>
    /// <returns>A task completing after modem disposal.</returns>
    public async ValueTask DisposeAsync()
    {
        lock (_sync) { if (_disposed) { return; } _disposed = true; }
        await CloseAsync().ConfigureAwait(false);
        Media.OnTrace -= Received;
        Media.OnError -= Error;
        await Media.DisposeAsync().ConfigureAwait(false);
    }
}
