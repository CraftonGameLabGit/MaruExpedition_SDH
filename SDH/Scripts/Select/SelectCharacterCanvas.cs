using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectCharacterCanvas : MonoBehaviour
{
    [SerializeField] private Transform SelectCharacterPanel;
    [SerializeField] private SelectControl selectControl;
    [SerializeField] private RectTransform selectCursor;
    [SerializeField] private Sprite randomIcon; // 무작위
    [SerializeField] private RectTransform returnButton; // 돌아가기
    [SerializeField] private SelectInfoCanvas selectInfoCanvas;
    public int NowSelectedIdx => nowSelectedIdx - 1; // 0번은 무작위버튼이라 -1 해줘야함
    private int nowSelectedIdx = -1; // 지금 선택한 캐릭터
    private int length; // 그리드 레이아웃 그룹 길이
    private IEnumerator selectEffect;

    private void Awake()
    {
        length = SelectCharacterPanel.GetComponent<GridLayoutGroup>().constraintCount;
    }

    private void Start() // 시작하면 아이콘들 생성 후 숨기기
    {
        for(int i = 0; i < Managers.Asset.CharacterIcons.Length; i++)
        {
            GameObject characterOption = Instantiate(Managers.Asset.OptionTemplate, SelectCharacterPanel);
            characterOption.GetComponent<OptionTemplate>().SetCharacterIcon(i);
        }

        // 기본 선택은 0번
        nowSelectedIdx = 0;
        selectInfoCanvas.SetCharacterThumbnail(nowSelectedIdx - 1); // 1을 빼야하는 것을 기억해

        GetComponent<Canvas>().enabled = false;
        SelectCharacterPanel.gameObject.SetActive(false); // 캐릭터들은 게임오브젝트라서 이것도 꺼줘야함
    }

    public void StartCharacterCanvasIE()
    {
        StartCoroutine(StartCharacterCanvas());
    }

    private IEnumerator StartCharacterCanvas()
    {
        SetNowSelectIdx(nowSelectedIdx == -1 ? 0 : nowSelectedIdx);

        GetComponent<Canvas>().enabled = true;
        SelectCharacterPanel.gameObject.SetActive(true);
        selectInfoCanvas.OnCharacterInfo();

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
        for (int i = 0; i < SelectCharacterPanel.childCount; i++)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(SelectCharacterPanel.GetChild(i).GetComponent<RectTransform>(), Managers.InputControl.InputMouseMove, GetComponent<Canvas>().worldCamera))
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
        if (newSelectedIdx > SelectCharacterPanel.childCount - 1) return; // 인덱스 밖
        if (nowSelectedIdx == newSelectedIdx) return;
        if (nowSelectedIdx == -1 && newSelectedIdx < 0) return;

        // 네비게이션 사운드 재생
        SoundManager.Instance.PlaySFX("ui_navigate");

        if (newSelectedIdx < 0) // 돌아가기
        {
            nowSelectedIdx = -1;

            selectCursor.position = returnButton.position;
            selectCursor.sizeDelta = returnButton.sizeDelta + Constant.cursorAddSize;

            selectInfoCanvas.SetCharacterThumbnail(-2); // -1에서 1 빼서 -2
        }
        else
        {
            nowSelectedIdx = newSelectedIdx;

            RectTransform rect = SelectCharacterPanel.GetChild(nowSelectedIdx).GetComponent<RectTransform>();
            selectCursor.position = rect.position;
            selectCursor.sizeDelta = rect.sizeDelta + Constant.cursorAddSize;

            selectInfoCanvas.SetCharacterThumbnail(nowSelectedIdx - 1); // 1을 빼야하는 것을 기억해
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
            SelectCharacterPanel.gameObject.SetActive(false);

            selectInfoCanvas.OffCharacterInfo();
            selectControl.StartSelectVehicle();
        }
        else if (nowSelectedIdx == 0) // 무작위 버튼
        {
            SoundManager.Instance.PlaySFX("ui_confirm");
            
            Managers.InputControl.ResetUIAction();
            GetComponent<Canvas>().enabled = false;
            SelectCharacterPanel.gameObject.SetActive(false);

            List<int> unlockedIndices = new();

            for (int i = 1; i < SelectCharacterPanel.childCount; i++)
            {
                var iconInfo = SelectCharacterPanel.GetChild(i).GetComponentInChildren<IconInfo>();
                Debug.Log(iconInfo.unlockType.ToString());
                if (Managers.Unlock.IsMissionCleared(iconInfo.unlockType))
                {
                    unlockedIndices.Add(i);
                }
            }

            if (unlockedIndices.Count > 0)
            {
                int randomIdx = unlockedIndices[Random.Range(0, unlockedIndices.Count)];
                SetNowSelectIdx(randomIdx);
                selectInfoCanvas.EndCharacterInfo();
                selectControl.StartSelectDifficulty();
            }
            else
            {
                Debug.LogWarning("해금된 캐릭터가 없습니다!");
            }

            //SetNowSelectIdx(Random.Range(1, SelectCharacterPanel.childCount));
            //selectInfoCanvas.EndCharacterInfo();
            //selectControl.StartSelectDifficulty();
        }
        else
        {
            if (!Managers.Unlock.IsMissionCleared(SelectCharacterPanel.GetChild(nowSelectedIdx).GetComponentInChildren<IconInfo>().unlockType)) return;
            
            SoundManager.Instance.PlaySFX("ui_confirm");
            
            Managers.InputControl.ResetUIAction();
            GetComponent<Canvas>().enabled = false;
            SelectCharacterPanel.gameObject.SetActive(false);

            selectInfoCanvas.EndCharacterInfo();
            selectControl.StartSelectDifficulty();
        }
    }

    private void Quit()
    {
        SoundManager.Instance.PlaySFX("ui_back");

        StopAllCoroutines();
        Managers.InputControl.ResetUIAction();
        GetComponent<Canvas>().enabled = false;
        SelectCharacterPanel.gameObject.SetActive(false);

        selectInfoCanvas.OffCharacterInfo();
        selectControl.StartSelectVehicle();
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
