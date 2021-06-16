using System.Collections;
using System.Threading.Tasks;
using NSMedieval.UI;

namespace GoingMedievalModLauncher.ui
{
	public static class PromptQueue
	{

		private static Queue q = new Queue();

		public static void ShowPrompt(PromptPanelData data, bool handleInput = true)
		{
			q.Enqueue(new Data()
			{
				data = data,
				handleInput = handleInput
			});
			
			if(q.Count > 1 ) return;

			_loop();
			
		}

		private static async void _loop()
		{
			while ( q.Count != 0 )
			{
				do
				{
					await Task.Delay(100);
				} while ( UIController.Instance.IsPromptShown );

				Data d = (Data) q.Dequeue();
				UIController.Instance.ShowPrompt(d.data, d.handleInput);
			}
		}


		private class Data
		{

			public PromptPanelData data;
			public bool handleInput;

		}
		
	}
}