using UnityEngine;
public class PerformanceManager : Singleton<PerformanceManager>
{

    private const int tasksCount = 3;
    public TaskPerformanceRecorder[] tasksPerformanceRecorders=new TaskPerformanceRecorder[tasksCount];
    [SerializeField] float minimumBarValue=0.3f;
    [SerializeField] float wrongEntryPenalty = 0.1f;
    private void Start()
    {
        for (int i = 0; i < tasksCount; i++)
            ResetTask(i);
    }
    public void ResetTask(int taskIndex)
    {
        tasksPerformanceRecorders[taskIndex].speed = 0;
        tasksPerformanceRecorders[taskIndex].logic = 1;
    }
    public void SpeedRecord(int taskIndex,float timeLeft)
    {
        tasksPerformanceRecorders[taskIndex].speed += timeLeft;
        if (tasksPerformanceRecorders[taskIndex].speed<minimumBarValue)
        {
            tasksPerformanceRecorders[taskIndex].speed = minimumBarValue;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            for (int i = 0; i < 3; i++)
            {
                string textToPrint = "task id " + i; 
                textToPrint+="\n"+ " speed = " + tasksPerformanceRecorders[i].speed.ToString();
                textToPrint+= "| logic = " + tasksPerformanceRecorders[i].logic.ToString();
                print(textToPrint);
            }
        }
    }
    public void LogicRecord(int taskIndex,bool noEntry=false)
    {
        tasksPerformanceRecorders[taskIndex].logic-= wrongEntryPenalty;
        if (tasksPerformanceRecorders[taskIndex].logic< minimumBarValue|| noEntry)
        {
            tasksPerformanceRecorders[taskIndex].logic = minimumBarValue;
        }
    }
}
