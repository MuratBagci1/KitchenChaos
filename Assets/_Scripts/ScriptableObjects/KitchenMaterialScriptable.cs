using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KitchenMaterial", menuName = "KitchenMaterial")]
public class KitchenMaterialScriptible : ScriptableObject
{
    public Transform kitchenMaterialPrefab;
    //public Transform slicedKitchenMaterialprefab;
    public Sprite sprite;
    public string objectName;
}
