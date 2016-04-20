using System.Collections.Generic;
using System.Linq;

using Sandbox.ModAPI;
using VRage.Game.ModAPI;

namespace ServerCleaner
{
	public class RespawnShipDeleter : RepeatedDeleter<IMyCubeGrid, RespawnShipDeleter.RespawnShipDeletionContext>
	{
		public class RespawnShipDeletionContext : CubeGridDeletionContext
		{
			public List<IMyIdentity> PlayerIdentities = new List<IMyIdentity>();
			public List<long> OnlinePlayerIds = new List<long>();

			public List<string> NameStringsForDeletion = new List<string>();
			public List<string> NameStringsForLaterDeletion = new List<string>();
		}

		public static readonly string[] RespawnShipNames = { "Atmospheric Lander mk.1", "RespawnShip", "RespawnShip2" };

		public RespawnShipDeleter(double interval, double playerDistanceTreshold) : base(interval, playerDistanceTreshold, new RespawnShipDeletionContext())
		{
		}

		protected override void PrepareDeletionContext(RespawnShipDeletionContext context)
		{
			base.PrepareDeletionContext(context);

			context.PlayerIdentities.Clear();
			MyAPIGateway.Players.GetAllIdentites(context.PlayerIdentities);

			context.OnlinePlayerIds.Clear();

			foreach (var player in context.Players)
			{
				if (player.Client == null)
					continue;

				context.OnlinePlayerIds.Add(player.PlayerID);
			}

			context.NameStringsForDeletion.Clear();
			context.NameStringsForLaterDeletion.Clear();
		}

		protected override bool BeforeDelete(IMyCubeGrid entity, RespawnShipDeletionContext context)
		{
			// Is it a respawn ship?

			if (!RespawnShipNames.Contains(entity.DisplayName))
				return false;

			// Is the owner online?

			var nameString = string.Format("{0} (owned by {1})", entity.DisplayName, Utilities.GetOwnerNameString(entity, context.PlayerIdentities));

			foreach (var ownerID in entity.SmallOwners)
			{
				if (!context.OnlinePlayerIds.Contains(ownerID))
					continue;

				// The owner is online, warn him

				context.NameStringsForLaterDeletion.Add(nameString);
				return false;
			}

			context.NameStringsForDeletion.Add(nameString);
			return true;
		}

		protected override void AfterDeletion(RespawnShipDeletionContext context)
		{
			if (context.EntitiesForDeletion.Count > 0)
			{
				Utilities.ShowMessageFromServer("Deleted {0} respawn ship(s) that had no owner online and no players within {1} m: {2}.",
					context.EntitiesForDeletion.Count, PlayerDistanceThreshold, string.Join(", ", context.NameStringsForLaterDeletion));
			}

			if (context.NameStringsForLaterDeletion.Count > 0)
			{
				Utilities.ShowMessageFromServer("I'm going to delete the following respawn ship(s) later unless they are renamed: {0}",
					string.Join(", ", context.NameStringsForLaterDeletion));
			}
		}
	}
}
