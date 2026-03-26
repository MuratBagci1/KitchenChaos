using Unity.Netcode;
using UnityEngine;

public class KitchenMaterial : NetworkBehaviour
{
    [SerializeField] private KitchenMaterialScriptible kitchenMaterialSO;

    private IKitchenObjectParent kitchenMaterialParent;

    private FollowTransform followTransform;

    protected virtual void Awake()
    {
        followTransform = GetComponent<FollowTransform>();
    }

    public KitchenMaterialScriptible GetKitchenMaterialSO()
    {
        return kitchenMaterialSO;
    }

    public void SetKitchenMaterialParent(IKitchenObjectParent kitchenMaterialParent)
    {
        NetworkObjectReference parentNetworkObjectReference = kitchenMaterialParent.GetNetworkObject();

        SetKitchenObjectParenServerRpc(parentNetworkObjectReference);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetKitchenObjectParenServerRpc(NetworkObjectReference kitchenMaterialParentNetworkObjectReference)
    {
        SetKitchenObjectParentClientRpc(kitchenMaterialParentNetworkObjectReference);
    }

    [ClientRpc]
    private void SetKitchenObjectParentClientRpc(NetworkObjectReference kitchenMaterialParentNetworkObjectReference)
    {
        kitchenMaterialParentNetworkObjectReference.TryGet(out NetworkObject networkObject);
        IKitchenObjectParent kitchenMaterialParent = networkObject.GetComponent<IKitchenObjectParent>();

        if (this.kitchenMaterialParent != null)
        {
            this.kitchenMaterialParent.ClearKitchenMaterial();
        }

        this.kitchenMaterialParent = kitchenMaterialParent;

        if (kitchenMaterialParent.HasKitchenMaterial())
        {
            Debug.Log("Kitchen Material Parent already has a kitchen object");
        }

        this.kitchenMaterialParent.SetKitchenMaterial(this);

        followTransform.SetTargetTransform(kitchenMaterialParent.GetKitchenMaterialFollowTransform());
    }

    public IKitchenObjectParent GetKitchenMaterialParent()
    {
        return kitchenMaterialParent;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void ClearKitchenMaterialOnParent()
    {
        kitchenMaterialParent.ClearKitchenMaterial();
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject)
    {
        if(this is  PlateKitchenObject)
        {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        }
        else
        {
            plateKitchenObject = null;
            return false;
        }
    }

    public static void SpawnKitchenMaterial(KitchenMaterialScriptible kitchenMaterialSO, IKitchenObjectParent parent)
    {
        KitchenGameMultiplayer.Instance.SpawnKitchenObject(kitchenMaterialSO, parent);
    }

    public static void DestroyKitchenMaterial(KitchenMaterial kitchenMaterial)
    {
        KitchenGameMultiplayer.Instance.DestroyKitchenMaterial(kitchenMaterial);
    }

    public NetworkObjectReference GetNetworkObject()
    {
        return NetworkObject;
    }
}
