using UnityEngine;
using UnityEngine.UI;

public class PlateIconSingleUI : MonoBehaviour
{
    [SerializeField] private Image image;

    public void SetKitchenObjectSO(KitchenMaterialScriptible kitchenMaterialSO)
    {
        image.sprite = kitchenMaterialSO.sprite;
    }
}