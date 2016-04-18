using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SpaceEngineersCleanerMod
{
	public static class Logger
	{
		public const string DateTimeFormat = "yyyy-MM-dd hh:mm:ss.fff";

		private static TextWriter writer;

		public static void Initialize()
		{
			if (Initialized)
				return;

			var oldContent = "";
			var fileName = string.Format("ServerCleaner_{0}.log", Path.GetFileNameWithoutExtension(MyAPIGateway.Session.CurrentPath));

			if (MyAPIGateway.Utilities.FileExistsInGlobalStorage(fileName))
			{
				using (var reader = MyAPIGateway.Utilities.ReadFileInGlobalStorage(fileName))
				{
					oldContent = reader.ReadToEnd();
				}
			}

			writer = MyAPIGateway.Utilities.WriteFileInGlobalStorage(fileName);
			writer.Write(oldContent);

			Initialized = true;
		}

		public static void Close()
		{
			if (!Initialized)
				return;

			writer.Close();

			Initialized = false;
		}

		public static void WriteLine(string line)
		{
			if (!Initialized)
				return;

			writer.WriteLine("[{0}] {1}", DateTime.Now.ToString(DateTimeFormat), line);
		}

		public static void WriteLine(string format, params object[] args)
		{
			WriteLine(string.Format(format, args));
		}

		public static bool Initialized { get; private set; }
	}
}
