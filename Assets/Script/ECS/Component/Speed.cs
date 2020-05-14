using Unity.Entities;

namespace Breakout.Component
{
	[GenerateAuthoringComponent]
	public struct Speed : IComponentData
	{
		public float m_speed;
	}
}
