using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryManagerSingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private Transform iconContainer;
    [SerializeField] private Transform iconTemplate;

    private void Awake()
    {
        iconTemplate.gameObject.SetActive(false);
    }

    public void SetRecipeSO(RecipeScriptableObject recipeSO)
    {
        recipeNameText.text = recipeSO.recipeName;

        foreach (Transform child in iconContainer)
        {
            if (child == iconTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (KitchenMaterialScriptible kitchenMaterial in recipeSO.kitchenMaterialSOList)
        {
            Transform generatedIcon = Instantiate(iconTemplate, iconContainer);
            generatedIcon.GetComponent<Image>().sprite = kitchenMaterial.sprite;

            generatedIcon.gameObject.SetActive(true);
        }
    }
}