using Unity.Entities;
using UnityEngine;

namespace Breakout.Component
{

	[GenerateAuthoringComponent]
	public struct PaddleInput : IComponentData
	{
		public KeyCode m_leftKey;
		public KeyCode m_rightKey;
	}
}
