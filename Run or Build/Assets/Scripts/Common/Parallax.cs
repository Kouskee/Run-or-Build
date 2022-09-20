using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private Transform _followingTarget;
    [SerializeField, Range(0, 1)] private float _parallaxStrength;
    [SerializeField] private bool _disableVerticalParallax;

    private Vector3 _targetPreviousPosition;

    private void Start()
    {
        if (!_followingTarget)
            _followingTarget = Camera.main.transform;

        _targetPreviousPosition = _followingTarget.position;
    }
    
    private void FixedUpdate()
    {
        var delta = _followingTarget.position - _targetPreviousPosition;

        if (_disableVerticalParallax)
        {
            transform.localPosition = new Vector3(transform.position.x, _followingTarget.position.y, 0);
            delta.y = 0;
        }

        _targetPreviousPosition = _followingTarget.position;
        transform.position += delta * _parallaxStrength;
    }
}