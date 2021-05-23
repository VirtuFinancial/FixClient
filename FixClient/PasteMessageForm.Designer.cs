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
            this.resetMessageCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // filterEmptyFieldsCheckBox
            // 
            this.filterEmptyFieldsCheckBox.AutoSize = true;
            this.filterEmptyFieldsCheckBox.Checked = true;
            this.filterEmptyFieldsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.filterEmptyFieldsCheckBox.Location = new System.Drawing.Point(13, 46);
            this.filterEmptyFieldsCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.filterEmptyFieldsCheckBox.Name = "filterEmptyFieldsCheckBox";
            this.filterEmptyFieldsCheckBox.Size = new System.Drawing.Size(120, 19);
            this.filterEmptyFieldsCheckBox.TabIndex = 2;
            this.filterEmptyFieldsCheckBox.Text = "Filter empty fields";
            this.filterEmptyFieldsCheckBox.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(485, 143);
            this.okButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(88, 27);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(390, 143);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(88, 27);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.defineUnknownAsCustomCheckBox);
            this.groupBox1.Controls.Add(this.resetMessageCheckBox);
            this.groupBox1.Controls.Add(this.filterEmptyFieldsCheckBox);
            this.groupBox1.Location = new System.Drawing.Point(14, 29);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.groupBox1.Size = new System.Drawing.Size(555, 108);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            // 
            // defineUnknownAsCustomCheckBox
            // 
            this.defineUnknownAsCustomCheckBox.AutoSize = true;
            this.defineUnknownAsCustomCheckBox.Checked = true;
            this.defineUnknownAsCustomCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.defineUnknownAsCustomCheckBox.Location = new System.Drawing.Point(13, 20);
            this.defineUnknownAsCustomCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.defineUnknownAsCustomCheckBox.Name = "defineUnknownAsCustomCheckBox";
            this.defineUnknownAsCustomCheckBox.Size = new System.Drawing.Size(230, 19);
            this.defineUnknownAsCustomCheckBox.TabIndex = 3;
            this.defineUnknownAsCustomCheckBox.Text = "Define custom fields for unknown tags";
            this.defineUnknownAsCustomCheckBox.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(504, 15);
            this.label1.TabIndex = 6;
            this.label1.Text = "A message has been successfully parsed from the clipboard.  When pasting, would y" +
    "ou like to?";
            // 
            // resetMessageCheckBox
            // 
            this.resetMessageCheckBox.AutoSize = true;
            this.resetMessageCheckBox.Checked = true;
            this.resetMessageCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.resetMessageCheckBox.Location = new System.Drawing.Point(13, 71);
            this.resetMessageCheckBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.resetMessageCheckBox.Name = "resetMessageCheckBox";
            this.resetMessageCheckBox.Size = new System.Drawing.Size(190, 19);
            this.resetMessageCheckBox.TabIndex = 0;
            this.resetMessageCheckBox.Text = "Reset the existing message first";
            this.resetMessageCheckBox.UseVisualStyleBackColor = true;
            // 
            // PasteMessageForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(586, 181);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PasteMessageForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FIX Client";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox resetMessageCheckBox;
        private System.Windows.Forms.CheckBox filterEmptyFieldsCheckBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox defineUnknownAsCustomCheckBox;
    }
}