using System;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
    public event EventHandler OnPlateCreated;
    public event EventHandler OnPlateRemoved;

    private float spawnPlateTimer;
    private int plateSpawnAmount;
    [SerializeField] private int plateSpawnAmountMax;
    [SerializeField] private float spawnPlateDuration;
    [SerializeField] private KitchenMaterialScriptible plateKitchenObjectSO;

    private void Update()
    {
        if (!IsServer)
            return;

        spawnPlateTimer += Time.deltaTime;

        if(spawnPlateTimer > spawnPlateDuration)
        {
            spawnPlateTimer = 0;
            if(GameManager.Instance.IsGamePlaying() && plateSpawnAmount < plateSpawnAmountMax)
            {
                SpawnPlateServerRpc();
            }
        }
    }

    [ServerRpc]
    private void SpawnPlateServerRpc()
    {
        SpawnPlateClientRpc();
    }

    [ClientRpc]
    private void SpawnPlateClientRpc()
    {
        plateSpawnAmount++;
        OnPlateCreated?.Invoke(this, EventArgs.Empty);
    }

    public override void Interact(Player player)
    {
        if(!player.HasKitchenMaterial() && plateSpawnAmount > 0)
        {
            KitchenMaterial.SpawnKitchenMaterial(plateKitchenObjectSO, player);

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
        plateSpawnAmount--;

        OnPlateRemoved?.Invoke(this, EventArgs.Empty);
    }
}
