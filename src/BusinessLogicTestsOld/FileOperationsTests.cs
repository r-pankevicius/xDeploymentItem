using BusinessLogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace BusinessLogicTestsOld
{
	[TestClass]
    public class FileOperationsTests
	{
		public TestContext TestContext { get; set; }

		[TestMethod]
		public void NonExistingFileDoesntExist()
		{
			const string filename = "😺";
			bool exists = FileOperations.FileExists(filename);
			Assert.IsFalse(exists, $"File '{filename} should not exist.");
		}

		[TestMethod]
		[DeploymentItem("1-line.txt")]
		public void FileDeployedFromRoot()
		{
			string fullPath = Path.Combine(
				TestContext.DeploymentDirectory, "1-line.txt");
			Assert.IsTrue(FileOperations.FileExists(fullPath),
				$"File '{fullPath} should be deployed. Did you forget to set 'Copy if newer' property?");
		}

		[TestMethod]
		[DeploymentItem("1-line.txt", "thedirectory")]
		public void FileDeployedFromRootToSpecifiedDirectory()
		{
			string fullPath = Path.Combine(
				TestContext.DeploymentDirectory, "thedirectory", "1-line.txt");
			Assert.IsTrue(FileOperations.FileExists(fullPath),
				$"File '{fullPath} should be deployed. Did you forget to set 'Copy if newer' property?");
		}
	}
}
