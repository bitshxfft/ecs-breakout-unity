using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct AABBData : IComponentData
{
	public int2 m_bottomLeft;
	public int2 m_topRight;
}