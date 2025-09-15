using UnityEngine;
using UnityEngine.UI;

public class OldSelectCanavs : MonoBehaviour // 선택창 캔버스 구버전이지만 아까워서 남김 (특히 저 스크롤 넘어가며 자동으로 비율 조정하는 부분)
{
    private ScrollRect scrollRect;
    private int nowSelectedIdx; // 지금 선택한 시작옵션
    private float step; // 아이템 하나가 차지하는 스크롤 거리 비율
    public bool IsOn
    {
        get
        {
            return isOn;
        }
        set
        {
            isOn = value;
        }
    }
    private bool isOn; // 현재 옵션을 설정 중인지 확인 (false면 동작 안함)

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();

        nowSelectedIdx = 0;
        scrollRect.content.GetChild(nowSelectedIdx).GetComponent<Image>().color = Color.green; // 0번 항목 선택
    }

    private void Start()
    {
        isOn = true;

        scrollRect.horizontalNormalizedPosition = 0f; // 스크롤을 0f로 초기화하지 않거나 계산을 Awake에 넣으면 scrollRect.content.rect.width가 설정되지 않아 터짐
        step = (scrollRect.content.GetChild(0).GetComponent<RectTransform>().rect.width + scrollRect.content.GetComponent<HorizontalLayoutGroup>().spacing) / (scrollRect.content.rect.width - scrollRect.viewport.rect.width);

        StartGetInput();
    }

    public void StartGetInput()
    {
        Managers.InputControl.UseAction += SelectNowSelected;
        //Managers.InputControl.MoveAction += GetInput;
    }

    private void GetInput()
    {
        //if (Managers.InputControl.InputMove.x == -1) MinusNowSelectedIdx();
        //else if (Managers.InputControl.InputMove.x == 1) PlusNowSelectedIdx();
    }

    private void SelectNowSelected() // 현재 선택한 옵션을 실행
    {
        Managers.InputControl.ResetUIAction();
        //scrollRect.content.GetChild(nowSelectedIdx).GetComponent<SelectOption>().ChooseOption();
    }

    private void MinusNowSelectedIdx() // 왼쪽 옵션으로 넘어가고 색을 변경
    {
        if (nowSelectedIdx <= 0) return; // 인덱스 밖

        scrollRect.content.GetChild(nowSelectedIdx).GetComponent<Image>().color = Color.white; // 원래 선택한 옵션 강조 해제
        nowSelectedIdx--;
        scrollRect.content.GetChild(nowSelectedIdx).GetComponent<Image>().color = Color.green; // 새로 선택한 옵션 강조 설정

        scrollRect.horizontalNormalizedPosition = Mathf.Min(scrollRect.horizontalNormalizedPosition, step * nowSelectedIdx); // 스크롤을 왼쪽으로 이동해야 한다면 이동
    }

    private void PlusNowSelectedIdx() // 오른쪽 옵션으로 넘어가고 색을 변경. MinusNowSelectedIdx와 동일한 주석은 생략
    {
        if (nowSelectedIdx >= scrollRect.content.childCount - 1) return;

        scrollRect.content.GetChild(nowSelectedIdx).GetComponent<Image>().color = Color.white; //
        nowSelectedIdx++;
        scrollRect.content.GetChild(nowSelectedIdx).GetComponent<Image>().color = Color.green; //

        scrollRect.horizontalNormalizedPosition = Mathf.Max(scrollRect.horizontalNormalizedPosition, 1f - step * (scrollRect.content.childCount - 1 - nowSelectedIdx));
    }
}
