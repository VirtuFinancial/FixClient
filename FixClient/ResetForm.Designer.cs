/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: ResetForm.Designer.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿namespace FixClient
{
    partial class ResetForm
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
            this.iconPictureBox = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.noButton = new System.Windows.Forms.Button();
            this.yesButton = new System.Windows.Forms.Button();
            this.retainActiveGtdOrdersCheckBox = new System.Windows.Forms.CheckBox();
            this.retainActiveGtcOrdersCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.resetGeneratedIdsCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // iconPictureBox
            // 
            this.iconPictureBox.Location = new System.Drawing.Point(28, 23);
            this.iconPictureBox.Name = "iconPictureBox";
            this.iconPictureBox.Size = new System.Drawing.Size(48, 48);
            this.iconPictureBox.TabIndex = 1;
            this.iconPictureBox.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.iconPictureBox);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(454, 159);
            this.panel1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(83, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(358, 48);
            this.label1.TabIndex = 2;
            this.label1.Text = "This will erase the session message history and reset the sequence numbers, conti" +
    "nue?";
            // 
            // noButton
            // 
            this.noButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.noButton.DialogResult = System.Windows.Forms.DialogResult.No;
            this.noButton.Location = new System.Drawing.Point(366, 172);
            this.noButton.Name = "noButton";
            this.noButton.Size = new System.Drawing.Size(75, 23);
            this.noButton.TabIndex = 3;
            this.noButton.Text = "No";
            this.noButton.UseVisualStyleBackColor = true;
            // 
            // yesButton
            // 
            this.yesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.yesButton.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.yesButton.Location = new System.Drawing.Point(285, 172);
            this.yesButton.Name = "yesButton";
            this.yesButton.Size = new System.Drawing.Size(75, 23);
            this.yesButton.TabIndex = 4;
            this.yesButton.Text = "Yes";
            this.yesButton.UseVisualStyleBackColor = true;
            // 
            // retainActiveGtdOrdersCheckBox
            // 
            this.retainActiveGtdOrdersCheckBox.AutoSize = true;
            this.retainActiveGtdOrdersCheckBox.Checked = true;
            this.retainActiveGtdOrdersCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.retainActiveGtdOrdersCheckBox.Location = new System.Drawing.Point(10, 39);
            this.retainActiveGtdOrdersCheckBox.Name = "retainActiveGtdOrdersCheckBox";
            this.retainActiveGtdOrdersCheckBox.Size = new System.Drawing.Size(147, 17);
            this.retainActiveGtdOrdersCheckBox.TabIndex = 2;
            this.retainActiveGtdOrdersCheckBox.Text = "Retain active GTD orders";
            this.retainActiveGtdOrdersCheckBox.UseVisualStyleBackColor = true;
            // 
            // retainActiveGtcOrdersCheckBox
            // 
            this.retainActiveGtcOrdersCheckBox.AutoSize = true;
            this.retainActiveGtcOrdersCheckBox.Checked = true;
            this.retainActiveGtcOrdersCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.retainActiveGtcOrdersCheckBox.Location = new System.Drawing.Point(10, 16);
            this.retainActiveGtcOrdersCheckBox.Name = "retainActiveGtcOrdersCheckBox";
            this.retainActiveGtcOrdersCheckBox.Size = new System.Drawing.Size(146, 17);
            this.retainActiveGtcOrdersCheckBox.TabIndex = 1;
            this.retainActiveGtcOrdersCheckBox.Text = "Retain active GTC orders";
            this.retainActiveGtcOrdersCheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.resetGeneratedIdsCheckBox);
            this.groupBox1.Controls.Add(this.retainActiveGtcOrdersCheckBox);
            this.groupBox1.Controls.Add(this.retainActiveGtdOrdersCheckBox);
            this.groupBox1.Location = new System.Drawing.Point(86, 54);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(355, 89);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // resetGeneratedIdsCheckBox
            // 
            this.resetGeneratedIdsCheckBox.AutoSize = true;
            this.resetGeneratedIdsCheckBox.Checked = true;
            this.resetGeneratedIdsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.resetGeneratedIdsCheckBox.Location = new System.Drawing.Point(10, 62);
            this.resetGeneratedIdsCheckBox.Name = "resetGeneratedIdsCheckBox";
            this.resetGeneratedIdsCheckBox.Size = new System.Drawing.Size(263, 17);
            this.resetGeneratedIdsCheckBox.TabIndex = 4;
            this.resetGeneratedIdsCheckBox.Text = "Reset generated IDs (ClOrdID, ListID, AllocIC etc) ";
            this.resetGeneratedIdsCheckBox.UseVisualStyleBackColor = true;
            // 
            // ResetForm
            // 
            this.AcceptButton = this.yesButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 207);
            this.Controls.Add(this.yesButton);
            this.Controls.Add(this.noButton);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ResetForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "FIX Client";
            ((System.ComponentModel.ISupportInitialize)(this.iconPictureBox)).EndInit();
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox iconPictureBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button noButton;
        private System.Windows.Forms.Button yesButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox retainActiveGtdOrdersCheckBox;
        private System.Windows.Forms.CheckBox retainActiveGtcOrdersCheckBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox resetGeneratedIdsCheckBox;
    }
}