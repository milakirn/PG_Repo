using UnityEngine;

namespace PlatformServices
{
    public static class PlatformUserStats
    {
        private static IPlatformUserStats platform;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
#if !DISABLESTEAMWORKS
            platform = new SteamUserStats();
            return;

#elif UNITY_ANDROID
            platform = new AndroidUserStats();
            return;
#endif
        }

        public static bool SetAchievement(string achievementId)
        {
            Debug.Log($"Achivements: {achievementId}");
            return platform.SetAchievement(achievementId);
        }
    }

    public interface IPlatformUserStats
    {
        public bool SetAchievement(string achievementId);
    }

#if !DISABLESTEAMWORKS
    public class SteamUserStats : IPlatformUserStats
    {
        public bool SetAchievement(string achievementId)
        {
            return Steamworks.SteamUserStats.SetAchievement(achievementId);
        }
    }
#elif UNITY_ANDROID

    public class AndroidUserStats : IPlatformUserStats
    {
        public bool SetAchievement(string achievementId)
        {
            Social.ReportProgress("achievementId", 100f, null);
            return true; //Fake
        }
    }

#endif
}