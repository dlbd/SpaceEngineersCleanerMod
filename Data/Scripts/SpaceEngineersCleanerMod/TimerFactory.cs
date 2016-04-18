using System.Collections.Generic;
using System.Timers;

namespace SpaceEngineersCleanerMod
{
	public static class TimerFactory
	{
		private static readonly List<Timer> timers = new List<Timer>();

		public static Timer CreateTimer()
		{
			var timer = new Timer();
			timers.Add(timer);
			return timer;
		}

		public static void CloseAllTimers()
		{
			foreach (var timer in timers)
				timer.Close();

			timers.Clear();
		}
	}
}
