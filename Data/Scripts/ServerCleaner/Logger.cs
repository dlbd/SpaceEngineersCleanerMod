using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ServerCleaner
{
	public static class Logger
	{
		public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

		private static TextWriter writer;

		public static void Initialize()
		{
			if (Initialized)
				return;

			var oldContent = "";
			var fileName = string.Format("ServerCleaner_{0}.log", Path.GetFileNameWithoutExtension(MyAPIGateway.Session.CurrentPath));

			if (MyAPIGateway.Utilities.FileExistsInLocalStorage(fileName, typeof(Logger)))
			{
				using (var reader = MyAPIGateway.Utilities.ReadFileInLocalStorage(fileName, typeof(Logger)))
				{
					oldContent = reader.ReadToEnd();
				}
			}

			writer = MyAPIGateway.Utilities.WriteFileInLocalStorage(fileName, typeof(Logger));
			writer.Write(oldContent);
			writer.Flush();

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
			writer.Flush();
		}

		public static void WriteLine(string format, params object[] args)
		{
			WriteLine(string.Format(format, args));
		}

		public static bool Initialized { get; private set; }
	}
}
