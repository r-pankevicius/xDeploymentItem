using System;
using xDeploy;
using Xunit;
using Xunit.Abstractions;

namespace BusinessLogicTestsNew
{
	public class FileOperationsTests : IDisposable
	{
		readonly ITestOutputHelper _testOutputWriter;
		readonly XDeploymentHelper _deployMe;

		public FileOperationsTests(ITestOutputHelper outputWriter)
		{
			_testOutputWriter = outputWriter;
			_deployMe = new XDeploymentHelper();
		}

		public void Dispose()
		{
			_deployMe?.Dispose();
		}

		[Fact]
		public void DeploymentFileExists()
		{
			// ...
		}

	}
}
