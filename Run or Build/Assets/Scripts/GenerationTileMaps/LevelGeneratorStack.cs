using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class LevelGeneratorStack : MonoBehaviour
{
    [SerializeField] private Tilemap _tileMap;
    [SerializeField] private TileBase _tile;
    [SerializeField] private Vector2Int _startBoundaries;

    public int Width;
    public int Height;

    public List<MapSettings> MapSettings = new List<MapSettings>();

    private int[] _outputs;
    private int[,] _map;
    private Vector2Int _offset = new Vector2Int(200, 0);

    private void Start()
    {
        GenerateMap();
        _outputs = new int[Height];
        _map = new int[Width, Height];
    }

    public void GenerateMap(int index = 0)
    {
        var outputsNewMap = new int[Height];

        float seed;
        if (MapSettings[index].randomSeed)
        {
            seed = Time.time.GetHashCode();
        }
        else
        {
            seed = MapSettings[index].seed.GetHashCode();
        }

        //Generate the map depending on the algorithm selected
        switch (MapSettings[index].algorithm)
        {
            case Algorithm.PerlinCave:
                //First generate our array
                _map = MapFunctions.GenerateArray(Width, Height, true);
                //Next generate the perlin noise onto the array
                _map = MapFunctions.PerlinNoiseCave(_map, MapSettings[index].intervalToExit, MapSettings[index].modifier,
                    MapSettings[index].edgesAreWalls);
                break;
            case Algorithm.RandomWalkCave:
                //First generate our array
                _map = MapFunctions.GenerateArray(Width, Height, false);
                //Next generate the random walk cave
                _map = MapFunctions.RandomWalkCave(_map, seed, MapSettings[index].clearAmount);
                break;
            case Algorithm.RandomWalkCaveCustom:
                //First generate our array
                _map = MapFunctions.GenerateArray(Width, Height, false);
                //Next generate the custom random walk cave
                _map = MapFunctions.RandomWalkCaveCustom(_map, seed, MapSettings[index].clearAmount);
                break;
            case Algorithm.CellularAutomataVonNeuman:
                //First generate the cellular automata array
                _map = MapFunctions.GenerateCellularAutomata(Width, Height, seed, MapSettings[index].fillAmount,
                    MapSettings[index].edgesAreWalls);
                //Next smooth out the array using the von neumann rules
                _map = MapFunctions.SmoothVNCellularAutomata(_map, MapSettings[index].intervalToExit, MapSettings[index].edgesAreWalls,
                    MapSettings[index].smoothAmount);
                break;
            case Algorithm.CellularAutomataMoore:
                //First generate the cellular automata array
                _map = MapFunctions.GenerateCellularAutomata(Width, Height, seed, MapSettings[index].fillAmount,
                    MapSettings[index].edgesAreWalls);
                //Next smooth out the array using the Moore rules
                _map = MapFunctions.SmoothMooreCellularAutomata(_map, MapSettings[index].intervalToExit, MapSettings[index].edgesAreWalls,
                    MapSettings[index].smoothAmount, ref outputsNewMap);
                break;
            case Algorithm.DirectionalTunnel:
                //First generate our array
                _map = MapFunctions.GenerateArray(Width, Height, false);
                //Next generate the tunnel through the array
                _map = MapFunctions.DirectionalTunnel(_map, MapSettings[index].minPathWidth, MapSettings[index].maxPathWidth,
                    MapSettings[index].maxPathChange, MapSettings[index].roughness, MapSettings[index].windyness);
                break;
        }

        if (index == 0)
        {
            MapFunctions.RenderMap(_map, _tileMap, _tile, _startBoundaries);
            _outputs = outputsNewMap;
        }
        else
        {
            MapFunctions.RenderMapWithOffset(_map, _tileMap, _tile, _offset, _outputs);
            _outputs = outputsNewMap;
            _offset.x += Width;
        }
    }

    public void ClearMap()
    {
        for (int x = 0; x < _map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < _map.GetUpperBound(1); y++)
            {
                _tileMap.SetTile(new Vector3Int(x, y, 0), null);
            }
        }
        _tileMap.CompressBounds();
        _offset = Vector2Int.zero;
    }
}

[CustomEditor(typeof(LevelGeneratorStack))]
public class LevelGeneratorStackEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //Create a reference to our script
        var levelGen = (LevelGeneratorStack) target;

        //List of editors to only show if we have elements in the map settings list
        var mapEditors = new List<Editor>();

        for (int i = 0; i < levelGen.MapSettings.Count; i++)
        {
            if (levelGen.MapSettings[i] == null) continue;

            var mapLayerEditor = CreateEditor(levelGen.MapSettings[i]);
            mapEditors.Add(mapLayerEditor);
        }

        if (mapEditors.Count <= 0) return;

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        for (int i = 0; i < mapEditors.Count; i++)
        {
            mapEditors[i].OnInspectorGUI();
        }

        if (GUILayout.Button("Generate"))
        {
            levelGen.GenerateMap();
        }

        if (GUILayout.Button("Clear"))
        {
            levelGen.ClearMap();
        }
    }
}