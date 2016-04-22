namespace ServerCleaner
{
	public class Configuration
	{
		public bool FloatingObjectDeletion_Enabled = true;
		public int FloatingObjectDeletion_Interval = 7 * 60 * 1000;
		public int FloatingObjectDeletion_PlayerDistanceThreshold = 100;

		public bool UnownedGridDeletion_Enabled = true;
		public int UnownedGridDeletion_Interval = 9 * 60 * 1000;
		public int UnownedGridDeletion_PlayerDistanceThreshold = 500;
		public int UnownedGridDeletion_BlockCountThreshold = 50;

		public bool DamagedGridDeletion_Enabled = true;
		public int DamagedGridDeletion_Interval = 10 * 60 * 1000;
		public int DamagedGridDeletion_PlayerDistanceThreshold = 500;
		public int DamagedGridDeletion_BlockCountThreshold = 5;

		public bool RespawnShipDeletion_Enabled = true;
		public int RespawnShipDeletion_Interval = 11 * 60 * 1000;
		public int RespawnShipDeletion_PlayerDistanceThresholdForWarning = 50;
		public int RespawnShipDeletion_PlayerDistanceThresholdForDeletion = 1000;

		public bool UnrenamedGridDeletion_Enabled = true;
		public int UnrenamedGridDeletion_Interval = 13 * 60 * 1000;
		public int UnrenamedGridDeletion_PlayerDistanceThresholdForWarning = 0;
		public int UnrenamedGridDeletion_PlayerDistanceThresholdForDeletion = 1000;
		public bool UnrenamedGridDeletion_WarnOnly = true;
	}
}
