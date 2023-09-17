using UnityEngine.UI;
using UnityEngine;

public class TimeCounter : MonoBehaviour
{
    Text timeElapsed;
    private void Start()
    {
        timeElapsed = GetComponent<TimeControllerDebugger>().timeElapsed;
    }
    private void Update()
    {
        timeElapsed.text = Time.timeSinceLevelLoad.ToString();
    }
}
