using Unity.Entities;
using UnityEngine;

namespace Breakout.Component.Input
{

	[GenerateAuthoringComponent]
	public struct PaddleInput : IComponentData
	{
		public KeyCode m_leftKey;
		public KeyCode m_rightKey;
		public KeyCode m_spawnBallKey;
	}
}
