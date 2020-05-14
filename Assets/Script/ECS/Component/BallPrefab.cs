using Unity.Entities;

namespace Breakout.Component
{
	[GenerateAuthoringComponent]
	public struct BallPrefab : IComponentData
	{
		public Entity m_prefab;
	}
}
