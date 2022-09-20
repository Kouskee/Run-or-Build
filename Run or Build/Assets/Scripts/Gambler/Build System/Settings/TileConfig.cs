using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Tile", menuName = "Gambler/Tile")]
public class TileConfig : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private int _count;
    [SerializeField] private TileBase _tile;

    public string Name => _name;
    public int Count => _count;
    public TileBase Tile => _tile;
}
