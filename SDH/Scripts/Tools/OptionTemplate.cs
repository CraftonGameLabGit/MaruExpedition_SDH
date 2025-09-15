using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionTemplate : MonoBehaviour
{
    public Image OptionInnerTemplate => optionInnerTemplate;

    [SerializeField] Image optionInnerTemplate;

    public void SetVehicleIcon(int idx, bool isProp = false, float sizeCoef = -1f)
    {
        if (isProp) Destroy(GetComponent<Image>());

        GameObject vehicle = Instantiate(Managers.Asset.VehicleIcons[idx], transform);

        if (Managers.Unlock.IsMissionCleared(vehicle.GetComponent<IconInfo>().unlockType)) Destroy(optionInnerTemplate.gameObject);
        else optionInnerTemplate.transform.SetAsLastSibling(); // 맨 아래로
       
        if (sizeCoef < 0f) return;
        vehicle.GetComponent<RectTransform>().localScale *= sizeCoef;
    }

    public void SetCharacterIcon(int idx, bool isProp = false, float sizeCoef = -1f)
    {
        if (isProp) Destroy(GetComponent<Image>());
     
        GameObject character = Instantiate(Managers.Asset.CharacterIcons[idx], transform);

        Destroy(optionInnerTemplate.gameObject);

        if (SceneManager.GetActiveScene().name == "Select")
        {
            if (Managers.Unlock.IsMissionCleared(character.GetComponent<IconInfo>().unlockType))
            {
            }
            else
            {
                Destroy(character);
                character = Instantiate(Managers.Asset.Shadow, transform);
            }
        }

        if (sizeCoef < 0f) return;
        character.transform.localScale *= sizeCoef;
        //Destroy(character.GetComponentInChildren<Animator>());
    }

    public void SetInner(Sprite sprite, float size = -1f)
    {
        optionInnerTemplate.sprite = sprite;
        if (size < 0f) return;
        optionInnerTemplate.GetComponent<RectTransform>().localScale = new(size, size, 1f);
    }

    public void SetDiffficulty(Sprite sprite, float size = -1f, bool isClear  = true)
    {
        if (isClear)  optionInnerTemplate.sprite = sprite;
        if (size < 0f) return;
        optionInnerTemplate.GetComponent<RectTransform>().localScale = new(size, size, 1f);
    }
}
