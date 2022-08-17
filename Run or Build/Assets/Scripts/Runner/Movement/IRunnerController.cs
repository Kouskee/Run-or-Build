using UnityEngine;

public interface IRunnerController
{
    void Start();
    void Update(float deltaTime);
    void FixedUpdate(float deltaTime);
    void Move(Vector2 input);
    void Jump(float jumpForce);
    
    bool CanJump { get; set; }
    bool IsDash { get; set; }
}
