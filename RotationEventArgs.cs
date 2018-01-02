using System;
using System.Collections.Generic;
using System.Text;

namespace JpegRotator
{
	/// <summary>
	/// Event arguments class that contains an Orientation instance to indicate the new orientation
	/// of an image as the result of an image rotation.
	/// </summary>
	public class RotationEventArgs : EventArgs
	{
		private Orientation _newOrientation;
		private Orientation _originalOrientation;

		/// <summary>
		/// Creates a new RotationEventArgs instance.
		/// </summary>
		/// <param name="originalOrientation">The original (pre-rotation) orientation of the image that was rotated. </param>
		/// <param name="newOrientation">The new (post-rotation) orientation of the image that was rotated. </param>
		public RotationEventArgs(Orientation originalOrientation, Orientation newOrientation)
		{
			this._originalOrientation = originalOrientation;
			this._newOrientation = newOrientation;
		}

		public Orientation NewOrientation
		{
			get
			{
				return this._newOrientation;
			}
		}

		public Orientation OriginalOrientation
		{
			get
			{
				return this._originalOrientation;
			}
		}
	}
}

