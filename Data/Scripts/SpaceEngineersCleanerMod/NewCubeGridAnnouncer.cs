using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sandbox.ModAPI;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.ModAPI;

namespace SpaceEngineersCleanerMod
{
	[MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]
	public class NewCubeGridAnnouncer : MySessionComponentBase
	{
		// TODO: expand this to delete cargo ships (esp. Argentavis) that enter gravity wells

		public const int AnnounceEveryTicks = 500;

		private bool initialized, unloaded, registeredEntityAddHandler;

		private readonly List<string> cubeGridNamesToAnnounce = new List<string>();
		private int ticks;
		
		public override void UpdateBeforeSimulation()
		{
			base.UpdateBeforeSimulation();

			if (unloaded || !Utilities.IsGameRunning())
				return;

			if (!initialized)
			{
				Initialize();
				initialized = true;
			}

			ticks++;

			if (cubeGridNamesToAnnounce.Count > 0 && ticks % AnnounceEveryTicks == 0)
			{
				Utilities.ShowMessageFromServer("New grid(s) appeared: {0}.", string.Join(", ", cubeGridNamesToAnnounce));
				cubeGridNamesToAnnounce.Clear();
				ticks = 0;
			}
		}

		private void Initialize()
		{
			if (!MyAPIGateway.Multiplayer.MultiplayerActive || MyAPIGateway.Multiplayer.IsServer)
			{
				MyAPIGateway.Entities.OnEntityAdd += Entities_OnEntityAdd;
				registeredEntityAddHandler = true;
			}
		}

		protected override void UnloadData()
		{
			if (!unloaded)
			{
				if (registeredEntityAddHandler)
					MyAPIGateway.Entities.OnEntityAdd -= Entities_OnEntityAdd;

				unloaded = true;
			}

			base.UnloadData();
		}

		private void Entities_OnEntityAdd(IMyEntity entity)
		{
			var cubeGrid = entity as IMyCubeGrid;

			if (cubeGrid == null)
				return;

			if (cubeGrid.Physics == null)
				return; // projection/block placement indicator?

			cubeGridNamesToAnnounce.Add(cubeGrid.DisplayName);
		}
	}
}
