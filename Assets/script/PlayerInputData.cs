using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct PlayerInputData : IComponentData
{
	public KeyCode m_leftKey;
	public KeyCode m_rightKey;
}
