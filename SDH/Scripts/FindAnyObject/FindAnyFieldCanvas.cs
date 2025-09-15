using UnityEngine;

public class FindAnyFieldCanvas : MonoBehaviour
{

    [SerializeField] private CurtainFadeOut panelCanvasGroup;
    [SerializeField] private SetClearTxt clearTxt;
    [SerializeField] private SetClearDelayTxt clearDelayTxt;
    [SerializeField] private ActStartDirect startDirect;
    [SerializeField] private ActGameOverDirect gameOverDirect;
    [SerializeField] private ActGameClearDirect gameClearDirect;

    private void Awake()
    {
        try { Managers.SceneFlow.FieldCanvas = this; }
        catch { }
    }

    public void StartStartDirect(float waitTime, float maxTime)
    {
        startDirect.gameObject.SetActive(true);
        startDirect.StartFadeOut(waitTime, maxTime);
    }

    public void StartFadeOut(float maxTime)
    {
        panelCanvasGroup.gameObject.SetActive(true);
        StartCoroutine(panelCanvasGroup.FadeOut(maxTime));
    }

    public void StartClearTxt() // 클리어 텍스트 설정 및 동전을 위한 유예 시간
    {
        clearTxt.gameObject.SetActive(true);
        clearDelayTxt.gameObject.SetActive(true);
        clearDelayTxt.StartDelayTxt();
    }

    public void StartGameOverDirect(float maxTime)
    {
        gameOverDirect.gameObject.SetActive(true);
        gameOverDirect.StartFadeOut(maxTime);
    }

    public void StartGameClearDirect(float maxTime)
    {
        gameClearDirect.gameObject.SetActive(true);
        gameClearDirect.StartFadeOut(maxTime);
    }

    public void OnDestroy()
    {
        Managers.SceneFlow.FieldCanvas = null;
    }
}
