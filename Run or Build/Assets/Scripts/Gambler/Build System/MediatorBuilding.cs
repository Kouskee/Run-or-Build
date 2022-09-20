using Gambler.Build_System;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MediatorBuilding : MonoBehaviour
{
    [SerializeField] private Tilemap _tileMap;
    [SerializeField] private BuildingSystem _buildingSystem;

    private Button _button;
    private TMP_Text _text;
    private Camera _camera;
    private TileBase _tile;
    
    private TileFabric _tileFabric;
    private BuildingFabric _buildingFabric;

    public void Init(BuildingFabric buildingFabric, TileFabric tileFabric)
    {
        _buildingFabric = buildingFabric;
        _tileFabric = tileFabric;
        _buildingSystem.Init(buildingFabric, tileFabric);
    }
    
    private void Awake()
    {
        _camera = Camera.main;
    }
    
    public void CreateBuilding(Button button)
    {
        var config = _buildingFabric.GetBuilding(button.name);
        _buildingSystem.StartPlacingBuilding(config);
    }

    public void CreateTilesTest(Button button)
    {
        _text = button.GetComponentInChildren<TMP_Text>();
        var config = _tileFabric.GetTile(button.name);
        _text.text = config.Count.ToString();
        _tile = config.Tile;
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (_tile == null) return;

        var ray = _camera.ScreenPointToRay(Input.mousePosition);
        var plane = new Plane(Vector3.forward, Vector3.zero);
        plane.Raycast(ray, out var distance);
        var clickWorldPos = ray.GetPoint(distance);

        var clickCell = _tileMap.WorldToCell(clickWorldPos);
        
        if(_tileMap.GetTile(clickCell) != null) return; 
        
        _text.text = (int.Parse(_text.text) - 1).ToString();
        _tileMap.SetTile(clickCell, _tile);
    }
}
