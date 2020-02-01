using System;
using System.Diagnostics;
using System.IO;

namespace OCR_App
{
	//Copy tesseract library to tesseractPath
	//var tesseractPath = @"c:\temp\tesseract-master.1153";
	//var imagePath = @"c:\temp\testImg.png";
	//var imageFile = File.ReadAllBytes(imagePath);
	//var text = OCR.ParseText(tesseractPath, imageFile, "eng");

	public static class OCR
    {
		/// <summary>
		/// Returns HOCR file from image
		/// </summary>
		/// <param name="tesseractPath"></param>
		/// <param name="imageFile"></param>
		/// <param name="lang"></param>
		/// <returns>hocr file</returns>
		public static string ParseText(string tesseractPath, byte[] imageFile, params string[] lang)
		{
			string output = string.Empty;
			var tempOutputFile = Path.GetTempPath() + Guid.NewGuid();
			var tempImageFile = Path.GetTempFileName();

			try
			{
				File.WriteAllBytes(tempImageFile, imageFile);

				ProcessStartInfo info = new ProcessStartInfo();
				info.WorkingDirectory = tesseractPath;
				info.WindowStyle = ProcessWindowStyle.Hidden;
				info.UseShellExecute = false;
				info.FileName = "cmd.exe";
				info.Arguments =
					"/c tesseract.exe " +
					// Image file.
					tempImageFile + " " +
					// Output file (tesseract add '.txt' at the end)
					tempOutputFile +
					// Languages.
					" -l " + string.Join("+", lang) + " tessdata/configs/hocr";

				// Start tesseract.
				Process process = Process.Start(info);
				process.WaitForExit();
				if (process.ExitCode == 0)
				{
					// Exit code: success.
					output = File.ReadAllText(tempOutputFile + ".hocr");

				}
				else
				{
					throw new Exception("Error. Tesseract stopped with an error code = " + process.ExitCode);
				}

			}
			finally
			{
				File.Delete(tempImageFile);
				File.Delete(tempOutputFile + ".hocr");
			}
			return output;
		}
	}
}
