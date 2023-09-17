using UnityEngine;
using System.Collections;

public class Text3DAnimationController : MonoBehaviour
{
    protected TextGroup textGroup;

    protected bool IsLast;

    [SerializeField] protected float stopTime = 0;
    [SerializeField] protected GameObject textModel;
    protected Quaternion startPos;
    protected void Awake()
    {
        AddTextModel();
    }
    protected virtual void Start()
    {
        textGroup = transform.parent.GetComponent<TextGroup>();
        IsLast = textGroup.GetTextsCount == transform.GetSiblingIndex()+1;
        startPos = transform.rotation;
    }
    protected void AddTextModel()
    {
        GameObject _textModel= Instantiate(textModel, transform.GetChild(0));
        _textModel.transform.SetParent(transform);
        Destroy(transform.GetChild(0).gameObject);
    }
    public virtual void StartMove()
    {
        // if stop time is larger than zero, the text has to stop in the middle, so the only difference here is ease method,
        // if there is a stop in the middle it is easeInOut else it's only ease in
        if (stopTime <= 0)
            LeanTween.rotate(gameObject, TextsManager3D.Instance.GetMidVector, textGroup.MidRotateDuration).setEaseInSine().setOnComplete(OnMidRotationFinished);
        else
            LeanTween.rotate(gameObject, TextsManager3D.Instance.GetMidVector, textGroup.MidRotateDuration).setEaseInOutSine().setOnComplete(OnMidRotationFinished);
    }
    public virtual void OnMidRotationFinished()
    {
        // Same as Start move, if there is no stop time only ease out else ease both ways after waiting for stop time
        if (stopTime <= 0)
            LeanTween.rotate(gameObject, TextsManager3D.Instance.GetEndVector, textGroup.MidRotateDuration).setEaseOutSine().
                setOnComplete(OnAnimationEnd);
        else
            textGroup.PauseSingleText(transform.GetSiblingIndex(), stopTime);
    }
    public virtual void ContinueAnimation()
    {
        LeanTween.rotate(gameObject, TextsManager3D.Instance.GetEndVector, textGroup.MidRotateDuration).setEaseInOutSine().
            setOnComplete(OnAnimationEnd);
    }
    public void OnAnimationEnd()
    {
        transform.rotation = startPos;
        if (IsLast)
            textGroup.AnimationsReachedLoopPoint();
    }
    public void Stop()
    {
        transform.rotation = startPos;
        gameObject.SetActive(false);
    }
}
