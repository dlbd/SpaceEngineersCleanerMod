using System.Collections.Generic;
using System.Timers;

namespace ServerCleaner
{
	public static class TimerFactory // is this class necessary since there is an IUpdatable interface?
	{
		private static List<Timer> timers = new List<Timer>();

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
