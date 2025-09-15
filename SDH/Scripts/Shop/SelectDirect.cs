using System.Collections;
using UnityEngine;

public class SelectDirect : MonoBehaviour // 선택한 SelectDisplayTemplate의 강조 효과
{
    private void Start()
    {
        StartCoroutine(Direct());
    }

    private IEnumerator Direct()
    {
        float nowTime, maxTime;
        Vector3 endPos;

        nowTime = 0f; maxTime = 0.2f;
        endPos = transform.localPosition + Vector3.down * 50f;

        while (nowTime <= maxTime) // 아래로 살짝 내려갔다가
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, endPos, 0.2f);
            nowTime += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = endPos;

        nowTime = 0f; maxTime = 3f;
        endPos = transform.localPosition + Vector3.up * 1200f;

        while (nowTime <= maxTime) // 위로 이동
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, endPos, 0.2f);
            nowTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
