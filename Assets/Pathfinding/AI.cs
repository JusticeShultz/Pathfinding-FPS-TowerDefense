using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public float NodeSwitchDistance = 0.5f;
    public float MovementSpeed = 10.0f;
    public float Health = 2;

    private List<Node> localPath = new List<Node>();
    private Rigidbody rb;

    [HideInInspector]public bool Targeted = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        localPath = new List<Node>(WorldGrid.GridObj.Path);
    }

    void Update()
    {
        if (Health <= 0) Destroy(gameObject);

        if (localPath.Count > 1)
        {
            if (Vector3.Distance(transform.position, localPath[0].vPosition) < NodeSwitchDistance)
            {
                localPath.Remove(localPath[0]);
            }

            //Vector3 lel = new Vector3();
            //lel = localPath[0].vPosition;
            //lel.y = transform.position.y;
            //lel.z = localPath[0].vPosition.z;

            transform.LookAt(localPath[0].vPosition);
            transform.rotation = Quaternion.Euler(new Vector3(0, transform.eulerAngles.y, 0));
            rb.velocity = (transform.forward * MovementSpeed) + new Vector3(0, rb.velocity.y, 0);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /*private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        for(int i = 0; i < localPath.Count-1; ++i)
        {
            //Gizmos.DrawWireSphere(localPath[i].vPosition, 2.0f);
            Gizmos.DrawLine(localPath[i].vPosition, localPath[i + 1].vPosition);
        }
    }*/
}