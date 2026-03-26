
using System;
using Unity.Netcode;

public class TrashCounter : BaseCounter
{
    public static event EventHandler OnAnyObjectTrash;

    new public static void ResetStaticData()
    {
        OnAnyObjectTrash = null;
    }

    public override void Interact(Player player)
    {
        if(player.HasKitchenMaterial())
        {
            KitchenMaterial.DestroyKitchenMaterial(player.GetKitchenMaterial());

            InteractLogicServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        OnAnyObjectTrash?.Invoke(this, EventArgs.Empty);
    }
}