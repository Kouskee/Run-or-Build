using System.Collections;
using Mirror;
using UnityEngine;

public class RunnerChecker : NetworkBehaviour
{
    [SerializeField] private Altar _altar;
    [SerializeField] private Transform _runner;
    [SerializeField] private LevelGenerator _generator;
    [SerializeField] private float _frequencyCheckExit;
    
    [Header("Spawn Points")]
    [SerializeField] private int _frequencyCreatePoint;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _distance;

    private Vector3 _spawnPoint;
    private int _positionXToSpawn;
    private int _positionYToRespawn;
    
    private int _count;
    
    private readonly RaycastHit2D[] _groundHit = new RaycastHit2D[1];
    
    private void Start()
    {
        _positionXToSpawn = _generator.Width;
        _positionYToRespawn = _generator.Height / 2;
        
        StartCoroutine(CheckForExitFromBorders());
        StartCoroutine(CreateSpawnPoint());
    }

    private IEnumerator CheckForExitFromBorders()
    {
        while (true)
        {
            if (_runner.position.y <= -_positionYToRespawn || _runner.position.y >= _positionYToRespawn)
                _runner.position = _spawnPoint;
            if (_runner.position.x >= _positionXToSpawn - 10)
            {
                if (isServer)
                {
                    _generator.GenerateMap(++_count);
                    _altar.UseAltar();
                }
                _positionXToSpawn += _positionXToSpawn;
            }
            yield return new WaitForSeconds(_frequencyCheckExit);
        }
    }

    private IEnumerator CreateSpawnPoint()
    {
        while (true)
        {
            var ground = Physics2D.RaycastNonAlloc(_runner.position, Vector2.down, _groundHit, _distance, _groundLayer);
            if (ground > 0)
                _spawnPoint = _runner.position;
            yield return new WaitForSeconds(_frequencyCreatePoint);
        }
    }

    private void FixedUpdate()
    {
        transform.position = _runner.position;
    }
}