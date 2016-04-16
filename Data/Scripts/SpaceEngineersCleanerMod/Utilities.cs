using System.Collections.Generic;
using System.Text;

using Sandbox.ModAPI;
using VRage.Game.ModAPI;
using VRageMath;

namespace SpaceEngineersCleanerMod
{
	public static class Utilities
	{
		public static bool IsGameRunning()
		{
			return
				MyAPIGateway.Entities != null &&
				MyAPIGateway.Multiplayer != null &&
				MyAPIGateway.Players != null &&
				MyAPIGateway.Utilities != null;
		}

		public static void ShowMessageFromServer(string text)
		{
			if (!MyAPIGateway.Multiplayer.MultiplayerActive)
			{
				ShowMessageFromServerOnClient(text);
			}
			else  if (MyAPIGateway.Multiplayer.IsServer)
			{
				var bytes = Encoding.Unicode.GetBytes(text);

				var players = new List<IMyPlayer>();
				MyAPIGateway.Players.GetPlayers(players, p => p != null);

				foreach (var player in players)
					MyAPIGateway.Multiplayer.SendMessageTo(MessageIds.MessageFromServer, bytes, player.SteamUserId);
			}
		}

		public static void ShowMessageFromServer(string format, params object[] args)
		{
			ShowMessageFromServer(string.Format(format, args));
		}

		public static void ShowMessageFromServerOnClient(string text)
		{
			MyAPIGateway.Utilities.ShowMessage("Server", text);
		}

		public static bool AnyWithinDistance(Vector3D position, List<Vector3D> otherPositions, double threshold)
		{
			foreach (var otherPosition in otherPositions)
				if ((otherPosition - position).Length() < threshold)
					return true;

			return false;
		}
	}
}
