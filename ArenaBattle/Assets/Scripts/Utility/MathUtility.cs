//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// wt.shin: Math utilities
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;

namespace Devil.Common
{
    public static class MathUtility
	{
		public static Vector3 DividedBy(this Vector3 A, Vector3 B)
		{
			return new Vector3
			(
				A.x / B.x,
				A.y / B.y,
				A.z / B.z
			);
		}

		public static Vector3 MultiplyBy(this Vector3 A, Vector3 B)
		{
			return new Vector3
			(
				A.x * B.x,
				A.y * B.y,
				A.z * B.z
			);
		}

		// This algorithm is also very simple but the basis of it is much more complex. 
		// The formula looks like: .30 * R + .59 * G + .11 * B. The percentages here 
		// relate to how perceptive the eye is to a given color. 
		public static float GetColorWeight(this Vector3 color)
		{
			return (.30f * Mathf.Abs(color.x)) + (.59f * Mathf.Abs(color.y)) + (.11f * Mathf.Abs(color.z));
		}
	}
}