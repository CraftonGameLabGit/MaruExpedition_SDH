using UnityEngine;
using UnityEngine.UI;

public class OldSelectCanavs : MonoBehaviour // ����â ĵ���� ������������ �Ʊ���� ���� (Ư�� �� ��ũ�� �Ѿ�� �ڵ����� ���� �����ϴ� �κ�)
{
    private ScrollRect scrollRect;
    private int nowSelectedIdx; // ���� ������ ���ۿɼ�
    private float step; // ������ �ϳ��� �����ϴ� ��ũ�� �Ÿ� ����
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
    private bool isOn; // ���� �ɼ��� ���� ������ Ȯ�� (false�� ���� ����)

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();

        nowSelectedIdx = 0;
        scrollRect.content.GetChild(nowSelectedIdx).GetComponent<Image>().color = Color.green; // 0�� �׸� ����
    }

    private void Start()
    {
        isOn = true;

        scrollRect.horizontalNormalizedPosition = 0f; // ��ũ���� 0f�� �ʱ�ȭ���� �ʰų� ����� Awake�� ������ scrollRect.content.rect.width�� �������� �ʾ� ����
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

    private void SelectNowSelected() // ���� ������ �ɼ��� ����
    {
        Managers.InputControl.ResetUIAction();
        //scrollRect.content.GetChild(nowSelectedIdx).GetComponent<SelectOption>().ChooseOption();
    }

    private void MinusNowSelectedIdx() // ���� �ɼ����� �Ѿ�� ���� ����
    {
        if (nowSelectedIdx <= 0) return; // �ε��� ��

        scrollRect.content.GetChild(nowSelectedIdx).GetComponent<Image>().color = Color.white; // ���� ������ �ɼ� ���� ����
        nowSelectedIdx--;
        scrollRect.content.GetChild(nowSelectedIdx).GetComponent<Image>().color = Color.green; // ���� ������ �ɼ� ���� ����

        scrollRect.horizontalNormalizedPosition = Mathf.Min(scrollRect.horizontalNormalizedPosition, step * nowSelectedIdx); // ��ũ���� �������� �̵��ؾ� �Ѵٸ� �̵�
    }

    private void PlusNowSelectedIdx() // ������ �ɼ����� �Ѿ�� ���� ����. MinusNowSelectedIdx�� ������ �ּ��� ����
    {
        if (nowSelectedIdx >= scrollRect.content.childCount - 1) return;

        scrollRect.content.GetChild(nowSelectedIdx).GetComponent<Image>().color = Color.white; //
        nowSelectedIdx++;
        scrollRect.content.GetChild(nowSelectedIdx).GetComponent<Image>().color = Color.green; //

        scrollRect.horizontalNormalizedPosition = Mathf.Max(scrollRect.horizontalNormalizedPosition, 1f - step * (scrollRect.content.childCount - 1 - nowSelectedIdx));
    }
}
