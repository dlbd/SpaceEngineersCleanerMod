using System.Linq;

using VRage.Game.ModAPI;

namespace ServerCleaner
{
	/// <summary>
	/// Deleter of cubegrids that have few blocks and no owners.
	/// </summary>
	public class TrashDeleter : RepeatedDeleter<IMyCubeGrid, CubeGridDeletionContext>
	{
		public TrashDeleter(double interval, double playerDistanceTreshold, int blockCountThreshold) : base(interval, playerDistanceTreshold, new CubeGridDeletionContext())
		{
			BlockCountThreshold = blockCountThreshold;
		}

		protected override bool BeforeDelete(IMyCubeGrid entity, CubeGridDeletionContext context)
		{
			if (entity.SmallOwners.Count > 0)
				return false;

			context.CurrentEntitySlimBlocks.Clear();
			entity.GetBlocks(context.CurrentEntitySlimBlocks);

			if (context.CurrentEntitySlimBlocks.Count > BlockCountThreshold)
				return false;

			if (context.CurrentEntitySlimBlocks.Any(slimBlock => Utilities.IsConnectableToOtherGrids(slimBlock)))
				return false;

			return true;
		}

		protected override void AfterDeletion(CubeGridDeletionContext context)
		{
			Utilities.ShowMessageFromServer("Deleted {0} grid(s) that had fewer than {1} blocks, no owner and no players within {2} m: {3}.",
				context.EntitiesForDeletion.Count, BlockCountThreshold, PlayerDistanceThreshold, string.Join(", ", context.EntitiesForDeletionNames));
		}

		public int BlockCountThreshold { get; private set; }
	}
}
