using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SelectDifficultyCanvas : MonoBehaviour
{
    [SerializeField] private Transform SelectDifficultyPanel;
    [SerializeField] private SelectControl selectControl;
    [SerializeField] private RectTransform selectCursor;
    [SerializeField] private Sprite startIcon; // 시작하기
    public Sprite[] NumbersIcon => numbersIcon;
    [SerializeField] private Sprite[] numbersIcon; // 난이도 스프라이트
    [SerializeField] private RectTransform returnButton; // 돌아가기
    [SerializeField] private SelectInfoCanvas selectInfoCanvas;
    public int NowSelectedIdx => nowSelectedIdx; // 얘는 -1 안해줘도 됨
    private int nowSelectedIdx = -1; // 지금 선택한 난이도
    private int length; // 그리드 레이아웃 그룹 길이
    private IEnumerator selectEffect;

    private void Awake()
    {
        length = SelectDifficultyPanel.GetComponent<GridLayoutGroup>().constraintCount;
    }

    private void Start() // 시작하면 아이콘들 생성 후 숨기기
    {
        GameObject difficultyOption;

        difficultyOption = Instantiate(Managers.Asset.OptionTemplate, SelectDifficultyPanel); // 난이도 0
        difficultyOption.GetComponent<OptionTemplate>().SetDiffficulty(numbersIcon[0], Constant.difficultyIconSize, true);

        difficultyOption = Instantiate(Managers.Asset.OptionTemplate, SelectDifficultyPanel); // 난이도 1
        difficultyOption.GetComponent<OptionTemplate>().SetDiffficulty(numbersIcon[1], Constant.difficultyIconSize, Managers.Unlock.IsMissionCleared(EUnlockType.ZeroClearCount));
        if (Managers.Unlock.IsMissionCleared(EUnlockType.ZeroClearCount)) difficultyOption.GetComponent<Image>().color = new Color32(6, 214, 160, 255);

        difficultyOption = Instantiate(Managers.Asset.OptionTemplate, SelectDifficultyPanel); // 난이도 2
        difficultyOption.GetComponent<OptionTemplate>().SetDiffficulty(numbersIcon[2], Constant.difficultyIconSize, Managers.Unlock.IsMissionCleared(EUnlockType.OneClearCount));
        if (Managers.Unlock.IsMissionCleared(EUnlockType.OneClearCount)) difficultyOption.GetComponent<Image>().color = new Color32(255, 209, 102, 255);

        difficultyOption = Instantiate(Managers.Asset.OptionTemplate, SelectDifficultyPanel); // 난이도 3
        difficultyOption.GetComponent<OptionTemplate>().SetDiffficulty(numbersIcon[3], Constant.difficultyIconSize, Managers.Unlock.IsMissionCleared(EUnlockType.TwoClearCount));
        if (Managers.Unlock.IsMissionCleared(EUnlockType.TwoClearCount)) difficultyOption.GetComponent<Image>().color = new Color32(239, 71, 111, 255);


        // 기본 선택은 0번
        nowSelectedIdx = 0;
        selectInfoCanvas.SetDifficultyThumbnail(nowSelectedIdx, numbersIcon[nowSelectedIdx]); // 얘는 1 안빼도 됨

        GetComponent<Canvas>().enabled = false;
    }

    public void StartDifficultyCanvasIE()
    {
        StartCoroutine(StartDifficultyCanvas());
    }

    public IEnumerator StartDifficultyCanvas()
    {
        SetNowSelectIdx(nowSelectedIdx == -1 ? 0 : nowSelectedIdx);

        GetComponent<Canvas>().enabled = true;
        selectInfoCanvas.OnDifficultyInfo();

        yield return null;

        Managers.InputControl.CoroutineRunner = this;
        Managers.InputControl.UseAction += Select;
        Managers.InputControl.SubScribeUIMove(GetLeft, GetRight, GetDown, GetUp);
        Managers.InputControl.QuitAction += Quit;
        Managers.InputControl.MouseMoveAction += GetMouseInput;
        Managers.InputControl.MouseUseAction += Select;
        if (Cursor.visible) GetMouseInput();
    }

    private void GetLeft() { SetNowSelectIdx(nowSelectedIdx - 1); }
    private void GetRight() { SetNowSelectIdx(nowSelectedIdx == -1 ? 1 : nowSelectedIdx + 1); }
    private void GetDown() { SetNowSelectIdx(nowSelectedIdx == -1 ? 0 : nowSelectedIdx + length); }
    private void GetUp() { SetNowSelectIdx(nowSelectedIdx - length); }

    private void GetMouseInput()
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(returnButton.GetComponent<RectTransform>(), Managers.InputControl.InputMouseMove, GetComponent<Canvas>().worldCamera))
        {
            Managers.InputControl.PossInputMouseUse = true;
            SetNowSelectIdx(-1);
            return;
        }
        for (int i = 0; i < SelectDifficultyPanel.childCount; i++)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(SelectDifficultyPanel.GetChild(i).GetComponent<RectTransform>(), Managers.InputControl.InputMouseMove, GetComponent<Canvas>().worldCamera))
            {
                Managers.InputControl.PossInputMouseUse = true;
                SetNowSelectIdx(i);
                return;
            }
        }

        Managers.InputControl.PossInputMouseUse = false;
    }

    private void SetNowSelectIdx(int newSelectedIdx) // 다른 옵션으로 넘어가고 커서 위치 변경
    {
        if (newSelectedIdx > SelectDifficultyPanel.childCount - 1) return; // 인덱스 밖
        if (nowSelectedIdx == newSelectedIdx) return;
        if (nowSelectedIdx == -1 && newSelectedIdx < 0) return;

        // 네비게이션 사운드 재생
        SoundManager.Instance.PlaySFX("ui_navigate");

        if (newSelectedIdx < 0)
        {
            nowSelectedIdx = -1;

            selectCursor.position = returnButton.position;
            selectCursor.sizeDelta = returnButton.sizeDelta + Constant.cursorAddSize;

            selectInfoCanvas.SetDifficultyThumbnail(-1, null); // 1 안빼도 되니까
        }
        else
        {
            nowSelectedIdx = newSelectedIdx;

            RectTransform rect = SelectDifficultyPanel.GetChild(nowSelectedIdx).GetComponent<RectTransform>();
            selectCursor.position = rect.position;
            selectCursor.sizeDelta = rect.sizeDelta + Constant.cursorAddSize;

            switch (nowSelectedIdx)
            {
                case 0:
                    selectInfoCanvas.SetDifficultyThumbnail(nowSelectedIdx, numbersIcon[nowSelectedIdx]); // 얘는 1 안빼도 됨
                    break;
                case 1:
                    selectInfoCanvas.SetDifficultyThumbnail(nowSelectedIdx, numbersIcon[nowSelectedIdx], Managers.Unlock.IsMissionCleared(EUnlockType.ZeroClearCount));
                    break;
                case 2:
                    selectInfoCanvas.SetDifficultyThumbnail(nowSelectedIdx, numbersIcon[nowSelectedIdx], Managers.Unlock.IsMissionCleared(EUnlockType.OneClearCount));
                    break;
                case 3:
                    selectInfoCanvas.SetDifficultyThumbnail(nowSelectedIdx, numbersIcon[nowSelectedIdx], Managers.Unlock.IsMissionCleared(EUnlockType.TwoClearCount));
                    break;
            }
        }

        if (selectEffect != null) StopCoroutine(selectEffect);
        StartCoroutine(selectEffect = SelectEffect(Constant.cursorEffectTime));
    }

    private void Select()
    {

        if (nowSelectedIdx == -1) // 돌아가기 버튼
        {
            SoundManager.Instance.PlaySFX("ui_back");
            
            Managers.InputControl.ResetUIAction();
            GetComponent<Canvas>().enabled = false;

            selectInfoCanvas.OffDificultyInfo();
            selectControl.StartSelectCharacter();
        }
        else
        {
            if (nowSelectedIdx == 1 && !Managers.Unlock.IsMissionCleared(EUnlockType.ZeroClearCount))        // 1난이도
            {
                return;
            }
            if (nowSelectedIdx == 2 && !Managers.Unlock.IsMissionCleared(EUnlockType.OneClearCount))        // 2난이도
            {
                return;
            }
            if (nowSelectedIdx == 3 && !Managers.Unlock.IsMissionCleared(EUnlockType.TwoClearCount))        //3난이도
            {
                return;
            }

            SoundManager.Instance.PlaySFX("ui_confirm");
            
            Managers.InputControl.ResetUIAction();
            //GetComponent<Canvas>().enabled = false;

            selectControl.StartGame();
        }
    }

    private void Quit()
    {
        SoundManager.Instance.PlaySFX("ui_back");
        
        Managers.InputControl.ResetUIAction();
        GetComponent<Canvas>().enabled = false;

        selectInfoCanvas.OffDificultyInfo();
        selectControl.StartSelectCharacter();
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
}
