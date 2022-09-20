using UnityEngine;

[CreateAssetMenu(fileName = "GamblerConfig", menuName = "Characters/GamblerConfig")]
    public class GamblerConfig : ScriptableObject
    {
        [Header("Move")] 
        [SerializeField, Range(1, 20)] private float _speed;
        [SerializeField, Range(1, 10)] private float _drag;
        [SerializeField, Range(1, 10)] private int _acceleration;
        
        public float Speed => _speed;
        public float Drag => _drag;
        public float Acceleration => _acceleration;
    }
