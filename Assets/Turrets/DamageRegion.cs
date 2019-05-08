using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageRegion : MonoBehaviour
{
    public float Damage;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            other.GetComponent<AI>().Health -= Damage;
        }
    }
}
