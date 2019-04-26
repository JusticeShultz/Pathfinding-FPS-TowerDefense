﻿using System.Collections;
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
    }

    public void ForceUpdate()
    {
        CreateGrid();
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

            Vector2[] points = new Vector2[(Path.Count + 2)];
            Vector2[] points1 = new Vector2[(points.Length)];
            Vector2[] finalpoints = new Vector2[(points.Length + points1.Length)];
            //Vector3[] points1 = new Vector3[Path.Count + 2];

            points[0] = new Vector2(StartPosition.transform.position.x, StartPosition.transform.position.z);
            //points1[0] = StartPosition.transform.position + StartPosition.transform.right;

            int q = 1;

            foreach (Node a in Path)
            {
                points[q] = new Vector2(a.vPosition.x, a.vPosition.z);
                ++q;
            }

            points[q] = new Vector2(EndPosition.transform.position.x, EndPosition.transform.position.z);

            points1[0] = new Vector2(StartPosition.transform.position.x, StartPosition.transform.position.z);

            for (int i = 1; i < points.Length-1; ++i)
            {
                Vector2 forward = points[i+1] - points[i ];
                Vector2 right = new Vector2(forward.y, -forward.x);
                Vector2 rhs = points[i + 1] + right.normalized;

                //Vector3.Cross(new Vector3(points[i].x, 0, points[i].y), new Vector3(points[i+1].x, 0, points[i+1].y));
                points1[i] =  rhs;
            }

            points1[points1.Length-1] = new Vector2(EndPosition.transform.position.x, EndPosition.transform.position.z);

            //finalpoints = points.Concat(points1).ToArray();

            for(int i = 0; i < points.Length-1; i+=2)
            {
                finalpoints[i] = points[i];
                finalpoints[i+1] = points1[i];
            }

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
            int[] indices = GimmeTriangles(points.Length);//tr.Triangulate();

            // Create the Vector3 vertices
            Vector3[] vertices = new Vector3[points.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vector3(points[i].x, 0, points[i].y);
            }

            // Create the mesh
            Mesh msh = new Mesh();
            msh.vertices = vertices;
            msh.triangles = indices;
            msh.RecalculateNormals();
            msh.RecalculateBounds();
            LineRenderer.mesh = msh;

            /*mesh.vertices = points;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            LineRenderer.mesh = mesh;*/
        }
    }

    public int[] GimmeTriangles(int length)
    {
        //int[] temp = new int[length];
        List<int> temp = new List<int>();
       
        for (int p =0; p< length-1; p+=2)
        {
            temp.Add(p);
            temp.Add(p + 2);
            temp.Add(p + 1);
            temp.Add(p - 1);
            temp.Add(p + 3);
            temp.Add(p + 1);
        }

        return temp.ToArray();
    }

    public bool IsBlocked()
    {
        if (FinalPath == null || NodeArray == null || Blocked)
            return true;

        return false;
    }
}