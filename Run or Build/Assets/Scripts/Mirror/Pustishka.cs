using Mirror;
using UnityEngine;

public class Pustishka : NetworkBehaviour
{
    private int _count;

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.X)) return; // TODO заменить на алтарь
        if (isServer)
            ChangeRoles();
    }

    private bool _isRunner;
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