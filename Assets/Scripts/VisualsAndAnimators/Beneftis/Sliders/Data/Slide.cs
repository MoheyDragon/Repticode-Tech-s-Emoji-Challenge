using UnityEngine;
using System;
[Serializable]
public class Slide
{
    public Texture texture;
    public float duration;
}
[Serializable]
public class GroupOfSlides
{
    public Slide[] Slides;
}

