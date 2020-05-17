using Unity.Entities;

namespace Breakout.Component.Prefab
{
	[GenerateAuthoringComponent]
	public struct BrickPrefab : IComponentData
	{
		public Entity m_prefab_level1;
		public Entity m_prefab_level2;
		public Entity m_prefab_level3;
		public Entity m_prefab_level4;
		public Entity m_prefab_level5;
		public Entity m_prefab_level6;
	}
}
