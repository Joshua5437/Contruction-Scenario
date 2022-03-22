using System.Collections.Generic;
using UnityEngine;

namespace Manus.Polygon.Utilities
{
	public static class SkeletonBindPoseUtilities
	{
		/// <summary>
		/// Put model in bindpose
		/// </summary>
		/// <param name="p_GameObject">Model root</param>
		public static void SampleBindPose(GameObject p_GameObject)
		{
			List<SkinnedMeshRenderer> t_Skins = new List<SkinnedMeshRenderer>(p_GameObject.GetComponentsInChildren<SkinnedMeshRenderer>());
			t_Skins.Sort(new SkinTransformHierarchySorter());
			foreach (SkinnedMeshRenderer t_Skin in t_Skins)
			{
				Matrix4x4 t_GoMatrix = t_Skin.transform.localToWorldMatrix;
				List<Transform> t_Bones = new List<Transform>(t_Skin.bones);
				Vector3[] t_BackupLocalPosition = new Vector3[t_Bones.Count];

				// backup local position of bones. Only use rotation given by bind pose
				for (int i = 0; i < t_Bones.Count; i++)
				{
					t_BackupLocalPosition[i] = t_Bones[i].localPosition;
				}

				// Set all parents to be null to be able to set global alignments of bones without affecting their children.
				Dictionary<Transform, Transform> t_Parents = new Dictionary<Transform, Transform>();
				foreach (Transform t_Bone in t_Bones)
				{
					t_Parents[t_Bone] = t_Bone.parent;
					t_Bone.parent = null;
				}

				// Set global space position and rotation of each bone
				for (int i = 0; i < t_Bones.Count; i++)
				{
					Vector3 t_Position;
					Quaternion t_Rotation;
					GetBindPoseBonePositionRotation(t_GoMatrix, t_Skin.sharedMesh.bindposes[i], t_Bones[i], out t_Position, out t_Rotation);
					t_Bones[i].position = t_Position;
					t_Bones[i].rotation = t_Rotation;
				}

				// Reconnect bones in their original hierarchy
				foreach (Transform t_Bone in t_Bones)
					t_Bone.parent = t_Parents[t_Bone];

				// put back local postion of bones
				for (int i = 0; i < t_Bones.Count; i++)
				{
					t_Bones[i].localPosition = t_BackupLocalPosition[i];
				}
			}
		}
		
		/// <summary>
		/// Get the bindpose of a specific bone
		/// </summary>
		public static void GetBindPoseBonePositionRotation(Matrix4x4 p_SkinMatrix, Matrix4x4 p_BoneMatrix, Transform p_Bone, out Vector3 p_Position, out Quaternion p_Rotation)
		{
			// Get global matrix for bone
			Matrix4x4 t_BindMatrixGlobal = p_SkinMatrix * p_BoneMatrix.inverse;

			// Get local X, Y, Z, and position of matrix
			Vector3 t_MX = new Vector3(t_BindMatrixGlobal.m00, t_BindMatrixGlobal.m10, t_BindMatrixGlobal.m20);
			Vector3 t_MY = new Vector3(t_BindMatrixGlobal.m01, t_BindMatrixGlobal.m11, t_BindMatrixGlobal.m21);
			Vector3 t_MZ = new Vector3(t_BindMatrixGlobal.m02, t_BindMatrixGlobal.m12, t_BindMatrixGlobal.m22);
			Vector3 t_MP = new Vector3(t_BindMatrixGlobal.m03, t_BindMatrixGlobal.m13, t_BindMatrixGlobal.m23);

			// Set position
			// Adjust scale of matrix to compensate for difference in binding scale and model scale
			float t_BindScale = t_MZ.magnitude;
			float t_ModelScale = Mathf.Abs(p_Bone.lossyScale.z);
			p_Position = t_MP * (t_ModelScale / t_BindScale);

			// Set rotation
			// Check if scaling is negative and handle accordingly
			if (Vector3.Dot(Vector3.Cross(t_MX, t_MY), t_MZ) >= 0)
				p_Rotation = Quaternion.LookRotation(t_MZ, t_MY);
			else
				p_Rotation = Quaternion.LookRotation(-t_MZ, -t_MY);
		}

		private class SkinTransformHierarchySorter : IComparer<SkinnedMeshRenderer>
		{
			public int Compare(SkinnedMeshRenderer p_SkinA, SkinnedMeshRenderer p_SkinB)
			{
				Transform t_A = p_SkinA.transform;
				Transform t_B = p_SkinB.transform;
				while (true)
				{
					if (t_A == null)
						if (t_B == null)
							return 0;
						else
							return -1;
					if (t_B == null)
						return 1;
					t_A = t_A.parent;
					t_B = t_B.parent;
				}
			}
		}

		/// <summary>
		/// Move transform without moving childs
		/// </summary>
		/// <param name="p_Transform">Transform to move</param>
		/// <param name="p_Position">New position</param>
		public static void MoveTransform(this Transform p_Transform, Vector3 p_Position)
		{
			// Find all children
			var t_Children = new List<Transform>();
			for (int i = 0; i < p_Transform.childCount; i++)
			{
				t_Children.Add(p_Transform.GetChild(i));
			}

			// Un-parent all children
			foreach (Transform t_Child in t_Children)
			{
				t_Child.SetParent(null);
			}

			// Change rotation
			p_Transform.position = p_Position;

			// Re-parent all children
			foreach (Transform t_Child in t_Children)
			{
				t_Child.SetParent(p_Transform);
			}
		}

		/// <summary>
		/// Rotate transform without moving childs
		/// </summary>
		/// <param name="p_Transform">Transform to rotate</param>
		/// <param name="p_Rotation">New rotation</param>
		public static void ReorientTransform(this Transform p_Transform, Quaternion p_Rotation)
		{
			// Find all children
			var t_Children = new List<Transform>();
			for (int i = 0; i < p_Transform.childCount; i++)
			{
				t_Children.Add(p_Transform.GetChild(i));
			}
			
			// Un-parent all children
			foreach (Transform t_Child in t_Children)
			{
				t_Child.SetParent(null);
			}

			// Change rotation
			p_Transform.rotation = p_Rotation;

			// Re-parent all children
			foreach (Transform t_Child in t_Children)
			{
				t_Child.SetParent(p_Transform);
			}
		}

		/// <summary>
		/// Scale transform without scaling childs
		/// </summary>
		/// <param name="p_Transform">Transform to scale</param>
		/// <param name="p_Scale">New scale</param>
		public static void ScaleTransform(this Transform p_Transform, Vector3 p_Scale)
		{
			// Find all children
			var t_Children = new List<Transform>();
			for (int i = 0; i < p_Transform.childCount; i++)
			{
				t_Children.Add(p_Transform.GetChild(i));
			}

			// Un-parent all children
			foreach (Transform t_Child in t_Children)
			{
				t_Child.SetParent(null);
			}

			// Change rotation
			p_Transform.localScale = p_Scale;

			// Re-parent all children
			foreach (Transform t_Child in t_Children)
			{
				t_Child.SetParent(p_Transform);
			}
		}
	}
}


