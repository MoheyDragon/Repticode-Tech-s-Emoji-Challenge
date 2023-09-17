using UnityEngine;

public class PivotTracker : MonoBehaviour
{
    [SerializeField] Transform target;
    void Update()
    {
        transform.LookAt(target);
    }
}
