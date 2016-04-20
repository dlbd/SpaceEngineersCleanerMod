using System;

using Sandbox.ModAPI;

namespace ServerCleaner
{
	public abstract class RepeatedDeleter<TEntity, TDeletionContext> : RepeatedAction
		where TEntity : class
		where TDeletionContext : DeletionContext
	{
		private readonly TDeletionContext context;

		public RepeatedDeleter(double interval, double playerDistanceTreshold, TDeletionContext initialDeletionContext) : base(interval)
		{
			context = initialDeletionContext;
			PlayerDistanceThreshold = playerDistanceTreshold;
		}

		protected override void Run()
		{
			try
			{
				PrepareDeletionContext(context);

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

					if (!BeforeDelete(entity, context))
						continue;

					context.EntitiesForDeletion.Add(untypedEntity);
				}

				foreach (var entity in context.EntitiesForDeletion)
				{
					if (entity.SyncObject == null)
						entity.Delete();
					else
						entity.SyncObject.SendCloseRequest();

					context.EntitiesForDeletionNames.Add(entity.DisplayName);
				}

				AfterDeletion(context);
			}
			catch (Exception ex)
			{
				Logger.WriteLine("Exception in RepeatedDeleter.Run(): {0}", ex);
				Utilities.ShowMessageFromServer("Oh no, there was an error while I was deleting stuff, let's hope nothing broke: " + ex.Message);
			}
		}

		protected virtual void PrepareDeletionContext(TDeletionContext context)
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

			context.EntitiesForDeletion.Clear();
			context.EntitiesForDeletionNames.Clear();
		}

		protected virtual bool BeforeDelete(TEntity entity, TDeletionContext context)
		{
			return true;
		}

		protected virtual void AfterDeletion(TDeletionContext context)
		{
		}

		public double PlayerDistanceThreshold { get; private set; }
	}
}
