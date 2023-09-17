using UnityEngine;
using RTLTMPro;
public class BarNameFading : MonoBehaviour
{
    RTLTextMeshPro barName;
    [SerializeField] float fadeSpeed = 1f;
    private void Awake()
    {
        barName = GetComponent<RTLTextMeshPro>();
        isFadingIn = true;
    }
    bool isFadingIn;
    public void FadeOut()
    {
        isFadingIn = false;
    }
    private void Update()
    {
        barName.color = GetNextAlphaDegree(barName.color);

        if (barName.color.a <= 0 || barName.color.a >= 1)
        {
            enabled = false;
        }
    }
    private Color GetNextAlphaDegree(Color color)
    {
        float direction = isFadingIn ? 1 : -1;
        return new Color(color.r, color.g, color.b, color.a + fadeSpeed * direction);
    }
    public void SetText(string text)
    {
        barName.text = text;
    }
}
