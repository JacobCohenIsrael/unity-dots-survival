using UnityEngine;

public class UnitSelectionUiManager : MonoBehaviour
{
    [SerializeField] private RectTransform selectionAreaRectTransform;
    [SerializeField] private Canvas screenCanvas;
    private void Start()
    {
        UnitSelectionManager.Instance.OnSelectionAreaStart += UnitSelectionManager_OnSelectionAreaStart;
        UnitSelectionManager.Instance.OnSelectionAreaEnd += UnitSelectionManager_OnSelectionAreaEnd;
        selectionAreaRectTransform.gameObject.SetActive(false);
    }
    
    private void Update()
    {
        if (selectionAreaRectTransform.gameObject.activeSelf)
        {
            UpdateVisual();
        }
    }
    
    private void OnDestroy()
    {
        UnitSelectionManager.Instance.OnSelectionAreaStart -= UnitSelectionManager_OnSelectionAreaStart;
        UnitSelectionManager.Instance.OnSelectionAreaEnd -= UnitSelectionManager_OnSelectionAreaEnd;
    }

    private void UnitSelectionManager_OnSelectionAreaStart()
    {
        selectionAreaRectTransform.gameObject.SetActive(true);
        UpdateVisual();
    }
    
    private void UnitSelectionManager_OnSelectionAreaEnd()
    {
        selectionAreaRectTransform.gameObject.SetActive(false);
    }

    private void UpdateVisual()
    {
        var selectionAreaRect = UnitSelectionManager.Instance.GetSelectionAreaRect();

        var canvasScale = screenCanvas.transform.localScale.x;
        selectionAreaRectTransform.anchoredPosition = new Vector2(selectionAreaRect.x, selectionAreaRect.y) / canvasScale;
        selectionAreaRectTransform.sizeDelta = new Vector2(selectionAreaRect.width, selectionAreaRect.height) / canvasScale;
    }

}
