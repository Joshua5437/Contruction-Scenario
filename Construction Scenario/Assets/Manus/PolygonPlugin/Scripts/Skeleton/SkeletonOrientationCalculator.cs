using UnityEngine;
using Hermes.Protocol.Polygon;
using Manus.Utility;
using System;

namespace Manus.Polygon.Utilities
{
	public static class SkeletonOrientationCalculator
	{
		/// <summary>
		/// Calculate the rotation for the new bone
		/// </summary>
		/// <param name="p_Bone">Bone to get calculation for</param>
		/// <param name="p_Skeleton">The rest of the skeleton</param>
		public static void CalculateOrientation(this Bone p_Bone, SkeletonBoneReferences p_Skeleton)
		{
			Quaternion t_DesiredRotation = new Quaternion(0, 0, 0, 0);

			switch (p_Bone.type)
			{
				case BoneType.Root:
					{
						Tuple<Transform, Transform>[] t_Bones =
							{
								Tuple.Create(p_Skeleton.armLeft.hand.wrist.bone, p_Skeleton.armRight.hand.wrist.bone),
								Tuple.Create(p_Skeleton.legLeft.foot.bone, p_Skeleton.legRight.foot.bone)
							};

						Vector3 t_AimDirection = CalculateForward(t_Bones);
						Vector3 t_UpDirection = Vector3.up;

						t_DesiredRotation = Quaternion.LookRotation(t_AimDirection, t_UpDirection);
					}

					break;

				case BoneType.Head:
				case BoneType.LeftFoot:
				case BoneType.RightFoot:
				case BoneType.LeftToes:
				case BoneType.RightToes:
				case BoneType.LeftToesEnd:
				case BoneType.RightToesEnd:

					{
						Tuple<Transform, Transform>[] t_Bones =
							{
								Tuple.Create(p_Skeleton.armLeft.hand.wrist.bone, p_Skeleton.armRight.hand.wrist.bone),
								Tuple.Create(p_Skeleton.legLeft.foot.bone, p_Skeleton.legRight.foot.bone)
							};

						Vector3 t_AimDirection = CalculateForward(t_Bones);
						Vector3 t_UpDirection = Vector3.up;

						t_DesiredRotation = Quaternion.LookRotation(t_AimDirection, t_UpDirection);
					}

					break;
				case BoneType.Hips:
					{
						Tuple<Transform, Transform>[] t_Bones =
							{
								Tuple.Create(p_Skeleton.armLeft.hand.wrist.bone, p_Skeleton.armRight.hand.wrist.bone),
								Tuple.Create(p_Skeleton.legLeft.foot.bone, p_Skeleton.legRight.foot.bone)
							};

						Vector3 t_HeadConnection = p_Skeleton.head.neck.bone ? p_Skeleton.head.neck.bone.position : p_Skeleton.head.head.bone.position;
						Vector3 t_AimDirection = (p_Skeleton.body.spine.bone ? p_Skeleton.body.spine.bone.position : t_HeadConnection) - p_Bone.bone.position;
						Vector3 t_UpDirection = -CalculateForward(t_Bones);

						t_DesiredRotation = Quaternion.LookRotation(t_AimDirection, t_UpDirection);
					}

					break;
				case BoneType.Neck:

					{
						Tuple<Transform, Transform>[] t_Bones =
							{
								Tuple.Create(p_Skeleton.armLeft.hand.wrist.bone, p_Skeleton.armRight.hand.wrist.bone),
								Tuple.Create(p_Skeleton.legLeft.foot.bone, p_Skeleton.legRight.foot.bone)
							};

						Vector3 t_AimDirection = p_Skeleton.head.head.bone.position - p_Bone.bone.position;
						Vector3 t_UpDirection = -CalculateForward(t_Bones);

						t_DesiredRotation = Quaternion.LookRotation(t_AimDirection, t_UpDirection);
					}

					break;
				case BoneType.Spine:

					{
						Tuple<Transform, Transform>[] t_Bones =
							{
								Tuple.Create(p_Skeleton.armLeft.hand.wrist.bone, p_Skeleton.armRight.hand.wrist.bone),
								Tuple.Create(p_Skeleton.legLeft.foot.bone, p_Skeleton.legRight.foot.bone)
							};

						Vector3 t_HeadConnection = p_Skeleton.head.neck.bone ? p_Skeleton.head.neck.bone.position : p_Skeleton.head.head.bone.position;
						Vector3 t_AimDirection = (p_Skeleton.body.chest.bone ? p_Skeleton.body.chest.bone.position : t_HeadConnection) - p_Bone.bone.position;
						Vector3 t_UpDirection = -CalculateForward(t_Bones);

						t_DesiredRotation = Quaternion.LookRotation(t_AimDirection, t_UpDirection);
					}

					break;
				case BoneType.Chest:

					{
						Tuple<Transform, Transform>[] t_Bones =
							{
								Tuple.Create(p_Skeleton.armLeft.hand.wrist.bone, p_Skeleton.armRight.hand.wrist.bone),
								Tuple.Create(p_Skeleton.legLeft.foot.bone, p_Skeleton.legRight.foot.bone)
							};

						Vector3 t_HeadConnection = p_Skeleton.head.neck.bone ? p_Skeleton.head.neck.bone.position : p_Skeleton.head.head.bone.position;
						Vector3 t_AimDirection = (p_Skeleton.body.upperChest.bone ? p_Skeleton.body.upperChest.bone.position : t_HeadConnection) - p_Bone.bone.position;
						Vector3 t_UpDirection = -CalculateForward(t_Bones);

						t_DesiredRotation = Quaternion.LookRotation(t_AimDirection, t_UpDirection);
					}

					break;
				case BoneType.UpperChest:

					{
						Tuple<Transform, Transform>[] t_Bones =
							{
								Tuple.Create(p_Skeleton.armLeft.hand.wrist.bone, p_Skeleton.armRight.hand.wrist.bone),
								Tuple.Create(p_Skeleton.legLeft.foot.bone, p_Skeleton.legRight.foot.bone)
							};

						Vector3 t_HeadConnection = p_Skeleton.head.neck.bone ? p_Skeleton.head.neck.bone.position : p_Skeleton.head.head.bone.position;
						Vector3 t_AimDirection = t_HeadConnection - p_Bone.bone.position;
						Vector3 upDirection = -CalculateForward(t_Bones);

						t_DesiredRotation = Quaternion.LookRotation(t_AimDirection, upDirection);
					}

					break;
				case BoneType.LeftUpperLeg:

					{
						Tuple<Transform, Transform>[] t_Bones =
							{
								Tuple.Create(p_Skeleton.armLeft.hand.wrist.bone, p_Skeleton.armRight.hand.wrist.bone),
								Tuple.Create(p_Skeleton.legLeft.foot.bone, p_Skeleton.legRight.foot.bone)
							};

						Vector3 t_AimDirection = p_Skeleton.legLeft.lowerLeg.bone.position - p_Bone.bone.position;
						Vector3 t_UpDirection = CalculateForward(t_Bones);

						t_DesiredRotation = Quaternion.LookRotation(t_AimDirection, t_UpDirection);
					}

					break;
				case BoneType.RightUpperLeg:

					{
						Tuple<Transform, Transform>[] t_Bones =
							{
								Tuple.Create(p_Skeleton.armLeft.hand.wrist.bone, p_Skeleton.armRight.hand.wrist.bone),
								Tuple.Create(p_Skeleton.legLeft.foot.bone, p_Skeleton.legRight.foot.bone)
							};

						Vector3 t_AimDirection = p_Skeleton.legRight.lowerLeg.bone.position - p_Bone.bone.position;
						Vector3 t_UpDirection = CalculateForward(t_Bones);

						t_DesiredRotation = Quaternion.LookRotation(t_AimDirection, t_UpDirection);
					}

					break;
				case BoneType.LeftLowerLeg:

					{
						Tuple<Transform, Transform>[] t_Bones =
							{
								Tuple.Create(p_Skeleton.armLeft.hand.wrist.bone, p_Skeleton.armRight.hand.wrist.bone),
								Tuple.Create(p_Skeleton.legLeft.foot.bone, p_Skeleton.legRight.foot.bone)
							};

						Vector3 t_AimDirection = p_Skeleton.legLeft.foot.bone.position - p_Bone.bone.position;
						Vector3 t_UpDirection = CalculateForward(t_Bones);

						t_DesiredRotation = Quaternion.LookRotation(t_AimDirection, t_UpDirection);
					}

					break;
				case BoneType.RightLowerLeg:

					{
						Tuple<Transform, Transform>[] t_Bones =
							{
								Tuple.Create(p_Skeleton.armLeft.hand.wrist.bone, p_Skeleton.armRight.hand.wrist.bone),
								Tuple.Create(p_Skeleton.legLeft.foot.bone, p_Skeleton.legRight.foot.bone)
							};

						Vector3 t_AimDirection = p_Skeleton.legRight.foot.bone.position - p_Bone.bone.position;
						Vector3 t_UpDirection = CalculateForward(t_Bones);

						t_DesiredRotation = Quaternion.LookRotation(t_AimDirection, t_UpDirection);
					}

					break;
				case BoneType.LeftShoulder:

					{
						Vector3 t_AimDirection = p_Skeleton.armLeft.upperArm.bone.position - p_Bone.bone.position;
						Vector3 t_UpDirection = Vector3.up;

						t_DesiredRotation = Quaternion.LookRotation(t_AimDirection, t_UpDirection);
					}

					break;
				case BoneType.RightShoulder:

					{
						Vector3 t_AimDirection = p_Skeleton.armRight.upperArm.bone.position - p_Bone.bone.position;
						Vector3 t_UpDirection = Vector3.up;

						t_DesiredRotation = Quaternion.LookRotation(t_AimDirection, t_UpDirection);
					}

					break;
				case BoneType.LeftUpperArm:

					{
						Vector3 t_AimDirection = p_Skeleton.armLeft.lowerArm.bone.position - p_Bone.bone.position;
						Vector3 t_UpDirection = Vector3.up;

						t_DesiredRotation = Quaternion.LookRotation(t_AimDirection, t_UpDirection);
					}

					break;
				case BoneType.RightUpperArm:

					{
						Vector3 t_AimDirection = p_Skeleton.armRight.lowerArm.bone.position - p_Bone.bone.position;
						Vector3 t_UpDirection = Vector3.up;

						t_DesiredRotation = Quaternion.LookRotation(t_AimDirection, t_UpDirection);
					}

					break;
				case BoneType.LeftLowerArm:

					{
						Vector3 t_AimDirection = p_Skeleton.armLeft.hand.wrist.bone.position - p_Bone.bone.position;

						Vector3 t_ArmSide = Vector3.Cross(p_Bone.bone.position - p_Skeleton.armLeft.upperArm.bone.position, Vector3.up).normalized;
						Vector3 t_UpDirection = Vector3.Cross(t_ArmSide, p_Bone.bone.position - p_Skeleton.armLeft.upperArm.bone.position).normalized;

						t_DesiredRotation = Quaternion.LookRotation(t_AimDirection, t_UpDirection);
					}

					break;
				case BoneType.RightLowerArm:

					{
						Vector3 t_AimDirection = p_Skeleton.armRight.hand.wrist.bone.position - p_Bone.bone.position;

						Vector3 t_ArmSide = Vector3.Cross(p_Bone.bone.position - p_Skeleton.armRight.upperArm.bone.position, Vector3.up).normalized;
						Vector3 t_UpDirection = Vector3.Cross(t_ArmSide, p_Bone.bone.position - p_Skeleton.armRight.upperArm.bone.position).normalized;

						t_DesiredRotation = Quaternion.LookRotation(t_AimDirection, t_UpDirection);
					}

					break;
				case BoneType.LeftHand:

					{
						HandBoneReferences t_Hand = p_Skeleton.armLeft.hand;

						// Forward Vector
						Vector3 t_AimDirection = Vector3.zero;
						int t_FingerCount = 0;

						if (t_Hand.index.proximal.bone)
						{
							t_AimDirection += t_Hand.index.proximal.bone.position - t_Hand.wrist.bone.position;
							t_FingerCount++;
						}

						if (t_Hand.middle.proximal.bone)
						{
							t_AimDirection += t_Hand.middle.proximal.bone.position - t_Hand.wrist.bone.position;
							t_FingerCount++;
						}

						if (t_Hand.ring.proximal.bone)
						{
							t_AimDirection += t_Hand.ring.proximal.bone.position - t_Hand.wrist.bone.position;
							t_FingerCount++;
						}

						if (t_Hand.pinky.proximal.bone)
						{
							t_AimDirection += t_Hand.pinky.proximal.bone.position - t_Hand.wrist.bone.position;
							t_FingerCount++;
						}

						t_AimDirection = (t_AimDirection / t_FingerCount).normalized;

						// Up Vector
						Vector3 t_UpDirection = Vector3.zero;
						t_FingerCount = 0;

						if (t_Hand.index.proximal.bone && t_Hand.middle.proximal.bone)
						{
							t_UpDirection += Vector3.Cross(t_Hand.middle.proximal.bone.position - t_Hand.index.proximal.bone.position, t_AimDirection);
							t_FingerCount++;
						}

						if (t_Hand.middle.proximal.bone && t_Hand.ring.proximal.bone)
						{
							t_UpDirection += Vector3.Cross(t_Hand.ring.proximal.bone.position - t_Hand.middle.proximal.bone.position, t_AimDirection);
							t_FingerCount++;
						}

						if (t_Hand.ring.proximal.bone && t_Hand.pinky.proximal.bone)
						{
							t_UpDirection += Vector3.Cross(t_Hand.pinky.proximal.bone.position - t_Hand.ring.proximal.bone.position, t_AimDirection);
							t_FingerCount++;
						}

						t_UpDirection = (t_UpDirection / t_FingerCount).normalized;

						if (t_FingerCount == 0)
						{
							if (p_Skeleton.armLeft.lowerArm.bone != null)
								t_DesiredRotation = p_Skeleton.armLeft.lowerArm.bone.rotation * p_Skeleton.armLeft.lowerArm.desiredRotationOffset;
						}
						else
						{
							t_DesiredRotation = Quaternion.LookRotation(t_AimDirection, t_UpDirection);
						}
					}

					break;
				case BoneType.RightHand:
					
					{
						HandBoneReferences t_Hand = p_Skeleton.armRight.hand;

						// Forward Vector
						Vector3 t_AimDirection = Vector3.zero;
						int t_FingerCount = 0;

						if (t_Hand.index.proximal.bone)
						{
							t_AimDirection += t_Hand.index.proximal.bone.position - t_Hand.wrist.bone.position;
							t_FingerCount++;
						}

						if (t_Hand.middle.proximal.bone)
						{
							t_AimDirection += t_Hand.middle.proximal.bone.position - t_Hand.wrist.bone.position;
							t_FingerCount++;
						}

						if (t_Hand.ring.proximal.bone)
						{
							t_AimDirection += t_Hand.ring.proximal.bone.position - t_Hand.wrist.bone.position;
							t_FingerCount++;
						}

						if (t_Hand.pinky.proximal.bone)
						{
							t_AimDirection += t_Hand.pinky.proximal.bone.position - t_Hand.wrist.bone.position;
							t_FingerCount++;
						}

						t_AimDirection = (t_AimDirection / t_FingerCount).normalized;

						// Up Vector
						Vector3 t_UpDirection = Vector3.zero;
						t_FingerCount = 0;

						if (t_Hand.index.proximal.bone && t_Hand.middle.proximal.bone)
						{
							t_UpDirection += Vector3.Cross(t_Hand.middle.proximal.bone.position - t_Hand.index.proximal.bone.position, t_AimDirection);
							t_FingerCount++;
						}

						if (t_Hand.middle.proximal.bone && t_Hand.ring.proximal.bone)
						{
							t_UpDirection += Vector3.Cross(t_Hand.ring.proximal.bone.position - t_Hand.middle.proximal.bone.position, t_AimDirection);
							t_FingerCount++;
						}

						if (t_Hand.ring.proximal.bone && t_Hand.pinky.proximal.bone)
						{
							t_UpDirection += Vector3.Cross(t_Hand.pinky.proximal.bone.position - t_Hand.ring.proximal.bone.position, t_AimDirection);
							t_FingerCount++;
						}

						t_UpDirection = (t_UpDirection / t_FingerCount).normalized;

						if (t_FingerCount == 0)
						{
							if (p_Skeleton.armRight.lowerArm.bone != null)
								t_DesiredRotation = p_Skeleton.armRight.lowerArm.bone.rotation * p_Skeleton.armRight.lowerArm.desiredRotationOffset;
						}
						else
						{
							t_DesiredRotation = Quaternion.LookRotation(t_AimDirection, -t_UpDirection);
						}
					}

					break;

				case BoneType.LeftThumbProximal:
				case BoneType.RightThumbProximal:
				case BoneType.LeftIndexProximal:
				case BoneType.RightIndexProximal:
				case BoneType.LeftMiddleProximal:
				case BoneType.RightMiddleProximal:
				case BoneType.LeftRingProximal:
				case BoneType.RightRingProximal:
				case BoneType.LeftPinkyProximal:
				case BoneType.RightPinkyProximal:
				case BoneType.LeftThumbMiddle:
				case BoneType.RightThumbMiddle:
				case BoneType.LeftIndexMiddle:
				case BoneType.RightIndexMiddle:
				case BoneType.LeftMiddleMiddle:
				case BoneType.RightMiddleMiddle:
				case BoneType.LeftRingMiddle:
				case BoneType.RightRingMiddle:
				case BoneType.LeftPinkyMiddle:
				case BoneType.RightPinkyMiddle:
				case BoneType.LeftThumbDistal:
				case BoneType.RightThumbDistal:
				case BoneType.LeftIndexDistal:
				case BoneType.RightIndexDistal:
				case BoneType.LeftMiddleDistal:
				case BoneType.RightMiddleDistal:
				case BoneType.LeftRingDistal:
				case BoneType.RightRingDistal:
				case BoneType.LeftPinkyDistal:
				case BoneType.RightPinkyDistal:
				case BoneType.LeftThumbTip:
				case BoneType.RightThumbTip:
				case BoneType.LeftIndexTip:
				case BoneType.RightIndexTip:
				case BoneType.LeftMiddleTip:
				case BoneType.RightMiddleTip:
				case BoneType.LeftRingTip:
				case BoneType.RightRingTip:
				case BoneType.LeftPinkyTip:
				case BoneType.RightPinkyTip:
					// Maybe implement this later, but for now, skip fingers
					break;
				default:
					Debug.LogError("No rotation available for this bonetype");
					break;
			}

			if (t_DesiredRotation.IsValid())
			{
				p_Bone.desiredRotationOffset = Quaternion.Inverse(p_Bone.bone.rotation) * t_DesiredRotation;
			}
		}

		/// <summary>
		/// Calculate the forward direction
		/// </summary>
		/// <param name="p_Directions">collection of transform pairs, every pair should be a left and right transform</param>
		public static Vector3 CalculateForward(Tuple<Transform, Transform>[] p_Directions)
		{
			Vector3 t_Forward = Vector3.zero;
			foreach (Tuple<Transform, Transform> t_DirectionPairs in p_Directions)
			{
				Vector3 t_Cross = Vector3.Cross(t_DirectionPairs.Item2.position - t_DirectionPairs.Item1.position, Vector3.up);
				t_Cross.y = 0;

				t_Forward += t_Cross.normalized;
			}

			return t_Forward / p_Directions.Length;
		}
	}
}

