namespace CubiclesPrinter.View
{
    partial class PrintingControlForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrintingControlForm));
            this.comboBoxPrinters = new System.Windows.Forms.ComboBox();
            this.labelChoosePrinter = new System.Windows.Forms.Label();
            this.radioButtonColored = new System.Windows.Forms.RadioButton();
            this.radioButtonGrayScale = new System.Windows.Forms.RadioButton();
            this.panelTop = new System.Windows.Forms.Panel();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.labelTitle = new System.Windows.Forms.Label();
            this.panelControl = new System.Windows.Forms.Panel();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.bPrint = new System.Windows.Forms.Button();
            this.panelStatus = new System.Windows.Forms.Panel();
            this.labelStatus = new System.Windows.Forms.Label();
            this.labelStatusTitle = new System.Windows.Forms.Label();
            this.panelPrint = new System.Windows.Forms.Panel();
            this.bAdvancedSettings = new System.Windows.Forms.Button();
            this.comboBoxDuplex = new System.Windows.Forms.ComboBox();
            this.labelColorOption = new System.Windows.Forms.Label();
            this.labelDuplexOption = new System.Windows.Forms.Label();
            this.panelColor = new System.Windows.Forms.Panel();
            this.radioButtonUnknownColorInit = new System.Windows.Forms.RadioButton();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.panelControl.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.panelStatus.SuspendLayout();
            this.panelPrint.SuspendLayout();
            this.panelColor.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxPrinters
            // 
            this.comboBoxPrinters.DropDownHeight = 120;
            this.comboBoxPrinters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPrinters.DropDownWidth = 200;
            this.comboBoxPrinters.Font = new System.Drawing.Font("Cambria", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.comboBoxPrinters.FormattingEnabled = true;
            this.comboBoxPrinters.IntegralHeight = false;
            this.comboBoxPrinters.Location = new System.Drawing.Point(237, 11);
            this.comboBoxPrinters.Margin = new System.Windows.Forms.Padding(5);
            this.comboBoxPrinters.Name = "comboBoxPrinters";
            this.comboBoxPrinters.Size = new System.Drawing.Size(367, 30);
            this.comboBoxPrinters.TabIndex = 0;
            this.toolTip.SetToolTip(this.comboBoxPrinters, "The list of available printers.");
            this.comboBoxPrinters.SelectedIndexChanged += new System.EventHandler(this.comboBoxPrinters_SelectedIndexChanged);
            // 
            // labelChoosePrinter
            // 
            this.labelChoosePrinter.Location = new System.Drawing.Point(3, 13);
            this.labelChoosePrinter.Name = "labelChoosePrinter";
            this.labelChoosePrinter.Size = new System.Drawing.Size(212, 26);
            this.labelChoosePrinter.TabIndex = 1;
            this.labelChoosePrinter.Text = "Choose Printer:";
            this.labelChoosePrinter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // radioButtonColored
            // 
            this.radioButtonColored.AutoSize = true;
            this.radioButtonColored.Location = new System.Drawing.Point(16, 4);
            this.radioButtonColored.Name = "radioButtonColored";
            this.radioButtonColored.Size = new System.Drawing.Size(92, 26);
            this.radioButtonColored.TabIndex = 3;
            this.radioButtonColored.Text = "Colored";
            this.toolTip.SetToolTip(this.radioButtonColored, "Print colored.");
            this.radioButtonColored.UseVisualStyleBackColor = true;
            this.radioButtonColored.CheckedChanged += new System.EventHandler(this.radioButtonColored_CheckedChanged);
            // 
            // radioButtonGrayScale
            // 
            this.radioButtonGrayScale.AutoSize = true;
            this.radioButtonGrayScale.Location = new System.Drawing.Point(208, 4);
            this.radioButtonGrayScale.Name = "radioButtonGrayScale";
            this.radioButtonGrayScale.Size = new System.Drawing.Size(107, 26);
            this.radioButtonGrayScale.TabIndex = 4;
            this.radioButtonGrayScale.Text = "GrayScale";
            this.toolTip.SetToolTip(this.radioButtonGrayScale, "Print grayscaled.");
            this.radioButtonGrayScale.UseVisualStyleBackColor = true;
            this.radioButtonGrayScale.CheckedChanged += new System.EventHandler(this.radioButtonGrayScale_CheckedChanged);
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.panelTop.Controls.Add(this.pictureBoxLogo);
            this.panelTop.Controls.Add(this.labelTitle);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(794, 70);
            this.panelTop.TabIndex = 5;
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.Location = new System.Drawing.Point(3, 3);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(64, 64);
            this.pictureBoxLogo.TabIndex = 1;
            this.pictureBoxLogo.TabStop = false;
            // 
            // labelTitle
            // 
            this.labelTitle.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelTitle.Location = new System.Drawing.Point(178, 6);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(452, 58);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "Cubicles Printing Control Options ";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelControl
            // 
            this.panelControl.Controls.Add(this.panelBottom);
            this.panelControl.Controls.Add(this.panelStatus);
            this.panelControl.Controls.Add(this.panelPrint);
            this.panelControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl.Location = new System.Drawing.Point(0, 70);
            this.panelControl.Name = "panelControl";
            this.panelControl.Size = new System.Drawing.Size(794, 289);
            this.panelControl.TabIndex = 6;
            // 
            // panelBottom
            // 
            this.panelBottom.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panelBottom.Controls.Add(this.bPrint);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 171);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(794, 68);
            this.panelBottom.TabIndex = 10;
            // 
            // bPrint
            // 
            this.bPrint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bPrint.BackColor = System.Drawing.SystemColors.Control;
            this.bPrint.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.bPrint.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bPrint.Location = new System.Drawing.Point(298, 7);
            this.bPrint.Name = "bPrint";
            this.bPrint.Size = new System.Drawing.Size(208, 54);
            this.bPrint.TabIndex = 9;
            this.bPrint.Text = "Print";
            this.toolTip.SetToolTip(this.bPrint, "Print the chosen data.");
            this.bPrint.UseVisualStyleBackColor = false;
            this.bPrint.Click += new System.EventHandler(this.bPrint_Click);
            // 
            // panelStatus
            // 
            this.panelStatus.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.panelStatus.Controls.Add(this.labelStatus);
            this.panelStatus.Controls.Add(this.labelStatusTitle);
            this.panelStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelStatus.Location = new System.Drawing.Point(0, 239);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Size = new System.Drawing.Size(794, 50);
            this.panelStatus.TabIndex = 21;
            // 
            // labelStatus
            // 
            this.labelStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelStatus.Location = new System.Drawing.Point(0, 17);
            this.labelStatus.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(794, 33);
            this.labelStatus.TabIndex = 9;
            this.labelStatus.Text = "Status:";
            // 
            // labelStatusTitle
            // 
            this.labelStatusTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelStatusTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelStatusTitle.Location = new System.Drawing.Point(0, 0);
            this.labelStatusTitle.Margin = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.labelStatusTitle.Name = "labelStatusTitle";
            this.labelStatusTitle.Size = new System.Drawing.Size(794, 17);
            this.labelStatusTitle.TabIndex = 10;
            this.labelStatusTitle.Text = "Status:";
            this.labelStatusTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelPrint
            // 
            this.panelPrint.Controls.Add(this.comboBoxDuplex);
            this.panelPrint.Controls.Add(this.labelColorOption);
            this.panelPrint.Controls.Add(this.labelDuplexOption);
            this.panelPrint.Controls.Add(this.panelColor);
            this.panelPrint.Controls.Add(this.comboBoxPrinters);
            this.panelPrint.Controls.Add(this.labelChoosePrinter);
            this.panelPrint.Controls.Add(this.bAdvancedSettings);
            this.panelPrint.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelPrint.Location = new System.Drawing.Point(0, 0);
            this.panelPrint.Name = "panelPrint";
            this.panelPrint.Size = new System.Drawing.Size(794, 172);
            this.panelPrint.TabIndex = 19;
            // 
            // bAdvancedSettings
            // 
            this.bAdvancedSettings.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.bAdvancedSettings.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.ActiveCaption;
            this.bAdvancedSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bAdvancedSettings.Location = new System.Drawing.Point(298, 127);
            this.bAdvancedSettings.Name = "bAdvancedSettings";
            this.bAdvancedSettings.Size = new System.Drawing.Size(208, 36);
            this.bAdvancedSettings.TabIndex = 22;
            this.bAdvancedSettings.Text = "Advanced Settings";
            this.toolTip.SetToolTip(this.bAdvancedSettings, "Browse file path to new PDF file.");
            this.bAdvancedSettings.UseVisualStyleBackColor = true;
            this.bAdvancedSettings.Click += new System.EventHandler(this.bAdvancedSettings_Click);
            // 
            // comboBoxDuplex
            // 
            this.comboBoxDuplex.DropDownHeight = 120;
            this.comboBoxDuplex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDuplex.DropDownWidth = 200;
            this.comboBoxDuplex.Font = new System.Drawing.Font("Cambria", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.comboBoxDuplex.FormattingEnabled = true;
            this.comboBoxDuplex.IntegralHeight = false;
            this.comboBoxDuplex.Items.AddRange(new object[] {
            "Single",
            "Duplex Vertical",
            "Duplex Horizontal",
            "Default"});
            this.comboBoxDuplex.Location = new System.Drawing.Point(237, 49);
            this.comboBoxDuplex.Margin = new System.Windows.Forms.Padding(5);
            this.comboBoxDuplex.Name = "comboBoxDuplex";
            this.comboBoxDuplex.Size = new System.Drawing.Size(367, 30);
            this.comboBoxDuplex.TabIndex = 21;
            this.toolTip.SetToolTip(this.comboBoxDuplex, "The list of available duplex print options.");
            this.comboBoxDuplex.SelectedIndexChanged += new System.EventHandler(this.comboBoxDuplex_SelectedIndexChanged);
            // 
            // labelColorOption
            // 
            this.labelColorOption.Location = new System.Drawing.Point(3, 89);
            this.labelColorOption.Name = "labelColorOption";
            this.labelColorOption.Size = new System.Drawing.Size(211, 22);
            this.labelColorOption.TabIndex = 19;
            this.labelColorOption.Text = "Print Color Option:";
            this.labelColorOption.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelDuplexOption
            // 
            this.labelDuplexOption.Location = new System.Drawing.Point(3, 52);
            this.labelDuplexOption.Name = "labelDuplexOption";
            this.labelDuplexOption.Size = new System.Drawing.Size(211, 22);
            this.labelDuplexOption.TabIndex = 20;
            this.labelDuplexOption.Text = "Double-Sided Print:";
            this.labelDuplexOption.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panelColor
            // 
            this.panelColor.Controls.Add(this.radioButtonUnknownColorInit);
            this.panelColor.Controls.Add(this.radioButtonGrayScale);
            this.panelColor.Controls.Add(this.radioButtonColored);
            this.panelColor.Location = new System.Drawing.Point(237, 85);
            this.panelColor.Name = "panelColor";
            this.panelColor.Size = new System.Drawing.Size(480, 36);
            this.panelColor.TabIndex = 18;
            // 
            // radioButtonUnknownColorInit
            // 
            this.radioButtonUnknownColorInit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radioButtonUnknownColorInit.AutoSize = true;
            this.radioButtonUnknownColorInit.Checked = true;
            this.radioButtonUnknownColorInit.Location = new System.Drawing.Point(421, 4);
            this.radioButtonUnknownColorInit.Name = "radioButtonUnknownColorInit";
            this.radioButtonUnknownColorInit.Size = new System.Drawing.Size(56, 26);
            this.radioButtonUnknownColorInit.TabIndex = 5;
            this.radioButtonUnknownColorInit.TabStop = true;
            this.radioButtonUnknownColorInit.Text = "Init";
            this.toolTip.SetToolTip(this.radioButtonUnknownColorInit, "Print grayscaled.");
            this.radioButtonUnknownColorInit.UseVisualStyleBackColor = true;
            this.radioButtonUnknownColorInit.Visible = false;
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 5000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.ReshowDelay = 100;
            this.toolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip.ToolTipTitle = "Tips of Cubicles Printing Control";
            // 
            // PrintingControlForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(794, 359);
            this.Controls.Add(this.panelControl);
            this.Controls.Add(this.panelTop);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Cambria", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PrintingControlForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cubicles Printing Control";
            this.Load += new System.EventHandler(this.PrintingControlForm_Load);
            this.Shown += new System.EventHandler(this.PrintingControlForm_Shown);
            this.VisibleChanged += new System.EventHandler(this.PrintingControlForm_VisibleChanged);
            this.panelTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.panelControl.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            this.panelStatus.ResumeLayout(false);
            this.panelPrint.ResumeLayout(false);
            this.panelColor.ResumeLayout(false);
            this.panelColor.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxPrinters;
        private System.Windows.Forms.Label labelChoosePrinter;
        private System.Windows.Forms.RadioButton radioButtonColored;
        private System.Windows.Forms.RadioButton radioButtonGrayScale;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Panel panelControl;
        private System.Windows.Forms.Button bPrint;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.Panel panelColor;
        private System.Windows.Forms.Panel panelPrint;
        private System.Windows.Forms.Label labelColorOption;
        private System.Windows.Forms.Label labelDuplexOption;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Panel panelStatus;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Label labelStatusTitle;
        private System.Windows.Forms.ComboBox comboBoxDuplex;
        private System.Windows.Forms.Button bAdvancedSettings;
        private System.Windows.Forms.RadioButton radioButtonUnknownColorInit;
    }
}