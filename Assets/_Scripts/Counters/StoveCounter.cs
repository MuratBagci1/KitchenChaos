using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using static CuttingCounter;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler OnCook;
    public event EventHandler OffCook;
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    [SerializeField] private FryingRecipeScriptable[] fryingRecipeArray;
    private FryingRecipeScriptable fryingRecipeSO;

    private NetworkVariable<float> fryingTimer = new NetworkVariable<float>(0f);
    private NetworkVariable<float> burningTimer = new NetworkVariable<float>(0f);
    private NetworkVariable<State> state = new NetworkVariable<State>(State.Idle);

    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned
    }

    public override void OnNetworkSpawn()
    {
        fryingTimer.OnValueChanged += FryingTimer_OnValueChanged;
        burningTimer.OnValueChanged += BurningTimer_OnValueChanged;
        state.OnValueChanged += State_OnValueChanged;
    }

    private void FryingTimer_OnValueChanged(float oldValue, float newValue)
    {
        float fryingTimerMax = fryingRecipeSO != null ? fryingRecipeSO.fryingTimerMax : 1f;
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = fryingTimer.Value / fryingTimerMax
        });
    }

    private void BurningTimer_OnValueChanged(float oldValue, float newValue)
    {
        float burningTimerMax = fryingRecipeSO != null ? fryingRecipeSO.fryingTimerMax : 1f;
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = burningTimer.Value / burningTimerMax
        });
    }

    private void State_OnValueChanged(State oldState, State newState)
    {
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
        {
            state = state.Value
        });

        if (state.Value == State.Idle || state.Value == State.Burned)
        {
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = 0f
            });
        }
    }

    private void Update()
    {
        if (!IsServer)
            return;

        if(HasKitchenMaterial())
        {
            switch (state.Value)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer.Value += Time.deltaTime;                                     

                    if (fryingTimer.Value >= fryingRecipeSO.fryingTimerMax)
                    {
                        KitchenMaterial.DestroyKitchenMaterial(GetKitchenMaterial());

                        KitchenMaterial.SpawnKitchenMaterial(fryingRecipeSO.output, this);

                        int kitchenMaterialIndex = KitchenGameMultiplayer.Instance.GetKitchenMaterialSOIndex(GetKitchenMaterial().GetKitchenMaterialSO());
                        SetFryingRecipeSOClientRpc(kitchenMaterialIndex);

                        burningTimer.Value = 0;
                        state.Value = State.Fried;
                    }
                    break;
                case State.Fried:
                    burningTimer.Value += Time.deltaTime;                                  
                    
                    if (burningTimer.Value >= fryingRecipeSO.fryingTimerMax)
                    {
                        KitchenMaterial.DestroyKitchenMaterial(GetKitchenMaterial());

                        KitchenMaterial.SpawnKitchenMaterial(fryingRecipeSO.output, this);

                        state.Value = State.Burned;
                    }
                    break;
                case State.Burned:
                    break;
                default:
                    break;
            }
        }
    }

    public override void Interact(Player player)
    {
        if (!HasKitchenMaterial())
        {
            if(player.HasKitchenMaterial())
            {
                if (HasRecipeWithInput(player.GetKitchenMaterial().GetKitchenMaterialSO()))
                {
                    KitchenMaterial kitchenMaterialToCook = player.GetKitchenMaterial();

                    kitchenMaterialToCook.SetKitchenMaterialParent(this);

                    int kitchenMaterialIndex = KitchenGameMultiplayer.Instance.GetKitchenMaterialSOIndex(kitchenMaterialToCook.GetKitchenMaterialSO());
                    InteractLogicPlaceObjectOnCounterServerRpc(kitchenMaterialIndex);
                }
            }
        }
        else
        {
            if(player.HasKitchenMaterial())
            {
                if (player.GetKitchenMaterial().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenMaterial().GetKitchenMaterialSO()))
                    {
                        KitchenGameMultiplayer.Instance.DestroyKitchenMaterial(GetKitchenMaterial());

                        SetStateIdleServerRpc();

                        //OffCook?.Invoke(this, EventArgs.Empty);

                        //fryingTimer.Value = 0;
                        //burningTimer.Value = 0;
                    }
                }
            }
            else
            {
                GetKitchenMaterial().SetKitchenMaterialParent(player);

                SetStateIdleServerRpc();

                //OffCook?.Invoke(this, EventArgs.Empty);

                //fryingTimer.Value = 0;
                //burningTimer.Value = 0;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetStateIdleServerRpc()
    {
        state.Value = State.Idle;

        OffCook?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc(int kitchenMaterialIndex)
    {
        fryingTimer.Value = 0;
        state.Value = State.Frying;
        SetFryingRecipeSOClientRpc(kitchenMaterialIndex);
    }

    [ClientRpc]
    private void SetFryingRecipeSOClientRpc(int kitchenMaterialIndex)
    {
        fryingRecipeSO = GetFryingRecipeSOWithInput(KitchenGameMultiplayer.Instance.GetKithenMaterialSOFromIndex(kitchenMaterialIndex));

        burningTimer.Value = 0;

        OnCook?.Invoke(this, EventArgs.Empty);
    }

    private bool HasRecipeWithInput(KitchenMaterialScriptible input)
    {
        if (GetFryingRecipeSOWithInput(input) != null)
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
        return GetFryingRecipeSOWithInput(input).output;
    }

    private FryingRecipeScriptable GetFryingRecipeSOWithInput(KitchenMaterialScriptible input)
    {
        foreach (FryingRecipeScriptable item in fryingRecipeArray)
        {
            if (item.input == input)
            {
                Debug.Log(item.input.name);
                return item;
            }
        }
        return null;

    }

    public bool IsFried()
    {
        return state.Value == State.Fried;
    }

    //Cook - tiers

    //Particles

    //Bar
}