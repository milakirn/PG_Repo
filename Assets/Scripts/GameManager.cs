using PlatformServices;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void Initialize()
    {
        if (Instance != null)
            return;
        
        Instantiate(Resources.Load("GameManager"));
    }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlatformUserStats.SetAchievement("welcomeAchivement");
    }
}