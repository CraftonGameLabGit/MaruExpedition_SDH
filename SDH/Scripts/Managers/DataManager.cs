using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


[System.Serializable]
public class PlayerData // ���� ��ü ������
{
    public string playerName = "Default";

    public GameData gameData = null; // ���� �������� ���� ������

    public List<long> unlockConditionList = new List<long>(new long[Enum.GetValues(typeof(EUnlockType)).Length]);
}

[System.Serializable] // GameObject�� Dictionary�� ����ȭ �� �� ����
public class GameData // ���� �� ���� ������
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
    public List<CharacterData> characters; // charactersIdx�� ���׷��̵� ���
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

    public GameData() // ���� ���縦 ������ �ȵȴٴ� ��
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
public class CharacterData // ���׷��̵� ���¸� ������ ���� ����
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

    public void Init() // �Ѹ� �ڵ� �ε�
    {
        localPlayerData = LoadData() ?? new();
    }

    public void Clear() // ���� �� �ڵ�����
    {
        Debug.Log("�� ������ ���� ��");
        SaveData(localPlayerData);
    }

    public void SaveData(PlayerData data)
    {
        filePath = Path.Combine(Application.persistentDataPath, "PlayerData.bin");

        BinaryFormatter formatter = new();
        using FileStream stream = new(filePath, FileMode.Create);
        formatter.Serialize(stream, data);
    }

    public void SaveGameData() // �ΰ��� �����͸� ����
    {
        Debug.Log("�ΰ��� ������ ���� ��");
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

    public void ResetData() // ������ �ʱ�ȭ
    {
        localPlayerData = new PlayerData();
    }
}
