using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UpdateVRView : MonoBehaviour
{
    public Button N_Button;
    public Camera VR_Camera;

    void Start()
    {
        Button btn = N_Button.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
      VR_Camera.rect = new Rect(0,0,0.7f,1);
    }
}
