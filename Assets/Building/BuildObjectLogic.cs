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
    public static bool CanAfford;
    public GameObject BuiltPrefab;
    public int myCost;

    [HideInInspector] public bool CanBuild = true;

    private void Awake()
    {
        CanAfford = false;
        myCost = GetComponent<MoneyHandler>().Cost;
        CanBuild = true;
        LogicSystem = this;    
    }

    void Update ()
    {
        if (myCost > MoneyHandler.Money) CanAfford = false;
        else CanAfford = true;

        if (CanBuild && BuildingSystem.InBuildRange && myCost <= MoneyHandler.Money)
            foreach (MeshRenderer mesh in MaterialObjects)
                mesh.material = GoodBuildLocation;
        else
            foreach (MeshRenderer mesh in MaterialObjects)
                mesh.material = BadBuildLocation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name != "Ground" && other.name != "TurretDetector")
            CanBuild = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name != "Ground" && other.name != "TurretDetector")
            CanBuild = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name != "Ground" && other.name != "TurretDetector")
            CanBuild = true;
    }
}
