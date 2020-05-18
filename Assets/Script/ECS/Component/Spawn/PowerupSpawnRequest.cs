using Breakout.Component.Powerup;
using Unity.Entities;

namespace Breakout.Component.Spawn
{
	public struct PowerupSpawnRequest : IComponentData
	{
		public PowerupType m_powerup;
	}
}
