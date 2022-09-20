using System.Collections;
using UnityEngine;

public class RunnerMovementChecker : MonoBehaviour
{
    [SerializeField] private Transform _runner;
    [SerializeField] private LevelGeneratorStack _generator;
    [SerializeField] private int _frequencyCheckExit;
    
    [Header("Spawn Points")]
    [SerializeField] private int _frequencyCreatePoint;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _distance;

    private Vector3 _spawnPoint;
    private int _positionXToSpawn;
    public int _positionYToRespawn;
    
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
            if (_runner.position.x >= _positionXToSpawn)
            {
                _generator.GenerateMap(1);
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