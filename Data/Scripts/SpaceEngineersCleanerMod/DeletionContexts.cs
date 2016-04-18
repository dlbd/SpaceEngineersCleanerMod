using System.Collections.Generic;

using VRageMath;
using VRage.Game.ModAPI;
using VRage.ModAPI;

namespace SpaceEngineersCleanerMod
{
	public class DeletionContext
	{
		public HashSet<IMyEntity> Entities = new HashSet<IMyEntity>();
		public List<IMyPlayer> Players = new List<IMyPlayer>();
		public List<Vector3D> PlayerPositions = new List<Vector3D>();

		public List<IMyEntity> EntitiesForDeletion = new List<IMyEntity>();
		public List<string> EntitiesForDeletionNames = new List<string>();
	}

	public class CubeGridDeletionContext : DeletionContext
	{
		public List<IMySlimBlock> CurrentEntitySlimBlocks = new List<IMySlimBlock>();
	}
}
