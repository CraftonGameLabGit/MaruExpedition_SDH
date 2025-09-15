using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


[System.Serializable]
public class PlayerData // 게임 전체 데이터
{
    public string playerName = "Default";

    public GameData gameData = null; // 현재 진행중인 게임 데이터

    public List<long> unlockConditionList = new List<long>(new long[Enum.GetValues(typeof(EUnlockType)).Length]);
}

[System.Serializable] // GameObject나 Dictionary는 직렬화 할 수 없음
public class GameData // 게임 한 판의 데이터
{
    // Stage
    public float playTime;
    public int difficulty;
    public int world;
    public int stage;
    public bool onField;
    public int enemyTotalKill;
    public int enemyKill;
    // PlayerControl
    public int vehicleIdx;
    public bool[] charactersCheck;
    public List<CharacterData> characters; // charactersIdx와 업그레이드 기록
    // Status
    public int gold;
    public int totalGold;
    public float maxHp;
    public float hp;
    // Shop
    public bool isHired;
    public int tipIdx;
    public bool[] tipIdxList;
    // Record
    public List<KeyValuePair<ECharacterType, int>> totalDamageRecord;
    // SteamAchievement
    public bool isNoGold;
    public bool isNoDamage;
    public bool isNoUpgrade;

    public GameData() // 깊은 복사를 잊으면 안된다는 점
    {
        playTime = Managers.Stage.PlayTime;
        difficulty = Managers.Stage.Difficulty;
        world = Managers.Stage.World;
        stage = Managers.Stage.Stage;
        onField = Managers.Stage.OnField;
        enemyTotalKill = Managers.Stage.EnemyTotalKill;
        enemyKill = Managers.Stage.EnemyKill;
        vehicleIdx = Managers.PlayerControl.VehicleIdx;
        charactersCheck = Managers.PlayerControl.CharactersCheck.ToArray();
        characters = Enumerable.Range(0, Managers.PlayerControl.CharactersIdx.Count).Select(x => new CharacterData(x)).ToList();
        gold = Managers.Status.Gold;
        totalGold = Managers.Status.TotalGold;
        maxHp = Managers.Status.MaxHp;
        hp = Managers.Status.Hp;
        isHired = Managers.Shop.IsHired;
        tipIdx = Managers.Shop.TipIdx;
        tipIdxList = Managers.Shop.TipIdxList.ToArray();
        totalDamageRecord = Managers.Record.TotalDamageRecordDictToList();
        isNoGold = Managers.SteamAchievement.IsNoGold;
        isNoDamage = Managers.SteamAchievement.IsNoDamage;
        isNoUpgrade = Managers.SteamAchievement.IsNoUpgrade;
    }
}

[System.Serializable]
public class CharacterData // 업그레이드 상태를 포함한 동료 정보
{
    public int characterIdx;
    public List<string> upgrades;
    public List<float> upgradeValues;

    public CharacterData(int idx)
    {
        characterIdx = Managers.PlayerControl.CharactersIdx[idx];
        UpgradeController upgradeController = Managers.PlayerControl.Characters[idx].GetComponent<UpgradeController>();
        upgrades = upgradeController.SaveUpgrade();
        upgradeValues = upgradeController.SaveUpgradeValue();
    }
}

public class DataManager
{
    public PlayerData LocalPlayerData => localPlayerData;
    private PlayerData localPlayerData;
    private string filePath;

    public void Init() // 켜면 자동 로드
    {
        localPlayerData = LoadData() ?? new();
    }

    public void Clear() // 종료 시 자동저장
    {
        Debug.Log("총 데이터 저장 중");
        SaveData(localPlayerData);
    }

    public void SaveData(PlayerData data)
    {
        filePath = Path.Combine(Application.persistentDataPath, "PlayerData.bin");

        BinaryFormatter formatter = new();
        using FileStream stream = new(filePath, FileMode.Create);
        formatter.Serialize(stream, data);
    }

    public void SaveGameData() // 인게임 데이터만 저장
    {
        Debug.Log("인게임 데이터 저장 중");
        localPlayerData.gameData = new();
    }

    private PlayerData LoadData()
    {
        filePath = Path.Combine(Application.persistentDataPath, "PlayerData.bin");

        if (File.Exists(filePath))
        {
            BinaryFormatter formatter = new();
            using FileStream stream = new(filePath, FileMode.Open);
            if (stream.Length == 0) return null;
            return (PlayerData)formatter.Deserialize(stream);
        }

        return null;
    }

    public void ResetData() // 데이터 초기화
    {
        localPlayerData = new PlayerData();
    }
}
