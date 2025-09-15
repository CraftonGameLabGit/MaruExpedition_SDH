using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class ActStartDirect : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Transform roomPanel;
    [SerializeField] TextMeshProUGUI stageTxt;

    public void StartFadeOut(float waitTime, float maxTime)
    {
        StartCoroutine(Directing(waitTime, maxTime));
    }

    private IEnumerator Directing(float waitTime, float maxTime) // �ʵ� ���� ����
    {
        var fieldProgressLocalized = new LocalizedString("New Table", "UI_Stage_Progress");
        fieldProgressLocalized.Arguments = new object[] { Managers.Stage.World };
        fieldProgressLocalized.StringChanged += (localized) => { stageTxt.text = localized; };

        for (int i = 1; i <= Managers.Asset.StageCounts[Managers.Stage.NowStage.world]; i++) // ������� �������� ����
        {
            if (i < Managers.Stage.NowStage.stage)
            {
                Instantiate(Managers.Asset.RoomIcons[0], roomPanel);
            }
            else if(i == Managers.Asset.StageCounts[Managers.Stage.NowStage.world])
            {
                Instantiate(Managers.Asset.RoomIcons[2], roomPanel);
            }
            else
            {
                Instantiate(Managers.Asset.RoomIcons[1], roomPanel);
            }
        }

        GameObject vehicleIcon = Instantiate(Managers.Asset.OptionTemplate, transform);
        vehicleIcon.GetComponent<OptionTemplate>().SetVehicleIcon(Managers.PlayerControl.VehicleIdx, true, 0.4f);

        Canvas.ForceUpdateCanvases();

        float nowTime = 0f;
        Vector2 startPos = Managers.Stage.NowStage.stage == 1 ? roomPanel.GetChild(0).GetComponent<RectTransform>().position + Vector3.left * 3.333f : roomPanel.GetChild(Managers.Stage.NowStage.stage - 2).GetComponent<RectTransform>().position; // ù �������������� �� �������� ����
        Vector2 endPos = roomPanel.GetChild(Managers.Stage.NowStage.stage - 1).GetComponent<RectTransform>().position; // �ε����� 0���� �����ϹǷ� �������� ������ 1�� ����� ��
        Debug.Log("RRRRR " + startPos + " " + endPos);

        RectTransform vehicleRect = vehicleIcon.GetComponent<RectTransform>();

        while (nowTime <= waitTime)
        {
            vehicleRect.position = Vector2.Lerp(startPos, endPos, nowTime / waitTime);

            nowTime += Mathf.Min(Time.deltaTime, 0.05f);
            yield return null;
        }
        vehicleRect.position = endPos;

        nowTime = 0f; // ������� ���̵� �ƿ� ����

        while (nowTime <= maxTime)
        {
            canvasGroup.alpha = (maxTime - nowTime) / maxTime;
            nowTime += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
        yield break;
    }
}
