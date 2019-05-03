using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretAI : MonoBehaviour
{
    public GameObject FirePoint;
    public GameObject Shot;
    public float ShotSpeed;
    public float FireRate;
    public bool Aim = true;
    public bool shoulderTurret = false;

    private float fireRate_nontemp = 0f;

    GameObject target;

    private void Start()
    {
        fireRate_nontemp = FireRate;
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

            if(shoulderTurret)
            {
                shot.GetComponent<Bullet>().Damage += BuildController_Attempt3.PortableDamage;
            }
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
        if (shoulderTurret)
            FireRate = fireRate_nontemp - BuildController_Attempt3.PortableFireRate;
                
        if (target != null && Aim)
            transform.parent.LookAt(target.transform.position);
    }
}
