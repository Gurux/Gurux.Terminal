//
// --------------------------------------------------------------------------
//  Gurux Ltd
// 
//
//
// Filename:        $HeadURL: svn://utopia/projects/Old/GuruxTerminal/GXTerminal%20csharp%20Sample/Form1.Designer.cs $
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

using System.Diagnostics;
using System;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using System.Data;

namespace GXTerminalSample
{
	partial class Form1
	{
		[STAThread]
		static void Main()
		{
			System.Windows.Forms.Application.Run(new Form1());
		}
		#region "Windows Form Designer generated code "
		[System.Diagnostics.DebuggerNonUserCode()]public Form1()
		{
			//This call is required by the Windows Form Designer.
			InitializeComponent();
		}
		//Form overrides dispose to clean up the component list.
		[System.Diagnostics.DebuggerNonUserCode()]protected override void Dispose(bool Disposing)
		{
			if (Disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(Disposing);
		}
        //Required by the Windows Form Designer
		public System.Windows.Forms.TextBox RSSITB;
		public System.Windows.Forms.TextBox BERTB;
		public System.Windows.Forms.TextBox NetworkStatusTB;
		public System.Windows.Forms.TextBox BatteryCapacityTB;
		public System.Windows.Forms.TextBox PowerConsumptionTB;
		public System.Windows.Forms.Timer StatusTimer;
		public System.Windows.Forms.Label Label9;
		public System.Windows.Forms.Label Label8;
		public System.Windows.Forms.Label Label4;
		public System.Windows.Forms.Label Line2;
		public System.Windows.Forms.Label Label3;
		public System.Windows.Forms.Label Label2;
		public System.Windows.Forms.Label Line3;
		public System.Windows.Forms.GroupBox Frame5;
		public System.Windows.Forms.CheckBox HexCB;
		public System.Windows.Forms.CheckBox SyncBtn;
        public System.Windows.Forms.Button SendBtn;
        public System.Windows.Forms.GroupBox Frame4;
        public System.Windows.Forms.Timer PacketCounterTimer;
		public System.Windows.Forms.TextBox IntervalTB;
		public System.Windows.Forms.Timer IntervalTimer;
		public System.Windows.Forms.Button IntervalBtn;
		public System.Windows.Forms.Label Label7;
		public System.Windows.Forms.GroupBox Frame3;
		public System.Windows.Forms.TextBox SendText;
		public System.Windows.Forms.TextBox ReceivedText;
		public System.Windows.Forms.Label _Label1_1;
		public System.Windows.Forms.Label _Label1_0;
		public System.Windows.Forms.GroupBox Frame1;
		public System.Windows.Forms.Button OpenBtn;
		public System.Windows.Forms.Button CloseBtn;
        public System.Windows.Forms.Button PropertiesBtn;
		//NOTE: The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.
		//Do not modify it using the code editor.
		[System.Diagnostics.DebuggerStepThrough()]private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.Frame5 = new System.Windows.Forms.GroupBox();
            this.RSSITB = new System.Windows.Forms.TextBox();
            this.BERTB = new System.Windows.Forms.TextBox();
            this.NetworkStatusTB = new System.Windows.Forms.TextBox();
            this.BatteryCapacityTB = new System.Windows.Forms.TextBox();
            this.PowerConsumptionTB = new System.Windows.Forms.TextBox();
            this.Label9 = new System.Windows.Forms.Label();
            this.Label8 = new System.Windows.Forms.Label();
            this.Label4 = new System.Windows.Forms.Label();
            this.Line2 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.Line3 = new System.Windows.Forms.Label();
            this.StatusTimer = new System.Windows.Forms.Timer(this.components);
            this.Frame4 = new System.Windows.Forms.GroupBox();
            this.HexCB = new System.Windows.Forms.CheckBox();
            this.SyncBtn = new System.Windows.Forms.CheckBox();
            this.SendBtn = new System.Windows.Forms.Button();
            this.PacketCounterTimer = new System.Windows.Forms.Timer(this.components);
            this.Frame3 = new System.Windows.Forms.GroupBox();
            this.IntervalTB = new System.Windows.Forms.TextBox();
            this.IntervalBtn = new System.Windows.Forms.Button();
            this.Label7 = new System.Windows.Forms.Label();
            this.IntervalTimer = new System.Windows.Forms.Timer(this.components);
            this.Frame1 = new System.Windows.Forms.GroupBox();
            this.SendText = new System.Windows.Forms.TextBox();
            this.ReceivedText = new System.Windows.Forms.TextBox();
            this._Label1_1 = new System.Windows.Forms.Label();
            this._Label1_0 = new System.Windows.Forms.Label();
            this.OpenBtn = new System.Windows.Forms.Button();
            this.CloseBtn = new System.Windows.Forms.Button();
            this.PropertiesBtn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.MinSizeTB = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.EOPText = new System.Windows.Forms.TextBox();
            this.WaitTimeTB = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.Frame2 = new System.Windows.Forms.GroupBox();
            this.SentTB = new System.Windows.Forms.TextBox();
            this.ReceivedTB = new System.Windows.Forms.TextBox();
            this.Label5 = new System.Windows.Forms.Label();
            this.Label6 = new System.Windows.Forms.Label();
            this.Frame5.SuspendLayout();
            this.Frame4.SuspendLayout();
            this.Frame3.SuspendLayout();
            this.Frame1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.Frame2.SuspendLayout();
            this.SuspendLayout();
            // 
            // Frame5
            // 
            this.Frame5.BackColor = System.Drawing.SystemColors.Control;
            this.Frame5.Controls.Add(this.RSSITB);
            this.Frame5.Controls.Add(this.BERTB);
            this.Frame5.Controls.Add(this.NetworkStatusTB);
            this.Frame5.Controls.Add(this.BatteryCapacityTB);
            this.Frame5.Controls.Add(this.PowerConsumptionTB);
            this.Frame5.Controls.Add(this.Label9);
            this.Frame5.Controls.Add(this.Label8);
            this.Frame5.Controls.Add(this.Label4);
            this.Frame5.Controls.Add(this.Line2);
            this.Frame5.Controls.Add(this.Label3);
            this.Frame5.Controls.Add(this.Label2);
            this.Frame5.Controls.Add(this.Line3);
            this.Frame5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame5.Location = new System.Drawing.Point(0, 281);
            this.Frame5.Name = "Frame5";
            this.Frame5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Frame5.Size = new System.Drawing.Size(393, 113);
            this.Frame5.TabIndex = 21;
            this.Frame5.TabStop = false;
            this.Frame5.Text = "Modem status";
            // 
            // RSSITB
            // 
            this.RSSITB.AcceptsReturn = true;
            this.RSSITB.BackColor = System.Drawing.SystemColors.Window;
            this.RSSITB.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.RSSITB.Enabled = false;
            this.RSSITB.ForeColor = System.Drawing.SystemColors.WindowText;
            this.RSSITB.Location = new System.Drawing.Point(8, 32);
            this.RSSITB.MaxLength = 0;
            this.RSSITB.Name = "RSSITB";
            this.RSSITB.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.RSSITB.Size = new System.Drawing.Size(89, 20);
            this.RSSITB.TabIndex = 26;
            // 
            // BERTB
            // 
            this.BERTB.AcceptsReturn = true;
            this.BERTB.BackColor = System.Drawing.SystemColors.Window;
            this.BERTB.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.BERTB.Enabled = false;
            this.BERTB.ForeColor = System.Drawing.SystemColors.WindowText;
            this.BERTB.Location = new System.Drawing.Point(8, 80);
            this.BERTB.MaxLength = 0;
            this.BERTB.Name = "BERTB";
            this.BERTB.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.BERTB.Size = new System.Drawing.Size(89, 20);
            this.BERTB.TabIndex = 25;
            // 
            // NetworkStatusTB
            // 
            this.NetworkStatusTB.AcceptsReturn = true;
            this.NetworkStatusTB.BackColor = System.Drawing.SystemColors.Window;
            this.NetworkStatusTB.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.NetworkStatusTB.Enabled = false;
            this.NetworkStatusTB.ForeColor = System.Drawing.SystemColors.WindowText;
            this.NetworkStatusTB.Location = new System.Drawing.Point(232, 32);
            this.NetworkStatusTB.MaxLength = 0;
            this.NetworkStatusTB.Name = "NetworkStatusTB";
            this.NetworkStatusTB.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.NetworkStatusTB.Size = new System.Drawing.Size(89, 20);
            this.NetworkStatusTB.TabIndex = 24;
            // 
            // BatteryCapacityTB
            // 
            this.BatteryCapacityTB.AcceptsReturn = true;
            this.BatteryCapacityTB.BackColor = System.Drawing.SystemColors.Window;
            this.BatteryCapacityTB.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.BatteryCapacityTB.Enabled = false;
            this.BatteryCapacityTB.ForeColor = System.Drawing.SystemColors.WindowText;
            this.BatteryCapacityTB.Location = new System.Drawing.Point(120, 32);
            this.BatteryCapacityTB.MaxLength = 0;
            this.BatteryCapacityTB.Name = "BatteryCapacityTB";
            this.BatteryCapacityTB.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.BatteryCapacityTB.Size = new System.Drawing.Size(89, 20);
            this.BatteryCapacityTB.TabIndex = 23;
            // 
            // PowerConsumptionTB
            // 
            this.PowerConsumptionTB.AcceptsReturn = true;
            this.PowerConsumptionTB.BackColor = System.Drawing.SystemColors.Window;
            this.PowerConsumptionTB.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.PowerConsumptionTB.Enabled = false;
            this.PowerConsumptionTB.ForeColor = System.Drawing.SystemColors.WindowText;
            this.PowerConsumptionTB.Location = new System.Drawing.Point(120, 80);
            this.PowerConsumptionTB.MaxLength = 0;
            this.PowerConsumptionTB.Name = "PowerConsumptionTB";
            this.PowerConsumptionTB.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.PowerConsumptionTB.Size = new System.Drawing.Size(89, 20);
            this.PowerConsumptionTB.TabIndex = 22;
            // 
            // Label9
            // 
            this.Label9.BackColor = System.Drawing.SystemColors.Control;
            this.Label9.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label9.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label9.Location = new System.Drawing.Point(8, 16);
            this.Label9.Name = "Label9";
            this.Label9.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label9.Size = new System.Drawing.Size(33, 17);
            this.Label9.TabIndex = 31;
            this.Label9.Text = "RSSI:";
            // 
            // Label8
            // 
            this.Label8.BackColor = System.Drawing.SystemColors.Control;
            this.Label8.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label8.Location = new System.Drawing.Point(8, 64);
            this.Label8.Name = "Label8";
            this.Label8.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label8.Size = new System.Drawing.Size(25, 17);
            this.Label8.TabIndex = 30;
            this.Label8.Text = "BER:";
            // 
            // Label4
            // 
            this.Label4.BackColor = System.Drawing.SystemColors.Control;
            this.Label4.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label4.Location = new System.Drawing.Point(232, 16);
            this.Label4.Name = "Label4";
            this.Label4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label4.Size = new System.Drawing.Size(81, 17);
            this.Label4.TabIndex = 29;
            this.Label4.Text = "Network Status:";
            // 
            // Line2
            // 
            this.Line2.BackColor = System.Drawing.SystemColors.WindowText;
            this.Line2.Location = new System.Drawing.Point(112, 16);
            this.Line2.Name = "Line2";
            this.Line2.Size = new System.Drawing.Size(1, 88);
            this.Line2.TabIndex = 32;
            // 
            // Label3
            // 
            this.Label3.BackColor = System.Drawing.SystemColors.Control;
            this.Label3.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label3.Location = new System.Drawing.Point(120, 16);
            this.Label3.Name = "Label3";
            this.Label3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label3.Size = new System.Drawing.Size(81, 17);
            this.Label3.TabIndex = 28;
            this.Label3.Text = "Battery Capacity:";
            // 
            // Label2
            // 
            this.Label2.BackColor = System.Drawing.SystemColors.Control;
            this.Label2.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label2.Location = new System.Drawing.Point(120, 64);
            this.Label2.Name = "Label2";
            this.Label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label2.Size = new System.Drawing.Size(97, 17);
            this.Label2.TabIndex = 27;
            this.Label2.Text = "Power Consumption:";
            // 
            // Line3
            // 
            this.Line3.BackColor = System.Drawing.SystemColors.WindowText;
            this.Line3.Location = new System.Drawing.Point(224, 16);
            this.Line3.Name = "Line3";
            this.Line3.Size = new System.Drawing.Size(1, 88);
            this.Line3.TabIndex = 33;
            // 
            // StatusTimer
            // 
            this.StatusTimer.Interval = 30000;
            this.StatusTimer.Tick += new System.EventHandler(this.StatusTimer_Tick);
            // 
            // Frame4
            // 
            this.Frame4.BackColor = System.Drawing.SystemColors.Control;
            this.Frame4.Controls.Add(this.HexCB);
            this.Frame4.Controls.Add(this.SyncBtn);
            this.Frame4.Controls.Add(this.SendBtn);
            this.Frame4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame4.Location = new System.Drawing.Point(0, 144);
            this.Frame4.Name = "Frame4";
            this.Frame4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Frame4.Size = new System.Drawing.Size(393, 49);
            this.Frame4.TabIndex = 18;
            this.Frame4.TabStop = false;
            this.Frame4.Text = "Send";
            // 
            // HexCB
            // 
            this.HexCB.BackColor = System.Drawing.SystemColors.Control;
            this.HexCB.Checked = true;
            this.HexCB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.HexCB.Cursor = System.Windows.Forms.Cursors.Default;
            this.HexCB.ForeColor = System.Drawing.SystemColors.ControlText;
            this.HexCB.Location = new System.Drawing.Point(88, 16);
            this.HexCB.Name = "HexCB";
            this.HexCB.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.HexCB.Size = new System.Drawing.Size(57, 25);
            this.HexCB.TabIndex = 8;
            this.HexCB.Text = "Hex";
            this.HexCB.UseVisualStyleBackColor = false;
            // 
            // SyncBtn
            // 
            this.SyncBtn.BackColor = System.Drawing.SystemColors.Control;
            this.SyncBtn.Cursor = System.Windows.Forms.Cursors.Default;
            this.SyncBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.SyncBtn.Location = new System.Drawing.Point(224, 16);
            this.SyncBtn.Name = "SyncBtn";
            this.SyncBtn.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.SyncBtn.Size = new System.Drawing.Size(57, 25);
            this.SyncBtn.TabIndex = 20;
            this.SyncBtn.Text = "Sync";
            this.SyncBtn.UseVisualStyleBackColor = false;
            this.SyncBtn.CheckedChanged += new System.EventHandler(this.SyncBtn_CheckedChanged);
            // 
            // SendBtn
            // 
            this.SendBtn.BackColor = System.Drawing.SystemColors.Control;
            this.SendBtn.Cursor = System.Windows.Forms.Cursors.Default;
            this.SendBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.SendBtn.Location = new System.Drawing.Point(312, 16);
            this.SendBtn.Name = "SendBtn";
            this.SendBtn.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.SendBtn.Size = new System.Drawing.Size(73, 25);
            this.SendBtn.TabIndex = 19;
            this.SendBtn.Text = "Send";
            this.SendBtn.UseVisualStyleBackColor = false;
            this.SendBtn.Click += new System.EventHandler(this.SendBtn_Click);
            // 
            // PacketCounterTimer
            // 
            this.PacketCounterTimer.Interval = 1000;
            this.PacketCounterTimer.Tick += new System.EventHandler(this.PacketCounterTimer_Tick);
            // 
            // Frame3
            // 
            this.Frame3.BackColor = System.Drawing.SystemColors.Control;
            this.Frame3.Controls.Add(this.IntervalTB);
            this.Frame3.Controls.Add(this.IntervalBtn);
            this.Frame3.Controls.Add(this.Label7);
            this.Frame3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame3.Location = new System.Drawing.Point(0, 393);
            this.Frame3.Name = "Frame3";
            this.Frame3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Frame3.Size = new System.Drawing.Size(393, 49);
            this.Frame3.TabIndex = 9;
            this.Frame3.TabStop = false;
            this.Frame3.Text = "Tester";
            // 
            // IntervalTB
            // 
            this.IntervalTB.AcceptsReturn = true;
            this.IntervalTB.BackColor = System.Drawing.SystemColors.Window;
            this.IntervalTB.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.IntervalTB.Enabled = false;
            this.IntervalTB.ForeColor = System.Drawing.SystemColors.WindowText;
            this.IntervalTB.Location = new System.Drawing.Point(56, 16);
            this.IntervalTB.MaxLength = 0;
            this.IntervalTB.Name = "IntervalTB";
            this.IntervalTB.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.IntervalTB.Size = new System.Drawing.Size(89, 20);
            this.IntervalTB.TabIndex = 11;
            this.IntervalTB.Text = "10";
            // 
            // IntervalBtn
            // 
            this.IntervalBtn.BackColor = System.Drawing.SystemColors.Control;
            this.IntervalBtn.Cursor = System.Windows.Forms.Cursors.Default;
            this.IntervalBtn.Enabled = false;
            this.IntervalBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.IntervalBtn.Location = new System.Drawing.Point(152, 16);
            this.IntervalBtn.Name = "IntervalBtn";
            this.IntervalBtn.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.IntervalBtn.Size = new System.Drawing.Size(73, 25);
            this.IntervalBtn.TabIndex = 10;
            this.IntervalBtn.Text = "Go/Stop";
            this.IntervalBtn.UseVisualStyleBackColor = false;
            // 
            // Label7
            // 
            this.Label7.BackColor = System.Drawing.SystemColors.Control;
            this.Label7.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label7.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label7.Location = new System.Drawing.Point(8, 16);
            this.Label7.Name = "Label7";
            this.Label7.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label7.Size = new System.Drawing.Size(57, 17);
            this.Label7.TabIndex = 12;
            this.Label7.Text = "Interval";
            // 
            // IntervalTimer
            // 
            this.IntervalTimer.Interval = 1;
            this.IntervalTimer.Tick += new System.EventHandler(this.IntervalTimer_Tick);
            // 
            // Frame1
            // 
            this.Frame1.BackColor = System.Drawing.SystemColors.Control;
            this.Frame1.Controls.Add(this.SendText);
            this.Frame1.Controls.Add(this.ReceivedText);
            this.Frame1.Controls.Add(this._Label1_1);
            this.Frame1.Controls.Add(this._Label1_0);
            this.Frame1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame1.Location = new System.Drawing.Point(0, 0);
            this.Frame1.Name = "Frame1";
            this.Frame1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Frame1.Size = new System.Drawing.Size(337, 137);
            this.Frame1.TabIndex = 6;
            this.Frame1.TabStop = false;
            // 
            // SendText
            // 
            this.SendText.AcceptsReturn = true;
            this.SendText.BackColor = System.Drawing.SystemColors.Window;
            this.SendText.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.SendText.Enabled = false;
            this.SendText.ForeColor = System.Drawing.SystemColors.WindowText;
            this.SendText.Location = new System.Drawing.Point(8, 32);
            this.SendText.MaxLength = 0;
            this.SendText.Multiline = true;
            this.SendText.Name = "SendText";
            this.SendText.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.SendText.Size = new System.Drawing.Size(321, 33);
            this.SendText.TabIndex = 4;
            // 
            // ReceivedText
            // 
            this.ReceivedText.AcceptsReturn = true;
            this.ReceivedText.BackColor = System.Drawing.SystemColors.Window;
            this.ReceivedText.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.ReceivedText.ForeColor = System.Drawing.SystemColors.WindowText;
            this.ReceivedText.Location = new System.Drawing.Point(8, 88);
            this.ReceivedText.MaxLength = 0;
            this.ReceivedText.Name = "ReceivedText";
            this.ReceivedText.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ReceivedText.Size = new System.Drawing.Size(321, 20);
            this.ReceivedText.TabIndex = 5;
            this.ReceivedText.TabStop = false;
            // 
            // _Label1_1
            // 
            this._Label1_1.BackColor = System.Drawing.SystemColors.Control;
            this._Label1_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_1.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_1.Location = new System.Drawing.Point(8, 8);
            this._Label1_1.Name = "_Label1_1";
            this._Label1_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_1.Size = new System.Drawing.Size(89, 17);
            this._Label1_1.TabIndex = 0;
            this._Label1_1.Text = "Send:";
            // 
            // _Label1_0
            // 
            this._Label1_0.BackColor = System.Drawing.SystemColors.Control;
            this._Label1_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._Label1_0.ForeColor = System.Drawing.SystemColors.ControlText;
            this._Label1_0.Location = new System.Drawing.Point(8, 72);
            this._Label1_0.Name = "_Label1_0";
            this._Label1_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._Label1_0.Size = new System.Drawing.Size(89, 17);
            this._Label1_0.TabIndex = 7;
            this._Label1_0.Text = "Received:";
            // 
            // OpenBtn
            // 
            this.OpenBtn.BackColor = System.Drawing.SystemColors.Control;
            this.OpenBtn.Cursor = System.Windows.Forms.Cursors.Default;
            this.OpenBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.OpenBtn.Location = new System.Drawing.Point(344, 24);
            this.OpenBtn.Name = "OpenBtn";
            this.OpenBtn.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.OpenBtn.Size = new System.Drawing.Size(73, 25);
            this.OpenBtn.TabIndex = 1;
            this.OpenBtn.Text = "Open";
            this.OpenBtn.UseVisualStyleBackColor = false;
            this.OpenBtn.Click += new System.EventHandler(this.OpenBtn_Click);
            // 
            // CloseBtn
            // 
            this.CloseBtn.BackColor = System.Drawing.SystemColors.Control;
            this.CloseBtn.Cursor = System.Windows.Forms.Cursors.Default;
            this.CloseBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseBtn.Enabled = false;
            this.CloseBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.CloseBtn.Location = new System.Drawing.Point(344, 56);
            this.CloseBtn.Name = "CloseBtn";
            this.CloseBtn.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.CloseBtn.Size = new System.Drawing.Size(73, 25);
            this.CloseBtn.TabIndex = 2;
            this.CloseBtn.Text = "Close";
            this.CloseBtn.UseVisualStyleBackColor = false;
            this.CloseBtn.Click += new System.EventHandler(this.CloseBtn_Click);
            // 
            // PropertiesBtn
            // 
            this.PropertiesBtn.BackColor = System.Drawing.SystemColors.Control;
            this.PropertiesBtn.Cursor = System.Windows.Forms.Cursors.Default;
            this.PropertiesBtn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.PropertiesBtn.Location = new System.Drawing.Point(344, 88);
            this.PropertiesBtn.Name = "PropertiesBtn";
            this.PropertiesBtn.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.PropertiesBtn.Size = new System.Drawing.Size(73, 25);
            this.PropertiesBtn.TabIndex = 3;
            this.PropertiesBtn.Text = "Properties";
            this.PropertiesBtn.UseVisualStyleBackColor = false;
            this.PropertiesBtn.Click += new System.EventHandler(this.PropertiesBtn_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.MinSizeTB);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.EOPText);
            this.groupBox1.Controls.Add(this.WaitTimeTB);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox1.Location = new System.Drawing.Point(1, 191);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.groupBox1.Size = new System.Drawing.Size(393, 49);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Synchronous settings";
            // 
            // MinSizeTB
            // 
            this.MinSizeTB.AcceptsReturn = true;
            this.MinSizeTB.BackColor = System.Drawing.SystemColors.Window;
            this.MinSizeTB.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.MinSizeTB.Enabled = false;
            this.MinSizeTB.ForeColor = System.Drawing.SystemColors.WindowText;
            this.MinSizeTB.Location = new System.Drawing.Point(310, 15);
            this.MinSizeTB.MaxLength = 0;
            this.MinSizeTB.Name = "MinSizeTB";
            this.MinSizeTB.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.MinSizeTB.Size = new System.Drawing.Size(72, 20);
            this.MinSizeTB.TabIndex = 26;
            this.MinSizeTB.Text = "0";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.SystemColors.Control;
            this.label10.Cursor = System.Windows.Forms.Cursors.Default;
            this.label10.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label10.Location = new System.Drawing.Point(272, 16);
            this.label10.Name = "label10";
            this.label10.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label10.Size = new System.Drawing.Size(32, 13);
            this.label10.TabIndex = 25;
            this.label10.Text = "EOP:";
            // 
            // EOPText
            // 
            this.EOPText.AcceptsReturn = true;
            this.EOPText.BackColor = System.Drawing.SystemColors.Window;
            this.EOPText.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.EOPText.Enabled = false;
            this.EOPText.ForeColor = System.Drawing.SystemColors.WindowText;
            this.EOPText.Location = new System.Drawing.Point(192, 15);
            this.EOPText.MaxLength = 0;
            this.EOPText.Name = "EOPText";
            this.EOPText.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.EOPText.Size = new System.Drawing.Size(72, 20);
            this.EOPText.TabIndex = 24;
            this.EOPText.Text = "\\r\\n";
            // 
            // WaitTimeTB
            // 
            this.WaitTimeTB.AcceptsReturn = true;
            this.WaitTimeTB.BackColor = System.Drawing.SystemColors.Window;
            this.WaitTimeTB.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.WaitTimeTB.Enabled = false;
            this.WaitTimeTB.ForeColor = System.Drawing.SystemColors.WindowText;
            this.WaitTimeTB.Location = new System.Drawing.Point(80, 16);
            this.WaitTimeTB.MaxLength = 0;
            this.WaitTimeTB.Name = "WaitTimeTB";
            this.WaitTimeTB.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.WaitTimeTB.Size = new System.Drawing.Size(61, 20);
            this.WaitTimeTB.TabIndex = 13;
            this.WaitTimeTB.Text = "5000";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.SystemColors.Control;
            this.label11.Cursor = System.Windows.Forms.Cursors.Default;
            this.label11.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label11.Location = new System.Drawing.Point(154, 16);
            this.label11.Name = "label11";
            this.label11.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label11.Size = new System.Drawing.Size(32, 13);
            this.label11.TabIndex = 16;
            this.label11.Text = "EOP:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.SystemColors.Control;
            this.label12.Cursor = System.Windows.Forms.Cursors.Default;
            this.label12.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label12.Location = new System.Drawing.Point(8, 18);
            this.label12.Name = "label12";
            this.label12.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label12.Size = new System.Drawing.Size(58, 13);
            this.label12.TabIndex = 15;
            this.label12.Text = "Wait Time:";
            // 
            // Frame2
            // 
            this.Frame2.BackColor = System.Drawing.SystemColors.Control;
            this.Frame2.Controls.Add(this.SentTB);
            this.Frame2.Controls.Add(this.ReceivedTB);
            this.Frame2.Controls.Add(this.Label5);
            this.Frame2.Controls.Add(this.Label6);
            this.Frame2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame2.Location = new System.Drawing.Point(0, 234);
            this.Frame2.Name = "Frame2";
            this.Frame2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Frame2.Size = new System.Drawing.Size(393, 49);
            this.Frame2.TabIndex = 23;
            this.Frame2.TabStop = false;
            this.Frame2.Text = "Packet statistics";
            // 
            // SentTB
            // 
            this.SentTB.AcceptsReturn = true;
            this.SentTB.BackColor = System.Drawing.SystemColors.Window;
            this.SentTB.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.SentTB.Enabled = false;
            this.SentTB.ForeColor = System.Drawing.SystemColors.WindowText;
            this.SentTB.Location = new System.Drawing.Point(56, 16);
            this.SentTB.MaxLength = 0;
            this.SentTB.Name = "SentTB";
            this.SentTB.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.SentTB.Size = new System.Drawing.Size(89, 20);
            this.SentTB.TabIndex = 15;
            this.SentTB.Text = "0";
            // 
            // ReceivedTB
            // 
            this.ReceivedTB.AcceptsReturn = true;
            this.ReceivedTB.BackColor = System.Drawing.SystemColors.Window;
            this.ReceivedTB.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.ReceivedTB.Enabled = false;
            this.ReceivedTB.ForeColor = System.Drawing.SystemColors.WindowText;
            this.ReceivedTB.Location = new System.Drawing.Point(224, 16);
            this.ReceivedTB.MaxLength = 0;
            this.ReceivedTB.Name = "ReceivedTB";
            this.ReceivedTB.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ReceivedTB.Size = new System.Drawing.Size(105, 20);
            this.ReceivedTB.TabIndex = 14;
            this.ReceivedTB.Text = "0";
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.BackColor = System.Drawing.SystemColors.Control;
            this.Label5.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label5.Location = new System.Drawing.Point(8, 16);
            this.Label5.Name = "Label5";
            this.Label5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label5.Size = new System.Drawing.Size(32, 13);
            this.Label5.TabIndex = 17;
            this.Label5.Text = "Sent:";
            // 
            // Label6
            // 
            this.Label6.AutoSize = true;
            this.Label6.BackColor = System.Drawing.SystemColors.Control;
            this.Label6.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label6.Location = new System.Drawing.Point(160, 16);
            this.Label6.Name = "Label6";
            this.Label6.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label6.Size = new System.Drawing.Size(56, 13);
            this.Label6.TabIndex = 16;
            this.Label6.Text = "Received:";
            // 
            // Form1
            // 
            this.AcceptButton = this.OpenBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.CloseBtn;
            this.ClientSize = new System.Drawing.Size(425, 452);
            this.Controls.Add(this.Frame2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.Frame5);
            this.Controls.Add(this.Frame4);
            this.Controls.Add(this.Frame3);
            this.Controls.Add(this.Frame1);
            this.Controls.Add(this.OpenBtn);
            this.Controls.Add(this.CloseBtn);
            this.Controls.Add(this.PropertiesBtn);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(147, 109);
            this.Name = "Form1";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "GXTerminal Sample";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Frame5.ResumeLayout(false);
            this.Frame5.PerformLayout();
            this.Frame4.ResumeLayout(false);
            this.Frame3.ResumeLayout(false);
            this.Frame3.PerformLayout();
            this.Frame1.ResumeLayout(false);
            this.Frame1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.Frame2.ResumeLayout(false);
            this.Frame2.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

        private System.ComponentModel.IContainer components;
        public GroupBox groupBox1;
        public TextBox MinSizeTB;
        public Label label10;
        public TextBox EOPText;
        public TextBox WaitTimeTB;
        public Label label11;
        public Label label12;
        public GroupBox Frame2;
        public TextBox SentTB;
        public TextBox ReceivedTB;
        public Label Label5;
        public Label Label6;
        private Gurux.Terminal.GXTerminal gxTerminal1;
	}
}
