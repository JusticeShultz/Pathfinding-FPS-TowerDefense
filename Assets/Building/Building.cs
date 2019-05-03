using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public GameObject BuildObject;
    public LayerMask buildMask;
    public float MaxBuildRange = 10.0f;
    public bool InBuildRange = true;
    public GameObject StartingBuildObject;
    public Pathfinding Pathfinder;
    public UnityEngine.UI.Text CannotBlockText;

    private Vector3 rotate;

    void Start()
    {
        BuildObjectLogic.BuildingSystem = this;
        BuildObject = Instantiate(StartingBuildObject);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !ToggleBuildMenu.TabOpen)
        {
            if (BuildObjectLogic.LogicSystem.CanBuild && InBuildRange && BuildObjectLogic.CanAfford)
            {
                GameObject build = Instantiate(BuildObject.GetComponent<BuildObjectLogic>().BuiltPrefab, BuildObject.transform.position, BuildObject.transform.rotation);

                Pathfinder.ForceUpdate();

                if (WorldGrid.GridObj.IsBlocked())
                {
                    Destroy(build);
                    Pathfinder.ForceUpdate();
                    StartCoroutine(BlockText());
                }
                else
                {
                    //BuildersController.builder.buildHeap.Add(build);
                    //BuildController_Attempt2.Instance.buildHeap.Add(build);
                    BuildController_Attempt3.Instance.buildHeap.Add(build);
                    MoneyHandler.Money -= BuildObjectLogic.LogicSystem.myCost;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
            rotate = new Vector3(0, rotate.y + 45, 0);
        if (Input.GetKey(KeyCode.E))
            rotate = new Vector3(0, rotate.y + (220 * Mathf.Deg2Rad), 0);
        if (Input.GetKey(KeyCode.T))
            rotate = Vector3.zero;

        BuildObject.transform.rotation = Quaternion.Slerp(BuildObject.transform.rotation, Quaternion.Euler(rotate), 0.1f);

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, MaxBuildRange, buildMask))
        {
            InBuildRange = true;
            Debug.DrawLine(transform.position, hit.point);

            if (hit.collider.name == "Ground")
                BuildObject.transform.position = Vector3.Lerp(BuildObject.transform.position, hit.point, 0.25f);
        }
        else
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, buildMask))
        {
            InBuildRange = false;

            if (hit.collider.name == "Ground")
                BuildObject.transform.position = Vector3.Lerp(BuildObject.transform.position, hit.point, 0.25f);
        }
    }

    private IEnumerator BlockText()
    {
        CannotBlockText.color = Color.red;
        yield return new WaitForSeconds(2.0f);
        CannotBlockText.color = Color.clear;
    }

    public void SetBuildObject(GameObject obj)
    {
        Destroy(BuildObject);
        BuildObject = Instantiate(obj);
    }
}