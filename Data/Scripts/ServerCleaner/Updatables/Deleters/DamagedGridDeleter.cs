using System.Linq;

using VRage.Game.ModAPI;

namespace ServerCleaner.Updatables.Deleters
{
	/// <summary>
	/// Deleter of cubegrids that have few blocks and some of which are damaged.
	/// </summary>
	public class DamagedGridDeleter : RepeatedDeleter<IMyCubeGrid, CubeGridDeletionContext>
	{
		private int blockCountThreshold;

		public DamagedGridDeleter(double interval, double playerDistanceThreshold, int blockCountThreshold)
			: base(interval, new CubeGridDeletionContext() { PlayerDistanceThreshold = playerDistanceThreshold })
		{
			this.blockCountThreshold = blockCountThreshold;
		}

		protected override bool BeforeDelete(IMyCubeGrid entity, CubeGridDeletionContext context)
		{
			context.CurrentEntitySlimBlocks.Clear();
			entity.GetBlocks(context.CurrentEntitySlimBlocks);

			if (context.CurrentEntitySlimBlocks.Count > blockCountThreshold)
				return false;

			if (context.CurrentEntitySlimBlocks.Any(slimBlock => Utilities.IsConnectableToOtherGrids(slimBlock)))
				return false;

			return context.CurrentEntitySlimBlocks.Any(slimBlock => slimBlock.CurrentDamage > 0);
		}

		protected override void AfterDeletion(CubeGridDeletionContext context)
		{
			if (context.EntitiesForDeletion.Count == 0)
				return;

			Utilities.ShowMessageFromServerToEveryone("Deleted {0} grid(s) that had fewer than {1} blocks, no players within {2} m, and some of the blocks were damaged: {3}.",
				context.EntitiesForDeletion.Count, blockCountThreshold, context.PlayerDistanceThreshold, string.Join(", ", context.EntitiesForDeletionNames));
		}
	}
}
