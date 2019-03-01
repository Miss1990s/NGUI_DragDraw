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
    Color mColor = Color.green;//当前颜色;

    Vector3 mLastPoint = -1 * Vector3.one;//上一个点
    Vector3 mLastVert1, mLastVert2;
    Vector3 mLastPirpendicular;//上一个垂线

    

    public DrawContainer(GameObject go , UIPanel panel, int maxLineCount = 100, int maxVertexCount = 10000)
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
        mRender.sortingOrder = panel.sortingOrder + 1;
        mRender.material.renderQueue = 3000;
    }
    /// <summary>
    /// 添加新线
    /// </summary>
    /// <param name="depth">线的深度</param>
    public void NewLine(int depth)
    {
        if (depth < 1) { Debug.LogError("Depth must >= 1"); return; }
        m_1_Depth = 1.0f - 1.0f/depth;
        mLastPoint = -1 * Vector3.one;
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
        if (mLastPoint == -1 * Vector3.one)
        {
            mLastPoint = pos;
            return;
        }
        Vector3 lineDir = pos - mLastPoint;
        Vector3 pirpendicular = GetPirpendicular(lineDir);
        //Add the first group of triangles for current line.
        if(mLineStack.Peek() == mVertexList.size)
        {
            Add2Vertices(mLastPoint, pirpendicular);
        }
        else if(IsBigTurnAngle(lineDir, mLastPirpendicular))//Check the turn angle, adds more vertices for big angle.
        {
            Add2Vertices(mLastPoint, pirpendicular);
        }
        
        if(Vector3.Angle(pirpendicular,mLastPirpendicular)<0)
        {
            Revise2Vertices(pos, pirpendicular);
        }
        else
        {
            Add2Vertices(pos, pirpendicular);
        }
        mLastPoint = pos;
        mLastPirpendicular = pirpendicular;

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

    private void Add2Vertices(Vector3 screenPos, Vector3 pirpendicular)
    {
        mLastVert1 = screenPos + pirpendicular * 0.5f * mWidth;
        mLastVert2 = screenPos - pirpendicular * 0.5f * mWidth;
        Vector3 pos1 = UICamera.currentCamera.ScreenToViewportPoint(mLastVert1);
        Vector3 pos2 = UICamera.currentCamera.ScreenToViewportPoint(mLastVert2);
        pos1 *= 2;pos1 -= Vector3.one; pos1.y = -pos1.y;
        pos2 *= 2;pos2 -= Vector3.one; pos2.y = -pos2.y;

        pos1.z = m_1_Depth;
        pos2.z = m_1_Depth;

        //Debug.LogFormat("pos={0},pos1={1},pos2={2}",pos, pos1, pos2);
        mVertexList.Add(pos1);
        mVertexList.Add(pos2);

        mColorList.Add(mColor);
        mColorList.Add(mColor);
        Add2Triangles();
    }
    private void Revise2Vertices(Vector3 screenPos, Vector3 pirpendicular)
    {
        mLastVert1 = screenPos + pirpendicular * 0.5f * mWidth;
        mLastVert2 = screenPos - pirpendicular * 0.5f * mWidth;
        Vector3 pos1 = UICamera.currentCamera.ScreenToViewportPoint(mLastVert1);
        Vector3 pos2 = UICamera.currentCamera.ScreenToViewportPoint(mLastVert2);
        pos1 *= 2; pos1 -= Vector3.one; pos1.y = -pos1.y;
        pos2 *= 2; pos2 -= Vector3.one; pos2.y = -pos2.y;

        pos1.z = m_1_Depth;
        pos2.z = m_1_Depth;

        //Debug.LogFormat("pos={0},pos1={1},pos2={2}",pos, pos1, pos2);
        mVertexList[mVertexList.size - 2] = pos1;
        mVertexList[mVertexList.size - 1] = pos2;

    }

    private void Add2Triangles()
    {
        int size = mVertexList.size;
        if (size < 4) {  return; }
        if (mLineStack.Contains(size - 2)) return;

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
    /// <param name="lineDir"></param>
    /// <returns></returns>
    private Vector3 GetPirpendicular(Vector3 lineDir)
    {
        
        Vector3 pirpendir = Vector3.Cross(lineDir, new Vector3(0, 0, 1));
        return Vector3.Normalize(pirpendir);
    }
    /// <summary>
    /// 根据线段走向和前一个垂线方向夹角是否太小
    /// </summary>
    /// <param name="lineDir">线段走向</param>
    /// <param name="pirpendir">前一个垂线方向</param>
    /// <returns></returns>
    private bool IsBigTurnAngle(Vector3 lineDir, Vector3 pirpendir)
    {
        float angle = Math.Abs(Vector3.Angle(lineDir, pirpendir));
        if(angle < 45 )
        {
            return true ;
        }
        else if(angle > 135)
        {
            return true;
        }
        return false;
    }
}

