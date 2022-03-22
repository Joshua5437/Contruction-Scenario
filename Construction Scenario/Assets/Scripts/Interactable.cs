using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
  [HideInInspector]
  public Hand m_ActiveHand = null;
}
