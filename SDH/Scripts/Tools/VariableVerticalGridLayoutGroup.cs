using UnityEngine;

public class VariableVerticalGridLayoutGroup : MonoBehaviour // ������ �Ʒ��� �������� ���� �׸��� ���̾ƿ�
{
    [SerializeField] private float leftPadding;
    [SerializeField] private float topPadding;
    [SerializeField] private float spacing;

    private void Start()
    {
        LayoutCells();
    }

    public void LayoutCells()
    {
        float yPos = -topPadding;

        for(int i = 0; i < transform.childCount; i++)
        {
            RectTransform rectTransform = transform.GetChild(i).GetComponent<RectTransform>();

            Vector2 size = rectTransform.sizeDelta;

            rectTransform.anchorMin = new(0, 1);
            rectTransform.anchorMax = new(0, 1);
            rectTransform.pivot = new(0, 1);
            rectTransform.anchoredPosition = new(leftPadding, yPos); // ��� ���Ǹ� ���� ������ ���� �������� ��� anchoredPosition�� ����

            yPos -= rectTransform.sizeDelta.y + spacing;
        }
    }
}
