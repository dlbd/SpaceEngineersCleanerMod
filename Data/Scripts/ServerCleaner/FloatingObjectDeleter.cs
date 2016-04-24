using VRage.Game.ModAPI;

namespace ServerCleaner.Updatables.Deleters
{
	public class FloatingObjectDeleter : RepeatedDeleter<IMyFloatingObject, DeletionContext<IMyFloatingObject>>
	{
		public FloatingObjectDeleter(double interval, double playerDistanceThreshold)
			: base(interval, new DeletionContext<IMyFloatingObject>() { PlayerDistanceThreshold = playerDistanceThreshold })
		{
		}

		protected override void AfterDeletion(DeletionContext<IMyFloatingObject> context)
		{
			if (context.EntitiesForDeletion.Count == 0)
				return;

			Utilities.ShowMessageFromServerToEveryone("Deleted {0} floating object(s) with no players within {1} m.", context.EntitiesForDeletion.Count, context.PlayerDistanceThreshold);
		}
	}
}
