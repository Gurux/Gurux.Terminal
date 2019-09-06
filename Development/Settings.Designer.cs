namespace Gurux.Terminal
{
partial class Settings
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.ServerPanel = new System.Windows.Forms.Panel();
            this.ServerCB = new System.Windows.Forms.CheckBox();
            this.StopBitsPanel = new System.Windows.Forms.Panel();
            this.StopBitsLbl = new System.Windows.Forms.Label();
            this.StopBitsCB = new System.Windows.Forms.ComboBox();
            this.ParityPanel = new System.Windows.Forms.Panel();
            this.ParityLbl = new System.Windows.Forms.Label();
            this.ParityCB = new System.Windows.Forms.ComboBox();
            this.DataBitsPanel = new System.Windows.Forms.Panel();
            this.DataBitsCB = new System.Windows.Forms.ComboBox();
            this.DataBitsLbl = new System.Windows.Forms.Label();
            this.BaudRatePanel = new System.Windows.Forms.Panel();
            this.BaudRateCB = new System.Windows.Forms.ComboBox();
            this.BaudRateLbl = new System.Windows.Forms.Label();
            this.PortNamePanel = new System.Windows.Forms.Panel();
            this.PortNameCB = new System.Windows.Forms.ComboBox();
            this.PortNameLbl = new System.Windows.Forms.Label();
            this.NumberPanel = new System.Windows.Forms.Panel();
            this.NumberTB = new System.Windows.Forms.TextBox();
            this.NumberLbl = new System.Windows.Forms.Label();
            this.ConnectionWaitTimePanel = new System.Windows.Forms.Panel();
            this.ConnectionWaitTimeTb = new System.Windows.Forms.TextBox();
            this.ConnectionWaitTimeLbl = new System.Windows.Forms.Label();
            this.CommandWaitTimePanel = new System.Windows.Forms.Panel();
            this.CommandWaitTimeTb = new System.Windows.Forms.TextBox();
            this.CommandWaitTimeLbl = new System.Windows.Forms.Label();
            this.HangsUpDelayPanel = new System.Windows.Forms.Panel();
            this.HangsUpDelayTb = new System.Windows.Forms.TextBox();
            this.HangsUpDelayLbl = new System.Windows.Forms.Label();
            this.ServerPanel.SuspendLayout();
            this.StopBitsPanel.SuspendLayout();
            this.ParityPanel.SuspendLayout();
            this.DataBitsPanel.SuspendLayout();
            this.BaudRatePanel.SuspendLayout();
            this.PortNamePanel.SuspendLayout();
            this.NumberPanel.SuspendLayout();
            this.ConnectionWaitTimePanel.SuspendLayout();
            this.CommandWaitTimePanel.SuspendLayout();
            this.HangsUpDelayPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ServerPanel
            // 
            this.ServerPanel.Controls.Add(this.ServerCB);
            this.ServerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.ServerPanel.Location = new System.Drawing.Point(0, 0);
            this.ServerPanel.Name = "ServerPanel";
            this.ServerPanel.Size = new System.Drawing.Size(284, 30);
            this.ServerPanel.TabIndex = 31;
            // 
            // ServerCB
            // 
            this.ServerCB.AutoSize = true;
            this.ServerCB.Location = new System.Drawing.Point(4, 6);
            this.ServerCB.Name = "ServerCB";
            this.ServerCB.Size = new System.Drawing.Size(64, 17);
            this.ServerCB.TabIndex = 13;
            this.ServerCB.Text = "ServerX";
            this.ServerCB.UseVisualStyleBackColor = true;
            this.ServerCB.CheckedChanged += new System.EventHandler(this.ServerCB_CheckedChanged);
            // 
            // StopBitsPanel
            // 
            this.StopBitsPanel.Controls.Add(this.StopBitsLbl);
            this.StopBitsPanel.Controls.Add(this.StopBitsCB);
            this.StopBitsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.StopBitsPanel.Location = new System.Drawing.Point(0, 180);
            this.StopBitsPanel.Name = "StopBitsPanel";
            this.StopBitsPanel.Size = new System.Drawing.Size(284, 30);
            this.StopBitsPanel.TabIndex = 37;
            // 
            // StopBitsLbl
            // 
            this.StopBitsLbl.AutoSize = true;
            this.StopBitsLbl.Location = new System.Drawing.Point(4, 7);
            this.StopBitsLbl.Name = "StopBitsLbl";
            this.StopBitsLbl.Size = new System.Drawing.Size(56, 13);
            this.StopBitsLbl.TabIndex = 15;
            this.StopBitsLbl.Text = "Stop BitsX";
            // 
            // StopBitsCB
            // 
            this.StopBitsCB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StopBitsCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.StopBitsCB.FormattingEnabled = true;
            this.StopBitsCB.Location = new System.Drawing.Point(122, 4);
            this.StopBitsCB.Name = "StopBitsCB";
            this.StopBitsCB.Size = new System.Drawing.Size(150, 21);
            this.StopBitsCB.TabIndex = 14;
            this.StopBitsCB.SelectedIndexChanged += new System.EventHandler(this.StopBitsCB_SelectedIndexChanged);
            // 
            // ParityPanel
            // 
            this.ParityPanel.Controls.Add(this.ParityLbl);
            this.ParityPanel.Controls.Add(this.ParityCB);
            this.ParityPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.ParityPanel.Location = new System.Drawing.Point(0, 150);
            this.ParityPanel.Name = "ParityPanel";
            this.ParityPanel.Size = new System.Drawing.Size(284, 30);
            this.ParityPanel.TabIndex = 36;
            // 
            // ParityLbl
            // 
            this.ParityLbl.AutoSize = true;
            this.ParityLbl.Location = new System.Drawing.Point(4, 7);
            this.ParityLbl.Name = "ParityLbl";
            this.ParityLbl.Size = new System.Drawing.Size(40, 13);
            this.ParityLbl.TabIndex = 15;
            this.ParityLbl.Text = "ParityX";
            // 
            // ParityCB
            // 
            this.ParityCB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ParityCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ParityCB.FormattingEnabled = true;
            this.ParityCB.Location = new System.Drawing.Point(122, 4);
            this.ParityCB.Name = "ParityCB";
            this.ParityCB.Size = new System.Drawing.Size(150, 21);
            this.ParityCB.TabIndex = 14;
            this.ParityCB.SelectedIndexChanged += new System.EventHandler(this.ParityCB_SelectedIndexChanged);
            // 
            // DataBitsPanel
            // 
            this.DataBitsPanel.Controls.Add(this.DataBitsCB);
            this.DataBitsPanel.Controls.Add(this.DataBitsLbl);
            this.DataBitsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.DataBitsPanel.Location = new System.Drawing.Point(0, 120);
            this.DataBitsPanel.Name = "DataBitsPanel";
            this.DataBitsPanel.Size = new System.Drawing.Size(284, 30);
            this.DataBitsPanel.TabIndex = 35;
            // 
            // DataBitsCB
            // 
            this.DataBitsCB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DataBitsCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DataBitsCB.FormattingEnabled = true;
            this.DataBitsCB.Location = new System.Drawing.Point(122, 4);
            this.DataBitsCB.Name = "DataBitsCB";
            this.DataBitsCB.Size = new System.Drawing.Size(150, 21);
            this.DataBitsCB.TabIndex = 15;
            this.DataBitsCB.SelectedIndexChanged += new System.EventHandler(this.DataBitsCB_SelectedIndexChanged);
            // 
            // DataBitsLbl
            // 
            this.DataBitsLbl.AutoSize = true;
            this.DataBitsLbl.Location = new System.Drawing.Point(4, 7);
            this.DataBitsLbl.Name = "DataBitsLbl";
            this.DataBitsLbl.Size = new System.Drawing.Size(57, 13);
            this.DataBitsLbl.TabIndex = 12;
            this.DataBitsLbl.Text = "Data BitsX";
            // 
            // BaudRatePanel
            // 
            this.BaudRatePanel.Controls.Add(this.BaudRateCB);
            this.BaudRatePanel.Controls.Add(this.BaudRateLbl);
            this.BaudRatePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.BaudRatePanel.Location = new System.Drawing.Point(0, 90);
            this.BaudRatePanel.Name = "BaudRatePanel";
            this.BaudRatePanel.Size = new System.Drawing.Size(284, 30);
            this.BaudRatePanel.TabIndex = 34;
            // 
            // BaudRateCB
            // 
            this.BaudRateCB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BaudRateCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.BaudRateCB.FormattingEnabled = true;
            this.BaudRateCB.Location = new System.Drawing.Point(122, 5);
            this.BaudRateCB.Name = "BaudRateCB";
            this.BaudRateCB.Size = new System.Drawing.Size(150, 21);
            this.BaudRateCB.TabIndex = 16;
            this.BaudRateCB.SelectedIndexChanged += new System.EventHandler(this.BaudRateCB_SelectedIndexChanged);
            // 
            // BaudRateLbl
            // 
            this.BaudRateLbl.AutoSize = true;
            this.BaudRateLbl.Location = new System.Drawing.Point(4, 7);
            this.BaudRateLbl.Name = "BaudRateLbl";
            this.BaudRateLbl.Size = new System.Drawing.Size(65, 13);
            this.BaudRateLbl.TabIndex = 10;
            this.BaudRateLbl.Text = "Baud RateX";
            // 
            // PortNamePanel
            // 
            this.PortNamePanel.Controls.Add(this.PortNameCB);
            this.PortNamePanel.Controls.Add(this.PortNameLbl);
            this.PortNamePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.PortNamePanel.Location = new System.Drawing.Point(0, 60);
            this.PortNamePanel.Name = "PortNamePanel";
            this.PortNamePanel.Size = new System.Drawing.Size(284, 30);
            this.PortNamePanel.TabIndex = 33;
            // 
            // PortNameCB
            // 
            this.PortNameCB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PortNameCB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PortNameCB.FormattingEnabled = true;
            this.PortNameCB.Location = new System.Drawing.Point(122, 5);
            this.PortNameCB.Name = "PortNameCB";
            this.PortNameCB.Size = new System.Drawing.Size(150, 21);
            this.PortNameCB.TabIndex = 15;
            this.PortNameCB.SelectedIndexChanged += new System.EventHandler(this.PortNameCB_SelectedIndexChanged_1);
            // 
            // PortNameLbl
            // 
            this.PortNameLbl.AutoSize = true;
            this.PortNameLbl.Location = new System.Drawing.Point(4, 8);
            this.PortNameLbl.Name = "PortNameLbl";
            this.PortNameLbl.Size = new System.Drawing.Size(33, 13);
            this.PortNameLbl.TabIndex = 12;
            this.PortNameLbl.Text = "PortX";
            // 
            // NumberPanel
            // 
            this.NumberPanel.Controls.Add(this.NumberTB);
            this.NumberPanel.Controls.Add(this.NumberLbl);
            this.NumberPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.NumberPanel.Location = new System.Drawing.Point(0, 30);
            this.NumberPanel.Name = "NumberPanel";
            this.NumberPanel.Size = new System.Drawing.Size(284, 30);
            this.NumberPanel.TabIndex = 32;
            // 
            // NumberTB
            // 
            this.NumberTB.Location = new System.Drawing.Point(122, 5);
            this.NumberTB.Name = "NumberTB";
            this.NumberTB.Size = new System.Drawing.Size(150, 20);
            this.NumberTB.TabIndex = 14;
            this.NumberTB.TextChanged += new System.EventHandler(this.NumberTB_TextChanged);
            // 
            // NumberLbl
            // 
            this.NumberLbl.AutoSize = true;
            this.NumberLbl.Location = new System.Drawing.Point(4, 8);
            this.NumberLbl.Name = "NumberLbl";
            this.NumberLbl.Size = new System.Drawing.Size(88, 13);
            this.NumberLbl.TabIndex = 12;
            this.NumberLbl.Text = "Phone NumberX:";
            // 
            // ConnectionWaitTimePanel
            // 
            this.ConnectionWaitTimePanel.Controls.Add(this.ConnectionWaitTimeTb);
            this.ConnectionWaitTimePanel.Controls.Add(this.ConnectionWaitTimeLbl);
            this.ConnectionWaitTimePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.ConnectionWaitTimePanel.Location = new System.Drawing.Point(0, 210);
            this.ConnectionWaitTimePanel.Name = "ConnectionWaitTimePanel";
            this.ConnectionWaitTimePanel.Size = new System.Drawing.Size(284, 30);
            this.ConnectionWaitTimePanel.TabIndex = 38;
            // 
            // ConnectionWaitTimeTb
            // 
            this.ConnectionWaitTimeTb.Location = new System.Drawing.Point(122, 5);
            this.ConnectionWaitTimeTb.Name = "ConnectionWaitTimeTb";
            this.ConnectionWaitTimeTb.Size = new System.Drawing.Size(150, 20);
            this.ConnectionWaitTimeTb.TabIndex = 14;
            this.ConnectionWaitTimeTb.TextChanged += new System.EventHandler(this.ConnectionWaitTimeTb_TextChanged);
            // 
            // ConnectionWaitTimeLbl
            // 
            this.ConnectionWaitTimeLbl.AutoSize = true;
            this.ConnectionWaitTimeLbl.Location = new System.Drawing.Point(4, 8);
            this.ConnectionWaitTimeLbl.Name = "ConnectionWaitTimeLbl";
            this.ConnectionWaitTimeLbl.Size = new System.Drawing.Size(116, 13);
            this.ConnectionWaitTimeLbl.TabIndex = 12;
            this.ConnectionWaitTimeLbl.Text = "ConnectionWaitTimeX:";
            // 
            // CommandWaitTimePanel
            // 
            this.CommandWaitTimePanel.Controls.Add(this.CommandWaitTimeTb);
            this.CommandWaitTimePanel.Controls.Add(this.CommandWaitTimeLbl);
            this.CommandWaitTimePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.CommandWaitTimePanel.Location = new System.Drawing.Point(0, 240);
            this.CommandWaitTimePanel.Name = "CommandWaitTimePanel";
            this.CommandWaitTimePanel.Size = new System.Drawing.Size(284, 30);
            this.CommandWaitTimePanel.TabIndex = 39;
            // 
            // CommandWaitTimeTb
            // 
            this.CommandWaitTimeTb.Location = new System.Drawing.Point(122, 5);
            this.CommandWaitTimeTb.Name = "CommandWaitTimeTb";
            this.CommandWaitTimeTb.Size = new System.Drawing.Size(150, 20);
            this.CommandWaitTimeTb.TabIndex = 14;
            this.CommandWaitTimeTb.TextChanged += new System.EventHandler(this.CommandWaitTimeTb_TextChanged);
            // 
            // CommandWaitTimeLbl
            // 
            this.CommandWaitTimeLbl.AutoSize = true;
            this.CommandWaitTimeLbl.Location = new System.Drawing.Point(4, 8);
            this.CommandWaitTimeLbl.Name = "CommandWaitTimeLbl";
            this.CommandWaitTimeLbl.Size = new System.Drawing.Size(112, 13);
            this.CommandWaitTimeLbl.TabIndex = 12;
            this.CommandWaitTimeLbl.Text = "Command WaitTimeX:";
            // 
            // HangsUpDelayPanel
            // 
            this.HangsUpDelayPanel.Controls.Add(this.HangsUpDelayTb);
            this.HangsUpDelayPanel.Controls.Add(this.HangsUpDelayLbl);
            this.HangsUpDelayPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.HangsUpDelayPanel.Location = new System.Drawing.Point(0, 270);
            this.HangsUpDelayPanel.Name = "HangsUpDelayPanel";
            this.HangsUpDelayPanel.Size = new System.Drawing.Size(284, 30);
            this.HangsUpDelayPanel.TabIndex = 40;
            // 
            // HangsUpDelayTb
            // 
            this.HangsUpDelayTb.Location = new System.Drawing.Point(122, 5);
            this.HangsUpDelayTb.Name = "HangsUpDelayTb";
            this.HangsUpDelayTb.Size = new System.Drawing.Size(150, 20);
            this.HangsUpDelayTb.TabIndex = 14;
            this.HangsUpDelayTb.TextChanged += new System.EventHandler(this.HangsUpDelayTb_TextChanged);
            // 
            // HangsUpDelayLbl
            // 
            this.HangsUpDelayLbl.AutoSize = true;
            this.HangsUpDelayLbl.Location = new System.Drawing.Point(4, 8);
            this.HangsUpDelayLbl.Name = "HangsUpDelayLbl";
            this.HangsUpDelayLbl.Size = new System.Drawing.Size(84, 13);
            this.HangsUpDelayLbl.TabIndex = 12;
            this.HangsUpDelayLbl.Text = "Hangs up delay:";
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 317);
            this.Controls.Add(this.HangsUpDelayPanel);
            this.Controls.Add(this.CommandWaitTimePanel);
            this.Controls.Add(this.ConnectionWaitTimePanel);
            this.Controls.Add(this.StopBitsPanel);
            this.Controls.Add(this.ParityPanel);
            this.Controls.Add(this.DataBitsPanel);
            this.Controls.Add(this.BaudRatePanel);
            this.Controls.Add(this.PortNamePanel);
            this.Controls.Add(this.NumberPanel);
            this.Controls.Add(this.ServerPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Terminal Settings";
            this.ServerPanel.ResumeLayout(false);
            this.ServerPanel.PerformLayout();
            this.StopBitsPanel.ResumeLayout(false);
            this.StopBitsPanel.PerformLayout();
            this.ParityPanel.ResumeLayout(false);
            this.ParityPanel.PerformLayout();
            this.DataBitsPanel.ResumeLayout(false);
            this.DataBitsPanel.PerformLayout();
            this.BaudRatePanel.ResumeLayout(false);
            this.BaudRatePanel.PerformLayout();
            this.PortNamePanel.ResumeLayout(false);
            this.PortNamePanel.PerformLayout();
            this.NumberPanel.ResumeLayout(false);
            this.NumberPanel.PerformLayout();
            this.ConnectionWaitTimePanel.ResumeLayout(false);
            this.ConnectionWaitTimePanel.PerformLayout();
            this.CommandWaitTimePanel.ResumeLayout(false);
            this.CommandWaitTimePanel.PerformLayout();
            this.HangsUpDelayPanel.ResumeLayout(false);
            this.HangsUpDelayPanel.PerformLayout();
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel ServerPanel;
    private System.Windows.Forms.CheckBox ServerCB;
    private System.Windows.Forms.Panel StopBitsPanel;
    private System.Windows.Forms.Label StopBitsLbl;
    private System.Windows.Forms.ComboBox StopBitsCB;
    private System.Windows.Forms.Panel ParityPanel;
    private System.Windows.Forms.Label ParityLbl;
    private System.Windows.Forms.ComboBox ParityCB;
    private System.Windows.Forms.Panel DataBitsPanel;
    private System.Windows.Forms.ComboBox DataBitsCB;
    private System.Windows.Forms.Label DataBitsLbl;
    private System.Windows.Forms.Panel BaudRatePanel;
    private System.Windows.Forms.ComboBox BaudRateCB;
    private System.Windows.Forms.Label BaudRateLbl;
    private System.Windows.Forms.Panel PortNamePanel;
    private System.Windows.Forms.ComboBox PortNameCB;
    private System.Windows.Forms.Label PortNameLbl;
    private System.Windows.Forms.Panel NumberPanel;
    private System.Windows.Forms.TextBox NumberTB;
    private System.Windows.Forms.Label NumberLbl;
        private System.Windows.Forms.Panel ConnectionWaitTimePanel;
        private System.Windows.Forms.TextBox ConnectionWaitTimeTb;
        private System.Windows.Forms.Label ConnectionWaitTimeLbl;
        private System.Windows.Forms.Panel CommandWaitTimePanel;
        private System.Windows.Forms.TextBox CommandWaitTimeTb;
        private System.Windows.Forms.Label CommandWaitTimeLbl;
        private System.Windows.Forms.Panel HangsUpDelayPanel;
        private System.Windows.Forms.TextBox HangsUpDelayTb;
        private System.Windows.Forms.Label HangsUpDelayLbl;
    }
}