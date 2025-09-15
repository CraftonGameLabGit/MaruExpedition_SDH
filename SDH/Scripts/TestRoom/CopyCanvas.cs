using UnityEngine;

public class CopyCanvas : MonoBehaviour
{
    [SerializeField] private GameObject characterCanvas;

    public void MakeCanvas(GameObject ch)
    {
        GameObject newCanvas = Instantiate(characterCanvas);

        newCanvas.GetComponent<TestShopCharacterCanvas>().SetShopCharacterCanvas(ch);
    }
}
