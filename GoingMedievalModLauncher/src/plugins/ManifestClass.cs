using System;

namespace GoingMedievalModLauncher.plugins
{
	[Serializable]
	public class ManifestClass
	{

		public string id { get; set; }
		public string version { get; set; }
		public string name {get; set; }
		
		public string description { get; set; }
		public string requirement { get; set; }
		public string[] dependencies { get; set; }
		public bool codeOnly { get; set; }

		public ManifestClass()
		{
			description = "";
			requirement = ModLoaderPluginContainer.Instance.ID;
			dependencies = new string[0];
		}

	}
}