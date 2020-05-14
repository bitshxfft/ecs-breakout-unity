using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct BallInput : IComponentData
{
	public KeyCode m_launchKey;
}
