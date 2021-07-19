using System;

namespace GoingMedievalModLauncher.plugins
{
	[Serializable]
	public class ManifestClass
	{

		public string id;
		public string version;
		public string name;

		public string description;
		public string requirement;
		public string[] dependencies;
		public bool codeOnly;
		public bool noCode;

		public ManifestClass()
		{
			description = "";
			requirement = ModLoaderPluginContainer.Instance.ID;
			dependencies = new string[0];
			codeOnly = false;
			noCode = false;
		}

	}
}