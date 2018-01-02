using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace JpegRotater
{
	public class DualImagePanel : Panel
	{
		private Orientation _orientation = Orientation.Initial;

		private PictureBox _originalImagePictureBox;
		private PictureBox _rotatedImagePictureBox;
		private Label _filenameLabel;
		private string _filename;

		//TODO: Could make this more efficient by just instantiating one copy of this bitmap
		//for the entire application, not one for every DualImagePanel.
		private Bitmap _arrowBitmap;

		#region Raised Events

		public delegate void RotationDelegate(object sender, RotationEventArgs e);
		public event RotationDelegate RotationEvent;

		#endregion

		#region Construction

		/// <summary>
		/// This class is a Panel that displays two images.  The one on the left is the "original"
		/// image, a thumbnail view of an image as it currently is on the disk.  On the right is the
		/// "rotated" image, an rotated version of the original image.
		/// </summary>
		/// <param name="thumbnailImageWidth">The maximum width of the thumbnail images. </param>
		/// <param name="thumbnailImageHeight">The maximum height of the thumbnail images. </param>
		public DualImagePanel(int thumbnailImageWidth, int thumbnailImageHeight)
		{
			this._originalImagePictureBox = new PictureBox();
			this._originalImagePictureBox.Location = new Point(10, 10);
			this._originalImagePictureBox.Size = new Size(thumbnailImageWidth, thumbnailImageHeight);
			this._originalImagePictureBox.MouseClick += new MouseEventHandler(PictureBox_MouseClick);
			this.Controls.Add(this._originalImagePictureBox);

			this._rotatedImagePictureBox = new PictureBox();
			this._rotatedImagePictureBox.Location = new Point(45 + thumbnailImageWidth, 10);
			this._rotatedImagePictureBox.Size = new Size(thumbnailImageWidth, thumbnailImageHeight);
			this._rotatedImagePictureBox.MouseClick += new MouseEventHandler(PictureBox_MouseClick);
			this.Controls.Add(this._rotatedImagePictureBox);

			this._filenameLabel = new Label();
			this._filenameLabel.Size = new Size(300, 50);
			this._filenameLabel.Location = new Point(0, 155);
			//this._filenameLabel.Font = new Font(this._filenameLabel.Font, FontStyle.Bold);
			this.Controls.Add(this._filenameLabel);

			this._arrowBitmap = new Bitmap(typeof(DualImagePanel), "Arrow3.bmp");
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Sets the "Original Image" for this DualImagePanel, which is the thumbnail version
		/// of the image as it is on the disk (i.e. with no rotation applied).
		/// </summary>
		/// <param name="originalImage">The non-rotated thumbnail image. </param>
		/// <param name="orientation">The initial orientation of the large version of the 
		/// specified image. </param>
		/// <param name="filename">The fully-specified (including path) filename of the large
		/// version of the image. </param>
		public void SetOriginalImage(Image originalImageThumbnail, Orientation orientation, string filename)
		{
			this._originalImagePictureBox.Image = originalImageThumbnail;
			
			//At this point, we want to calculate the "correction" for this image, to get the 
			//rotated image to (hopefully, if the EXIF data we got was accurate) appear upright.
			switch (orientation)
			{
				case Orientation.Initial:
					//No action necessary.
					this._orientation = Orientation.Initial;
					break;
				
				case Orientation.Left:
					//The original image is rotated counterclockwise, so we need to rotate 
					//clockwise to get it to appear upright.
					this._orientation = Orientation.Right;
					break;

				case Orientation.Right:
					//The original image is rotated clockwise, so we need to rotate 
					//counterclockwise to get it to appear upright.
					this._orientation = Orientation.Left;
					break;
				
				case Orientation.Inverted:
					//The original image is inverted (rotated 180 degrees), so we invert
					//it again to get it to appear upright.
					this._orientation = Orientation.Inverted;
					break;

				default:
					throw new ApplicationException("Unrecognized Orientation value: " + orientation);
			}

			this._filename = filename;

			//Get the name of the file with the filename only, not the fully specified path.
			string filenameWithNoPath = System.IO.Path.GetFileName(filename);
			this._filenameLabel.Text = filenameWithNoPath;

			UpdateRotatedImage();

			UpdateColor();
		}

		/// <summary>
		/// Sets the original image to be the rotated image, and sets the orientation to Initial.
		/// This should be called after the rotated image has been saved to disk, replacing the 
		/// original image.
		/// </summary>
		public void SetRotatedImageAsOriginalImage()
		{
			this._originalImagePictureBox.Image = this._rotatedImagePictureBox.Image;
			this._orientation = Orientation.Initial;
			UpdateColor();
		}

		/// <summary>
		/// Gets the current orientation of the image in this DualImagePanel.
		/// </summary>
		public Orientation Orientation
		{
			get
			{
				return this._orientation;
			}
		}

		/// <summary>
		/// Gets the fully-specified Filename of the image in this DualImagePanel.
		/// </summary>
		public string Filename
		{
			get
			{
				return this._filename;
			}
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Activated when one of the ImagePanels on this DualImagePanel is clicked.
		/// </summary>
		/// <param name="sender">The clicked ImagePanel. </param>
		/// <param name="e">Information about the click event. </param>
		private void PictureBox_MouseClick(object sender, MouseEventArgs e)
		{
			HandleMouseClick(e);
		}

		/// <summary>
		/// Activated when this DualImagePanel is clicked.
		/// </summary>
		/// <param name="e">Information about the click event. </param>
		protected override void OnMouseClick(MouseEventArgs e)
		{
			HandleMouseClick(e);
		}

		/// <summary>
		/// Activated when this DualImagePanel paints.  Does the default base class
		/// paint, then draws an arrow between the original image and the rotated image
		/// if the current orientation is not the initial orientation.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			if (this._orientation != Orientation.Initial)
			{
				e.Graphics.DrawImage(_arrowBitmap, 160, 65);
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Sets the "Rotated Image" for this DualImagePanel, which is the thumbnail version
		/// of the rotated version of the image.
		/// </summary>
		private void UpdateRotatedImage()
		{
			//Create a rotated version of the image.
			Image originalImage = this._originalImagePictureBox.Image;
			Image rotatedImage = Utility.RotateImage(originalImage, this._orientation);
			this._rotatedImagePictureBox.Image = rotatedImage;
		}

		/// <summary>
		/// Rotates the image in this DualImagePanel.  Direction of rotation is determined
		/// by which mouse button was clicked.
		/// </summary>
		/// <param name="e">The information about the click event. </param>
		private void HandleMouseClick(MouseEventArgs e)
		{
			//Rotate left (counterclockwise) on a left click or right (clockwise) on a 
			//right click.  Ignore other button clicks.
			if (e.Button == MouseButtons.Left)
			{
				this.RotateCounterclockwise();
			}
			else if (e.Button == MouseButtons.Right)
			{
				this.RotateClockwise();
			}
		}

		/// <summary>
		/// Rotates the image counterclockwise (left).
		/// </summary>
		private void RotateCounterclockwise()
		{
			//Save the initial (pre-rotation) orienatation to use when we fire an event below.
			Orientation originalOrientation = this._orientation;

			//Update the orientation flag for this DualImagePanel.
			this._orientation = (Orientation)((((int)this._orientation) + 1) % 4);

			//Update the rotated image thumbnail to match the new orientation.
			UpdateRotatedImage();

			//Update the color of the control.
			UpdateColor();

			//Fire an event to let the main form know that an image was rotated.
			FireRotationEvent(originalOrientation, this._orientation);
		}

		/// <summary>
		/// Rotates the image clockwise (right).
		/// </summary>
		private void RotateClockwise()
		{
			//Save the initial (pre-rotation) orienatation to use when we fire an event below.
			Orientation originalOrientation = this._orientation;

			//Update the orientation flag for this DualImagePanel.
			this._orientation = (Orientation)((((int)this._orientation) + 3) % 4);

			//Update the rotated image thumbnail to match the new orientation.
			UpdateRotatedImage();

			//Update the color of the control.
			UpdateColor();

			//Fire an event to let the main form know that an image was rotated.
			FireRotationEvent(originalOrientation, this._orientation);
		}

		/// <summary>
		/// Updates the background color of this DualImagePanel based on the current
		/// orientation of the rotated image.
		/// </summary>
		private void UpdateColor()
		{
			if (this._orientation == Orientation.Initial)
			{
				this.BackColor = Color.LightGray;
			}
			else
			{
				this.BackColor = Color.LightBlue;
			}
		}

		/// <summary>
		/// Fires a RotationEvent to notify the main form that the image has been rotated.
		/// </summary>
		/// <param name="originalOrientation">The original (pre-rotation) orientation. </param>
		/// <param name="newOrientation">The new orientation. </param>
		private void FireRotationEvent(Orientation originalOrientation, Orientation newOrientation)
		{
			if (this.RotationEvent != null)
			{
				RotationEventArgs rotationEventArgs = new RotationEventArgs(originalOrientation, newOrientation);
				this.RotationEvent(this, rotationEventArgs);
			}
		}

		#endregion
	}
}
