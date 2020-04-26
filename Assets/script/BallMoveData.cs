using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct BallMoveData : IComponentData
{
	public float2 m_direction;
	public float m_speed;
}
