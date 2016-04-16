using VRage.Game.ModAPI;

namespace SpaceEngineersCleanerMod
{
	public class FloatingObjectRemover : RepeatedRemover<IMyFloatingObject, RemovalContext>
	{
		public FloatingObjectRemover(ITimerFactory timerFactory, double interval, double playerDistanceTreshold) : base(timerFactory, interval, playerDistanceTreshold, new RemovalContext())
		{
		}

		protected override void AfterRemoval(RemovalContext context)
		{
			Utilities.ShowMessageFromServer("Removed {0} floating objects with no players within {1} m.", context.EntitiesForRemoval.Count, PlayerDistanceThreshold);
		}
	}
}
