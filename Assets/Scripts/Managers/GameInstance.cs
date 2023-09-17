using UnityEngine;
public class GameInstance : Singleton<GameInstance>
{
    protected override void Awake()
    {
        base.Awake();
        Screen.SetResolution(2160, 3840, true);
        DontDestroyOnLoad(gameObject);
    }
}
