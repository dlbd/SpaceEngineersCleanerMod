using System;
using System.IO;

using Sandbox.ModAPI;

namespace ServerCleaner.Updatables
{
	public class MessageFromFileShower : RepeatedAction
	{
		private string nextMessage;

		public MessageFromFileShower(double interval) : base(interval)
		{
		}

		protected override bool ShouldRun()
		{
			try
			{
				var fileName = string.Format("NextMessage_{0}.txt", Path.GetFileNameWithoutExtension(MyAPIGateway.Session.CurrentPath));

				if (MyAPIGateway.Utilities.FileExistsInLocalStorage(fileName, GetType()))
				{
					using (var reader = MyAPIGateway.Utilities.ReadFileInLocalStorage(fileName, GetType()))
					{
						nextMessage = reader.ReadToEnd();
					}
				}

				using (var writer = MyAPIGateway.Utilities.WriteFileInLocalStorage(fileName, GetType()))
				{
				}

				return !string.IsNullOrWhiteSpace(nextMessage);
			}
			catch (Exception ex)
			{
				Logger.WriteLine("Exception in MessageFromFileShower.ShouldRun(): {0}", ex);
				return false;
			}
		}

		protected override void Run()
		{
			if (string.IsNullOrWhiteSpace(nextMessage))
				return;

			Utilities.ShowMessageFromServerToEveryone(nextMessage);
			nextMessage = null;
		}
	}
}
