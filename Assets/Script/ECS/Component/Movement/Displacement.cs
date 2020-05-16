using Unity.Entities;
using Unity.Mathematics;

namespace Breakout.Component.Movement
{
	[GenerateAuthoringComponent]
	public struct Displacement : IComponentData
	{
		public float2 m_displacement;
	}
}
