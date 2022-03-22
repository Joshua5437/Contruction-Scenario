using Valve.VR;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hand : MonoBehaviour
{
  public SteamVR_Action_Boolean m_GrabAction = null;

  private SteamVR_Behaviour_Pose m_Pose = null;
  private FixedJoint m_Joint = null;

  private Interactable m_CurrentInteractable = null;
  public List<Interactable> m_ContactInteractables = new List<Interactable>();

  private void Awake()
  {
    m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
    m_Joint = GetComponent<FixedJoint>();
  }

  private void Update()
  {
    // Down
    if(m_GrabAction.GetStateDown(m_Pose.inputSource))
    {
      print(m_Pose.inputSource + "Trigger Down");
      Pickup();
    }

    // Up
    if(m_GrabAction.GetStateUp(m_Pose.inputSource))
    {
      print(m_Pose.inputSource + "Trigger Up");
      Drop();
    }

  }

  private void OnTriggerEnter(Collider other)
  {
    if(!other.gameObject.CompareTag("Interactable"))
      return;

    m_ContactInteractables.Add(other.gameObject.GetComponent<Interactable>());
  }

  private void OnTriggerExit(Collider other)
  {
    if(!other.gameObject.CompareTag("Interactable"))
      return;

    m_ContactInteractables.Remove(other.gameObject.GetComponent<Interactable>());
  }

  public void Pickup()
  {
    // Get GetNearestInteractable
    m_CurrentInteractable = GetNearestInteractable();

    // Null Check
    if(!m_CurrentInteractable)
      return;

    // Already held, Check
    if(m_CurrentInteractable.m_ActiveHand)
      m_CurrentInteractable.m_ActiveHand.Drop();

    // position
    m_CurrentInteractable.transform.position = transform.position;

    // Attach
    Rigidbody targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();
    m_Joint.connectedBody = targetBody;

    // Set active hand
    m_CurrentInteractable.m_ActiveHand = this;

  }

  public void Drop()
  {
    // Null Check
    if(!m_CurrentInteractable)
      return;

    // Apply velocity
    Rigidbody targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();
    targetBody.velocity = m_Pose.GetVelocity();
    targetBody.angularVelocity = m_Pose.GetAngularVelocity();

    // Detach
    m_Joint.connectedBody = null;

    // Clear
    m_CurrentInteractable.m_ActiveHand = null;
    m_CurrentInteractable = null;

  }

  private Interactable GetNearestInteractable()
  {
    Interactable nearest = null;
    float minDistance = float.MaxValue;
    float distance = 0.0f;

    foreach(Interactable interactable in m_ContactInteractables)
    {
      distance = (interactable.transform.position - transform.position).sqrMagnitude;

      if(distance < minDistance)
      {
        minDistance = distance;
        nearest = interactable;
      }
    }
    return nearest;
  }
}
