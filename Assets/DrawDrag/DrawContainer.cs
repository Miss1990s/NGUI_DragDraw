using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawContainer
{
    BetterList<Vector3> mVertexList;
    BetterList<Color> mColorList;
    Stack<int> mLineStack;//记录每根线在 mVertexList 中开始的 index
    List<int> mTriangles;//记录三角形

    Mesh mMesh;
    MeshFilter mFilter;
    MeshRenderer mRender;

    float m_1_Depth;//当前线的深度值
    int mWidth = 2;//当前线的宽度；
    Color mColor;//当前颜色;
    Vector3 mLastPoint;//上一个点

    

    public DrawContainer(GameObject go , int maxLineCount = 100, int maxVertexCount = 10000)
    {
        mMesh = new Mesh();
        mMesh.hideFlags = HideFlags.DontSave;
        mMesh.name = go.name+"LineMesh";
        mMesh.MarkDynamic();

        mFilter = go.AddMissingComponent<MeshFilter>();
        mRender = go.AddMissingComponent<MeshRenderer>();

        mVertexList = new BetterList<Vector3>();
        mColorList = new BetterList<Color>();
        mLineStack = new Stack<int>();
        mTriangles = new List<int>();

        mLineStack.Push(0);
        Shader shader = Shader.Find("Unlit/Transparent DrawLine");
        mRender.material = new Material(shader);
    }
    /// <summary>
    /// 添加新线
    /// </summary>
    /// <param name="depth">线的深度</param>
    public void NewLine(int depth)
    {
        m_1_Depth = 1.0f/depth;
        if (mLineStack.Peek() == mVertexList.size) return; //当前已经是一根新线
        mLineStack.Push(mVertexList.size);
        Debug.LogError("NewLine " + mLineStack.Count);
    }

    /// <summary>
    /// 根据采样点，添加两个世界坐标顶点
    /// </summary>
    /// <param name="pos">屏幕坐标</param>
    public void AddPoint()
    {

        Vector3 pos = UICamera.lastEventPosition;
        
        if (mLastPoint == pos) return;
        mLastPoint = pos;

        
        Debug.Log(pos);

        pos = UICamera.currentCamera.ScreenToViewportPoint(pos);

        pos.z = m_1_Depth;
        Debug.Log(pos);
        Vector3 lineDir = GetRightLineDirection(pos);

        mVertexList.Add(pos + lineDir * 0.5f * mWidth);
        mVertexList.Add(pos - lineDir * 0.5f * mWidth);
        mColorList.Add(Color.green);
        mColorList.Add(Color.green);
        Add2Triangles();
        RenderLines();
    }

    public void DeleteLine()
    {
        int vertexCount = mVertexList.size;
        int startIndex = mLineStack.Pop();
        for (int i=vertexCount-1;i>= startIndex; i--)
        {
            mVertexList.Pop();
        }
        DeleteTriangles(vertexCount - 2 - startIndex);
        RenderLines();
    }

    private void Add2Triangles()
    {
        int size = mVertexList.size;
        if (size < 4) {  return; }
        
        
        mTriangles.Add(size - 2);
        mTriangles.Add(size - 4);
        mTriangles.Add(size - 3);

        mTriangles.Add(size - 1);
        mTriangles.Add(size - 2);
        mTriangles.Add(size - 3);
    }
    private void DeleteTriangles(int count)
    {
        int size = mVertexList.size;
        mTriangles.RemoveRange(size - count * 3,count *3);
    }

    private void RenderLines()
    {
        if(mMesh.vertexCount != mVertexList.size)
        mMesh.Clear();
        mMesh.vertices = mVertexList.ToArray();
        int indexCount = (mVertexList.size - 2) * 3;
        mMesh.colors = (mColorList.ToArray());
        mMesh.SetTriangles(mTriangles,0);
        mFilter.mesh = mMesh;
    }

    /// <summary>
    /// 根据当前采样点和前一个采样点计算线方向的平面归一化垂线
    /// </summary>
    /// <param name="rpos"></param>
    /// <returns></returns>
    private Vector3 GetRightLineDirection(Vector3 rpos)
    {
        if (mLineStack.Peek() >= mVertexList.size)
        {
            return Vector3.one;
        }
        Vector3 lastPos = 0.5f * (mVertexList[mVertexList.size - 1] + mVertexList[mVertexList.size - 2]);
        Vector3 lineDir = (rpos - lastPos);
        Vector3 penDir = Vector3.Cross(lineDir, new Vector3(0, 0, 1));

        return Vector3.Normalize(penDir);
    }
}

