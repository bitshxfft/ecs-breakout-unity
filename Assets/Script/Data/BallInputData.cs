using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct BallInputData : IComponentData
{
	public KeyCode m_launchKey;
}
