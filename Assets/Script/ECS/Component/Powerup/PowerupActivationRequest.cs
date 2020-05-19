using Unity.Entities;

namespace Breakout.Component.Powerup
{
	public struct PowerupActivationRequest : IComponentData
	{
		public Powerup m_powerup;
	}
}
