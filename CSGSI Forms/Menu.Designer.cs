using System.Windows.Forms;

namespace CSGSI_Forms
{
    partial class Menu
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.spVolume = new System.Windows.Forms.TrackBar();
            this.Reloadmusic = new System.Windows.Forms.Button();
            this.LocalAllPlayers = new System.Windows.Forms.CheckBox();
            this.Local = new System.Windows.Forms.GroupBox();
            this.LocalFreeze = new System.Windows.Forms.CheckBox();
            this.LocalFlashbang = new System.Windows.Forms.CheckBox();
            this.LocalKillingSpree = new System.Windows.Forms.CheckBox();
            this.LocalMusic = new System.Windows.Forms.CheckBox();
            this.Network = new System.Windows.Forms.GroupBox();
            this.ShowOverlay = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.address = new System.Windows.Forms.TextBox();
            this.NetworkHealth = new System.Windows.Forms.CheckBox();
            this.ConnectButton = new System.Windows.Forms.Button();
            this.LocalHostButton = new System.Windows.Forms.Button();
            this.ServerLog = new System.Windows.Forms.TextBox();
            this.ServerButton = new System.Windows.Forms.Button();
            this.StopServerButton = new System.Windows.Forms.Button();
            this.SettingsButton = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize) (this.spVolume)).BeginInit();
            this.Local.SuspendLayout();
            this.Network.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.SettingsButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // spVolume
            // 
            this.spVolume.Location = new System.Drawing.Point(12, 12);
            this.spVolume.Maximum = 100;
            this.spVolume.Name = "spVolume";
            this.spVolume.Size = new System.Drawing.Size(268, 45);
            this.spVolume.TabIndex = 3;
            this.spVolume.TickFrequency = 5;
            this.spVolume.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.spVolume.Value = 100;
            this.spVolume.ValueChanged += new System.EventHandler(this.SpecialEvent);
            // 
            // Reloadmusic
            // 
            this.Reloadmusic.Location = new System.Drawing.Point(53, 63);
            this.Reloadmusic.Name = "Reloadmusic";
            this.Reloadmusic.Size = new System.Drawing.Size(186, 30);
            this.Reloadmusic.TabIndex = 1;
            this.Reloadmusic.Text = "Reload music";
            this.Reloadmusic.UseVisualStyleBackColor = true;
            this.Reloadmusic.Click += new System.EventHandler(this.SpecialEvent);
            // 
            // LocalAllPlayers
            // 
            this.LocalAllPlayers.AutoSize = true;
            this.LocalAllPlayers.Checked = true;
            this.LocalAllPlayers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.LocalAllPlayers.Location = new System.Drawing.Point(6, 111);
            this.LocalAllPlayers.Name = "LocalAllPlayers";
            this.LocalAllPlayers.Size = new System.Drawing.Size(74, 17);
            this.LocalAllPlayers.TabIndex = 5;
            this.LocalAllPlayers.Text = "All Players";
            this.LocalAllPlayers.UseVisualStyleBackColor = true;
            this.LocalAllPlayers.UseWaitCursor = true;
            // 
            // Local
            // 
            this.Local.Controls.Add(this.LocalFreeze);
            this.Local.Controls.Add(this.LocalFlashbang);
            this.Local.Controls.Add(this.LocalKillingSpree);
            this.Local.Controls.Add(this.LocalMusic);
            this.Local.Controls.Add(this.LocalAllPlayers);
            this.Local.Location = new System.Drawing.Point(12, 99);
            this.Local.Name = "Local";
            this.Local.Size = new System.Drawing.Size(118, 132);
            this.Local.TabIndex = 8;
            this.Local.TabStop = false;
            this.Local.Text = "Local:";
            // 
            // LocalFreeze
            // 
            this.LocalFreeze.AutoSize = true;
            this.LocalFreeze.Checked = true;
            this.LocalFreeze.CheckState = System.Windows.Forms.CheckState.Checked;
            this.LocalFreeze.Location = new System.Drawing.Point(6, 88);
            this.LocalFreeze.Name = "LocalFreeze";
            this.LocalFreeze.Size = new System.Drawing.Size(72, 17);
            this.LocalFreeze.TabIndex = 6;
            this.LocalFreeze.Text = "Перерыв";
            this.LocalFreeze.UseVisualStyleBackColor = true;
            this.LocalFreeze.UseWaitCursor = true;
            // 
            // LocalFlashbang
            // 
            this.LocalFlashbang.AutoSize = true;
            this.LocalFlashbang.Checked = true;
            this.LocalFlashbang.CheckState = System.Windows.Forms.CheckState.Checked;
            this.LocalFlashbang.Location = new System.Drawing.Point(6, 19);
            this.LocalFlashbang.Name = "LocalFlashbang";
            this.LocalFlashbang.Size = new System.Drawing.Size(97, 17);
            this.LocalFlashbang.TabIndex = 5;
            this.LocalFlashbang.Text = "Flash and Burn";
            this.LocalFlashbang.UseVisualStyleBackColor = true;
            this.LocalFlashbang.UseWaitCursor = true;
            // 
            // LocalKillingSpree
            // 
            this.LocalKillingSpree.AutoSize = true;
            this.LocalKillingSpree.Checked = true;
            this.LocalKillingSpree.CheckState = System.Windows.Forms.CheckState.Checked;
            this.LocalKillingSpree.Location = new System.Drawing.Point(6, 42);
            this.LocalKillingSpree.Name = "LocalKillingSpree";
            this.LocalKillingSpree.Size = new System.Drawing.Size(100, 17);
            this.LocalKillingSpree.TabIndex = 5;
            this.LocalKillingSpree.Text = "Серия убийств";
            this.LocalKillingSpree.UseVisualStyleBackColor = true;
            this.LocalKillingSpree.UseWaitCursor = true;
            // 
            // LocalMusic
            // 
            this.LocalMusic.AutoSize = true;
            this.LocalMusic.Checked = true;
            this.LocalMusic.CheckState = System.Windows.Forms.CheckState.Checked;
            this.LocalMusic.Location = new System.Drawing.Point(6, 65);
            this.LocalMusic.Name = "LocalMusic";
            this.LocalMusic.Size = new System.Drawing.Size(66, 17);
            this.LocalMusic.TabIndex = 5;
            this.LocalMusic.Text = "Музыка";
            this.LocalMusic.UseVisualStyleBackColor = true;
            this.LocalMusic.UseWaitCursor = true;
            // 
            // Network
            // 
            this.Network.Controls.Add(this.ShowOverlay);
            this.Network.Controls.Add(this.label1);
            this.Network.Controls.Add(this.address);
            this.Network.Controls.Add(this.NetworkHealth);
            this.Network.Location = new System.Drawing.Point(136, 99);
            this.Network.Name = "Network";
            this.Network.Size = new System.Drawing.Size(144, 98);
            this.Network.TabIndex = 9;
            this.Network.TabStop = false;
            this.Network.Text = "Network:";
            // 
            // ShowOverlay
            // 
            this.ShowOverlay.AutoSize = true;
            this.ShowOverlay.Location = new System.Drawing.Point(6, 36);
            this.ShowOverlay.Name = "ShowOverlay";
            this.ShowOverlay.Size = new System.Drawing.Size(122, 17);
            this.ShowOverlay.TabIndex = 8;
            this.ShowOverlay.Text = "Включить Оверлей";
            this.ShowOverlay.UseVisualStyleBackColor = true;
            this.ShowOverlay.CheckedChanged += new System.EventHandler(this.SpecialEvent);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "IP:Port";
            // 
            // address
            // 
            this.address.Location = new System.Drawing.Point(6, 72);
            this.address.Name = "address";
            this.address.Size = new System.Drawing.Size(132, 20);
            this.address.TabIndex = 6;
            // 
            // NetworkHealth
            // 
            this.NetworkHealth.AutoSize = true;
            this.NetworkHealth.Checked = true;
            this.NetworkHealth.CheckState = System.Windows.Forms.CheckState.Checked;
            this.NetworkHealth.Location = new System.Drawing.Point(6, 19);
            this.NetworkHealth.Name = "NetworkHealth";
            this.NetworkHealth.Size = new System.Drawing.Size(134, 17);
            this.NetworkHealth.TabIndex = 5;
            this.NetworkHealth.Text = "Low Health And Death";
            this.NetworkHealth.UseVisualStyleBackColor = true;
            this.NetworkHealth.UseWaitCursor = true;
            // 
            // ConnectButton
            // 
            this.ConnectButton.Enabled = false;
            this.ConnectButton.Location = new System.Drawing.Point(136, 203);
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(68, 23);
            this.ConnectButton.TabIndex = 6;
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.UseVisualStyleBackColor = true;
            this.ConnectButton.Click += new System.EventHandler(this.SpecialEvent);
            // 
            // LocalHostButton
            // 
            this.LocalHostButton.Location = new System.Drawing.Point(206, 203);
            this.LocalHostButton.Name = "LocalHostButton";
            this.LocalHostButton.Size = new System.Drawing.Size(68, 23);
            this.LocalHostButton.TabIndex = 10;
            this.LocalHostButton.Text = "Localhost";
            this.LocalHostButton.UseVisualStyleBackColor = true;
            this.LocalHostButton.Click += new System.EventHandler(this.SpecialEvent);
            // 
            // ServerLog
            // 
            this.ServerLog.Enabled = false;
            this.ServerLog.Location = new System.Drawing.Point(12, 257);
            this.ServerLog.Multiline = true;
            this.ServerLog.Name = "ServerLog";
            this.ServerLog.Size = new System.Drawing.Size(268, 72);
            this.ServerLog.TabIndex = 11;
            // 
            // ServerButton
            // 
            this.ServerButton.Location = new System.Drawing.Point(136, 228);
            this.ServerButton.Name = "ServerButton";
            this.ServerButton.Size = new System.Drawing.Size(84, 23);
            this.ServerButton.TabIndex = 12;
            this.ServerButton.Text = "Start Server";
            this.ServerButton.UseVisualStyleBackColor = true;
            this.ServerButton.Click += new System.EventHandler(this.SpecialEvent);
            // 
            // StopServerButton
            // 
            this.StopServerButton.Enabled = false;
            this.StopServerButton.Location = new System.Drawing.Point(226, 228);
            this.StopServerButton.Name = "StopServerButton";
            this.StopServerButton.Size = new System.Drawing.Size(48, 23);
            this.StopServerButton.TabIndex = 13;
            this.StopServerButton.Text = "Stop";
            this.StopServerButton.UseVisualStyleBackColor = true;
            this.StopServerButton.Click += new System.EventHandler(this.SpecialEvent);
            // 
            // SettingsButton
            // 
            this.SettingsButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.SettingsButton.Image = global::CSGSI_Forms.Properties.Resources.Settings;
            this.SettingsButton.Location = new System.Drawing.Point(245, 63);
            this.SettingsButton.Name = "SettingsButton";
            this.SettingsButton.Size = new System.Drawing.Size(35, 30);
            this.SettingsButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.SettingsButton.TabIndex = 7;
            this.SettingsButton.TabStop = false;
            this.SettingsButton.Click += new System.EventHandler(this.SpecialEvent);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::CSGSI_Forms.Properties.Resources.On;
            this.pictureBox1.Location = new System.Drawing.Point(12, 63);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(35, 30);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Tag = "unMute";
            this.pictureBox1.Click += new System.EventHandler(this.MuteUnMute);
            // 
            // Menu
            // 
            this.AcceptButton = this.ConnectButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(289, 341);
            this.Controls.Add(this.StopServerButton);
            this.Controls.Add(this.ServerButton);
            this.Controls.Add(this.ServerLog);
            this.Controls.Add(this.LocalHostButton);
            this.Controls.Add(this.ConnectButton);
            this.Controls.Add(this.Network);
            this.Controls.Add(this.Local);
            this.Controls.Add(this.SettingsButton);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.spVolume);
            this.Controls.Add(this.Reloadmusic);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Menu";
            this.Text = "Offline";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CloseApplication);
            this.Shown += new System.EventHandler(this.StartThreads);
            ((System.ComponentModel.ISupportInitialize) (this.spVolume)).EndInit();
            this.Local.ResumeLayout(false);
            this.Local.PerformLayout();
            this.Network.ResumeLayout(false);
            this.Network.PerformLayout();
            ((System.ComponentModel.ISupportInitialize) (this.SettingsButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize) (this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
        private System.Windows.Forms.TrackBar spVolume;
        private System.Windows.Forms.Button Reloadmusic;
        private PictureBox pictureBox1;
        private CheckBox LocalAllPlayers;
        private PictureBox SettingsButton;
        private GroupBox Local;
        private GroupBox Network;
        private CheckBox LocalKillingSpree;
        private CheckBox LocalFlashbang;
        private CheckBox LocalMusic;
        private CheckBox NetworkHealth;
        private Button ConnectButton;
        private Label label1;
        private TextBox address;
        private CheckBox LocalFreeze;
        private Button LocalHostButton;
        private TextBox ServerLog;
        private Button ServerButton;
        private Button StopServerButton;
        private CheckBox ShowOverlay;
    }
}

