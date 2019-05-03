using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject ToSpawn1;
    public GameObject ToSpawn2;
    public UnityEngine.UI.Image Fillbar;
    public UnityEngine.UI.Text WaveCounter;

    public float SpawnRate = 1.0f;
    float HealthMult = 1f;
    int LeftToSpawn = 0;
    float ToSpawn = 1;
    float TimeWaited = 0f;
    int Wave = 0;

    void Start()
    {
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        StartCoroutine(FillBarVisualizer());
        yield return new WaitForSeconds(SpawnRate);
        ToSpawn *= 1.35f;
        LeftToSpawn = (int)Mathf.Round(ToSpawn);
        HealthMult *= 1.1f;
        StartCoroutine(WaveCreator());
        StartCoroutine(Spawn());
        ++Wave;
        WaveCounter.text = "" + Wave;
    }

    private IEnumerator WaveCreator()
    {
        yield return new WaitForSeconds(0.2f);
        GameObject spawn;

        if (Random.Range(1, 4) == 2)
            spawn = Instantiate(ToSpawn1, transform.position, transform.rotation);
        else spawn = Instantiate(ToSpawn2, transform.position, transform.rotation);

        spawn.name = "Enemy(Clone)";
        spawn.GetComponent<AI>().Health = HealthMult;

        --LeftToSpawn;

        if (LeftToSpawn > 0) StartCoroutine(WaveCreator());
    }

    private IEnumerator FillBarVisualizer()
    {
        Fillbar.fillAmount = 0;

        while (Fillbar.fillAmount < 0.95f)
        {
            yield return new WaitForSeconds(1);
            //StartCoroutine(LerpFill());
            Fillbar.fillAmount += 0.05f;
            //Fillbar.fillAmount = Mathf.Lerp(Fillbar.fillAmount, 1, 0.1f);
        }
    }

    //private IEnumerator LerpFill()
    //{
    //    float current = Fillbar.fillAmount;

    //    while(Fillbar.fillAmount < current + 0.05f)
    //    {
    //        yield return new WaitForSeconds(0.01f);
    //        Fillbar.fillAmount += 0.01f;
    //    }
    //}
}