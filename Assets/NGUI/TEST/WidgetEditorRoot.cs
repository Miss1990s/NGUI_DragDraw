using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WidgetEditorRoot : MonoBehaviour {
    public static Transform root;
	// Use this for initialization
	void Start () {
        root = transform;
        transform.gameObject.SetActive(false);
	}

    internal static void StartEdit(Vector3 position)
    {
        root.gameObject.SetActive(true);
        SynPosition(position);
    }

    internal static void SynPosition(Vector3 position)
    {
        root.position = position;
    }

    internal static void EndEdit()
    {
        root.gameObject.SetActive(false);
    }
}
