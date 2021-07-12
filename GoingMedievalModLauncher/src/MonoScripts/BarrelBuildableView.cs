using System.Collections.Generic;
using NSMedieval.Construction;
using NSMedieval.Layers;
using NSMedieval.Model;
using UnityEngine;

namespace GoingMedievalModLauncher.MonoScripts
{
	public class BarrelBuildableView : BaseBuildableView<BuildingInstance>, IHideObject
	{


		public override InfoPanelData UpdateCallback()
		{
			return GetInfoPanelData();
		}

		protected override Bounds? GetObjectBounds()
		{
			Vector3 position = this.transform.position;
			return new Bounds?(new Bounds(new Vector3(position.x, position.y + (float) this.BuildableInstance.Size.y / 2f, position.z), (Vector3) this.BuildableInstance.Size));
		}

		private BuildingInstance _instance;

		protected override void Awake()
		{
			base.Awake();
			ClickCollider = gameObject.GetComponentInChildren<BoxCollider>();
			var renderer = gameObject.GetComponentInChildren<MeshRenderer>();
			BlueprintMeshRenderers = new List<MeshRenderer> { renderer };
			FoundationMeshRenderers = new List<MeshRenderer> { renderer };
			FinishedMeshRenderers = new List<MeshRenderer> { renderer };

		}

		public override BuildingInstance BuildableInstance => _instance;
		
		public override void Setup(BuildingInstance furnitureInstance)
		{
			this._instance = furnitureInstance;
			base.Setup(furnitureInstance);
		}

		public override void EnterPool()
		{
			base.EnterPool();
			SetPlaceableMaterial();
			this._instance = null;
		}

	}
}