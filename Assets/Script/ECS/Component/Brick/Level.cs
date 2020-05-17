using Unity.Entities;

namespace Breakout.Component.Brick
{
	[GenerateAuthoringComponent]
	public struct Level : IComponentData
	{
		public int m_level;
	}
}
