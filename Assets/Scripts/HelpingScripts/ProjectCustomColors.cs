using System.Collections.Generic;
using UnityEngine;

public class ProjectCustomColors :Singleton<ProjectCustomColors>
{
    [SerializeField] Color pale, red, transparent,green, moonLight;
    public Color Pale => pale;
    public Color Red => red;
    public Color Transparent => transparent;
    public Color Green => green;
    public Color MoonLight=>moonLight;
}
