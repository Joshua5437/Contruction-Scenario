using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using HProt = Hermes.Protocol;

namespace Manus.Polygon
{
	[DisallowMultipleComponent]
	[UnityEngine.AddComponentMenu("Manus/Polygon/Polygon")]
	public class Polygon : MonoBehaviour
	{
		public Animator animator;
		public SkeletonBoneReferences boneReferences;

		/// <summary>
		/// Returns true if the animator attached to this gameObject has a valid humanoid avatar
		/// </summary>
		public bool isAnimatorValid
		{
			get
			{
				if (animator == null || animator.avatar == null || !animator.avatar.isValid || !animator.avatar.isHuman)
				{
					Debug.LogWarning((animator == null ? "No animator assigned" : "Assigned animator does not have a valid human avatar") + ", trying to find one");
					animator = FindValidAnimatorInHierarchy() ?? animator;

					if (animator == null) return false;
				}

				return true;
			}
		}

		#region Monobehaviour Callbacks

		private void Awake()
		{
			PolygonValidation.polygonRoutine = StartCoroutine(PolygonValidation.AddPolygonCoroutine());
		}

		private void Reset()
		{
			boneReferences?.Clear();
		}

		#endregion

		#region Private Methods

		private Animator FindValidAnimatorInHierarchy()
		{
			Animator[] t_AnimatorsInHierarchy = GetComponentsInChildren<Animator>();
			foreach (Animator t_Ac in t_AnimatorsInHierarchy)
			{
				if (t_Ac.avatar != null && t_Ac.avatar.isValid && t_Ac.avatar.isHuman)
				{
					Debug.Log("Valid animator found");
					return t_Ac;
				}
			}

			Debug.LogWarning(t_AnimatorsInHierarchy.Length == 0 ? "No animators found in hierarchy" : "No animator found with a valid human avatar, go to the settings and set the 'Animation Type' to 'Humanoid'");

			return null;
		}

		#endregion
	}
}