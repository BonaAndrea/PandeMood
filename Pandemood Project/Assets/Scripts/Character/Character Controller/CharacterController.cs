using UnityEngine;
using UnityEngine.Events;

public class CharacterController : MonoBehaviour
{

    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement

    private Vector3 m_Velocity = Vector3.zero;

    private Rigidbody _Rigidbody;

    private Animator animator;

    public float turnSmoothtime = 0.1f;
    public float runSpeed = 40f;
    public float jumpStrength = 5f;
    public bool isRunningOnZ = true;
    public string jumpButton = "Jump";

    float horizontalMove = 0f;
    float turnSmoothVelocity;

    private void Awake()
    {
        _Rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("HorizontalSpeed", h);
        Debug.Log(h);
        horizontalMove = h * runSpeed;
        float velocityY = _Rigidbody.velocity.y;
        if (Input.GetButtonDown(jumpButton) && velocityY <= 0f + 0.001f && velocityY >= 0f - 0.001f)
        {
            _Rigidbody.AddForce(transform.up * jumpStrength);
        }
        Vector3 direction = new Vector3(horizontalMove, 0f, 0f).normalized;
        if(direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothtime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
    }

    private void FixedUpdate()
    {
        Move(horizontalMove);
    }

    public void Move(float move)
    {
        Vector3 targetVelocity;
        targetVelocity = new Vector3(move * 10f, _Rigidbody.velocity.y, _Rigidbody.velocity.z);
        _Rigidbody.velocity = Vector3.SmoothDamp(_Rigidbody.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
    }

}