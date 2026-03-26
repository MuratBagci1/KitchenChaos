using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugUI : MonoBehaviour
{
    [SerializeField] private GameObject debugTextPrefab;
    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
    [SerializeField] private ScrollRect scrollRect;

    public static DebugUI Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void AddDebug(string content)
    {
        GameObject instantiatedObject = Instantiate(debugTextPrefab, verticalLayoutGroup.transform);

        TextMeshProUGUI text = instantiatedObject.GetComponent<TextMeshProUGUI>();

        text.text = content;

        StartCoroutine(ScrollToBottomNextFrame());
    }

    private IEnumerator ScrollToBottomNextFrame()
    {
        yield return null; // wait for layout update
        scrollRect.verticalNormalizedPosition = 0f; // 0 = bottom, 1 = top
    }
}
