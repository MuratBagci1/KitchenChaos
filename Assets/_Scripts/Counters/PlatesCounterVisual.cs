using System.Collections.Generic;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour
{
    [SerializeField] private GameObject plateVisualPrefab;
    [SerializeField] private PlatesCounter platesCounter;
    [SerializeField] private Transform instantiatePosition;

    private List<GameObject> plateVisualObjectList;

    private void Awake()
    {
        plateVisualObjectList = new List<GameObject>();
    }

    private void Start()
    {
        platesCounter.OnPlateCreated += PlatesCounter_OnPlateCreated;
        platesCounter.OnPlateRemoved += PlatesCounter_OnInteracted;
    }

    private void PlatesCounter_OnPlateCreated(object sender, System.EventArgs e)
    {
        GameObject spawnedPlateVisual = Instantiate(plateVisualPrefab, instantiatePosition);

        float PlateOffsetY = .1f;

        spawnedPlateVisual.transform.localPosition = new Vector3(0, PlateOffsetY * plateVisualObjectList.Count, 0);

        plateVisualObjectList.Add(spawnedPlateVisual);
    }

    private void PlatesCounter_OnInteracted(object sender, System.EventArgs e)
    {
        GameObject lastPlate = plateVisualObjectList[plateVisualObjectList.Count - 1];

        Destroy(lastPlate);

        plateVisualObjectList.Remove(lastPlate);
    }
}