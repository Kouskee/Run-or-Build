using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class Altar : NetworkBehaviour
{
    [SerializeField] private MediatorBuilding _mediator;
    [SerializeField] private DictionarySlots[] _slotsDictionary;
    
    private Dictionary<Button, BuildingConfig> _slots;

    private bool _isRunner;

    public void Init()
    {
        _slots = new Dictionary<Button, BuildingConfig>(_slotsDictionary.Length);
        foreach (var slot in _slotsDictionary)
        {
            _slots.Add(slot.Button, slot.Config);
        }
        var inventory = new Inventory(_slots);
        _mediator.Init(inventory);
    }

    public void UseAltar()
    {
        InitInventory();
        ChangeRoles();
    }

    [ClientRpc]
    private void InitInventory()
    {
        var inventory = new Inventory(_slots);
        _mediator.Init(inventory);
    }

    private void Update()
    {
        if(!isServer) return;
        if (!Input.GetKeyDown(KeyCode.X)) return; // TODO заменить на алтарь
        UseAltar();
    }
    
    private void ChangeRoles()
    {
        Player playerF = null, playerS = null;
        var connections = NetworkServer.connections;
        foreach (var connection in connections)
        {
            var player = connection.Value.identity.GetComponent<Player>();
            if (playerF == null)
                playerF = player;
            else
                playerS = player;
        }

        _isRunner = !_isRunner;
        playerF.ChangeRole(_isRunner);
        playerS.ChangeRole(!_isRunner);
    }
}

[Serializable]
public class DictionarySlots
{
    public Button Button;
    public BuildingConfig Config;
}