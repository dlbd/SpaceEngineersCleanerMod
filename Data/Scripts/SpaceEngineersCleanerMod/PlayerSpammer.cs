using Sandbox.ModAPI;

namespace SpaceEngineersCleanerMod
{
	public class PlayerSpammer : RepeatedAction
	{
		private int timesNotified;

		public PlayerSpammer(ITimerFactory timerFactory) : base(timerFactory, 1000/*10 * 60 * 1000*/)
		{
		}

		public override void Run()
		{
			timesNotified++;
			Utilities.ShowMessageFromServer(string.Format("This is a test. Something could have happened {0} time(s).", timesNotified));
		}
	}
}
