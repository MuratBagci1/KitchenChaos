using System;
using Unity.Netcode;
using UnityEngine;

public class BaseCounter : NetworkBehaviour, IKitchenObjectParent
{
    public static event EventHandler OnDroppedSomething;

    public static void ResetStaticData()
    {
        OnDroppedSomething = null;
    }

    [SerializeField] private Transform counterTopPoint;

    private KitchenMaterial kitchenMaterial;

    public virtual void Interact(Player player)
    {

    }

    public virtual void InteractAlternate(Player player)
    {
        //Debug.LogError("BaseCounter.InteractAlternate();");
    }

    public Transform GetKitchenMaterialFollowTransform()
    {
        return counterTopPoint;
    }

    public void SetKitchenMaterial(KitchenMaterial kitchenMaterial)
    {
        if (kitchenMaterial != null)
        {
            OnDroppedSomething?.Invoke(this, EventArgs.Empty);
        }
        this.kitchenMaterial = kitchenMaterial;
    }

    public KitchenMaterial GetKitchenMaterial()
    {
        return kitchenMaterial;
    }

    public void ClearKitchenMaterial()
    {
        kitchenMaterial = null;
    }

    public bool HasKitchenMaterial()
    {
        return kitchenMaterial != null;
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}
