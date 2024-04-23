using System;
using xDeploy;
using Xunit;

namespace BusinessLogicTestsNew;

/// <summary>
/// xDeploymentItem self-check tests.
/// </summary>
public class FileOperationsSelfTests
{
	[Fact]
	public void DoesntAllowToDeployToUpperFolder()
	{
		using var deployer = new XDeploymentHelper(this);

		Assert.Throws<ArgumentException>(() => deployer.DeployEmbeddedResource("1-line.txt", ".."));
		Assert.Throws<ArgumentException>(() => deployer.DeployEmbeddedResource("1-line.txt", @"dir\..\.."));
		Assert.Throws<ArgumentException>(() => deployer.DeployEmbeddedResource("1-line.txt", "dir/../.."));
	}
}
