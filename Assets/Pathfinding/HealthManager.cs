using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthManager : MonoBehaviour
{
    public static float Health = 25;

    public float StartingHealth = 25;
    public string LoseScene;
    public UnityEngine.UI.Image Fill;

    private void Start ()
    {
        Health = StartingHealth;    
    }

    void Update ()
    {
        Fill.fillAmount = Health / StartingHealth;
        if (Health <= 0) SceneManager.LoadScene(LoseScene);
	}
}
