using Unity.Entities;

namespace Breakout.Component.Prefab
{
	[GenerateAuthoringComponent]
	public struct BallPrefab : IComponentData
	{
		public Entity m_prefab;
	}
}
