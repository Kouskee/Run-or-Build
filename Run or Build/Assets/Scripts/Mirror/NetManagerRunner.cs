using Gambler.Build_System;
using UnityEngine;
using Mirror;

public class NetManagerRunner : NetworkManager
{
    [Space] 
    [SerializeField] private GameObject _runner;
    [SerializeField] private GameObject _gambler;
    [Space] [Header("Gambler")] 
    [SerializeField] private MediatorBuilding _mediator;
    [SerializeField] private Canvas _gamblerUI;
    [SerializeField] private BuildingConfig[] _buildings;
    [SerializeField] private TileConfig[] _tiles;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        var player = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    //выполняется перед добавлением клиента, поэтому заранее префабу задаются параметры
    public override void OnStartClient()
    {
        base.OnStartClient();
        
        playerPrefab.GetComponent<Player>().Init(_runner, _gambler, _gamblerUI);
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        var buildingFabric = new BuildingFabric(_buildings);
        var tileFabric = new TileFabric(_tiles);
        _mediator.Init(buildingFabric, tileFabric);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        var ownedObjects = new NetworkIdentity[conn.clientOwnedObjects.Count];
        conn.clientOwnedObjects.CopyTo(ownedObjects);
        foreach (var networkIdentity in ownedObjects)
        {
            if (!networkIdentity.GetComponent<Player>())
                networkIdentity.RemoveClientAuthority();
        }
        base.OnServerDisconnect(conn);
    }
}