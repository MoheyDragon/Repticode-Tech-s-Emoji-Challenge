using UnityEngine.UI;
using UnityEngine;
public class TimeControllerDebugger : MonoBehaviour
{
              public Text timeElapsed;
    [SerializeField] Text currentSpeedText;
    [SerializeField] float increasmentUnit = 1;
    [SerializeField] bool enableTimeCounter;
    private void Awake()
    {
        GetComponent<TimeCounter>().enabled = enableTimeCounter;
        timeElapsed.transform.parent.gameObject.SetActive(enableTimeCounter);
        string path = Application.dataPath + "/../timeController.json";
        System.IO.StreamReader reader = new System.IO.StreamReader(path);
        string data = reader.ReadToEnd();
        ExternalDebuggingController debugTimeController = JsonUtility.FromJson<ExternalDebuggingController>(data);
        gameObject.SetActive(debugTimeController.isDebugging);
    }
    private void Update()
    {
        if(Input.GetKey(KeyCode.LeftShift))
            Time.timeScale = 10;
        if(Input.GetKey(KeyCode.CapsLock))
            Time.timeScale = 50;
        if (Input.GetKey(KeyCode.LeftControl))
            Time.timeScale = 0.000001f;
        if (Input.GetKey(KeyCode.LeftAlt))
            Time.timeScale = 1;
    }
    public void SpeedUp()
    {
        if (Time.timeScale+(1*increasmentUnit)>100)
            Time.timeScale = 100;
        else
            Time.timeScale += 1 * increasmentUnit;

        currentSpeedText.text = Time.timeScale.ToString();
    }
    public void SpeedDown()
    {
        if (Time.timeScale-(1*increasmentUnit)<=0)
            Time.timeScale = 1;
        else
            Time.timeScale -= 1 * increasmentUnit;

        currentSpeedText.text = Time.timeScale.ToString();
    }

}
