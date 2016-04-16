using System.Timers;

namespace SpaceEngineersCleanerMod
{
	public interface ITimerFactory
	{
		Timer CreateTimer();
	}
}
