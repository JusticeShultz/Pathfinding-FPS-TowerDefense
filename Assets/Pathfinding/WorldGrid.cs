using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum Style { Entire, Path, PathBuildings, None}

public class WorldGrid : MonoBehaviour
{
    public static WorldGrid GridObj;
    public Transform StartPosition;
    public Transform EndPosition;
    public LayerMask WallMask;
    public LayerMask YMask;
    public Vector2 vGridWorldSize;
    public float fNodeRadius;
    public float fDistanceBetweenNodes;
    public Style GridStyle;
    public bool CanGoDiagonal = true;
    public List<Node> FinalPath;
    public Vector3 DrawOffset;
    public MeshFilter LineRenderer;
    public bool ShouldDebug = true;
    public float LinePrecision = 0.01f;
    public float DrawInterval = 0.1f;

    bool NeedsUpdate = false;

    //public LineRenderer lineRenderer;

    [HideInInspector] public List<Node> Path = new List<Node>();
    [HideInInspector] public bool Blocked = false;

    Node[,] NodeArray;
    float fNodeDiameter;
    int iGridSizeX, iGridSizeY;

    private void Start()
    {
        GridObj = this;
        fNodeDiameter = fNodeRadius * 2;
        iGridSizeX = Mathf.RoundToInt(vGridWorldSize.x / fNodeDiameter);
        iGridSizeY = Mathf.RoundToInt(vGridWorldSize.y / fNodeDiameter);
        CreateGrid();
        NeedsUpdate = true;
    }

    public void ForceUpdate()
    {
        CreateGrid();
        NeedsUpdate = true;
    }

    void CreateGrid()
    {
        NodeArray = new Node[iGridSizeX, iGridSizeY];
        Vector3 bottomLeft = transform.position - Vector3.right * vGridWorldSize.x / 2 - Vector3.forward * vGridWorldSize.y / 2;
        for (int x = 0; x < iGridSizeX; x++)
        {
            for (int y = 0; y < iGridSizeY; y++)
            {
                Vector3 worldPoint = bottomLeft + Vector3.right * (x * fNodeDiameter + fNodeRadius) + Vector3.forward * (y * fNodeDiameter + fNodeRadius);
                bool Wall = true;

                if (Physics.CheckSphere(worldPoint, fNodeRadius, WallMask))
                {
                    Wall = false;
                }

                NodeArray[x, y] = new Node(Wall, worldPoint, x, y);
            }
        }
    }

    public List<Node> GetNeighboringNodes(Node a_NeighborNode)
    {
        List<Node> NeighborList = new List<Node>();
        int icheckX;
        int icheckY;

        //Left
        icheckX = a_NeighborNode.iGridX + 1;
        icheckY = a_NeighborNode.iGridY;
        if (icheckX >= 0 && icheckX < iGridSizeX)
        {
            if (icheckY >= 0 && icheckY < iGridSizeY)
            {
                NeighborList.Add(NodeArray[icheckX, icheckY]);
            }
        }

        //Right
        icheckX = a_NeighborNode.iGridX - 1;
        icheckY = a_NeighborNode.iGridY;
        if (icheckX >= 0 && icheckX < iGridSizeX)
        {
            if (icheckY >= 0 && icheckY < iGridSizeY)
            {
                NeighborList.Add(NodeArray[icheckX, icheckY]);
            }
        }

        //Top
        icheckX = a_NeighborNode.iGridX;
        icheckY = a_NeighborNode.iGridY + 1;
        if (icheckX >= 0 && icheckX < iGridSizeX)
        {
            if (icheckY >= 0 && icheckY < iGridSizeY)
            {
                NeighborList.Add(NodeArray[icheckX, icheckY]);
            }
        }

        //Bottom
        icheckX = a_NeighborNode.iGridX;
        icheckY = a_NeighborNode.iGridY - 1;
        if (icheckX >= 0 && icheckX < iGridSizeX)
        {
            if (icheckY >= 0 && icheckY < iGridSizeY)
            {
                NeighborList.Add(NodeArray[icheckX, icheckY]);
            }
        }

        if (CanGoDiagonal)
        {
            //Top Left
            icheckX = a_NeighborNode.iGridX + 1;
            icheckY = a_NeighborNode.iGridY + 1;
            if (icheckX >= 0 && icheckX < iGridSizeX)
            {
                if (icheckY >= 0 && icheckY < iGridSizeY)
                {
                    NeighborList.Add(NodeArray[icheckX, icheckY]);
                }
            }

            //Top Right
            icheckX = a_NeighborNode.iGridX - 1;
            icheckY = a_NeighborNode.iGridY + 1;
            if (icheckX >= 0 && icheckX < iGridSizeX)
            {
                if (icheckY >= 0 && icheckY < iGridSizeY)
                {
                    NeighborList.Add(NodeArray[icheckX, icheckY]);
                }
            }

            //Bottom Left
            icheckX = a_NeighborNode.iGridX + 1;
            icheckY = a_NeighborNode.iGridY - 1;
            if (icheckX >= 0 && icheckX < iGridSizeX)
            {
                if (icheckY >= 0 && icheckY < iGridSizeY)
                {
                    NeighborList.Add(NodeArray[icheckX, icheckY]);
                }
            }

            //Bottom Right
            icheckX = a_NeighborNode.iGridX - 1;
            icheckY = a_NeighborNode.iGridY - 1;
            if (icheckX >= 0 && icheckX < iGridSizeX)
            {
                if (icheckY >= 0 && icheckY < iGridSizeY)
                {
                    NeighborList.Add(NodeArray[icheckX, icheckY]);
                }
            }
        }

        return NeighborList;
    }

    public Node NodeFromWorldPoint(Vector3 a_vWorldPos)
    {
        float ixPos = ((a_vWorldPos.x + vGridWorldSize.x / 2) / vGridWorldSize.x);
        float iyPos = ((a_vWorldPos.z + vGridWorldSize.y / 2) / vGridWorldSize.y);

        ixPos = Mathf.Clamp01(ixPos);
        iyPos = Mathf.Clamp01(iyPos);

        int ix = Mathf.RoundToInt((iGridSizeX - 1) * ixPos);
        int iy = Mathf.RoundToInt((iGridSizeY - 1) * iyPos);

        return NodeArray[ix, iy];
    }

    private void OnDrawGizmos()
    {
        if (GridStyle == Style.None) return;

        Gizmos.DrawWireCube(transform.position, new Vector3(vGridWorldSize.x, 1, vGridWorldSize.y));

        if (GridStyle == Style.Entire)
        {
            if (NodeArray != null)
            {
                if (FinalPath == null) { return; }
                Path = new List<Node>(FinalPath);

                foreach (Node n in NodeArray)
                {
                    RaycastHit hit;

                    if (Physics.Raycast(n.vPosition + (Vector3.up * 100), -Vector3.up, out hit, Mathf.Infinity, YMask))
                    {
                        n.vPosition = hit.point + (Vector3.up * 0.5f);
                    }

                    if (n.bIsWall)
                    {
                        Gizmos.color = Color.white;
                    }
                    else
                    {
                        Gizmos.color = Color.yellow;
                    }

                    if (FinalPath != null)
                    {
                        if (FinalPath.Contains(n))
                        {
                            Gizmos.color = Color.red;
                        }

                    }

                    Gizmos.DrawCube(n.vPosition, Vector3.one * (fNodeDiameter - fDistanceBetweenNodes) + DrawOffset);
                }
            }
        }

        if (GridStyle == Style.Path)
        {
            if (NodeArray != null)
            {
                if (Path.Count > 0 && Path != null)
                    Path.Clear();

                if (FinalPath == null) { return; }
                Path = new List<Node>(FinalPath);

                foreach (Node n in NodeArray)
                {
                    if (FinalPath != null)
                    {
                        if (FinalPath.Contains(n))
                        {
                            RaycastHit hit;

                            if (Physics.Raycast(n.vPosition + (Vector3.up * 100), -Vector3.up, out hit, Mathf.Infinity, YMask))
                            {
                                n.vPosition = hit.point + (Vector3.up * 0.5f);
                            }

                            Gizmos.color = new Color(1, 0, 0, 0.5f);
                            Gizmos.DrawCube(n.vPosition, Vector3.one * (fNodeDiameter - fDistanceBetweenNodes) + DrawOffset);
                            continue;
                        }
                    }
                }
            }
        }

        if (GridStyle == Style.PathBuildings)
        {
            if (NodeArray != null)
            {
                if (FinalPath == null) { return; }
                Path = new List<Node>(FinalPath);

                foreach (Node n in NodeArray)
                {
                    RaycastHit hit;

                    if (Physics.Raycast(n.vPosition + (Vector3.up * 100), -Vector3.up, out hit, Mathf.Infinity, YMask))
                    {
                        n.vPosition = hit.point + (Vector3.up * 0.5f);
                    }

                    if (FinalPath != null)
                    {
                        if (FinalPath.Contains(n))
                        {
                            Gizmos.color = Color.red;
                            Gizmos.DrawCube(n.vPosition, Vector3.one * (fNodeDiameter - fDistanceBetweenNodes) + DrawOffset);
                            continue;
                        }
                    }

                    if (!n.bIsWall)
                    {
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawCube(n.vPosition, Vector3.one * (fNodeDiameter - fDistanceBetweenNodes) + DrawOffset);
                        continue;
                    }
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (!NeedsUpdate) return;
        else NeedsUpdate = false;

        #region ignore
        if (NodeArray != null)
        {
            if (FinalPath == null) return;

            Path = new List<Node>(FinalPath);

            foreach (Node n in NodeArray)
            {
                if (FinalPath != null)
                {
                    if (FinalPath.Contains(n))
                    {
                        RaycastHit hit;

                        if (Physics.Raycast(n.vPosition + (Vector3.up * 100), -Vector3.up, out hit, Mathf.Infinity, YMask))
                        {
                            n.vPosition = hit.point + (Vector3.up * 0.5f);
                        }
                    }
                }
            }
            #endregion

            List<Vector2> LerpedPath = new List<Vector2>();

            /*int uh = 0;
            foreach (Node a in Path)
            {
                LerpedPath[uh] = new Vector2(a.vPosition.x, a.vPosition.z);
                ++uh;
            }*/

            Vector2 FakeObject = new Vector2(StartPosition.transform.position.x, StartPosition.transform.position.z);

            foreach (Node n in Path)
            {
                int justInCase = 0;

                while(Vector2.Distance(new Vector2(n.vPosition.x, n.vPosition.z), FakeObject) >= LinePrecision)
                {
                    //FakeObject = Vector2.Lerp(FakeObject, new Vector2(n.vPosition.x, n.vPosition.z), DrawInterval);
                    FakeObject = Vector3.MoveTowards(FakeObject, new Vector2(n.vPosition.x, n.vPosition.z), DrawInterval);
                    LerpedPath.Add(FakeObject);

                    ++justInCase;

                    if (justInCase > 2500) break;
                }
            }

            Vector2[] tPoints = new Vector2[(LerpedPath.Count + 1) * 2];

            for(int i = 0; i < LerpedPath.Count * 2; i+=2)
            {
                int whereWeAt = i / 2;
                try
                {
                    Vector3 s = Vector3.up;
                    Vector3 f = Vector3.zero;

                    if(ShouldDebug)
                        Debug.DrawRay(LerpedPath[whereWeAt], Vector3.down, Color.white);
                    Vector2 dir = new Vector2(LerpedPath[whereWeAt + 1].x - LerpedPath[whereWeAt].x, LerpedPath[whereWeAt + 1].y - LerpedPath[whereWeAt].y);
                    //Debug.Log(dir);
                    if (ShouldDebug)
                        Debug.DrawRay(LerpedPath[whereWeAt], dir.normalized * 15.0f, Color.magenta);
                    Vector2 perp = new Vector2(dir.y, -dir.x) * 10.0f;
                    tPoints[i] = new Vector2(LerpedPath[whereWeAt].x, LerpedPath[whereWeAt].y) + perp.normalized;
                    tPoints[i + 1] = new Vector2(LerpedPath[whereWeAt].x, LerpedPath[whereWeAt].y) - perp.normalized;

                    if (ShouldDebug)
                    {
                        Debug.DrawRay(LerpedPath[whereWeAt], new Vector3(perp.x, 0.0f, perp.y), Color.blue);
                        Debug.DrawRay(LerpedPath[whereWeAt], -(new Vector3(perp.x, 0.0f, perp.y)), Color.green);
                    }
                }
                catch
                {
                    //Debug.LogError("i:" + i + "\nwhereWeAt: " + whereWeAt);
                }
            }

            //points[0] = new Vector2(StartPosition.transform.position.x, StartPosition.transform.position.z) -
            //            new Vector2(StartPosition.transform.right.x, StartPosition.transform.right.z);
            //points1[0] = new Vector2(StartPosition.transform.position.x, StartPosition.transform.position.z) +
            //            new Vector2(StartPosition.transform.right.x, StartPosition.transform.right.z);
            //points[points.Length-1] = new Vector2(EndPosition.transform.position.x, EndPosition.transform.position.z) -
            //            new Vector2(EndPosition.transform.right.x, EndPosition.transform.right.z);
            //points1[points1.Length-1] = new Vector2(EndPosition.transform.position.x, EndPosition.transform.position.z) +
            //            new Vector2(EndPosition.transform.right.x, EndPosition.transform.right.z);

            var things = tPoints;

            if (ShouldDebug)
                for (int i = 0; i < things.Length; ++i)
                {
                    Debug.DrawRay(new Vector3(things[i].x, 0.0f, things[i].y), Vector3.up * 10.0f, Color.red);
                }

            //Triangulator test = new Triangulator(tPoints);
            //int[] indices = test.Triangulate();

            Vector3[] vertices = new Vector3[tPoints.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vector3(tPoints[i].x, 0, tPoints[i].y);
            }

            Mesh msh = new Mesh();

            msh.vertices = vertices;
            msh.triangles = GimmeTriangles(tPoints.Length);

            msh.RecalculateBounds();
            msh.RecalculateNormals();
            msh.RecalculateTangents();
            LineRenderer.mesh = msh;
        }
    }

    public int[] GimmeTriangles(int length)
    {
        //Debug.Log(length);
        //int[] temp = new int[length];
        List<int> temp = new List<int>();

        for (int ti = 0; ti < length - 2; ti += 2)
        {
            //first
            temp.Add(ti);
            temp.Add(ti + 1);
            temp.Add(ti + 3);
            //second
            temp.Add(ti);
            temp.Add(ti + 3);
            temp.Add(ti + 2);
        }

        int[] tris = temp.ToArray();//.Reverse().ToArray();
        return tris;
    }

    public bool IsBlocked()
    {
        if (FinalPath == null || NodeArray == null || Blocked)
            return true;

        return false;
    }
}

            ///first
            //temp[ti] = vi;
            //temp[ti + 1] = vi + 3;
            //temp[ti + 2] = vi + 1;
            ///second
            //temp[ti + 3] = vi;
            //temp[ti + 4] = vi + 2;
            //temp[ti + 5] = vi + 3;
/*
            temp.Add(p);
            temp.Add(p + 2);
            temp.Add(p + 1);
            temp.Add(p - 1);
            temp.Add(p + 3);
            temp.Add(p + 1);
            
    //if (p + 6 > length)
            //{

            //    //temp.Add(p);
            //    //temp.Add(p + 2);
            //    //temp.Add(p - 1);
            //    break;
            //}

            //temp.Add(p);
            //temp.Add(p + 2);
            //temp.Add(p + 1);

            //temp.Add(p - 1);
            //temp.Add(p + 3);
            //temp.Add(p + 1);

            
            _ > _
            ^ + v
            _ > _

 */
            /*int e = 1;

            foreach (Node v in Path)
            {
                points1[e] = v.vPosition + new Vector3(1, 0, 1);
                ++e;
            }*/

            //points1[e] = EndPosition.transform.position + EndPosition.transform.right;

            /*for (int i = q; q + e < q; ++q)
            {
                points[q] = points1[e];
                points1[e] = Vector3.zero;
            }*/

            /*lineRenderer.positionCount = points.Length;

            for (int i = 0; i < points.Length; i++)
            {
                lineRenderer.SetPosition(i, points[i]);
            }*/

            //Triangulator tr = new Triangulator(points);
            
            /*mesh.vertices = points;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            LineRenderer.mesh = mesh;*/