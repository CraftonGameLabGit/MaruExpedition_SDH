using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMPCanvas : MonoBehaviour
{
    [SerializeField] private RectTransform[] manas;
    [SerializeField] private Image[] manaMPs;
    [SerializeField] private Image[] manasBackGround;
    [SerializeField] private GameObject skillReadyEffect;
    private bool isOn; // 필드전인지 (마나 세팅을 할 지)
    private bool isOverlap; // (이전 UI 기준) UI가 겹치는지
    private Color initBackgroundManaColor;

    private void Awake()
    {
        isOn = false;
        initBackgroundManaColor = manasBackGround[0].color;
    }

    public void StartManaCanvas()
    {
        isOn = true;
        SetMPCanvas();
    }

    public void SetMPCanvas()
    {
        if (!isOn) return;

        StopAllCoroutines();

        int x = 0;
        for (int i = 0; i < Managers.PlayerControl.Characters.Count; i++)
        {
            Character nowCharacter = Managers.PlayerControl.Characters[i].GetComponent<Character>();

            if (nowCharacter.IsGround)
            {
                if (nowCharacter.maxMP != nowCharacter.CurrentMP)
                    manasBackGround[x].gameObject.SetActive(false);

                manas[x].gameObject.SetActive(true);

                // 겹침 처리
                float baseX = nowCharacter.transform.position.x;
                float manasY = 2f;
                for (int j = 0; j < x; j++)
                {
                    if (Mathf.Abs(manas[j].position.x - baseX) < 0.3f)
                    {
                        manasY += 0.5f;
                    }
                }

                StartCoroutine(ManaBar(x, Managers.PlayerControl.Characters[i], manasY));
                x++;
            }
            else
            {
                manasBackGround[i].gameObject.SetActive(false);
                manas[i].gameObject.SetActive(false);
                manas[i].GetComponent<Image>().fillAmount = 0f;
            }
        }

        for (; x < Constant.maxCharacter; x++)
        {
            manasBackGround[x].gameObject.SetActive(false);
            manas[x].gameObject.SetActive(false);
            manas[x].GetComponent<Image>().fillAmount = 0f;
        }
    }

    public void HideMPCanvas()
    {
        isOn = false;
        StopAllCoroutines();
        for (int i = 0; i < Constant.maxCharacter; i++)
        {
            manasBackGround[i].gameObject.SetActive(false);
            manas[i].gameObject.SetActive(false);
            manaMPs[i].fillAmount = 0f;
        }
    }

    private IEnumerator ManaBar(int idx, GameObject characterObj, float manasY)
    {
        Character character = characterObj.GetComponent<Character>();
        bool isFill = false;

        while (true)
        {
            manaMPs[idx].fillAmount = character.CurrentMP / character.maxMP;
            manas[idx].position = new(characterObj.transform.position.x, transform.position.y + manasY, characterObj.transform.position.z);

            if (manaMPs[idx].fillAmount == 1 && !isFill)
            {
                isFill = true;
                manasBackGround[idx].gameObject.SetActive(true);
                Instantiate(skillReadyEffect, manasBackGround[idx].transform.position, Quaternion.identity, transform);
            }

            yield return null;
        }
    }
}
