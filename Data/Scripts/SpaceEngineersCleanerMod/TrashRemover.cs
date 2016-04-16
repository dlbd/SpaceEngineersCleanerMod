using System.Collections.Generic;

using VRage.Game.ModAPI;

namespace SpaceEngineersCleanerMod
{
	public class TrashRemovalContext : RemovalContext
	{
		public List<IMySlimBlock> CurrentEntitySlimBlocks = new List<IMySlimBlock>();
	}

	/// <summary>
	/// Deleter of cubegrids that have few blocks and no owners.
	/// </summary>
	public class TrashRemover : RepeatedRemover<IMyCubeGrid, TrashRemovalContext>
	{
		private const int BlockCountThreshold = 50;
	
		public TrashRemover(ITimerFactory timerFactory, double interval, double playerDistanceTreshold) : base(timerFactory, interval, playerDistanceTreshold, new TrashRemovalContext())
		{
		}

		protected override void PrepareRemovalContext(TrashRemovalContext context)
		{
			base.PrepareRemovalContext(context);

			context.CurrentEntitySlimBlocks = new List<IMySlimBlock>();
		}

		protected override bool ShouldDeleteEntity(IMyCubeGrid entity, TrashRemovalContext context)
		{
			if (entity.SmallOwners.Count > 0)
				return false;

			context.CurrentEntitySlimBlocks.Clear();
			entity.GetBlocks(context.CurrentEntitySlimBlocks);

			if (context.CurrentEntitySlimBlocks.Count > BlockCountThreshold)
				return false;

			return true;
		}

		protected override void AfterRemoval(RemovalContext context)
		{
			Utilities.ShowMessageFromServer("Removed {0} grid(s) that had fewer than {1} blocks, no owner and no players within {2} m: {3}.",
				context.EntitiesForRemoval.Count, BlockCountThreshold, PlayerDistanceThreshold, string.Join(", ", context.EntitiesForRemovalNames));
		}
	}
}
