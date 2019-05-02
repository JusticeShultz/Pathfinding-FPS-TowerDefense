using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject ToSpawn1;
    public GameObject ToSpawn2;

    public float SpawnRate = 1.0f;

    void Start ()
    {
        StartCoroutine(Spawn());
	}

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(SpawnRate);

        GameObject spawn;

        if (Random.Range(1, 4) == 2)
            spawn = Instantiate(ToSpawn1, transform.position, transform.rotation);
        else spawn = Instantiate(ToSpawn2, transform.position, transform.rotation);

        spawn.name = "Enemy(Clone)";
        StartCoroutine(Spawn());
    }
}
