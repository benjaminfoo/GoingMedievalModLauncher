using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GoingMedievalModLauncher.Engine;
using I2.Loc;
using Newtonsoft.Json;
using NSEipix.Base;
using NSEipix.ObjectMapper;
using NSEipix.Repository;
using NSMedieval.Crops;
using NSMedieval.Enums;
using NSMedieval.Production;


namespace GoingMedievalModLauncher
{
	
	/// <summary>
	/// A class that contains a plugin.
	/// </summary>
	public class PluginContainer
	{

		/// <summary>
		/// The mod's folder.
		/// </summary>
		private readonly DirectoryInfo _path;

		/// <summary>
		/// The assets folder of the mod.
		/// </summary>
		private readonly DirectoryInfo _assest;
		
		/// <summary>
		/// The mod's code directory where the dlls are found.
		/// </summary>
		private readonly DirectoryInfo _code;
		
		/// <summary>
		/// The plugin's code if needed.
		/// </summary>
		public readonly IPlugin plugin;

		/// <summary>
		/// The name of the plugin / mod.
		/// </summary>
		public string Name { get; }
		
		/// <summary>
		/// The id of plugin / mod.
		/// </summary>
		public string ID { get; }

		/// <summary>
		/// A alphanumeric string which describes the functionality of the plugin / mod.
		/// </summary>
		public string Description { get; }

		/// <summary>
		/// A alphanumeric string which describes the version of the plugin / mod.
		/// </summary>
		public string Version { get; }

		/// <summary>
		/// a boolean variable which indicates that the mod is active or not
		/// </summary>
		public bool activeState { get; set; }

		protected PluginContainer(DirectoryInfo path, Dictionary<string, string> manifest)
		{
			if ( path != null )
			{

				_path = path;
				var dirs = _path.GetDirectories("assets"); // temp variable to hold the DirectoryInfo array

				if ( dirs.Length > 0 )
				{
					_assest = dirs[0];
				}

				dirs = _path.GetDirectories("code");
				if ( dirs.Length > 0 )
				{
					_code = dirs[0];
				}
			}

			//load up the data.
			if ( manifest != null )
			{
				//required data
				ID = manifest["id"];
				Version = manifest["version"];
				//optional data
				Name =  manifest.TryGetValue("name", out string name) ? name : ID;
				Description = manifest.TryGetValue("description", out string desc) ? desc: "";
				
				//For the invalid plugin
				activeState = true;
			}

			if ( _code != null )
			{
				// load all the assemblies from the directory
				ICollection<Assembly> assemblies = new List<Assembly>();
				foreach ( FileInfo dllFile in _code.GetFiles("*.dll") )
				{
					Logger.Instance.info("Found dll: " + dllFile + " ...");
					AssemblyName an = AssemblyName.GetAssemblyName(dllFile.FullName);
					Assembly assembly = Assembly.Load(an);
					assemblies.Add(assembly);
				}

				// Check if the assembly is valid (we don't want to load any dll within the mods-directory
				Type pluginType = typeof(IPlugin);
				List<Type> validPlugins = new List<Type>();
				foreach ( Assembly assembly in assemblies )
				{
					if ( assembly != null )
					{
						Type[] types = assembly.GetTypes();
						foreach ( Type type in types )
						{
							if ( type.IsInterface || type.IsAbstract )
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

				//TODO: more erros?
				if ( validPlugins.Count == 0 )
				{
					Logger.Instance.info("No valid plugin were found for this directory.");

				}
				else if ( validPlugins.Count > 2 )
				{
					Logger.Instance.info("More than two plugins were found in this directory!");
				}
				else
				{
					var plugin = (IPlugin) Activator.CreateInstance(validPlugins[0]);

					try
					{
						plugin.initialize();
						// TODO:  plugin.start(doorstepGameObject);
					}
					catch (Exception e)
					{
						Logger.Instance.info("An error happened initalizting a plugin!\n" + e);
					}

					this.plugin = plugin;
				}
			}
		}

		private bool LoadLanguageFile()
		{
			
			Logger.Instance.info("Loading language file for: " + Name);

			if ( _assest == null )
			{
				Logger.Instance.info("Unable to load language file from nonexistent directory!");
				return false;
			}
			
			FileInfo langFile = null;
			foreach ( var file in _assest.EnumerateFiles("*.json") )
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
					if ( id < 0 )
					{
						continue;
					}
					var term = source.AddTerm(ID + ":" + node.Key);
					term.SetTranslation(id, translation.Value);
				}
			}
			
			source.UpdateDictionary();

			return true;

		}

		//TODO: make the method universal.
		public bool LoadJsonRepositories<T, M>( T repo, FileInfo json, out RepositoryDto<M> dto) where T : 
		JsonRepository<T, M> 
		where
		 M: Model
		{

			dto = null;
			
			if ( json == null || !json.Exists )
			{
				Logger.Instance.info("The file was null, or empty so the repository can ot be loaded.");
				return false;
			}

			var desInfo = typeof(T).GetMethod(
				"GetSerializer", BindingFlags
					.Instance | BindingFlags.NonPublic);
			if ( desInfo == null )
			{
				Logger.Instance.info("Unable to get the serializer method. How did this happened?");
				return false;
			}
			var deserializer = 
				desInfo.Invoke(repo, new []{json.FullName}) as ISerializer<RepositoryDto<M>>;
			if ( deserializer == null )
			{
				Logger.Instance.info("The serializer returned was null. This should happen.");
				return false;
			}
			dto = deserializer.Deserialize();
			var id = typeof(RoomType).GetField("id", BindingFlags.NonPublic | BindingFlags.Instance);
			foreach ( var type in dto.Repository )
			{
				id.SetValue(type, ID+":"+id.GetValue(type));
			}
			return true;
		}

		public static PluginContainer Create(DirectoryInfo dir)
		{
			Logger.Instance.info("Creating plugin container for: " + dir.Name);
			
			//Get the manifest.json file(s).
			var manifestf = dir.GetFiles("manifest.json");

			if ( manifestf.Length <= 0 )
			{
				Logger.Instance.info("Unable to find manifest file for: "+dir.Name);
				return InvalidPluginContainer.Instance;
			}
			
			//Load up the manifest

			var manifest = JsonConvert.DeserializeObject<Dictionary<string, string>>(
				manifestf[0].OpenText().ReadToEnd());
			
			//Now, searching for the necessary fields in the manifest.
			if ( !manifest.ContainsKey("id") || !manifest.ContainsKey("version") )
			{
				return InvalidPluginContainer.Instance;
			}
			
			var container = new PluginContainer(dir, manifest);

			if ( !container.LoadLanguageFile() )
			{
				Logger.Instance.info("An error occured while loading the langugae file.");
			}
			
			RepositoryPatch<CropfieldRepository, Cropfield>.PostDeserialization += delegate(CropfieldRepository repo)
			{
				if ( container._assest == null )
				{
					return;
				}
				
				if ( container.LoadJsonRepositories<CropfieldRepository, Cropfield>(repo, new FileInfo(Path.Combine
					(container._assest.FullName, "Cropfields/Cropfields.json")) ,out var
					rooms) )
				{
					foreach ( var item in rooms.Repository )
					{
						repo.Add(item);
					}
				}
				
			};

			RepositoryPatch<RoomTypeRepository, RoomType>.PostDeserialization +=
				delegate(RoomTypeRepository repo)
				{

					if ( container._assest == null )
					{
						return;
					}

					if ( container.LoadJsonRepositories<RoomTypeRepository, RoomType>(repo, new FileInfo(Path.Combine
					(container._assest.FullName, "Data/RoomTypes.json")) ,out var
					 rooms) )
					{
						foreach ( var item in rooms.Repository )
						{
							repo.Add(item);
						}
					}

				};

			return container;


		}
	}
	
		
	/// <summary>
	/// An invalid plugins container;
	/// </summary>
	public class InvalidPluginContainer : PluginContainer
	{

		public static readonly InvalidPluginContainer Instance = new InvalidPluginContainer();

		private InvalidPluginContainer() : base(null, null)
		{
			//invalid plugins are not active
			activeState = false;
		}

	}

}