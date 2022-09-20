using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnergyGambler
{
    private Image _energyImage;
    private TMP_Text _energyTxt;
    private int _energy;
    private int _cost;

    private readonly int _minEnergy = 0, _maxEnergy;

    public EnergyGambler(int maxEnergy)
    {
        _maxEnergy = maxEnergy;
        _energy = maxEnergy;
    }

    public void StealEnergy()
    {
        _energy = Mathf.Clamp(_energy - _cost, _minEnergy, _maxEnergy);
    }

    public bool CanBuild(int cost)
    {
        _cost = cost;
        return _energy >= cost;
    }
}