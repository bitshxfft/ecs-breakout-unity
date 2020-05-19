using System;
using Unity.Entities;

namespace Breakout.Component.Powerup
{
	[Flags]
	public enum PowerupType : int
	{
		None		= 0,
		PaddleSpeed	= 1 << 0,
		BallSpeed	= 1 << 1,
		Multiball	= 1 << 3
	}

	// -----------------------------------------------------------------------------------
	// -----------------------------------------------------------------------------------

	[GenerateAuthoringComponent]
	public struct PowerupData : IComponentData
	{
		public PowerupType m_powerup;
		public float m_context;
	}
}
