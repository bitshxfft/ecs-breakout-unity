using Unity.Entities;
using UnityEngine;

namespace Breakout.Component
{
	[GenerateAuthoringComponent]
	public struct BallInput : IComponentData
	{
		public KeyCode m_launchKey;
	}
}
