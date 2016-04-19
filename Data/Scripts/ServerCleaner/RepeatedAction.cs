using System.Timers;

using Sandbox.ModAPI;

namespace ServerCleaner
{
	public abstract class RepeatedAction
	{
		// TODO: run on the main thread in Before/AfterSimulation. Use the timer only for elapsed checking. No InvokeRun needed that way. Where is IsTrash() called in the main code?

		private readonly Timer timer;

		public RepeatedAction(double interval)
		{
			timer = TimerFactory.CreateTimer();
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
