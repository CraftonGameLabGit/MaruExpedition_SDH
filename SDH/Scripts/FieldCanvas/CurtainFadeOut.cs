using System.Collections;
using UnityEngine;

public class CurtainFadeOut : MonoBehaviour
{
    [SerializeField] private CanvasGroup curtainCanvasGroup;

    public IEnumerator FadeOut(float maxTime) // �ʵ尡 ������ �� ��ȯ �� ��ο����� ����
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
