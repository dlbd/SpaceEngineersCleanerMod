using System;
using System.Collections.Generic;

using Sandbox.ModAPI;
using VRage.ModAPI;
using VRage.Game.ModAPI;
using VRageMath;

namespace SpaceEngineersCleanerMod
{
	/// <summary>
	/// Deleter of cubegrids that have few blocks and no owners.
	/// </summary>
	public class TrashRemover : RepeatedAction
	{
		private const int BlockCountThreshold = 50;
		private const int PlayerDistanceThreshold = 500;

		public TrashRemover(ITimerFactory timerFactory) : base(timerFactory, 10 * 1000) // should run like every 10 minutes
		{
		}

		public override void Run()
		{
			var entities = new HashSet<IMyEntity>();
			MyAPIGateway.Entities.GetEntities(entities, e => e is IMyCubeGrid);

			var players = new List<IMyPlayer>();
			MyAPIGateway.Players.GetPlayers(players, p => p != null);

			var playerPositions = new List<Vector3D>();
			foreach (var player in players)
				playerPositions.Add(player.GetPosition());

			var slimBlocks = new List<IMySlimBlock>();
			var entitiesToDelete = new List<IMyEntity>();

			foreach (var entity in entities)
			{
				var cubeGrid = entity as IMyCubeGrid;

				if (cubeGrid.SmallOwners.Count > 0)
					continue;

				slimBlocks.Clear();
				cubeGrid.GetBlocks(slimBlocks);

				if (slimBlocks.Count > BlockCountThreshold)
					continue;

				if (Utilities.AnyWithinDistance(cubeGrid.GetPosition(), playerPositions, PlayerDistanceThreshold))
					continue;

				entitiesToDelete.Add(entity);
			}

			if (entitiesToDelete.Count == 0)
				return;

			var syncObjectWasNull = false;
			var deletedEntityNames = new List<string>();

			foreach (var entity in entitiesToDelete)
			{
				if (entity.SyncObject == null)
				{
					syncObjectWasNull = true;
					entity.Delete();
				}
				else
				{
					entity.SyncObject.SendCloseRequest();
				}

				deletedEntityNames.Add(entity.DisplayName);
			}

			Utilities.ShowMessageFromServer("Removed {0} grid(s) that had fewer than {1} blocks, no owner and no players within {2} m: {3}.",
				entitiesToDelete.Count, BlockCountThreshold, PlayerDistanceThreshold, string.Join(", ", deletedEntityNames));

			if (syncObjectWasNull)
				Utilities.ShowMessageFromServer("Also, SyncObject = null on at least one grid.");
		}
	}
}
