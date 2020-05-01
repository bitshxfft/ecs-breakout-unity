using Unity.Entities;

[GenerateAuthoringComponent]
public struct BallPrefabData : IComponentData
{
	public Entity m_prefab;
}
