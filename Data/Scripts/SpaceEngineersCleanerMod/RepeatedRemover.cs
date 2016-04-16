using System;
using System.Collections.Generic;

using Sandbox.ModAPI;
using VRage.ModAPI;
using VRage.Game.ModAPI;
using VRageMath;

namespace SpaceEngineersCleanerMod
{
	public class RemovalContext
	{
		public HashSet<IMyEntity> Entities = new HashSet<IMyEntity>();
		public List<IMyPlayer> Players = new List<IMyPlayer>();
		public List<Vector3D> PlayerPositions = new List<Vector3D>();

		public List<IMyEntity> EntitiesForRemoval = new List<IMyEntity>();
		public List<string> EntitiesForRemovalNames = new List<string>();
	}

	public abstract class RepeatedRemover<TEntity, TRemovalContext> : RepeatedAction
		where TEntity : class
		where TRemovalContext : RemovalContext
	{
		private readonly TRemovalContext context;

		public RepeatedRemover(ITimerFactory timerFactory, double interval, double playerDistanceTreshold, TRemovalContext initialRemovalContext) : base(timerFactory, interval)
		{
			context = initialRemovalContext;
			PlayerDistanceThreshold = playerDistanceTreshold;
		}

		public override void Run()
		{
			try
			{
				PrepareRemovalContext(context);

				foreach (var untypedEntity in context.Entities)
				{
					var entity = untypedEntity as TEntity;

					if (entity == null)
						continue;

					if (untypedEntity.MarkedForClose)
					{
						Utilities.ShowMessageFromServer("{0} is already marked for close :/", untypedEntity.DisplayName);
						continue;
					}

					if (untypedEntity.Closed)
					{
						Utilities.ShowMessageFromServer("{0} is already closed :/", untypedEntity.DisplayName);
						continue;
					}

					if (PlayerDistanceThreshold > 0 && Utilities.AnyWithinDistance(untypedEntity.GetPosition(), context.PlayerPositions, PlayerDistanceThreshold))
						continue;

					if (!ShouldDeleteEntity(entity, context))
						continue;

					context.EntitiesForRemoval.Add(untypedEntity);
				}

				if (context.EntitiesForRemoval.Count == 0)
					return;

				foreach (var entity in context.EntitiesForRemoval)
				{
					if (entity.SyncObject == null)
						entity.Delete();
					else
						entity.SyncObject.SendCloseRequest();

					context.EntitiesForRemovalNames.Add(entity.DisplayName);
				}

				AfterRemoval(context);
			}
			catch (Exception ex)
			{
				Utilities.ShowMessageFromServer("Oh no, there was an error while I was deleting stuff, let's hope nothing broke: " + ex.Message);
			}
		}

		protected virtual void PrepareRemovalContext(TRemovalContext context)
		{
			context.Entities.Clear();
			MyAPIGateway.Entities.GetEntities(context.Entities, entity => entity is TEntity);

			context.Players.Clear();
			MyAPIGateway.Players.GetPlayers(context.Players, player => player != null);

			if (PlayerDistanceThreshold > 0)
			{
				context.PlayerPositions.Clear();

				foreach (var player in context.Players)
					context.PlayerPositions.Add(player.GetPosition());
			}

			context.EntitiesForRemoval.Clear();
			context.EntitiesForRemovalNames.Clear();
		}

		protected virtual bool ShouldDeleteEntity(TEntity entity, TRemovalContext context)
		{
			return true;
		}

		protected virtual void AfterRemoval(RemovalContext context)
		{
		}

		public double PlayerDistanceThreshold { get; private set; }
	}
}
