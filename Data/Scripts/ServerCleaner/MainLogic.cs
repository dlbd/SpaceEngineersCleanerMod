using System.Text;

using Sandbox.ModAPI;
using VRage.Game.Components;

namespace ServerCleaner
{
	[MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
	public class MainLogic : MySessionComponentBase
	{
		// TODO: configuration read from a file
		// TODO: something that deletes shot up pirate drones
		// TODO: start collecting player login times for future inactive player removal

		public const int FloatingObjectDeletion_Interval = 5 * 60 * 1000;
		public const int FloatingObjectDeletion_PlayerDistanceThreshold = 100;

		public const int TrashDeletion_Interval = 9 * 60 * 1000;
		public const int TrashDeletion_PlayerDistanceThreshold = 500;
		public const int TrashDeletion_BlockCountThreshold = 50;

		public const int RespawnShipDeletion_Interval = 11 * 60 * 1000;
		public const int RespawnShipDeletion_PlayerDistanceThresholdForWarning = 50;
		public const int RespawnShipDeletion_PlayerDistanceThresholdForDeletion = 1000;

		public const int UnrenamedShipDeletion_Interval = 13 * 60 * 1000;
		public const int UnrenamedShipDeletion_PlayerDistanceThresholdForWarning = 0;
		public const int UnrenamedShipDeletion_PlayerDistanceThresholdForDeletion = 1000;

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

		private void Initialize()
		{
			if (!MyAPIGateway.Multiplayer.MultiplayerActive || MyAPIGateway.Multiplayer.IsServer)
			{
				Logger.Initialize();

				repeatedActions = new RepeatedAction[]
				{
					new FloatingObjectDeleter(FloatingObjectDeletion_Interval, FloatingObjectDeletion_PlayerDistanceThreshold),
					new TrashDeleter(TrashDeletion_Interval, TrashDeletion_PlayerDistanceThreshold, TrashDeletion_BlockCountThreshold),
					new RespawnShipDeleter(RespawnShipDeletion_Interval, RespawnShipDeletion_PlayerDistanceThresholdForWarning, RespawnShipDeletion_PlayerDistanceThresholdForDeletion),
					new UnrenamedGridDeleter(UnrenamedShipDeletion_Interval, UnrenamedShipDeletion_PlayerDistanceThresholdForWarning, UnrenamedShipDeletion_PlayerDistanceThresholdForDeletion)
				};
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
