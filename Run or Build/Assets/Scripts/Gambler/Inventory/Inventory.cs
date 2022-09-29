using System.Collections.Generic;
using UnityEngine.UI;

public class Inventory
{
    private readonly Dictionary<string, InventorySlot> _slots;

    public Inventory(Dictionary<Button, BuildingConfig> buildings)
    {
        _slots = new Dictionary<string, InventorySlot>(buildings.Count);

        foreach (var building in buildings)
        {
            var slot = new InventorySlot(building.Key, building.Value);
            _slots.Add(building.Value.Name, slot);
            slot.ReturnBuilding();
        }
    }

    public InventorySlot GetSlot(string building)
    {
        return _slots.ContainsKey(building) ? _slots[building] : null;
    }
}