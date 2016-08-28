using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sandbox.ModAPI;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRageMath;

namespace ServerCleaner
{
	public static partial class Utilities
	{
		public const string ServerNameInChat = "Server";
		public const int MaxDisplayedMessageLength = 400; // the chat window can fit about 200 W characters
		public const string MessageSnip = " [...]";

		public static readonly Predicate<IMyPlayer> AllPlayerSelector = player => true;
		public static readonly Predicate<IMyPlayer> AdminPlayerSelector = player => MyAPIGateway.Session.IsUserAdmin(player.SteamUserId);

		public static bool IsGameRunning()
		{
			return
				MyAPIGateway.Entities != null &&
				MyAPIGateway.Multiplayer != null &&
				MyAPIGateway.Players != null &&
				MyAPIGateway.Session != null &&
				MyAPIGateway.Utilities != null;
		}

		private static void SendMessageToPlayers(ushort id, byte[] bytes, Predicate<IMyPlayer> playerSelector = null)
		{
			var players = new List<IMyPlayer>();
			MyAPIGateway.Players.GetPlayers(players, player => player != null);

			foreach (var player in players)
			{
				if (playerSelector != null && !playerSelector(player))
					continue;

				MyAPIGateway.Multiplayer.SendMessageTo(MessageIds.MessageFromServer, bytes, player.SteamUserId);
			}
		}

		public static void ShowMessageFromServer(string text, Predicate<IMyPlayer> playerSelector = null)
		{
			Logger.WriteLine("{0}: {1}", ServerNameInChat, text);

			if (!MyAPIGateway.Multiplayer.MultiplayerActive)
			{
				ShowMessageFromServerOnClient(text);
			}
			else  if (MyAPIGateway.Multiplayer.IsServer)
			{
				SendMessageToPlayers(MessageIds.MessageFromServer, Encoding.Unicode.GetBytes(text), playerSelector);
			}
		}

		public static void ShowMessageFromServerToEveryone(string format, params object[] args)
		{
			ShowMessageFromServer(string.Format(format, args));
		}

		public static void ShowMessageFromServerToAdmins(string format, params object[] args)
		{
			ShowMessageFromServer(string.Format(format, args), AdminPlayerSelector);
		}

		public static void ShowMessageFromServerOnClient(string text)
		{
			if (text.Length > MaxDisplayedMessageLength)
				text = text.Substring(0, MaxDisplayedMessageLength - MessageSnip.Length) + MessageSnip;

			MyAPIGateway.Utilities.ShowMessage(ServerNameInChat, text);
		}

		public static void ShowPopupFromServer(string title, string subtitle, string text, Predicate<IMyPlayer> playerSelector = null)
		{
			Logger.WriteLine("\r\n-----\r\n{0}\r\n-----\r\n{1}\r\n-----\r\n{2}\r\n-----", title, subtitle, text);

			if (!MyAPIGateway.Multiplayer.MultiplayerActive)
			{
				ShowPopupFromServerOnClient(title, subtitle, text);
			}
			else if (MyAPIGateway.Multiplayer.IsServer)
			{
				var message = string.Format("{0}\0{1}\0{2}", title, subtitle, text);
				SendMessageToPlayers(MessageIds.PopupFromServer, Encoding.Unicode.GetBytes(message), playerSelector);
			}
		}

		public static void ShowPopupFromServerToEveryone(string title, string subtitle, string text)
		{
			ShowPopupFromServer(title, subtitle, text);
		}

		public static void ShowPopupFromServerToAdmins(string title, string subtitle, string text)
		{
			ShowPopupFromServer(title, subtitle, text, player => MyAPIGateway.Session.IsUserAdmin(player.SteamUserId));
		}

		public static void ShowPopupFromServerOnClient(string title, string subtitle, string text)
		{
			MyAPIGateway.Utilities.ShowMissionScreen(title, "", subtitle, text, null, "Close");
		}

		public static bool AnyWithinDistance(Vector3D position, List<Vector3D> otherPositions, double threshold)
		{
			foreach (var otherPosition in otherPositions)
				if ((otherPosition - position).Length() < threshold)
					return true;

			return false;
		}

		public static string GetOwnerNameString(IMyEntity entity, List<IMyIdentity> playerIdentities)
		{
			var cubeGrid = entity as IMyCubeGrid;
			return cubeGrid == null ? "???" : GetOwnerNameString(cubeGrid.SmallOwners, playerIdentities);
		}

		public static string GetOwnerNameString(List<long> ownerIds, List<IMyIdentity> playerIdentities)
		{
			if (ownerIds.Count == 0)
				return "noone";

			var names = playerIdentities
				.Where(identity => ownerIds.Contains(identity.PlayerId))
				.Select(identity => identity.DisplayName)
				.ToList();

			if (names.Count != ownerIds.Count)
				names.Add("???");

			return string.Join(" & ", names);
		}
	}
}
