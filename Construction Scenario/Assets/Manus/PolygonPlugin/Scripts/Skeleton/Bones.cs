using UnityEngine;
using Hermes.Protocol.Polygon;
using Hermes.Protocol;
using Manus.Hermes;
using HProt = Hermes.Protocol;

namespace Manus.Polygon
{
	[System.Serializable]
	public class Bone
	{
		/// <summary>
		/// Is bone optional for polygon
		/// </summary>
		public bool optional = false;

		/// <summary>
		/// Is true when the bone is the reorientated parent
		/// </summary>
		public bool isReorientated = false;

		public BoneType type;
		public Transform bone;

		/// <summary>
		/// Desired rotation offset of the parent
		/// </summary>
		public Quaternion desiredRotationOffset;

		public Bone(bool p_Optional, BoneType p_Type)
		{
			optional = p_Optional;
			type = p_Type;
		}

		/// <summary>
		/// Assign bone transform
		/// </summary>
		/// <param name="p_Bone"></param>
		public void AssignTransform(Transform p_Bone)
		{
			bone = p_Bone;
		}

		/// <summary>
		/// Convert to hermes bone
		/// </summary>
		/// <param name="p_Bone">Unity bone</param>
		public static implicit operator HProt.Polygon.Bone(Bone p_Bone)
		{
			return new HProt.Polygon.Bone()  
			{
				Type = p_Bone.type,
				Position = new Translation() { Full = p_Bone.bone.position.ToProto() }, 
				Rotation = new Orientation() { Full = p_Bone.bone.rotation.ToProto() },
			};
		}
	}
}