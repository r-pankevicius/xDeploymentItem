using BusinessLogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace BusinessLogicTestsOld
{
	[TestClass]
    public class FileOperationsTests
	{
		public TestContext TestContext { get; set; }

		/// <summary>
		/// Checks if we don't use same folder for multiple deployments.
		/// <para>
		/// We have to ignore it in VisualStudio UnitTesting because running
		/// all tests at once deploys files and folders to the same place
		/// for all tests.
		/// </para>
		/// </summary>
		[TestMethod]
		[Ignore]
		public void NotDeployedFileDoesntExist()
		{
			TestContext.WriteLine($"Deployment directory is {TestContext.DeploymentDirectory}");

			string[] pathsToCheck = {
				"1-line.txt",
				"2-lines.txt",
				"thedirectory"
			};

			foreach (string fileOrFolder in pathsToCheck)
			{
				string fullPath = Path.Combine(TestContext.DeploymentDirectory, fileOrFolder);
				bool exists = File.Exists(fullPath);
				Assert.IsFalse(exists, $"File '{fullPath} should not exist.");
				exists = Directory.Exists(fullPath);
				Assert.IsFalse(exists, $"Directory '{fullPath} should not exist.");
			}
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

		/* EXAMPLE
		[TestMethod]
		[DeploymentItem("MyFile.txt")]
		public void MyMethodWorksOnMyFile()
		{
			string fullPathToMyFile = Path.Combine(TestContext.DeploymentDirectory, "MyFile.txt");
			Assert.IsTrue(MyClass.MyMethod(fullPathToMyFile));
		}
		*/
	}
}
