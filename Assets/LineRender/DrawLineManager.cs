using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLineManager : MonoBehaviour {

    public GameObject render;
    // Use this for initialization

    private GameObject ins;
    private DrawLineRenderDemo lineRenderer;

    // Update is called once per frame
    void Update () {
		if(Input.GetMouseButtonDown(0))
        {
            if(ins == null)
            {
                ins = GameObject.Instantiate(render, Vector3.zero, Quaternion.Euler(Vector3.zero), null);
                lineRenderer = ins.AddMissingComponent<DrawLineRenderDemo>();
            }

        }
        if (Input.GetMouseButtonUp(0))
        {
            lineRenderer.enabled = false;
            ins = null;
        }

    }
}
