using UnityEngine;

public class VariableVerticalGridLayoutGroup : MonoBehaviour // 위에서 아래로 내려오는 가변 그리드 레이아웃
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
            rectTransform.anchoredPosition = new(leftPadding, yPos); // 계산 편의를 위해 오른쪽 위를 기준으로 잡고 anchoredPosition을 변경

            yPos -= rectTransform.sizeDelta.y + spacing;
        }
    }
}
