using UnityEngine;

public class ClearCounter : BaseCounter
{
    public override void Interact(Player player)
    {
        //put the kitchen object to clear counter
        if (!HasKitchenMaterial())
        {
            if(player.HasKitchenMaterial())
            {
                player.GetKitchenMaterial().SetKitchenMaterialParent(this);
            }
        }
        //take kitchen object from clear counter
        else
        {
            if (player.HasKitchenMaterial())
            {
                if(player.GetKitchenMaterial().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenMaterial().GetKitchenMaterialSO()))
                    {
                        KitchenGameMultiplayer.Instance.DestroyKitchenMaterial(GetKitchenMaterial());
                    }
                }
                else
                {
                    if (GetKitchenMaterial().TryGetPlate(out plateKitchenObject))
                    {
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenMaterial().GetKitchenMaterialSO()))
                        {
                            KitchenGameMultiplayer.Instance.DestroyKitchenMaterial(player.GetKitchenMaterial());
                        }
                    }
                }
            }
            else
            {
                GetKitchenMaterial().SetKitchenMaterialParent(player);
            }
        }
    }
}