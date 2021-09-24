using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GoingMedievalModLauncher.Engine;
using GoingMedievalModLauncher.MonoScripts;
using GoingMedievalModLauncher.ui;
using I2.Loc;
using Newtonsoft.Json;
using NSEipix.Base;
using NSEipix.ObjectMapper;
using NSEipix.Repository;
using NSMedieval;
using NSMedieval.Construction;
using NSMedieval.Crops;
using NSMedieval.Model;
using NSMedieval.Model.MapNew;
using NSMedieval.Production;
using NSMedieval.Repository;
using NSMedieval.Research;
using UnityEngine;

namespace GoingMedievalModLauncher.plugins
{


	public enum ContainerState
	{

		NIL,
		ACTIVE,
		INACTIVE,
		INVALID_REQUIREMENT,
		INVALID_DEPENDENCY

	}

	public interface IPluginContainer
	{

		/// <summary>
		/// The plugin's code if needed.
		/// </summary>
		IPlugin plugin { get; }

		/// <summary>
		/// The name of the plugin / mod.
		/// </summary>
		string Name { get; }
		
		/// <summary>
		/// The id of plugin / mod.
		/// </summary>
		string ID { get; }

		/// <summary>
		/// A alphanumeric string which describes the functionality of the plugin / mod.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// A alphanumeric string which describes the version of the plugin / mod.
		/// </summary>
		string Version { get; }
		/// <summary>
		/// The required plugin of this plugin.
		/// Only ONE requirement is valid, and if the requirement is not found, then the plugin's first phase isn't executed
		/// </summary>
		string Requirement { get;  }
		
		/// <summary>
		/// The dependencies of the plugin
		/// This array SHOULD NOT contain the requirement. If the dependency is not loaded then the plugin's first phase isn't executed
		/// </summary>
		/// TODO: Change it to second phase if we need a second phase.
		string[] Dependencies { get; }

		/// <summary>
		/// a boolean variable which indicates that the mod is active or not
		/// </summary>
		bool ActiveState
		{
			get;
			set;

		}

		/// <summary>
		/// the state of the plugin. On loading it defaults to NIL.
		/// </summary>
		ContainerState State
		{
			get;
			set;
		}
		
		/// <summary>
		/// Indicates that the launcher should not search in the assets directory
		/// </summary>
		bool CodeOnly { get; }
		
		/// <summary>
		/// Indicates that the launcher should not search in the code directory
		/// </summary>
		bool NoCode { get; }

		void Init();

		void Deinit(MonoBehaviour o);

	}
	
	
	/// <summary>
	/// A class that contains a plugin.
	/// </summary>
	public class PluginContainer : IPluginContainer
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
		
		public IPlugin plugin { get; }
		
		public string Name { get; }
		
		public string ID { get; }
		
		public string Description { get; }
		
		public string Version { get; }
		
		public string Requirement { get;  }
		
		public string[] Dependencies { get; }
		
		public bool ActiveState
		{
			get => State == ContainerState.ACTIVE;
			set => State = value ? ContainerState.ACTIVE : ContainerState.INACTIVE;

		}
		
		public ContainerState State
		{
			get => _state;
			set
			{
				if ( _state != ContainerState.NIL && _state != ContainerState.ACTIVE
				                                  && _state != ContainerState.INACTIVE ) return;

				if ( _state == ContainerState.NIL || value == ContainerState.ACTIVE || value == ContainerState.INACTIVE )
				{
					_state = value;
				}
			}
		}

		/// <summary>
		/// The backing variable of the State property.
		/// </summary>
		private ContainerState _state;
		
		public bool CodeOnly { get; }
		public bool NoCode { get; }

		private event Action<MonoBehaviour> OnDeinit;
		private event Action OnInit;

		private PluginContainer(DirectoryInfo path, ManifestClass manifest)
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
				ID = manifest.id;
				Version = manifest.version;
				//optional data
				Name =  manifest.name ?? ID;
				Description = manifest.description;
				Requirement = manifest.requirement;
				Dependencies = manifest.dependencies;
				CodeOnly = manifest.codeOnly;
				NoCode = manifest.noCode;

			}

			_state = ContainerState.ACTIVE;
			
			if ( !CodeOnly )
			{
				RegisterJsonRepositoryLoader<MapRepository, Map>();
				RegisterJsonRepositoryLoader<MapSizeRepository, MapSize>();

				RegisterJsonRepositoryLoaderWithIdField<ResourceRepository, Resource>("resourceId");

				RegisterJsonRepositoryLoader<RoomTypeRepository, RoomType>();
				RegisterJsonRepositoryLoader<CropfieldRepository, Cropfield>();
				RegisterJsonRepositoryLoaderWithIdField<ProductionRepository, Production>("productionId");

				RegisterJsonRepositoryLoader<ResearchRepository, ResearchModel>();
			}

			if ( !NoCode && _code != null )
			{
				ICollection<Assembly> assemblies = new List<Assembly>();
				// load all the assemblies from the directory
				foreach ( FileInfo dllFile in _code.GetFiles("*.dll") )
				{
					Launcher.LOGGER.Info("Found dll: " + dllFile + " ...");
					AssemblyName an = AssemblyName.GetAssemblyName(dllFile.FullName);
					Assembly assembly = Assembly.Load(an);
					assemblies.Add(assembly);
				}
				
				// Check if the assembly is valid (we don't want to load any dll within the mods-directory)
				Type pluginType = typeof(IPlugin);
				List<Type> validPlugins = new List<Type>();
				
				try
				{
					foreach ( Assembly assembly in assemblies )
					{
						if ( assembly != null )
						{
							Type[] types = Type.EmptyTypes;
							try
							{
								types = assembly.GetTypes();
							}
							catch (ReflectionTypeLoadException e)
							{
								Launcher.LOGGER.Info("There were some class type that the launcher"
								                            + "could not load. Trying to find the class anyways.");
								if ( e.Types != null )
								{
									types = e.Types;
								}
								
							}
							foreach ( Type type in types )
							{

								if ( type == null || type.IsInterface || type.IsAbstract )
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
					Launcher.LOGGER.Error(e,
						"Unable to get Plugin's type. Maybe the code is referencing an older launcher?");
				}

				//TODO: more errors?
				if ( validPlugins.Count == 0 )
				{
					Launcher.LOGGER.Info("No valid plugin were found for this directory.");
					ActiveState = false;

				}
				else if ( validPlugins.Count > 2 )
				{
					Launcher.LOGGER.Info("More than two plugins were found in this directory!");
					ActiveState = false;
				}
				else
				{
					IPlugin instance = null;
					try
					{
						instance = (IPlugin) Activator.CreateInstance(validPlugins[0]);

						void I()
						{
							try
							{
								instance.initialize();
								// TODO:  plugin.start(doorstepGameObject);
							}
							catch (Exception e)
							{
								Launcher.LOGGER.Error(e,$"Unable to initalize plugin {Name}");
							}
						}

						void D(MonoBehaviour o)
						{
							try
							{
								instance.disable(o);
								// TODO:  plugin.stop(doorstepGameObject);
							}
							catch (Exception e)
							{
								Launcher.LOGGER.Error(e,$"Unable to disbale plugin {Name}");
							}
						}

						OnInit += I;
						OnDeinit += D;
					}
					catch (Exception e)
					{
						Launcher.LOGGER.Error(e,"An error happened instantiating a plugin!\n");
					}
					finally
					{
						plugin = instance;
					}
				}
			}
			else
			{
				plugin = null;
			}

		}

		~PluginContainer()
		{
			OnDeinit = null;
			OnInit = null;
		}

		private bool LoadLanguageFile()
		{
			
			Launcher.LOGGER.Info("Loading language file for: " + Name);

			if ( _assest == null )
			{
				Launcher.LOGGER.Info("Unable to load language file from nonexistent directory!");
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
				Launcher.LOGGER.Info("No langugae file was found for: " + Name);
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
		private bool LoadJsonRepositoryWithIdField<T, M>( T repo, FileInfo json, out RepositoryDto<M> dto, string
		 idFieldName) 
			where T : JsonRepository<T, M> 
			where M: Model
		{

			//set up, so if error happens, null is returned.
			dto = null;
			
			//If the file does not exist, log it, and return false
			if ( json == null || !json.Exists )
			{
				Launcher.LOGGER.Info("The file was null, or empty so the repository can ot be loaded.");
				return false;
			}

			var desInfo = typeof(T).GetMethod(
				"GetSerializer", BindingFlags
					.Instance | BindingFlags.NonPublic);
			if ( desInfo == null )
			{
				Launcher.LOGGER.Info("Unable to get the serializer method. How did this happened?");
				return false;
			}
			// With RepositoryDto we can easily deserialize
			var deserializer = 
				desInfo.Invoke(repo, new object[]{json.FullName}) as ISerializer<RepositoryDto<M>>;
			if ( deserializer == null )
			{
				Launcher.LOGGER.Info("The serializer returned was null. This should happen.");
				return false;
			}
			dto = deserializer.Deserialize();
			var id = typeof(M).GetField(idFieldName, BindingFlags.NonPublic | BindingFlags.Instance);

			if ( id == null )
			{
				Launcher.LOGGER.Info("The id of the model class does not exist.");
				return false;
			}

			if ( dto == null )
			{
				Launcher.LOGGER.Info("Can not get the dto. Is the json valid? Can be deserialized?");

				return false;
			}
			
			foreach ( var type in dto.Repository )
			{
				//Add the mod id to the begining of the normal id
				id.SetValue(type, $"#{ID}:{id.GetValue(type)}");
			}
			return true;
		}

		private void RegisterJsonRepositoryLoader<T, M>()
			where T : JsonRepository<T, M>
			where M : Model
			=> RegisterJsonRepositoryLoaderWithIdField<T, M>("id");

		private void RegisterJsonRepositoryLoaderWithIdField<T, M>(string fieldName)
			where T : JsonRepository<T, M>
			where M : Model
		{
			Launcher.LOGGER.Info(
				"Adding register event for: <" + typeof(T).Name + ", " + typeof(M).Name + "> in mod: " + ID);

			//If the assets directory is not exist, we won't be able to find the file
			if ( _assest == null )
			{
				Launcher.LOGGER.Info("Assets directory was null on loading repository: <" + typeof(T).Name + ", " + typeof(M).Name + "> in mod: " + ID);

				return;
			}
			
			var jsonFile = typeof(T).GetMethod("JsonFile", BindingFlags.NonPublic | BindingFlags.Instance);

			if ( jsonFile == null )
			{
				Launcher.LOGGER.Info("Unable to get the protected method \"JsonFile\".");

				return;
			}

			RepositoryDto<M> dto = null;
			JsonRepository<T, M> rep = null;
			

				void Del(T repo)
				{

					rep = repo;

					Launcher.LOGGER.Info(
						"registry event fired for: <" + typeof(T).Name + ", " + typeof(M).Name + "> in mod: " + ID);

					//Get the overriden JsonFile method, that return the relative path to the json file

					if ( LoadJsonRepositoryWithIdField<T, M>(
						repo, new FileInfo(
							Path.Combine(
								_assest.FullName, jsonFile
									.Invoke(repo, null) as string)), out dto, fieldName) )
					{
						foreach ( var item in dto.Repository )
						{
							Launcher.LOGGER.Info($"Added item {item.GetID()} to repository {typeof(T).Name}");
							repo.Add(item);
						}
						
						//DO NOT! RECURSION ALERT!
						//repo.Reload();
					}

				}

				void Rem(MonoBehaviour o)
				{
					if ( rep == null || dto == null ) return;

					foreach ( var item in dto.Repository )
					{
						rep.RemoveByID(item.GetID());
					}
					rep.Reload();
				}

				void I()
				{
					RepositoryPatch<T, M>.PostDeserialization += Del;
				}

				void D(MonoBehaviour o)
				{
					RepositoryPatch<T, M>.PostDeserialization -= Del;
				}

				OnInit += I;
				OnDeinit += D;
				OnDeinit += Rem;

		}

		private void LoadAssetsBundles()
		{
			if(_assest == null)
				return;

			DirectoryInfo bundles = new DirectoryInfo(Path.Combine(_assest.FullName, "Bundles"));

			foreach ( var fileInfo in bundles.EnumerateFiles() )
			{
				if(fileInfo.Name.Contains("manifest"))
					continue;
				
				var bundle = AssetBundle.LoadFromFile(fileInfo.FullName);
				if(bundle == null)
					continue;
				
				Launcher.LOGGER.Info($"Bundle: {bundle.name}");

				bundle.LoadAllAssets();

				var assets = bundle.LoadAllAssets<GameObject>();

				foreach ( var mapper in assets )
				{
					PrefabRepositoryPatch.OnStart += delegate(PrefabRepository repository)
					{
						mapper.AddComponent<BarrelBuildableView>();
						repository.Add(new KeyGameObjectPair($"{ID}:{mapper.name}", mapper));
						repository.Add(new KeyGameObjectPair($"{ID}:{mapper.name}_preview", mapper));
						Launcher.LOGGER.Info($"#{ID}:{mapper.name}");
					};
				}
				
			}
		}

		public virtual void Init()
		{
			if ( CodeOnly ) return;

			if ( !LoadLanguageFile() )
			{
				Launcher.LOGGER.Info("An error occured while loading the language file.");
			}

			//LoadAssetsBundles();

			OnInit?.Invoke();
		}

		public void Deinit(MonoBehaviour o)
		{
			OnDeinit?.Invoke(o);
		}

		public static IPluginContainer Create(DirectoryInfo dir)
		{
			Launcher.LOGGER.Info("Creating plugin container for: " + dir.Name);
			
			//Get the manifest.json file(s).
			var manifestf = dir.GetFiles("manifest.json");

			if ( manifestf.Length <= 0 )
			{
				Launcher.LOGGER.Info("Unable to find manifest file for: "+dir.Name);
				return InvalidPluginContainer.Instance;
			}
			
			//Load up the manifest

			var manifest = JsonConvert.DeserializeObject<ManifestClass>(
				manifestf[0].OpenText().ReadToEnd());

			var m = JsonConvert.DeserializeObject(manifestf[0].OpenText().ReadToEnd());

			//Now, searching for the necessary fields in the manifest and colliding fields.
			if ( manifest.id == null || manifest.version == null  || manifest.codeOnly && manifest.noCode)
			{
				return InvalidPluginContainer.Instance;
			}
			
			//created the container, that will be returned.
			var container = new PluginContainer(dir, manifest);

			return container;


		}
	}
	
		
	/// <summary>
	/// An invalid plugins container;
	/// </summary>
	public sealed class InvalidPluginContainer : Singleton<InvalidPluginContainer>, IPluginContainer
	{

		public IPlugin plugin => null;
		public string Name => "INVALID";
		public string ID => "INVALID";
		public string Description=> "INVALID";
		public string Version => "INVALID";
		public string Requirement => "INVALID";
		public string[] Dependencies => null;
		public bool ActiveState { get => false; set{} }
		public ContainerState State { get=>ContainerState.INACTIVE; set{} }
		public bool CodeOnly => false;
		public bool NoCode => true;

		public void Init()
		{
			throw new NotImplementedException();
		}

		public void Deinit(MonoBehaviour o)
		{
			throw new NotImplementedException();
		}

		private InvalidPluginContainer(){}

	}

	/// <summary>
	/// The modloader's container that other mods/plugins can reference.
	/// </summary>
	public sealed class ModLoaderPluginContainer : Singleton<ModLoaderPluginContainer>, IPluginContainer
	{

		public string Version => "v0.3.5";
		public string Requirement => "";
		public string[] Dependencies => null;
		public bool ActiveState { get=> true; set{} }
		public ContainerState State { get=> ContainerState.ACTIVE; set{} }

		public string ID => "modloader";
		public string Description => "The core of the modding.";
		public IPlugin plugin => null;
		public string Name => "Mod Loader";

		public bool CodeOnly => true;

		public bool NoCode => false;

		public void Init()
		{
		}

		public void Deinit(MonoBehaviour o)
		{
			
		}

		private ModLoaderPluginContainer()
		{
			MainMenuPatch.OnMenuStartPost += () =>
			{
				MainMenuPatch.AddMainMenuButton("ModsButton", "Mods", () =>
				{
					if(EngineLauncher.Instance.modManagerWindow == null) return;

					EngineLauncher.Instance.modManagerWindow.shown = !EngineLauncher.Instance.modManagerWindow.shown;
					
				}, 15);
			};
		}

	}

}