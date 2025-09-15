using System;
using UnityEngine;

public class StatusManager // 인게임 플레이 스탯 관리
{
    public FindAnyGoldTxt goldTxt;

    // 추가: HP 변경 이벤트
    public event Action OnHpChanged;

    public int Gold
    {
        get
        {
            return gold;
        }
        set
        {
            gold = value;

            goldTxt?.SetText();
        }
    }
    private int gold; // 인게임 재화

    public int TotalGold
    {
        get
        {
            return totalGold;
        }
        set
        {
            totalGold = value;
        }
    }
    private int totalGold; // 인게임 재화

    // 🎯 골드 수집률 추적 변수 추가
    public int StageCoinsDropped { get; private set; } // 스테이지에서 떨어진 코인 개수
    public int StageCoinsCollected { get; private set; } // 스테이지에서 수집한 코인 개수

    public float MaxHp
    {
        get
        {
            return maxHp;
        }
        set
        {
            maxHp = value;
        }
    }
    private float maxHp; // 인게임에 진입할 때마다 재설정되는 체력값
    public float Hp
    {
        get
        {
            return hp;
        }
        set
        {
            hp = value;
            OnHpChanged?.Invoke(); // HP가 바뀔 때마다 이벤트 호출
            Managers.Data.LocalPlayerData.gameData.hp = hp; // 체력은 도르마무 불가능하게 줄어들 때마다 저장
        }
    }
    private float hp; // 인게임 체력
    public int RiderCount
    {
        get
        {
            return riderCount;
        }
        set
        {
            if (value > riderCount) Managers.Cam?.LandCharacter(); // 착지했다면 카메라 흔들기
            riderCount = value;
            Managers.PlayerControl.NowPlayer.GetComponent<PlayerMPCanvas>().SetMPCanvas();
        }
    }
    private int riderCount = 0; // 비행체에 탄 인원 수

    public void StartGame() // 게임 시작. 루트 함수는 Stage임
    {
        gold = 0;
        totalGold = 0;
        maxHp = 100;
        hp = maxHp; // 체력 저장 기능 때문에 게임 시작 및 상점 씬 시작 때 초기화

        // 🎯 코인 수집률 초기화
        ResetStageCoins();
    }

    public void LoadGame()
    {
        gold = Managers.Data.LocalPlayerData.gameData.gold;
        totalGold = Managers.Data.LocalPlayerData.gameData.totalGold;
        maxHp = Managers.Data.LocalPlayerData.gameData.maxHp;
        hp = Managers.Data.LocalPlayerData.gameData.hp; // 체력 저장 기능 때문에 게임 시작 및 상점 씬 시작 때 초기화
        OnHpChanged?.Invoke(); // HP가 바뀌었으니 이벤트 호출

        // 🎯 코인 수집률 초기화 (게임 이어하기 시)
        ResetStageCoins();
    }
    
    /// <summary>
    /// 스테이지 시작 시 코인 개수 초기화
    /// </summary>
    public void ResetStageCoins()
    {
        StageCoinsDropped = 0;
        StageCoinsCollected = 0;
        Debug.Log("[StatusManager] 스테이지 코인 개수 초기화됨");
    }
    
    /// <summary>
    /// 코인이 떨어질 때 호출
    /// </summary>
    public void AddCoinDropped(int count = 1)
    {
        StageCoinsDropped += count;
        Debug.Log($"[StatusManager] 코인 드롭: +{count}, 총 떨어진 개수: {StageCoinsDropped}");
    }
    
    /// <summary>
    /// 코인을 수집할 때 호출
    /// </summary>
    public void AddCoinCollected(int count = 1)
    {
        StageCoinsCollected += count;
        Debug.Log($"[StatusManager] 코인 수집: +{count}, 총 수집 개수: {StageCoinsCollected}");
    }
    
    /// <summary>
    /// 현재 스테이지의 골드 수집률 계산 (0.0 ~ 1.0)
    /// </summary>
    public float GetGoldCollectRate()
    {
        if (StageCoinsDropped == 0)
        {
            return 1.0f; // 코인이 하나도 안 떨어진 경우 100%로 간주
        }
        
        float rate = (float)StageCoinsCollected / StageCoinsDropped;
        Debug.Log($"[StatusManager] 골드 수집률: {StageCoinsCollected}/{StageCoinsDropped} = {rate:F2} ({rate * 100:F1}%)");
        return rate;
    }
}
