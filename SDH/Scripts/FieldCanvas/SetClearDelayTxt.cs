using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class SetClearDelayTxt : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI delayTxt;
    [SerializeField] TextMeshProUGUI delayTimeTxt;

    public void StartDelayTxt()
    {
        delayTimeTxt.gameObject.SetActive(true);
        StartCoroutine(SetDelayTxt());
    }

    private IEnumerator SetDelayTxt() // ���������� ������ 3�� �� �̵�
    {
        var delayLocalized = new LocalizedString("New Table", "UI_Stage_ClearDelay");
        delayLocalized.StringChanged += (localized) => { delayTxt.text = localized; };

        float nowTime = 0f, maxTime = 3f;

        while (nowTime <= maxTime)
        {
            if (GameObject.FindGameObjectsWithTag("Item").Length == 0) break; // ������ ���ٸ� �ٷ� ����
            delayTimeTxt.text = Mathf.Ceil(maxTime - nowTime).ToString();
            nowTime += Time.deltaTime;
            yield return null;
        }

        delayTxt.text = "";
        delayTimeTxt.text = "";

        Managers.Stage.OnField = false;
    }
}
