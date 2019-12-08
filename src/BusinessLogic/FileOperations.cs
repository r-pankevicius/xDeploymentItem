using System;
using System.IO;

namespace BusinessLogic
{
	/// <summary>
	/// This is a "business" class under tests.
	/// </summary>
	public static class FileOperations
	{
		public static bool FileExists(string pathToFile)
		{
			bool result = File.Exists(pathToFile);
			return result;
		}

		public static int GetLineCount(string pathToFile)
		{
			if (!File.Exists(pathToFile))
				throw new ArgumentException($"File {pathToFile} doesn't exist.");

			int result = File.ReadAllLines(pathToFile).Length;
			return result;
		}
	}
}
