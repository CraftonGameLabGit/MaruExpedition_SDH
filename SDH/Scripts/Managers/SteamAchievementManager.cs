using Steamworks;

// ACHIEVEMENT_Welcome: 게임 켜면 달성

public class SteamAchievementManager
{
    public bool IsNoGold { get { return isNoGold; } set { isNoGold = value; } }
    private bool isNoGold; // 노골드 클리어
    public bool IsNoDamage { get { return isNoDamage; } set { isNoDamage = value; } }
    private bool isNoDamage; // 노피격 클리어
    public bool IsNoUpgrade { get { return isNoUpgrade; } set { isNoUpgrade = value; } }
    private bool isNoUpgrade; //노업글 클리어

    public void StartGame()
    {
        isNoGold = true;
        isNoDamage = true;
        isNoUpgrade = true;
    }

    public void LoadGame()
    {
        isNoGold = Managers.Data.LocalPlayerData.gameData.isNoGold;
        isNoDamage = Managers.Data.LocalPlayerData.gameData.isNoDamage;
        isNoUpgrade = Managers.Data.LocalPlayerData.gameData.isNoUpgrade;
    }

    public void Achieve(string apiName) // 도전과제 달성
    {
        if (!SteamManager.Initialized) return;

        SteamUserStats.GetAchievement(apiName, out bool isAchieved);

        if (!isAchieved)
        {
            SteamUserStats.SetAchievement(apiName);
            SteamUserStats.StoreStats();
            CheckPrefect();
        }
    }

    private void CheckPrefect() // 다른 모든 도전과제 클리어 체크
    {
        SteamUserStats.RequestCurrentStats();

        int unlockAchievement = 0, totalAchievement = (int)SteamUserStats.GetNumAchievements();
        for(uint i = 0; i < totalAchievement; i++)
        {
            string apiName = SteamUserStats.GetAchievementName(i);
            SteamUserStats.GetAchievement(apiName, out bool isAchieved);
            if(isAchieved) unlockAchievement++;
        }

        if (totalAchievement - 1 <= unlockAchievement)
        {
            SteamUserStats.SetAchievement("ACHIEVEMENT_Perfect");
            SteamUserStats.StoreStats();
        }
    }
}
