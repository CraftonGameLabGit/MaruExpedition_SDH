using System.Collections;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class MainSceneCanvas : MonoBehaviour // 메인 메뉴 캔버스
{
    [SerializeField] private RectTransform selectCursor;
    [SerializeField] private List<RectTransform> optionsTxt;

    // 설명/설정 Canvas 연결
    [SerializeField] private GameObject howToPlayCanvas;
    [SerializeField] private GameObject gameSettingCanvas;
    [SerializeField] private Canvas curtain;
    private int nowSelectIdx;
    private IEnumerator selectEffect;

    private void Start()
    {
        Managers.SteamAchievement.Achieve("ACHIEVEMENT_Welcome");
        // 메인 메뉴에서는 Time.timeScale을 항상 1로 설정
        Time.timeScale = 1f;

        if (Managers.Data.LocalPlayerData.gameData == null)
        {
            optionsTxt[0].GetComponent<TextMeshProUGUI>().color = new(1f, 1f, 1f, 0.2f); // 저장된 게임이 없다면 재개 버튼 비활성
        }

        Canvas.ForceUpdateCanvases();
        // 초기 선택은 시작버튼
        nowSelectIdx = 1;
        optionsTxt[nowSelectIdx].GetComponent<TextMeshProUGUI>().color = new(0f, 0.5f, 1f);
        selectCursor.localPosition = optionsTxt[nowSelectIdx].localPosition + new Vector3(-650f, -250f, 0f); // 옵션패널 로컬포지션 하드코딩 @@@@@@@@@@

        StartTmpCanvas();
    }

    public void StartTmpCanvas()
    {
        // 기존 이벤트 먼저 해제 (중복 구독 방지)
        Managers.InputControl.ResetUIAction();

        // 이벤트 다시 구독
        Managers.InputControl.CoroutineRunner = this;
        Managers.InputControl.UseAction += Select;
        Managers.InputControl.SubScribeUIMove(null, null, GetDown, GetUp);
        Managers.InputControl.MouseMoveAction += GetMouseInput;
        Managers.InputControl.MouseUseAction += Select;

        if (Cursor.visible) GetMouseInput();
    }
    
    public void StartTmpCanvasDelayed()
    {
        StartCoroutine(DelayedInputReactivation());
    }
    
    private IEnumerator DelayedInputReactivation()
    {
        // 한 프레임 대기하여 이벤트 구독 해제가 완전히 처리되도록 함
        yield return null;
        
        StartTmpCanvas();
    }

    private void GetDown() { if (!IsAnyModalActive()) SetNowSelectIdx(nowSelectIdx + 1); }
    private void GetUp() { if (!IsAnyModalActive()) SetNowSelectIdx(nowSelectIdx - 1); }

    private void GetMouseInput()
    {
        // 모달 상태 체크
        if (IsAnyModalActive())
        {
            Managers.InputControl.PossInputMouseUse = false;
            return;
        }

        int iStart = Managers.Data.LocalPlayerData.gameData == null ? 1 : 0;
        for (int i = iStart; i < optionsTxt.Count; i++)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(optionsTxt[i].GetComponent<RectTransform>(), Managers.InputControl.InputMouseMove))
            {
                Managers.InputControl.PossInputMouseUse = true;
                SetNowSelectIdx(i);
                return;
            }
        }

        Managers.InputControl.PossInputMouseUse = false;
    }

    private void SetNowSelectIdx(int newIdx)
    {
        if (newIdx < 0 || newIdx > optionsTxt.Count - 1) 
        {
            return;
        }
        if (newIdx == 0 && Managers.Data.LocalPlayerData.gameData == null) return;

        if (nowSelectIdx == newIdx) 
        {
            return;
        }

        // 네비게이션 사운드 재생
        SoundManager.Instance.PlaySFX("ui_navigate");
        
        optionsTxt[nowSelectIdx].GetComponent<TextMeshProUGUI>().color = Color.white;
        
        nowSelectIdx = newIdx;
        optionsTxt[nowSelectIdx].GetComponent<TextMeshProUGUI>().color = new(0f, 0.5f, 1f);

        selectCursor.localPosition = optionsTxt[nowSelectIdx].localPosition + new Vector3(-650f, -250f, 0f); // 옵션패널 로컬포지션 하드코딩 @@@@@@@@@@

        if (selectEffect != null) StopCoroutine(selectEffect);
        StartCoroutine(selectEffect = SelectEffect(0.3f));
    }

    private void Select()
    {
        // 모달 상태 체크 추가
        if (IsAnyModalActive())
        {
            return;
        }
        
        Managers.InputControl.ResetUIAction();

        switch (nowSelectIdx)
        {
            case 0: // 이어하기
                if (Managers.Data.LocalPlayerData.gameData == null) return;
                SoundManager.Instance.PlaySFX("ui_confirm");
                Managers.InputControl.ResetUIAction();
                StartCoroutine(ContinueGameIE());
                break;

            case 1: // 시작
                SoundManager.Instance.PlaySFX("ui_confirm");
                Managers.InputControl.ResetUIAction();
                Managers.SceneFlow.GotoScene("Select");
                break;

            case 2: // 설명
                SoundManager.Instance.PlaySFX("ui_confirm");
                if (howToPlayCanvas != null)
                {
                    howToPlayCanvas.SetActive(true);
                }
                break;

            case 3: // 설정
                SoundManager.Instance.PlaySFX("ui_confirm");
                if (gameSettingCanvas != null)
                {
                    GameSettingController settingController = gameSettingCanvas.GetComponent<GameSettingController>();
                    if (settingController != null)
                    {
                        settingController.OpenSettingsPanel();
                    }
                }
                break;

            case 4: // 종료
                SoundManager.Instance.PlaySFX("ui_confirm");
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                break;

            default:
                Debug.Log("이 로그가 뜨면 잘못된건데?");
                break;
        }
    }
    
    // 모달 상태 확인 메서드
    private bool IsAnyModalActive()
    {
        bool modalActive = (howToPlayCanvas != null && howToPlayCanvas.activeSelf) || 
                          GameSettingController.IsSettingsActive;
        
        return modalActive;
    }



    private IEnumerator SelectEffect(float maxTime)
    {
        float nowTime = 0f;
        while (true)
        {
            selectCursor.GetChild(0).localPosition = Vector2.Lerp(new(-200f, 0f), new(-150f, 0f), nowTime / maxTime);
            selectCursor.GetChild(1).localPosition = Vector2.Lerp(new(200f, 0f), new(150f, 0f), nowTime / maxTime);
            nowTime += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator ContinueGameIE()
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

        GetComponent<GameStartControl>().ContinueGame();
    }
}
