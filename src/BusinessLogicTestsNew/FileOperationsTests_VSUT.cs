using BusinessLogic;
using System.IO;
using xDeploy;
using Xunit;
using Xunit.Abstractions;

namespace BusinessLogicTestsNew;

/// <summary>
/// Side-by-side tests that match Visual Studio Unit Test version of FileOperationsTests.
/// </summary>
public class FileOperationsTests_VSUT
{
	readonly ITestOutputHelper testOutputWriter;

	public FileOperationsTests_VSUT(ITestOutputHelper outputWriter)
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
		string deploymentDirectory;

		using (var deployer = new XDeploymentHelper(this))
		{
			testOutputWriter.WriteLine($"Deployment directory is {deployer.DeploymentDirectory}");
			// DeploymentDirectory should not exist before first deployment
			Assert.Null(deployer.DeploymentDirectory);

			string pathReturned = deployer.CreateDeploymentDirectory();
			Assert.Equal(pathReturned, deployer.DeploymentDirectory);

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

			deploymentDirectory = deployer.DeploymentDirectory;
		}

		Assert.False(Directory.Exists(deploymentDirectory), "Deployment directory should be deleted in Dispose()");
	}

	[Fact]
	public void FileDeployedFromRoot()
	{
		using var deployer = new XDeploymentHelper(this);

		string fullPathToFile = deployer.DeployEmbeddedResource("1-line.txt"); // -> Out\1 - line.txt
		testOutputWriter.WriteLine($"Deployment directory is {deployer.DeploymentDirectory}");
		Assert.True(FileOperations.FileExists(fullPathToFile));
	}

	[Fact]
	public void FileDeployedFromRootToSpecifiedDirectory()
	{
		using var deployer = new XDeploymentHelper(this);

		string fullPathToFile = deployer.DeployEmbeddedResource(
			"1-line.txt", "thedirectory"); // -> Out\thedirectory\1-line.txt
		testOutputWriter.WriteLine($"Deployment directory is {deployer.DeploymentDirectory}");
		Assert.True(FileOperations.FileExists(fullPathToFile),
			$"File '{fullPathToFile} should be deployed. Did you forget to set 'Embedded resource' property?");
	}

	[Fact]
	public void FileDeployedFromRootToSpecifiedNestedDirectory()
	{
		using var deployer = new XDeploymentHelper(this);

		string fullPathToFile = deployer.DeployEmbeddedResource(
			"1-line.txt", @"thedirectory\nesteddirectory"); // -> Out\thedirectory\nesteddirectory\1-line.txt
		testOutputWriter.WriteLine($"Deployment directory is {deployer.DeploymentDirectory}");
		Assert.True(FileOperations.FileExists(fullPathToFile),
			$"File '{fullPathToFile} should be deployed. Did you forget to set 'Embedded resource' property?");
	}

	[Fact]
	public void FileDeployedFromNestedFolder()
	{
		using var deployer = new XDeploymentHelper(this);

		string fullPathToFile = deployer.DeployEmbeddedResource(@"SomeFolder\2-lines.txt"); // -> Out\2-lines.txt
		testOutputWriter.WriteLine($"Deployment directory is {deployer.DeploymentDirectory}");
		Assert.True(FileOperations.FileExists(fullPathToFile),
			$"File '{fullPathToFile} should be deployed. Did you forget to set 'Embedded resource' property?");
	}

	[Fact]
	public void FileDeployedFromNestedFolderToSpecifiedDirectory()
	{
		using var deployer = new XDeploymentHelper(this);

		string pfullPathToFileathToFile = deployer.DeployEmbeddedResource(
			@"SomeFolder\2-lines.txt", "thedirectory"); // -> Out\thedirectory\2-lines.txt
		testOutputWriter.WriteLine($"Deployment directory is {deployer.DeploymentDirectory}");
		Assert.True(FileOperations.FileExists(pfullPathToFileathToFile),
			$"File '{pfullPathToFileathToFile} should be deployed. Did you forget to set 'Embedded resource' property?");
	}

	[Fact]
	public void FileDeployedFromNestedFolderToSpecifiedNestedDirectory()
	{
		using var deployer = new XDeploymentHelper(this);

		string fullPathToFile = deployer.DeployEmbeddedResource(
			@"SomeFolder\2-lines.txt", @"thedirectory\nesteddirectory"); // -> Out\thedirectory\nesteddirectory\2-lines.txt
		testOutputWriter.WriteLine($"Deployment directory is {deployer.DeploymentDirectory}");
		Assert.True(FileOperations.FileExists(fullPathToFile),
			$"File '{fullPathToFile} should be deployed. Did you forget to set 'Embedded resource' property?");
	}
}
