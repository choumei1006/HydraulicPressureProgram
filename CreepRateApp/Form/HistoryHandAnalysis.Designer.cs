namespace CreepRateApp.Form
{
    partial class HistoryHandAnalysis
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HistoryHandAnalysis));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            this.gridControl1 = new DevExpress.XtraGrid.GridControl();
            this.analysisModelBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colFileName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTAL = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTEU = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTER = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTer_Teu = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTal_Teu = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colRuHuaLv = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colIsHuiZhuTie = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAction = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemButtonEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.analysisModelBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEdit1)).BeginInit();
            this.SuspendLayout();
            // 
            // gridControl1
            // 
            this.gridControl1.Cursor = System.Windows.Forms.Cursors.Default;
            this.gridControl1.DataSource = this.analysisModelBindingSource;
            this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl1.Location = new System.Drawing.Point(0, 0);
            this.gridControl1.MainView = this.gridView1;
            this.gridControl1.Name = "gridControl1";
            this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemButtonEdit1});
            this.gridControl1.Size = new System.Drawing.Size(1053, 536);
            this.gridControl1.TabIndex = 0;
            this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // analysisModelBindingSource
            // 
            this.analysisModelBindingSource.DataSource = typeof(CreepRateApp.entity.AnalysisModel);
            // 
            // gridView1
            // 
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colFileName,
            this.colTime,
            this.colTAL,
            this.colTEU,
            this.colTER,
            this.colTer_Teu,
            this.colTal_Teu,
            this.colRuHuaLv,
            this.colIsHuiZhuTie,
            this.colId,
            this.colAction});
            this.gridView1.GridControl = this.gridControl1;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsView.ShowDetailButtons = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // colFileName
            // 
            this.colFileName.Caption = "文件名";
            this.colFileName.FieldName = "FileName";
            this.colFileName.Name = "colFileName";
            this.colFileName.OptionsColumn.FixedWidth = true;
            this.colFileName.Visible = true;
            this.colFileName.VisibleIndex = 0;
            this.colFileName.Width = 140;
            // 
            // colTime
            // 
            this.colTime.Caption = "分析时间";
            this.colTime.FieldName = "Time";
            this.colTime.Name = "colTime";
            this.colTime.OptionsColumn.FixedWidth = true;
            this.colTime.Visible = true;
            this.colTime.VisibleIndex = 1;
            this.colTime.Width = 140;
            // 
            // colTAL
            // 
            this.colTAL.Caption = "初晶温度TAL";
            this.colTAL.FieldName = "TAL";
            this.colTAL.Name = "colTAL";
            this.colTAL.Visible = true;
            this.colTAL.VisibleIndex = 2;
            this.colTAL.Width = 123;
            // 
            // colTEU
            // 
            this.colTEU.Caption = "共晶最低温度TEU";
            this.colTEU.FieldName = "TEU";
            this.colTEU.Name = "colTEU";
            this.colTEU.Visible = true;
            this.colTEU.VisibleIndex = 3;
            this.colTEU.Width = 129;
            // 
            // colTER
            // 
            this.colTER.Caption = "共晶最高温度TER";
            this.colTER.FieldName = "TER";
            this.colTER.Name = "colTER";
            this.colTER.Visible = true;
            this.colTER.VisibleIndex = 4;
            this.colTER.Width = 128;
            // 
            // colTer_Teu
            // 
            this.colTer_Teu.Caption = "TER-TEU";
            this.colTer_Teu.FieldName = "Ter_Teu";
            this.colTer_Teu.Name = "colTer_Teu";
            this.colTer_Teu.Visible = true;
            this.colTer_Teu.VisibleIndex = 5;
            this.colTer_Teu.Width = 76;
            // 
            // colTal_Teu
            // 
            this.colTal_Teu.Caption = "TAL-TEU";
            this.colTal_Teu.FieldName = "Tal_Teu";
            this.colTal_Teu.Name = "colTal_Teu";
            this.colTal_Teu.Visible = true;
            this.colTal_Teu.VisibleIndex = 6;
            this.colTal_Teu.Width = 102;
            // 
            // colRuHuaLv
            // 
            this.colRuHuaLv.Caption = "蠕化率";
            this.colRuHuaLv.FieldName = "RuHuaLv";
            this.colRuHuaLv.Name = "colRuHuaLv";
            this.colRuHuaLv.Visible = true;
            this.colRuHuaLv.VisibleIndex = 7;
            this.colRuHuaLv.Width = 74;
            // 
            // colIsHuiZhuTie
            // 
            this.colIsHuiZhuTie.Caption = "是否灰铸铁";
            this.colIsHuiZhuTie.FieldName = "IsHuiZhuTie";
            this.colIsHuiZhuTie.Name = "colIsHuiZhuTie";
            this.colIsHuiZhuTie.OptionsColumn.FixedWidth = true;
            this.colIsHuiZhuTie.Visible = true;
            this.colIsHuiZhuTie.VisibleIndex = 8;
            this.colIsHuiZhuTie.Width = 70;
            // 
            // colId
            // 
            this.colId.FieldName = "Id";
            this.colId.Name = "colId";
            // 
            // colAction
            // 
            this.colAction.Caption = "查看";
            this.colAction.ColumnEdit = this.repositoryItemButtonEdit1;
            this.colAction.Name = "colAction";
            this.colAction.OptionsColumn.FixedWidth = true;
            this.colAction.Visible = true;
            this.colAction.VisibleIndex = 9;
            this.colAction.Width = 50;
            // 
            // repositoryItemButtonEdit1
            // 
            this.repositoryItemButtonEdit1.AutoHeight = false;
            this.repositoryItemButtonEdit1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.repositoryItemButtonEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("repositoryItemButtonEdit1.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, "", null, null, true)});
            this.repositoryItemButtonEdit1.Name = "repositoryItemButtonEdit1";
            this.repositoryItemButtonEdit1.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repositoryItemButtonEdit1.Click += new System.EventHandler(this.repositoryItemButtonEdit1_Click);
            // 
            // HistoryHandAnalysis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1053, 536);
            this.Controls.Add(this.gridControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HistoryHandAnalysis";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "历史分析记录";
            ((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.analysisModelBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEdit1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridControl1;
        private System.Windows.Forms.BindingSource analysisModelBindingSource;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn colFileName;
        private DevExpress.XtraGrid.Columns.GridColumn colTime;
        private DevExpress.XtraGrid.Columns.GridColumn colTAL;
        private DevExpress.XtraGrid.Columns.GridColumn colTEU;
        private DevExpress.XtraGrid.Columns.GridColumn colTER;
        private DevExpress.XtraGrid.Columns.GridColumn colTer_Teu;
        private DevExpress.XtraGrid.Columns.GridColumn colTal_Teu;
        private DevExpress.XtraGrid.Columns.GridColumn colRuHuaLv;
        private DevExpress.XtraGrid.Columns.GridColumn colIsHuiZhuTie;
        private DevExpress.XtraGrid.Columns.GridColumn colId;
        private DevExpress.XtraGrid.Columns.GridColumn colAction;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButtonEdit1;
    }
}