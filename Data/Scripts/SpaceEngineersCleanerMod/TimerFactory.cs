using System.Collections.Generic;
using System.Timers;

namespace SpaceEngineersCleanerMod
{
	public class TimerFactory : ITimerFactory
	{
		private readonly List<Timer> timers = new List<Timer>();

		public Timer CreateTimer()
		{
			var timer = new Timer();
			timers.Add(timer);
			return timer;
		}

		public void CloseAllTimers()
		{
			foreach (var timer in timers)
				timer.Close();
		}
	}
}
