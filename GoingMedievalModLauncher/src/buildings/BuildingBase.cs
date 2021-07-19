using NSMedieval.Construction;
using NSMedieval.Types;

namespace GoingMedievalModLauncher.buildings
{

	public static class BuildingCategories
	{

		public const BuildingCategoryUI ModCategory = (BuildingCategoryUI) (-1);
		public const BuildingSubCategoryUI ModSubCategory = (BuildingSubCategoryUI)(-1);

	}

	public class BuildingBase : Building
	{

		public new BuildingCategoryUI BuildingCategoryUI => BuildingCategories.ModCategory;
		public new BuildingSubCategoryUI BuildingSubCategoryUI => BuildingCategories.ModSubCategory;

	}
}