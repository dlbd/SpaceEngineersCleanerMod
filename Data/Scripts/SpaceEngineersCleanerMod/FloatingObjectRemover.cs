using System.Collections.Generic;

using Sandbox.ModAPI;
using VRage.ModAPI;
using VRage.Game.ModAPI;
using VRageMath;

namespace SpaceEngineersCleanerMod
{
	public class FloatingObjectRemover : RepeatedAction
	{
		public const int PlayerDistanceThreshold = 100;

		public FloatingObjectRemover(ITimerFactory timerFactory) : base(timerFactory, 10 * 1000) // should run like every 5 minutes
		{
		}

		public override void Run()
		{
			var entities = new HashSet<IMyEntity>();
			MyAPIGateway.Entities.GetEntities(entities);

			var players = new List<IMyPlayer>();
			MyAPIGateway.Players.GetPlayers(players, p => p != null);

			var playerPositions = new List<Vector3D>();
			foreach (var player in players)
				playerPositions.Add(player.GetPosition());

			var entitiesToRemove = new List<IMyEntity>();

			foreach (var entity in entities)
			{
				if (!(entity is IMyFloatingObject))
					continue;

				if (Utilities.AnyWithinDistance(entity.GetPosition(), playerPositions, PlayerDistanceThreshold))
					continue;

				entitiesToRemove.Add(entity);
			}

			if (entitiesToRemove.Count == 0)
				return;

			foreach (var entity in entitiesToRemove)
				entity.Delete();

			Utilities.ShowMessageFromServer("Removed {0} floating objects with no players within {1} m.", entitiesToRemove.Count, PlayerDistanceThreshold);
		}
	}
}
