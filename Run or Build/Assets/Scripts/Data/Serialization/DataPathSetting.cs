using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "Path", menuName = "Data/DataPathSetting")]
public class DataPathSetting : ScriptableObject
{
    [SerializeField] private string _name;

    public string DataPath => Path.Combine(Application.dataPath, _name);
}