using System;
using Unity.Collections;
using Unity.Mathematics;

namespace Breakout.Component
{
	[Flags]
	public enum CollisionLayer : uint
	{
		None		= 0,
		Paddle		= 1 << 0,
		Ball		= 1 << 1,
		Brick 		= 1 << 2,
		LevelBounds	= 1 << 3
	}

	// --------------------------------------------------------------------------------
	// --------------------------------------------------------------------------------

	public readonly struct Collision
	{
		public readonly AABB m_otherAABB;
		public readonly int2 m_otherPosition;

		// ----------------------------------------------------------------------------

		public Collision(AABB otherAABB, int2 otherPosition)
		{
			m_otherAABB = otherAABB;
			m_otherPosition = otherPosition;
		}	
	}

	// --------------------------------------------------------------------------------
	// --------------------------------------------------------------------------------

	public class Collisions
	{
		public NativeArray<Collision> m_intersections;
	}
}
