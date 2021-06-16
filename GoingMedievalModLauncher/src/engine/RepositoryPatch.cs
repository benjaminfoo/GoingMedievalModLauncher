using NSEipix.Repository;

namespace GoingMedievalModLauncher.Engine
{
	public static class RepositoryPatch<T, M> where T : Repository<T, M> where M : NSEipix.Base.Model
	{

		public delegate void onAction(T repo);
		
		public static event onAction PostDeserialization;

		private static bool IsEventNull => PostDeserialization == null;

		public static void CallEvent(T repo)
		{
			if ( !IsEventNull )
			{
				PostDeserialization(repo);
			}
		}
		
	}
	
}