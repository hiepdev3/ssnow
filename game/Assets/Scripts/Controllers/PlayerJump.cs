using UnityEngine;
using UnityEngine.Events;

public class PlayerJump : MonoBehaviour
{
   

    public FloatVariable JumpForce;

    public IntVariable RemainingJumps;

    public BoolVariable IsAlive;

    [Tooltip("Event invoked when player jumps.")]
    public UnityEvent PlayerJumpEvent;

    private Rigidbody2D rigidBody;

    private bool inputJump;

    private void Awake()
    {
        this.rigidBody = this.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && this.IsAlive.Value)
        {
            this.inputJump = true;
        }
    }
   
    private void FixedUpdate()
    {
        // Player jump
        if (this.inputJump && this.RemainingJumps.Value > 0)
        {
            this.Jump(this.JumpForce.Value);
            this.RemainingJumps.ApplyChange(-1);
            this.PlayerJumpEvent.Invoke();

            #if UNITY_EDITOR
                Debug.Log(string.Format("PlayerJump.Jump [JumpForce: {0}] [RemainingJumps: {1}]", this.JumpForce.Value, this.RemainingJumps.Value));
            #endif
        }

        // Cleanup
        this.inputJump = false;
    }

    /// <summary>
    /// Increase vertical velocity of the player.
    /// </summary>
    /// <param name="force"></param>
    private void Jump(float force)
    {
        this.rigidBody.linearVelocity = new Vector2(this.rigidBody.linearVelocity.x, (Vector2.up.y * force));
        this.inputJump = false;
    }
}
