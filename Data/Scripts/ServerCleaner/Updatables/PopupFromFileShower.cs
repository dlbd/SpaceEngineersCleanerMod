using System;
using System.IO;

using Sandbox.ModAPI;

namespace ServerCleaner.Updatables
{
	public class PopupFromFileShower : RepeatedAction
	{
		private string nextTitle, nextSubtitle, nextText;

		public PopupFromFileShower(double interval) : base(interval)
		{
		}

		protected override bool ShouldRun()
		{
			try
			{
				var fileName = string.Format("NextPopup_{0}.txt", Path.GetFileNameWithoutExtension(MyAPIGateway.Session.CurrentPath));

				if (MyAPIGateway.Utilities.FileExistsInLocalStorage(fileName, GetType()))
				{
					using (var reader = MyAPIGateway.Utilities.ReadFileInLocalStorage(fileName, GetType()))
					{
						nextTitle = reader.ReadLine();
						nextSubtitle = nextTitle == null ? null : reader.ReadLine();
						nextText = nextSubtitle == null ? null : reader.ReadToEnd();
					}
				}

				using (var writer = MyAPIGateway.Utilities.WriteFileInLocalStorage(fileName, GetType()))
				{
				}

				return IsNextPopupAvailable;
			}
			catch (Exception ex)
			{
				Logger.WriteLine("Exception in MessageFromFileShower.ShouldRun(): {0}", ex);
				return false;
			}
		}

		protected override void Run()
		{
			if (!IsNextPopupAvailable)
				return;

			Utilities.ShowPopupFromServerToEveryone(nextTitle, nextSubtitle ?? "", nextText ?? "");

			nextTitle = null;
			nextSubtitle = null;
			nextText = null;
		}

		private bool IsNextPopupAvailable
		{
			get
			{
				return
					!string.IsNullOrWhiteSpace(nextTitle) ||
					!string.IsNullOrWhiteSpace(nextSubtitle) ||
					!string.IsNullOrWhiteSpace(nextText);
			}
		}
	}
}
