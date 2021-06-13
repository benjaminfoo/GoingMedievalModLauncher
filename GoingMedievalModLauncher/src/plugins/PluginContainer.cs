using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using I2.Loc;
using Newtonsoft.Json;
using NSMedieval.Enums;


namespace GoingMedievalModLauncher
{
	
	/// <summary>
	/// A class that contains a plugin.
	/// TODO: the static tags (Name description) should be separated from code?
	/// </summary>
	public class PluginContainer
	{

		/// <summary>
		/// the mod's folder
		/// </summary>
		private readonly DirectoryInfo _path;
		/// <summary>
		/// The contained plugin
		/// </summary>
		public readonly IPlugin plugin;

		public string Name => plugin.Name;

		/// <summary>
		/// A alphanumeric string which describes the functionality of the contained plugin / mod.
		/// </summary>
		public string Description => plugin.Description;

		/// <summary>
		/// A alphanumeric string which describes the version of the contained plugin / mod.
		/// </summary>
		public string Version => plugin.Version;

		/// <summary>
		/// a boolean variable which indicates that the contained mod is active or not
		/// </summary>
		public bool activeState {
			get => plugin.activeState;
			set => plugin.activeState = value;

		}

		protected PluginContainer(DirectoryInfo path, IPlugin plugin)
		{
			_path = path;
			this.plugin = plugin;
		}

		public bool loadLanguageFile()
		{
			Logger.Instance.info("Loading language file for: " + Name);
			FileInfo langFile = null;
			foreach ( var file in _path.EnumerateFiles("*.json") )
			{
				if ( file.Name == "lang.json" && file.Length != 0)
				{
					langFile = file;
				}
			}

			if ( langFile == null )
			{
				//No language file was found.
				Logger.Instance.info("No langugae file was found for: " + Name);
				return true;
			}

			var data = (Dictionary<string, Dictionary<string, string>>) JsonConvert.DeserializeObject(
				langFile.OpenText().ReadToEnd(), 
				typeof(Dictionary<string, Dictionary<string, string>>));
			
			var source = LocalizationManager.Sources[0];
			
			foreach ( var node in data )
			{
				foreach ( var translation in node.Value )
				{
					var id = source.GetLanguageIndex(translation.Key);
					var term = source.AddTerm(node.Key);
					term.SetTranslation(id, translation.Value);
				}
			}
			
			source.UpdateDictionary();

			return true;

		}

		public static PluginContainer Create(DirectoryInfo dir)
		{
			Logger.Instance.info("Creating plugin container for: " + dir.Name);

			// load all the assemblies from the directory
			ICollection<Assembly> assemblies = new List<Assembly>();
			foreach (FileInfo dllFile in dir.GetFiles("*.dll"))
			{
				Logger.Instance.info("Found dll: " + dllFile + " ...");
				AssemblyName an = AssemblyName.GetAssemblyName(dllFile.FullName);
				Assembly assembly = Assembly.Load(an);
				assemblies.Add(assembly);
			}
			
			// Check if the assembly is valid (we don't want to load any dll within the mods-directory
			Type pluginType = typeof(IPlugin);
			List<Type> validPlugins = new List<Type>();
			foreach (Assembly assembly in assemblies)
			{
				if (assembly != null)
				{
					Type[] types = assembly.GetTypes();
					foreach (Type type in types)
					{
						if (type.IsInterface || type.IsAbstract)
						{
							continue;
						}
						
						if 
						(
							type.GetInterface(pluginType.FullName) != null &&
							type.GetInterfaces().Contains(typeof(IPlugin))
						)
						{
							validPlugins.Add(type);
						}
					}
				}
			}

			if ( validPlugins.Count == 0 )
			{
				Logger.Instance.info("No valid plugin were found for this directory.");

				return InvalidPluginContainer.Instance;
			}

			if ( validPlugins.Count > 2 )
			{
				Logger.Instance.info("More than two plugins were found in this directory!");
				
				return InvalidPluginContainer.Instance;
			}
			
			//TODO: more erros?

			var plugin = (IPlugin) Activator.CreateInstance(validPlugins[0]);

			try
			{
				plugin.initialize();
				// TODO:  plugin.start(doorstepGameObject);
			}
			catch (Exception e)
			{
				Logger.Instance.info("An error happened initalizting a plugin!\n"+ e);

				return InvalidPluginContainer.Instance;
			}
			
			var container = new PluginContainer(dir, plugin);

			container.loadLanguageFile();

			return container;

		}
	}
	
		
	/// <summary>
	/// An invalid plugins container;
	/// </summary>
	public class InvalidPluginContainer : PluginContainer
	{

		public static readonly InvalidPluginContainer Instance = new InvalidPluginContainer();

		private InvalidPluginContainer() : base(null, null){}

	}

}