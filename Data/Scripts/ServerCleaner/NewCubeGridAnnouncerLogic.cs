using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sandbox.ModAPI;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.ModAPI;

namespace ServerCleaner
{
	[MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)] // Cargo ships are spawned in BeforeSimulation. Perhaps, by the time AfterSimulation runs, they will be fully spawned...
	public class NewCubeGridAnnouncerLogic : MySessionComponentBase
	{
		// TODO: expand this to delete cargo ships (esp. Argentavis) that enter gravity wells

		public const int CheckAndAnnounceEveryTicks = 1000;

		private bool initialized, unloaded, registeredEntityAddHandler;

		private List<string> cubeGridNamesToAnnounce = new List<string>();
		private List<IMyCubeGrid> cubeGridsToCheck = new List<IMyCubeGrid>();
		private int ticks;
		
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

			ticks++;

			if (ticks % CheckAndAnnounceEveryTicks == 0)
			{
				if (cubeGridsToCheck.Count > 0)
				{
					foreach (var cubeGrid in cubeGridsToCheck)
						cubeGridNamesToAnnounce.Add(GetCubeGridNameToAnnounce(cubeGrid));

					cubeGridsToCheck.Clear();
				}

				if (cubeGridNamesToAnnounce.Count > 0)
				{
					Utilities.ShowMessageFromServerToEveryone("New grid(s) appeared: {0}.", string.Join(", ", cubeGridNamesToAnnounce));
					cubeGridNamesToAnnounce.Clear();
				}

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
			try
			{
				var cubeGrid = entity as IMyCubeGrid;

				if (cubeGrid == null)
					return;

				if (cubeGrid.Physics == null)
					return; // projection/block placement indicator?

				cubeGridsToCheck.Add(cubeGrid);
			}
			catch (Exception ex)
			{
				Logger.WriteLine("Exception in Entities_OnEntityAdd(): {0}", ex);
			}
		}

		private string GetCubeGridNameToAnnounce(IMyCubeGrid cubeGrid)
		{
			try
			{
				var owners = cubeGrid.SmallOwners;

				var identities = new List<IMyIdentity>();
				MyAPIGateway.Players.GetAllIdentites(identities, identity => identity != null && owners.Contains(identity.PlayerId));

				// Is there an easier way to detect non-human players?

				var notHuman = identities.Count > 0 && identities
					.Select(identity => MyAPIGateway.Session.Factions.TryGetPlayerFaction(identity.PlayerId))
					.All(faction => faction != null && !faction.AcceptHumans);

				var nameString = Utilities.GetOwnerNameString(owners, identities);
				return string.Format("{0} (owned by {1}{2})", cubeGrid.DisplayName, nameString,
					notHuman ? " - is that a drone/cargo ship?" : "");
			}
			catch (Exception ex)
			{
				Logger.WriteLine("Exception in GetCubeGridNameToAnnounce", ex);
				return "a thing full of errors :/";
			}
		}
	}
}
