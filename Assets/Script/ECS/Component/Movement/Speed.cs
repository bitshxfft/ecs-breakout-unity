using Unity.Entities;

namespace Breakout.Component.Movement
{
	[GenerateAuthoringComponent]
	public struct Speed : IComponentData
	{
		public float m_speed;
	}
}
