using Manus.Polygon;
using Manus.Polygon.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manus.Polygon
{
	[DisallowMultipleComponent]
	[UnityEngine.AddComponentMenu("Manus/Polygon/Polygon Switch")]
	public class PolygonSwitch : MonoBehaviour
	{
		public Polygon[] polygonCharacters = null;
		public KeyCode nextCharacterKey = KeyCode.Greater;
		public KeyCode previousCharacterKey = KeyCode.Less;

		public bool isLocal = false;

		public int currentIndex { get; private set; } = 0;

		private Coroutine m_ToggleRoutine = null;

		private void Start()
		{
			if (gameObject.GetComponent<PolygonSwitchSync>() == null)
			{
				isLocal = true;
				LoadCharacter(0);
			}
		}

		private void Update()
		{
			if (!isLocal)
				return;

			if (Input.GetKeyDown(nextCharacterKey))
				NextCharacter();
			if (Input.GetKeyDown(previousCharacterKey))
				PreviousCharacter();
		}

		/// <summary>
		/// Swap polygon character with another character
		/// </summary>
		/// <param name="p_Index">Polygon character index</param>
		public void LoadCharacter(int p_Index)
		{
			currentIndex = p_Index;
			currentIndex = Mathf.Clamp(currentIndex, 0, polygonCharacters.Length - 1);

			if (m_ToggleRoutine != null) 
				StopCoroutine(m_ToggleRoutine);

			m_ToggleRoutine = StartCoroutine(DelayToggle());
		}

		/// <summary>
		/// Enable the next character
		/// </summary>
		[ContextMenu("Next")]
		private void NextCharacter()
		{
			LoadCharacter(currentIndex == polygonCharacters.Length - 1 ? 0 : currentIndex + 1);
		}

		/// <summary>
		/// Enable the previous character
		/// </summary>
		[ContextMenu("Previous")]
		private void PreviousCharacter()
		{
			LoadCharacter(currentIndex == 0 ? polygonCharacters.Length - 1 : currentIndex - 1);
		}

		/// <summary>
		/// Add a delay to the toggle
		/// </summary>
		/// <returns></returns>
		private IEnumerator DelayToggle()
		{
			polygonCharacters[currentIndex].gameObject.SetActive(true);

			var t_Renderers = polygonCharacters[currentIndex].gameObject.GetComponentsInChildren<Renderer>();
			foreach (var t_Rend in t_Renderers)
			{
				t_Rend.enabled = false;
			}

			// Wait for animation to get started
			yield return new WaitForSeconds(.1f);

			foreach (var t_Rend in t_Renderers)
			{
				t_Rend.enabled = true;
			}

			for (int i = 0; i < polygonCharacters.Length; i++)
			{
				if (i == currentIndex)
					continue;

				polygonCharacters[i].gameObject.SetActive(false);
			}
		}
	}
}