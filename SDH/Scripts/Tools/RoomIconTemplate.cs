using UnityEngine;
using UnityEngine.UI;

public enum RoomType
{
    Clear,
    Battle,
    Boss
}

public class RoomIconTemplate : MonoBehaviour
{
    [SerializeField] Image roomInnerIcon;
    [SerializeField] Sprite[] sprites;
    [SerializeField] Sprite bossIcon;

    public void SetRoomIcon(RoomType roomType)
    {
        GetComponent<Image>().sprite = sprites[(int)roomType];

        if (roomType != RoomType.Boss) Destroy(roomInnerIcon.gameObject);

        roomInnerIcon.sprite = bossIcon;
    }
}
