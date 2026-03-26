using UnityEngine;

[CreateAssetMenu(fileName = "KitchenMaterial", menuName = "CuttingRecipe")]
public class CuttingRecipeScriptable : ScriptableObject
{
    public KitchenMaterialScriptible input;
    public KitchenMaterialScriptible output;
    public int cuttingProgressMax;
}
