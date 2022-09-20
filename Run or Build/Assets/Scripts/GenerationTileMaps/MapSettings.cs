using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

///<summary>
///This script makes the MapLayer class a scriptable object
///To create a new MapLayer, right click in the project view and select Map Layer Settings
///You can then use this script with the LevelGenerator to generate your level

public enum Algorithm
{
    PerlinCave, RandomWalkCave, RandomWalkCaveCustom, CellularAutomataVonNeuman, CellularAutomataMoore, DirectionalTunnel
}

[CreateAssetMenu(fileName ="MapSettings", menuName = "Generation/Map Settings", order = 0)]
public class MapSettings : ScriptableObject
{
    public Algorithm algorithm;    
    public bool randomSeed;	
    public float seed;	
    public int fillAmount;
	public int smoothAmount;
    public int clearAmount;
    public int intervalToExit;
    public int minPathWidth, maxPathWidth, maxPathChange, roughness, windyness;
    public bool edgesAreWalls;
    public float modifier;
}

//Custom UI for our class
[CustomEditor(typeof(MapSettings))]
public class MapSettings_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        MapSettings mapLayer = (MapSettings)target;
		GUI.changed = false;
		EditorGUILayout.LabelField(mapLayer.name, EditorStyles.boldLabel);

		mapLayer.algorithm = (Algorithm)EditorGUILayout.EnumPopup(new GUIContent("Generation Method", "The generation method we want to use to generate the map"), mapLayer.algorithm);
		mapLayer.randomSeed = EditorGUILayout.Toggle("Random Seed", mapLayer.randomSeed);

		//Only appear if we have the random seed set to false
        if (!mapLayer.randomSeed)
        {
            mapLayer.seed = EditorGUILayout.FloatField("Seed", mapLayer.seed);
        }

		//Shows different options depending on what algorithm is selected
        switch (mapLayer.algorithm)
        {
            case Algorithm.PerlinCave:
                mapLayer.edgesAreWalls = EditorGUILayout.Toggle("Edges Are Walls", mapLayer.edgesAreWalls);
                mapLayer.modifier = EditorGUILayout.Slider("Modifier", mapLayer.modifier, 0.0001f, 1.0f);
                mapLayer.intervalToExit = EditorGUILayout.IntSlider("Interval Exit", mapLayer.intervalToExit, 0, 15);
                break;
            case Algorithm.RandomWalkCave:
                mapLayer.clearAmount = EditorGUILayout.IntSlider("Amount To Clear", mapLayer.clearAmount, 0, 100);
                break;
            case Algorithm.RandomWalkCaveCustom:
                mapLayer.clearAmount = EditorGUILayout.IntSlider("Amount To Clear", mapLayer.clearAmount, 0, 100);
                break;
            case Algorithm.CellularAutomataVonNeuman:
                mapLayer.edgesAreWalls = EditorGUILayout.Toggle("Edges Are Walls", mapLayer.edgesAreWalls);
                mapLayer.intervalToExit = EditorGUILayout.IntSlider("Interval Exit", mapLayer.intervalToExit, 0, 15);
                mapLayer.fillAmount = EditorGUILayout.IntSlider("Fill Percentage", mapLayer.fillAmount, 0, 100);
				mapLayer.smoothAmount = EditorGUILayout.IntSlider("Smooth Amount", mapLayer.smoothAmount, 0, 10);
				break;
            case Algorithm.CellularAutomataMoore:
                mapLayer.edgesAreWalls = EditorGUILayout.Toggle("Edges Are Walls", mapLayer.edgesAreWalls);
                mapLayer.intervalToExit = EditorGUILayout.IntSlider("Interval Exit", mapLayer.intervalToExit, 0, 15);
                mapLayer.fillAmount = EditorGUILayout.IntSlider("Fill Percentage", mapLayer.fillAmount, 0, 100);
                mapLayer.smoothAmount = EditorGUILayout.IntSlider("Smooth Amount", mapLayer.smoothAmount, 0, 10);
                break;
            case Algorithm.DirectionalTunnel:
                mapLayer.minPathWidth = EditorGUILayout.IntField("Minimum Path Width", mapLayer.minPathWidth);
                mapLayer.maxPathWidth = EditorGUILayout.IntField("Maximum Path Width", mapLayer.maxPathWidth);
                mapLayer.maxPathChange = EditorGUILayout.IntField("Maximum Path Change", mapLayer.maxPathChange);
                mapLayer.windyness = EditorGUILayout.IntSlider(new GUIContent("Windyness", "This is checked against a random number to determine if we can change the paths current x position"), mapLayer.windyness, 0, 100);
                mapLayer.roughness = EditorGUILayout.IntSlider(new GUIContent("Roughness", "This is checked against a random number to determine if we can change the width of the tunnel"), mapLayer.roughness, 0, 100);
                break;
        }

		EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
		AssetDatabase.SaveAssets();

		if(GUI.changed)
			EditorUtility.SetDirty(mapLayer);
    }
}
