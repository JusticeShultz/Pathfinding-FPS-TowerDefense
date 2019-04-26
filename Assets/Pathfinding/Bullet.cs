﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody rb;
    [HideInInspector] public GameObject target;
    [HideInInspector] public float speed;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Enemy(Clone)")
        {
            Destroy(collision.gameObject);
        }

        Destroy(gameObject);
    }

    private void Update()
    {
        if (target == null) return; //Destroy(gameObject);
        else
        {
            if (target.transform != null)
                transform.LookAt(target.transform.position);
            rb.velocity = transform.forward * speed;
        }
    }
}