using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
{
    public event EventHandler OnRecipeSpawn;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;

    [SerializeField] private RecipeListScriptableObject recipeListSO;
    private List<RecipeScriptableObject> waitingRecipeSOList;

    private float spawnRecipeTimer = 4f;
    [SerializeField] private float spawnRecipeDuration;
    [SerializeField] private int waitingRecipesMax;

    private int successfulDeliveryCount = 0;

    public static DeliveryManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        waitingRecipeSOList = new List<RecipeScriptableObject>();
        spawnRecipeTimer = spawnRecipeDuration;
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }
        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0)
        {
            spawnRecipeTimer = spawnRecipeDuration;

            if (GameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipesMax)
            {
                int waitingRecipeSOIndex = UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count);
                SpawnNewWaitingRecipeClientRpc(waitingRecipeSOIndex);
            }
        }
    }

    [ClientRpc]
    private void SpawnNewWaitingRecipeClientRpc(int waitingRecipeSOIndex)
    {
        RecipeScriptableObject waitingRecipeSO = recipeListSO.recipeSOList[waitingRecipeSOIndex];

        waitingRecipeSOList.Add(waitingRecipeSO);
        OnRecipeSpawn?.Invoke(this, EventArgs.Empty);
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            RecipeScriptableObject waitingRecipeSO = waitingRecipeSOList[i];

            if (waitingRecipeSO.kitchenMaterialSOList.Count == plateKitchenObject.GetKitchenMaterialSOList().Count)
            {
                bool plateContentsMatchesTheRecipe = true;
                foreach (KitchenMaterialScriptible recipeKitchenObjectSO in waitingRecipeSO.kitchenMaterialSOList)
                {
                    bool ingredientFound = false;
                    foreach (KitchenMaterialScriptible plateKitchenObjectSO in plateKitchenObject.GetKitchenMaterialSOList())
                    {
                        if (plateKitchenObjectSO == recipeKitchenObjectSO)
                        {
                            ingredientFound = true;
                            break;
                        }
                    }
                    if (!ingredientFound)
                    {
                        plateContentsMatchesTheRecipe = false;
                    }
                }
                if (plateContentsMatchesTheRecipe)
                {
                    DeliverCorrectRecipeServerRpc(i);

                    return;
                }
            }
        }
        DeliverIncorrectRecipeServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverCorrectRecipeServerRpc(int waitingRecipeSOListIndex)
    {
        DeliverCorrectRecipeClientRpc(waitingRecipeSOListIndex);
    }

    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(int waitingRecipeSOListIndex)
    {
        waitingRecipeSOList.RemoveAt(waitingRecipeSOListIndex);

        OnRecipeCompleted?.Invoke(this, EventArgs.Empty);

        OnRecipeSuccess?.Invoke(this, EventArgs.Empty);

        successfulDeliveryCount++;
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverIncorrectRecipeServerRpc()
    {
        DeliverIncorrectRecipeClientRpc();
    }

    [ClientRpc]
    private void DeliverIncorrectRecipeClientRpc()
    {
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeScriptableObject> GetWaitingRecipeSOList()
    {
        return waitingRecipeSOList;
    }

    public int GetSuccessfulDeliveryCount()
    {
        return successfulDeliveryCount;
    }
}