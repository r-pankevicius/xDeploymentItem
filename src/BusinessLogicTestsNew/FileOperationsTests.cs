using xDeploy;
using Xunit;

namespace BusinessLogicTestsNew;

public class FileOperationsTests
{
	[Theory]
	[InlineData("1-line.txt", "line 1")]
	[InlineData("SomeFolder/2-lines.txt", "line 1\nline 2")]
	[InlineData(@"SomeFolder\2-lines.txt", "line 1\nline 2")]
	public void ReadsStrings(string resourcePath, string expectedString)
	{
		using var deployer = new XDeploymentHelper(this);

		Assert.Equal(expectedString, deployer.GetEmbeddedResourceAsString(resourcePath));
	}
}
