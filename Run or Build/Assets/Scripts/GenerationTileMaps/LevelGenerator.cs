using System.Collections.Generic;
using Data.Serialization;
using Mirror;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelGenerator : NetworkBehaviour
{
    [SerializeField] private DataPathSetting _path;
    [SerializeField] private Tilemap _tileMap;
    [SerializeField] private TileBase _tile;
    [SerializeField] private Vector2Int _startBoundaries;

    public int Width;
    public int Height;

    public List<MapSettings> MapSettings = new List<MapSettings>();

    private int[] _outputs;
    private int[,] _map;
    private Vector2Int _offset = new Vector2Int(200, 0);
    
    public override void OnStartServer()
    {
        base.OnStartServer();
        _outputs = new int[Height];
        _map = new int[Width, Height];
        GenerateMap();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
            ClearMap();
        if (Input.GetKeyDown(KeyCode.F))
            GenerateMap();
    }
    
    public void GenerateMap(int index = 0)
    {
        var outputsNewMap = new int[Height];
        
        float seed;
        if (MapSettings[index].randomSeed)
            seed = Random.Range(0f, 100f);
        else
            seed = MapSettings[index].seed.GetHashCode();
        
        switch (MapSettings[index].algorithm)
        {
            case Algorithm.PerlinCave:
                _map = MapFunctions.GenerateArray(Width, Height, true);
                _map = MapFunctions.PerlinNoiseCave(_map, MapSettings[index].intervalToExit, MapSettings[index].modifier,
                    MapSettings[index].edgesAreWalls, ref outputsNewMap);
                break;
            case Algorithm.CellularAutomataVonNeuman:
                _map = MapFunctions.GenerateCellularAutomata(Width, Height, seed, MapSettings[index].fillAmount,
                    MapSettings[index].edgesAreWalls);
                _map = MapFunctions.SmoothVNCellularAutomata(_map, MapSettings[index].intervalToExit, MapSettings[index].edgesAreWalls,
                    MapSettings[index].smoothAmount);
                break;
            case Algorithm.CellularAutomataMoore:
                _map = MapFunctions.GenerateCellularAutomata(Width, Height, seed, MapSettings[index].fillAmount,
                    MapSettings[index].edgesAreWalls);
                _map = MapFunctions.SmoothMooreCellularAutomata(_map, MapSettings[index].intervalToExit, MapSettings[index].edgesAreWalls,
                    MapSettings[index].smoothAmount, ref outputsNewMap);
                break;
            case Algorithm.DirectionalTunnel:
                _map = MapFunctions.GenerateArray(Width, Height, false);
                _map = MapFunctions.DirectionalTunnel(_map, MapSettings[index].minPathWidth, MapSettings[index].maxPathWidth,
                    MapSettings[index].maxPathChange, MapSettings[index].roughness, MapSettings[index].windyness);
                break;
        }

        _map = MapFunctions.ClearEnter(_map, _startBoundaries);

        if (index == 0)
            StartCoroutine(MapFunctions.RenderMapCoroutine(_map, _tileMap, _tile));
        else
        {
            StartCoroutine(MapFunctions.RenderMapWithOffset(_map, _tileMap, _tile, _offset, _outputs));
            _offset.x += Width;
        }

        _outputs = outputsNewMap;
        
        var mapList = new List<Vector3IntS>();

        for (int x = 0; x < _map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < _map.GetUpperBound(1); y++)
            {
                if(_map[x,y] != 0)
                    mapList.Add(new Vector3IntS(x, y, 0));
            }
        }
        
        var data = new MapData()
        {
            map = mapList
        };
        BinarySerializer.Serializer(_path.DataPath, data);
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
        _offset = Vector2Int.zero;
    }
}

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorStackEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //Create a reference to our script
        var levelGen = (LevelGenerator) target;

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