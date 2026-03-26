using Unity.Netcode;
using UnityEngine;

public interface IKitchenObjectParent
{
    public Transform GetKitchenMaterialFollowTransform();

    public void SetKitchenMaterial(KitchenMaterial kitchenMaterial);

    public KitchenMaterial GetKitchenMaterial();

    public void ClearKitchenMaterial();

    public bool HasKitchenMaterial();

    public NetworkObject GetNetworkObject();
}
