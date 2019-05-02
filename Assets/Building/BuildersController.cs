#region Includes
using System.Collections.Generic;
using UnityEngine;
#endregion

#region Task variables
public enum TaskToComplete { Build, Collect, Idle }
public class Tasks
{
    public GameObject interactingWith;
    public TaskToComplete task;
    public bool Complete = false;

    public Tasks(GameObject gameObject, TaskToComplete taskToComplete, bool complete)
    {
        interactingWith = gameObject;
        task = taskToComplete;
        Complete = complete;
    }
}
#endregion

public class BuildersController : MonoBehaviour
{
    #region Variables
    public static BuildersController builder; //Who we are.
    public List<GameObject> scrapHeap = new List<GameObject>(); //The scrap that is on the field.
    public List<GameObject> buildHeap = new List<GameObject>(); //The buildings queued to build.
    public List<Tasks> taskList = new List<Tasks>(); //The current task list we are working with.
    public GameObject HomeBase; //The home base(Where we return when we are going idle).
    public UnityEngine.UI.Text HomeBaseText; //The text that states what our bot is doing.
    public float MovementSpeed = 5.0f; //The movement speed of our builder.
    public float MoveDistance = 2.0f; //The max distance we may be from our desired object.
    public Animator anim; //The animation controller our object uses.
    float StunTime = 0f; //The time remaining that we are stunned for.
    #endregion

    #region Initialize
    void Start ()
    {
        builder = this;
        taskList.Add(new Tasks(HomeBase, TaskToComplete.Idle, false));
	}
    #endregion

    #region Build logic

    void Update ()
    {
        #region What I "should" do.
        /*
            * Bot will prioritize building and collecting over being idle.
            * The bot will not go idle if there are things to build or collect.
            * Building will stun the bot for a short period.
            * Collecting will give money.
        */
        #endregion

        #region Task assignment
        //Don't do anything if stunned.
        if (StunTime > 0)
        {
            StunTime -= Time.deltaTime;
            return;
        }
        else
        {
            //Task completion checks:
            if (taskList.Count > 0)
                if (taskList[0].Complete)
                    taskList.Remove(taskList[0]);
        }

        //If we have no other tasks: be idle.
        if (scrapHeap.Count <= 0 && buildHeap.Count <= 0 && taskList.Count <= 0)
            taskList.Add(new Tasks(HomeBase, TaskToComplete.Idle, false));
        //If we have something to build in the heap, throw it into the list of tasks.
        if (buildHeap.Count > 0)
        {
            //We now have a purpose to exist, stop being idle.
            if(taskList.Count > 0)
                if(taskList[0].task == TaskToComplete.Idle)
                    taskList.Remove(taskList[0]);

            taskList.Add(new Tasks(buildHeap[0], TaskToComplete.Build, false));
            buildHeap.Remove(buildHeap[0]);
        }
        //Otherwise if we have something to collect and nothing to build, throw it into the list of tasks.
        else
        {
            //Check if there is scrap to collect.
            if (scrapHeap.Count > 0)
            {
                //We now have a purpose to exist, stop being idle.
                if (taskList.Count > 0)
                    if (taskList[0].task == TaskToComplete.Idle)
                        taskList.Remove(taskList[0]);

                taskList.Add(new Tasks(scrapHeap[0], TaskToComplete.Collect, false));
                scrapHeap.Remove(scrapHeap[0]);
            }
        }
        #endregion

        #region Task do-er
        if (taskList[0].task == TaskToComplete.Build)
        {
            if(!taskList[0].Complete)
                GoToBuild(taskList[0]);
            else
            {
                taskList.Remove(taskList[0]);
            }

            HomeBaseText.text = "Builder Task: Building";
        }
        else if (taskList[0].task == TaskToComplete.Collect)
        {
            if (!taskList[0].Complete)
                GoToCollect(taskList[0]);


            HomeBaseText.text = "Builder Task: Collecting";
        }
        else if (taskList[0].task == TaskToComplete.Idle)
        {
            if (!taskList[0].Complete)
                GoBack(taskList[0]);


            HomeBaseText.text = "Builder Task: Idle";
        }
        #endregion
    }

    #region Tasks
    void GoToCollect(Tasks task)
    {
        if (Vector3.Distance(task.interactingWith.transform.position, transform.position - (Vector3.up * 4)) < MoveDistance)
        {
            taskList[0].Complete = true;
            Destroy(task.interactingWith);
            MoneyHandler.Money += 35;
            StunTime = 1.0f;
            anim.SetBool("Moving", false);
        }
        else
        {
            transform.LookAt(task.interactingWith.transform.position);
            transform.position = Vector3.MoveTowards(transform.position, (task.interactingWith.transform.position + (Vector3.up * 4)), Time.deltaTime * MovementSpeed);
            anim.SetBool("Moving", true);
        }
    }

    void GoToBuild(Tasks task)
    {
        if (Vector3.Distance(task.interactingWith.transform.position, transform.position - (Vector3.up * 4)) < MoveDistance)
        {
            Instantiate(task.interactingWith.GetComponent<QueuedBuildItem>().BuildObject, task.interactingWith.transform.position, task.interactingWith.transform.rotation);
            taskList[0].Complete = true;
            Destroy(task.interactingWith);
            StunTime = 3.5f;
            anim.SetTrigger("Build");
            anim.SetBool("Moving", false);
        }
        else
        {
            transform.LookAt(task.interactingWith.transform.position);
            transform.position = Vector3.MoveTowards(transform.position, (task.interactingWith.transform.position + (Vector3.up * 4)), Time.deltaTime * MovementSpeed);
            anim.SetBool("Moving", true);
        }
    }

    void GoBack(Tasks task)
    {
        if (Vector3.Distance(task.interactingWith.transform.position, transform.position) < MoveDistance * 0.5f)
        {
            taskList[0].Complete = true;
            anim.SetBool("Moving", false);
            transform.rotation = task.interactingWith.transform.rotation;
        }
        else
        {
            transform.LookAt(task.interactingWith.transform.position);
            transform.position = Vector3.MoveTowards(transform.position, task.interactingWith.transform.position, Time.deltaTime * MovementSpeed);
            anim.SetBool("Moving", true);
        }
    }
    #endregion

    #endregion
}