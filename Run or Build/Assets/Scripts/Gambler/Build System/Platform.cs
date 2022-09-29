using System;
using Mirror;
using UnityEngine;

[Serializable]
public class Platform : NetworkBehaviour
{
    private SpriteRenderer _mainRenderer;
    
    private bool _canDestroy;
    private Color _color;

    private void Awake() =>
        _mainRenderer = GetComponentInChildren<SpriteRenderer>();

    private void Start() => _color = _mainRenderer.color;

    public void SetTransparent(bool available)
    {
        _color.a = available ? 1 : 0.5f;
        _mainRenderer.color = _color;
    }

    public void SetNormalServer() => GetComponent<BoxCollider2D>().isTrigger = false;

    public void SetNormal()
    {
        GetComponent<BoxCollider2D>().isTrigger = false;
        _canDestroy = true;
    } 
    
    private void OnMouseOver()
    {
        if (!Input.GetMouseButtonDown(1)) return;
        if (!_canDestroy) return;
        DestroySelf();
    }

    [Command(requiresAuthority = false)]
    private void DestroySelf() => NetworkServer.Destroy(gameObject);
}