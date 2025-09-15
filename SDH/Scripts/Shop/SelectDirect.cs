using System.Collections;
using UnityEngine;

public class SelectDirect : MonoBehaviour // ������ SelectDisplayTemplate�� ���� ȿ��
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

        while (nowTime <= maxTime) // �Ʒ��� ��¦ �������ٰ�
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, endPos, 0.2f);
            nowTime += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = endPos;

        nowTime = 0f; maxTime = 3f;
        endPos = transform.localPosition + Vector3.up * 1200f;

        while (nowTime <= maxTime) // ���� �̵�
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, endPos, 0.2f);
            nowTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
