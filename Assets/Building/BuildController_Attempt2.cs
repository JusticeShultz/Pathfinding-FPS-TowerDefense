//Attempt #2 at a FSM for a builder.

using System.Collections.Generic;
using UnityEngine;

#region Tasks and other data
public enum Actions { Idle, IdleToCollect, IdleToBuild, Collect, Build}
public class TaskData
{
    public GameObject interactingWith;
    public Actions task;
    public bool Complete = false;

    public TaskData(GameObject gameObject, Actions taskToComplete, bool complete)
    {
        interactingWith = gameObject;
        task = taskToComplete;
        Complete = complete;
    }
}
#endregion

public class BuildController_Attempt2 : MonoBehaviour
{
    #region Variables

    public static BuildController_Attempt2 Instance; //The instance of this object as a singleton.
    public GameObject homeBase; //The home base(Where we return when we are going idle).
    public Animator animationController; //The animation controller our object uses.
    public UnityEngine.UI.Text homeBaseText; //The text that states what our bot is doing.
    public bool canCollect = true;
    public bool canBuild = true;
    public float movementSpeed = 5.0f; //The movement speed of our builder.
    public float moveDistance = 2.0f; //The max distance we may be from our desired object.
    
    //[These lists are handled by other scripts and simply keep us aware of the board state so that we do not need to use finds, etc]
    [HideInInspector] public List<GameObject> scrapHeap = new List<GameObject>(); //The scrap that is on the field.
    [HideInInspector] public List<GameObject> buildHeap = new List<GameObject>(); //The buildings queued to build.
    [HideInInspector] public TaskData currentState; //This handles the current state we are on.
    float StunTime = 0f; //The time remaining that we are stunned for.

    #endregion

    //Grabs the instance of our builder so other scripts may access us easier.
    void Awake ()
    {
        //Grab the instance.
        Instance = this;
        //Set us to idle to start with.
        currentState = new TaskData(homeBase, Actions.Idle, true);
    }

    //Handles to logic of our builder.
	void LateUpdate ()
    {
        //Update the current states text.
        UpdateDisplayState();

        //If we are stunned:
        if (StunTime > 0)
        {
            //Decrement the stun time.
            StunTime -= Time.deltaTime;

            //Set the task to be completed when we become unstunned.
            if (StunTime <= 0)
                currentState.Complete = true;

            //Return to prevent further execution of tasks until we are unstunned/reached 0 on StunTime.
            return;
        }

        //Execute our current state.
        StateExecution();

        //Check if we have completed our state.
        if (CompletedStateTask())
        {
            //If so, go idle and look for a new task.
            currentState.task = Actions.Idle;
            currentState.interactingWith = homeBase;
            StateAssignment();
        }
    }

    //This handles the transition from idle to other states.
    void StateAssignment()
    {
        //If we are idle, do the transition checks:
        if (currentState.task == Actions.Idle)
        {
            //[Prioritize building over collecting]
            //If there is stuff to build and we can build.
            if (buildHeap.Count > 0 && canBuild)
            {
                //Fill the current state with relevant data.
                currentState = new TaskData(buildHeap[0], Actions.IdleToBuild, false);
                //Pull the build object from the heap.
                buildHeap.Remove(buildHeap[0]);
            }
            else //If there is stuff to collect and we are allowed to collect.
            if (scrapHeap.Count > 0 && canCollect)
            {
                //Fill the current state with relevant data.
                currentState = new TaskData(scrapHeap[0], Actions.IdleToCollect, false);
                //Pull the scrap object from the heap.
                scrapHeap.Remove(scrapHeap[0]);
            }
        }
    }

    //Check if the state has been marked for completion.
    bool CompletedStateTask()
    {
        //Check if marked for completion.
        if (currentState.Complete) return true;
        //Idle will always be complete.
        if (currentState.task == Actions.Idle) return true;
        //Otherwise, we are not done.
        return false;
    }

    //This will execute our state and handle getting to completion.
    void StateExecution()
    {
        //Check if we are transitioning from Idle to Building.
        if (currentState.task == Actions.IdleToBuild)
            //Move to the build location and then transition to build once there.
            IdleToBuild();

        //Check if we are transitioning from Idle to Collecting.
        if (currentState.task == Actions.IdleToCollect)
            //Move to the collection location and then transition to pickup once there.
            IdleToCollect();

        //Check if we are idle.
        if (currentState.task == Actions.Idle)
            //If we are walk home. If we are already home then sit there idle.
            WalkHome();
    }

    //This will display our state at the builders home base.
    void UpdateDisplayState()
    {
        if (currentState.task == Actions.Idle)
            homeBaseText.text = "Builder Task: Idle";
        if (currentState.task == Actions.Build)
            homeBaseText.text = "Builder Task: Building";
        if (currentState.task == Actions.Collect)
            homeBaseText.text = "Builder Task: Collecting";
        if (currentState.task == Actions.IdleToCollect)
            homeBaseText.text = "Builder Task: Moving to collect";
        if (currentState.task == Actions.IdleToBuild)
            homeBaseText.text = "Builder Task: Moving to build";
    }

    #region States (Handles the different states the object may have)

    void WalkHome()
    {
        if (!(Vector3.Distance(currentState.interactingWith.transform.position, transform.position) < moveDistance * 0.5f)) 
            MoveTowards();
        else Idle();
    }

    void Idle()
    {
        currentState.Complete = true;
        animationController.SetBool("Moving", false);
        transform.rotation = currentState.interactingWith.transform.rotation;
    }

    void IdleToBuild()
    {
        //Check if we are not in range, if we aren't, move to the object.
        if (!(Vector3.Distance(currentState.interactingWith.transform.position, transform.position - (Vector3.up * 4)) < moveDistance))
            MoveTowards();
        else Build(); //Otherwise, transition to the build state.
    }

    void IdleToCollect()
    {
        //Check if we are not in range, if we aren't, move to the object.
        if (!(Vector3.Distance(currentState.interactingWith.transform.position, transform.position - (Vector3.up * 4)) < moveDistance))
            MoveTowards();
        else Collect(); //Otherwise, transition to the collect state.
    }

    void Collect()
    {
        //Prevent looping of this state.
        if (currentState.Complete) return;
        //Stun us for a short time.
        StunTime = 1.0f;
        //Specify we are collecting.
        currentState.task = Actions.Collect;
        //Destroy the scrap.
        Destroy(currentState.interactingWith);
        //Give us some money.
        MoneyHandler.Money += 35;
        //Set moving to be false.
        animationController.SetBool("Moving", false);
    }

    void Build()
    {
        //Prevent looping of this state.
        if (currentState.Complete) return;
        //Set our current task to build.
        currentState.task = Actions.Build;
        //Build the object.
        Instantiate(currentState.interactingWith.GetComponent<QueuedBuildItem>().BuildObject, currentState.interactingWith.transform.position, currentState.interactingWith.transform.rotation);
        //Destroy the template.
        Destroy(currentState.interactingWith);
        //Stun us for a short time so our animation may carry out.
        StunTime = 3.5f;
        //Tell our animation to build.
        animationController.SetTrigger("Build");
        //Tell our animation to stop moving.
        animationController.SetBool("Moving", false);
    }

    void MoveTowards()
    {
        //Look at the object.
        transform.LookAt(currentState.interactingWith.transform.position);
        //Move to the object.
        transform.position = Vector3.MoveTowards(transform.position, (currentState.interactingWith.transform.position + (Vector3.up * 4)), Time.deltaTime * movementSpeed);
        //Tell our animation controller we are moving.
        animationController.SetBool("Moving", true);
    }

    #endregion
}