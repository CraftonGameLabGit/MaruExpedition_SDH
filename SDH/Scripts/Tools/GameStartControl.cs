using UnityEngine;

public class GameStartControl : MonoBehaviour
{
    public void StartNewGame(int newVehicleIdx, int newCharacterIdx, int newDifficultyIdx) // 새로운 게임 시작
    {
        Managers.PlayerControl.NowPlayer = Instantiate(Managers.Asset.Vehicles[newVehicleIdx], new(-5000f, 0f, 0f), Quaternion.identity);
        Managers.PlayerControl.VehicleIdx = newVehicleIdx;
        Managers.PlayerControl.Characters.Add(Instantiate(Managers.Asset.Characters[newCharacterIdx], Managers.PlayerControl.NowPlayer.transform));
        Managers.PlayerControl.CharactersIdx.Add(newCharacterIdx);
        Managers.Stage.Difficulty = newDifficultyIdx;

        Managers.Stage.StartGame();

        Managers.PlayerControl.CharactersCheck[newCharacterIdx] = true;

        // 🎯 첫 스테이지 진입 로깅 (새 게임)
        LogStageEnterEvent();

        Managers.SceneFlow.GotoScene("Field");
    }

    public void ContinueGame() // DataManager에 저장된 기존 게임 이어서 시작
    {
        if (Managers.Data.LocalPlayerData.gameData == null) return; // 저장된 데이터가 없음

        // 데이터 로드, 깊은 복사를 잊지 마
        GameData nowData = Managers.Data.LocalPlayerData.gameData;

        Managers.PlayerControl.CharactersCheck = new bool[Managers.Asset.Characters.Length];

        Managers.PlayerControl.NowPlayer = Instantiate(Managers.Asset.Vehicles[nowData.vehicleIdx], new(-5000f, 0f, 0f), Quaternion.identity);
        for(int i = 0; i < nowData.characters.Count; i++)
        {
            CharacterData character = nowData.characters[i];

            Managers.PlayerControl.CharactersIdx.Add(character.characterIdx);
            Managers.PlayerControl.Characters.Add(Instantiate(Managers.Asset.Characters[character.characterIdx], Managers.PlayerControl.NowPlayer.transform));
            Managers.PlayerControl.CharactersCheck[character.characterIdx] = true;
            UpgradeController upgradeController = Managers.PlayerControl.Characters[i].GetComponent<UpgradeController>();
            upgradeController.LoadUpgrade(character.upgrades, character.upgradeValues);
        }

        Managers.Stage.LoadGame();

        // 🎯 스테이지 진입 로깅 (게임 이어하기)
        LogStageEnterEvent();

        // 시작
        if (Managers.Stage.OnField) Managers.SceneFlow.GotoScene("Field");
        else Managers.SceneFlow.GotoScene("Shop");
    }
    
    /// <summary>
    /// Stage_Enter 이벤트 로깅 - 실제 게임 데이터 사용
    /// </summary>
    private void LogStageEnterEvent()
    {
        try
        {
            // 현재 스테이지 정보 가져오기
            int worldID = Managers.Stage.World;
            int stageNumber = Managers.Stage.Stage;
            int difficulty = Managers.Stage.Difficulty;
            
            // Analytics 이벤트 전송
            if (AnalyticsManager.Instance != null)
            {
                AnalyticsManager.Instance.LogStageEnter(worldID, stageNumber, difficulty.ToString());
                
                Debug.Log($"[Stage_Enter] WorldID={worldID}, StageNumber={stageNumber}, Difficulty={difficulty}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Stage_Enter] 로깅 실패: {e.Message}");
        }
    }

    public void OpenDiscord()
    {
        Application.OpenURL("https://discord.com/invite/DhggjCkFDU"); // 원하는 URL로 변경
    }

    public void OpenLog()
    {
        Application.OpenURL("https://store.steampowered.com/news/app/3818230?emclan=103582791475205499&emgid=541111741871818514"); // 원하는 URL로 변경
    }

    public void GoToEndingCredit()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("EndingCredit");
    }
}
