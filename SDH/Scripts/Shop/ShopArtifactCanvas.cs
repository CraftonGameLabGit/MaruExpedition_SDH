using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopArtifactCanvas : MonoBehaviour
{
    [SerializeField] RectTransform[] artifactItems; // ��Ƽ��Ʈ ����Ʈ (�������� ���ư��� ��ư)
    [SerializeField] private ShopCanavs shopCanavs;
    private int nowArtifactSelectedIdx;
    private int[] artifactsIdx; // ���� ������ ��Ƽ��Ʈ �ε���

    private void Start()
    {
        SetArtifact();
    }
    
    public void StartShopArtifactCanvas() // �Է� �޴� �� �Ӹ� �ƴ϶� ��Ƽ��Ʈ ����â ���⵵ �߰�
    {
        StartCoroutine(StartDirect(1f));
        SetNowArtifactSelectIdx(artifactItems.Length - 1);
    }

    private IEnumerator StartDirect(float maxTime)  // ������ ���� �� �Է¹ޱ�
    {
        float nowTime = 0f;

        while (nowTime <= maxTime)
        {
            for (int i = 0; i < artifactItems.Length; i++)
            {
                artifactItems[i].localPosition = Vector3.Lerp(artifactItems[i].localPosition, new(0f, 240f - 110f * i, 0f), 0.2f);
            }

            nowTime += maxTime;
            yield return null;
        }

        for (int i = 0; i < artifactItems.Length; i++)
        {
            artifactItems[i].localPosition = new(0f, 240f - 110f * i, 0f);
        }

        Managers.InputControl.UseAction += Select;
        //Managers.InputControl.MoveAction += GetInput;
    }

    private void Select()
    {
        if (nowArtifactSelectedIdx == artifactItems.Length - 1)
        {
            Managers.InputControl.ResetUIAction();
            StartCoroutine(EndDirect(1f));
        }
        else
        {
            BuyArtifact();
        }
    }

    private void GetInput()
    {
        //if (Managers.InputControl.InputMove.y < -Constant.deadZone) SetNowArtifactSelectIdx(nowArtifactSelectedIdx + 1);
        //else if (Managers.InputControl.InputMove.y > Constant.deadZone) SetNowArtifactSelectIdx(nowArtifactSelectedIdx - 1);
    }

    private void SetNowArtifactSelectIdx(int newIdx)
    {
        if (newIdx < 0 || newIdx > artifactItems.Length - 1) return; // �������̶�� ���� ����

        artifactItems[nowArtifactSelectedIdx].GetComponent<Image>().color = Color.white;
        nowArtifactSelectedIdx = newIdx;
        artifactItems[nowArtifactSelectedIdx].GetComponent<Image>().color = Color.green;
    }

    private void SetArtifact() // ���� ���� �� ��Ƽ��Ʈ ����
    {
        artifactsIdx = new int[artifactItems.Length - 1];

        for(int i = 0; i < artifactsIdx.Length; i++)
        {
            if (Managers.Artifact.ArtifactCounts + i >= Managers.Artifact.ArtifactLists.Count) // ��Ƽ��Ʈ�� �� á�ٸ�
            {
                artifactsIdx[i] = -2;
                continue;
            }

            do
            {
                artifactsIdx[i] = Random.Range(0, Managers.Artifact.ArtifactLists.Count);
            }
            while (IsRedundant(i, artifactsIdx[i])); // �� ���� �ʾҴٸ� �������� ���� ������ ä���
        }

        SetArtifactText();
    }

    private bool IsRedundant(int idx ,int nowArtifactIdx) // ���� �ߺ� Ȯ�� true�� �ߺ�
    {
        for(int i = 0; i < idx; i++)
        {
            if (nowArtifactIdx == artifactsIdx[i]) return true;
        }

        return Managers.Artifact.ArtifactLists[nowArtifactIdx].artifactBaseTemplate.isPurchased;
    }

    private void SetArtifactText() // ��Ƽ��Ʈ �ؽ�Ʈ ����
    {
        for (int i = 0; i < artifactsIdx.Length; i++)
        {
            if (artifactsIdx[i] == -2)
            {
                artifactItems[i].GetComponentInChildren<TextMeshProUGUI>().text = "(�԰� ��)";
            }
            else if (artifactsIdx[i] == -1)
            {
                artifactItems[i].GetComponentInChildren<TextMeshProUGUI>().text = "(ǰ��)";
            }
            else
            {
                artifactItems[i].GetComponentInChildren<TextMeshProUGUI>().text = Managers.Artifact.ArtifactLists[artifactsIdx[i]].artifactBaseTemplate.explain;
            }
        }
    }

    private IEnumerator EndDirect(float maxTime) // �Է� ���� �� ������ ���� �� ���� �Է� ����
    {
        Managers.InputControl.ResetUIAction();

        float nowTime = 0f;

        while (nowTime <= maxTime)
        {
            for (int i = 0; i < artifactItems.Length; i++)
            {
                artifactItems[i].localPosition = Vector3.Lerp(artifactItems[i].localPosition, new(0f, 350f, 0f), 0.2f);
            }

            nowTime += maxTime;
            yield return null;
        }

        for (int i = 0; i < artifactItems.Length; i++)
        {
            artifactItems[i].localPosition = new(0f, 350f, 0f);
        }

        GetComponent<Canvas>().enabled = false;
        shopCanavs.StartShopCanvas();
    }

    public void BuyArtifact()
    {
        if (artifactsIdx[nowArtifactSelectedIdx] < 0) return;
        if (Managers.Status.Gold < 300) return;

        Managers.Status.Gold -= 300;
        Managers.Artifact.BuyArtifact(artifactsIdx[nowArtifactSelectedIdx]);
        ApplyOnceArtifact(artifactsIdx[nowArtifactSelectedIdx]);
        artifactsIdx[nowArtifactSelectedIdx] = -1;

        SetArtifactText();
    }

    private void ApplyOnceArtifact(int nowArtifactIdx) // 1ȸ�� ��Ƽ��Ʈ ����
    {
        switch (nowArtifactIdx)
        {
            case 5:
                //Managers.Artifact.ApplyArtifact(5, Managers.PlayerControl.NowPlayer.GetComponentInChildren<PlayerHP>());
                break;
        }
    }
}