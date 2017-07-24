namespace HockeyStats
{
    partial class FilterModal
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
            this.filterMenuStrip = new System.Windows.Forms.MenuStrip();
            this.filterLeaguesDropDown = new System.Windows.Forms.ToolStripMenuItem();
            this.filterTeamsDropDown = new System.Windows.Forms.ToolStripMenuItem();
            this.filterDraftTeamsDropDown = new System.Windows.Forms.ToolStripMenuItem();
            this.applyFiltersButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.clearFiltersButton = new System.Windows.Forms.Button();
            this.autoFilterOutLeaguesCheckBox = new System.Windows.Forms.CheckBox();
            this.autoFilterOutTeamsCheckBox = new System.Windows.Forms.CheckBox();
            this.autoFilterOutDraftTeamsCheckBox = new System.Windows.Forms.CheckBox();
            this.filterMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // filterMenuStrip
            // 
            this.filterMenuStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.filterMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filterLeaguesDropDown,
            this.filterTeamsDropDown,
            this.filterDraftTeamsDropDown});
            this.filterMenuStrip.Location = new System.Drawing.Point(22, 9);
            this.filterMenuStrip.Name = "filterMenuStrip";
            this.filterMenuStrip.Size = new System.Drawing.Size(205, 24);
            this.filterMenuStrip.TabIndex = 0;
            this.filterMenuStrip.Text = "Leagues";
            // 
            // filterLeaguesDropDown
            // 
            this.filterLeaguesDropDown.Name = "filterLeaguesDropDown";
            this.filterLeaguesDropDown.Size = new System.Drawing.Size(62, 20);
            this.filterLeaguesDropDown.Text = "Leagues";
            // 
            // filterTeamsDropDown
            // 
            this.filterTeamsDropDown.Name = "filterTeamsDropDown";
            this.filterTeamsDropDown.Size = new System.Drawing.Size(53, 20);
            this.filterTeamsDropDown.Text = "Teams";
            // 
            // filterDraftTeamsDropDown
            // 
            this.filterDraftTeamsDropDown.Name = "filterDraftTeamsDropDown";
            this.filterDraftTeamsDropDown.Size = new System.Drawing.Size(82, 20);
            this.filterDraftTeamsDropDown.Text = "Draft Teams";
            // 
            // applyFiltersButton
            // 
            this.applyFiltersButton.Location = new System.Drawing.Point(10, 125);
            this.applyFiltersButton.Name = "applyFiltersButton";
            this.applyFiltersButton.Size = new System.Drawing.Size(75, 23);
            this.applyFiltersButton.TabIndex = 1;
            this.applyFiltersButton.Text = "Apply Filters";
            this.applyFiltersButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(172, 126);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // clearFiltersButton
            // 
            this.clearFiltersButton.Location = new System.Drawing.Point(91, 125);
            this.clearFiltersButton.Name = "clearFiltersButton";
            this.clearFiltersButton.Size = new System.Drawing.Size(75, 23);
            this.clearFiltersButton.TabIndex = 3;
            this.clearFiltersButton.Text = "Clear Filters";
            this.clearFiltersButton.UseVisualStyleBackColor = true;
            // 
            // autoFilterOutLeaguesCheckBox
            // 
            this.autoFilterOutLeaguesCheckBox.AutoSize = true;
            this.autoFilterOutLeaguesCheckBox.Location = new System.Drawing.Point(22, 45);
            this.autoFilterOutLeaguesCheckBox.Name = "autoFilterOutLeaguesCheckBox";
            this.autoFilterOutLeaguesCheckBox.Size = new System.Drawing.Size(195, 17);
            this.autoFilterOutLeaguesCheckBox.TabIndex = 4;
            this.autoFilterOutLeaguesCheckBox.Text = "Automatically filter out new Leagues";
            this.autoFilterOutLeaguesCheckBox.UseVisualStyleBackColor = true;
            // 
            // autoFilterOutTeamsCheckBox
            // 
            this.autoFilterOutTeamsCheckBox.AutoSize = true;
            this.autoFilterOutTeamsCheckBox.Location = new System.Drawing.Point(22, 68);
            this.autoFilterOutTeamsCheckBox.Name = "autoFilterOutTeamsCheckBox";
            this.autoFilterOutTeamsCheckBox.Size = new System.Drawing.Size(186, 17);
            this.autoFilterOutTeamsCheckBox.TabIndex = 5;
            this.autoFilterOutTeamsCheckBox.Text = "Automatically filter out new Teams";
            this.autoFilterOutTeamsCheckBox.UseVisualStyleBackColor = true;
            // 
            // autoFilterOutDraftTeamsCheckBox
            // 
            this.autoFilterOutDraftTeamsCheckBox.AutoSize = true;
            this.autoFilterOutDraftTeamsCheckBox.Location = new System.Drawing.Point(22, 91);
            this.autoFilterOutDraftTeamsCheckBox.Name = "autoFilterOutDraftTeamsCheckBox";
            this.autoFilterOutDraftTeamsCheckBox.Size = new System.Drawing.Size(212, 17);
            this.autoFilterOutDraftTeamsCheckBox.TabIndex = 6;
            this.autoFilterOutDraftTeamsCheckBox.Text = "Automatically filter out new Draft Teams";
            this.autoFilterOutDraftTeamsCheckBox.UseVisualStyleBackColor = true;
            // 
            // FilterModal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.ClientSize = new System.Drawing.Size(259, 160);
            this.Controls.Add(this.autoFilterOutDraftTeamsCheckBox);
            this.Controls.Add(this.autoFilterOutTeamsCheckBox);
            this.Controls.Add(this.autoFilterOutLeaguesCheckBox);
            this.Controls.Add(this.clearFiltersButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.applyFiltersButton);
            this.Controls.Add(this.filterMenuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainMenuStrip = this.filterMenuStrip;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FilterModal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Filter";
            this.filterMenuStrip.ResumeLayout(false);
            this.filterMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip filterMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem filterLeaguesDropDown;
        private System.Windows.Forms.ToolStripMenuItem filterTeamsDropDown;
        private System.Windows.Forms.Button applyFiltersButton;
        private System.Windows.Forms.ToolStripMenuItem filterDraftTeamsDropDown;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button clearFiltersButton;
        private System.Windows.Forms.CheckBox autoFilterOutLeaguesCheckBox;
        private System.Windows.Forms.CheckBox autoFilterOutTeamsCheckBox;
        private System.Windows.Forms.CheckBox autoFilterOutDraftTeamsCheckBox;
    }
}