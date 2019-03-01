using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WidgetHelper : MonoBehaviour {
    [ContextMenu("Execute")]
    // Use this for initialization
    void Start () {
        UIWidget[] ws = GetComponentsInChildren<UIWidget>();
        for (int i = 0; i < ws.Length; i++)
            ws[i].depth = i + 1;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
