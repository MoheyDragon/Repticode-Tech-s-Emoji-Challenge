using UnityEngine;
[CreateAssetMenu(menuName ="Task")]
public class Task :ScriptableObject
{
    public string code;
    public string sizeInCode;
    public Sprite[] emojiFaceParts;
    public Texture emoji;
    public Color color;
    public Vector3 size;
    public TaskDescription taskDescription;
}