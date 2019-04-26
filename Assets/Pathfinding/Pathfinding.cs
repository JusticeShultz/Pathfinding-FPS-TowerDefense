using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    WorldGrid GridReference;
    public Transform StartPosition;
    public Transform TargetPosition;
    Vector3 LastPoint1;
    Vector3 LastPoint2;
    bool LastDiagonal;
    
    private void Awake()
    {
        GridReference = GetComponent<WorldGrid>();
    }

    private void Update()
    {
        if (StartPosition.position != LastPoint1 || TargetPosition.position != LastPoint2 || GridReference.CanGoDiagonal != LastDiagonal)
        {
            GridReference.ForceUpdate();
            FindPath(StartPosition.position, TargetPosition.position);
            LastPoint1 = StartPosition.position;
            LastPoint2 = TargetPosition.position;
            LastDiagonal = GridReference.CanGoDiagonal;
        }
    }

    public void ForceUpdate()
    {
        GridReference.ForceUpdate();
        FindPath(StartPosition.position, TargetPosition.position);
        LastPoint1 = StartPosition.position;
        LastPoint2 = TargetPosition.position;
        LastDiagonal = GridReference.CanGoDiagonal;
    }

    void FindPath(Vector3 a_StartPos, Vector3 a_TargetPos)
    {
        Node StartNode = GridReference.NodeFromWorldPoint(a_StartPos);
        Node TargetNode = GridReference.NodeFromWorldPoint(a_TargetPos);

        List<Node> OpenList = new List<Node>();
        HashSet<Node> ClosedList = new HashSet<Node>();

        OpenList.Add(StartNode);

        GridReference.FinalPath = null;

        while (OpenList.Count > 0)
        {
            Node CurrentNode = OpenList[0];
            for (int i = 1; i < OpenList.Count; i++)
            {
                if (OpenList[i].FCost < CurrentNode.FCost || OpenList[i].FCost == CurrentNode.FCost && OpenList[i].ihCost < CurrentNode.ihCost)
                {
                    CurrentNode = OpenList[i];
                    
                }
            }

            OpenList.Remove(CurrentNode);
            ClosedList.Add(CurrentNode);

            if (CurrentNode == TargetNode)
            {
                GetFinalPath(StartNode, TargetNode);
                //Debug.Log("REEEEE I'M BLOCKED");
                //WorldGrid.GridObj.Blocked = true;
                break;
            }
       

            foreach (Node NeighborNode in GridReference.GetNeighboringNodes(CurrentNode))
            {
                if (!NeighborNode.bIsWall || ClosedList.Contains(NeighborNode))
                {
                    continue;
                }

                int MoveCost = CurrentNode.igCost + GetManhattenDistance(CurrentNode, NeighborNode);

                if (MoveCost < NeighborNode.igCost || !OpenList.Contains(NeighborNode))
                {
                    NeighborNode.igCost = MoveCost;
                    NeighborNode.ihCost = GetManhattenDistance(NeighborNode, TargetNode);
                    NeighborNode.ParentNode = CurrentNode;

                    if (!OpenList.Contains(NeighborNode))
                    {
                        OpenList.Add(NeighborNode);
                    }
                }
            }
        }
    }

    void GetFinalPath(Node a_StartingNode, Node a_EndNode)
    {
        List<Node> FinalPath = new List<Node>();
        Node CurrentNode = a_EndNode;

        while (CurrentNode != a_StartingNode)
        {
            FinalPath.Add(CurrentNode);
            CurrentNode = CurrentNode.ParentNode;
        }

        FinalPath.Reverse();

        GridReference.FinalPath = FinalPath;
    }

    int GetManhattenDistance(Node a_nodeA, Node a_nodeB)
    {
        int ix = Mathf.Abs(a_nodeA.iGridX - a_nodeB.iGridX);
        int iy = Mathf.Abs(a_nodeA.iGridY - a_nodeB.iGridY);
        return ix + iy;
    }
}