using UnityEngine;

public interface IRunnerController
{
    void Start();
    void Update(float deltaTime);
    void FixedUpdate(float deltaTime);
    void Jump(float jumpForce);
    void Move(Vector2 axis);
    
    bool CanJump { set; }
}
