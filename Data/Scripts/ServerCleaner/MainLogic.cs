using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Sandbox.ModAPI;
using VRage.Game.Components;

namespace ServerCleaner
{
	[MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
	public class MainLogic : MySessionComponentBase
	{
		// TODO: something that deletes shot up pirate drones
		// TODO: start collecting player login times for future inactive player removal

		private bool initialized, unloaded, registeredMessageHandler;
		private RepeatedAction[] repeatedActions;

		public override void UpdateAfterSimulation()
		{
			base.UpdateAfterSimulation();

			if (unloaded || !Utilities.IsGameRunning())
				return;

			if (!initialized)
			{
				Initialize();
				initialized = true;
			}

			if (repeatedActions != null)
			{
				for (var actionIndex = 0; actionIndex < repeatedActions.Length; actionIndex++)
					repeatedActions[actionIndex].UpdateAfterSimulation();
			}
		}

		private Configuration GetConfiguration()
		{
			var config = new Configuration();

			try
			{
				var fileName = string.Format("Config_{0}.xml", Path.GetFileNameWithoutExtension(MyAPIGateway.Session.CurrentPath));

				if (MyAPIGateway.Utilities.FileExistsInLocalStorage(fileName, GetType()))
				{
					using (var reader = MyAPIGateway.Utilities.ReadFileInLocalStorage(fileName, GetType()))
					{
						config = MyAPIGateway.Utilities.SerializeFromXML<Configuration>(reader.ReadToEnd());
					}
				}

				using (var writer = MyAPIGateway.Utilities.WriteFileInLocalStorage(fileName, GetType()))
				{
					writer.Write(MyAPIGateway.Utilities.SerializeToXML(config));
				}
			}
			catch (Exception ex)
			{
				Logger.WriteLine("Exception in MainLogic.GetConfiguration(), using the default settings: {0}", ex);
			}

			return config;
		}

		private void Initialize()
		{
			if (!MyAPIGateway.Multiplayer.MultiplayerActive || MyAPIGateway.Multiplayer.IsServer)
			{
				Logger.Initialize();

				var config = GetConfiguration();

				var repeatedActions = new List<RepeatedAction>();

				if (config.FloatingObjectDeletion_Enabled)
					repeatedActions.Add(new FloatingObjectDeleter(config.FloatingObjectDeletion_Interval, config.FloatingObjectDeletion_PlayerDistanceThreshold));

				if (config.UnownedGridDeletion_Enabled)
					repeatedActions.Add(new UnownedGridDeleter(config.UnownedGridDeletion_Interval, config.UnownedGridDeletion_PlayerDistanceThreshold, config.UnownedGridDeletion_BlockCountThreshold));

				if (config.DamagedGridDeletion_Enabled)
					repeatedActions.Add(new DamagedGridDeleter(config.DamagedGridDeletion_Interval, config.DamagedGridDeletion_PlayerDistanceThreshold, config.DamagedGridDeletion_BlockCountThreshold));

				if (config.RespawnShipDeletion_Enabled)
					repeatedActions.Add(new RespawnShipDeleter(config.RespawnShipDeletion_Interval, config.RespawnShipDeletion_PlayerDistanceThresholdForWarning, config.RespawnShipDeletion_PlayerDistanceThresholdForDeletion));

				if (config.UnrenamedGridDeletion_Enabled)
					repeatedActions.Add(new UnrenamedGridDeleter(config.UnrenamedGridDeletion_Interval, config.UnrenamedGridDeletion_PlayerDistanceThresholdForWarning, config.UnrenamedGridDeletion_PlayerDistanceThresholdForDeletion));

				this.repeatedActions = repeatedActions.ToArray();
			}

			if (MyAPIGateway.Multiplayer.MultiplayerActive && !MyAPIGateway.Multiplayer.IsServer)
			{
				MyAPIGateway.Multiplayer.RegisterMessageHandler(MessageIds.MessageFromServer, ShowMessageFromServer);
				registeredMessageHandler = true;
			}
		}

		protected override void UnloadData()
		{
			if (!unloaded)
			{
				TimerFactory.CloseAllTimers();

				if (registeredMessageHandler)
					MyAPIGateway.Multiplayer.UnregisterMessageHandler(MessageIds.MessageFromServer, ShowMessageFromServer);

				Logger.Close();

				unloaded = true;
			}

			base.UnloadData();
		}

		private void ShowMessageFromServer(byte[] encodedMessage)
		{
			Utilities.ShowMessageFromServerOnClient(Encoding.Unicode.GetString(encodedMessage));
		}
	}
}
