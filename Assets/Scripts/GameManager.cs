using PlatformServices;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        PlatformUserStats.SetAchievement("welcomeAchivement");
    }
}