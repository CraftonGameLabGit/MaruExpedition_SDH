using System.Collections;
using UnityEngine;

public class CurtainFadeOut : MonoBehaviour
{
    [SerializeField] private CanvasGroup curtainCanvasGroup;

    public IEnumerator FadeOut(float maxTime) // 필드가 끝나고 씬 전환 시 어두워지는 연출
    {
        float nowTime = 0f;

        while (nowTime <= maxTime)
        {
            curtainCanvasGroup.alpha = nowTime / maxTime;
            nowTime += Time.deltaTime;
            yield return null;
        }

        curtainCanvasGroup.alpha = 1f;
        yield break;
    }
}
