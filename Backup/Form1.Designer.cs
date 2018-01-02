namespace JpegRotater
{
	partial class JpegRotator
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
			this._chooseFolderButton = new System.Windows.Forms.Button();
			this._folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this._flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
			this._statusStrip = new System.Windows.Forms.StatusStrip();
			this._statusStripLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this._saveButton = new System.Windows.Forms.Button();
			this._instructionsLabel = new System.Windows.Forms.Label();
			this._blueBackgroundLabel = new System.Windows.Forms.Label();
			this._labelStep1 = new System.Windows.Forms.Label();
			this._labelStep2 = new System.Windows.Forms.Label();
			this._labelStep3 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this._instructionsLabel2 = new System.Windows.Forms.Label();
			this._statusStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// _chooseFolderButton
			// 
			this._chooseFolderButton.Location = new System.Drawing.Point(245, 3);
			this._chooseFolderButton.Name = "_chooseFolderButton";
			this._chooseFolderButton.Size = new System.Drawing.Size(100, 23);
			this._chooseFolderButton.TabIndex = 0;
			this._chooseFolderButton.Text = "Select Folder...";
			this._chooseFolderButton.UseVisualStyleBackColor = true;
			this._chooseFolderButton.Click += new System.EventHandler(this._chooseFolderButton_Click);
			// 
			// _folderBrowserDialog
			// 
			this._folderBrowserDialog.ShowNewFolderButton = false;
			// 
			// _flowLayoutPanel
			// 
			this._flowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._flowLayoutPanel.AutoScroll = true;
			this._flowLayoutPanel.AutoScrollMargin = new System.Drawing.Size(5, 5);
			this._flowLayoutPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this._flowLayoutPanel.Location = new System.Drawing.Point(12, 122);
			this._flowLayoutPanel.Name = "_flowLayoutPanel";
			this._flowLayoutPanel.Size = new System.Drawing.Size(703, 358);
			this._flowLayoutPanel.TabIndex = 3;
			// 
			// _statusStrip
			// 
			this._statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._statusStripLabel});
			this._statusStrip.Location = new System.Drawing.Point(0, 514);
			this._statusStrip.Name = "_statusStrip";
			this._statusStrip.Size = new System.Drawing.Size(727, 22);
			this._statusStrip.TabIndex = 4;
			// 
			// _statusStripLabel
			// 
			this._statusStripLabel.Name = "_statusStripLabel";
			this._statusStripLabel.Size = new System.Drawing.Size(0, 17);
			// 
			// _saveButton
			// 
			this._saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._saveButton.Enabled = false;
			this._saveButton.Location = new System.Drawing.Point(247, 485);
			this._saveButton.Name = "_saveButton";
			this._saveButton.Size = new System.Drawing.Size(100, 25);
			this._saveButton.TabIndex = 1;
			this._saveButton.Text = "Finish";
			this._saveButton.UseVisualStyleBackColor = true;
			this._saveButton.Click += new System.EventHandler(this._saveButton_Click);
			// 
			// _instructionsLabel
			// 
			this._instructionsLabel.AutoSize = true;
			this._instructionsLabel.Location = new System.Drawing.Point(48, 93);
			this._instructionsLabel.Name = "_instructionsLabel";
			this._instructionsLabel.Size = new System.Drawing.Size(315, 13);
			this._instructionsLabel.TabIndex = 6;
			this._instructionsLabel.Text = "Verify the new rotation for each image (shown on the right) below.";
			// 
			// _blueBackgroundLabel
			// 
			this._blueBackgroundLabel.AutoSize = true;
			this._blueBackgroundLabel.Location = new System.Drawing.Point(48, 69);
			this._blueBackgroundLabel.Name = "_blueBackgroundLabel";
			this._blueBackgroundLabel.Size = new System.Drawing.Size(318, 13);
			this._blueBackgroundLabel.TabIndex = 8;
			this._blueBackgroundLabel.Text = "camera, if available.  (Blue background indicates a rotated image.)";
			// 
			// _labelStep1
			// 
			this._labelStep1.AutoSize = true;
			this._labelStep1.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._labelStep1.Location = new System.Drawing.Point(22, 3);
			this._labelStep1.Name = "_labelStep1";
			this._labelStep1.Size = new System.Drawing.Size(216, 24);
			this._labelStep1.TabIndex = 9;
			this._labelStep1.Text = "1. Load Your Photos";
			// 
			// _labelStep2
			// 
			this._labelStep2.AutoSize = true;
			this._labelStep2.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._labelStep2.Location = new System.Drawing.Point(22, 32);
			this._labelStep2.Name = "_labelStep2";
			this._labelStep2.Size = new System.Drawing.Size(370, 24);
			this._labelStep2.TabIndex = 10;
			this._labelStep2.Text = "2. Check and Adjust Photo Rotation";
			// 
			// _labelStep3
			// 
			this._labelStep3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._labelStep3.AutoSize = true;
			this._labelStep3.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._labelStep3.Location = new System.Drawing.Point(22, 486);
			this._labelStep3.Name = "_labelStep3";
			this._labelStep3.Size = new System.Drawing.Size(219, 24);
			this._labelStep3.TabIndex = 11;
			this._labelStep3.Text = "3. Save the Changes";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(48, 56);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(331, 13);
			this.label1.TabIndex = 12;
			this.label1.Text = "Your photos are automatically rotated based on saved data from your";
			// 
			// _instructionsLabel2
			// 
			this._instructionsLabel2.AutoSize = true;
			this._instructionsLabel2.Location = new System.Drawing.Point(48, 106);
			this._instructionsLabel2.Name = "_instructionsLabel2";
			this._instructionsLabel2.Size = new System.Drawing.Size(297, 13);
			this._instructionsLabel2.TabIndex = 13;
			this._instructionsLabel2.Text = "You can left-click or right-click an image to adjust the rotation.";
			// 
			// JpegRotator
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(727, 536);
			this.Controls.Add(this._instructionsLabel2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._labelStep3);
			this.Controls.Add(this._labelStep2);
			this.Controls.Add(this._labelStep1);
			this.Controls.Add(this._blueBackgroundLabel);
			this.Controls.Add(this._instructionsLabel);
			this.Controls.Add(this._saveButton);
			this.Controls.Add(this._statusStrip);
			this.Controls.Add(this._flowLayoutPanel);
			this.Controls.Add(this._chooseFolderButton);
			this.MinimumSize = new System.Drawing.Size(398, 393);
			this.Name = "JpegRotator";
			this.Text = "Digital Photo Auto-Rotator";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.JpegRotator_FormClosed);
			this._statusStrip.ResumeLayout(false);
			this._statusStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _chooseFolderButton;
		private System.Windows.Forms.FolderBrowserDialog _folderBrowserDialog;
		private System.Windows.Forms.FlowLayoutPanel _flowLayoutPanel;
		private System.Windows.Forms.StatusStrip _statusStrip;
		private System.Windows.Forms.ToolStripStatusLabel _statusStripLabel;
		private System.Windows.Forms.Button _saveButton;
		private System.Windows.Forms.Label _instructionsLabel;
		private System.Windows.Forms.Label _blueBackgroundLabel;
		private System.Windows.Forms.Label _labelStep1;
		private System.Windows.Forms.Label _labelStep2;
		private System.Windows.Forms.Label _labelStep3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label _instructionsLabel2;
	}
}

