using UnityEngine;

public class ShopManager // 상점 관리 (stageManager와는 다르게 상점 내 기능에 초점)
{
    public bool IsHired
    {
        get
        {
            return isHired;
        }
        set
        {
            isHired = value;
        }
    }
    private bool isHired; // 이번 상점에서 고용했는지 (세이브데이터용 변수니까 이 값은 필드전이 끝날 때 바꿔줘야 함)
    public int TipIdx
    {
        get
        {
            return tipIdx;
        }
        set
        {
            tipIdx = value;
        }
    }
    private int tipIdx; // 지금 상점에서 보여줄 팁.
    public bool[] TipIdxList => tipIdxList;
    private bool[] tipIdxList; // 보여준 팁 리스트

    public void StartGame()
    {
        tipIdxList = new bool[4];
    }

    public void LoadGame()
    {
        isHired = Managers.Data.LocalPlayerData.gameData.isHired;
        tipIdx = Managers.Data.LocalPlayerData.gameData.tipIdx;
        tipIdxList = Managers.Data.LocalPlayerData.gameData.tipIdxList;
    }

    public void NextTipIdx() // 다음에 보여줄 팁 설정
    {
        tipIdxList[tipIdx] = true;

        if (tipIdxList[0] && tipIdxList[1] && tipIdxList[2] && tipIdxList[3]) tipIdxList[0] = false; tipIdxList[1] = false; tipIdxList[2] = false; tipIdxList[3] = false; // 전부 봤다면 리셋

        tipIdx = Random.Range(0, 4);
        while (tipIdxList[tipIdx]) tipIdx = tipIdx == 3 ? 0 : tipIdx + 1;
    }
}
