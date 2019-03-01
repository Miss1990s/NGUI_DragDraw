using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawDragPanel : MonoBehaviour {
    DragContainer mDragContainer;
    DrawContainer mDrawContainer;

    public int depth = 1;
    void Awake()
    {
        mDrawContainer = new DrawContainer(gameObject);
    }    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

    void OnDragStart()
    {

        
        mDrawContainer.NewLine(depth++);
    }
    void OnDrag()
    {
        mDrawContainer.AddPoint();
    }

}
