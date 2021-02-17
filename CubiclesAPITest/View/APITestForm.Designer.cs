namespace CubiclesAPITest.View
{
    partial class APITestForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(APITestForm));
            this.textBoxRequest = new System.Windows.Forms.TextBox();
            this.labelRawRequest = new System.Windows.Forms.Label();
            this.panelAPI = new System.Windows.Forms.Panel();
            this.panelDynamic = new System.Windows.Forms.Panel();
            this.textBoxResults = new System.Windows.Forms.TextBox();
            this.flowLayoutPanelParameters = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelMethod = new System.Windows.Forms.Label();
            this.labelResult = new System.Windows.Forms.Label();
            this.labelInput = new System.Windows.Forms.Label();
            this.comboBoxMethods = new System.Windows.Forms.ComboBox();
            this.bExecute = new System.Windows.Forms.Button();
            this.panelRaw = new System.Windows.Forms.Panel();
            this.bExecuteRaw = new System.Windows.Forms.Button();
            this.panelAPI.SuspendLayout();
            this.panelDynamic.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelRaw.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxRequest
            // 
            this.textBoxRequest.Location = new System.Drawing.Point(14, 25);
            this.textBoxRequest.Name = "textBoxRequest";
            this.textBoxRequest.Size = new System.Drawing.Size(509, 20);
            this.textBoxRequest.TabIndex = 1;
            // 
            // labelRawRequest
            // 
            this.labelRawRequest.AutoSize = true;
            this.labelRawRequest.BackColor = System.Drawing.Color.Transparent;
            this.labelRawRequest.Location = new System.Drawing.Point(11, 9);
            this.labelRawRequest.Name = "labelRawRequest";
            this.labelRawRequest.Size = new System.Drawing.Size(72, 13);
            this.labelRawRequest.TabIndex = 2;
            this.labelRawRequest.Text = "Raw Request";
            // 
            // panelAPI
            // 
            this.panelAPI.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panelAPI.Controls.Add(this.panelDynamic);
            this.panelAPI.Controls.Add(this.panel1);
            this.panelAPI.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelAPI.Location = new System.Drawing.Point(0, 55);
            this.panelAPI.Name = "panelAPI";
            this.panelAPI.Size = new System.Drawing.Size(634, 397);
            this.panelAPI.TabIndex = 3;
            // 
            // panelDynamic
            // 
            this.panelDynamic.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.panelDynamic.Controls.Add(this.textBoxResults);
            this.panelDynamic.Controls.Add(this.flowLayoutPanelParameters);
            this.panelDynamic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDynamic.Location = new System.Drawing.Point(0, 89);
            this.panelDynamic.Name = "panelDynamic";
            this.panelDynamic.Size = new System.Drawing.Size(634, 308);
            this.panelDynamic.TabIndex = 4;
            // 
            // textBoxResults
            // 
            this.textBoxResults.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxResults.Location = new System.Drawing.Point(253, 0);
            this.textBoxResults.Multiline = true;
            this.textBoxResults.Name = "textBoxResults";
            this.textBoxResults.ReadOnly = true;
            this.textBoxResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxResults.Size = new System.Drawing.Size(381, 308);
            this.textBoxResults.TabIndex = 1;
            // 
            // flowLayoutPanelParameters
            // 
            this.flowLayoutPanelParameters.Dock = System.Windows.Forms.DockStyle.Left;
            this.flowLayoutPanelParameters.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelParameters.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanelParameters.Name = "flowLayoutPanelParameters";
            this.flowLayoutPanelParameters.Padding = new System.Windows.Forms.Padding(15, 10, 15, 10);
            this.flowLayoutPanelParameters.Size = new System.Drawing.Size(253, 308);
            this.flowLayoutPanelParameters.TabIndex = 0;
            this.flowLayoutPanelParameters.WrapContents = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelMethod);
            this.panel1.Controls.Add(this.labelResult);
            this.panel1.Controls.Add(this.labelInput);
            this.panel1.Controls.Add(this.comboBoxMethods);
            this.panel1.Controls.Add(this.bExecute);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(634, 89);
            this.panel1.TabIndex = 5;
            // 
            // labelMethod
            // 
            this.labelMethod.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.labelMethod.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelMethod.Location = new System.Drawing.Point(14, 31);
            this.labelMethod.Name = "labelMethod";
            this.labelMethod.Size = new System.Drawing.Size(608, 32);
            this.labelMethod.TabIndex = 7;
            this.labelMethod.Text = "Method";
            // 
            // labelResult
            // 
            this.labelResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelResult.AutoSize = true;
            this.labelResult.BackColor = System.Drawing.Color.Transparent;
            this.labelResult.Location = new System.Drawing.Point(256, 68);
            this.labelResult.Name = "labelResult";
            this.labelResult.Size = new System.Drawing.Size(37, 13);
            this.labelResult.TabIndex = 6;
            this.labelResult.Text = "Result";
            // 
            // labelInput
            // 
            this.labelInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelInput.AutoSize = true;
            this.labelInput.BackColor = System.Drawing.Color.Transparent;
            this.labelInput.Location = new System.Drawing.Point(12, 68);
            this.labelInput.Name = "labelInput";
            this.labelInput.Size = new System.Drawing.Size(31, 13);
            this.labelInput.TabIndex = 5;
            this.labelInput.Text = "Input";
            // 
            // comboBoxMethods
            // 
            this.comboBoxMethods.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMethods.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBoxMethods.FormattingEnabled = true;
            this.comboBoxMethods.Location = new System.Drawing.Point(14, 7);
            this.comboBoxMethods.Name = "comboBoxMethods";
            this.comboBoxMethods.Size = new System.Drawing.Size(509, 21);
            this.comboBoxMethods.TabIndex = 0;
            this.comboBoxMethods.SelectedIndexChanged += new System.EventHandler(this.comboBoxMethods_SelectedIndexChanged);
            // 
            // bExecute
            // 
            this.bExecute.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
            this.bExecute.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.bExecute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bExecute.Location = new System.Drawing.Point(526, 6);
            this.bExecute.Name = "bExecute";
            this.bExecute.Size = new System.Drawing.Size(96, 23);
            this.bExecute.TabIndex = 1;
            this.bExecute.Text = "Execute";
            this.bExecute.UseVisualStyleBackColor = true;
            this.bExecute.Click += new System.EventHandler(this.bSend_Click);
            // 
            // panelRaw
            // 
            this.panelRaw.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.panelRaw.Controls.Add(this.bExecuteRaw);
            this.panelRaw.Controls.Add(this.textBoxRequest);
            this.panelRaw.Controls.Add(this.labelRawRequest);
            this.panelRaw.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelRaw.Location = new System.Drawing.Point(0, 0);
            this.panelRaw.Name = "panelRaw";
            this.panelRaw.Size = new System.Drawing.Size(634, 55);
            this.panelRaw.TabIndex = 4;
            // 
            // bExecuteRaw
            // 
            this.bExecuteRaw.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DeepSkyBlue;
            this.bExecuteRaw.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.bExecuteRaw.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bExecuteRaw.Location = new System.Drawing.Point(526, 24);
            this.bExecuteRaw.Name = "bExecuteRaw";
            this.bExecuteRaw.Size = new System.Drawing.Size(96, 23);
            this.bExecuteRaw.TabIndex = 3;
            this.bExecuteRaw.Text = "Execute";
            this.bExecuteRaw.UseVisualStyleBackColor = true;
            this.bExecuteRaw.Click += new System.EventHandler(this.bExecuteRaw_Click);
            // 
            // APITestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 452);
            this.Controls.Add(this.panelAPI);
            this.Controls.Add(this.panelRaw);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "APITestForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cubicles API Test Form";
            this.panelAPI.ResumeLayout(false);
            this.panelDynamic.ResumeLayout(false);
            this.panelDynamic.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panelRaw.ResumeLayout(false);
            this.panelRaw.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxRequest;
        private System.Windows.Forms.Label labelRawRequest;
        private System.Windows.Forms.Panel panelAPI;
        private System.Windows.Forms.Panel panelRaw;
        private System.Windows.Forms.Button bExecute;
        private System.Windows.Forms.ComboBox comboBoxMethods;
        private System.Windows.Forms.Panel panelDynamic;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelParameters;
        private System.Windows.Forms.TextBox textBoxResults;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelResult;
        private System.Windows.Forms.Label labelInput;
        private System.Windows.Forms.Label labelMethod;
        private System.Windows.Forms.Button bExecuteRaw;
    }
}