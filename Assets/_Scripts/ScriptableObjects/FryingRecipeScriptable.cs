using UnityEngine;

[CreateAssetMenu(fileName = "KitchenMaterial", menuName = "FryingRecipe")]
public class FryingRecipeScriptable : ScriptableObject
{
    public KitchenMaterialScriptible input;
    public KitchenMaterialScriptible output;
    public float fryingTimerMax;
}
