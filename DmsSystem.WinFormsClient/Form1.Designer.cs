namespace DmsSystem.WinFormsClient
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            mainMenuStrip = new MenuStrip();
            menuItem_Shareholder = new ToolStripMenuItem();
            menuItem_Shareholder_SubItem1 = new ToolStripMenuItem();
            menuItem_Shareholder_SubItem2 = new ToolStripMenuItem();
            menuItem_Dividend = new ToolStripMenuItem();
            menuItem_Mandate = new ToolStripMenuItem();
            panelMainContent = new Panel();
            panel_A_Report = new Panel();
            chkShmtSource4 = new CheckBox();
            txtShmtSource4Path = new TextBox();
            chkShmtSource1 = new CheckBox();
            txtShmtSource1Path = new TextBox();
            chkHoldingDetails = new CheckBox();
            txtHoldingDetailsPath = new TextBox();
            btnConfirm = new Button();
            btnClose = new Button();
            panel_SubItem2 = new Panel();
            label_SubItem2 = new Label();
            mainMenuStrip.SuspendLayout();
            panelMainContent.SuspendLayout();
            panel_A_Report.SuspendLayout();
            panel_SubItem2.SuspendLayout();
            SuspendLayout();
            // 
            // mainMenuStrip
            // 
            mainMenuStrip.Items.AddRange(new ToolStripItem[] { menuItem_Shareholder, menuItem_Dividend, menuItem_Mandate });
            mainMenuStrip.Location = new Point(0, 0);
            mainMenuStrip.Name = "mainMenuStrip";
            mainMenuStrip.Size = new Size(800, 24);
            mainMenuStrip.TabIndex = 0;
            mainMenuStrip.Text = "menuStrip1";
            // 
            // menuItem_Shareholder
            // 
            menuItem_Shareholder.DropDownItems.AddRange(new ToolStripItem[] { menuItem_Shareholder_SubItem1, menuItem_Shareholder_SubItem2 });
            menuItem_Shareholder.Name = "menuItem_Shareholder";
            menuItem_Shareholder.Size = new Size(68, 20);
            menuItem_Shareholder.Text = "1. 股東會";
            // 
            // menuItem_Shareholder_SubItem1
            // 
            menuItem_Shareholder_SubItem1.Name = "menuItem_Shareholder_SubItem1";
            menuItem_Shareholder_SubItem1.Size = new Size(193, 22);
            menuItem_Shareholder_SubItem1.Text = "股東會 - 檔案上傳作業";
            // 
            // menuItem_Shareholder_SubItem2
            // 
            menuItem_Shareholder_SubItem2.Name = "menuItem_Shareholder_SubItem2";
            menuItem_Shareholder_SubItem2.Size = new Size(193, 22);
            menuItem_Shareholder_SubItem2.Text = "- 項下2";
            // 
            // menuItem_Dividend
            // 
            menuItem_Dividend.Name = "menuItem_Dividend";
            menuItem_Dividend.Size = new Size(56, 20);
            menuItem_Dividend.Text = "2. 配息";
            // 
            // menuItem_Mandate
            // 
            menuItem_Mandate.Name = "menuItem_Mandate";
            menuItem_Mandate.Size = new Size(56, 20);
            menuItem_Mandate.Text = "3. 全委";
            // 
            // panelMainContent
            // 
            panelMainContent.Controls.Add(panel_A_Report);
            panelMainContent.Controls.Add(panel_SubItem2);
            panelMainContent.Dock = DockStyle.Fill;
            panelMainContent.Location = new Point(0, 24);
            panelMainContent.Name = "panelMainContent";
            panelMainContent.Padding = new Padding(10);
            panelMainContent.Size = new Size(800, 426);
            panelMainContent.TabIndex = 1;
            // 
            // panel_A_Report
            // 
            panel_A_Report.Controls.Add(chkShmtSource4);
            panel_A_Report.Controls.Add(txtShmtSource4Path);
            panel_A_Report.Controls.Add(chkShmtSource1);
            panel_A_Report.Controls.Add(txtShmtSource1Path);
            panel_A_Report.Controls.Add(chkHoldingDetails);
            panel_A_Report.Controls.Add(txtHoldingDetailsPath);
            panel_A_Report.Controls.Add(btnConfirm);
            panel_A_Report.Controls.Add(btnClose);
            panel_A_Report.Dock = DockStyle.Fill;
            panel_A_Report.Font = new Font("微軟正黑體", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 136);
            panel_A_Report.Location = new Point(10, 10);
            panel_A_Report.Name = "panel_A_Report";
            panel_A_Report.Size = new Size(780, 406);
            panel_A_Report.TabIndex = 0;
            panel_A_Report.Visible = false;
            // 
            // chkShmtSource4
            // 
            chkShmtSource4.AutoSize = true;
            chkShmtSource4.Location = new Point(20, 20);
            chkShmtSource4.Name = "chkShmtSource4";
            chkShmtSource4.Size = new Size(195, 21);
            chkShmtSource4.TabIndex = 0;
            chkShmtSource4.Text = "公司基本資料 (shmtsource4)";
            chkShmtSource4.UseVisualStyleBackColor = true;
            // 
            // txtShmtSource4Path
            // 
            txtShmtSource4Path.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtShmtSource4Path.Location = new Point(20, 50);
            txtShmtSource4Path.Name = "txtShmtSource4Path";
            txtShmtSource4Path.ReadOnly = true;
            txtShmtSource4Path.Size = new Size(740, 25);
            txtShmtSource4Path.TabIndex = 1;
            txtShmtSource4Path.Text = "C:\\Temp\\公司基本資料.XLS";
            // 
            // chkShmtSource1
            // 
            chkShmtSource1.AutoSize = true;
            chkShmtSource1.Location = new Point(20, 90);
            chkShmtSource1.Name = "chkShmtSource1";
            chkShmtSource1.Size = new Size(182, 21);
            chkShmtSource1.TabIndex = 2;
            chkShmtSource1.Text = "股東會明細 (shmtsource1)";
            chkShmtSource1.UseVisualStyleBackColor = true;
            // 
            // txtShmtSource1Path
            // 
            txtShmtSource1Path.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtShmtSource1Path.Location = new Point(20, 120);
            txtShmtSource1Path.Name = "txtShmtSource1Path";
            txtShmtSource1Path.ReadOnly = true;
            txtShmtSource1Path.Size = new Size(740, 25);
            txtShmtSource1Path.TabIndex = 3;
            txtShmtSource1Path.Text = "-C:\\Temp\\股東會明細yyyymmdd.CSV";
            // 
            // chkHoldingDetails
            // 
            chkHoldingDetails.AutoSize = true;
            chkHoldingDetails.Location = new Point(20, 160);
            chkHoldingDetails.Name = "chkHoldingDetails";
            chkHoldingDetails.Size = new Size(155, 21);
            chkHoldingDetails.TabIndex = 4;
            chkHoldingDetails.Text = "持股明細資料 (阿拉丁)";
            chkHoldingDetails.UseVisualStyleBackColor = true;
            // 
            // txtHoldingDetailsPath
            // 
            txtHoldingDetailsPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtHoldingDetailsPath.Location = new Point(20, 190);
            txtHoldingDetailsPath.Name = "txtHoldingDetailsPath";
            txtHoldingDetailsPath.ReadOnly = true;
            txtHoldingDetailsPath.Size = new Size(740, 25);
            txtHoldingDetailsPath.TabIndex = 5;
            txtHoldingDetailsPath.Text = "C:\\Temp\\阿拉丁持股明細資料.CSV";
            // 
            // btnConfirm
            // 
            btnConfirm.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnConfirm.Location = new Point(554, 240);
            btnConfirm.Name = "btnConfirm";
            btnConfirm.Size = new Size(100, 35);
            btnConfirm.TabIndex = 6;
            btnConfirm.Text = "確定";
            btnConfirm.UseVisualStyleBackColor = true;
            btnConfirm.Click += btnConfirm_Click;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.Location = new Point(660, 240);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(100, 35);
            btnClose.TabIndex = 7;
            btnClose.Text = "關閉";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click_1;
            // 
            // panel_SubItem2
            // 
            panel_SubItem2.Controls.Add(label_SubItem2);
            panel_SubItem2.Dock = DockStyle.Fill;
            panel_SubItem2.Location = new Point(10, 10);
            panel_SubItem2.Name = "panel_SubItem2";
            panel_SubItem2.Size = new Size(780, 406);
            panel_SubItem2.TabIndex = 1;
            panel_SubItem2.Visible = false;
            // 
            // label_SubItem2
            // 
            label_SubItem2.AutoSize = true;
            label_SubItem2.Font = new Font("微軟正黑體", 12F, FontStyle.Regular, GraphicsUnit.Point, 136);
            label_SubItem2.Location = new Point(20, 20);
            label_SubItem2.Name = "label_SubItem2";
            label_SubItem2.Size = new Size(178, 20);
            label_SubItem2.TabIndex = 0;
            label_SubItem2.Text = "這裡是「項下2」的畫面";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(panelMainContent);
            Controls.Add(mainMenuStrip);
            MainMenuStrip = mainMenuStrip;
            Name = "Form1";
            Text = "DMS 系統";
            mainMenuStrip.ResumeLayout(false);
            mainMenuStrip.PerformLayout();
            panelMainContent.ResumeLayout(false);
            panel_A_Report.ResumeLayout(false);
            panel_A_Report.PerformLayout();
            panel_SubItem2.ResumeLayout(false);
            panel_SubItem2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        // --- UI 控制項的宣告 ---
        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuItem_Shareholder;
        private System.Windows.Forms.ToolStripMenuItem menuItem_Shareholder_SubItem1;
        private System.Windows.Forms.ToolStripMenuItem menuItem_Shareholder_SubItem2;
        private System.Windows.Forms.ToolStripMenuItem menuItem_Dividend;
        private System.Windows.Forms.ToolStripMenuItem menuItem_Mandate;
        private System.Windows.Forms.Panel panelMainContent;
        // Panel for A報告
        private System.Windows.Forms.Panel panel_A_Report;
        private System.Windows.Forms.CheckBox chkShmtSource4;
        private System.Windows.Forms.TextBox txtShmtSource4Path;
        private System.Windows.Forms.CheckBox chkShmtSource1;
        private System.Windows.Forms.TextBox txtShmtSource1Path;
        private System.Windows.Forms.CheckBox chkHoldingDetails;
        private System.Windows.Forms.TextBox txtHoldingDetailsPath;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnConfirm;
        // Panel for 項下2
        private System.Windows.Forms.Panel panel_SubItem2;
        private System.Windows.Forms.Label label_SubItem2;

    }
}
