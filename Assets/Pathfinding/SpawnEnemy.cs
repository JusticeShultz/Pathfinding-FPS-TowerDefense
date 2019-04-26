using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject ToSpawn;
    public float SpawnRate = 1.0f;

    void Start ()
    {
        StartCoroutine(Spawn());
	}

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(SpawnRate);
        Instantiate(ToSpawn, transform.position, transform.rotation);
        StartCoroutine(Spawn());
    }
}
