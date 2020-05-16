using Unity.Entities;
using UnityEngine;

namespace Breakout.Component.Input
{
	[GenerateAuthoringComponent]
	public struct BallInput : IComponentData
	{
		public KeyCode m_launchKey;
	}
}
