/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: PasteMessageForm.Designer.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿namespace FixClient
{
    partial class PasteMessageForm
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
            this.filterEmptyFieldsCheckBox = new System.Windows.Forms.CheckBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.defineUnknownAsCustomCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.smartPasteCheckGroupBox = new FixClient.CheckGroupBox();
            this.resetMessageCheckBox = new System.Windows.Forms.CheckBox();
            this.processGroupsCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.smartPasteCheckGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // filterEmptyFieldsCheckBox
            // 
            this.filterEmptyFieldsCheckBox.AutoSize = true;
            this.filterEmptyFieldsCheckBox.Checked = true;
            this.filterEmptyFieldsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.filterEmptyFieldsCheckBox.Location = new System.Drawing.Point(11, 40);
            this.filterEmptyFieldsCheckBox.Name = "filterEmptyFieldsCheckBox";
            this.filterEmptyFieldsCheckBox.Size = new System.Drawing.Size(106, 17);
            this.filterEmptyFieldsCheckBox.TabIndex = 2;
            this.filterEmptyFieldsCheckBox.Text = "Filter empty fields";
            this.filterEmptyFieldsCheckBox.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(413, 185);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(332, 185);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.defineUnknownAsCustomCheckBox);
            this.groupBox1.Controls.Add(this.filterEmptyFieldsCheckBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 25);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(476, 67);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            // 
            // defineUnknownAsCustomCheckBox
            // 
            this.defineUnknownAsCustomCheckBox.AutoSize = true;
            this.defineUnknownAsCustomCheckBox.Checked = true;
            this.defineUnknownAsCustomCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.defineUnknownAsCustomCheckBox.Location = new System.Drawing.Point(11, 17);
            this.defineUnknownAsCustomCheckBox.Name = "defineUnknownAsCustomCheckBox";
            this.defineUnknownAsCustomCheckBox.Size = new System.Drawing.Size(206, 17);
            this.defineUnknownAsCustomCheckBox.TabIndex = 3;
            this.defineUnknownAsCustomCheckBox.Text = "Define custom fields for unknown tags";
            this.defineUnknownAsCustomCheckBox.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(454, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "A message has been successfully parsed from the clipboard.  When pasting, would y" +
    "ou like to?";
            // 
            // smartPasteCheckGroupBox
            // 
            this.smartPasteCheckGroupBox.Controls.Add(this.resetMessageCheckBox);
            this.smartPasteCheckGroupBox.Controls.Add(this.processGroupsCheckBox);
            this.smartPasteCheckGroupBox.Location = new System.Drawing.Point(13, 100);
            this.smartPasteCheckGroupBox.Name = "smartPasteCheckGroupBox";
            this.smartPasteCheckGroupBox.Size = new System.Drawing.Size(475, 77);
            this.smartPasteCheckGroupBox.TabIndex = 9;
            this.smartPasteCheckGroupBox.TabStop = false;
            this.smartPasteCheckGroupBox.Text = "Smart Paste";
            this.smartPasteCheckGroupBox.CheckedChanged += new System.EventHandler(this.SmartPasteCheckGroupBoxCheckedChanged);
            // 
            // resetMessageCheckBox
            // 
            this.resetMessageCheckBox.AutoSize = true;
            this.resetMessageCheckBox.Checked = true;
            this.resetMessageCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.resetMessageCheckBox.Location = new System.Drawing.Point(19, 24);
            this.resetMessageCheckBox.Name = "resetMessageCheckBox";
            this.resetMessageCheckBox.Size = new System.Drawing.Size(174, 17);
            this.resetMessageCheckBox.TabIndex = 0;
            this.resetMessageCheckBox.Text = "Reset the existing message first";
            this.resetMessageCheckBox.UseVisualStyleBackColor = true;
            // 
            // processGroupsCheckBox
            // 
            this.processGroupsCheckBox.AutoSize = true;
            this.processGroupsCheckBox.Checked = true;
            this.processGroupsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.processGroupsCheckBox.Location = new System.Drawing.Point(19, 47);
            this.processGroupsCheckBox.Name = "processGroupsCheckBox";
            this.processGroupsCheckBox.Size = new System.Drawing.Size(196, 17);
            this.processGroupsCheckBox.TabIndex = 1;
            this.processGroupsCheckBox.Text = "Attempt to process repeating groups";
            this.processGroupsCheckBox.UseVisualStyleBackColor = true;
            // 
            // PasteMessageForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(502, 218);
            this.Controls.Add(this.smartPasteCheckGroupBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PasteMessageForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FIX Client";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.smartPasteCheckGroupBox.ResumeLayout(false);
            this.smartPasteCheckGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox resetMessageCheckBox;
        private System.Windows.Forms.CheckBox processGroupsCheckBox;
        private System.Windows.Forms.CheckBox filterEmptyFieldsCheckBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox defineUnknownAsCustomCheckBox;
        private CheckGroupBox smartPasteCheckGroupBox;
    }
}