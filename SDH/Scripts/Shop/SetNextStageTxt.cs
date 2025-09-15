using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class SetNextStageTxt : MonoBehaviour // 다음 스테이지를 알려주는 텍스트
{
    private void Start()
    {
        TextMeshProUGUI nextStageTxt = GetComponent<TextMeshProUGUI>();
        var nextStageLocalized = new LocalizedString("New Table", Managers.Stage.Stage + 1 != Managers.Asset.StageCounts[Managers.Stage.World] ? "UI_Shop_NextStage" : "UI_Shop_NextStageBoss");
        nextStageLocalized.Arguments = Managers.Stage.Stage == Managers.Asset.StageCounts[Managers.Stage.World] ? new object[] { Managers.Stage.World + 1, 1 } : new object[] { Managers.Stage.World, Managers.Stage.Stage + 1 };
        nextStageLocalized.StringChanged += (localized) => { nextStageTxt.text = localized; };
    }
}
