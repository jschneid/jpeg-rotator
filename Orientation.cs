using System;
using System.Collections.Generic;
using System.Text;

namespace JpegRotator
{
	/// <summary>
	/// Represents the current rotated orientation of an image relative to that image's
	/// initial orientation.
	/// </summary>
	public enum Orientation
	{
		Initial = 0,	//Image hasn't been rotated
		Left = 1,		//Image rotated 90 degrees counterclockwise
		Inverted = 2,	//Image rotated 180 degrees 
		Right = 3		//Image rotated 90 degrees clockwise
	}
}
