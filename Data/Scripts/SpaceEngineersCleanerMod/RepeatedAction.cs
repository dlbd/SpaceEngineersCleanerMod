using System;
using System.Timers;

using Sandbox.ModAPI;

namespace SpaceEngineersCleanerMod
{
	public abstract class RepeatedAction
	{
		private readonly Timer timer;

		public RepeatedAction(ITimerFactory timerFactory, double interval)
		{
			if (timerFactory == null)
				throw new ArgumentNullException("timerFactory");

			timer = timerFactory.CreateTimer();
			timer.AutoReset = true;
			timer.Interval = interval;
			timer.Elapsed += (sender, e) => InvokeRun();
			timer.Start();
		}

		private void InvokeRun()
		{
			MyAPIGateway.Utilities.InvokeOnGameThread(() =>
			{
				if (!Utilities.IsGameRunning())
					return;

				Run();
			});
		}

		public abstract void Run();
	}
}
