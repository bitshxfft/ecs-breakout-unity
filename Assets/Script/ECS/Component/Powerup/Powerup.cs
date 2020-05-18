using System;

namespace Breakout.Component.Powerup
{
	[Flags]
	public enum PowerupType : int
	{
		PaddleSpeedUp	= 1 << 0,
		BallSpeedUp		= 1 << 1,
		BallSpeedDown	= 1 << 2,
		PaddleSizeUp	= 1 << 3,
		PaddleSizeDown	= 1 << 4,
		Multiball_1		= 1 << 5,
		Multiball_2		= 1 << 6,
		Multiball_3		= 1 << 7,
		Multiball_5		= 1 << 8,
		Multiball_10	= 1 << 9,
	}
}
