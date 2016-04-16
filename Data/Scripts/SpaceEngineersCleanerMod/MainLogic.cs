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
		private TimerFactory timerFactory = new TimerFactory();

		private bool initialized, unloaded, registeredMessageHandler;
		private object[] services;

		public override void UpdateBeforeSimulation()
		{
			base.UpdateBeforeSimulation();
			
			if (!initialized && Utilities.IsGameRunning())
			{
				Initialize();
				initialized = true;
			}
		}

		private void Initialize()
		{
			if (!MyAPIGateway.Multiplayer.MultiplayerActive || MyAPIGateway.Multiplayer.IsServer)
			{
				services = new object[]
				{
					//new PlayerSpammer(timerFactory, 10 * 1000),
					new TrashRemover(timerFactory, 10 * 1000, 100), // change later: should run about every 10 minutes
					new FloatingObjectRemover(timerFactory, 10 * 1000, 100), // change later: should run about every 5 minutes
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
				timerFactory.CloseAllTimers();

				if (registeredMessageHandler)
					MyAPIGateway.Multiplayer.UnregisterMessageHandler(MessageIds.MessageFromServer, ShowMessageFromServer);

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
