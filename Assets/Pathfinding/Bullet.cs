using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float Damage = 1;

    Rigidbody rb;
    [HideInInspector] public GameObject target;
    [HideInInspector] public float speed;
    float Timer = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Enemy(Clone)")
        {
            //Destroy(collision.gameObject);
            collision.gameObject.GetComponent<AI>().Health -= Damage;
        }

        Destroy(gameObject);
    }

    private void Update()
    {
        Timer += Time.deltaTime;
        //Don't live for more than 4 seconds, this will help reduce lag.
        if (Timer >= 4) Destroy(gameObject);

        if (target == null) return;
        else
        {
            if (target.transform != null)
                transform.LookAt(target.transform.position);
            rb.velocity = transform.forward * speed;
        }
    }
}
