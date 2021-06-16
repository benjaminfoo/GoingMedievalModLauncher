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
using NSMedieval.Production;
using NSMedieval.Research;

namespace GoingMedievalModLauncher.plugins
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
		public bool ActiveState { get; set; }

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
				ActiveState = true;
			}

			if ( _code != null )
			{
				ICollection<Assembly> assemblies = new List<Assembly>();
				// load all the assemblies from the directory
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
				
				try
				{
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
				}
				catch (Exception e)
				{
					Logger.Instance.info("Unable to get Plugin's type. Maybe the ode is referencing an older launcher?");
					Logger.Instance.info(e.ToString());
					
				}

				//TODO: more erros?
				if ( validPlugins.Count == 0 )
				{
					Logger.Instance.info("No valid plugin were found for this directory.");
					ActiveState = false;

				}
				else if ( validPlugins.Count > 2 )
				{
					Logger.Instance.info("More than two plugins were found in this directory!");
					ActiveState = false;
				}
				else
				{
					var instance = (IPlugin) Activator.CreateInstance(validPlugins[0]);

					try
					{
						instance.initialize();
						// TODO:  plugin.start(doorstepGameObject);
					}
					catch (Exception e)
					{
						Logger.Instance.info("An error happened initalizting a plugin!\n" + e);
					}

					plugin = instance;
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
		private bool LoadJsonRepository<T, M>( T repo, FileInfo json, out RepositoryDto<M> dto) 
			where T : JsonRepository<T, M> 
		where M: Model
		{

			//set up, so if error happens, null is returned.
			dto = null;
			
			//If the file does not exist, log it, and return false
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
			// With RepositoryDto we can easily deserialize
			var deserializer = 
				desInfo.Invoke(repo, new object[]{json.FullName}) as ISerializer<RepositoryDto<M>>;
			if ( deserializer == null )
			{
				Logger.Instance.info("The serializer returned was null. This should happen.");
				return false;
			}
			dto = deserializer.Deserialize();
			var id = typeof(M).GetField("id", BindingFlags.NonPublic | BindingFlags.Instance);

			if ( id == null )
			{
				Logger.Instance.info("The id of the model class does not exist.");
				return false;
			}

			if ( dto == null )
			{
				Logger.Instance.info("Can not get the dto. Is the json valid? Can be deserialized?");

				return false;
			}
			
			foreach ( var type in dto.Repository )
			{
				//Add the mod id to the begining of the normal id
				id.SetValue(type, $"#{ID}:{id.GetValue(type)}");
			}
			return true;
		}

		private void RegisterJsonRepositoryLoader<T, M>(string relativePath)
			where T : JsonRepository<T, M>
			where M : Model
		{
			Logger.Instance.info(
				"Adding register event for: <" + typeof(T).Name + ", " + typeof(M).Name + "> in mod: " + ID);
			RepositoryPatch<T, M>.PostDeserialization +=
				delegate(T repo)
				{

					//If the assets directory is not exist, we won't be able to find the file
					if ( _assest == null )
					{
						Logger.Instance.info(
							"Assets directory was null on loading repository: <"
							+ typeof(T).Name + ", " + typeof(M).Name + "> in mod: " + ID);
						return;
					}

					Logger.Instance.info(
						"registry event fired for: <" + typeof(T).Name + ", " + typeof(M).Name + "> in mod: " + ID);

					if ( LoadJsonRepository<T, M>(repo,
						new FileInfo(Path.Combine(
								_assest.FullName ,relativePath)),
						out var rooms) )
					{
						foreach ( var item in rooms.Repository )
						{
							repo.Add(item);
						}
					}

				};
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
			
			//created the container, that will be returned.
			var container = new PluginContainer(dir, manifest);

			if ( !container.LoadLanguageFile() )
			{
				Logger.Instance.info("An error occured while loading the language file.");
			}

			container.RegisterJsonRepositoryLoader<RoomTypeRepository, RoomType>("Data/RoomTypes.json");

			container.RegisterJsonRepositoryLoader<ResearchRepository, ResearchModel>("Research/Research.json");

			container.RegisterJsonRepositoryLoader<CropfieldRepository, Cropfield>("CropFields/CropFields.json");

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
			ActiveState = false;
		}

	}

}