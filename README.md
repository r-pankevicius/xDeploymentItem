# xDeploymentItem

The initial idea was to port Visual Studio Unit Testing DeploymentItem attribute to xUnit.
But after three days of thinking I redesigned it while initial idea was not lost.

# Porting Visual Studio Unit Testing DeploymentItem
## 1. Change "Copy to output directory" to embedded resources
![Change to embedded resource](./images/change-to-embedded-resources.png)
## 2. Change tests
From:
```csharp
[TestMethod]
[DeploymentItem("MyFile.txt")]
public void MyMethodWorksOnMyFile()
{
	string fullPathToMyFile = Path.Combine(TestContext.DeploymentDirectory, "MyFile.txt");
	Assert.IsTrue(MyClass.MyMethod(fullPathToMyFile));
}
```
To:
```csharp
[Fact]
public void MyMethodWorksOnMyFile()
{
	using (var deployer = new XDeploymentHelper(this))
	{
		string fullPathToMyFile = deployer.DeployEmbeddedResource("MyFile.txt");
		Assert.True(MyClass.MyMethod(fullPathToMyFile));
	}
}
```
## 3. That's all...
It should work, otherwisefill the issue if it doesn't.
All the XDeploymentHelper logic is [one small file](./src/xDeploymentItem/XDeploymentHelper.cs), copy it to your code base, read and debug it.

# Links

You can find that Visual Studio DeploymentItem documentation
[here](https://docs.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.testtools.unittesting.deploymentitemattribute?view=mstest-net-1.2.0).


