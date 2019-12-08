using System;

namespace xDeploy
{
	public class XDeploymentHelper : IDisposable
	{

		public string DeploymentDirectory { get; private set; }

		public void Dispose()
		{
			// ...
		}

		public string CreateDeploymentDirectory()
		{
			throw new NotImplementedException();
		}

		public string DeployEmbeddedResource(string resourcePath)
		{
			throw new NotImplementedException();
		}

		public string DeployEmbeddedResource(string resourcePath, string outputSubDirectory)
		{
			throw new NotImplementedException();
		}
	}
}
