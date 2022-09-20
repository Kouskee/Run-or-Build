using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "Gambler/Building")]
public class BuildingConfig : ScriptableObject
{
   [SerializeField] private string _name;
   [SerializeField] private int _cost;
   [SerializeField] private Building _building;

   public string Name => _name;
   public int Cost => _cost;
   public Building Building => _building;
}
