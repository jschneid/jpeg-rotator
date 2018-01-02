using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;


namespace JpegRotater
{
	/// <summary>
	/// This class defines static methods that create and manipulate images.
	/// </summary>
	class Utility
	{
		//Private constructor to prevent instantiation.
		private Utility()
		{
		}

		public const string APP_NAME = "Digital Photo Auto-Rotator";

		/// <summary>
		/// The tag number of the Orientation tag from the EXIF specification.
		/// See: http://park2.wakwak.com/~tsuruzoh/Computer/Digicams/exif-e.html
		/// </summary>
		private const int ROTATION_PROPERTY_ID = 0X0112;

		/// <summary>
		/// Returns a Bitmap with the specified image resized to fit within the target size,
		/// preserving the aspect ratio of the original image.
		/// 
		/// If the specified image completely fits within the target size (i.e. the image is 
		/// smaller than the target in both width and height), a bitmap with a copy of the 
		/// original image (with its size unchanged) is returned.
		/// </summary>
		/// <param name="image">The image to resize. </param>
		/// <param name="targetSize">The rectangluar size that the resized image needs to fit within. </param>
		/// <returns>A Bitmap with the resized image. </returns>
		public static Bitmap ResizeImageToFitRectangle(Image image, Size targetSize)
		{
			//TODO: Need to worry about horiz/vert centering of the new image if original isn't a square?

			//If the image is smaller than the target, don't do any resizing.
			if (image.Width <= targetSize.Width && image.Height <= targetSize.Height)
			{
				Bitmap bitmap = new Bitmap(image);
				return bitmap;
			}

			//Determine the target width and height of the new image such that the aspect ratio of the
			//original will be preserved.
			//float widthRatio = ((float)image.Width) / ((float)targetSize.Width);
			//float heightRatio = ((float)image.Height) / ((float)targetSize.Height);
			int widthDelta = image.Width - targetSize.Width;
			int heightDelta = image.Height - targetSize.Height;
			double resizeRatio;
			if (widthDelta >= heightDelta)
			{
				//The image needs to be resized such that the width of the image will fit within the width
				//of the target area.
				resizeRatio = ((double)image.Width) / ((double)targetSize.Width);
			}
			else
			{
				//The image needs to be resized such that the height of the image will fit within the height
				//of the target area.
				resizeRatio = ((double)image.Height) / ((double)targetSize.Height);
			}
			int targetWidth = (int)(image.Width / resizeRatio);
			int targetHeight = (int)(image.Height / resizeRatio);

			Bitmap resizedImage = new Bitmap(targetWidth, targetHeight);  //TODO: What should be used for the format param if anything?
			resizedImage.SetResolution(image.HorizontalResolution, image.VerticalResolution); //TODO: NEcessary?
			Graphics resizedImageGraphics = Graphics.FromImage(resizedImage);

			//Use HighQualityBicubic mode to produce the highest-quality shrunk images.
			resizedImageGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

			resizedImageGraphics.DrawImage(image,
				new Rectangle(0, 0, targetWidth, targetHeight),
				new Rectangle(0, 0, image.Width, image.Height),
				GraphicsUnit.Pixel);
			resizedImageGraphics.Dispose();
			return resizedImage;

		}

		/// <summary>
		/// Gets the System.Drawing.RotateFlipType that is equivalent to the specified Orientation.
		/// </summary>
		/// <param name="orientation">The Orientation to convert. </param>
		/// <returns>The equivalent RotateFlipType. </returns>
		public static RotateFlipType RotateFlipTypeFromOrientation(Orientation orientation)
		{
			switch (orientation)
			{
				case Orientation.Initial:
					return RotateFlipType.RotateNoneFlipNone;
				case Orientation.Left:
					return RotateFlipType.Rotate270FlipNone;
				case Orientation.Inverted:
					return RotateFlipType.Rotate180FlipNone;
				case Orientation.Right:
					return RotateFlipType.Rotate90FlipNone;
				default:
					throw new ApplicationException("Unrecognized Orientation value specfied: " + orientation.ToString());
			}
		}

		/// <summary>
		/// Gets the System.Drawing.Imaging.EncoderValue that is equivalent to the specified 
		/// Orientation.  Note: Orientation.Initial is not supported.
		/// </summary>
		/// <param name="orientation">The Orientation to convert. </param>
		/// <returns>The equivalent EncoderValue. </returns>
		public static EncoderValue EncoderValueFromOrientation(Orientation orientation)
		{
			switch (orientation)
			{
				case Orientation.Left:
					return EncoderValue.TransformRotate270;
				case Orientation.Inverted:
					return EncoderValue.TransformRotate180;
				case Orientation.Right:
					return EncoderValue.TransformRotate90;
				case Orientation.Initial:
					throw new ApplicationException("Orientation.Initial is not supported.");
				default:
					throw new ApplicationException("Unrecognized Orientation value specfied: " + orientation.ToString());
			}
		}


		/// <summary>
		/// Gets the Orientation enum value that is equivalent to the specified EXIF
		/// Orientation tag value.
		/// 
		/// Values used in this method are taken from the EXIF file format spec at:
		/// http://park2.wakwak.com/~tsuruzoh/Computer/Digicams/exif-e.html
		/// </summary>
		/// <param name="exifRotationPropertyValue">The EXIF Orientation tag value. </param>
		/// <returns>An Orientation enum value.</returns>
		public static Orientation OrientationFromExifRotationPropertyValue(int exifRotationPropertyValue)
		{
			switch (exifRotationPropertyValue)
			{
				case 1:
					return Orientation.Initial;
				case 3:
					return Orientation.Inverted; //NOTE: I haven't tested this one! Might really be 4. See http://park2.wakwak.com/~tsuruzoh/Computer/Digicams/exif-e.html
				case 6:
					return Orientation.Left;
				case 8:
					return Orientation.Right;
				default:
					//NOTE: Would be nice to perform more sophisticated error handling here;
					//for now, just treat the file as though the EXIF data didn't specify any
					//rotation.  The user can decide whether the image should be rotated.
					return Orientation.Initial;
			}
		}

		/// <summary>
		/// Gets the EXIF Orientation tag value that is equivalent to the specified 
		/// Orientation enum value.
		/// </summary>
		/// <param name="orientation">The Orientation enum value. </param>
		/// <returns>The EXIF Orientation tag value. </returns>
		public static int ExifRotationPropertyValueFromOrientation(Orientation orientation)
		{
			switch (orientation)
			{
				case Orientation.Initial:
					return 1;
				case Orientation.Inverted:
					return 3;
				case Orientation.Left:
					return 6;
				case Orientation.Right:
					return 8;
				default:
					throw new ApplicationException("Unrecognized Orientation value: " + orientation);
			}
		}

		/// <summary>
		/// Returns a new Image that is equivalent to the specified image, at the specified
		/// orientation.  (The new image is the input image rotated to match the specified 
		/// orientation.)
		/// 
		/// Note: The rotated image may not be a lossless version of the specified image. 
		/// (I haven't tested to be sure, and the documentation for the Image.RotateFlip 
		/// method used to perform the rotation doesn't address the issue.)  This method may 
		/// not be suitable for creating files to be saved out to disk, but it does work 
		/// plenty well enough for in-memory manipulation of thumbnail images.
		/// </summary>
		/// <param name="image">The image to be rotated. </param>
		/// <param name="orientation">The desired orientation of the rotated image. </param>
		/// <returns>A new image that is equivalent to the input image with the specified 
		/// rotation applied. </returns>
		public static Image RotateImage(Image image, Orientation orientation)
		{
			//Get the RotateFlipType that corresponds to the new orientation.
			RotateFlipType rotateFlipType = RotateFlipTypeFromOrientation(orientation);

			Image rotatedImage = (Image)image.Clone();
			rotatedImage.RotateFlip(rotateFlipType);
			return rotatedImage;
		}

		/// <summary>
		/// Gets the current orientation of an image, based on the EXIF header data from
		/// the .jpg file.
		/// </summary>
		/// <param name="image">The image to inspect. </param>
		/// <returns>The orientation of the image, or Orientiation.Initial if no orientation
		/// was found, or if invalid/unsupported orientation information was found. </returns>
		public static Orientation GetImageOrientation(Image image)
		{
			int exifRotationPropertyValue = image.GetPropertyItem(ROTATION_PROPERTY_ID).Value[0];

			Orientation orientation = OrientationFromExifRotationPropertyValue(exifRotationPropertyValue);
			return orientation;
		}

		/// <summary>
		/// Overwrites the specified file on disk with a new version of the file rotated according
		/// to the specified Orientation.  Exceptions generated by an error writing the new file will
		/// bubble up from this method.  
		/// 
		/// Before the original file is overwritten, a copy is made in the temp files folder (so that 
		/// the original image isn't lost if the write of the rotated version fails).  This temporary
		/// copy is deleted before the method returns if the write of the new rotated image is successful.
		/// </summary>
		/// <param name="filename">The fully-specified name of the file to rotate. </param>
		/// <param name="orientation">The orientation to apply to the rotated file. </param>
		public static void RotateAndSaveImageFile(string filename, Orientation orientation)
		{
			if (orientation == Orientation.Initial)
			{
				//No rotation was specified, so no need to change anything.
				return;
			}

			//Make a backup copy of the original image in the temp folder (so that if we hit a
			//failure during writing of the new image, both the original and new copies of the 
			//image aren't gone).  
			string backupFilename = GetBackupFilename(filename);
			System.IO.File.Copy(filename, backupFilename, true);

			//Since our original image is dependant on the existing file on disk to be saved,
			//we can't delete the original image file until the rotated image file is written.
			//Get a temporary file that we'll use below to write the new image data.
			string tempFilename = System.IO.Path.GetTempFileName();

			//Load the image from the specified filename
			using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
			{

				using (Image image = Image.FromStream(fileStream, true, false))
				{

					//Calculate the new value for the EXIF rotation property, and set it on the image.  (This is
					//calculated as the original EXIF rotation as read from the file, with the rotation performed
					//in the app applied to that value.  Thus if the original EXIF rotation tag was incorrect,
					//and the user manually rotated the image to be upright, the tag will continue to be incorrect.)
					PropertyItem rotationPropertyItem = image.GetPropertyItem(ROTATION_PROPERTY_ID);
					Orientation originalOrientation = OrientationFromExifRotationPropertyValue(rotationPropertyItem.Value[0]);
					Orientation newOrientation = (Orientation)((((int)originalOrientation) + ((int)orientation)) % 4);
					int newExifRotationPropertyValue = ExifRotationPropertyValueFromOrientation(newOrientation);
					rotationPropertyItem.Value[0] = (byte)newExifRotationPropertyValue;
					image.SetPropertyItem(rotationPropertyItem);

					//Setup the EncoderParameters with the information that we will be performing when 
					//saving the rotated file.
					EncoderParameters encoderParams = new EncoderParameters(1);
					EncoderValue rotationEncoderValue = EncoderValueFromOrientation(orientation);
					EncoderParameter rotationParam = new EncoderParameter(Encoder.Transformation, (long)rotationEncoderValue);
					encoderParams.Param[0] = rotationParam;

					//Setup the .jpg ImageCodecInfo that we need to save the rotated file.
					ImageCodecInfo jpegCodecInfo = GetEncoderInfo("image/jpeg");

					using (FileStream outFileStream = new FileStream(tempFilename, FileMode.Open, FileAccess.Write))
					{
						image.Save(outFileStream, jpegCodecInfo, encoderParams);
					}
				}
			}

			//Delete the original image (making room for us to rename the new, rotated copy).
			System.IO.File.Delete(filename);

			try
			{
				//Now, move the rotated image that we saved out as a temporary file to the name and
				//location of the original file.
				File.Move(tempFilename, filename);
			}
			catch
			{
				//An error occurred moving the rotated file.  Try and retore the original file from
				//the copy that we saved in the temp folder, so that the file isn't just "gone" as
				//far as the user can tell.
				File.Move(backupFilename, filename);

				//Let the exception bubble up -- an error dialog will be displayed to the user.
				throw;
			}

			//Since we got to this point without an exception being thrown, the write of the new
			//file was apparently successful.  Delete the copy of the original file that we made
			//in the temp folder.
			System.IO.File.Delete(backupFilename);
		}

		/// <summary>
		/// Gets a fully-specified filename which has the path of the system's temp files folder
		/// and the file name of the specified file.  
		/// </summary>
		/// <param name="originalFilename">The original fully-specified filename. </param>
		/// <returns>The filename of the original file in the temp folder. </returns>
		private static string GetBackupFilename(string originalFilename)
		{
			string tempPath = System.IO.Path.GetTempPath();
			string fileNameWithoutPath = System.IO.Path.GetFileName(originalFilename);
			string tempFileName = tempPath + fileNameWithoutPath;
			return tempFileName;
		}


		/// <summary>
		/// Returns in ImageCodecInfo to use for saving out files with the specified MIME
		/// type to disk with the System.Drawing.Image.Save method.
		/// 
		/// Code adapted from: http://www.eggheadcafe.com/articles/20030706.asp
		/// </summary>
		private static ImageCodecInfo GetEncoderInfo(string mimeType)
		{
			int j;
			ImageCodecInfo[] encoders;
			encoders = ImageCodecInfo.GetImageEncoders();
			for (j = 0; j < encoders.Length; ++j)
			{
				if (encoders[j].MimeType == mimeType)
				{
					return encoders[j];
				}
			}

			throw new ApplicationException("Couldn't find an encoder for MIME type: " + mimeType);
		}
	}
}
