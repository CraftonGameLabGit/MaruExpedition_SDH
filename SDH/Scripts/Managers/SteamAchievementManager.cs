using Steamworks;

// ACHIEVEMENT_Welcome: ���� �Ѹ� �޼�

public class SteamAchievementManager
{
    public bool IsNoGold { get { return isNoGold; } set { isNoGold = value; } }
    private bool isNoGold; // ���� Ŭ����
    public bool IsNoDamage { get { return isNoDamage; } set { isNoDamage = value; } }
    private bool isNoDamage; // ���ǰ� Ŭ����
    public bool IsNoUpgrade { get { return isNoUpgrade; } set { isNoUpgrade = value; } }
    private bool isNoUpgrade; //����� Ŭ����

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

    public void Achieve(string apiName) // �������� �޼�
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

    private void CheckPrefect() // �ٸ� ��� �������� Ŭ���� üũ
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
