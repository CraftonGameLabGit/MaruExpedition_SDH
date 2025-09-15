using System.Collections;
using UnityEngine;

public class SelectControl : MonoBehaviour
{
    [SerializeField] private SelectVehicleCanvas selectVehicleCanvas;
    [SerializeField] private SelectCharacterCanvas selectCharacterCanvas;
    [SerializeField] private SelectDifficultyCanvas selectDifficultyCanvas;
    [SerializeField] private Canvas curtain;

    private void Start()
    {
        StartCoroutine(StartDelay());
    }

    private IEnumerator StartDelay()
    {
        yield return null;
        StartSelectVehicle();
    }

    public void StartSelectVehicle()
    {
        selectVehicleCanvas.StartVehicleCanvasIE();
    }

    public void StartSelectCharacter()
    {
        selectCharacterCanvas.StartCharacterCanvasIE();
    }

    public void StartSelectDifficulty()
    {
        selectDifficultyCanvas.StartDifficultyCanvasIE();
    }

    public void StartGame()
    {
        StartCoroutine(StartGameIE());
    }

    public IEnumerator StartGameIE()
    {
        curtain.enabled = true;
        float nowTime = 0f, maxTime = 0.4f;
        while (nowTime <= maxTime)
        {
            curtain.GetComponent<CanvasGroup>().alpha = nowTime / maxTime;
            nowTime += Time.deltaTime;
            yield return null;
        }
        curtain.GetComponent<CanvasGroup>().alpha = 1f;

        yield return new WaitForSeconds(0.1f);

        LogRunStartEvent();

        GetComponent<GameStartControl>().StartNewGame(selectVehicleCanvas.NowSelectedIdx, selectCharacterCanvas.NowSelectedIdx, selectDifficultyCanvas.NowSelectedIdx);
    }

    /// <summary>
    /// Run_Start 이벤트 로깅 - 실제 게임 데이터 사용
    /// </summary>
    private void LogRunStartEvent()
    {
        try
        {
            // 선택된 비행선 정보 가져오기
            string selectedShip = GetSelectedShipName();
            
            // 선택된 캐릭터 정보 가져오기  
            string selectedCharacter = GetSelectedCharacterName();
            
            // 선택된 난이도 (숫자 그대로 - int 타입)
            int selectedDifficulty = selectDifficultyCanvas.NowSelectedIdx;
            
            // 🎯 실제 이전 런 데이터 가져오기
            int previousWorldID = -1;
            int previousStageNumber = -1;
            
            if (Managers.Data != null && Managers.Data.LocalPlayerData != null && 
                Managers.Data.LocalPlayerData.gameData != null)
            {
                // 이전 게임 데이터가 존재하는 경우
                var previousGameData = Managers.Data.LocalPlayerData.gameData;
                previousWorldID = previousGameData.world;
                previousStageNumber = previousGameData.stage;
                
                Debug.Log($"[Run_Start] 이전 런 데이터 발견: PreviousWorldID={previousWorldID}, PreviousStageNumber={previousStageNumber}");
            }
            else
            {
                // 최초 플레이인 경우
                Debug.Log($"[Run_Start] 최초 플레이 - 이전 런 데이터 없음");
            }
            
            // Analytics 이벤트 전송
            if (AnalyticsManager.Instance != null)
            {
                AnalyticsManager.Instance.LogRunStart(
                    selectedShip, 
                    selectedCharacter, 
                    selectedDifficulty.ToString(),
                    previousWorldID,
                    previousStageNumber
                );
                
                Debug.Log($"[Run_Start] SelectedShip={selectedShip}, SelectedCharacter={selectedCharacter}, SelectedDifficulty={selectedDifficulty}, PreviousWorldID={previousWorldID}, PreviousStageNumber={previousStageNumber}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Run_Start] 로깅 실패: {e.Message}");
        }
    }
    
    /// <summary>
    /// 선택된 비행선 이름 가져오기
    /// </summary>
    private string GetSelectedShipName()
    {
        int shipIdx = selectVehicleCanvas.NowSelectedIdx;
        
        // 무작위 선택(-1)인 경우
        if (shipIdx == -1)
        {
            return "Random";
        }
        
        // 실제 선택된 비행선의 nameKey 반환
        if (shipIdx >= 0 && shipIdx < Managers.Asset.Vehicles.Length)
        {
            var playerStatus = Managers.Asset.Vehicles[shipIdx].GetComponent<PlayerStatus>();
            return playerStatus != null ? playerStatus.playerNameKey : $"Ship_{shipIdx}";
        }
        
        return $"Ship_{shipIdx}"; // 혹시 모를 경우 인덱스 반환
    }
    
    /// <summary>
    /// 선택된 캐릭터 이름 가져오기
    /// </summary>
    private string GetSelectedCharacterName()
    {
        int characterIdx = selectCharacterCanvas.NowSelectedIdx;
        
        // 무작위 선택(-1)인 경우
        if (characterIdx == -1)
        {
            return "Random";
        }
        
        // 실제 선택된 캐릭터의 nameKey 반환
        if (characterIdx >= 0 && characterIdx < Managers.Asset.Characters.Length)
        {
            var character = Managers.Asset.Characters[characterIdx].GetComponent<Character>();
            return character != null ? character.characterNameKey : $"Character_{characterIdx}";
        }
        
        return $"Character_{characterIdx}"; // 혹시 모를 경우 인덱스 반환
    }
}
