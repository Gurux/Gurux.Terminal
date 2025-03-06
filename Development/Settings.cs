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
using System.ComponentModel;
using System.Windows.Forms;
using Gurux.Common;
namespace Gurux.Terminal
{
    /// <summary>
    /// Settings dialog.
    /// </summary>
    partial class Settings : Form, IGXPropertyPage, INotifyPropertyChanged
    {
        private bool _initialize = true;
        private GXTerminal Target;

        private PropertyChangedEventHandler propertyChanged;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Dirty
        {
            get;
            set;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                propertyChanged += value;
            }
            remove
            {
                propertyChanged -= value;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Settings(GXTerminal target)
        {
            InitializeComponent();
            Target = target;
        }

        private void PortNameCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (BaudRatePanel.Enabled)
            {
                this.BaudRateCB.Items.Clear();
                BaudRateCB.DropDownStyle = ComboBoxStyle.DropDownList;
                //User can't change values when connection is open.
                if (Target.IsOpen)
                {
                    BaudRateCB.Items.Add(Target.BaudRate);
                    this.BaudRateCB.SelectedItem = 0;
                }
                else
                {
                    foreach (int it in GXTerminal.GetAvailableBaudRates(PortNameCB.Text))
                    {
                        if (it == 0)
                        {
                            BaudRateCB.DropDownStyle = ComboBoxStyle.DropDown;
                        }
                        else
                        {
                            this.BaudRateCB.Items.Add(it);
                        }
                    }
                    this.BaudRateCB.SelectedItem = Target.BaudRate;
                }
            }
        }

        #region IGXPropertyPage Members

        void IGXPropertyPage.Apply()
        {
            Target.Server = this.ServerCB.Checked;
            Target.PhoneNumber = this.NumberTB.Text;
            Target.PortName = this.PortNameCB.Text;
            Target.BaudRate = Convert.ToInt32(this.BaudRateCB.Text);
            Target.DataBits = Convert.ToInt32(this.DataBitsCB.Text);
            Target.Parity = (System.IO.Ports.Parity)this.ParityCB.SelectedItem;
            Target.StopBits = (System.IO.Ports.StopBits)this.StopBitsCB.SelectedItem;
            Target.ConnectionWaitTime = Convert.ToInt32(this.ConnectionWaitTimeTb.Text);
            Target.CommandWaitTime = Convert.ToInt32(this.CommandWaitTimeTb.Text);
            Target.HangsUpDelay = Convert.ToInt32(this.HangsUpDelayTb.Text);
            Dirty = false;
        }

        void IGXPropertyPage.Initialize()
        {
            //Update texts.
            Text = Gurux.Terminal.Properties.Resources.SettingsTxt;
            NumberLbl.Text = Gurux.Terminal.Properties.Resources.PhoneNumberTxt;
            PortNameLbl.Text = Gurux.Terminal.Properties.Resources.PortNameTxt;
            BaudRateLbl.Text = Gurux.Terminal.Properties.Resources.BaudRate;
            DataBitsLbl.Text = Gurux.Terminal.Properties.Resources.DataBits;
            ParityLbl.Text = Gurux.Terminal.Properties.Resources.Parity;
            StopBitsLbl.Text = Gurux.Terminal.Properties.Resources.StopBits;
            ConnectionWaitTimeLbl.Text = Gurux.Terminal.Properties.Resources.ConnectionWaitTimeTxt;
            CommandWaitTimeLbl.Text = Gurux.Terminal.Properties.Resources.CommandWaitTimeTxt;
            HangsUpDelayLbl.Text = Gurux.Terminal.Properties.Resources.HangsUpDelayTxt;
            NumberTB.Text = Target.PhoneNumber;
            ServerCB.Checked = Target.Server;
            ConnectionWaitTimeTb.Text = Target.ConnectionWaitTime.ToString();
            CommandWaitTimeTb.Text = Target.CommandWaitTime.ToString();
            HangsUpDelayTb.Text = Target.HangsUpDelay.ToString();

            NumberPanel.Enabled = (Target.ConfigurableSettings & AvailableMediaSettings.PhoneNumber) != 0;
            //Server is disabled because it is not work correctly yet.
            //ServerPanel.Enabled = (Target.ConfigurableSettings & AvailableMediaSettings.Server) != 0;
            ServerPanel.Enabled = false;

            //Hide controls which user do not want to show.
            PortNamePanel.Enabled = (Target.ConfigurableSettings & AvailableMediaSettings.PortName) != 0;
            if (PortNamePanel.Enabled)
            {
                if (Target.AvailablePorts != null)
                {
                    PortNameCB.Items.AddRange(Target.AvailablePorts);
                }
                else
                {
                    PortNameCB.Items.AddRange(GXTerminal.GetPortNames());
                }
                if (this.PortNameCB.Items.Contains(Target.PortName))
                {
                    this.PortNameCB.SelectedItem = Target.PortName;
                }
                else
                {
                    this.PortNameCB.SelectedIndex = 0;
                }
            }
            BaudRatePanel.Enabled = (Target.ConfigurableSettings & AvailableMediaSettings.BaudRate) != 0;
            if (BaudRatePanel.Enabled)
            {
                PortNameCB_SelectedIndexChanged(null, null);
                this.BaudRateCB.SelectedItem = Target.BaudRate;
            }
            DataBitsPanel.Enabled = (Target.ConfigurableSettings & AvailableMediaSettings.DataBits) != 0;
            if (DataBitsPanel.Enabled)
            {
                this.DataBitsCB.Items.Add(7);
                this.DataBitsCB.Items.Add(8);
                this.DataBitsCB.SelectedItem = Target.DataBits;
            }

            ParityPanel.Enabled = (Target.ConfigurableSettings & AvailableMediaSettings.Parity) != 0;
            if (ParityPanel.Enabled)
            {
                this.ParityCB.Items.Add(System.IO.Ports.Parity.None);
                this.ParityCB.Items.Add(System.IO.Ports.Parity.Odd);
                this.ParityCB.Items.Add(System.IO.Ports.Parity.Even);
                this.ParityCB.Items.Add(System.IO.Ports.Parity.Mark);
                this.ParityCB.Items.Add(System.IO.Ports.Parity.Space);
                this.ParityCB.SelectedItem = Target.Parity;
            }

            StopBitsPanel.Enabled = (Target.ConfigurableSettings & AvailableMediaSettings.StopBits) != 0;
            if (StopBitsPanel.Enabled)
            {
                this.StopBitsCB.Items.Add(System.IO.Ports.StopBits.None);
                this.StopBitsCB.Items.Add(System.IO.Ports.StopBits.One);
                this.StopBitsCB.Items.Add(System.IO.Ports.StopBits.OnePointFive);
                this.StopBitsCB.Items.Add(System.IO.Ports.StopBits.Two);
                this.StopBitsCB.SelectedItem = Target.StopBits;
            }

            ConnectionWaitTimePanel.Enabled = (Target.ConfigurableSettings & AvailableMediaSettings.ConnectionWaitTime) != 0;
            CommandWaitTimePanel.Enabled = (Target.ConfigurableSettings & AvailableMediaSettings.CommandWaitTime) != 0;
            HangsUpDelayPanel.Enabled = (Target.ConfigurableSettings & AvailableMediaSettings.HangsUpDelay) != 0;
            UpdateEditBoxSizes();
            Dirty = false;
            _initialize = false;
        }

        /// <summary>
        /// Because label lenght depends from the localization string, edit box sizes must be update.
        /// </summary>
        private void UpdateEditBoxSizes()
        {
            //Find max length of the localization string.
            int maxLength = 0;
            foreach (Control it in this.Controls)
            {
                if (it.Enabled)
                {
                    foreach (Control it2 in it.Controls)
                    {
                        if (it2 is Label && it2.Right > maxLength)
                        {
                            maxLength = it2.Right;
                        }
                    }
                }
            }
            //Increase edit control length.
            foreach (Control it in this.Controls)
            {
                if (it.Enabled)
                {
                    foreach (Control it2 in it.Controls)
                    {
                        if (it2 is ComboBox || it2 is TextBox)
                        {
                            it2.Width += it2.Left - maxLength - 10;
                            it2.Left = maxLength + 10;
                        }
                    }
                }
            }
        }

        #endregion

        private void ServerCB_CheckedChanged(object sender, EventArgs e)
        {
            NumberTB.Enabled = !ServerCB.Checked;
            if (!_initialize)
            {
                Target.Server = this.ServerCB.Checked;
            }
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs("Server"));
            }
        }

        private void NumberTB_TextChanged(object sender, EventArgs e)
        {
            Dirty = true;
            if (!_initialize)
            {
                Target.PhoneNumber = this.NumberTB.Text;
            }
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs("Number"));
            }
        }

        private void PortNameCB_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            Dirty = true;
            if (!_initialize)
            {
                Target.PortName = this.PortNameCB.Text;
            }
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs("PortName"));
            }
        }

        private void BaudRateCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            Dirty = true;
            if (!_initialize)
            {
                Target.BaudRate = Convert.ToInt32(this.BaudRateCB.Text);
            }
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs("BaudRate"));
            }
        }

        private void DataBitsCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            Dirty = true;
            if (!_initialize)
            {
                Target.DataBits = Convert.ToInt32(this.DataBitsCB.Text);
            }
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs("DataBits"));
            }
        }

        private void ParityCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            Dirty = true;
            if (!_initialize)
            {
                Target.Parity = (System.IO.Ports.Parity)this.ParityCB.SelectedItem;
            }
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs("Parity"));
            }
        }

        private void StopBitsCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            Dirty = true;
            if (!_initialize)
            {
                Target.StopBits = (System.IO.Ports.StopBits)this.StopBitsCB.SelectedItem;
            }
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs("StopBits"));
            }
        }

        private void ConnectionWaitTimeTb_TextChanged(object sender, EventArgs e)
        {
            Dirty = true;
            if (!_initialize)
            {
                Target.ConnectionWaitTime = Convert.ToInt32(this.ConnectionWaitTimeTb.Text);
            }
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs("ConnectionWaitTime"));
            }
        }

        private void CommandWaitTimeTb_TextChanged(object sender, EventArgs e)
        {
            Dirty = true;
            if (!_initialize)
            {
                Target.CommandWaitTime = Convert.ToInt32(this.CommandWaitTimeTb.Text);
            }
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs("CommandWaitTime"));
            }
        }

        private void HangsUpDelayTb_TextChanged(object sender, EventArgs e)
        {
            Dirty = true;
            if (!_initialize)
            {
                Target.HangsUpDelay = Convert.ToInt32(this.HangsUpDelayTb.Text);
            }
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs("HangsUpDelay"));
            }
        }
    }
}
