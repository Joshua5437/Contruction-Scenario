using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manus.Polygon
{
	public class PolygonStatusUI : MonoBehaviour
	{
		private CanvasGroup m_CanvasGroup = null;
		private HermesStatusUI m_HermesStatus;

		private Coroutine m_Routine = null;
		private bool m_FoundPolygon = false;
		private bool m_Licensed = false;

		private void OnEnable()
		{
			m_HermesStatus = FindObjectOfType<HermesStatusUI>();
			m_CanvasGroup = GetComponent<CanvasGroup>();
			m_CanvasGroup.alpha = 0;

			m_Routine = StartCoroutine(CheckForPolygon());

			Toggle(PolygonValidation.polygonLicensed);
			PolygonValidation.licenseUpdate += Toggle;
		}

		private void OnDisable()
		{
			if (m_Routine != null)
				StopCoroutine(m_Routine);
			m_Routine = null;
			m_FoundPolygon = false;

			PolygonValidation.licenseUpdate -= Toggle;
		}

		private void Update()
		{
			if (Time.time < 2f)
				return;

			if ((m_HermesStatus == null || m_HermesStatus.connected) && m_FoundPolygon)
			{
				m_CanvasGroup.alpha = m_Licensed ? 0f : 1f;
			} else
			{
				m_CanvasGroup.alpha = 0;
			}
		}

		private void Toggle(bool p_Licensed)
		{
			m_Licensed = p_Licensed;
		}

		private IEnumerator CheckForPolygon()
		{
			while (true)
			{
				yield return new WaitForSeconds(1);

				m_FoundPolygon = false;

				var t_Polygons = FindObjectsOfType<Polygon>();
				foreach (var t_Polygon in t_Polygons)
				{
					if (t_Polygon.enabled && t_Polygon.gameObject.activeInHierarchy)
						m_FoundPolygon = true;
				}
			}
		}
	}
}