using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace JpegRotater
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			try
			{
				Application.Run(new JpegRotator());
			}
			catch (Exception ex)
			{
				MessageBox.Show("An unexpected error occurred.  Message: " + ex.Message, Utility.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}
	}
}