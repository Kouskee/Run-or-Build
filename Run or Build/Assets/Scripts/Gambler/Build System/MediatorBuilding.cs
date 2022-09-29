using UnityEngine;
using UnityEngine.Tilemaps;

public class MediatorBuilding : MonoBehaviour
{
    [SerializeField] private Tilemap _tileMap;
    [SerializeField] private BuildingSystem _buildingSystem;

    private Inventory _inventory;
    private InventorySlot _slot;

    public void Init(Inventory inventory)
    {
        _inventory = inventory;
        _buildingSystem.Init(inventory, _tileMap);
    }
    
    public void ClearSelection()
    {
        _buildingSystem.ClearHashObjectsOnClient();
    }
    
    public void CreateBuilding(string name)
    {
        var slot = _inventory.GetSlot(name);
        ReturnBuilding(slot);
        _buildingSystem.StartPlacingBuilding(slot);
    }

    public void CreateTile(string name)
    {
        var slot = _inventory.GetSlot(name);
        ReturnBuilding(slot);
        _buildingSystem.SpawnTile(slot);
    }

    private void ReturnBuilding(InventorySlot slot)
    {
        if (_slot != null)
        {
            if (_slot != slot) _slot.ReturnBuilding();
        }
        _slot = slot;
    }

    // private void Update()
    // {
    //     if (!Input.GetMouseButtonDown(0)) return;
    //     if (_buildingConfig == null) return;
    //
    //     var ray = _camera.ScreenPointToRay(Input.mousePosition);
    //     var plane = new Plane(Vector3.forward, Vector3.zero);
    //     plane.Raycast(ray, out var distance);
    //     var clickWorldPos = ray.GetPoint(distance);
    //
    //     var clickCell = _tileMap.WorldToCell(clickWorldPos);
    //     //Debug.Log(_tileMap.GetCellCenterWorld(clickCell)); // робит
    //     if(_tileMap.GetTile(clickCell) == _buildingConfig.Tile) return; 
    //     
    //     _text.text = (int.Parse(_text.text) - 1).ToString();
    //     _tileMap.SetTile(clickCell, _buildingConfig.Tile);
    // }
}