using UnityEngine.Events;
using UnityEngine;

public class ActivationEscButton : MonoBehaviour
{
    [SerializeField] UnityEvent OnPress;
    [SerializeField] KeyCode key=KeyCode.Escape;
    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            OnPress?.Invoke();
        }    
    }
}
