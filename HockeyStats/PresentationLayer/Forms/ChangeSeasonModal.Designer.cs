﻿namespace HockeyStats
{
    partial class ChangeSeasonModal
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
            this.label1 = new System.Windows.Forms.Label();
            this.changeSeasonButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.changeSeasonDomainUpDown = new System.Windows.Forms.DomainUpDown();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Season";
            // 
            // changeSeasonButton
            // 
            this.changeSeasonButton.Location = new System.Drawing.Point(12, 67);
            this.changeSeasonButton.Name = "changeSeasonButton";
            this.changeSeasonButton.Size = new System.Drawing.Size(92, 23);
            this.changeSeasonButton.TabIndex = 2;
            this.changeSeasonButton.Text = "Change Season";
            this.changeSeasonButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(120, 67);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // changeSeasonDomainUpDown
            // 
            this.changeSeasonDomainUpDown.Location = new System.Drawing.Point(15, 25);
            this.changeSeasonDomainUpDown.Name = "changeSeasonDomainUpDown";
            this.changeSeasonDomainUpDown.ReadOnly = true;
            this.changeSeasonDomainUpDown.Size = new System.Drawing.Size(120, 20);
            this.changeSeasonDomainUpDown.TabIndex = 4;
            this.changeSeasonDomainUpDown.Text = "Season";
            // 
            // ChangeSeasonModal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.ClientSize = new System.Drawing.Size(214, 99);
            this.Controls.Add(this.changeSeasonDomainUpDown);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.changeSeasonButton);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChangeSeasonModal";
            this.Text = "Change Season";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button changeSeasonButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.DomainUpDown changeSeasonDomainUpDown;
    }
}