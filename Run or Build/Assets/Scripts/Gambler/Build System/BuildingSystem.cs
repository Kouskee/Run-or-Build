using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class BuildingSystem : NetworkBehaviour
{
    private Tilemap _tileMap;
    private Platform _platform;
    private Camera _camera;
    private BoxCollider2D _colliderBuilding;
    private Inventory _inventory;
    private InventorySlot _slot;
    private Coroutine _placingCoroutine;
    
    private string _buildingName;

    private List<Platform> _platforms = new List<Platform>();
    private readonly Collider2D[] _resultOverlap = new Collider2D[5];

    public void Init(Inventory inventory, Tilemap tileMap)
    {
        _inventory = inventory;
        _tileMap = tileMap;
        ClearHashObjectsOnClient();
    }

    private void Awake()
    {
        _camera = Camera.main;
    }

    public void StartPlacingBuilding(InventorySlot slot)
    {
        ClearHashObjectsOnClient();
        _slot = slot;
        if(!_slot.CanPlaceBuilding()) return;
        
        var config = slot.TryGetBuilding();
        if(_buildingName == config.Name) return;

        _buildingName = config.Name;
        _platform = Instantiate(config.Platform);
        _platform.TryGetComponent(out _colliderBuilding);
        
        _placingCoroutine = StartCoroutine(PlacingBuildingCoroutine());
    }

    public void SpawnTile(InventorySlot slot)
    {
        ClearHashObjectsOnClient();
        _slot = slot;
        _placingCoroutine = StartCoroutine(SpawnTileCoroutine());
    }

    private IEnumerator SpawnTileCoroutine()
    {
        var config = _slot.TryGetBuilding();
        while (true)
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                var ray = _camera.ScreenPointToRay(Input.mousePosition);
                var plane = new Plane(Vector3.forward, Vector3.zero);
                plane.Raycast(ray, out var distance);
                var clickWorldPos = ray.GetPoint(distance);
                var clickCell = _tileMap.WorldToCell(clickWorldPos);
                
                TrySpawnTileCmd(clickCell, config.Name);
            }
            
            yield return null;
        }
    }
    
    [Command]
    private void TrySpawnTileCmd(Vector3Int cellPosition, string buildingName)
    {
        if(cellPosition.x < 0 || cellPosition.y < 0) return;
        var slot = _inventory.GetSlot(buildingName);
        if(!slot.CanPlaceBuilding()) return;
        var tile = slot.TryGetBuilding().Tile;
        if (_tileMap.GetTile(cellPosition) == tile) return;
        slot.ApplyChanges();
        _tileMap.SetTile(cellPosition, tile);
        SpawnTileClientRpc(cellPosition, buildingName);
    }

    [ClientRpc]
    private void SpawnTileClientRpc(Vector3Int cellPosition, string buildingName)
    {
        if(isServer) return;
        var slot = _inventory.GetSlot(buildingName);
        var tile = slot.TryGetBuilding().Tile;
        slot.ApplyChanges();
        _tileMap.SetTile(cellPosition, tile);
    }
    
    private IEnumerator PlacingBuildingCoroutine()
    {
        while (true)
        {
            var groundPlane = new Plane(Vector3.back, Vector3.zero);
            var ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (!groundPlane.Raycast(ray, out var position)) yield return null;

            var worldPosition = ray.GetPoint(position);

            var x = worldPosition.x;
            var y = worldPosition.y;

            var available = !IsPlaceTaken();

            _platform.transform.position = new Vector3(x, y, 0);
            _platform.SetTransparent(available);

            if (available && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                PlaceFlyingBuilding(_platform.transform.position, _buildingName);

            yield return null;
        }
    }

    private bool IsPlaceTaken()
    {
        var overlap = Physics2D.OverlapBoxNonAlloc(_colliderBuilding.transform.position, _colliderBuilding.size, 0, _resultOverlap);
        return overlap > 1;
    }

    [Command]
    private void PlaceFlyingBuilding(Vector3 position, string buildingName)
    {
        var slot = _inventory.GetSlot(buildingName);
        slot.ApplyChanges();
        var buildingPrefab = slot.TryGetBuilding().Platform;
        var go = Instantiate(buildingPrefab, position, Quaternion.identity).gameObject;
        _platforms.Add(buildingPrefab);
        
        NetworkServer.Spawn(go);
        var building = go.GetComponent<Platform>();
        building.SetNormalServer();
        ReportToClient(building, buildingName);
        
        ClearHashObjectsOnClient();
    }

    [ClientRpc]
    private void ReportToClient(Platform platform, string buildingName)
    {
        if(!isServer)_inventory.GetSlot(buildingName).ApplyChanges();
        platform.SetNormalServer();
        if (hasAuthority) SetActiveOnClient(platform);
    }

    [ClientCallback]
    private void SetActiveOnClient(Platform platform) =>
        platform.SetNormal();
    
    
    public void ClearHashObjectsOnClient()
    {
        ClearHashObjects();
        if(isServer) ClearHashCmd();
    }

    [ClientRpc]
    private void ClearHashCmd()
    {
        if(!isServer) ClearHashObjects();
    }

    private void ClearHashObjects()
    {
        _slot?.ReturnBuilding();
        if(_placingCoroutine != null) StopCoroutine(_placingCoroutine);
        if (_platform == null) return;
        Destroy(_platform.gameObject);
        _platform = null;
        _buildingName = null;
    }
}