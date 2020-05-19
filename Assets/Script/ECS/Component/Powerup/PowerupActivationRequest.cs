using Unity.Entities;

namespace Breakout.Component.Powerup
{
	public struct PaddleSizePowerupActivationRequest : IComponentData
	{
		public float m_sizelMultiplier;
	}

	// --------------------------------------------------------------------------------
	// --------------------------------------------------------------------------------

	public struct PaddleSpeedPowerupActivationRequest : IComponentData
	{
		public float m_speedlMultiplier;
	}

	// --------------------------------------------------------------------------------
	// --------------------------------------------------------------------------------

	public struct BallSpeedPowerupActivationRequest : IComponentData
	{
		public float m_speedlMultiplier;
	}

	// --------------------------------------------------------------------------------
	// --------------------------------------------------------------------------------

	public struct MultiballPowerupActivationRequest : IComponentData
	{
		public int m_ballCount;
	}
}
