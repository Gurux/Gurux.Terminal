//
//--------------------------------------------------------------------------
//  Gurux Ltd
// 
//
//
// Filename:        $HeadURL: svn://utopia/projects/Old/GuruxTerminal/GXTerminal%20csharp%20Sample/Form1.cs $
//
// Version:         $Revision: 3104 $,
//                  $Date: 2010-12-03 13:43:16 +0200 (pe, 03 joulu 2010) $
//                  $Author: kurumi $
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
// More information of Gurux Terminal (Modem) solution : http://www.gurux.org/GXTerminal
//
// This code is licensed under the GNU General Public License v2. 
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------

using System;
using Gurux.Terminal;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using Microsoft.VisualBasic;
using System.Data;
using System.Text;
using System.Diagnostics;
using Gurux.Common;

namespace GXTerminalSample
{
    internal partial class Form1 : System.Windows.Forms.Form
    {

        #region Close
        /// <summary>
        /// Closes Terminal connection.
		/// </summary>
		/// <param name="eventSender"></param>
		/// <param name="eventArgs"></param>
		private void CloseBtn_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            try
            {
                gxTerminal1.Close();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }
        #endregion //Close

        #region OnError
        /// <summary>
        /// Show occured error.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="exception"></param>
        private void gxTerminal1_OnError(object sender, Exception exception)
        {
            try
            {
                gxTerminal1.Close();
                MessageBox.Show(exception.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion //OnError

        #region OnMediaStateChange
        /// <summary>
        /// Update UI when media state changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gxTerminal1_OnMediaStateChange(object sender, MediaStateEventArgs e)
        {
            try
            {
                bool bOpen;
                bOpen = e.State == Gurux.Common.MediaState.Open;
                HexCB.Enabled = !bOpen;
                OpenBtn.Enabled = !bOpen;
                SendText.Enabled = bOpen;
                SendBtn.Enabled = bOpen;
                CloseBtn.Enabled = bOpen;
                ReceivedText.Enabled = bOpen;
                StatusTimer.Enabled = bOpen;
                PacketCounterTimer.Enabled = bOpen;
                //Close interval timer if media is closed.
                if (!bOpen)
                {
                    IntervalTB.Enabled = false;
                    IntervalTimer.Enabled = false;
                    IntervalBtn.Enabled = false;
                }
                else
                {
                    IntervalTB.Enabled = true;
                    IntervalBtn.Enabled = true;
                }
                //Read network status if media is opened.
                if (bOpen)
                {
                    StatusTimer_Tick(StatusTimer, new System.EventArgs());
                }
                else
                {
                    RSSITB.Text = "";
                    BERTB.Text = "";
                    BatteryCapacityTB.Text = "";
                    PowerConsumptionTB.Text = "";
                    NetworkStatusTB.Text = "";
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }
        #endregion //OnMediaStateChange

        #region OnReceived

        /// <summary>
        /// Show received data.
        /// </summary>
        private void OnReceived(object sender, ReceiveEventArgs e)
        {
            try
            {
                //We receive byte array from GXSerial and this must be changed to chars.
                if (HexCB.Checked)
                {
                    ReceivedText.Text += BitConverter.ToString((byte[])e.Data);
                }
                else
                {
                    //Get received data as string.
                    ReceivedText.Text += System.Text.Encoding.ASCII.GetString((byte[])e.Data);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);

            }
        }
        /// <summary>
        /// Show received data.
        /// </summary>
        private void gxTerminal1_OnReceived(object sender, ReceiveEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new ReceivedEventHandler(OnReceived), sender, e);
                }
                else
                {
                    OnReceived(sender, e);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }
        #endregion //OnReceived

        /// <summary>
        /// Read selected item.
		/// </summary>
		/// <param name="eventSender"></param>
		/// <param name="eventArgs"></param>
		private void IntervalTimer_Tick(System.Object eventSender, System.EventArgs eventArgs)
        {
            try
            {
                SendBtn_Click(SendBtn, new System.EventArgs());
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        #region Open
        /// <summary>
        /// Open Terminal connection.
		/// </summary>
		private void OpenBtn_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            try
            {
                gxTerminal1.Open();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }
        #endregion //Open

        /// <summary>
        /// Update sent/received byte counts.
        /// </summary>
        /// <param name="eventSender"></param>
        /// <param name="eventArgs"></param>
        private void PacketCounterTimer_Tick(System.Object eventSender, System.EventArgs eventArgs)
        {
            try
            {
                ReceivedTB.Text = gxTerminal1.BytesReceived.ToString();
                SentTB.Text = gxTerminal1.BytesSent.ToString();
                gxTerminal1.ResetByteCounters();
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        #region Properties

        /// <summary>
        /// Show GXTerminal media properties.
		/// </summary>
		private void PropertiesBtn_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            try
            {
                if (gxTerminal1.Properties(this))
                {
                    GXTerminalSample.Properties.Settings.Default.MediaSetting = gxTerminal1.Settings;
                    GXTerminalSample.Properties.Settings.Default.Save();
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }
        #endregion //Properties

        #region Send
        /// <summary>
        /// Send data.
		/// </summary>
		/// <param name="eventSender"></param>
		/// <param name="eventArgs"></param>
		private void SendBtn_Click(System.Object eventSender, System.EventArgs eventArgs)
        {
            try
            {
                ReceivedText.Text = string.Empty;
                if (SyncBtn.Checked) // Sends data synchronously.
                {
                    if (HexCB.Checked)
                    {
                        // Sends data as byte array.                        
                        lock (gxTerminal1.Synchronous)
                        {
                            Gurux.Common.ReceiveParameters<byte[]> p = new Gurux.Common.ReceiveParameters<byte[]>()
                            {
                                WaitTime = Convert.ToInt32(WaitTimeTB.Text),
                                Eop = EOPText.Text,
                                Count = Convert.ToInt32(MinSizeTB.Text)
                            };
                            gxTerminal1.Send(ASCIIEncoding.ASCII.GetBytes(SendText.Text));
                            if (gxTerminal1.Receive(p))
                            {
                                ReceivedText.Text = Convert.ToString(p.Reply);
                            }
                        }
                    }
                    else
                    {
                        // Sends data as ASCII string.
                        lock (gxTerminal1.Synchronous)
                        {
                            Gurux.Common.ReceiveParameters<string> p = new Gurux.Common.ReceiveParameters<string>()
                            {
                                WaitTime = Convert.ToInt32(WaitTimeTB.Text),
                                Eop = EOPText.Text,
                                Count = Convert.ToInt32(MinSizeTB.Text)
                            };
                            gxTerminal1.Send(SendText.Text);
                            if (gxTerminal1.Receive(p))
                            {
                                ReceivedText.Text = Convert.ToString(p.Reply);
                            }
                        }
                    }
                }
                else // Sends data asynchronously.
                {
                    if (HexCB.Checked)
                    {
                        // Sends data as byte array.
                        gxTerminal1.Send(ASCIIEncoding.ASCII.GetBytes(SendText.Text));
                    }
                    else
                    {
                        // Sends data as ASCII string.
                        gxTerminal1.Send(SendText.Text);
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }
        #endregion //Send

        /// <summary>
        /// Update modem state.
        /// </summary>
        /// <param name="eventSender"></param>
        /// <param name="eventArgs"></param>
		private void StatusTimer_Tick(System.Object eventSender, System.EventArgs eventArgs)
        {
            try
            {
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
                RSSITB.Text = "";
                BERTB.Text = "";
                BatteryCapacityTB.Text = "";
                PowerConsumptionTB.Text = "";
                NetworkStatusTB.Text = "";
            }
        }

        /// <summary>
        /// End of Packet is used only when data is send synchronously.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SyncBtn_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                MinSizeTB.Enabled = WaitTimeTB.Enabled = EOPText.Enabled = SyncBtn.Checked;
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        void gxTerminal1_OnTrace(object sender, TraceEventArgs e)
        {
            if ((e.Type & TraceTypes.Info) != 0)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
            else if ((e.Type & TraceTypes.Error) != 0)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
            else if ((e.Type & TraceTypes.Warning) != 0)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
            else if ((e.Type & TraceTypes.Sent) != 0)
            {
                System.Diagnostics.Debug.WriteLine("<- " + e.ToString());
            }
            else if ((e.Type & TraceTypes.Received) != 0)
            {
                System.Diagnostics.Debug.WriteLine("-> " + e.ToString());
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                gxTerminal1 = new GXTerminal();
                gxTerminal1.Settings = GXTerminalSample.Properties.Settings.Default.MediaSetting;
                gxTerminal1.Trace = TraceLevel.Verbose;
                gxTerminal1.OnTrace += new TraceEventHandler(gxTerminal1_OnTrace);
                gxTerminal1.OnError += new ErrorEventHandler(gxTerminal1_OnError);
                gxTerminal1.OnReceived += new ReceivedEventHandler(gxTerminal1_OnReceived);
                gxTerminal1.OnMediaStateChange += new MediaStateChangeEventHandler(gxTerminal1_OnMediaStateChange);
                PropertiesBtn.Enabled = true;
                if (gxTerminal1.IsOpen)
                {
                    gxTerminal1_OnMediaStateChange(this, new MediaStateEventArgs(MediaState.Open));
                }
                else
                {
                    gxTerminal1_OnMediaStateChange(this, new MediaStateEventArgs(MediaState.Closed));
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }
    }
}
