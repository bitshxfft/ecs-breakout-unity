using Unity.Entities;

namespace Breakout.Component.Prefab
{
	[GenerateAuthoringComponent]
	public struct PowerupPrefab : IComponentData
	{
		public Entity m_prefab;
	}
}
