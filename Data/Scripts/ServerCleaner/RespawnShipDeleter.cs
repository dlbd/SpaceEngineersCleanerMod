using System.Collections.Generic;
using System.Linq;

using Sandbox.ModAPI;
using VRage.Game.ModAPI;

namespace ServerCleaner
{
	public class RespawnShipDeleter : RepeatedDeleter<IMyCubeGrid, ComplexCubeGridDeletionContext>
	{
		public static string[] RespawnShipNames = { "Atmospheric Lander mk.1", "RespawnShip", "RespawnShip2" };

		public RespawnShipDeleter(double interval, double playerDistanceThresholdForWarning, double playerDistanceThresholdForDeletion) : base(interval, new ComplexCubeGridDeletionContext()
		{
			PlayerDistanceThreshold = playerDistanceThresholdForWarning,
			PlayerDistanceThresholdForActualDeletion = playerDistanceThresholdForDeletion
		})
		{
		}

		protected override bool BeforeDelete(IMyCubeGrid entity, ComplexCubeGridDeletionContext context)
		{
			// Is it a respawn ship?

			if (!RespawnShipNames.Contains(entity.DisplayName))
				return false;

			// Are any of the owners online?

			var nameString = string.Format("{0} (owned by {1})", entity.DisplayName, Utilities.GetOwnerNameString(entity, context.PlayerIdentities));

			foreach (var ownerID in entity.SmallOwners)
			{
				if (!context.OnlinePlayerIds.Contains(ownerID))
					continue;

				// At least one owner is online, warn him

				context.NameStringsForLaterDeletion.Add(nameString);
				return false;
			}

			// Are any other players nearby?

			if (context.PlayerDistanceThresholdForActualDeletion > 0 && Utilities.AnyWithinDistance(entity.GetPosition(), context.PlayerPositions, context.PlayerDistanceThresholdForActualDeletion))
				return false;

			context.NameStringsForDeletion.Add(nameString);
			return true;
		}

		protected override void AfterDeletion(ComplexCubeGridDeletionContext context)
		{
			if (context.EntitiesForDeletion.Count > 0)
			{
				Utilities.ShowMessageFromServer("Deleted {0} respawn ship(s) that had no owner online and no players within {1} m: {2}.",
					context.EntitiesForDeletion.Count, context.PlayerDistanceThresholdForActualDeletion, string.Join(", ", context.NameStringsForLaterDeletion));
			}

			if (context.NameStringsForLaterDeletion.Count > 0)
			{
				Utilities.ShowMessageFromServer("I'm going to delete the following respawn ship(s) later unless they are renamed: {0}",
					string.Join(", ", context.NameStringsForLaterDeletion));
			}
		}
	}
}
