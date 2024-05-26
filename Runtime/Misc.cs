using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
	public enum UpdateMethod {
		Update, FixedUpdate, LateUpdate
	}

	[System.Flags]
	public enum TransformMask {
		None           = 0b0_000_000_000_000_000,
		Global         = 0b0_111_000_111_000_111,
		Position       = 0b0_000_000_000_000_111,
		Rotation       = 0b0_000_000_111_000_000,
		UniformScale   = 0b1_000_000_000_000_000,
		Local          = 0b0_111_111_000_111_000,
		LocalPosition  = 0b0_000_000_000_111_000,
		LocalRotation  = 0b0_000_111_000_000_000,
		LocalScale     = 0b0_111_000_000_000_000,
		PositionX      = 0b0_000_000_000_000_001,
		PositionY      = 0b0_000_000_000_000_010,
		PositionZ      = 0b0_000_000_000_000_100,
		LocalPositionX = 0b0_000_000_000_001_000,
		LocalPositionY = 0b0_000_000_000_010_000,
		LocalPositionZ = 0b0_000_000_000_100_000,
		RotationX      = 0b0_000_000_001_000_000,
		RotationY      = 0b0_000_000_010_000_000,
		RotationZ      = 0b0_000_000_100_000_000,
		LocalRotationX = 0b0_000_001_000_000_000,
		LocalRotationY = 0b0_000_010_000_000_000,
		LocalRotationZ = 0b0_000_100_000_000_000,
		LocalScaleX    = 0b0_001_000_000_000_000,
		LocalScaleY    = 0b0_010_000_000_000_000,
		LocalScaleZ    = 0b0_100_000_000_000_000
	}
}
