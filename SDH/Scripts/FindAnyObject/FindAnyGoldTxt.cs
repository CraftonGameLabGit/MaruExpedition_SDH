using TMPro;
using UnityEngine;

public class FindAnyGoldTxt : MonoBehaviour
{
    [SerializeField] RectTransform goldImage;

    private void Awake()
    {
        try { Managers.Status.goldTxt = this; }
        catch { }
        SetText();
    }

    public void SetText()
    {
        GetComponent<TextMeshProUGUI>().text = Managers.Status.Gold.ToString();
        Canvas.ForceUpdateCanvases();
        goldImage.anchoredPosition = new(-10f - GetComponent<RectTransform>().rect.width, 0);
    }

    private void OnDestroy()
    {
        Managers.Status.goldTxt = null;
    }
}
