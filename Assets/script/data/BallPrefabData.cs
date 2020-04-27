using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct BallPrefabData : IComponentData
{
	public Entity m_prefab;
	public float3 m_defaultPosition;
}
