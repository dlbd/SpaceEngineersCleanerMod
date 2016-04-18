namespace SpaceEngineersCleanerMod
{
	public class PlayerSpammer : RepeatedAction
	{
		private int timesNotified;

		public PlayerSpammer(double interval) : base(interval)
		{
		}

		public override void Run()
		{
			timesNotified++;
			Utilities.ShowMessageFromServer(string.Format("This is a test. Something could have happened {0} time(s).", timesNotified));
		}
	}
}
