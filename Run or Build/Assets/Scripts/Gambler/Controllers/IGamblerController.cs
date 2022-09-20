using UnityEngine;

public interface IGamblerController
{
    void Start();
    void Update(float deltaTime);
    void FixedUpdate();
    void Move(Vector2 axis);
}