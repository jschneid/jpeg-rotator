using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

//TODO: 
// - main UI -- add "arrow" between to images on each panel

// Nice-to-have:
// Highlight/warn on images whose dimensions don't line up with the EXIF data (e.g. "left" or "right" image with width > height)
// Test: Handle a file with .jpg extension but not a real .jpg (e.g. .txt renamed to .jpg)
// ".jpeg" extension
// Handle case where file is read-only on disk or due to ACL
// Refresh button to refresh contents of current folder (efficiently if possible, faster then just reload it)

// Document: Why is the file size shrinking?  Are we losing quality?
// - Note possible loss of quality in the docs



namespace JpegRotator
{
	/// <summary>
	/// This is the main form of the JpegRotator application.
	/// </summary>
	public partial class JpegRotator : Form
	{
		#region Private Delegates and Member Variables

		private delegate void ControlParamDelegate(Control control);
		private delegate void StringParamDelegate(string text);
		private delegate void IntParamDelegate(int value);
		private delegate void BoolParamDelegate(bool value);
		private delegate void VoidParamDelegate();
		private delegate void RotatablePanelDelegate(DualImagePanel panel);

		private ControlParamDelegate _addItemToFlowLayoutPanel;
		private VoidParamDelegate _updateStatusText;
		private IntParamDelegate _updateProgressDialog;
		private IntParamDelegate _setProgressDialogFileCount;
		private VoidParamDelegate _operationFinished;
		private RotatablePanelDelegate _setRotatedImageAsOriginalImage;
		
		private Thread _workerThread;

		private int _imageCount = 0;
		private int _rotatedImageCount = 0;

		private ProgressDialog _progressDialog;
		private bool _cancelOperation;

		#endregion

		#region Construction

		/// <summary>
		/// Creates a new JpegRotator instance.
		/// </summary>
		public JpegRotator()
		{
			InitializeComponent();

			//Set up delegates for methods that can be called by the background thread.
			this._addItemToFlowLayoutPanel = new ControlParamDelegate(this.AddItemToFlowLayoutPanel);
			this._updateStatusText = new VoidParamDelegate(this.UpdateStatusText);
			this._setProgressDialogFileCount = new IntParamDelegate(this.SetProgressDialogFileCount);
			this._updateProgressDialog = new IntParamDelegate(this.UpdateProgressDialog);
			this._operationFinished = new VoidParamDelegate(this.OperationFinished);
			this._setRotatedImageAsOriginalImage = new RotatablePanelDelegate(this.SetRotatedImageAsOriginalImage);

			//Set the initial path for the "Select Folder" dialog to "My Pictures".
			this._folderBrowserDialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Activated when the "Choose Folder..." button is clicked.  Brings up a dialog to allow
		/// the user to choose a folder, and kicks off the image load after a folder is selected.
		/// </summary>
		/// <param name="sender">Unused. </param>
		/// <param name="e">Unused. </param>
		private void _chooseFolderButton_Click(object sender, EventArgs e)
		{
			DialogResult result = this._folderBrowserDialog.ShowDialog();

			//todo temp: for now, hard-code path 
			//this._folderBrowserDialog.SelectedPath = @"C:\Documents and Settings\Jon\My Documents\My Pictures\_test";
			//DialogResult result = DialogResult.OK;

			//If the dialog was closed with the Cancel button, bail out at this point.
			if (result != DialogResult.OK)
			{
				return;
			}

			//Disable the buttons on the UI while we're busy loading files.
			DisableUIElements();

			//Clear existing contents of the flow layout panel, if any.
			this._flowLayoutPanel.Controls.Clear();

			//Reset the flag that indicates whether the Cancel button on the Progress dialog has been clicked.
			this._cancelOperation = false;

			//Start loading the images from disk on a background thread.  (Use a background thread so the UI
			//doesn't hang until the load is finished.)
			this._workerThread = new Thread(new ThreadStart(this.PopulateImages));
			this._workerThread.Start();

			//Show a "Loading..." dialog.  (We'll auto-close the dialog when the load is finished.)
			this._progressDialog = new ProgressDialog(ProgressDialogType.Loading);
			this._progressDialog.CancelButtonClick += new EventHandler(_progressDialog_CancelButtonClick);
			this._progressDialog.ShowDialog();
		}

		/// <summary>
		/// Activated when the Cancel button is clicked on the Progress dialog during image load.
		/// </summary>
		/// <param name="sender">The Progress dialog. </param>
		/// <param name="e">Unused. </param>
		void _progressDialog_CancelButtonClick(object sender, EventArgs e)
		{
			this._cancelOperation = true;
			UpdateUIElements();
		}

		/// <summary>
		/// Activated when the Save button on the form has been clicked.  Saves the rotations made
		/// on the form to disk, overwriting the original files.
		/// </summary>
		/// <param name="sender">Unused. </param>
		/// <param name="e">Unused. </param>
		private void _saveButton_Click(object sender, EventArgs e)
		{
			//Show a confirmation dialog before we overwrite files.
			DialogResult result = MessageBox.Show("Proceed to overwrite your original image files with the rotated versions?", Utility.APP_NAME, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

			if (result != DialogResult.OK)
			{
				return;
			}

			//Reset the flag that indicates whether the Cancel button on the Progress dialog has been clicked.
			this._cancelOperation = false;

			//Disable the buttons on the UI while we're busy saving files.
			DisableUIElements();

			//Launch the background thread to save the files.
			this._workerThread = new Thread(new ThreadStart(this.SaveRotations));
			this._workerThread.Start();

			//Show a "Saving..." dialog.  (We'll auto-close the dialog when the save is finished.)
			this._progressDialog = new ProgressDialog(ProgressDialogType.Saving);
			this._progressDialog.CancelButtonClick += new EventHandler(_progressDialog_CancelButtonClick);
			this._progressDialog.ShowDialog();
		}

		/// <summary>
		/// Activated when the main form is closed.  Kills the background thread if it is currently 
		/// running.
		/// </summary>
		/// <param name="sender">Unused. </param>
		/// <param name="e">Unused. </param>
		private void JpegRotator_FormClosed(object sender, FormClosedEventArgs e)
		{
			//Before we exit, kill the worker thread if it exists and is currently busy.
			if (this._workerThread != null)
			{
				this._workerThread.Abort();
			}
		}

		/// <summary>
		/// Activated when an image is rotated on one of the DualImagePanel instances on the form.
		/// Updates our count of rotated images, updates the status bar text, and updates the state
		/// of UI elements on the form.
		/// </summary>
		/// <param name="sender">Unused. </param>
		/// <param name="e">The details of the rotation that was performed. </param>
		private void innerPanel_RotationEvent(object sender, RotationEventArgs e)
		{
			if (e.NewOrientation == Orientation.Initial)
			{
				//We just rotated *from* upright orientation, so there's now one more rotated
				//image on the form.
				this._rotatedImageCount--;
				UpdateStatusText();
				UpdateUIElements();
			}
			else if (e.OriginalOrientation == Orientation.Initial)
			{
				//We just rotated *to* upright orientation, so there's no one less rotated
				//image on the form.
				this._rotatedImageCount++;
				UpdateStatusText();
				UpdateUIElements();
			}
		}

		#endregion

		#region UI Methods (invoked by the worker thread)

		/// <summary>
		/// Activated by the background thread to add a new item to the _flowLayoutPanel.
		/// </summary>
		/// <param name="control">The item to add to the _flowLayoutPanel. </param>
		private void AddItemToFlowLayoutPanel(Control control)
		{
			this._flowLayoutPanel.Controls.Add(control);
		}

		/// <summary>
		/// Activated by the background thread to inform the Progress dialog of the number of files
		/// being loaded during the current load (so that the dialog can establish the maximum value
		/// for the progress meter).
		/// </summary>
		/// <param name="fileCount">The number of files being loaded. </param>
		private void SetProgressDialogFileCount(int fileCount)
		{
			if (this._progressDialog != null)
			{
				this._progressDialog.SetFileCount(fileCount);
			}
		}

		/// <summary>
		/// Activated by the background thread to update the progress meter on the Progress dialog.
		/// (This should only be called after SetProgressDialogFileCount has been called for the 
		/// current load.)
		/// </summary>
		/// <param name="currentFile">The 1-based index of the file currently being loaded. </param>
		private void UpdateProgressDialog(int currentFile)
		{
			if (this._progressDialog != null)
			{
				this._progressDialog.UpdateProgress(currentFile);
			}
		}

		/// <summary>
		/// Activated when the background thread has finished loading or saving images.  Closes the Progress
		/// dialog and reactivates disabled UI elements.
		/// </summary>
		private void OperationFinished()
		{
			//Close the Progress dialog.
			this._progressDialog.Close();
			this._progressDialog = null;

			//Reenable the buttons on the UI.
			UpdateUIElements();
		}

		/// <summary>
		/// Activated by the background thread when the rotated image for the specified panel has 
		/// been saved to disk, replacing the original image on disk.  On the specified DualImagePanel, 
		/// sets the original image to be the rotated image, and sets the orientation to Initial.  
		/// </summary>
		private void SetRotatedImageAsOriginalImage(DualImagePanel panel)
		{
			panel.SetRotatedImageAsOriginalImage();
		}

		/// <summary>
		/// Updates the text on the form's status bar with the number of loaded images and the
		/// number of currently rotated images.
		/// </summary>
		private void UpdateStatusText()
		{
			string statusText = this._rotatedImageCount + " rotated image(s)  ";
			statusText += this._imageCount + " total image(s)";
			this._statusStripLabel.Text = statusText;
		}

		#endregion 

		#region Private Methods

		/// <summary>
		/// Updates the disabled/enabled state of UI elements.
		/// </summary>
		private void UpdateUIElements()
		{
			this._chooseFolderButton.Enabled = true;
			this._saveButton.Enabled = (this._rotatedImageCount > 0);
		}

		/// <summary>
		/// Disables the buttons on the UI while we're busy loading or saving files.
		/// </summary>
		private void DisableUIElements()
		{
			this._chooseFolderButton.Enabled = false;
			this._saveButton.Enabled = false;
		}

		#endregion

		#region Worker Thread Methods

		/// <summary>
		/// Populates the _flowLayoutPanel with images from the directory selected in the 
		/// _folderBrowserDialog.  This method is designed to be called on a background thread.
		/// </summary>
		private void PopulateImages()
		{
			//TODO: Support ".jpeg" also?  Can I do that with a comma or some other syntax e.g. *.jpg,*.jpeg ?
			string[] filenames = System.IO.Directory.GetFiles(this._folderBrowserDialog.SelectedPath, "*.jpg");

			//Set the current directory that we'll use to load and save files
			System.IO.Directory.SetCurrentDirectory(this._folderBrowserDialog.SelectedPath);

			//Inform the progress dialog of the count of files that we'll be loading.  (This is needed by
			//the progress dialog to calculate the percentage finished based on the number of the current
			//file being processed.)
			Invoke(this._setProgressDialogFileCount, new object[] { filenames.Length });

			//Build the list of images that we'll display in the dialog.
			List<string> errorFiles = new List<string>();
			int imageIndex = 1;
			foreach (string filename in filenames)
			{
				//Skip loading the remainder of the files if the Cancel button on the progress meter was clicked.
				if (this._cancelOperation == true)
				{
					break;
				}

				//Update the Loading dialog with the number of files loaded so far.
				Invoke(this._updateProgressDialog, new object[] { imageIndex });

				//The height and width of the maximum allowed size of thumbnail images.
				const int maxThumbnailHeightWidth = 140;
				const int borderWidth = 6;
				 
				DualImagePanel innerPanel = new DualImagePanel(maxThumbnailHeightWidth, maxThumbnailHeightWidth);
				innerPanel.Width = 335; 
				innerPanel.Height = 170; 
				innerPanel.Location = new Point(borderWidth, borderWidth);
				innerPanel.BackColor = this.BackColor;
				innerPanel.RotationEvent += new DualImagePanel.RotationDelegate(innerPanel_RotationEvent);

				//Get the image from the file, create a small (thumbnail) version of the image
				//that will fit within the containing panel, and put the thumbnail image into 
				//the panel.  Then release the original image.  (For lots of large images, it 
				//would take too much memory to hold all of the originals in memory.)
				try
				{
					//NOTE: Using Image.FromStream() instead of Image.FromFile() to load images, it is *much* faster!
					//Thanks to http://www.shahine.com/omar/CommentView,guid,673c131f-26db-4f44-9908-c2667da832ad.aspx
					using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
					{
						using (Image image = Image.FromStream(fileStream, true, false))
						{
							Image thumbnailImage = Utility.ResizeImageToFitRectangle(image, new Size(maxThumbnailHeightWidth, maxThumbnailHeightWidth));
							
							Orientation imageOrientation = Utility.GetImageOrientation(image);
							innerPanel.SetOriginalImage(thumbnailImage, imageOrientation, filename);
						}
					}
					this._imageCount++;
				}
				catch (Exception)
				{
					//Note: I've seen an OutOfMemory exception get thrown when trying to read a file with 
					//an extension of .jpg, but that isn't really an image file.  Regardless of what went 
					//wrong, just skip the file and go on to the next one.
					errorFiles.Add(filename);
					continue;
				}

				//For computers with low memory and large pictures: release memory now
				GC.Collect();

				//Add the innerPanel to the form.
				object[] controlArguments = new object[] { innerPanel };
				this.Invoke(this._addItemToFlowLayoutPanel, controlArguments);

				if (innerPanel.Orientation != Orientation.Initial)
				{
					this._rotatedImageCount++;
				}

				imageIndex++;
			}

			//If there were any images that we couldn't load successfully, display an error message
			//with the name(s) of the files that failed to load.
			if (errorFiles.Count > 0)
			{ 
				string errorMessage = "Error: " + errorFiles.Count + " file(s) did not load correctly and were skipped:" + Environment.NewLine + Environment.NewLine;
				errorMessage += errorFiles[0];
				for (int fileIndex = 1; fileIndex < errorFiles.Count; fileIndex++)
				{
					//Show a max of 50 failed filenames
					if (fileIndex > 50)
					{
						errorMessage += ", ...";
						break;
					}

					errorMessage += ", " + errorFiles[fileIndex];
				}
				MessageBox.Show(errorMessage, Utility.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}

			//Update the status bar text and close the progress dialog.
			this.Invoke(this._updateStatusText);
			this.Invoke(this._operationFinished);
		}

		/// <summary>
		/// For each image on the form that has been rotated, overwrites the original version 
		/// of that file on disk with the version rotated to match the orientation of the
		/// rotated thumbnail image in the image's panel.  This method is designed to be called
		/// on a background thread.
		/// </summary>
		private void SaveRotations()
		{
			//Note: The progress meter logic assumes that this._rotatedImageCount matches the number of
			//panels in _flowLayoutPanel that have been rotated to an orientation other than Orientation.Initial.
			Invoke(this._setProgressDialogFileCount, new object[] { this._rotatedImageCount });
			
			//The 1-based index of the image being saved, to use to update the progress dialog.
			int imageIndex = 1;

			//Loop through all of the photos on the form.  For each one that is rotated, generate the rotated
			//version of the large image file and save it to disk, replacing the original version.
			foreach (DualImagePanel imagePanel in this._flowLayoutPanel.Controls)
			{
				//Skip loading the remainder of the files if the Cancel button on the progress meter was clicked.
				if (this._cancelOperation == true)
				{
					break;
				}

				//If the image in this panel hasn't been rotated, then skip it.
				if (imagePanel.Orientation == Orientation.Initial)
				{
					continue;
				}

				Invoke(this._updateProgressDialog, new object[] { imageIndex });

				try
				{
					Utility.RotateAndSaveImageFile(imagePanel.Filename, imagePanel.Orientation);

					//Update the image panel to show the rotated image as the original image.
					Invoke(this._setRotatedImageAsOriginalImage, new object[] { imagePanel });
					this._rotatedImageCount--;
				}
				catch (Exception e)
				{
					MessageBox.Show("An error occurred while trying to save file: " + imagePanel.Filename + Environment.NewLine + Environment.NewLine + "Error: " + e.Message, Utility.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				imageIndex++;
			}

			//Update the status bar text and close the progress dialog.
			this.Invoke(this._updateStatusText);
			this.Invoke(this._operationFinished);
		}

		#endregion
	}
}