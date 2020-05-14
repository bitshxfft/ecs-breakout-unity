using Unity.Entities;
using Unity.Mathematics;

namespace Breakout.Component
{

	[GenerateAuthoringComponent]
	public struct Direction : IComponentData
	{
		public float2 m_direction;
	}
}
