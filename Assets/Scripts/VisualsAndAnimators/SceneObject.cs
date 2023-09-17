using UnityEngine;

public class SceneObject : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected CanvasGroup _canvasGroup;
    public CanvasGroup canvasGroup => _canvasGroup;
}
