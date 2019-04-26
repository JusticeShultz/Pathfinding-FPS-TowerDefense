using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAI : MonoBehaviour
{
    public GameObject FirePoint;
    public GameObject Shot;
    public float ShotSpeed;
    public float FireRate;

    GameObject target;

    private void Start()
    {
        StartCoroutine(Fire());
    }

    private IEnumerator Fire()
    {
        yield return new WaitForSeconds(FireRate);

        if (target != null)
        {
            GameObject shot = Instantiate(Shot, FirePoint.transform.position, transform.rotation);
            shot.GetComponent<Rigidbody>().velocity = transform.parent.forward * ShotSpeed;
            shot.GetComponent<Bullet>().target = target;
            shot.GetComponent<Bullet>().speed = ShotSpeed;
        }

        StartCoroutine(Fire());
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.name == "Enemy(Clone)" && target == null) if(!other.GetComponent<AI>().Targeted)
        {
            target = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (target == other.gameObject) target = null;
    }

    private void Update()
    {
        if (target != null)
            transform.parent.LookAt(target.transform.position);
    }
}
