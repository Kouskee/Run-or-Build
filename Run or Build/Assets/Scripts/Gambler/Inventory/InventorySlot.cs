using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot
{
    private Button _button;
    private TMP_Text _text;
    private BuildingConfig _config;

    private int _count;

    public InventorySlot(Button button, BuildingConfig config)
    {
        _button = button;
        _button.interactable = true;
        _text = button.GetComponentInChildren<TMP_Text>();
        _config = config;
        _count = _config.Count;
    }

    public BuildingConfig TryGetBuilding()
    {
        _button.interactable = false;
        _text.text = _count.ToString();
        return _config;
    }

    public bool CanPlaceBuilding()
    {
        return _count > 0;
    }

    public void ApplyChanges()
    {
        _count--;
        _text.text = Mathf.Clamp(_count, 0, 999).ToString();
    }

    public void ReturnBuilding()
    {
        if(_count > 0)
            _button.interactable = true;
        _text.text = _config.Name;
    }
}