using System;
using Unity.Netcode;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
    public static event EventHandler OnAnyCut;

    new public static void ResetStaticData()
    {
        OnAnyCut = null;
    }

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnCut;

    public class OnProgressChangedEventArgs
    {
        public float progressNormalized;
    }

    [SerializeField] private CuttingRecipeScriptable[] cutKitchenObjectSOArray;

    private int cuttingProgress;

    public override void Interact(Player player)
    {
        //put the kitchen object to clear counter
        if (!HasKitchenMaterial())
        {
            if (player.HasKitchenMaterial())
            {
                KitchenMaterial takenMaterial = player.GetKitchenMaterial();
                if (HasRecipeWithInput(takenMaterial.GetKitchenMaterialSO()))
                {
                    takenMaterial.SetKitchenMaterialParent(this);

                    InteractLogicPlaceObjectOnCounterServerRpc();
                }
            }
        }
        //take kitchen object from clear counter
        else
        {
            if (player.HasKitchenMaterial())
            {
                if (player.GetKitchenMaterial().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenMaterial().GetKitchenMaterialSO()))
                    {
                        KitchenGameMultiplayer.Instance.DestroyKitchenMaterial(GetKitchenMaterial());
                    }
                }
            }
            else
            {
                GetKitchenMaterial().SetKitchenMaterialParent(player);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc()
    {
        InteractLogicPlaceObjectOnCounterClientRpc();
    }

    [ClientRpc]
    private void InteractLogicPlaceObjectOnCounterClientRpc()
    {
        cuttingProgress = 0;

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = 0f
        });
    }

    public override void InteractAlternate(Player player)
    {
        if (HasRecipeWithInput(GetKitchenMaterial().GetKitchenMaterialSO()))
        {
            CutObjectServerRpc();
            TestCuttingProgressDoneServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CutObjectServerRpc()
    {
        if (HasRecipeWithInput(GetKitchenMaterial().GetKitchenMaterialSO()))
        {
            CutObjectClientRpc();
        }
    }

    [ClientRpc]
    private void CutObjectClientRpc() 
    {
        cuttingProgress++;

        CuttingRecipeScriptable cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenMaterial().GetKitchenMaterialSO());

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
        });

        OnCut?.Invoke(this, EventArgs.Empty);

        OnAnyCut?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership =false)]
    private void TestCuttingProgressDoneServerRpc()
    {
        if (HasRecipeWithInput(GetKitchenMaterial().GetKitchenMaterialSO()))
        {
            CuttingRecipeScriptable cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenMaterial().GetKitchenMaterialSO());

            if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
            {
                KitchenMaterialScriptible kitchenMaterialToSpawn = GetOutputForInput(GetKitchenMaterial().GetKitchenMaterialSO());

                KitchenMaterial.DestroyKitchenMaterial(GetKitchenMaterial());

                KitchenMaterial.SpawnKitchenMaterial(kitchenMaterialToSpawn, this);
            }
        }
    }

    private bool HasRecipeWithInput(KitchenMaterialScriptible input)
    {
        if (GetCuttingRecipeSOWithInput(input) != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private KitchenMaterialScriptible GetOutputForInput(KitchenMaterialScriptible input)
    {
        //if (GetCuttingRecipeSOWithInput(input) != null)
        //{
        //    return GetCuttingRecipeSOWithInput(input).output;
        //}
        //else
        //{
        //    return null;
        //}

        return GetCuttingRecipeSOWithInput(input).output;
    }

    private CuttingRecipeScriptable GetCuttingRecipeSOWithInput(KitchenMaterialScriptible input)
    {
        foreach (CuttingRecipeScriptable item in cutKitchenObjectSOArray)
        {
            if (item.input == input)
            {
                return item;
            }
        }
        return null;

    }
}
