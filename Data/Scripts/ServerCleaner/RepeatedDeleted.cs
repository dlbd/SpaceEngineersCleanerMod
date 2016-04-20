using System;

namespace ServerCleaner
{
	public abstract class RepeatedDeleter<TEntity, TDeletionContext> : RepeatedAction
		where TEntity : class
		where TDeletionContext : DeletionContext<TEntity>
	{
		private TDeletionContext context;

		public RepeatedDeleter(double interval, TDeletionContext initialDeletionContext) : base(interval)
		{
			context = initialDeletionContext;
		}

		protected override void Run()
		{
			try
			{
				context.Prepare();

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

					if (context.PlayerDistanceThreshold > 0 && Utilities.AnyWithinDistance(untypedEntity.GetPosition(), context.PlayerPositions, context.PlayerDistanceThreshold))
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

		protected virtual bool BeforeDelete(TEntity entity, TDeletionContext context)
		{
			return true;
		}

		protected virtual void AfterDeletion(TDeletionContext context)
		{
		}
	}
}
