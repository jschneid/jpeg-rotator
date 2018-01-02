using System;
using System.Windows.Forms;
using System.Drawing;

namespace JpegRotater
{
	/// <summary>
	/// The available modes of the progress dialog.
	/// </summary>
	public enum ProgressDialogType
	{
		Loading,
		Saving
	}

	/// <summary>
	/// Implements a "progress meter" dialog.
	/// </summary>
	public partial class ProgressDialog : Form
	{
		public event EventHandler CancelButtonClick;

		private int _fileCount;
		private ProgressDialogType _type;

		/// <summary>
		/// Creates a new ProgressDialog instance.
		/// </summary>
		/// <param name="progressDialogType">The mode of the dialog, affecting the text displayed 
		/// on the dialog.</param>
		public ProgressDialog(ProgressDialogType progressDialogType)
		{
			InitializeComponent();
			
			//Save the specified progressDialogType for use elsewhere in this class.
			this._type = progressDialogType;

			//Set the dialog caption based on the dialog type.
			if (progressDialogType == ProgressDialogType.Loading)
			{
				this.Text = "Loading...";
			}
			else if (progressDialogType == ProgressDialogType.Saving)
			{
				this.Text = "Saving...";
			}
			else
			{
				throw new NotImplementedException("Unrecognized progressDialogType value: " + progressDialogType);
			}
		}

		/// <summary>
		/// Sets the count of items currently being handled by the process whose progress is
		/// being displayed by the dialog.  
		/// </summary>
		/// <param name="fileCount">The item count. </param>
		public void SetFileCount(int fileCount)
		{
			this._fileCount = fileCount;
		}

		/// <summary>
		/// Updates the dialog based on the specified index (the index of the item currently 
		/// being processed).
		/// </summary>
		/// <param name="currentFile">The 1-based index of the item being processed. </param>
		public void UpdateProgress(int currentFile)
		{
			//Update the text shown on the dialog.
			if (this._type == ProgressDialogType.Loading)
			{
				this._progressLabel.Text = "Opening file " + currentFile + " of " + this._fileCount + "...";
			}
			else
			{
				this._progressLabel.Text = "Saving file " + currentFile + " of " + this._fileCount + "...";
			}

			//Center the label horizontally on the dialog.
			this._progressLabel.Location = new Point((this.Width / 2) - (this._progressLabel.Width / 2), this._progressLabel.Location.Y);

			//Calculate the process percentage, and update the progress bar with the calculated value.
			int progressPercent = (int)((((float)(currentFile - 1)) / ((float)this._fileCount)) * 100);
			this._progressBar.Value = progressPercent;
		}

		/// <summary>
		/// Activated when the Cancel button is clicked.  Fires a CancelButtonClick event.
		/// </summary>
		/// <param name="sender">Unused. </param>
		/// <param name="e">Unused. </param>
		private void _cancelButton_Click(object sender, EventArgs e)
		{
			FireCancelEvent();

			//Disable the Cancel button and change the text to indicate to the user that their
			//click has been processed.
			this._cancelButton.Enabled = false;
			this._cancelButton.Text = "Cancelling...";
		}

		/// <summary>
		/// Activated when the form is closed.
		/// </summary>
		/// <param name="sender">Unused. </param>
		/// <param name="e">Unused. </param>
		private void ProgressDialog_FormClosed(object sender, FormClosedEventArgs e)
		{
			//Treat the user closing the form via Alt+F4 or whatever other means
			//as a click on the Cancel button.  
			FireCancelEvent();
		}

		/// <summary>
		/// Fires a CancelButtonClick event to notify the main form that the operation in progress
		/// should be cancelled (if the operation hasn't already finished).
		/// </summary>
		private void FireCancelEvent()
		{
			if (this.CancelButtonClick != null)
			{
				EventArgs eventArgs = new EventArgs();
				this.CancelButtonClick(this, eventArgs);
			}
		}
	}
}