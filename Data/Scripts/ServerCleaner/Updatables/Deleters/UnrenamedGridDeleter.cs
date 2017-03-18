using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Sandbox.ModAPI;
using VRage.Game.ModAPI;

namespace ServerCleaner.Updatables.Deleters
{
	/// <summary>
	/// Deleter of grids that have default names. Does not delete the ships matched by the RespawnShipDeleter class.
	/// </summary>
	public class UnrenamedGridDeleter : RepeatedDeleter<IMyCubeGrid, ComplexCubeGridDeletionContext>
	{
		public static Regex[] DefaultNameRegexes = new[]
		{
			new Regex("^Small Grid [0-9]+$"),
			new Regex("^Small Ship [0-9]+$"),
			new Regex("^Large Grid [0-9]+$"),
			new Regex("^Large Ship [0-9]+$"),
			new Regex("^Static Grid [0-9]+$"),
			new Regex("^Platform [0-9]+$")
		};

		private List<string> vipNames;
		private bool warnOnly;

		public UnrenamedGridDeleter(double interval, double playerDistanceThresholdForWarning, double playerDistanceThresholdForDeletion, bool warnOnly, bool messageAdminsOnly, List<string> vipNames)
			: base(interval, messageAdminsOnly, new ComplexCubeGridDeletionContext()
		{
			PlayerDistanceThreshold = playerDistanceThresholdForWarning,
			PlayerDistanceThresholdForActualDeletion = playerDistanceThresholdForDeletion
		})
		{
			this.warnOnly = warnOnly;
			this.vipNames = vipNames;
		}

		protected override bool BeforeDelete(IMyCubeGrid entity, ComplexCubeGridDeletionContext context)
		{
			// Is the grid unrenamed?

			if (!IsNameDefault(entity.DisplayName))
				return false;

			// Is there a beacon or an antenna? Merge blocks reset ship names, renaming can get quite tedious

			context.CurrentEntitySlimBlocks.Clear();
			entity.GetBlocksIncludingFromStaticallyAttachedCubeGrids(context.CurrentEntitySlimBlocks);

			if (context.CurrentEntitySlimBlocks.Any(slimBlock => slimBlock.FatBlock != null && (slimBlock.FatBlock is IMyRadioAntenna || slimBlock.FatBlock is IMyBeacon)))
				return false;

			// Wheel stator=null workaround

			if (context.CurrentEntitySlimBlocks.IsAttachedWheelGrid())
				return false;

			// Are any of the owners online or VIP?

			var nameString = string.Format("{0} (owned by {1})", entity.DisplayName, Utilities.GetOwnerNameString(entity, context.PlayerIdentities));

			foreach (var ownerID in entity.SmallOwners)
			{
				if (context.PlayerIdentities.Any(identity => identity.IdentityId == ownerID && vipNames.Contains(identity.DisplayName)))
					return false;

				if (!context.OnlinePlayerIds.Contains(ownerID))
					continue;

				// At least one owner is online, warn him

				context.NameStringsForLaterDeletion.Add(nameString);
				return false;
			}

			// Are any other players nearby?

			if (context.PlayerDistanceThresholdForActualDeletion > 0 && Utilities.AnyWithinDistance(entity.GetPosition(), context.PlayerPositions, context.PlayerDistanceThresholdForActualDeletion))
				return false;

			if (warnOnly)
				return false;

			context.NameStringsForDeletion.Add(nameString);
			return true;
		}

		public static bool IsNameDefault(string name)
		{
			foreach (var regex in DefaultNameRegexes)
			{
				if (regex.IsMatch(name))
					return true;
			}

			return false;
		}

		protected override void AfterDeletion(ComplexCubeGridDeletionContext context)
		{
			if (context.EntitiesForDeletion.Count > 0)
			{
				ShowMessageFromServer("Deleted {0} unrenamed grid(s) with no antennas or beacons that had no owner online and no players within {1} m: {2}.",
					context.EntitiesForDeletion.Count, context.PlayerDistanceThresholdForActualDeletion, string.Join(", ", context.NameStringsForDeletion));
			}

			if (context.NameStringsForLaterDeletion.Count > 0)
			{
				ShowMessageFromServer("I'm going to delete the following unrenamed grid(s) later unless they are renamed or an antenna or a beacon is added: {0}",
					string.Join(", ", context.NameStringsForLaterDeletion));
			}
		}
	}
}
