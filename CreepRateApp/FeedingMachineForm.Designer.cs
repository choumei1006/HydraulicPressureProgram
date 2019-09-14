namespace CreepRateApp
{
    partial class FeedingMachineForm
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
            this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
            this.xtp_HandleMode = new DevExpress.XtraTab.XtraTabPage();
            this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
            this.bParamInput = new DevExpress.XtraEditors.SimpleButton();
            this.bSwitch = new DevExpress.XtraEditors.ToggleSwitch();
            this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
            this.aParamInput = new DevExpress.XtraEditors.SimpleButton();
            this.aSwitch = new DevExpress.XtraEditors.ToggleSwitch();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
            this.te_speed = new DevExpress.XtraEditors.TextEdit();
            this.te_FeedLength = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.xtp_AutoMode = new DevExpress.XtraTab.XtraTabPage();
            this.autoModeSwitch = new DevExpress.XtraEditors.ToggleSwitch();
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
            this.xtraTabControl1.SuspendLayout();
            this.xtp_HandleMode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
            this.groupControl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bSwitch.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
            this.groupControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.aSwitch.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.te_speed.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.te_FeedLength.Properties)).BeginInit();
            this.xtp_AutoMode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.autoModeSwitch.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // xtraTabControl1
            // 
            this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
            this.xtraTabControl1.Name = "xtraTabControl1";
            this.xtraTabControl1.SelectedTabPage = this.xtp_HandleMode;
            this.xtraTabControl1.Size = new System.Drawing.Size(298, 281);
            this.xtraTabControl1.TabIndex = 0;
            this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
            this.xtp_HandleMode,
            this.xtp_AutoMode});
            // 
            // xtp_HandleMode
            // 
            this.xtp_HandleMode.Controls.Add(this.groupControl2);
            this.xtp_HandleMode.Controls.Add(this.groupControl1);
            this.xtp_HandleMode.Controls.Add(this.labelControl4);
            this.xtp_HandleMode.Controls.Add(this.labelControl3);
            this.xtp_HandleMode.Controls.Add(this.te_speed);
            this.xtp_HandleMode.Controls.Add(this.te_FeedLength);
            this.xtp_HandleMode.Controls.Add(this.labelControl2);
            this.xtp_HandleMode.Controls.Add(this.labelControl1);
            this.xtp_HandleMode.Name = "xtp_HandleMode";
            this.xtp_HandleMode.Size = new System.Drawing.Size(296, 255);
            this.xtp_HandleMode.Text = "手动模式";
            // 
            // groupControl2
            // 
            this.groupControl2.Controls.Add(this.bParamInput);
            this.groupControl2.Controls.Add(this.bSwitch);
            this.groupControl2.Location = new System.Drawing.Point(12, 180);
            this.groupControl2.Name = "groupControl2";
            this.groupControl2.Size = new System.Drawing.Size(264, 58);
            this.groupControl2.TabIndex = 11;
            this.groupControl2.Text = "B线";
            // 
            // bParamInput
            // 
            this.bParamInput.Location = new System.Drawing.Point(41, 25);
            this.bParamInput.Name = "bParamInput";
            this.bParamInput.Size = new System.Drawing.Size(75, 23);
            this.bParamInput.TabIndex = 7;
            this.bParamInput.Text = "参数写入";
            this.bParamInput.Click += new System.EventHandler(this.bParamInput_Click);
            // 
            // bSwitch
            // 
            this.bSwitch.Location = new System.Drawing.Point(141, 23);
            this.bSwitch.Name = "bSwitch";
            this.bSwitch.Properties.GlyphAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.bSwitch.Properties.OffText = "关";
            this.bSwitch.Properties.OnText = "开";
            this.bSwitch.Size = new System.Drawing.Size(95, 25);
            this.bSwitch.TabIndex = 9;
            this.bSwitch.Toggled += new System.EventHandler(this.bSwitch_Toggled);
            // 
            // groupControl1
            // 
            this.groupControl1.Controls.Add(this.aParamInput);
            this.groupControl1.Controls.Add(this.aSwitch);
            this.groupControl1.Location = new System.Drawing.Point(12, 97);
            this.groupControl1.Name = "groupControl1";
            this.groupControl1.Size = new System.Drawing.Size(264, 58);
            this.groupControl1.TabIndex = 10;
            this.groupControl1.Text = "A线";
            // 
            // aParamInput
            // 
            this.aParamInput.Location = new System.Drawing.Point(41, 25);
            this.aParamInput.Name = "aParamInput";
            this.aParamInput.Size = new System.Drawing.Size(75, 23);
            this.aParamInput.TabIndex = 6;
            this.aParamInput.Text = "参数写入";
            this.aParamInput.Click += new System.EventHandler(this.aParamInput_Click);
            // 
            // aSwitch
            // 
            this.aSwitch.Location = new System.Drawing.Point(141, 23);
            this.aSwitch.Name = "aSwitch";
            this.aSwitch.Properties.GlyphAlignment = DevExpress.Utils.HorzAlignment.Default;
            this.aSwitch.Properties.OffText = "关";
            this.aSwitch.Properties.OnText = "开";
            this.aSwitch.Size = new System.Drawing.Size(95, 25);
            this.aSwitch.TabIndex = 8;
            this.aSwitch.Toggled += new System.EventHandler(this.aSwitch_Toggled);
            // 
            // labelControl4
            // 
            this.labelControl4.Location = new System.Drawing.Point(240, 61);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(53, 14);
            this.labelControl4.TabIndex = 5;
            this.labelControl4.Text = "（米/分）";
            // 
            // labelControl3
            // 
            this.labelControl3.Location = new System.Drawing.Point(240, 17);
            this.labelControl3.Name = "labelControl3";
            this.labelControl3.Size = new System.Drawing.Size(36, 14);
            this.labelControl3.TabIndex = 4;
            this.labelControl3.Text = "（米）";
            // 
            // te_speed
            // 
            this.te_speed.Location = new System.Drawing.Point(79, 58);
            this.te_speed.Name = "te_speed";
            this.te_speed.Size = new System.Drawing.Size(155, 20);
            this.te_speed.TabIndex = 3;
            // 
            // te_FeedLength
            // 
            this.te_FeedLength.Location = new System.Drawing.Point(79, 14);
            this.te_FeedLength.Name = "te_FeedLength";
            this.te_FeedLength.Size = new System.Drawing.Size(155, 20);
            this.te_FeedLength.TabIndex = 2;
            // 
            // labelControl2
            // 
            this.labelControl2.Location = new System.Drawing.Point(12, 61);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(60, 14);
            this.labelControl2.TabIndex = 1;
            this.labelControl2.Text = "喂料速度：";
            // 
            // labelControl1
            // 
            this.labelControl1.Location = new System.Drawing.Point(12, 17);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(60, 14);
            this.labelControl1.TabIndex = 0;
            this.labelControl1.Text = "喂料长度：";
            // 
            // xtp_AutoMode
            // 
            this.xtp_AutoMode.Controls.Add(this.autoModeSwitch);
            this.xtp_AutoMode.Name = "xtp_AutoMode";
            this.xtp_AutoMode.Size = new System.Drawing.Size(292, 252);
            this.xtp_AutoMode.Text = "自动模式";
            // 
            // autoModeSwitch
            // 
            this.autoModeSwitch.Location = new System.Drawing.Point(101, 80);
            this.autoModeSwitch.Name = "autoModeSwitch";
            this.autoModeSwitch.Properties.OffText = "关";
            this.autoModeSwitch.Properties.OnText = "开";
            this.autoModeSwitch.Size = new System.Drawing.Size(95, 25);
            this.autoModeSwitch.TabIndex = 0;
            this.autoModeSwitch.Toggled += new System.EventHandler(this.autoModeSwitch_Toggled);
            // 
            // FeedingMachineForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(298, 281);
            this.Controls.Add(this.xtraTabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FeedingMachineForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "喂料机";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FeedingMachineForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FeedingMachineForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
            this.xtraTabControl1.ResumeLayout(false);
            this.xtp_HandleMode.ResumeLayout(false);
            this.xtp_HandleMode.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
            this.groupControl2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bSwitch.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
            this.groupControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.aSwitch.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.te_speed.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.te_FeedLength.Properties)).EndInit();
            this.xtp_AutoMode.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.autoModeSwitch.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
        private DevExpress.XtraTab.XtraTabPage xtp_HandleMode;
        private DevExpress.XtraTab.XtraTabPage xtp_AutoMode;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private DevExpress.XtraEditors.LabelControl labelControl3;
        private DevExpress.XtraEditors.TextEdit te_speed;
        private DevExpress.XtraEditors.TextEdit te_FeedLength;
        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.SimpleButton aParamInput;
        private DevExpress.XtraEditors.GroupControl groupControl2;
        private DevExpress.XtraEditors.SimpleButton bParamInput;
        private DevExpress.XtraEditors.ToggleSwitch bSwitch;
        private DevExpress.XtraEditors.GroupControl groupControl1;
        private DevExpress.XtraEditors.ToggleSwitch aSwitch;
        private DevExpress.XtraEditors.ToggleSwitch autoModeSwitch;

    }
}