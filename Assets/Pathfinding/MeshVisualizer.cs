using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshVisualizer : MonoBehaviour
{
    public int index;
    public int triangleIndex;

    public bool renderVert;

    private void OnDrawGizmos()
    {
        if(!Application.isPlaying) { return; }
        var mesh = GetComponent<MeshFilter>();

        Gizmos.color = Color.blue / 2;
        Color[] colors = { Color.red, Color.green, Color.blue };

        if (renderVert)
        {
            Gizmos.DrawSphere(transform.TransformPoint(mesh.mesh.vertices[index]), 0.5f);
        }
        else
        {
            for(int i = 0; i < 3; ++i)
            {
                Gizmos.color = colors[i];
                Gizmos.DrawCube(transform.TransformPoint(mesh.mesh.vertices[triangleIndex * 3 + i]), Vector3.one / 2);
            }
        }
    }
}
