using System;
using xDeploy;
using Xunit;

namespace BusinessLogicTestsNew
{
	/// <summary>
	/// xDeploymentItem specific tests.
	/// </summary>
	public class FileOperationsTests_Own
	{
		[Fact]
		public void DoesntAllowToDeployToUpperFolder()
		{
			using (var deployer = new XDeploymentHelper(this))
			{
				Assert.Throws<ArgumentException>(() => deployer.DeployEmbeddedResource("1-line.txt", ".."));
				Assert.Throws<ArgumentException>(() => deployer.DeployEmbeddedResource("1-line.txt", @"dir\..\.."));
				Assert.Throws<ArgumentException>(() => deployer.DeployEmbeddedResource("1-line.txt", "dir/../.."));
			}
		}
	}
}
