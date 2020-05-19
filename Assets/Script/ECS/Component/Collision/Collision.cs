using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Breakout.Component.Collision
{
	[Flags]
	public enum CollisionLayer : int
	{
		Paddle			= 1 << 0,
		Ball			= 1 << 1,
		Brick 			= 1 << 2,
		LevelBounds		= 1 << 3,
		Killzone		= 1 << 4,
		Powerup			= 1 << 5,
	}

	// --------------------------------------------------------------------------------
	// --------------------------------------------------------------------------------

	public struct CollisionData
	{
		public float2 m_otherMin;
		public float2 m_otherMax;
		public CollisionLayer m_otherLayer;
	}

	// --------------------------------------------------------------------------------
	// --------------------------------------------------------------------------------

	[InternalBufferCapacity(2)]
	public struct CollisionEvent : IBufferElementData
	{
		public CollisionData m_collisionData;

		// ----------------------------------------------------------------------------

		public static implicit operator CollisionData(CollisionEvent collisionEvent)
		{
			return collisionEvent.m_collisionData;
		}

		public static implicit operator CollisionEvent(CollisionData collisionData)
		{
			return new CollisionEvent { m_collisionData = collisionData };
		}
	}
}
