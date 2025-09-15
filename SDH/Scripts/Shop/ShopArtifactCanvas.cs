using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopArtifactCanvas : MonoBehaviour
{
    [SerializeField] RectTransform[] artifactItems; // 아티팩트 리스트 (마지막은 돌아가기 버튼)
    [SerializeField] private ShopCanavs shopCanavs;
    private int nowArtifactSelectedIdx;
    private int[] artifactsIdx; // 구매 가능한 아티팩트 인덱스

    private void Start()
    {
        SetArtifact();
    }
    
    public void StartShopArtifactCanvas() // 입력 받는 것 뿐만 아니라 아티팩트 구매창 연출도 추가
    {
        StartCoroutine(StartDirect(1f));
        SetNowArtifactSelectIdx(artifactItems.Length - 1);
    }

    private IEnumerator StartDirect(float maxTime)  // 연출이 끝난 뒤 입력받기
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
        if (newIdx < 0 || newIdx > artifactItems.Length - 1) return; // 고정값이라는 점에 유의

        artifactItems[nowArtifactSelectedIdx].GetComponent<Image>().color = Color.white;
        nowArtifactSelectedIdx = newIdx;
        artifactItems[nowArtifactSelectedIdx].GetComponent<Image>().color = Color.green;
    }

    private void SetArtifact() // 상점 입장 시 아티팩트 설정
    {
        artifactsIdx = new int[artifactItems.Length - 1];

        for(int i = 0; i < artifactsIdx.Length; i++)
        {
            if (Managers.Artifact.ArtifactCounts + i >= Managers.Artifact.ArtifactLists.Count) // 아티팩트가 다 찼다면
            {
                artifactsIdx[i] = -2;
                continue;
            }

            do
            {
                artifactsIdx[i] = Random.Range(0, Managers.Artifact.ArtifactLists.Count);
            }
            while (IsRedundant(i, artifactsIdx[i])); // 다 차지 않았다면 구매하지 않은 유물로 채우기
        }

        SetArtifactText();
    }

    private bool IsRedundant(int idx ,int nowArtifactIdx) // 유물 중복 확인 true면 중복
    {
        for(int i = 0; i < idx; i++)
        {
            if (nowArtifactIdx == artifactsIdx[i]) return true;
        }

        return Managers.Artifact.ArtifactLists[nowArtifactIdx].artifactBaseTemplate.isPurchased;
    }

    private void SetArtifactText() // 아티팩트 텍스트 설정
    {
        for (int i = 0; i < artifactsIdx.Length; i++)
        {
            if (artifactsIdx[i] == -2)
            {
                artifactItems[i].GetComponentInChildren<TextMeshProUGUI>().text = "(입고 중)";
            }
            else if (artifactsIdx[i] == -1)
            {
                artifactItems[i].GetComponentInChildren<TextMeshProUGUI>().text = "(품절)";
            }
            else
            {
                artifactItems[i].GetComponentInChildren<TextMeshProUGUI>().text = Managers.Artifact.ArtifactLists[artifactsIdx[i]].artifactBaseTemplate.explain;
            }
        }
    }

    private IEnumerator EndDirect(float maxTime) // 입력 종료 후 연출이 끝난 뒤 메인 입력 시작
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

    private void ApplyOnceArtifact(int nowArtifactIdx) // 1회성 아티팩트 적용
    {
        switch (nowArtifactIdx)
        {
            case 5:
                //Managers.Artifact.ApplyArtifact(5, Managers.PlayerControl.NowPlayer.GetComponentInChildren<PlayerHP>());
                break;
        }
    }
}