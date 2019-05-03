//Attempt #3 at a FSM for a builder.
using System.Collections.Generic;
using UnityEngine;

public abstract class Behavior
{
    public abstract Behavior DoBehavior();
}

public class IdleBehavior : Behavior
{
    BuildController_Attempt3 controller;
    float DeltaTime = 0f;

    public IdleBehavior(BuildController_Attempt3 val, float Dt)
    {
        controller = val;
        DeltaTime = Dt;
    }

    public override Behavior DoBehavior()
    {
        //Prioritize building.

        if (controller.buildHeap.Count > 0)
        {
            return new BuildBehavior(controller, DeltaTime);
        }
        else
        {
            if (controller.scrapHeap.Count > 0)
            {
                return new CollectBehavior(controller, DeltaTime);
            }
            else
            {
                if (!(Vector3.Distance(controller.homeBase.transform.position, controller.gameObject.transform.position) < controller.moveDistance * 0.5f))
                    MoveTowards();
                else Idle();

                return new IdleBehavior(controller, DeltaTime);
            }
        }
    }

    void Idle()
    {
        controller.homeBaseText.text = "Builder Task: Idle";
        controller.animationController.SetBool("Moving", false);
        controller.gameObject.transform.rotation = controller.homeBase.transform.rotation;
    }

    void MoveTowards()
    {
        controller.homeBaseText.text = "Builder Task: Going home";
        //Look at the object.
        controller.transform.LookAt(controller.homeBase.transform.position);
        //Move to the object.
        controller.transform.position = Vector3.MoveTowards(controller.gameObject.transform.position, controller.homeBase.transform.position, DeltaTime * controller.movementSpeed);
        //Tell our animation controller we are moving.
        controller.animationController.SetBool("Moving", true);
    }
}

public class CollectBehavior : Behavior
{
    BuildController_Attempt3 controller;
    GameObject CollectObject;
    bool Completed = false;
    float DeltaTime = 0f;

    public CollectBehavior(BuildController_Attempt3 val, float Dt)
    {
        controller = val;
        DeltaTime = Dt;
    }

    public override Behavior DoBehavior()
    {
        if (!(Vector3.Distance(controller.scrapHeap[0].transform.position, controller.gameObject.transform.position - (Vector3.up * 4)) < controller.moveDistance))
            MoveTowards();
        else Collect();

        if (Completed)
            return new IdleBehavior(controller, DeltaTime);
        else return new CollectBehavior(controller, DeltaTime);
    }

    void Collect()
    {
        controller.homeBaseText.text = "Builder Task: Collecting";
        CollectObject = controller.scrapHeap[0];
        controller.scrapHeap.Remove(controller.scrapHeap[0]);
        controller.StunTime = 0.25f;
        //Destroy the scrap.
        GameObject.Destroy(CollectObject);
        //Give us some money.
        MoneyHandler.Money += 35;
        //Set moving to be false.
        controller.animationController.SetBool("Moving", false);
        Completed = true;
    }

    void MoveTowards()
    {
        controller.homeBaseText.text = "Builder Task: Moving to collect";
        //Look at the object.
        controller.gameObject.transform.LookAt(controller.scrapHeap[0].transform.position);
        //Move to the object.
        controller.gameObject.transform.position = Vector3.MoveTowards(controller.gameObject.transform.position, (controller.scrapHeap[0].transform.position + (Vector3.up * 4)), DeltaTime * controller.movementSpeed);
        //Tell our animation controller we are moving.
        controller.animationController.SetBool("Moving", true);
    }
}

public class BuildBehavior : Behavior
{
    BuildController_Attempt3 controller;
    GameObject BuildObject;
    bool Completed = false;
    float DeltaTime = 0f;

    public BuildBehavior(BuildController_Attempt3 val, float Dt)
    {
        controller = val;
        DeltaTime = Dt;
    }

    public override Behavior DoBehavior()
    {
        if (!(Vector3.Distance(controller.buildHeap[0].transform.position, controller.gameObject.transform.position - (Vector3.up * 4)) < controller.moveDistance))
            MoveTowards();
        else Build();

        if (Completed)
            return new IdleBehavior(controller, DeltaTime);
        else return new BuildBehavior(controller, DeltaTime);
    }

    void Build()
    {
        controller.homeBaseText.text = "Builder Task: Building";
        BuildObject = controller.buildHeap[0];
        controller.buildHeap.Remove(controller.buildHeap[0]);
        //Build the object.
        GameObject.Instantiate(BuildObject.GetComponent<QueuedBuildItem>().BuildObject, BuildObject.transform.position, BuildObject.transform.rotation);
        //Destroy the template.
        GameObject.Destroy(BuildObject);
        //Stun us for a short time so our animation may carry out.
        controller.StunTime = 3.5f / controller.buildSpeed;
        //Tell our animation to build.
        controller.animationController.SetTrigger("Build");
        //Tell our animation to stop moving.
        controller.animationController.SetBool("Moving", false);
        Completed = true;
    }

    void MoveTowards()
    {
        controller.homeBaseText.text = "Builder Task: Moving to build";
        //Look at the object.
        controller.gameObject.transform.LookAt(controller.buildHeap[0].transform.position);
        //Move to the object.
        controller.gameObject.transform.position = Vector3.MoveTowards(controller.gameObject.transform.position, (controller.buildHeap[0].transform.position + (Vector3.up * 4)), DeltaTime * controller.movementSpeed);
        //Tell our animation controller we are moving.
        controller.animationController.SetBool("Moving", true);
    }
}

public class BuildController_Attempt3 : MonoBehaviour
{
    #region Variables

    public static int PortableDamage = 0;
    public static float PortableFireRate = 0f;
    public static BuildController_Attempt3 Instance; //The instance of this object as a singleton.
    public GameObject homeBase; //The home base(Where we return when we are going idle).
    public Animator animationController; //The animation controller our object uses.
    public UnityEngine.UI.Text homeBaseText; //The text that states what our bot is doing.
    public float movementSpeed = 5.0f; //The movement speed of our builder.
    public float moveDistance = 2.0f; //The max distance we may be from our desired object.
    public float buildSpeed = 1.0f; //The speed at which we build.
    public int[] UpgradeCost = new int[4] { 10, 10, 10, 10 };

    //[These lists are handled by other scripts and simply keep us aware of the board state so that we do not need to use finds, etc]
    [HideInInspector] public List<GameObject> scrapHeap = new List<GameObject>(); //The scrap that is on the field.
    [HideInInspector] public List<GameObject> buildHeap = new List<GameObject>(); //The buildings queued to build.
    [HideInInspector] public Behavior currentBehavior; //This handles the current state we are on.
    [HideInInspector] public float StunTime = 0f; //The time remaining that we are stunned for.

    #endregion
    
    void Awake ()
    {
        Instance = this; //Grabs the instance of our builder so other scripts may access us easier.
        currentBehavior = new IdleBehavior(this, Time.deltaTime); //Set the starting state to be idle.
    }

    void LateUpdate()
    {
        animationController.SetFloat("BuildSpeed", buildSpeed);

        if (StunTime > 0)
        {
            StunTime -= Time.deltaTime;
            return;
        }

        currentBehavior = currentBehavior.DoBehavior();
    }

    public void UpgradeMovementSpeed()
    {
        if(UpgradeCost[0] <= MoneyHandler.Money)
        {
            MoneyHandler.Money -= UpgradeCost[0];
            UpgradeCost[0] *= 2;
            movementSpeed += 0.3f;
        }
    }

    public void UpgradeBuildSpeed()
    {
        if (UpgradeCost[1] <= MoneyHandler.Money)
        {
            MoneyHandler.Money -= UpgradeCost[1];
            UpgradeCost[1] *= 4;
            buildSpeed += 0.25f;
        }
    }

    public void UpgradeShoulderTurretDamage()
    {
        if (UpgradeCost[2] <= MoneyHandler.Money)
        {
            MoneyHandler.Money -= UpgradeCost[2];
            UpgradeCost[2] *= 3;
            PortableDamage += 2;
        }
    }

    public void UpgradeShoulderTurretFireRate()
    {
        if (UpgradeCost[3] <= MoneyHandler.Money)
        {
            MoneyHandler.Money -= UpgradeCost[3];
            UpgradeCost[3] *= 5;
            PortableFireRate += 0.05f;
        }
    }
}