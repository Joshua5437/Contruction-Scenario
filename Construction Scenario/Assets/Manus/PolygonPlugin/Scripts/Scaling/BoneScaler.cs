using System;
using System.Collections.Generic;
using UnityEngine;

namespace Manus.Polygon
{
	public enum ScaleMode
	{
		Length,
		Percentage
	}

	public enum ScaleAxis
	{
		Length,
		Width,
		Height,
		Thickness,
		All
	}

	public class BoneScaler : IDisposable
	{
		private Transform m_ScaleBone;
		private Transform[] m_ScaleFixBones;

		/// <summary>
		/// Gets the child in the most forward direction
		/// </summary>
		public Transform lookAtChild { get; private set; }

		/// <summary>
		/// Get the default length of the bone, before scaling
		/// </summary>
		public float defaultLength { get; private set; }

		/// <summary>
		/// Get the length of the bone
		/// </summary>
		public float length { get { return Vector3.Distance(m_ScaleBone.transform.position, lookAtChild ? lookAtChild.position : m_ScaleBone.transform.position); } }

		/// <summary>
		/// Get the scale of the bone
		/// </summary>
		public Vector3 scale { get { return m_ScaleBone.localScale; } }

		public BoneScaler(Transform p_Bone, Transform[] p_ChildBones)
		{
			m_ScaleBone = p_Bone;

			// Instantiate scale fix bones
			m_ScaleFixBones = new Transform[] {};
			if (p_ChildBones != null && p_ChildBones.Length > 0)
			{
				var t_ScaleFixList = new List<Transform>();

				foreach (Transform t_Child in p_ChildBones)
				{
					Transform t_ScaleFixBone = new GameObject(t_Child.name + "_scaleFixBone").transform;

					t_ScaleFixBone.SetParent(t_Child.parent, true);
					t_ScaleFixBone.position = t_Child.position;
					t_ScaleFixBone.rotation = m_ScaleBone.rotation;
					t_ScaleFixBone.localScale = Vector3.one;
					
					t_Child.SetParent(t_ScaleFixBone);
					t_ScaleFixList.Add(t_ScaleFixBone);
				}

				m_ScaleFixBones = t_ScaleFixList.ToArray();
			}

			// Calculate the default bone length
			float t_SmallestAngle = float.MaxValue;
			lookAtChild = null;

			foreach (Transform t_Child in m_ScaleFixBones)
			{
				float angleToChild = Vector3.Angle(t_Child.position - p_Bone.position, m_ScaleBone.rotation * Vector3.forward);

				if (angleToChild < t_SmallestAngle && angleToChild < 20)
				{
					t_SmallestAngle = angleToChild;
					lookAtChild = t_Child;
				}
			}

			if (lookAtChild != null)
			{
				defaultLength = Vector3.Project(lookAtChild.position - p_Bone.position, m_ScaleBone.rotation * Vector3.forward).magnitude;
			}
		}

		/// <summary>
		/// Scale bone to new size
		/// </summary>
		/// <param name="p_Scale">The amount to scale it by</param>
		/// <param name="p_Axis">The direction to scale the bone in</param>
		/// <param name="p_ScaleMode">How to scale the bone, lenth makes the bone a specific length, percentage multiplies the size by the scale value</param>
		public void ScaleBone(float p_Scale, ScaleAxis p_Axis, ScaleMode p_ScaleMode)
		{
			float size = p_Scale;

			if (p_Axis == ScaleAxis.Length && p_ScaleMode == ScaleMode.Length)
			{
				if (length == 0) 
					return;
				size = p_Scale / length;
			}

			switch (p_Axis)
			{
				case ScaleAxis.Length:
					m_ScaleBone.localScale = new Vector3(m_ScaleBone.localScale.x, m_ScaleBone.localScale.y, size);
					break;
				case ScaleAxis.Width:
					m_ScaleBone.localScale = new Vector3(size, m_ScaleBone.localScale.y, m_ScaleBone.localScale.z);
					break;
				case ScaleAxis.Height:
					m_ScaleBone.localScale = new Vector3(m_ScaleBone.localScale.x, size, m_ScaleBone.localScale.z);
					break;
				case ScaleAxis.Thickness:
					m_ScaleBone.localScale = new Vector3(size, size, m_ScaleBone.localScale.z);
					break;
				case ScaleAxis.All:
					m_ScaleBone.localScale = new Vector3(size, size, size);
					break;
			}

			foreach (Transform scaleFixBone in m_ScaleFixBones)
			{
				scaleFixBone.localScale = new Vector3(1f / m_ScaleBone.localScale.x, 1f / m_ScaleBone.localScale.y, 1f / m_ScaleBone.localScale.z);
			}
		}

		/// <summary>
		/// Dispose of the scale bone
		/// </summary>
		public void Dispose()
		{
			ScaleBone(1, ScaleAxis.All, ScaleMode.Percentage);
			
			foreach (var t_ScaleFixBone in m_ScaleFixBones)
			{

				for (int i = 0; i < t_ScaleFixBone.childCount; i++)
				{
					t_ScaleFixBone.GetChild(i).SetParent(t_ScaleFixBone.parent, true);
				}

				GameObject.Destroy(t_ScaleFixBone.gameObject);
			}
		}
	}
}