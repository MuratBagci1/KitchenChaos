using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateKitchenObject : KitchenMaterial
{
    public event EventHandler<OnIngredientEventArgs> OnIngredientAdded;
    public class OnIngredientEventArgs : EventArgs
    {
        public KitchenMaterialScriptible kitchenMaterialSO;
    }

    [SerializeField] private List<KitchenMaterialScriptible> validKitchenMaterialSOList;

    private List<KitchenMaterialScriptible> kitchenMaterialSOList;

    protected override void Awake()
    {
        base.Awake();
        kitchenMaterialSOList = new List<KitchenMaterialScriptible>();
    }

    public bool TryAddIngredient(KitchenMaterialScriptible kitchenMaterialSO)
    {
        int kitchenMaterialIndex = KitchenGameMultiplayer.Instance.GetKitchenMaterialSOIndex(kitchenMaterialSO);

        if (!validKitchenMaterialSOList.Contains(kitchenMaterialSO))
        {
            return false;
        }
        else if (kitchenMaterialSOList.Contains(kitchenMaterialSO))
        {
            return false;
        }
        else
        {
            TryAddIngredientServerRpc(kitchenMaterialIndex);

            return true;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void TryAddIngredientServerRpc(int kitchenMaterialIndex)
    {
        TryAddIngredientClientRpc(kitchenMaterialIndex);
    }

    [ClientRpc]
    private void TryAddIngredientClientRpc(int kitchenMaterialIndex)
    {
        KitchenMaterialScriptible kitchenMaterialSO = KitchenGameMultiplayer.Instance.GetKithenMaterialSOFromIndex(kitchenMaterialIndex);

        kitchenMaterialSOList.Add(kitchenMaterialSO);

        OnIngredientAdded?.Invoke(this, new OnIngredientEventArgs
        {
            kitchenMaterialSO = kitchenMaterialSO
        });
    }

    public List<KitchenMaterialScriptible> GetKitchenMaterialSOList()
    {
        return kitchenMaterialSOList;
    }
}