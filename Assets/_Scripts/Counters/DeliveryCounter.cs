using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryCounter : BaseCounter
{
    public static DeliveryCounter Instance {  get; private set; }

    private void Awake()
    {
        Instance = this;
        //if (Instance != null && Instance != this)
        //{
        //    Destroy(Instance);

            
        //}
    }

    public override void Interact(Player player)
    {
        if(player.HasKitchenMaterial())
        {
            if (player.GetKitchenMaterial().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                DeliveryManager.Instance.DeliverRecipe(plateKitchenObject);

                KitchenMaterial.DestroyKitchenMaterial(player.GetKitchenMaterial());
            }
        }
    }
}
