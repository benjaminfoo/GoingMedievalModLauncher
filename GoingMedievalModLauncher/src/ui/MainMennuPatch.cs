namespace GoingMedievalModLauncher.ui
{
	public class MainMenuPatch
	{
		
		public delegate void onMenuStartPost();

		public static event onMenuStartPost OnMenuStartPost = () => {};
		
		public static void Start()
		{
			Logger.Instance.info("Start patch was executed.");

			OnMenuStartPost();

		}

	}
}