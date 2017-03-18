using System.Collections.Generic;
using Sandbox.ModAPI;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRageMath;

namespace ServerCleaner.Updatables.Deleters
{
	// These classes were orginally created to prevent extra object allocations,
	// but the code might be cleaner without them. Alternatively, they could
	// be expanded to include some kind of caching...

	public class DeletionContext<TEntity>
	{
		public double PlayerDistanceThreshold;

		public HashSet<IMyEntity> Entities = new HashSet<IMyEntity>();
		public List<IMyPlayer> Players = new List<IMyPlayer>();
		public List<Vector3D> PlayerPositions = new List<Vector3D>();

		public List<IMyEntity> EntitiesForDeletion = new List<IMyEntity>();
		public List<string> EntitiesForDeletionNames = new List<string>();

		public virtual void Prepare()
		{
			Entities.Clear();
			MyAPIGateway.Entities.GetEntities(Entities, entity => entity is TEntity);

			Players.Clear();
			MyAPIGateway.Players.GetPlayers(Players, player => player != null);

			PlayerPositions.Clear(); // Player positions are used by some deleters even when PlayerDistanceThreshold == 0
			foreach (var player in Players)
				PlayerPositions.Add(player.GetPosition());

			EntitiesForDeletion.Clear();
			EntitiesForDeletionNames.Clear();
		}
	}

	public class CubeGridDeletionContext : DeletionContext<IMyCubeGrid>
	{
		public List<IMySlimBlock> CurrentEntitySlimBlocks = new List<IMySlimBlock>();
	}

	public class ComplexCubeGridDeletionContext : CubeGridDeletionContext
	{
		public double PlayerDistanceThresholdForActualDeletion;

		public List<IMyIdentity> PlayerIdentities = new List<IMyIdentity>();
		public List<long> OnlinePlayerIds = new List<long>();

		public List<string> NameStringsForDeletion = new List<string>();
		public List<string> NameStringsForLaterDeletion = new List<string>();

		public override void Prepare()
		{
			base.Prepare();

			PlayerIdentities.Clear();
			MyAPIGateway.Players.GetAllIdentites(PlayerIdentities);

			OnlinePlayerIds.Clear();

			foreach (var player in Players)
			{
				if (player.Client == null)
					continue;

				OnlinePlayerIds.Add(player.IdentityId);
			}

			NameStringsForDeletion.Clear();
			NameStringsForLaterDeletion.Clear();
		}
	}
}
