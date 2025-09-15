using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class SetToolTip : MonoBehaviour // ù �ʵ� ���� �� �����ִ� �뵵
{
    [SerializeField] TextMeshProUGUI tipTxt;

    public void StartToolTip()
    {
        StartCoroutine(ToolTip());
    }

    private IEnumerator ToolTip()
    {
        if (Managers.InputControl.IsGamePad())
        {
            var tipLocalized = new LocalizedString("New Table", "UI_Stage_GamePadTips");
            tipLocalized.StringChanged += (localized) => { tipTxt.text = localized; };
        }
        else
        {
            var tipLocalized = new LocalizedString("New Table", "UI_Stage_KeyboardTips");
            tipLocalized.StringChanged += (localized) => { tipTxt.text = localized; };
        }

        yield return new WaitForSeconds(2.1f); // �ʵ� ���� ���� �ð��� 2.1��

        float nowTime = 0f, maxTime = 2f;
        while (nowTime <= maxTime)
        {
            tipTxt.color = new(1f, 1f, 1f, Mathf.Cos(nowTime / maxTime * Mathf.PI / 2));
            nowTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
