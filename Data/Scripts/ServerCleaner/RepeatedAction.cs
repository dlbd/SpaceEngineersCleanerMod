using System.Timers;

using Sandbox.ModAPI;

namespace ServerCleaner
{
	public abstract class RepeatedAction
	{
		private Timer timer;

		public RepeatedAction(double interval)
		{
			timer = TimerFactory.CreateTimer();
			timer.AutoReset = true;
			timer.Interval = interval;
			timer.Elapsed += (sender, e) => ShouldRun = true;
			timer.Start();
		}

		public void UpdateAfterSimulation()
		{
			if (!ShouldRun)
				return;

			ShouldRun = false;
			Run();
		}

		protected abstract void Run();

		public virtual bool ShouldRun { get; private set; }
	}
}
