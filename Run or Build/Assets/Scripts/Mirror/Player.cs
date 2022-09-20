using Cinemachine;
using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private GameObject _runner;
    [SerializeField] private GameObject _gambler;
    [Space]
    [SerializeField]private Canvas _gamblerUi;
    
    private NetworkIdentity _identity;
    private CinemachineVirtualCamera _cinemachine;

    public void Init(GameObject runner, GameObject gambler, Canvas gamblerUi)
    {
        _runner = runner;
        _gambler = gambler;
        _gamblerUi = gamblerUi;
    }

    private void Awake()
    {
        TryGetComponent(out _identity);

        var camera = Camera.main;
        var brain = camera.GetComponent<CinemachineBrain>();
        _cinemachine = (CinemachineVirtualCamera) brain.ActiveVirtualCamera;
    }

    public void ChangeRole(bool isRunner)
    {
        SwapRoles(isRunner ? _runner : _gambler);
        ChangeCamera(isRunner);
    }

    [ClientRpc]
    private void ChangeCamera(bool isRunner)
    {
        if (!isLocalPlayer) return;
        _cinemachine.Follow = isRunner ? _runner.transform : _gambler.transform;
        _gamblerUi.enabled = !isRunner;
    }
    
    private void SwapRoles(GameObject character)
    {
        var identity = character.GetComponent<NetworkIdentity>();
        RemoveClientAuthority(identity);
        identity.AssignClientAuthority(_identity.connectionToClient);
    }
    
    private void RemoveClientAuthority(NetworkIdentity identity)
    {
        if(identity.connectionToClient != null)
            identity.RemoveClientAuthority();
    }
}
