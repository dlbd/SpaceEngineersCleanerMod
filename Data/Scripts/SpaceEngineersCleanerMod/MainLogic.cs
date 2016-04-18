using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

using Sandbox.Common;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Definitions;
using Sandbox.Engine;
using Sandbox.Game;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Game.Components;
using VRage.ObjectBuilders;

namespace SpaceEngineersCleanerMod
{
	[MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]
	public class MainLogic : MySessionComponentBase
	{
		// TODO: configuration read from a file
		// TODO: another kind of trash detection (owned, but few blocks, no armor, thrusters or power, and the owner is offline)
		// TODO: separate distance thresholds for warning and deletion
		// TODO: something that deletes shot up pirate drones

		public const int FloatingObjectDeletion_Interval = 5 * 60 * 1000;
		public const int FloatingObjectDeletion_PlayerDistanceThreshold = 100;

		public const int TrashDeletion_Interval = 9 * 60 * 1000;
		public const int TrashDeletion_PlayerDistanceThreshold = 500;
		public const int TrashDeletion_BlockCountThreshold = 50;

		public const int RespawnShipDeletion_Interval = 11 * 60 * 1000;
		public const int RespawnShipDeletion_PlayerDistanceThreshold = 80; // not too far, so that players might see the message if they move away

		private bool initialized, unloaded, registeredMessageHandler;
		private object[] services;

		public override void UpdateBeforeSimulation()
		{
			base.UpdateBeforeSimulation();
			
			if (!initialized && !unloaded && Utilities.IsGameRunning())
			{
				Initialize();
				initialized = true;
			}
		}

		private void Initialize()
		{
			if (!MyAPIGateway.Multiplayer.MultiplayerActive || MyAPIGateway.Multiplayer.IsServer)
			{
				Logger.Initialize();

				services = new object[]
				{
					new FloatingObjectDeleter(FloatingObjectDeletion_Interval, FloatingObjectDeletion_PlayerDistanceThreshold),
					new TrashDeleter(TrashDeletion_Interval, TrashDeletion_PlayerDistanceThreshold, TrashDeletion_BlockCountThreshold),
					new RespawnShipDeleter(RespawnShipDeletion_Interval, RespawnShipDeletion_PlayerDistanceThreshold)
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

		private void ShowMessageFromServer(byte[] bytes)
		{
			Utilities.ShowMessageFromServerOnClient(Encoding.Unicode.GetString(bytes));
		}
	}
}
