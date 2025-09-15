using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ShopCanavs : MonoBehaviour
{
    [SerializeField] private Canvas curtain;
    [SerializeField] private ShopItemCanvas shopItemCanvas;
    [SerializeField] private ShopCharacterCanvas[] shopCharacterCanvas;
    [SerializeField] private ShopArtifactCanvas shopArtifactCanvas;
    [SerializeField] private RectTransform characterPanel;
    [SerializeField] private RectTransform dealPanel;
    [SerializeField] private RectTransform selectCursor;
    [SerializeField] private RectTransform readyIcon;
    private int nowShopSelectIdx; // 옵션 인덱스
    private IEnumerator selectEffect;

    public void StartShopCanvas()
    {
        selectCursor.GetComponent<Image>().enabled = true;
        shopItemCanvas.NowSelectedIdx = shopItemCanvas.NowSelectedIdx; // shopItemCanvas 재설정을 위한 코드

        Managers.InputControl.CoroutineRunner = this;
        Managers.InputControl.UseAction += Select;
        Managers.InputControl.QuitAction += HandleQuitAction; // QuitAction 구독 추가
        Managers.InputControl.SubScribeUIMove(GetLeft, GetRight, GetDown, GetUp);
        Managers.InputControl.MouseMoveAction += GetMouseInput;
        Managers.InputControl.MouseUseAction += Select;
        //GetMouseInput();
    }

    private void GetLeft() { SetNowSelectIdx(nowShopSelectIdx + 1); } // 인덱스 상 다른애들과 반대라는 것에 주의
    private void GetRight() { SetNowSelectIdx(nowShopSelectIdx - 1); }
    private void GetDown() { if (nowShopSelectIdx != 0) SetNowSelectIdx(0); } // 아래쪽을 누르면 바로 다음 스테이지 버튼으로
    private void GetUp() { if (nowShopSelectIdx == 0) SetNowSelectIdx(1); }

    private void GetMouseInput()
    {
        /*if (false) // 원래는 상점주인 마우스 감지 코드인데 미구현이라 막아둠
        {
            Managers.InputControl.PossInputMouseUse = true;
            SetNowSelectIdx(1);
            return;
        }*/
        if (RectTransformUtility.RectangleContainsScreenPoint(readyIcon, Managers.InputControl.InputMouseMove, GetComponent<Canvas>().worldCamera))
        {
            Managers.InputControl.PossInputMouseUse = true;
            SetNowSelectIdx(0);
            return;
        }
        for (int i = 0; i < characterPanel.childCount; i++)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(characterPanel.GetChild(i).GetComponent<RectTransform>(), Managers.InputControl.InputMouseMove, GetComponent<Canvas>().worldCamera))
            {
                Managers.InputControl.PossInputMouseUse = true;
                SetNowSelectIdx(i + 1);
                return;
            }
        }

        Managers.InputControl.PossInputMouseUse = false;
    }

    public void SetShopCharacter() // 상점창에 캐릭터 나열하고 캐릭터 별 정보창 설정
    {
        for (int i = 0; i < Managers.PlayerControl.Characters.Count; i++)
        {
            // 상점에 캐릭터 보이는 파트
            GameObject shopCharacter = Instantiate(Managers.Asset.OptionTemplate, characterPanel);
            Destroy(shopCharacter.GetComponent<Image>());
            shopCharacter.GetComponent<OptionTemplate>().SetCharacterIcon(Managers.PlayerControl.CharactersIdx[i]);
            SortingGroup sortingGroup = shopCharacter.AddComponent<SortingGroup>();
            sortingGroup.sortingLayerName = "FrontGround";
            // 정보창 설정하는 파트
            shopCharacterCanvas[i].SetShopCharacterCanvas(i);
        }


        // 기본값은 첫 동료 (1번 인덱스)
        nowShopSelectIdx = 1;
        shopItemCanvas.NowSelectedIdx = nowShopSelectIdx;
    }

    private void SetNowSelectIdx(int newIdx)
    {
        if (newIdx < 0 || newIdx > Managers.PlayerControl.Characters.Count) return;
        if (nowShopSelectIdx == newIdx) return;

        // 네비게이션 사운드 재생
        SoundManager.Instance.PlaySFX("ui_navigate");

        nowShopSelectIdx = newIdx;
        shopItemCanvas.NowSelectedIdx = nowShopSelectIdx;

        if (nowShopSelectIdx == 0) // 다음 스테이지로
        {
            selectCursor.GetComponent<Image>().pixelsPerUnitMultiplier = 1;
            selectCursor.position = readyIcon.position;
            selectCursor.sizeDelta = readyIcon.sizeDelta + Constant.cursorAddSize;
            dealPanel.gameObject.SetActive(false);
        }
        else
        {
            selectCursor.GetComponent<Image>().pixelsPerUnitMultiplier = 2;
            RectTransform rect = characterPanel.GetChild(nowShopSelectIdx - 1).GetComponent<RectTransform>();
            selectCursor.position = rect.position;
            selectCursor.sizeDelta = rect.sizeDelta + Constant.cursorAddSize;
            dealPanel.gameObject.SetActive(true);
            dealPanel.position = rect.position + Vector3.up * 8f;
        }

        if (selectEffect != null) StopCoroutine(selectEffect);
        StartCoroutine(selectEffect = SelectEffect(Constant.cursorEffectTime));
    }

    private void Select()
    {
        SoundManager.Instance.PlaySFX("ui_confirm");
        
        if (nowShopSelectIdx == 0)
        {
            Managers.InputControl.ResetUIAction();
            Managers.Shop.NextTipIdx();
            StartCoroutine(FadeOut());
            //Managers.Stage.GoNextStage();
            //Managers.Stage.OnField = true;
            //Managers.SceneFlow.GotoScene("Field");
        }
        else
        {
            Managers.InputControl.ResetUIAction();
            shopCharacterCanvas[nowShopSelectIdx - 1].GetComponent<Canvas>().enabled = true;
            shopCharacterCanvas[nowShopSelectIdx - 1].StartShopCharacterCanvas();
        }
    }

    private void HandleQuitAction()
    {
        // 이 메서드는 빈 상태로 두어서 QuitAction 이벤트가 InputControlManager까지 전파되도록 함
        // 상점에서는 특별한 처리 없이 설정창이 열리도록 허용
    }

    private IEnumerator SelectEffect(float maxTime)
    {
        Vector2 initSize = selectCursor.sizeDelta;
        float nowTime = 0f;
        while (nowTime <= maxTime)
        {
            selectCursor.sizeDelta = Vector2.Lerp(initSize * Constant.cursorEffectSize, initSize, nowTime / maxTime);
            nowTime += Time.deltaTime;
            yield return null;
        }
        selectCursor.sizeDelta = initSize;
    }

    private IEnumerator FadeOut() // 상점 종료 연출
    {
        curtain.enabled = true;
        float nowTime = 0f, fadeTime = 0.4f;
        while (nowTime <= fadeTime)
        {
            curtain.GetComponent<CanvasGroup>().alpha = nowTime / fadeTime;
            nowTime += Time.deltaTime;
            yield return null;
        }
        curtain.GetComponent<CanvasGroup>().alpha = 1f;

        yield return new WaitForSeconds(0.1f);

        Managers.Stage.GoNextStage();
        Managers.Stage.OnField = true;
        
        // 🎯 Stage_Enter 이벤트 로깅
        LogStageEnterEvent();
        
        Managers.SceneFlow.GotoScene("Field");
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
}
