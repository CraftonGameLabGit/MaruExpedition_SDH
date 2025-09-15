using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectVehicleCanvas : MonoBehaviour
{
    [SerializeField] private RectTransform SelectVehiclePanel;
    [SerializeField] private SelectControl selectControl;
    [SerializeField] private RectTransform selectCursor;
    [SerializeField] private Sprite randomIcon; // 무작위
    [SerializeField] private RectTransform returnButton; // 돌아가기
    [SerializeField] private SelectInfoCanvas selectInfoCanvas;
    public int NowSelectedIdx => nowSelectedIdx - 1; // 0번은 무작위버튼이라 -1 해줘야함
    private int nowSelectedIdx; // 지금 선택한 비행체
    private int length; // 그리드 레이아웃 그룹 길이
    private IEnumerator selectEffect;

    private void Awake()
    {
        length = SelectVehiclePanel.GetComponent<GridLayoutGroup>().constraintCount;
    }

    private void Start() // 시작하면 아이콘들 생성 후 숨기기
    {
        for (int i = 0; i < Managers.Asset.VehicleIcons.Length; i++) // 0번 선택지는 무작위 선택지
        {
            GameObject vehicleOption = Instantiate(Managers.Asset.OptionTemplate, SelectVehiclePanel);
            vehicleOption.GetComponent<OptionTemplate>().SetVehicleIcon(i);
        }

        // 기본 선택은 0번
        nowSelectedIdx = 0;
        selectInfoCanvas.SetVehicleThumbnail(nowSelectedIdx - 1); // 1을 빼야하는 것을 기억해

        GetComponent<Canvas>().enabled = false;
    }

    public void StartVehicleCanvasIE()
    {
        StartCoroutine(StartVehicleCanvas());
    }

    private IEnumerator StartVehicleCanvas()
    {
        GetComponent<Canvas>().enabled = true;
        selectInfoCanvas.OnVehicleInfo();

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
        for (int i = 0; i < SelectVehiclePanel.childCount; i++)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(SelectVehiclePanel.GetChild(i).GetComponent<RectTransform>(), Managers.InputControl.InputMouseMove, GetComponent<Canvas>().worldCamera))
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
        if (newSelectedIdx > SelectVehiclePanel.childCount - 1) return; // 인덱스 밖
        if (nowSelectedIdx == newSelectedIdx) return;
        if (nowSelectedIdx == -1 && newSelectedIdx < 0) return;

        // 네비게이션 사운드 재생
        SoundManager.Instance.PlaySFX("ui_navigate");

        if (newSelectedIdx < 0) // 돌아가기
        {
            nowSelectedIdx = -1;

            selectCursor.position = returnButton.position;
            selectCursor.sizeDelta = returnButton.sizeDelta + Constant.cursorAddSize;

            selectInfoCanvas.SetVehicleThumbnail(-2); // -1에서 1 빼서 -2
        }
        else
        {
            nowSelectedIdx = newSelectedIdx;

            RectTransform rect = SelectVehiclePanel.GetChild(nowSelectedIdx).GetComponent<RectTransform>();
            selectCursor.position = rect.position;
            selectCursor.sizeDelta = rect.sizeDelta + Constant.cursorAddSize;

            selectInfoCanvas.SetVehicleThumbnail(nowSelectedIdx - 1); // 1을 빼야하는 것을 기억해
        }

        if (selectEffect != null) StopCoroutine(selectEffect);
        StartCoroutine(selectEffect = SelectEffect(Constant.cursorEffectTime));
    }

    private void Select()
    {
        if (nowSelectedIdx == -1) // 돌아가기 버튼인데 첫 선택지라 전 씬으로 돌아감
        {
            SoundManager.Instance.PlaySFX("ui_back");
            
            Managers.InputControl.ResetUIAction();
            GetComponent<Canvas>().enabled = false;

            Managers.SceneFlow.GotoScene("Main");
        }
        else if(nowSelectedIdx == 0) // 무작위 버튼
        {
            SoundManager.Instance.PlaySFX("ui_confirm");
            
            Managers.InputControl.ResetUIAction();
            GetComponent<Canvas>().enabled = false;

            List<int> unlockedIndices = new();

            for (int i = 1; i < SelectVehiclePanel.childCount; i++)
            {
                var iconInfo = SelectVehiclePanel.GetChild(i).GetComponentInChildren<IconInfo>();
                if (Managers.Unlock.IsMissionCleared(iconInfo.unlockType))
                {
                    unlockedIndices.Add(i);
                }
            }

            if (unlockedIndices.Count > 0)
            {
                int randomIdx = unlockedIndices[Random.Range(0, unlockedIndices.Count)];
                SetNowSelectIdx(randomIdx);
                selectInfoCanvas.EndVehicleInfo();
                selectControl.StartSelectCharacter();
            }
            else
            {
                Debug.LogWarning("해금된 차량이 없습니다!");
            }

            //SetNowSelectIdx(Random.Range(1, SelectVehiclePanel.childCount));
            //selectInfoCanvas.EndVehicleInfo();
            //selectControl.StartSelectCharacter();
        }
        else
        {
            if (!Managers.Unlock.IsMissionCleared(SelectVehiclePanel.GetChild(nowSelectedIdx).GetComponentInChildren<IconInfo>().unlockType)) return;

            SoundManager.Instance.PlaySFX("ui_confirm");

            Managers.InputControl.ResetUIAction();
            GetComponent<Canvas>().enabled = false;

            selectInfoCanvas.EndVehicleInfo();
            selectControl.StartSelectCharacter();
        }
    }
    
    private void Quit()
    {
        SoundManager.Instance.PlaySFX("ui_back");
        
        Managers.InputControl.ResetUIAction();
        GetComponent<Canvas>().enabled = false;

        Managers.SceneFlow.GotoScene("Main");
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
