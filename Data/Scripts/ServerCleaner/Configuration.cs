using System.Xml.Serialization;

namespace ServerCleaner
{
	public class Configuration
	{
		public bool FloatingObjectDeletion_Enabled = true;
		public int FloatingObjectDeletion_Interval = 7 * 60 * 1000;
		public double FloatingObjectDeletion_PlayerDistanceThreshold = 100;
		public bool FloatingObjectDeletion_MessageAdminsOnly = false;

		public bool UnownedGridDeletion_Enabled = true;
		public int UnownedGridDeletion_Interval = 9 * 60 * 1000;
		public double UnownedGridDeletion_PlayerDistanceThreshold = 500;
		public int UnownedGridDeletion_BlockCountThreshold = 50;
		public bool UnownedGridDeletion_MessageAdminsOnly = false;

		public bool DamagedGridDeletion_Enabled = true;
		public int DamagedGridDeletion_Interval = 10 * 60 * 1000;
		public double DamagedGridDeletion_PlayerDistanceThreshold = 500;
		public int DamagedGridDeletion_BlockCountThreshold = 5;
		public bool DamagedGridDeletion_MessageAdminsOnly = false;

		public bool RespawnShipDeletion_Enabled = true;
		public int RespawnShipDeletion_Interval = 11 * 60 * 1000;
		[XmlArrayItem(ElementName = "GridName", Type = typeof(string))]
		public string[] RespawnShipDeletion_GridNames = { "Atmospheric Lander mk.1", "RespawnShip", "RespawnShip2" };
		public double RespawnShipDeletion_PlayerDistanceThresholdForWarning = 50;
		public double RespawnShipDeletion_PlayerDistanceThresholdForDeletion = 1000;
		public bool RespawnShipDeletion_MessageAdminsOnly = false;

		public bool UnrenamedGridDeletion_Enabled = true;
		public int UnrenamedGridDeletion_Interval = 13 * 60 * 1000;
		public double UnrenamedGridDeletion_PlayerDistanceThresholdForWarning = 0;
		public double UnrenamedGridDeletion_PlayerDistanceThresholdForDeletion = 1000;
		public bool UnrenamedGridDeletion_WarnOnly = true;
		public bool UnrenamedGridDeletion_MessageAdminsOnly = false;

		public bool MessagesFromFile_Enabled = true;
		public int MessagesFromFile_Interval = 60 * 1000;

		public bool PopupsFromFile_Enabled = true;
		public int PopupsFromFile_Interval = 60 * 1000;
	}
}
