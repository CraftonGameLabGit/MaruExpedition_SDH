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

    private IEnumerator SetDelayTxt() // 스테이지가 끝나고 3초 뒤 이동
    {
        var delayLocalized = new LocalizedString("New Table", "UI_Stage_ClearDelay");
        delayLocalized.StringChanged += (localized) => { delayTxt.text = localized; };

        float nowTime = 0f, maxTime = 3f;

        while (nowTime <= maxTime)
        {
            if (GameObject.FindGameObjectsWithTag("Item").Length == 0) break; // 코인이 없다면 바로 종료
            delayTimeTxt.text = Mathf.Ceil(maxTime - nowTime).ToString();
            nowTime += Time.deltaTime;
            yield return null;
        }

        delayTxt.text = "";
        delayTimeTxt.text = "";

        Managers.Stage.OnField = false;
    }
}
