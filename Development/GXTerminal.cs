//
// --------------------------------------------------------------------------
//  Gurux Ltd
// 
//
//
// Filename:        $HeadURL$
//
// Version:         $Revision$,
//                  $Date$
//                  $Author$
//
// Copyright (c) Gurux Ltd
//
//---------------------------------------------------------------------------
//
//  DESCRIPTION
//
// This file is a part of Gurux Device Framework.
//
// Gurux Device Framework is Open Source software; you can redistribute it
// and/or modify it under the terms of the GNU General Public License 
// as published by the Free Software Foundation; version 2 of the License.
// Gurux Device Framework is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
// See the GNU General Public License for more details.
//
// This code is licensed under the GNU General Public License v2. 
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Gurux.Common;
using System.IO;
using Gurux.Shared;
using System.Windows.Forms;
using System.Xml;
using System.Diagnostics;
using System.IO.Ports;
using System.Reflection;
using System.Threading;
using Gurux.Terminal.Properties;

namespace Gurux.Terminal
{    
    enum Progress
    {
        None,
        Connecting,
        Connected
    }

	/// <summary>
	/// A media component that enables communication of Terminal port.
    /// See help in http://www.gurux.org/index.php?q=Gurux.Terminal
	/// </summary>
    public class GXTerminal : IGXMedia, IGXVirtualMedia, INotifyPropertyChanged, IDisposable
    {
        bool IsVirtual, VirtualOpen;
        int m_ConnectionWaitTime = 30000;
        int m_CommandWaitTime = 3000;
        private object m_sync = new object();

        Progress Progress;
        bool m_Server;
        string m_PhoneNumber;
        TraceLevel m_Trace;
        static Dictionary<string, List<int>> BaudRates = new Dictionary<string, List<int>>();
        object m_Eop;
        GXSynchronousMediaBase m_syncBase;
        UInt64 m_BytesSent, m_BytesReceived;
        readonly object m_Synchronous = new object();        
		readonly object m_baseLock = new object();
        internal System.IO.Ports.SerialPort m_base = new System.IO.Ports.SerialPort();
        ReceiveThread m_Receiver;
        Thread m_ReceiverThread;

		/// <summary>
		/// Get baud rates supported by given Terminal port.
		/// </summary>
        static public int[] GetAvailableBaudRates(string portName)
        {
            if (BaudRates.ContainsKey(portName.ToLower()))
            {
                return BaudRates[portName.ToLower()].ToArray();
            }
            if (string.IsNullOrEmpty(portName))
            {
                portName = GXTerminal.GetPortNames()[0];                 
            }
            List<int> items = new List<int>();
            BaudRates[portName.ToLower()] = items;
            try
            {
                Int32 value = 0;
                using (SerialPort port = new SerialPort(portName))
                {
                    port.Open();
                    FieldInfo fi = port.BaseStream.GetType().GetField("commProp", BindingFlags.Instance | BindingFlags.NonPublic);
					if (fi != null)
					{
						object p = fi.GetValue(port.BaseStream);
                    	value = (Int32)p.GetType().GetField("dwSettableBaud", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(p);
					}
                }
				if (value != 0)
				{
	                if ((value & 0x1) != 0)
	                {
	                    items.Add(75);
	                }
	                if ((value & 0x2) != 0)
	                {
	                    items.Add(110);
	                }
	                if ((value & 0x8) != 0)
	                {
	                    items.Add(150);
	                }
	                if ((value & 0x10) != 0)
	                {
	                    items.Add(300);
	                }
	                if ((value & 0x20) != 0)
	                {
	                    items.Add(600);
	                }
	                if ((value & 0x40) != 0)
	                {
	                    items.Add(1200);
	                }
	                if ((value & 0x80) != 0)
	                {
	                    items.Add(1800);
	                }
	                if ((value & 0x100) != 0)
	                {
	                    items.Add(2400);
	                }
	                if ((value & 0x200) != 0)
	                {
	                    items.Add(4800);
	                }
	                if ((value & 0x400) != 0)
	                {
	                    items.Add(7200);
	                }
	                if ((value & 0x800) != 0)
	                {
	                    items.Add(9600);
	                }
	                if ((value & 0x1000) != 0)
	                {
	                    items.Add(14400);
	                }
	                if ((value & 0x2000) != 0)
	                {
	                    items.Add(19200);
	                }
	                if ((value & 0x4000) != 0)
	                {
	                    items.Add(38400);
	                }
	                if ((value & 0x8000) != 0)
	                {
	                    items.Add(56000);
	                }
	                if ((value & 0x40000) != 0)
	                {
	                    items.Add(57600);
	                }
	                if ((value & 0x20000) != 0)
	                {
	                    items.Add(115200);
	                }
	                if ((value & 0x10000) != 0)
	                {
	                    items.Add(128000);
	                }
	                if ((value & 0x10000000) != 0) //Programmable baud rate.
	                {
	                    items.Add(0);
	                }
				}
            }
            catch
            {        
				items.Clear ();
            }
			//Add default baud rates.
			if (items.Count == 0)
			{
				items.Add(300);
                items.Add(600);
                items.Add(1800);
                items.Add(2400);
                items.Add(4800);
                items.Add(9600);
                items.Add(19200);
                items.Add(38400);
                items.Add(0); //Programmable baud rate.	
			}
            return items.ToArray();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXTerminal()
        {
            ConfigurableSettings = AvailableMediaSettings.All;
            m_syncBase = new GXSynchronousMediaBase(1024);            
            //Events are not currently implemented in Mono's Terminal port.
            if (Environment.OSVersion.Platform != PlatformID.Unix)
            {
                m_base.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(GXTerminal_DataReceived);
            }         
        }

        internal void NotifyError(Exception ex)
        {
            if (m_OnError != null)
            {
                m_OnError(this, ex);
            }
            if (m_Trace >= TraceLevel.Error && m_OnTrace != null)
            {
                m_OnTrace(this, new TraceEventArgs(TraceTypes.Error, ex, null));
            }
        }

        void NotifyMediaStateChange(MediaState state)
        {
            if (m_Trace >= TraceLevel.Info && m_OnTrace != null)
            {
                m_OnTrace(this, new TraceEventArgs(TraceTypes.Info, state, null));
            }
            if (m_OnMediaStateChange != null)
            {
                m_OnMediaStateChange(this, new MediaStateEventArgs(state));
            }
        }

		/// <summary>
		/// What level of tracing is used.
		/// </summary>
		public TraceLevel Trace
		{
            get
            {
                return m_Trace;
            }
            set
            {
                m_Trace = m_syncBase.Trace = value;
            }
		}

        /// <summary>
        /// Gets the underlying System.IO.Stream object for a System.IO.Ports.SerialPort object.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public Stream BaseStream
        {
            get
            {
                lock (m_baseLock)
                {
                    return m_base.BaseStream;
                }
            }
        }

        private void HandleReceivedData(int index, byte[] buff, int totalCount)
        {
            if (totalCount != 0 && m_Trace == TraceLevel.Verbose && m_OnTrace != null)
            {
                TraceEventArgs arg = new TraceEventArgs(TraceTypes.Received, m_syncBase.m_Received, index, totalCount, null);
                m_OnTrace(this, arg);
            }
            lock (m_syncBase.m_ReceivedSync)
            {
                if (totalCount != 0 && Eop != null) //Search Eop if given.
                {
                    if (Eop is Array)
                    {
                        foreach (object eop in (Array)Eop)
                        {
                            totalCount = GXCommon.IndexOf(m_syncBase.m_Received, GXCommon.GetAsByteArray(eop), index, m_syncBase.m_ReceivedSize);
                            if (totalCount != -1)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        totalCount = GXCommon.IndexOf(m_syncBase.m_Received, GXCommon.GetAsByteArray(Eop), index, m_syncBase.m_ReceivedSize);
                    }
                }
            }
            if (this.IsSynchronous)
            {
                if (totalCount != -1)
                {
                    m_syncBase.m_ReceivedEvent.Set();
                }
            }
            else if (this.m_OnReceived != null)
            {
                if (totalCount != -1)
                {
                    buff = new byte[m_syncBase.m_ReceivedSize];
                    Array.Copy(m_syncBase.m_Received, buff, m_syncBase.m_ReceivedSize);
                    m_syncBase.m_ReceivedSize = 0;
                    m_OnReceived(this, new ReceiveEventArgs(buff, m_base.PortName));
                }
                else
                {
                    m_OnReceived(this, new ReceiveEventArgs(buff, m_base.PortName));
                }
            }
        }

        internal void GXTerminal_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                int count = 0;
                int index = m_syncBase.m_ReceivedSize;
                byte[] buff = null;
                int totalCount = 0;
                while (IsOpen && (count = m_base.BytesToRead) != 0)
                {
                    totalCount += count;
                    buff = new byte[count];
                    m_base.Read(buff, 0, count);
                    m_syncBase.AppendData(buff, 0, count);
                    m_BytesReceived += (uint)count;
                }
                HandleReceivedData(index, buff, totalCount);
            }
            catch (Exception ex)
            {
                if (this.IsSynchronous)
                {
                    m_syncBase.Exception = ex;
                    m_syncBase.m_ReceivedEvent.Set();
                }
                else
                {
                    NotifyError(ex);
                }
            }
        }

		/// <summary>
		/// Used baud rate for communication.
		/// </summary>
		/// <remarks>Can be changed without disconnecting.</remarks>
        [Browsable(true)]
        [DefaultValue(9600)]
        [MonitoringDescription("BaudRate")]
        public int BaudRate 
        {
            get
            {
                if (IsVirtual && m_OnGetPropertyValue != null)
                {
                    string value = m_OnGetPropertyValue("BaudRate");
                    if (value != null)
                    {
                        return int.Parse(value);
                    }
                }
                lock (m_baseLock)
                {
                    return m_base.BaudRate;
                }
            }
            set
            {
                lock (m_baseLock)
                {
                    bool change = m_base.BaudRate != value;
                    m_base.BaudRate = value;
                    if (change)
                    {
                        NotifyPropertyChanged("BaudRate");
                    }
                }
            }
        }

		/// <summary>
		/// True if the port is in a break state; otherwise, false.
		/// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool BreakState
        {
            get
            {
                if (IsVirtual && m_OnGetPropertyValue != null)
                {
                    string value = m_OnGetPropertyValue("BreakState");
                    if (value != null)
                    {
                        return bool.Parse(value);
                    }
                }
				lock(m_baseLock)
				{
	                return m_base.BreakState;
				}
            }
            set
            {                
				bool change;
				lock(m_baseLock)
				{
	                change = m_base.BreakState != value;
	                m_base.BreakState = value;
				}
	            if (change)
                {                    
                    NotifyPropertyChanged("BreakState");
                }
            }
        }

		/// <summary>
		/// Gets the number of bytes in the receive buffer.
		/// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public int BytesToRead
        {
            get
            {
				lock(m_baseLock)
				{
                	return m_base.BytesToRead;
				}
            }
        }
       
		/// <summary>
		/// Gets the number of bytes in the send buffer.
		/// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int BytesToWrite
        {
            get
            {
				lock(m_baseLock)
				{
	                return m_base.BytesToWrite;
				}
            }
        }

		/// <summary>
		/// Gets the state of the Carrier Detect line for the port.
		/// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CDHolding 
        {
            get
            {
				lock(m_baseLock)
				{
                	return m_base.CDHolding;
				}
            }
        }
       
		/// <summary>
		/// Gets the state of the Clear-to-Send line.
		/// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool CtsHolding
        {
            get
            {
				lock(m_baseLock)
				{
                	return m_base.CtsHolding;
				}
            }
        }

		/// <summary>
		/// Gets or sets the standard length of data bits per byte.
		/// </summary>
        [MonitoringDescription("DataBits")]
        [DefaultValue(8)]
        [Browsable(true)]
        public int DataBits
        {
            get
            {
                if (IsVirtual && m_OnGetPropertyValue != null)
                {
                    string value = m_OnGetPropertyValue("DataBits");
                    if (value != null)
                    {
                        return int.Parse(value);
                    }
                }
				lock(m_baseLock)
				{
                	return m_base.DataBits;
				}
            }
            set
            {
                bool change;
				lock(m_baseLock)
				{
					change = m_base.DataBits != value;
                	m_base.DataBits = value;
				}
                if (change)
                {
                    NotifyPropertyChanged("DataBits");
                }
            }
        }
       
		/// <summary>
		/// Gets or sets a value indicating whether null bytes are ignored when transmitted between the port and the receive buffer.
		/// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [MonitoringDescription("DiscardNull")]
        public bool DiscardNull
        {
            get
            {
                if (IsVirtual && m_OnGetPropertyValue != null)
                {
                    string value = m_OnGetPropertyValue("DiscardNull");
                    if (value != null)
                    {
                        return bool.Parse(value);
                    }
                }
				lock(m_baseLock)
				{
                	return m_base.DiscardNull;
				}
            }
            set
            {                
                bool change;
				lock(m_baseLock)
				{
					change = m_base.DiscardNull != value;
                	m_base.DiscardNull = value;
				}
                if (change)
                {
                    NotifyPropertyChanged("DiscardNull");
                }
            }
        }

		/// <summary>
		/// Gets the state of the Data Set Ready (DSR) signal.
		/// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool DsrHolding
        {
            get
            {
				lock(m_baseLock)
				{
                	return m_base.DsrHolding;
				}
            }
        }
       
		/// <summary>
		/// Gets or sets a value that enables the Data Terminal Ready (DTR) signal during Terminal communication.
		/// </summary>
        [DefaultValue(false)]
        [MonitoringDescription("DtrEnable")]
        [Browsable(true)]
        public bool DtrEnable
        {
            get
            {
                if (IsVirtual && m_OnGetPropertyValue != null)
                {
                    string value = m_OnGetPropertyValue("DtrEnable");
                    if (value != null)
                    {
                        return bool.Parse(value);
                    }
                }
				lock(m_baseLock)
				{
                	return m_base.DtrEnable;
				}
            }
            set
            {                
                bool change;
				lock(m_baseLock)
				{
					change = m_base.DtrEnable != value;
                	m_base.DtrEnable = value;
				}
                if (change)
                {
                    NotifyPropertyChanged("DtrEnable");
                }
            }
        }
       
		/// <summary>
		/// Gets or sets the byte encoding for pre- and post-transmission conversion of text.
		/// </summary>
        [MonitoringDescription("Encoding")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public Encoding Encoding
        {
            get
            {
				lock(m_baseLock)
				{
                	return m_base.Encoding;
				}
            }
            set
            {                
                bool change;
				lock(m_baseLock)
				{
					change = m_base.Encoding != value;
                	m_base.Encoding = value;
				}
                if (change)
                {
                    NotifyPropertyChanged("Encoding");
                }
            }
        }
        
		/// <summary>
		/// Gets or sets the handshaking protocol for Terminal port transmission of data.
		/// </summary>
        [Browsable(true)]
        [MonitoringDescription("Handshake")]
        public Handshake Handshake
        {
            get
            {
                if (IsVirtual && m_OnGetPropertyValue != null)
                {
                    string value = m_OnGetPropertyValue("Handshake");
                    if (value != null)
                    {
                        return (Handshake)int.Parse(value);
                    }
                }
				lock(m_baseLock)
				{
                	return m_base.Handshake;
				}
            }
            set
            {                
                bool change;
				lock(m_baseLock)
				{
					change = m_base.Handshake != value;
                	m_base.Handshake = value;
				}
                if (change)
                {
                    NotifyPropertyChanged("Handshake");
                }
            }
        }
        
		/// <summary>
		/// Gets a value indicating the open or closed status of the System.IO.Ports.SerialPort object.
		/// </summary>
        [Browsable(false)]
        public bool IsOpen
        {
            get
            {
				lock(m_baseLock)
				{
                	return m_base.IsOpen;
				}
            }            
        }

		/// <summary>
		/// Gets or sets the parity-checking protocol.
		/// </summary>
        [Browsable(true)]
        [MonitoringDescription("Parity")]
        public Parity Parity
        {
            get
            {
                if (IsVirtual && m_OnGetPropertyValue != null)
                {
                    string value = m_OnGetPropertyValue("Parity");
                    if (value != null)
                    {
                        return (Parity)int.Parse(value);
                    }
                }
				lock(m_baseLock)
				{
                	return m_base.Parity;
				}
            }
            set
            {                
                bool change;
				lock(m_baseLock)
				{
					change = m_base.Parity != value;
                	m_base.Parity = value;
				}
                if (change)
                {
                    NotifyPropertyChanged("Parity");
                }
            }
        }

		/// <summary>
		/// Gets or sets the byte that replaces invalid bytes in a data stream when a parity error occurs.
		/// </summary>
        [Browsable(true)]
        [MonitoringDescription("ParityReplace")]
        [DefaultValue(63)]
        public byte ParityReplace
        {
            get
            {
                if (IsVirtual && m_OnGetPropertyValue != null)
                {
                    string value = m_OnGetPropertyValue("ParityReplace");
                    if (value != null)
                    {
                        return byte.Parse(value);
                    }
                }
				lock(m_baseLock)
				{
                	return m_base.ParityReplace;
				}
            }
            set
            {                
                bool change;
				lock(m_baseLock)
				{
					change = m_base.ParityReplace != value;
                	m_base.ParityReplace = value;
				}
                if (change)
                {
                    NotifyPropertyChanged("ParityReplace");
                }
            }
        }        

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        public string PhoneNumber
        {
            get
            {
                if (IsVirtual && m_OnGetPropertyValue != null)
                {
                    string value = m_OnGetPropertyValue("PhoneNumber");
                    if (value != null)
                    {
                        return value;
                    }
                }
                lock (m_baseLock)
                {
                    return m_PhoneNumber;
                }
            }
            set
            {
                bool change;
                lock (m_baseLock)
                {
                    change = m_PhoneNumber != value;
                    m_PhoneNumber = value;
                }
                if (change)
                {
                    NotifyPropertyChanged("PhoneNumber");
                }
            }
        }

        /// <summary>
        /// Gets or sets how long (ms.) modem answer is waited when connection is made.
        /// </summary>
        public int ConnectionWaitTime
        {
            get
            {
                if (IsVirtual && m_OnGetPropertyValue != null)
                {
                    string value = m_OnGetPropertyValue("ConnectionWaitTime");
                    if (value != null)
                    {
                        return int.Parse(value);
                    }
                }
                lock (m_baseLock)
                {
                    return m_ConnectionWaitTime;
                }
            }
            set
            {
                bool change;
                lock (m_baseLock)
                {
                    change = m_ConnectionWaitTime != value;
                    m_ConnectionWaitTime = value;
                }
                if (change)
                {
                    NotifyPropertyChanged("ConnectionWaitTime");
                }
            }
        }

        /// <summary>
        /// Get or set how long (ms) modem answer is waited when command is send for the modem.
        /// </summary>
        public int CommandWaitTime
        {
            get
            {
                if (IsVirtual && m_OnGetPropertyValue != null)
                {
                    string value = m_OnGetPropertyValue("CommandWaitTime");
                    if (value != null)
                    {
                        return int.Parse(value);
                    }
                }
                lock (m_baseLock)
                {
                    return m_CommandWaitTime;
                }
            }
            set
            {
                bool change;
                lock (m_baseLock)
                {
                    change = m_CommandWaitTime != value;
                    m_CommandWaitTime = value;
                }
                if (change)
                {
                    NotifyPropertyChanged("CommandWaitTime");
                }
            }
        }  


        /// <summary>
        /// Server sunctionality is added later.
        /// </summary>
        [DefaultValue(false)]
        [Browsable(true)]
        internal bool Server
        {
            get
            {
                if (IsVirtual && m_OnGetPropertyValue != null)
                {
                    string value = m_OnGetPropertyValue("Server");
                    if (value != null)
                    {
                        return bool.Parse(value);
                    }
                }
                lock (m_baseLock)
                {
                    return m_Server;
                }
            }
            set
            {
                bool change = m_Server != value;                
                if (change)
                {
                    m_Server = value;
                    NotifyPropertyChanged("Server");
                }
            }
        }
       
		/// <summary>
		/// Gets or sets the port for communications, including but not limited to all available COM ports.
		/// </summary>
        [MonitoringDescription("PortName")]
        [Browsable(true)]
        [DefaultValue("COM1")]
        public string PortName
        {
            get
            {
                if (IsVirtual && m_OnGetPropertyValue != null)
                {
                    string value = m_OnGetPropertyValue("PortName");
                    if (value != null)
                    {
                        return value;
                    }
                }
				lock(m_baseLock)
				{
                	return m_base.PortName;
				}
            }
            set
            {               
                bool change;
				lock(m_baseLock)
				{
					change = m_base.PortName != value;
                	m_base.PortName = value;
				}
                if (change)
                {
                    NotifyPropertyChanged("PortName");
                }
            }
        }
        
		/// <summary>
		/// Gets or sets the size of the System.IO.Ports.SerialPort input buffer.
		/// </summary>
        [DefaultValue(4096)]
        [MonitoringDescription("ReadBufferSize")]
        [Browsable(true)]
        public int ReadBufferSize
        {
            get
            {
                if (IsVirtual && m_OnGetPropertyValue != null)
                {
                    string value = m_OnGetPropertyValue("ReadBufferSize");
                    if (value != null)
                    {
                        return int.Parse(value);
                    }
                }
				lock(m_baseLock)
				{
                	return m_base.ReadBufferSize;
				}
            }
            set
            {                
                bool change;
				lock(m_baseLock)
				{
					change = m_base.ReadBufferSize != value;
                	m_base.ReadBufferSize = value;
				}
                if (change)
                {
                    NotifyPropertyChanged("ReadBufferSize");
                }
            }
        }
       
		/// <summary>
		/// Gets or sets the number of milliseconds before a time-out occurs when a read operation does not finish.
		/// </summary>
        [MonitoringDescription("ReadTimeout")]
        [Browsable(true)]
        [DefaultValue(-1)]
        public int ReadTimeout
        {
            get
            {
                if (IsVirtual && m_OnGetPropertyValue != null)
                {
                    string value = m_OnGetPropertyValue("ReadTimeout");
                    if (value != null)
                    {
                        return int.Parse(value);
                    }
                }
				lock(m_baseLock)
				{
                	return m_base.ReadTimeout;
				}
            }
            set
            {                
                bool change;
				lock(m_baseLock)
				{
					change = m_base.ReadTimeout != value;
                	m_base.ReadTimeout = value;
				}
                if (change)
                {
                    NotifyPropertyChanged("ReadTimeout");
                }
            }
        }

		/// <summary>
		/// Gets or sets the number of bytes in the internal input buffer before a System.IO.Ports.SerialPort.DataReceived event occurs.
		/// </summary>
        [MonitoringDescription("ReceivedBytesThreshold")]
        [DefaultValue(1)]
        [Browsable(true)]
        public int ReceivedBytesThreshold
        {
            get
            {
                if (IsVirtual && m_OnGetPropertyValue != null)
                {
                    string value = m_OnGetPropertyValue("ReceivedBytesThreshold");
                    if (value != null)
                    {
                        return int.Parse(value);
                    }
                }
				lock(m_baseLock)
				{
                	return m_base.ReceivedBytesThreshold;
				}
            }
            set
            {                
                bool change;
				lock(m_baseLock)
				{
					change = m_base.ReceivedBytesThreshold != value;
                	m_base.ReceivedBytesThreshold = value;
				}
                if (change)
                {
                    NotifyPropertyChanged("ReceivedBytesThreshold");
                }
            }
        }

		/// <summary>
		/// Gets or sets a value indicating whether the Request to Send (RTS) signal is enabled during Terminal communication.
		/// </summary>
        [MonitoringDescription("RtsEnable")]
        [DefaultValue(false)]
        [Browsable(true)]
        public bool RtsEnable
        {
            get
            {
                if (IsVirtual && m_OnGetPropertyValue != null)
                {
                    string value = m_OnGetPropertyValue("RtsEnable");
                    if (value != null)
                    {
                        return bool.Parse(value);
                    }
                }
				lock(m_baseLock)
				{
                	return m_base.RtsEnable;
				}
            }
            set
            {                
                bool change;
				lock(m_baseLock)
				{
					change = m_base.RtsEnable != value;
                	m_base.RtsEnable = value;
				}
                if (change)
                {
                    NotifyPropertyChanged("RtsEnable");
                }
            }
        }
       
		/// <summary>
		/// Gets or sets the standard number of stopbits per byte.
		/// </summary>
        [MonitoringDescription("StopBits")]
        [Browsable(true)]
        public StopBits StopBits
        {
            get
            {
                if (IsVirtual && m_OnGetPropertyValue != null)
                {
                    string value = m_OnGetPropertyValue("StopBits");
                    if (value != null)
                    {
                        return (StopBits)int.Parse(value);
                    }
                }
				lock(m_baseLock)
				{
                	return m_base.StopBits;
				}
            }
            set
            {                
                bool change;
				lock(m_baseLock)
				{
					change = m_base.StopBits != value;
                	m_base.StopBits = value;
				}
                if (change)
                {
                    NotifyPropertyChanged("StopBits");
                }
            }
        }
        
		/// <summary>
		/// Gets or sets the size of the Terminal port output buffer.
		/// </summary>
        [Browsable(true)]
        [DefaultValue(2048)]
        [MonitoringDescription("WriteBufferSize")]
        public int WriteBufferSize
        {
            get
            {
                if (IsVirtual && m_OnGetPropertyValue != null)
                {
                    string value = m_OnGetPropertyValue("WriteBufferSize");
                    if (value != null)
                    {
                        return int.Parse(value);
                    }
                }
				lock(m_baseLock)
				{
                	return m_base.WriteBufferSize;
				}
            }
            set
            {
                bool change;
				lock(m_baseLock)
				{
					change = m_base.WriteBufferSize != value;
                	m_base.WriteBufferSize = value;
				}
                if (change)
                {
                    NotifyPropertyChanged("WriteBufferSize");
                }
            }
        }
        
		/// <summary>
		/// Gets or sets the number of milliseconds before a time-out occurs when a write operation does not finish.
		/// </summary>
        [MonitoringDescription("WriteTimeout")]
        [Browsable(true)]
        [DefaultValue(-1)]
        public int WriteTimeout         
        {
            get
            {
                if (IsVirtual && m_OnGetPropertyValue != null)
                {
                    string value = m_OnGetPropertyValue("WriteTimeout");
                    if (value != null)
                    {
                        return int.Parse(value);
                    }
                }
				lock(m_baseLock)
				{
                	return m_base.WriteTimeout;
				}
            }
            set
            {                
                bool change;
				lock(m_baseLock)
				{
					change = m_base.WriteTimeout != value;
                	m_base.WriteTimeout = value;
				}
                if (change)
                {
                    NotifyPropertyChanged("WriteTimeout");
                }
            }
        }

		/// <summary>
		/// Closes the port connection, sets the System.IO.Ports.SerialPort.IsOpen property to false, and disposes of the internal System.IO.Stream object.
		/// </summary>
        public void Close()
        {
            bool bOpen = VirtualOpen;
            if (!IsVirtual)
            {
                lock (m_baseLock)
                {
                    bOpen = m_base.IsOpen;
                }
            }
            if (bOpen)
            {
                try
                {
                    NotifyMediaStateChange(MediaState.Closing);
                }
                catch (Exception ex)
                {
                    NotifyError(ex);
                    throw;
                }
                finally
                {
                    try
                    {
                        if (!IsVirtual)
                        {
                            //
                            if (Progress != Progress.None)
                            {
                                try
                                {
                                    //Send AT
                                    lock (Synchronous)
                                    {
                                        if (Progress == Progress.Connected)
                                        {
                                            //We are not expecting reply.
                                            Thread.Sleep(1000);
                                            Gurux.Common.ReceiveParameters<string> p = new Gurux.Common.ReceiveParameters<string>()
                                            {
                                                WaitTime = m_ConnectionWaitTime,
                                                Count = 3
                                            };
                                            SendBytes(ASCIIEncoding.ASCII.GetBytes("+++"));
                                            //It's OK if this fails.
                                            Receive(p);
                                            SendCommand("ATH0\r", m_ConnectionWaitTime, null, false);
                                        }
                                    }
                                }
                                finally
                                {
                                    Progress = Progress.None;
                                }
                            }
                            if (m_Receiver != null)
                            {
                                m_Receiver.Closing.Set();
                            }
                            lock (m_baseLock)
                            {
                                m_base.Close();
                            }
                            if (m_ReceiverThread != null && m_ReceiverThread.IsAlive)
                            {
                                m_ReceiverThread.Join();
                            }
                        }
                    }
                    catch
                    {
                        //Ignore all errors on close.
                    }
                    VirtualOpen = false;
                    NotifyMediaStateChange(MediaState.Closed);
                }
            }
        }

		/// <summary>
		/// Discards data from the Terminal driver's receive buffer.
		/// </summary>
        public void DiscardInBuffer()
        {
			lock(m_baseLock)
			{
            	m_base.DiscardInBuffer();
			}
        }
        
		/// <summary>
		/// Discards data from the Terminal driver's transmit buffer.
		/// </summary>
		public void DiscardOutBuffer()
        {
			lock(m_baseLock)
			{
            	m_base.DiscardOutBuffer();
			}
        }

		/// <summary>
		/// Gets an array of Terminal port names for the current computer.
		/// </summary>
		/// <returns></returns>
        public static string[] GetPortNames()
        {
           	return System.IO.Ports.SerialPort.GetPortNames();
        }

		/// <summary>
		/// User defined available ports.
		/// </summary>
		/// <remarks>If this is not set ports are retrieved from current system.</remarks>
		public string[] AvailablePorts
		{
			get;
			set;
		}

		/// <summary>
		/// Opens a new Terminal port connection.
		/// </summary>
        /// <remarks>
        /// If connection is succeeded but Modem data is not move try to set following:
        /// DTE/Modem flow control
        /// AT&amp;K0  Disable flow control.
        /// AT&amp;Q0  Direct Asynchronous mode
        /// </remarks>
        public void Open()
        {
            Close();
            try
            {
                lock (m_syncBase.m_ReceivedSync)
                {
                    m_syncBase.m_LastPosition = 0;
                }
                NotifyMediaStateChange(MediaState.Opening);
                if (m_Trace >= TraceLevel.Info && m_OnTrace != null)
                {
                    string eop = Resources.None;
                    if (m_Eop is byte[])
                    {
                        eop = BitConverter.ToString(m_Eop as byte[], 0);
                    }
                    else if (m_Eop != null)
                    {
                        eop = m_Eop.ToString();
                    }
                    string str = string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} {11} {12} ", 
                        Resources.Settings,
                        Resources.Port,
                        m_base.PortName,
                        Resources.BaudRate,
                        m_base.BaudRate,
                        Resources.DataBits,
                        m_base.DataBits,
                        Resources.Parity,
                        m_base.Parity,
                        Resources.StopBits,
                        m_base.StopBits,
                        Resources.Eop,
                        eop);
                    m_OnTrace(this, new TraceEventArgs(TraceTypes.Info, str, null));
                }
				lock(m_baseLock)
				{
                	m_base.Open();
				}
                //Events are not currently implemented in Mono's Terminal port.
                if (!IsVirtual && Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    m_Receiver = new ReceiveThread(this);
                    m_ReceiverThread = new Thread(new ThreadStart(m_Receiver.Receive));
                    m_ReceiverThread.IsBackground = true;
                    m_ReceiverThread.Start();                    
                }                
                try
                {
                    this.DtrEnable = this.RtsEnable = true; 
                    //Send AT
                    lock (Synchronous)
                    {
                        if (InitializeCommands != null)
                        {
                            foreach (string it in InitializeCommands)
                            {
                                SendCommand(it + "\r\n", m_CommandWaitTime, null, true);
                            }
                        }
                        string reply;
                        if (this.Server)
                        {
                            if (string.Compare(SendCommand("AT\r\n", m_CommandWaitTime, null, false), "OK", true) != 0)
                            {                                
                                reply = SendCommand("AT\r\n", m_CommandWaitTime, null, true);
                                if (string.Compare(reply, "AT\r\r\n", true) != 0)
                                {
                                    throw new Exception(Resources.InvalidReply);
                                }
                            }
                            reply = SendCommand("ATA\r\n", m_CommandWaitTime, null, true);
                            if (string.Compare(reply, "ATA", true) != 0)
                            {
                                throw new Exception(Resources.InvalidReply);
                            }
                            Progress = Progress.Connecting;
                        }
                        else
                        {
                            //Send AT
                            if (string.Compare(SendCommand("AT\r", m_CommandWaitTime, null, false), "OK", true) != 0)
                            {                                
                                reply = SendCommand("AT\r", m_CommandWaitTime, null, true);
                                if (string.Compare(reply, "OK", true) != 0)
                                {
                                    throw new Exception(Resources.InvalidReply);
                                }
                            }                           
                            Progress = Progress.Connecting;
                            reply = SendCommand("ATD" + PhoneNumber + "\r\n", m_ConnectionWaitTime, null, true);
                            Progress = Progress.Connected;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Close();
                    throw ex;
                }
                NotifyMediaStateChange(MediaState.Open);
            }
            catch
            {
                Close();
                throw;
            }
        }
        
        void SendBytes(byte[] value)
        {
            lock (m_baseLock)
            {
                if (m_Trace == TraceLevel.Verbose && m_OnTrace != null)
                {
                    m_OnTrace(this, new TraceEventArgs(TraceTypes.Sent, value, null));
                }
                m_BytesSent += (uint)value.Length;
                //Reset last position if Eop is used.
                lock (m_syncBase.m_ReceivedSync)
                {
                    m_syncBase.m_LastPosition = 0;
                }
                m_base.Write(value, 0, value.Length);
            }
        }

        string SendCommand(string cmd, int wt, string eop, bool throwError)
        {
            Gurux.Common.ReceiveParameters<string> p = new Gurux.Common.ReceiveParameters<string>()
            {
                WaitTime = wt,
                Eop = eop == null ? "\r\n" : eop
            };
            if (p.Eop.Equals(""))
            {
                p.Eop = null;
                p.Count = cmd.Length;
            }
            SendBytes(ASCIIEncoding.ASCII.GetBytes(cmd));
            StringBuilder sb = new StringBuilder();
            int index = -1;
            string reply = "";
            while (index == -1)
            {
                if (!Receive(p))
                {
                    if (throwError)
                    {
                        throw new Exception(Resources.FailedToReceiveAnswerFromTheModemCheckSerialPort);
                    }
                    return "";
                }
                sb.Append(p.Reply);
                reply = sb.ToString();
                //Remove echo.                
                if (sb.Length >= cmd.Length && reply.StartsWith(cmd))
                {
                    sb.Remove(0, cmd.Length);
                    reply = sb.ToString();
                    //Remove echo and return if we are not expecting reply.
                    if (eop == "")
                    {
                        return "";
                    }
                }
                if (eop != null)
                {
                    index = reply.LastIndexOf(eop);
                }
                else if (reply.Length > 5)
                {
                    index = reply.LastIndexOf("\r\nOK\r\n");
                    if (index == -1)
                    {
                        index = reply.LastIndexOf("ERROR:");
                        if (index == -1)
                        {
                            index = reply.LastIndexOf("CONNECT");
                            if (index == -1)
                            {
                                index = reply.LastIndexOf("NO CARRIER");
                                if (index != -1)
                                {
                                    string str = Resources.ConnectionFailedNoCarrierWhenTelephoneCallWasBeingEstablished;
                                    int start = reply.IndexOf("CAUSE:");
                                    if (start != -1)
                                    {
                                        if (start < index)
                                        {
                                            str += reply.Substring(start, index - start).Trim();
                                        }
                                        else
                                        {
                                            str += reply.Substring(start).Trim();
                                        }
                                    }
                                    str += Environment.NewLine + SendCommand("AT+CEER\r", wt, null, false);                                    
                                    throw new Exception(str);
                                }
                                if (reply.LastIndexOf("ERROR") != -1)
                                {
                                    throw new Exception(Resources.ConnectionFailedErrorWhenTelephoneCallWasBeingEstablished);
                                }
                                if (reply.LastIndexOf("BUSY") != -1)
                                {
                                    throw new Exception(Resources.ConnectionFailedBusyWhenTelephoneCallWasBeingEstablished);
                                }
                            }
                        }
                    }
                    //If there is a message before OK show it.
                    else if (index != 0)
                    {
                        reply = reply.Remove(index);
                        index = 0;
                    }
                }
                p.Reply = null;
            }
            if (index != 0 & eop == null)
            {
                reply = reply.Remove(0, index);
            }
            reply = reply.Trim();
            return reply;
        }

        #region Events
        /// <summary>
        /// GXNet component sends received data through this method.
        /// </summary>
        [Description("GXNet component sends received data through this method.")]
        public event ReceivedEventHandler OnReceived
        {
            add
            {
                m_OnReceived += value;
            }
            remove
            {
                m_OnReceived -= value;
            }
        }

        /// <summary>
        /// Errors that occur after the connection is established, are sent through this method. 
        /// </summary>       
        [Description("Errors that occur after the connection is established, are sent through this method.")]
        public event Gurux.Common.ErrorEventHandler OnError
        {
            add
            {

                m_OnError += value;
            }
            remove
            {
                m_OnError -= value;
            }
        }

        /// <summary>
        /// Media component sends notification, when its state changes.
        /// </summary>       
        [Description("Media component sends notification, when its state changes.")]
        public event MediaStateChangeEventHandler OnMediaStateChange
        {
            add
            {
                m_OnMediaStateChange += value;
            }
            remove
            {
                m_OnMediaStateChange -= value;
            }
        }

        /// <summary>
        /// Called when the client is establishing a connection with a Net Server.
        /// </summary>
        [Description("Called when the client is establishing a connection with a Net Server.")]
        public event ClientConnectedEventHandler OnClientConnected
        {
            add
            {
                m_OnClientConnected += value;
            }
            remove
            {
                m_OnClientConnected -= value;
            }
        }

        /// <summary>
        /// Called when the client has been disconnected from the network server.
        /// </summary>
        [Description("Called when the client has been disconnected from the network server.")]
        public event ClientDisconnectedEventHandler OnClientDisconnected
        {
            add
            {
                m_OnClientDisconnected += value;
            }
            remove
            {
                m_OnClientDisconnected -= value;
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                m_OnPropertyChanged += value;
            }
            remove
            {
                m_OnPropertyChanged -= value;
            }
        }
        
        /// <inheritdoc cref="TraceEventHandler"/>
        [Description("Called when the Media is sending or receiving data.")]
        public event TraceEventHandler OnTrace
        {
            add
            {
                m_OnTrace += value;
            }
            remove
            {
                m_OnTrace -= value;
            }
        }        

        private void NotifyPropertyChanged(String info)
        {
            if (m_OnPropertyChanged != null)
            {
                m_OnPropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }


        //Events
        TraceEventHandler m_OnTrace;
        PropertyChangedEventHandler m_OnPropertyChanged;
        MediaStateChangeEventHandler m_OnMediaStateChange;
        ClientConnectedEventHandler m_OnClientConnected;
        ClientDisconnectedEventHandler m_OnClientDisconnected;
        internal Gurux.Common.ErrorEventHandler m_OnError;
        ReceivedEventHandler m_OnReceived;
        GetPropertyValueEventHandler m_OnGetPropertyValue;
        ReceivedEventHandler m_OnDataSend;

        #endregion //Events

        /// <inheritdoc cref="IGXMedia.ConfigurableSettings"/>
        public AvailableMediaSettings ConfigurableSettings
        {
            get
            {
                return (Gurux.Terminal.AvailableMediaSettings)((IGXMedia)this).ConfigurableSettings;
            }
            set
            {
                ((IGXMedia)this).ConfigurableSettings = (int)value;
            }
        }

        /// <inheritdoc cref="IGXMedia.Tag"/>
        public object Tag
        {
            get;
            set;
        }

        /// <inheritdoc cref="IGXMedia.MediaContainer"/>
        IGXMediaContainer IGXMedia.MediaContainer
        {
            get;
            set;
        }

        /// <inheritdoc cref="IGXMedia.SyncRoot"/>
        [Browsable(false), ReadOnly(true)]
        public object SyncRoot
        {
            get
            {
                //In some special cases when binary serialization is used this might be null
                //after deserialize. Just set it.
                if (m_sync == null)
                {
                    m_sync = new object();
                }
                return m_sync;
            }
        }

        /// <inheritdoc cref="IGXVirtualMedia.Virtual"/>
        bool IGXVirtualMedia.Virtual
        {
            get
            {
                return IsVirtual;
            }
            set
            {
                IsVirtual = value;
            }
        }

        /// <summary>
        /// Occurs when a property value is asked.
        /// </summary>
        event GetPropertyValueEventHandler IGXVirtualMedia.OnGetPropertyValue
        {
            add
            {
                m_OnGetPropertyValue += value;
            }
            remove
            {
                m_OnGetPropertyValue -= value;
            }
        }

        /// <summary>
        /// Occurs when data is sent on virtual mode.
        /// </summary>
        event ReceivedEventHandler IGXVirtualMedia.OnDataSend
        {
            add
            {
                m_OnDataSend += value;
            }
            remove
            {
                m_OnDataSend -= value;
            }
        }        

        /// <summary>
        /// Called when new data is received to the virtual media.
        /// </summary>
        /// <param name="data">received data</param>
        /// <param name="sender">Data sender.</param>
        void IGXVirtualMedia.DataReceived(byte[] data, string sender)
        {
            int index = m_syncBase.m_ReceivedSize;
            m_syncBase.AppendData(data, 0, data.Length);
            m_BytesReceived += (uint)data.Length;
            HandleReceivedData(index, data, data.Length);
        }


        /// <inheritdoc cref="IGXMedia.Synchronous"/>
        public object Synchronous
        {
            get
            {
                return m_Synchronous;
            }
        }

        /// <inheritdoc cref="IGXMedia.IsSynchronous"/>
        public bool IsSynchronous
        {
            get
            {
                bool reserved = System.Threading.Monitor.TryEnter(m_Synchronous, 0);
                if (reserved)
                {
                    System.Threading.Monitor.Exit(m_Synchronous);
                }
                return !reserved;
            }
        }

        /// <inheritdoc cref="IGXMedia.ResetSynchronousBuffer"/>
        public void ResetSynchronousBuffer()
        {
            lock (m_syncBase.m_ReceivedSync)
            {
                m_syncBase.m_ReceivedSize = 0;
            }
        }

        #region IGXMedia Members

        /// <summary>
        /// Sent byte count.
        /// </summary>
        /// <seealso cref="BytesReceived">BytesReceived</seealso>
        /// <seealso cref="ResetByteCounters">ResetByteCounters</seealso>
        [Browsable(false)]
        public UInt64 BytesSent
        {
            get
            {
                return m_BytesSent;
            }
        }

        /// <summary>
        /// Received byte count.
        /// </summary>
        /// <seealso cref="BytesSent">BytesSent</seealso>
        /// <seealso cref="ResetByteCounters">ResetByteCounters</seealso>
        [Browsable(false)]
        public UInt64 BytesReceived
        {
            get
            {
                return m_BytesReceived;
            }
        }

        /// <summary>
        /// Resets BytesReceived and BytesSent counters.
        /// </summary>
        /// <seealso cref="BytesSent">BytesSent</seealso>
        /// <seealso cref="BytesReceived">BytesReceived</seealso>
        public void ResetByteCounters()
        {
            m_BytesSent = m_BytesReceived = 0;
        }   

        void Gurux.Common.IGXMedia.Copy(object target)
        {
            GXTerminal Target = (GXTerminal)target;
            m_base.BaudRate = Target.m_base.BaudRate;
            m_base.StopBits = Target.m_base.StopBits;
            m_base.Parity = Target.m_base.Parity;
            m_base.DataBits = Target.m_base.DataBits;
        }

        /// <inheritdoc cref="IGXMedia.Eop"/>
        public object Eop
        {
            get
            {
                if (IsVirtual && m_OnGetPropertyValue != null)
                {
                    string value = m_OnGetPropertyValue("Eop");
                    if (value != null)
                    {
                        return int.Parse(value);
                    }
                }
                return m_Eop;
            }
            set
            {                 
                bool change = m_Eop != value;
                m_Eop = value;
                if (change)
                {
                    NotifyPropertyChanged("Eop");
                }
            }
        }

        /// <summary>
        /// Modem initial settings.
        /// </summary>
        string[] InitializeCommands
        {
            get;
            set;
        }

        /// <summary>
        /// Media settings as a XML string.
        /// </summary>
        public string Settings
        {
            get
            {
                string tmp = "";
                if (!string.IsNullOrEmpty(m_PhoneNumber))
                {
                    tmp += "<Number>" + m_PhoneNumber + "</Number>";
                }
                if (Server)
                {
                    tmp += "<Server>1</Server>";
                }
                if (!string.IsNullOrEmpty(m_base.PortName))
                {
                    tmp += "<Port>" + m_base.PortName + "</Port>";
                }
                if (m_base.BaudRate != 9600)
                {
                    tmp += "<Bps>" + m_base.BaudRate + "</Bps>";
                }
                if (m_base.StopBits != System.IO.Ports.StopBits.None)
                {
                    tmp += "<StopBits>" + (int)m_base.StopBits + "</StopBits>";
                }
                if (m_base.Parity != System.IO.Ports.Parity.None)
                {
                    tmp += "<Parity>" + (int)m_base.Parity + "</Parity>";
                }
                if (m_base.DataBits != 8)
                {
                    tmp += "<ByteSize>" + m_base.DataBits + "</ByteSize>";
                }
                if (this.InitializeCommands != null)
                {
                    tmp += "<Init>" + string.Join(";", InitializeCommands) + "</Init>";
                }
                return tmp;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.ConformanceLevel = ConformanceLevel.Fragment;
                    string str;
                    int result;
                    using (XmlReader xmlReader = XmlReader.Create(new System.IO.StringReader(value), settings))
                    {
                        while (xmlReader.Read())
                        {
                            if (xmlReader.IsStartElement())
                            {
                                switch (xmlReader.Name)
                                {
                                    case "Init":
                                        InitializeCommands = xmlReader.ReadString().Split(new char[]{';'});
                                        break;
                                    case "Number":
                                        m_PhoneNumber = xmlReader.ReadString();
                                        break;
                                    case "Server":
                                        m_Server = Convert.ToInt32(xmlReader.ReadString()) != 0;
                                        break;
                                    case "Port":
                                        m_base.PortName = xmlReader.ReadString();
                                        break;
                                    case "Bps":
                                        m_base.BaudRate = Convert.ToInt32(xmlReader.ReadString());
                                        break;
                                    case "StopBits":
                                        str = xmlReader.ReadString();
                                        if (int.TryParse(str, out result))
                                        {
                                            m_base.StopBits = (StopBits)result;
                                        }
                                        else
                                        {
                                            m_base.StopBits = (StopBits)Enum.Parse(typeof(StopBits), str);
                                        }

                                        break;
                                    case "Parity":
                                        str = xmlReader.ReadString();
                                        if (int.TryParse(str, out result))
                                        {
                                            m_base.Parity = (Parity)result;
                                        }
                                        else
                                        {
                                            m_base.Parity = (Parity)Enum.Parse(typeof(System.IO.Ports.Parity), str);
                                        }
                                        break;
                                    case "ByteSize":
                                        m_base.DataBits = Convert.ToInt32(xmlReader.ReadString());
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }       

        string Gurux.Common.IGXMedia.MediaType
        {
            get
            {
                return "Terminal";
            }
        }

        bool Gurux.Common.IGXMedia.Enabled
        {
            get
            {
                return GetPortNames().Length != 0;
            }
        }

        string Gurux.Common.IGXMedia.Name
        {
            get 
            {
                return m_base.PortName;
            }
        }

        /// <summary>
        /// Shows the serial port Properties dialog.
        /// </summary>
        /// <param name="parent">Owner window of the Properties dialog.</param>
        /// <returns>True, if the user has accepted the changes.</returns>
        /// <seealso cref="PortName">PortName</seealso>
        /// <seealso cref="BaudRate">BaudRate</seealso>
        /// <seealso cref="DataBits">DataBits</seealso>
        /// <seealso href="PropertiesDialog.html">Properties Dialog</seealso>
        public bool Properties(Form parent)
        {            
            return new Gurux.Shared.PropertiesForm(this.PropertiesForm, Gurux.Terminal.Properties.Resources.SettingsTxt, IsOpen).ShowDialog(parent) == DialogResult.OK;
        }

		/// <summary>
		/// Sends data asynchronously. <br/>
		/// No reply from the receiver, whether or not the operation was successful, is expected.
		/// </summary>
        public void Send(object data)
        {
            ((Gurux.Common.IGXMedia)this).Send(data, null);
        }

		/// <summary>
		/// Returns a new instance of the Settings form.
		/// </summary>
        public System.Windows.Forms.Form PropertiesForm
        {
            get
            {
                return new Settings(this);
            }
        }

        /// <inheritdoc cref="IGXMedia.Receive"/>        
        public bool Receive<T>(Gurux.Common.ReceiveParameters<T> args)
        {
            return m_syncBase.Receive(args);
        }

        void Gurux.Common.IGXMedia.Send(object data, string receiver)
        {			
            byte[] value = GXCommon.GetAsByteArray(data);			
			lock(m_baseLock)
			{
                if (m_Trace == TraceLevel.Verbose && m_OnTrace != null)
                {
                    m_OnTrace(this, new TraceEventArgs(TraceTypes.Sent, value, receiver));
                }
            	m_BytesSent += (uint) value.Length;
                //Reset last position if Eop is used.
                lock (m_syncBase.m_ReceivedSync)
                {
                    m_syncBase.m_LastPosition = 0;
                }
                if (!IsVirtual)
                {
                    m_base.Write(value, 0, value.Length);
                }
                else
                {
                    m_OnDataSend(this, new ReceiveEventArgs(data, receiver));
                }
			}
        }

        /// <inheritdoc cref="IGXMedia.Validate"/>
        public void Validate()
        {
            
        }

        int Gurux.Common.IGXMedia.ConfigurableSettings
        {
            get;
            set;
        }

        #endregion

        #region IDisposable Members

		/// <summary>
		/// Closes the connection.
		/// </summary>
        public void Dispose()
        {
            if (this.IsOpen)
            {
                Close();
            }            
        }

        #endregion        
    }
}
