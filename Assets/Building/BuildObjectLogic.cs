using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildObjectLogic : MonoBehaviour
{
    public MeshRenderer[] MaterialObjects = new MeshRenderer[0];
    public Material BadBuildLocation;
    public Material GoodBuildLocation;
    public static Building BuildingSystem;
    public static BuildObjectLogic LogicSystem;
    public GameObject BuiltPrefab;

    [HideInInspector] public bool CanBuild = true;

    private void Awake()
    {
        CanBuild = true;
        LogicSystem = this;    
    }

    void Update ()
    {
		if(CanBuild && BuildingSystem.InBuildRange)
            foreach (MeshRenderer mesh in MaterialObjects)
                mesh.material = GoodBuildLocation;
        else
            foreach (MeshRenderer mesh in MaterialObjects)
                mesh.material = BadBuildLocation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name != "Ground")
            CanBuild = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name != "Ground")
            CanBuild = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name != "Ground")
            CanBuild = true;
    }
}
