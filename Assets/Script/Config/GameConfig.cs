

namespace Breakout.Config
{
	public static class GameConfig
	{
		public const float k_newBallDelay = 0.25f;
		public const float k_defaultBallSpeed = 1_200.0f;
		public const float k_maxPaddleBallReflectAngle = 66.6f;

		public const float k_paddleSpeedUpChance	= 0.01f;
		public const float k_ballSpeedUpChance		= 0.1f;
		public const float k_ballSpeedDownChance	= 0.05f;
		public const float k_paddleSizeUpChance		= 0.05f;
		public const float k_paddleSizeDownChance	= 0.1f;
		public const float k_multiball1Chance		= 0.25f;
		public const float k_multiball2Chance		= 0.15f;
		public const float k_multiball3Chance		= 0.05f;
		public const float k_multiball5Chance		= 0.025f;
		public const float k_multiball10Chance		= 0.01f;

		public const float k_paddleSpeedUpValue		= k_paddleSpeedUpChance;
		public const float k_ballSpeedUpValue		= k_paddleSpeedUpValue	+ k_ballSpeedUpChance;
		public const float k_ballSpeedDownValue		= k_ballSpeedUpValue	+ k_ballSpeedDownChance;
		public const float k_paddleSizeUpValue		= k_ballSpeedDownValue	+ k_paddleSizeUpChance;
		public const float k_paddleSizeDownValue	= k_paddleSizeUpValue	+ k_paddleSizeDownChance;
		public const float k_multiball1Value		= k_paddleSizeDownValue + k_multiball1Chance;
		public const float k_multiball2Value		= k_multiball1Value		+ k_multiball2Chance;
		public const float k_multiball3Value		= k_multiball2Value		+ k_multiball3Chance;
		public const float k_multiball5Value		= k_multiball3Value		+ k_multiball5Chance;
		public const float k_multiball10Value		= k_multiball5Value		+ k_multiball10Chance;

		public const float k_paddleSpeedMultiplier	= 1.1f;
		public const float k_ballSpeedMultiplier	= 1.15f;
		public const float k_paddleSizeMultiplier	= 1.2f;
	}
}
