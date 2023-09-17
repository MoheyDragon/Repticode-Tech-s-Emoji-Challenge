using UnityEngine;
using System;

public class TopicsChoosingPanel3DGroup : MonoBehaviour
{
    Animator animator;
    readonly string enter = "Enter";
    readonly string exit = "Exit";
    Action currentCallBack;
    [SerializeField] float stretchSize = 1.7f;
    void Start()
    {
        animator = GetComponent<Animator>();
        panelStretchSize = Vector3.one * stretchSize;
        panelWithdrawSize = panels[0].transform.localScale;
        secondPanelMeshRenderer = panels[1].GetComponent<MeshRenderer>();
        secondEmojiMeshRenderer = panels[1].transform.GetChild(0).GetComponent<MeshRenderer>();
    }
    public void Enter(Action callBack = null)
    {
        animator.SetTrigger(enter);
        ShowSecondPanel();
        if (callBack != null)
            currentCallBack = callBack;
    }
    private void ShowSecondPanel()
    {
        secondPanelMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        secondEmojiMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

        secondPanelMeshRenderer.material.SetFloat("_alpha", 1);
        secondEmojiMeshRenderer.material.SetFloat("_alpha", 1);
    }
    public void Exit(Action callBack = null)
    {
        animator.SetTrigger(exit);
        currentCallBack = callBack;
    }

    public void OnAnimationEndCallBack()
    {
        currentCallBack?.Invoke();
        //Fade material instead (on animation fade the alpha color of material)
    }

    [SerializeField] GameObject[] panels;

    #region PanelStreching

    Vector3 panelStretchSize,panelWithdrawSize;
    readonly float strecthingDuration=1;
    public void StretchPanels()
    {
        foreach (GameObject panel in panels)
            StretchSinglePanel(panel);
    }
    private void StretchSinglePanel(GameObject panel)
    {
        LeanTween.scale(panel, panelStretchSize, strecthingDuration);
    }
    MeshRenderer secondPanelMeshRenderer;
    MeshRenderer secondEmojiMeshRenderer;
    readonly float fadeDuration = 1;
    public void FadeSecondPanel()
    {
        secondPanelMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        secondEmojiMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        LeanTween.value(gameObject, 1, 0, fadeDuration).setEaseInOutSine().setOnUpdate((float value) =>
        {
            secondPanelMeshRenderer.material.SetFloat("_alpha", value);
            secondEmojiMeshRenderer.material.SetFloat("_alpha", value);
        });
    }
    public void WithdrawPanels()
    {
        for (int i = 0; i < 2; i++)
        {
            LeanTween.scale(panels[i], panelWithdrawSize, strecthingDuration);
        }
    }

    #endregion

    public void PopChoice(int index, Action callBack)
    {
        BenefitsManager3D.Instance.PopUpSelectedTopic(panels[index], callBack);
        KeyboardManager.Instance.DisconnectInputReceiver();
    }
}

