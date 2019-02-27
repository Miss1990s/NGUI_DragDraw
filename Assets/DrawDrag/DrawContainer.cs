using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class DrawContainer
{
    List<Vector3> mPointList;
    List<Color> mColorList;
    List<int> LineList;//指向PointList Linde的index

    Mesh mesh;
    MeshFilter filter;
    MeshRenderer render;
    public DrawContainer(GameObject go)
    {
        mesh = new Mesh();
        filter = new MeshFilter();
        render = go.AddMissingComponent<MeshRenderer>();
    }

    public void NewLine()
    {
        LineList.Add(mPointList.Count);
    }

    public void AddPoint(Vector3 pos)
    {
        mPointList.Add(pos);
    }
    public void DeleteLine()
    {

    }
}

