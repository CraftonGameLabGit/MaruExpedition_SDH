using TMPro;
using UnityEngine;

public class SetDifficultyTxt : MonoBehaviour
{
    private void Start()
    {
        if (Managers.Stage.Difficulty == 0) Destroy(gameObject);
        else GetComponent<TextMeshProUGUI>().text = Managers.Stage.Difficulty.ToString();
    }
}
