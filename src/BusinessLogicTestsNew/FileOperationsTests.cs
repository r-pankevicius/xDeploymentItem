using BusinessLogic;
using System;
using System.IO;
using xDeploy;
using Xunit;
using Xunit.Abstractions;

namespace BusinessLogicTestsNew
{
	public class FileOperationsTests
	{
		readonly ITestOutputHelper testOutputWriter;

		public FileOperationsTests(ITestOutputHelper outputWriter)
		{
			testOutputWriter = outputWriter;
		}

		/// <summary>
		/// Checks if we don't use same folder for multiple deployments.
		/// <para>
		/// While in VisualStudio UnitTesting running multiple tests at once
		/// deploys files and folders to the same place, it's definitely a way
		/// to run into troubles. So xDeploy tries to keep separate deployment
		/// folders for each test.
		/// </para>
		/// </summary>
		[Fact]
		public void NotDeployedFileDoesntExist()
		{
			void RunChecks(XDeploymentHelper deployer)
			{
				string[] pathsToCheck = {
					"1-line.txt",
					"2-lines.txt",
					"thedirectory"
				};

				foreach (string fileOrFolder in pathsToCheck)
				{
					string fullPath = Path.Combine(deployer.DeploymentDirectory, fileOrFolder);
					bool exists = File.Exists(fullPath);
					Assert.False(exists, $"File '{fullPath} should not exist.");
					exists = Directory.Exists(fullPath);
					Assert.False(exists, $"Directory '{fullPath} should not exist.");
				}
			}

			using (var deployer = new XDeploymentHelper(this))
			{
				testOutputWriter.WriteLine($"Deployment directory is {deployer.DeploymentDirectory}");
				// DeploymentDirectory should not exist before first deployment
				Assert.Null(deployer.DeploymentDirectory);

				string pathReturned = deployer.CreateDeploymentDirectory();
				Assert.Equal(pathReturned, deployer.DeploymentDirectory);
				RunChecks(deployer);
			}
		}

		[Fact]
		public void FileDeployedFromRoot()
		{
			using (var deployer = new XDeploymentHelper(this))
			{
				string pathToFile = deployer.DeployEmbeddedResource("1-line.txt"); // -> Out\1 - line.txt

				testOutputWriter.WriteLine($"Deployment directory is {deployer.DeploymentDirectory}");

				string fullPath = Path.Combine(
					deployer.DeploymentDirectory, "1-line.txt");
				Assert.True(FileOperations.FileExists(fullPath),
					$"File '{fullPath} should be deployed. Did you forget to set 'Copy if newer' property?");
			}
		}

		[Fact]
		public void FileDeployedFromRootToSpecifiedDirectory()
		{
			using (var deployer = new XDeploymentHelper(this))
			{
				string pathToFile = deployer.DeployEmbeddedResource(
					"1-line.txt", "thedirectory"); // -> Out\thedirectory\1-line.txt

				testOutputWriter.WriteLine($"Deployment directory is {deployer.DeploymentDirectory}");

				string fullPath = Path.Combine(
					deployer.DeploymentDirectory, "thedirectory", "1-line.txt");
				Assert.True(FileOperations.FileExists(fullPath),
					$"File '{fullPath} should be deployed. Did you forget to set 'Copy if newer' property?");
			}
		}

		[Fact]
		public void FileDeployedFromRootToSpecifiedNestedDirectory()
		{
			using (var deployer = new XDeploymentHelper(this))
			{
				string pathToFile = deployer.DeployEmbeddedResource(
					"1-line.txt", @"thedirectory\nesteddirectory"); // -> Out\thedirectory\nesteddirectory\1-line.txt

				testOutputWriter.WriteLine($"Deployment directory is {deployer.DeploymentDirectory}");

				string fullPath = Path.Combine(
					deployer.DeploymentDirectory, "thedirectory", "nesteddirectory", "1-line.txt");
				Assert.True(FileOperations.FileExists(fullPath),
					$"File '{fullPath} should be deployed. Did you forget to set 'Copy if newer' property?");
			}
		}

		[Fact]
		public void FileDeployedFromNestedFolder()
		{
			using (var deployer = new XDeploymentHelper(this))
			{
				string pathToFile = deployer.DeployEmbeddedResource(@"SomeFolder\2-lines.txt"); // -> Out\2-lines.txt

				testOutputWriter.WriteLine($"Deployment directory is {deployer.DeploymentDirectory}");

				string fullPath = Path.Combine(
					deployer.DeploymentDirectory, "2-lines.txt");
				Assert.True(FileOperations.FileExists(fullPath),
					$"File '{fullPath} should be deployed. Did you forget to set 'Copy if newer' property?");
			}
		}

		[Fact]
		public void FileDeployedFromNestedFolderToSpecifiedDirectory()
		{
			using (var deployer = new XDeploymentHelper(this))
			{
				string pathToFile = deployer.DeployEmbeddedResource(
					@"SomeFolder\2-lines.txt", "thedirectory"); // -> Out\thedirectory\2-lines.txt

				testOutputWriter.WriteLine($"Deployment directory is {deployer.DeploymentDirectory}");

				string fullPath = Path.Combine(
					deployer.DeploymentDirectory, "thedirectory", "2-lines.txt");
				Assert.True(FileOperations.FileExists(fullPath),
					$"File '{fullPath} should be deployed. Did you forget to set 'Copy if newer' property?");
			}
		}

		[Fact]
		public void FileDeployedFromNestedFolderToSpecifiedNestedDirectory()
		{
			using (var deployer = new XDeploymentHelper(this))
			{
				string pathToFile = deployer.DeployEmbeddedResource(
					@"SomeFolder\2-lines.txt", @"thedirectory\nesteddirectory"); // -> Out\thedirectory\nesteddirectory\2-lines.txt

				testOutputWriter.WriteLine($"Deployment directory is {deployer.DeploymentDirectory}");

				string fullPath = Path.Combine(
					deployer.DeploymentDirectory, "thedirectory", "nesteddirectory", "2-lines.txt");
				Assert.True(FileOperations.FileExists(fullPath),
					$"File '{fullPath} should be deployed. Did you forget to set 'Copy if newer' property?");
			}
		}
	}
}
