using System.Collections.Generic;
using System.Linq;

using VRage.Game.ModAPI;
using VRage.ModAPI;

namespace SpaceEngineersCleanerMod
{
	public class RespawnShipDeleter : RepeatedDeleter<IMyCubeGrid, RespawnShipDeleter.RespawnShipDeletionContext>
	{
		public class RespawnShipDeletionContext : CubeGridDeletionContext
		{
			public List<long> OnlinePlayerIds = new List<long>();
		}

		public static readonly string[] RespawnShipNames = { "Atmospheric Lander mk.1", "RespawnShip", "RespawnShip2" };

		public RespawnShipDeleter(double interval, double playerDistanceTreshold) : base(interval, playerDistanceTreshold, new RespawnShipDeletionContext())
		{
		}

		protected override void PrepareDeletionContext(RespawnShipDeletionContext context)
		{
			base.PrepareDeletionContext(context);

			context.OnlinePlayerIds.Clear();

			foreach (var player in context.Players)
			{
				if (player.Client == null)
					continue;

				context.OnlinePlayerIds.Add(player.PlayerID);
			}
		}

		protected override bool BeforeDelete(IMyCubeGrid entity, RespawnShipDeletionContext context)
		{
			// Is it a respawn ship?

			if (!RespawnShipNames.Contains(entity.DisplayName))
				return false;

			// Is the owner online?

			foreach (var ownerID in entity.SmallOwners)
			{
				if (!context.OnlinePlayerIds.Contains(ownerID))
					continue;

				// The owner is online, warn him

				Utilities.ShowMessageFromServer("I'm going to delete {0} owned by {1} later unless it is renamed.",
					entity.DisplayName, GetOwnerNameString(entity.SmallOwners, context.Players));

				return false;
			}

			return true;
		}

		protected override void AfterDeletion(DeletionContext context)
		{
			var gridNamesWithOwners = context.EntitiesForDeletion
				.Select(entity => string.Format("{0} (owned by {1}", entity.DisplayName, GetOwnerNameString(entity, context.Players)));

			Utilities.ShowMessageFromServer("Deleted {0} respawn ship(s) that had no owner online and no players within {1} m: {2}.",
				context.EntitiesForDeletion.Count, PlayerDistanceThreshold, string.Join(", ", gridNamesWithOwners));
		}

		private static string GetOwnerNameString(IMyEntity entity, List<IMyPlayer> players)
		{
			var cubeGrid = entity as IMyCubeGrid;
			return cubeGrid == null ? "???" : GetOwnerNameString(cubeGrid.SmallOwners, players);
		}

		private static string GetOwnerNameString(List<long> ownerIds, List<IMyPlayer> players)
		{
			var result = string.Join(" & ", players
				.Where(player => ownerIds.Contains(player.PlayerID))
				.Select(player => player.DisplayName));

			return result.Length > 0 ? result : "noone";
		}
	}
}

