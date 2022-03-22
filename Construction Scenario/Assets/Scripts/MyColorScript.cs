using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyColorScript : MonoBehaviour
{
    public GameObject plane;
    private MeshRenderer myRenderer;

    private void Start()
    {
        Debug.Log("We made it here!");
        //Destroy(GameObject,3f);
    }

    public void OnClickColor()
    {
        myRenderer = plane.GetComponent<MeshRenderer>();
        myRenderer.enabled = !myRenderer.enabled;
    }

    /*
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    */
}
