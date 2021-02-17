namespace CubiclesPrinter.View
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.trayMenu = new System.Windows.Forms.NotifyIcon(this.components);
            this.bShowPrintControl = new System.Windows.Forms.Button();
            this.bCheckPrinterStatus = new System.Windows.Forms.Button();
            this.buttonJobStatus = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // trayMenu
            // 
            this.trayMenu.Icon = ((System.Drawing.Icon)(resources.GetObject("trayMenu.Icon")));
            this.trayMenu.Text = "Cubicles Printer";
            // 
            // bShowPrintControl
            // 
            this.bShowPrintControl.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.bShowPrintControl.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
            this.bShowPrintControl.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.bShowPrintControl.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bShowPrintControl.Location = new System.Drawing.Point(72, 12);
            this.bShowPrintControl.Name = "bShowPrintControl";
            this.bShowPrintControl.Size = new System.Drawing.Size(165, 41);
            this.bShowPrintControl.TabIndex = 0;
            this.bShowPrintControl.Text = "Show Printing Control";
            this.bShowPrintControl.UseVisualStyleBackColor = false;
            this.bShowPrintControl.Click += new System.EventHandler(this.bShowPrintControl_Click);
            // 
            // bCheckPrinterStatus
            // 
            this.bCheckPrinterStatus.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.bCheckPrinterStatus.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
            this.bCheckPrinterStatus.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.bCheckPrinterStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bCheckPrinterStatus.Location = new System.Drawing.Point(72, 59);
            this.bCheckPrinterStatus.Name = "bCheckPrinterStatus";
            this.bCheckPrinterStatus.Size = new System.Drawing.Size(165, 41);
            this.bCheckPrinterStatus.TabIndex = 1;
            this.bCheckPrinterStatus.Text = "Printer Status";
            this.bCheckPrinterStatus.UseVisualStyleBackColor = false;
            this.bCheckPrinterStatus.Click += new System.EventHandler(this.bCheckPrinterStatus_Click);
            // 
            // buttonJobStatus
            // 
            this.buttonJobStatus.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.buttonJobStatus.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
            this.buttonJobStatus.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.buttonJobStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonJobStatus.Location = new System.Drawing.Point(72, 106);
            this.buttonJobStatus.Name = "buttonJobStatus";
            this.buttonJobStatus.Size = new System.Drawing.Size(165, 41);
            this.buttonJobStatus.TabIndex = 2;
            this.buttonJobStatus.Text = "Job Status";
            this.buttonJobStatus.UseVisualStyleBackColor = false;
            this.buttonJobStatus.Click += new System.EventHandler(this.buttonJobStatus_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(314, 166);
            this.Controls.Add(this.buttonJobStatus);
            this.Controls.Add(this.bCheckPrinterStatus);
            this.Controls.Add(this.bShowPrintControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cubicles Printing Control Main";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon trayMenu;
        private System.Windows.Forms.Button bShowPrintControl;
        private System.Windows.Forms.Button bCheckPrinterStatus;
        private System.Windows.Forms.Button buttonJobStatus;
    }
}