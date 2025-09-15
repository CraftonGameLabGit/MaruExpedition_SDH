using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager // 씬 전환 관리 (전투-상점 등)
{
    public EnemySpawner EnemySpawner;

    public float PlayTime { get { return playTime; } set { playTime = value; } }
    private float playTime; // 플레이한 시간

    public bool GameOver { get { return gameOver; } set { gameOver = value; } }   // 게임오버 후에 적이 죽어도 다음 스테이지로 넘어가는 버그 막기용 
    private bool gameOver; 

    public int Difficulty {
        get
        {
            return difficulty;
        }
        set
        {
            difficulty = value;
        }
    }
    private int difficulty; // 승천
    public int World
    {
        get
        {
            return world;
        }
        set
        {
            world = value;
        }
    }
    private int world; // 월드 번호, 1-2 일 경우 1
    public int Stage
    {
        get
        {
            return stage;
        }
        set
        {
            stage = value;
        }
    }
    private int stage; // 스테이지 번호, 1-2 일 경우 2
    public StageSO NowStage => nowStage;
    private StageSO nowStage; // 현재 스테이지 정보
    public bool OnField
    {
        get
        {
            return onField;
        }
        set
        {
            onField = value;
            if (onField) // 상점 끝나고 스테이지로
            {
                Managers.Record.ClearStageDamageRecord(); // 스테이지 끝나고 피해량 기록 초기화
                                                          // 🎯 새 스테이지 시작 시 코인 개수 초기화
                Managers.Status.ResetStageCoins();

            }
            else // 스테이지 끝나고 상점으로
            {
                if (gameOver) return;   // 게임오버 후에 적이 죽어도 다음 스테이지로 넘어가는 버그 막기용 

                // 🎯 Stage_Result 이벤트 로깅 (성공)
                LogStageResultEvent("Win");

                if (Managers.Stage.Difficulty == 0 && world == 3 && stage == 4 || world == 4 && stage == 1) // 마지막 스테이지 클리어 (엔딩씬)
                {
                    Managers.Record.AddTotalDamageRecord(); // 스테이지 끝나고 총 데미지 레코드에 기록을 더하기

                    // 🎯 Run_End 이벤트 로깅 (게임 클리어 시)
                    LogRunEndEvent();

                    Managers.SceneFlow.FieldCanvas.StartGameClearDirect(1f);
                    EnemySpawner.StopAllCoroutines();
                    Managers.Stage.EnemySpawner.DeleteField();

                    Debug.Log("현재 스테이지 끝");
                    Managers.PlayerControl.NowPlayer.GetComponentInChildren<PlayerHP>().isClear = true;
                }
                else
                {
                    Managers.PlayerControl.NowPlayer.GetComponent<PlayerControl>().StageEnd();
                    Debug.Log("현재 스테이지 끝");
                    Managers.PlayerControl.NowPlayer.GetComponentInChildren<PlayerHP>().isEndFieldNoDamage = true;

                    Managers.Status.Gold += 200;
                    Managers.Status.TotalGold += 200;           // JSW가 추가했는데 Gold량 바꿀 때 같이 바꿔줘요 총 골드량이예요

                    Managers.Shop.IsHired = false; // 다시 상점에서 동료 고용 가능

                    SoundManager.Instance.PlaySFX("GameStageClear");      // JSW Stage Clear Sound

                    //if (Managers.PlayerControl.Characters.Count == 4) // 총 4명인 경우에만 데미지 레코드에 기록 더하기
                    //{
                    Managers.Record.AddTotalDamageRecord(); // 스테이지 끝나고 총 데미지 레코드에 기록을 더하기
                    //}
                    Managers.Record.PrintAllDamageRecord(isStage: true); // 스테이지에서 가한 피해량 출력
                    Managers.Record.PrintAllDamageRecord(isStage: false); // 총 가한 피해량 출력
                    //Managers.Record.PrintAllJumpTimeRecord(); // 모든 캐릭터의 점프 시간 기록 출력

                }
            }
        }
    } // ************************************************************** 이 줄에 코드 많음
    private bool onField; // true면 상점->필드, false면 필드->상점 이 변수가 호출되었다는 것은 스테이지나 상점이 끝났다는 의미
    public int EnemyTotalKill
    {
        get
        {
            return enemyTotalKill;
        }
        set
        {
            enemyTotalKill = value;
        }
    }
    private int enemyTotalKill; // 이번 게임에서 잡은 적 수
    public int EnemyKill { get { return enemyKill; } set { enemyKill = value; } }
    private int enemyKill; // 동전 생성용 잡은 적 수
    public bool PossCoin { get { return possCoin; } set { possCoin = value; } }
    private bool possCoin; // 마지막 웨이브에서 적이 얼마 안 남았다면 동전을 생성하지 않도록 보정

    public int CurEnemyCount
    {
        get
        {
            return curEnemyCount;
        }
        set
        {
            curEnemyCount = value;
        }
    }
    private int curEnemyCount; // 현재 스테이지에서 남아있는 적 수

    private int[] coinPerEnemyCount = { 10, 10, 8, 6}; // 난이도에 따른 적을 몇마리 처치해야 코인 1개가 생성되는지
    //private ValueTuple<int, int>[] coinMinMaxValue = new ValueTuple<int, int>[] // 난이도에 따른 코인 최소, 최대값
    //{
    //    (30, 40), // 0
    //    (30, 40), // 1
    //    (35, 45), // 2
    //    (40, 50)  // 3
    //};


    public void StartGame() // 게임 시작. 다른 매니저의 게임 시작도 이곳에서 (nowPlayer와 characters를 설정한 뒤 실행해야 함)
    {
        playTime = 0f;
        gameOver = false;
        nowStage = null;
        world = 1;
        stage = 1; // 상점에서 값을 1씩 추가하므로 1부터 시작
        enemyTotalKill = 0;
        onField = true;

        Managers.PlayerControl.StartGame();
        Managers.Status.StartGame();
        Managers.Shop.StartGame();
        //Managers.Artifact.StartGame();
        Managers.Record.ResetRecord();          // JSW 추가 게임 시작하면서 딜로그 기록 리셋
        Managers.SteamAchievement.StartGame();
    }

    public void LoadGame() // 기존 게임 이어서. 다른 매니저의 게임 이어서도 이곳에서 (비행체와 캐릭터 오브젝트 생성한 뒤 실행해야 함)
    {
        if (Managers.Data.LocalPlayerData.gameData == null) return; // 저장된 데이터가 없음

        playTime = Managers.Data.LocalPlayerData.gameData.playTime;
        gameOver = false;
        difficulty = Managers.Data.LocalPlayerData.gameData.difficulty;
        world = Managers.Data.LocalPlayerData.gameData.world;
        stage = Managers.Data.LocalPlayerData.gameData.stage;
        onField = Managers.Data.LocalPlayerData.gameData.onField;
        enemyTotalKill = Managers.Data.LocalPlayerData.gameData.enemyTotalKill;
        enemyKill = Managers.Data.LocalPlayerData.gameData.enemyKill;

        Managers.PlayerControl.LoadGame();
        Managers.Status.LoadGame();
        Managers.Shop.LoadGame();
        Managers.Record.LoadGame();
        Managers.SteamAchievement.LoadGame();
    }

    public void ResetGame() // 게임 끝. 다른 매니저의 게임 끝도 이곳에서
    {
        Managers.Data.LocalPlayerData.gameData = null; // 현재 게임 기록 삭제
        Managers.PlayerControl.ResetGame();
    }

    public void SetStage() // 현재 스테이지 시작하며 기본 설정
    {
        //Managers.Artifact.ApplyArtifact(0);
        //Managers.Status.Hp = Managers.Status.MaxHp; // 체력 저장 기능 때문에 게임 시작 및 상점 씬 시작 때 초기화
        curEnemyCount = 0;
        nowStage = Array.Find(Managers.Asset.StageTemplates, stageSO => stageSO.world == world && stageSO.stage == stage);
    }

    public void StartStage() // 현재 스테이지 시작. 위쪽 SetStage 다음에 실행되어야 함
    {
        EnemySpawner.StartSpawnEnemy();
        Managers.PlayerControl.NowPlayer.GetComponent<PlayerMPCanvas>().StartManaCanvas();
        Managers.Status.RiderCount = Managers.PlayerControl.Characters.Count;
    }

    public void GoNextStage()
    {
        if (Managers.Stage.Stage != Managers.Asset.StageCounts[Managers.Stage.World]) // 이 코드는 상점이 끝날 때 실행되니 1-1전투 1-1상점 1-2전투... 식으로 진행됨
        {
            Managers.Stage.Stage++;
        }
        else
        {
            Managers.Stage.World++;
            Managers.Stage.Stage = 1;
        }
    }

    public void PlusEnemyKill(Vector3 position) // 적 처치 수 증가
    {
        if (SceneManager.GetActiveScene().name == "TestRoom") return; // 이거 꼭 지워야함 테스트룸 용임 TestRoom

        enemyTotalKill++;
        if (!nowStage.isBossStage) enemyKill++; // 보스가 소환한 몬스터는 동전 생성 킬카운트하지 않음
        if (!nowStage.isBossStage && enemyKill >= coinPerEnemyCount[Difficulty]) // 10, 10, 8, 6마리마다 코인 생성
        {
            enemyKill -= coinPerEnemyCount[Difficulty];
            SpawnCoin(position);
        }
    }

    public void SpawnCoin(Vector3 position, int coinCount = 1) // 코인 생성
    {
        // 🎯 떨어진 코인 개수 추적
        Managers.Status.AddCoinDropped(coinCount);
        
        for (int i = 0; i < coinCount; i++)
        {
            GameObject coinObj = UnityEngine.Object.Instantiate(Managers.Asset.Coin, position, Quaternion.identity);
            coinObj.GetComponent<Coin>().SetCoinValue(UnityEngine.Random.Range(30, 40)); // 코인 값은 난이도에 따라 달라지는 랜덤값
        }
    }

    /// <summary>
    /// Stage_Result 이벤트 로깅 - 실제 게임 데이터 사용
    /// </summary>
    private void LogStageResultEvent(string result)
    {
        try
        {
            // 현재 스테이지 정보 가져오기
            int worldID = world;
            int stageNumber = stage;
            int difficulty = this.difficulty;
            float stageTime = playTime;
            
            // 데미지 요약 생성 (CSV 형태)
            string damageSummary = GenerateDamageSummary();
            
            // 🎯 실제 골드 수집률 계산
            float goldCollectRate = Managers.Status.GetGoldCollectRate();
            
            // Analytics 이벤트 전송
            if (AnalyticsManager.Instance != null)
            {
                AnalyticsManager.Instance.LogStageResult(
                    worldID, stageNumber, difficulty.ToString(), result,
                    stageTime, damageSummary, goldCollectRate
                );
                
                Debug.Log($"[Stage_Result] WorldID={worldID}, StageNumber={stageNumber}, Difficulty={difficulty}, Result={result}, StageTime={stageTime:F1}s, DamageSummary={damageSummary}, GoldCollectRate={goldCollectRate:F2}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Stage_Result] 로깅 실패: {e.Message}");
        }
    }
    
    /// <summary>
    /// 데미지 요약 CSV 문자열 생성
    /// </summary>
    private string GenerateDamageSummary()
    {
        try
        {
            var stageDamageRecord = new System.Collections.Generic.Dictionary<ECharacterType, int>();
            
            // 현재 스테이지 데미지 기록 가져오기
            foreach (ECharacterType charType in System.Enum.GetValues(typeof(ECharacterType)))
            {
                int damage = Managers.Record.GetDamageRecord(true, charType); // true = 스테이지 데미지
                if (damage > 0)
                {
                    stageDamageRecord[charType] = damage;
                }
            }
            
            // CSV 형태로 변환: "Archer:1000,Magician:500"
            var damageEntries = new System.Collections.Generic.List<string>();
            foreach (var kvp in stageDamageRecord)
            {
                damageEntries.Add($"{kvp.Key}:{kvp.Value}");
            }
            
            return damageEntries.Count > 0 ? string.Join(",", damageEntries) : "NoDamage:0";
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Stage_Result] 데미지 요약 생성 실패: {e.Message}");
            return "Error:0";
        }
    }
    
    /// <summary>
    /// Run_End 이벤트 로깅 - 실제 게임 데이터 사용
    /// </summary>
    private void LogRunEndEvent()
    {
        try
        {
            // 총 플레이 시간
            float totalPlayTime = playTime;
            
            // 사용한 캐릭터들 (CSV 형태)
            string charactersUsed = GenerateCharactersUsedCSV();
            
            // 데미지 요약 (총 데미지)
            string damageSummary = GenerateTotalDamageSummary();
            
            // 업그레이드 요약
            string upgradeSummary = GenerateUpgradeSummary();
            
            // 현재 난이도
            int difficulty = this.difficulty;
            
            // Analytics 이벤트 전송
            if (AnalyticsManager.Instance != null)
            {
                AnalyticsManager.Instance.LogRunEnd(
                    totalPlayTime, charactersUsed, damageSummary, 
                    upgradeSummary, difficulty.ToString()
                );
                
                Debug.Log($"[Run_End] TotalPlayTime={totalPlayTime:F1}s, CharactersUsed={charactersUsed}, DamageSummary={damageSummary}, UpgradeSummary={upgradeSummary}, Difficulty={difficulty}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Run_End] 로깅 실패: {e.Message}");
        }
    }
    
    /// <summary>
    /// 사용한 캐릭터들 CSV 문자열 생성
    /// </summary>
    private string GenerateCharactersUsedCSV()
    {
        try
        {
            var characterNames = new System.Collections.Generic.List<string>();
            
            // 현재 파티에 있는 캐릭터들의 nameKey 수집
            foreach (var characterObj in Managers.PlayerControl.Characters)
            {
                if (characterObj != null)
                {
                    var character = characterObj.GetComponent<Character>();
                    string characterName = character != null ? character.characterNameKey : "UnknownCharacter";
                    characterNames.Add(characterName);
                }
            }
            
            return characterNames.Count > 0 ? string.Join(",", characterNames) : "NoCharacters";
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Run_End] 캐릭터 목록 생성 실패: {e.Message}");
            return "Error";
        }
    }
    
    /// <summary>
    /// 총 데미지 요약 CSV 문자열 생성
    /// </summary>
    private string GenerateTotalDamageSummary()
    {
        try
        {
            var totalDamageRecord = new System.Collections.Generic.Dictionary<ECharacterType, int>();
            
            // 총 데미지 기록 가져오기 (false = 총 데미지)
            foreach (ECharacterType charType in System.Enum.GetValues(typeof(ECharacterType)))
            {
                int damage = Managers.Record.GetDamageRecord(false, charType); // false = 총 데미지
                if (damage > 0)
                {
                    totalDamageRecord[charType] = damage;
                }
            }
            
            // CSV 형태로 변환: "Archer:25000,Magician:18000"
            var damageEntries = new System.Collections.Generic.List<string>();
            foreach (var kvp in totalDamageRecord)
            {
                damageEntries.Add($"{kvp.Key}:{kvp.Value}");
            }
            
            return damageEntries.Count > 0 ? string.Join(",", damageEntries) : "NoDamage:0";
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Run_End] 총 데미지 요약 생성 실패: {e.Message}");
            return "Error:0";
        }
    }
    
    /// <summary>
    /// 업그레이드 요약 CSV 문자열 생성
    /// </summary>
    private string GenerateUpgradeSummary()
    {
        try
        {
            var upgradeEntries = new System.Collections.Generic.List<string>();
            
            // 각 캐릭터의 업그레이드 정보 수집
            foreach (var characterObj in Managers.PlayerControl.Characters)
            {
                if (characterObj != null)
                {
                    var character = characterObj.GetComponent<Character>();
                    var upgradeController = characterObj.GetComponent<UpgradeController>();
                    
                    if (character != null && upgradeController != null)
                    {
                        string characterName = character.characterNameKey ?? "UnknownCharacter";
                        int characterLevel = upgradeController.characterLevel;
                        
                        // 획득한 업그레이드들
                        var acquiredUpgrades = upgradeController._acquiredUpgrades;
                        string upgradeNames = acquiredUpgrades.Count > 0 
                            ? string.Join("|", acquiredUpgrades.Select(u => u.nameKey ?? "UnknownUpgrade"))
                            : "NoUpgrades";
                        
                        // "CharacterName_Level_Upgrades" 형태
                        upgradeEntries.Add($"{characterName}_Lv{characterLevel}_{upgradeNames}");
                    }
                }
            }
            
            return upgradeEntries.Count > 0 ? string.Join(",", upgradeEntries) : "NoUpgrades";
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Run_End] 업그레이드 요약 생성 실패: {e.Message}");
            return "Error";
        }
    }
}
