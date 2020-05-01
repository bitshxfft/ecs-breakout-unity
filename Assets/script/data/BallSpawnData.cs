using Unity.Entities;
using Unity.Mathematics;

public enum BallSpawnState : byte
{
	AttachToPaddle,
	Live,
}

public struct BallSpawnData : IComponentData
{
	public float3 m_position;
	public float3 m_direction;
	public float m_speed;
	public float m_delay;
	public BallSpawnState m_spawnState;
}