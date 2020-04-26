using Unity.Entities;

[GenerateAuthoringComponent]
public struct PaddleMoveData : IComponentData
{
	public int m_direction;
	public float m_speed;
}
