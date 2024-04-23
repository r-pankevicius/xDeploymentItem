using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace xDeploy
{
	/// <summary>
	/// Helper that deploys embedded resource files to a temporary
	/// folder before test run and cleans them after test is finished.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Tries to mimic VisualStudio UnitTesting DeploymentItem,
	/// https://docs.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.testtools.unittesting.deploymentitemattribute?view=mstest-net-1.2.0
	/// </para>
	/// <para>
	/// It's agnostic about unit test framework so can be used with any of them.
	/// </para>
	/// </remarks>
	public class XDeploymentHelper : IDisposable
	{
		/// <summary>
		/// Object or assembly used to calculate paths to embedded resources.
		/// </summary>
		readonly object referenceObject;

		bool disposedValue;

		/// <summary>
		/// .ctor
		/// </summary>
		/// <param name="testInstanceOrAssembly">
		/// The instance of unit test class running the test or assembly
		/// where embedded resource is located.
		/// Used to calculate paths to embedded resources.
		/// </param>
		public XDeploymentHelper(object testInstanceOrAssembly)
		{
			referenceObject = testInstanceOrAssembly ??
				throw new ArgumentNullException(nameof(testInstanceOrAssembly));
		}

		/// <summary>
		/// Full path to deployment directory under temporary folder.
		/// Is valid directory only after first file deployment ar a call
		/// too <see cref="CreateDeploymentDirectory"/>.
		/// </summary>
		public string DeploymentDirectory { get; private set; }

		/// <summary>
		/// Deletes all files you deployed. <see cref="DeploymentDirectory"/>.
		/// </summary>
		/// <remarks>
		/// If you are hunting after random failure on build machine,
		/// TEMPORARY (anyway it will last ages) don't dispose.
		/// Deployed files will not be deleted.
		/// You will be able to see what was left after your tests.
		/// </remarks>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (DeploymentDirectory != null)
					Directory.Delete(DeploymentDirectory, recursive: true);

				disposedValue = true;
			}
		}

		~XDeploymentHelper()
		{
			Dispose(disposing: false);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Creates new deployment directory if not created yet.
		/// </summary>
		/// <returns>A path to directory created.</returns>
		public string CreateDeploymentDirectory()
		{
			if (DeploymentDirectory == null)
			{
				string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
				var dirInfo = Directory.CreateDirectory(path);
				DeploymentDirectory = dirInfo.FullName;
			}

			return DeploymentDirectory;
		}

		/// <summary>
		/// Loads embedded resource and saves it as afile to a temporary folder.
		/// </summary>
		/// <param name="resourcePath">Embedded resource path (with / and \ separators)</param>
		/// <returns>The full path to the file deployed.</returns>
		public string DeployEmbeddedResource(string resourcePath)
		{
			var parsedResourcePath = ResourcePathParser.Parse(resourcePath);
			VerifyEmbeddedResourcePath(parsedResourcePath);

			using (Stream resourceStream = GetResourceStream(parsedResourcePath))
			{
				string deployToPath = GetTargetFilePath(parsedResourcePath);
				using (var fileOutputStream = new FileStream(deployToPath, FileMode.CreateNew))
				{
					resourceStream.CopyTo(fileOutputStream);
				}

				return deployToPath;
			}
		}

		/// <summary>
		/// Loads embedded resource and saves it as afile to a temporary folder.
		/// </summary>
		/// <param name="resourcePath">Embedded resource path (with / and \ separators)</param>
		/// <param name="outputSubDirectory">
		/// Subdirectory where to deploy resource file (with / and \ separators)
		/// </param>
		/// <returns>The full path to the file deployed.</returns>
		public string DeployEmbeddedResource(string resourcePath, string outputSubDirectory)
		{
			var parsedResourcePath = ResourcePathParser.Parse(resourcePath);
			VerifyEmbeddedResourcePath(parsedResourcePath);

			var parsedOutputSubdirectory = ResourcePathParser.Parse(outputSubDirectory);
			if (parsedOutputSubdirectory.IsRoot)
				throw new ArgumentException($"{nameof(outputSubDirectory)} should be relative, not root.");

			using (Stream resourceStream = GetResourceStream(parsedResourcePath))
			{
				string deployToPath = GetTargetFilePath(parsedResourcePath, parsedOutputSubdirectory);
				using (var fileOutputStream = new FileStream(deployToPath, FileMode.CreateNew))
				{
					resourceStream.CopyTo(fileOutputStream);
				}

				return deployToPath;
			}
		}

		/// <summary>
		/// Convenience method to get embedded resource using "path syntax".
		/// </summary>
		/// <param name="resourcePath">Embedded resource path (with / and \ separators)</param>
		/// <returns>Resource stream</returns>
		public Stream GetEmbeddedResource(string resourcePath)
		{
			var parsedResourcePath = ResourcePathParser.Parse(resourcePath);
			VerifyEmbeddedResourcePath(parsedResourcePath);
			return GetResourceStream(parsedResourcePath);
		}

		/// <summary>
		/// Convenience method to get embedded resource as string using "path syntax".
		/// Uses BOM header to detect file encoding (uses <see cref="StreamReader(Stream, bool)"/>).
		/// </summary>
		/// <param name="resourcePath">Embedded resource path (with / and \ separators)</param>
		/// <returns>Contents of embedded resource as <c>string</c>.</returns>
		public string GetEmbeddedResourceAsString(string resourcePath)
		{
			var parsedResourcePath = ResourcePathParser.Parse(resourcePath);
			VerifyEmbeddedResourcePath(parsedResourcePath);

			using var stream = GetResourceStream(parsedResourcePath);
			using var textReader = new StreamReader(stream, detectEncodingFromByteOrderMarks: true);

			string result = textReader.ReadToEnd();
			return result;
		}

		/// <summary>
		/// Convenience method to get embedded resource as string using "path syntax".
		/// </summary>
		/// <param name="resourcePath">Embedded resource path (with / and \ separators)</param>
		/// <param name="encoding">Text encoding used to read file.</param>
		/// <returns>Contents of embedded resource as <c>string</c>.</returns>
		public string GetEmbeddedResourceAsString(string resourcePath, Encoding encoding)
		{
			var parsedResourcePath = ResourcePathParser.Parse(resourcePath);
			VerifyEmbeddedResourcePath(parsedResourcePath);
			
			using var stream = GetResourceStream(parsedResourcePath);
			using var textReader = new StreamReader(stream, encoding);

			string result = textReader.ReadToEnd();
			return result;
		}

		#region Implementation

		void VerifyEmbeddedResourcePath(ResourcePathParser.ParsedPath parsedResourcePath)
		{
			if (referenceObject is Assembly && !parsedResourcePath.IsRoot)
			{
				throw new ArgumentException(
					"Resource path must start with / or \\ if you passed Assembly as constructor argument.");
			}
		}

		Stream GetResourceStream(ResourcePathParser.ParsedPath parsedResourcePath)
		{
			string pathToEmbeddedRes = string.Join(".", parsedResourcePath.Parts);

			var assembly = referenceObject as Assembly;
			if (assembly == null)
			{
				var type = referenceObject.GetType();
				var typeInfo = type.GetTypeInfo();
				assembly = typeInfo.Assembly;
				if (!parsedResourcePath.IsRoot)
				{
					pathToEmbeddedRes = string.Concat(type.Namespace, '.', pathToEmbeddedRes);
				}
			}
			
			Stream resourceStream = assembly.GetManifestResourceStream(pathToEmbeddedRes);
			if (resourceStream == null)
				throw new ArgumentException(
					$"Resource stream {pathToEmbeddedRes} was not found in {assembly.FullName}");

			return resourceStream;
		}

		string GetTargetFilePath(ResourcePathParser.ParsedPath parsedResourcePath)
		{
			string fileName = parsedResourcePath.Parts.Last();
			CreateDeploymentDirectory();
			string result = Path.Combine(DeploymentDirectory, fileName);
			return result;
		}

		string GetTargetFilePath(
			ResourcePathParser.ParsedPath parsedResourcePath,
			ResourcePathParser.ParsedPath parsedOutputSubdirectory)
		{
			string subDirectory = string.Join(
				Path.DirectorySeparatorChar.ToString(), parsedOutputSubdirectory.Parts);
			CreateDeploymentDirectory();
			string pathToSubdirectory = Path.Combine(DeploymentDirectory, subDirectory);
			Directory.CreateDirectory(pathToSubdirectory);

			string fileName = parsedResourcePath.Parts.Last();
			string result = Path.Combine(pathToSubdirectory, fileName);
			return result;
		}

		#endregion

		#region Inner classes

		internal static class ResourcePathParser
		{
			public static ParsedPath Parse(string path)
			{
				if (string.IsNullOrWhiteSpace(path))
					throw new ArgumentException(nameof(path));

				string withForwardSlashes = path.Replace('\\', '/');
				if (withForwardSlashes.Contains("//"))
					throw new ArgumentException(
						$"Double slashes are not allowed in path: '{path}'");

				string[] parts = withForwardSlashes.Split(
					new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length == 0)
					throw new ArgumentException(nameof(path));

				var invalidFileNameChars = new HashSet<char>(Path.GetInvalidFileNameChars());
				foreach (string part in parts)
				{
					char[] partChars = part.ToCharArray();

					if (partChars.All(ch => ch == '.'))
						throw new ArgumentException($"'Points only' parts are not allowed in paths.");

					if (partChars.Any(ch => invalidFileNameChars.Contains(ch)))
						throw new ArgumentException($"Invalid file name chars found in the part '{part}'");
				}

				bool isRoot = withForwardSlashes.StartsWith("/", StringComparison.Ordinal);

				return new ParsedPath(isRoot, parts);
			}

			internal struct ParsedPath
			{
				public bool IsRoot { get; private set; }
				public string[] Parts { get; private set; }

				public ParsedPath(bool isRoot, string[] parts)
				{
					IsRoot = isRoot;
					Parts = parts;
				}
			}
		}

		#endregion
	}
}
