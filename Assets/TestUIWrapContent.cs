using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUIWrapContent : MonoBehaviour {
    UIWrapContent wrapContent;
    public int min;
    public int max;
	// Use this for initialization
	void Start () {

    }

    public static void myOnInitItem(GameObject go, int wrapIndex, int realIndex)
    {
        Debug.Log("---------" + go.name + " wrapIndex " + wrapIndex + " realIndex " + realIndex);
        if (realIndex < 0 || realIndex > 5)
            go.SetActive(false);
        else
            go.SetActive(true);
    }

    // Update is called once per frame
    void Update () {

    }
}
