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
			TestContext.WriteLine($"Deployment directory is {TestContext.DeploymentDirectory}");

			const string filename = "😺";
			string pathToFile = Path.Combine(TestContext.DeploymentDirectory, filename);
			bool exists = FileOperations.FileExists(pathToFile);
			Assert.IsFalse(exists, $"File '{filename} should not exist.");
		}

		[TestMethod]
		[DeploymentItem("1-line.txt")]
		// -> Out\1-line.txt
		public void FileDeployedFromRoot()
		{
			TestContext.WriteLine($"Deployment directory is {TestContext.DeploymentDirectory}");

			string fullPath = Path.Combine(
				TestContext.DeploymentDirectory, "1-line.txt");
			Assert.IsTrue(FileOperations.FileExists(fullPath),
				$"File '{fullPath} should be deployed. Did you forget to set 'Copy if newer' property?");
		}

		[TestMethod]
		[DeploymentItem("1-line.txt", "thedirectory")]
		// -> Out\thedirectory\1-line.txt
		public void FileDeployedFromRootToSpecifiedDirectory()
		{
			TestContext.WriteLine($"Deployment directory is {TestContext.DeploymentDirectory}");

			string fullPath = Path.Combine(
				TestContext.DeploymentDirectory, "thedirectory", "1-line.txt");
			Assert.IsTrue(FileOperations.FileExists(fullPath),
				$"File '{fullPath} should be deployed. Did you forget to set 'Copy if newer' property?");
		}

		[TestMethod]
		[DeploymentItem("1-line.txt", @"thedirectory\nesteddirectory")]
		// -> Out\thedirectory\nesteddirectory\1-line.txt
		public void FileDeployedFromRootToSpecifiedNestedDirectory()
		{
			TestContext.WriteLine($"Deployment directory is {TestContext.DeploymentDirectory}");

			string fullPath = Path.Combine(
				TestContext.DeploymentDirectory, "thedirectory", "nesteddirectory", "1-line.txt");
			Assert.IsTrue(FileOperations.FileExists(fullPath),
				$"File '{fullPath} should be deployed. Did you forget to set 'Copy if newer' property?");
		}

		[TestMethod]
		[DeploymentItem(@"SomeFolder\2-lines.txt")]
		// -> Out\2-lines.txt
		public void FileDeployedFromNestedFolder()
		{
			TestContext.WriteLine($"Deployment directory is {TestContext.DeploymentDirectory}");

			string fullPath = Path.Combine(
				TestContext.DeploymentDirectory, "2-lines.txt");
			Assert.IsTrue(FileOperations.FileExists(fullPath),
				$"File '{fullPath} should be deployed. Did you forget to set 'Copy if newer' property?");
		}

		[TestMethod]
		[DeploymentItem(@"SomeFolder\2-lines.txt", "thedirectory")]
		// -> Out\thedirectory\2-lines.txt
		public void FileDeployedFromNestedFolderToSpecifiedDirectory()
		{
			TestContext.WriteLine($"Deployment directory is {TestContext.DeploymentDirectory}");

			string fullPath = Path.Combine(
				TestContext.DeploymentDirectory, "thedirectory", "2-lines.txt");
			Assert.IsTrue(FileOperations.FileExists(fullPath),
				$"File '{fullPath} should be deployed. Did you forget to set 'Copy if newer' property?");
		}

		[TestMethod]
		[DeploymentItem(@"SomeFolder\2-lines.txt", @"thedirectory\nesteddirectory")]
		// -> Out\thedirectory\nesteddirectory\2-lines.txt
		public void FileDeployedFromNestedFolderToSpecifiedNestedDirectory()
		{
			TestContext.WriteLine($"Deployment directory is {TestContext.DeploymentDirectory}");

			string fullPath = Path.Combine(
				TestContext.DeploymentDirectory, "thedirectory", "nesteddirectory", "1-line.txt");
			Assert.IsTrue(FileOperations.FileExists(fullPath),
				$"File '{fullPath} should be deployed. Did you forget to set 'Copy if newer' property?");
		}
	}
}
