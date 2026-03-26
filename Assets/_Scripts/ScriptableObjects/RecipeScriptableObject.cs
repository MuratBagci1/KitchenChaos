using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeSO", menuName = "RecipeSO")]
public class RecipeScriptableObject : ScriptableObject
{
    public List<KitchenMaterialScriptible> kitchenMaterialSOList;
    public string recipeName;
}
