using UnityEngine;

public class ShopManager // ���� ���� (stageManager�ʹ� �ٸ��� ���� �� ��ɿ� ����)
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
    private bool isHired; // �̹� �������� ����ߴ��� (���̺굥���Ϳ� �����ϱ� �� ���� �ʵ����� ���� �� �ٲ���� ��)
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
    private int tipIdx; // ���� �������� ������ ��.
    public bool[] TipIdxList => tipIdxList;
    private bool[] tipIdxList; // ������ �� ����Ʈ

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

    public void NextTipIdx() // ������ ������ �� ����
    {
        tipIdxList[tipIdx] = true;

        if (tipIdxList[0] && tipIdxList[1] && tipIdxList[2] && tipIdxList[3]) tipIdxList[0] = false; tipIdxList[1] = false; tipIdxList[2] = false; tipIdxList[3] = false; // ���� �ôٸ� ����

        tipIdx = Random.Range(0, 4);
        while (tipIdxList[tipIdx]) tipIdx = tipIdx == 3 ? 0 : tipIdx + 1;
    }
}
