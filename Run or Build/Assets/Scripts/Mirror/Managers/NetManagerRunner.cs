using UnityEngine;
using Mirror;
using Mirror.Managers;

public class NetManagerRunner : NetworkManager
{
    [Space]
    [SerializeField] private MapNetManager _mapNetManager;
    [Space] 
    [SerializeField] private GameObject _runner;
    [SerializeField] private GameObject _gambler;
    [Space] [Header("Gambler")] 
    [SerializeField] private Altar _altar;
    [SerializeField] private Canvas _gamblerUI;

    private NetworkIdentity _ownedObject;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        var playerGo = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, playerGo);

        if (_ownedObject == null) return;
        if (playerGo.TryGetComponent(out Player player))
            player.ChangeRole(!_ownedObject.TryGetComponent(out GamblerController _));
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
        _altar.Init();
    }
    
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        _mapNetManager.SerializeData();
        
        var ownedObjects = new NetworkIdentity[conn.clientOwnedObjects.Count];
        conn.clientOwnedObjects.CopyTo(ownedObjects);
        foreach (var networkIdentity in ownedObjects)
        {
            if (networkIdentity.GetComponent<Player>()) continue;
            networkIdentity.RemoveClientAuthority();
            _ownedObject = networkIdentity;
        }
        base.OnServerDisconnect(conn);
    }
}