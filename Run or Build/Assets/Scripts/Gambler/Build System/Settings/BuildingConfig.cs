using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum BuildingType
{
    Platform, Tile
}

[CreateAssetMenu(fileName = "Building", menuName = "Gambler/Building")]
public class BuildingConfig : ScriptableObject
{
    [EnumToggleButtons, SerializeField]
    private BuildingType _buildingType;
    
    [SerializeField] 
    private string _name;
    
    [BoxGroup("Settings"), SerializeField]
    private int _count;
    
    [ShowIf("_buildingType", BuildingType.Tile), BoxGroup("Settings"), SerializeField]
    private TileBase _tile;
    [ShowIf("_buildingType", BuildingType.Platform), BoxGroup("Settings"), SerializeField]
    private Platform _platform;

    public string Name => _name;
    public int Count => _count;
    public TileBase Tile => _tile;
    public Platform Platform => _platform;
}
