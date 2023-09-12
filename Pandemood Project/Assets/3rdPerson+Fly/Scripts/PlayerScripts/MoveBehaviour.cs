using System;
using UnityEngine;

// MoveBehaviour inherits from GenericBehaviour. This class corresponds to basic walk and run behaviour, it is the default behaviour.
namespace Scripts.PlayerScripts
{
    public class MoveBehaviour : GenericBehaviour
    {
        public float walkSpeed = 0.15f; // Default walk speed.
        public float runSpeed = 1.0f; // Default run speed.
        public float sprintSpeed = 2.0f; // Default sprint speed.
        public float speedDampTime; // Default damp time to change the animations based on current speed.
        public string jumpButton = "Jump"; // Default jump button.
        public string shrinkButton = "Shrink";
        public float jumpHeight = 1.5f; // Default jump height.
        public float jumpInertialForce = 10f; // Default horizontal inertial force when jumping.
        public float crouchSpeed = 0.4f;
        private float _speed, _speedSeeker; // Moving speed.
        private int _jumpBool; // Animator variable related to jumping.
        private int _groundedBool; // Animator variable related to whether or not the player is on ground.
        private bool _jump; // Boolean to determine whether or not the player started a jump.
        [SerializeField] private bool isColliding; // Boolean to determine if the player has collided with an obstacle.
        private bool _canVertical;
        public float pushPower;
        public bool invertLadderAxis; //****inserito
        private CapsuleCollider _verticalCollider;
        private bool _isClimbing;
        public bool isShrink;
        private Animator _mAnimator;
        private Vector3 _initialPosition;
        private BoxCollider _topCheck;

        private float _sphereDistance;
        private static readonly int Transform1 = Animator.StringToHash("Transform");
        private static readonly int Climbing = Animator.StringToHash("Climbing");
        private static readonly int ClimbDirection = Animator.StringToHash("ClimbDirection");
        private static readonly int Push = Animator.StringToHash("Push");
        private static readonly int Swim = Animator.StringToHash("Swim");

        private Animator _animator;
        private ParticleSystem _particleSystem;
        private Rigidbody _rigidbody;
        private BasicBehaviour _basicBehaviour;
        private CapsuleCollider _capsuleCollider;
        private float _sphereHeight, _sphereRadius;
        public bool canShrink;
        public float climbSpeed;

        // Start is always called after any Awake functions.
        private void Start()
        {
            // Set up the references.
            Physics.IgnoreLayerCollision(8, 9);
            _initialPosition = gameObject.transform.position;
            _jumpBool = Animator.StringToHash("Jump");
            _groundedBool = Animator.StringToHash("Grounded");
            BehaviourManager.GetAnim.SetBool(_groundedBool, true);
            _mAnimator = gameObject.GetComponent<Animator>();
            // Subscribe and register this behaviour as the default behaviour.
            BehaviourManager.SubscribeBehaviour(this);
            BehaviourManager.RegisterDefaultBehaviour(this.BehaviourCode);
            _speedSeeker = runSpeed;
            _verticalCollider = GetComponent<CapsuleCollider>();
            _isClimbing = false;
            isShrink = false;
            _topCheck = null;
            _sphereHeight = _verticalCollider.height;
            _sphereRadius = _verticalCollider.radius;
            _animator = GetComponent<Animator>();
            _particleSystem = GetComponent<ParticleSystem>();
            _rigidbody = GetComponent<Rigidbody>();
            _basicBehaviour = GetComponent<BasicBehaviour>();
            _capsuleCollider = _verticalCollider;
        }

        // Update is used to set features regardless the active behaviour.
        private void Update()
        {
            _canVertical = BehaviourManager.canVertical;
            // Get jump input
            if (!_jump && Input.GetButtonDown(jumpButton) && BehaviourManager.IsCurrentBehaviour(this.BehaviourCode) &&
                !BehaviourManager.IsOverriding() && !_animator.GetBool("Push"))
            {
                _jump = true;
            }

            CheckPush();
            ClimbManager();
            if (canShrink)
                CheckShrink();
        }

        private void CheckShrink()
        {
            var originalScale = new Vector3(1, 1, 1);
            var shrinkScale = new Vector3(0.3f, 0.3f, 0.3f);
            if (_mAnimator.GetBool(Transform1))
                if (_mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Transforming"))
                {
                    _mAnimator.SetBool(Transform1, false);
                    isShrink = !isShrink;
                }

            if (Input.GetButtonDown(shrinkButton) && !_isClimbing)
            {
                var res = new Collider[1];
                if (!isShrink || (isShrink &&
                                  Physics.OverlapCapsuleNonAlloc(
                                      transform.position + new Vector3(0, _sphereRadius + 0.02f, 0),
                                      transform.position - _verticalCollider.center + new Vector3(0, _sphereHeight, 0), _sphereRadius, res,
                                      ~LayerMask.GetMask("Player")) == 0))
                {
                    if (!_mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Transforming"))
                    {
                        _mAnimator.SetBool(Transform1, true);
                        _particleSystem.Play();
                    }
                }
            }

            if (!_mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Transforming")) return;
            switch (isShrink)
            {
                case false:
                    {
                        transform.localScale = Vector3.Lerp(transform.localScale, originalScale, 2.5f * Time.deltaTime);
                        if (!(Vector3.Distance(transform.localScale, originalScale) < 0.01f)) return;
                        transform.localScale = originalScale;
                        _particleSystem.Stop();
                        _basicBehaviour.UpdateBounds();
                        break;
                    }
                case true:
                    {
                        transform.localScale = Vector3.Lerp(transform.localScale, shrinkScale, 2.5f * Time.deltaTime);
                        if (!(Vector3.Distance(transform.localScale, shrinkScale) < 0.01f)) return;
                        transform.localScale = shrinkScale;
                        _particleSystem.Stop();
                        _basicBehaviour.UpdateBounds();
                        break;
                    }
            }
        }

        private void ClimbManager()
        {
            var player = gameObject;
            var mRigidbody = _rigidbody;
            var playerRotation = player.transform.rotation.eulerAngles;
            var v = Input.GetAxis("Vertical");
            if (_isClimbing)
            {
                _verticalCollider.radius = 0.29f;
                _basicBehaviour.enabled = false;
                //player.GetComponent<ThirdPersonCharacter>().enabled = false;
                mRigidbody.useGravity = false;
                mRigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX |
                                         RigidbodyConstraints.FreezePositionZ;
                playerRotation.y = 0;
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation,
                    Quaternion.Euler(playerRotation), BehaviourManager.turnSmoothing * 3);
                if (v != 0)
                {
                    var top = _topCheck == null;
                    if (top || player.transform.position.y + 0.9f <
                        _topCheck.bounds.center.y + _topCheck.size.y)
                    {
                        player.transform.Translate(Vector3.up * (v * climbSpeed * _speed * Time.deltaTime));
                        _animator.SetBool(Climbing, true);
                        _animator.speed = 1f;
                        _animator.SetFloat(ClimbDirection, 1f);
                    }
                    //else
                    //anim.speed = 0f;
                }

                if (Math.Abs(v - (-1)) < 0.1f)
                {
                    //if (bottomCheck != null)
                    //Debug.Log(bottomCheck.transform.position.y + (bottomCheck.bounds.center.y - bottomCheck.bounds.extents.y));
                    //if ((bottomCheck == null) || (bottomCheck != null && (player.transform.position.y - 0.1f) > (bottomCheck.bounds.center.y - bottomCheck.size.y / 2)))
                    //{
                    player.transform.Translate(Vector3.up * (v * climbSpeed / 2 * _speed * Time.deltaTime));
                    _animator.SetBool(Climbing, true);
                    _animator.speed = 1f;
                    _animator.SetFloat(ClimbDirection, -1f);
                    //}
                    //else
                    //_animator.speed = 0f;
                }

                if (v == 0)
                {
                    _animator.speed = 0f;
                }
                //Lascio commentato in caso si voglia implementare, è per fare saltare mentre si sta arrampicando
                /*if (Input.GetButton(jumpButton))
            {
                player.GetComponent<BasicBehaviour>().enabled = true;
                //player.GetComponent<ThirdPersonCharacter>().enabled = true;
                m_Rigidbody.useGravity = true;
                if(canVertical)
                m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                else if (!canVertical)
                {
                    m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionZ;
                }
                _animator.SetBool("Climbing", false);
                //_animator.SetBool("ClimbDown", false);
                //transform.Translate(Vector3.back * 0.1f);
            }*/
            }

            else
            {
                //player.GetComponent<ThirdPersonUserControl>().enabled = true;
                _basicBehaviour.enabled = true;
                _animator.SetBool(Climbing, false);
                mRigidbody.useGravity = true;
                switch (_canVertical)
                {
                    case true:
                        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationY |
                                                 RigidbodyConstraints.FreezeRotationX |
                                                 RigidbodyConstraints.FreezeRotationZ;
                        break;
                    case false:
                        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX |
                                                 RigidbodyConstraints.FreezePositionZ |
                                                 RigidbodyConstraints.FreezeRotationY |
                                                 RigidbodyConstraints.FreezeRotationZ;
                        break;
                }

                //m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                _animator.speed = 1f;
                //_animator.SetBool("ClimbDown", false);
            }
        }

        private void CheckPush()
        {
            if (!_animator.GetBool(Push)) return;
            _verticalCollider.radius = 0.53f;
            if (_basicBehaviour.IsMoving()) return;
            _animator.SetBool(Push, false);
            _verticalCollider.radius = 0.53f;
        }

        // LocalFixedUpdate overrides the virtual function of the base class.
        public override void LocalFixedUpdate()
        {
            // Call the basic movement manager.
            MovementManagement(BehaviourManager.GetH, BehaviourManager.GetV);

            // Call the jump manager.
            JumpManagement();
        }

        // Execute the idle and walk/run jump movements.
        private void JumpManagement()
        {
            // Start a new jump.
            if (_jump && !BehaviourManager.GetAnim.GetBool(_jumpBool) && BehaviourManager.IsGrounded())
            {
                // Set jump related parameters.
                BehaviourManager.LockTempBehaviour(this.BehaviourCode);
                BehaviourManager.GetAnim.SetBool(_jumpBool, true);
                _verticalCollider.radius = 0.23f;

                // Is a locomotion jump?
                if (!(BehaviourManager.GetAnim.GetFloat(SpeedFloat) >= 0)) return;
                // Temporarily change player friction to pass through obstacles.
                var material = _capsuleCollider.material;
                material.dynamicFriction = 0f;
                material.staticFriction = 0f;
                // Remove vertical velocity to avoid "super jumps" on slope ends.
                RemoveVerticalVelocity();
                // Set jump vertical impulse velocity.
                var velocity = 2f * Mathf.Abs(Physics.gravity.y) * jumpHeight;
                velocity = Mathf.Sqrt(velocity);
                if (!isShrink)
                    BehaviourManager.GetRigidBody.AddForce(Vector3.up * velocity, ForceMode.VelocityChange);
                else
                    BehaviourManager.GetRigidBody.AddForce(Vector3.up * velocity / 1.3f, ForceMode.VelocityChange);
            }
            // Is already jumping?
            else if (BehaviourManager.GetAnim.GetBool(_jumpBool))
            {
                // Keep forward movement while in the air.
                if (!BehaviourManager.IsGrounded() && !isColliding && BehaviourManager.GetTempLockStatus() &&
                    BehaviourManager.GetAnim.GetFloat(SpeedFloat) > 0)
                {
                    if (!isShrink)
                        BehaviourManager.GetRigidBody.AddForce(
                            transform.forward * (jumpInertialForce * Physics.gravity.magnitude * sprintSpeed),
                            ForceMode.Acceleration);
                    else
                        BehaviourManager.GetRigidBody.AddForce(
                            transform.forward * (jumpInertialForce * Physics.gravity.magnitude * sprintSpeed) / 2,
                            ForceMode.Acceleration);
                }

                // Has landed?
                if ((!(BehaviourManager.GetRigidBody.velocity.y < 0)) || !BehaviourManager.IsGrounded()) return;
                BehaviourManager.GetAnim.SetBool(_groundedBool, true);
                // Change back player friction to default.
                var material = _capsuleCollider.material;
                material.dynamicFriction = 0.6f;
                material.staticFriction = 0.6f;
                // Set jump related parameters.
                _jump = false;
                BehaviourManager.GetAnim.SetBool(_jumpBool, false);
                BehaviourManager.UnlockTempBehaviour(this.BehaviourCode);
                _verticalCollider.radius = 0.53f;
            }
        }

        // Deal with the basic player movement
        private void MovementManagement(float horizontal, float vertical)
        {
            if (!_canVertical)
                vertical = 0;
            // On ground, obey gravity.
            if (BehaviourManager.IsGrounded())
                BehaviourManager.GetRigidBody.useGravity = true;

            // Avoid takeoff when reached a slope end.
            else if (!BehaviourManager.GetAnim.GetBool(_jumpBool) && BehaviourManager.GetRigidBody.velocity.y > 0)
            {
                RemoveVerticalVelocity();
            }

            // Call function that deals with player orientation.
            Rotating(horizontal, vertical);

            // Set proper speed.
            Vector2 dir = new Vector2(horizontal, vertical);
            _speed = Vector2.ClampMagnitude(dir, 1f).magnitude;
            // This is for PC only, gamepads control speed via analog stick.
            _speedSeeker += Input.GetAxis("Mouse ScrollWheel");
            _speedSeeker = Mathf.Clamp(_speedSeeker, walkSpeed, runSpeed);
            _speed *= _speedSeeker;
            if (BehaviourManager.IsSprinting())
            {
                _speed = sprintSpeed;
            }

            if (BehaviourManager.IsCrouching())
            {
                _speed = crouchSpeed;
            }

            BehaviourManager.GetAnim.SetFloat(SpeedFloat, _speed, speedDampTime, Time.deltaTime);
        }

        // Remove vertical rigidbody velocity.
        private void RemoveVerticalVelocity()
        {
            Vector3 horizontalVelocity = BehaviourManager.GetRigidBody.velocity;
            horizontalVelocity.y = 0;
            BehaviourManager.GetRigidBody.velocity = horizontalVelocity;
        }

        // Rotate the player to match correct orientation, according to camera and key pressed.
        private void Rotating(float horizontal, float vertical)
        {
            //FIXARE PROBLEMA ROTAZIONE VERSO LA TELECAMERA
            // Get camera forward direction, without vertical component.
            var forward = BehaviourManager.playerCamera.TransformDirection(Vector3.forward);

            // Player is moving on ground, Y component of camera facing is not relevant.
            forward.y = 0.0f;
            forward = forward.normalized;

            // Calculate target direction based on camera forward and direction key.
            var right = new Vector3(forward.z, 0, -forward.x);
            if (_canVertical == false || _mAnimator.GetBool(Swim))
                vertical = 0;
            var targetDirection = forward * vertical + right * horizontal;


            // Lerp current direction to calculated target direction.
            if ((BehaviourManager.IsMoving() && targetDirection != Vector3.zero))
            {
                var targetRotation = Quaternion.LookRotation(targetDirection);

                var newRotation = Quaternion.Slerp(BehaviourManager.GetRigidBody.rotation, targetRotation,
                    BehaviourManager.turnSmoothing * 3);
                BehaviourManager.GetRigidBody.MoveRotation(newRotation);
                BehaviourManager.SetLastDirection(targetDirection);
            }

            // If idle, Ignore current camera facing and consider last moving direction.
            if (!(Mathf.Abs(horizontal) > 0.9 || Mathf.Abs(vertical) > 0.9))
            {
                BehaviourManager.Repositioning();
            }
        }

        // Collision detection.
        private void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ladder")) return;
            isColliding = true;
            // Slide on vertical obstacles
            if (BehaviourManager.IsCurrentBehaviour(this.GetBehaviourCode()) &&
                collision.GetContact(0).normal.y <= 0.1f)
            {
                var material = _capsuleCollider.material;
                material.dynamicFriction = 0f;
                material.staticFriction = 0f;
            }

            if (!collision.gameObject.CompareTag("Pushable") ||
                !_animator.GetBool(Push)) return;
            if (!_mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Pushing")) return;
            switch (_canVertical)
            {
                case true:
                    collision.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                    break;
                case false:
                    collision.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation |
                        RigidbodyConstraints.FreezePositionZ;
                    break;
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            isColliding = false;
            var material = _capsuleCollider.material;
            material.dynamicFriction = 0.6f;
            material.staticFriction = 0.6f;
            if (collision.gameObject.CompareTag("Pushable"))
            {
                collision.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ |
                                                                             RigidbodyConstraints.FreezePositionX |
                                                                             RigidbodyConstraints.FreezeRotation;
            }
        }

        public void Jump()
        {
            this._jump = true;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (!other.gameObject.CompareTag("Pushable")) return;
            if (other.transform.position.y > gameObject.transform.position.y)
                this.gameObject.GetComponent<Animator>().SetBool(Push, true);
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            var hitRigidbody = hit.collider.attachedRigidbody;
            if (hit.moveDirection.y < -0.3f)
                return;
            if (hitRigidbody != null && hitRigidbody.isKinematic == false &&
                this.gameObject.GetComponent<Animator>().GetBool(Push))
            {
                //Regola la forza di spinta in base alla massa del giocatore
                hitRigidbody.AddForce(hit.moveDirection * (_speed * 8 / hitRigidbody.mass), ForceMode.VelocityChange);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            var choose = 0;
            if (!other.gameObject.CompareTag("Ladder") || _mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jumping") || _mAnimator.GetCurrentAnimatorStateInfo(0).IsName("Falling")) return;
            if (other.gameObject.name == "Top")
                _topCheck = other.gameObject.GetComponent<BoxCollider>();
            if (other.gameObject.name == "Bottom")
                other.gameObject.GetComponent<BoxCollider>();
            if ((_isClimbing && Input.GetAxis("Horizontal") != 0))
            {
                _isClimbing = false;
                var playerPos = gameObject.transform.position;
                if (Input.GetAxis("Horizontal") == 0)
                {
                    choose = UnityEngine.Random.Range(1, 2);
                }

                if (Input.GetAxis("Horizontal") > 0 || choose == 1)
                {
                    //*** inserito
                    if (!invertLadderAxis)
                        playerPos.x += 1f;
                    else
                        playerPos.x -= 1f;
                    //****
                }

                if (Input.GetAxis("Horizontal") < 0 || choose == 2)
                {
                    //*** inserito
                    if (!invertLadderAxis)
                        playerPos.x -= 1f;
                    else
                        playerPos.x += 1f;
                    //****
                }

                gameObject.transform.position =
                    Vector3.Lerp(gameObject.transform.position, playerPos, Time.deltaTime * 10);
                RestoreZAxis();
            }

            if (Input.GetAxis("Vertical") > 0 && other.gameObject.name == "Bottom")
            {
                if (!_isClimbing)
                {
                    //Gira il giocatore verso la scala
                    var posizioneScala = other.transform.position;
                    posizioneScala.y = gameObject.transform.position.y;
                    posizioneScala.z -= 0.40f;
                    transform.position = posizioneScala;
                    var nuovaRotazione = Vector3.zero;
                    transform.rotation = Quaternion.Slerp(BehaviourManager.GetRigidBody.rotation,
                        Quaternion.Euler(nuovaRotazione),
                        BehaviourManager.turnSmoothing); // Quaternion.Euler(nuovaRotazione);
                    //Quaternion newRotation = Quaternion.Slerp(behaviourManager.GetRigidBody.rotation, targetRotation, behaviourManager.turnSmoothing);
                }

                _isClimbing = true;
            }

            if (!(Input.GetAxis("Vertical") < 0) || other.gameObject.name != "Top") return;
            {
                if (!_isClimbing)
                {
                    //Gira il giocatore verso la scala
                    var posizioneScala = other.transform.position;
                    var o = gameObject;
                    posizioneScala.y = o.transform.position.y;
                    posizioneScala.z -= 0.40f;
                    o.transform.position = posizioneScala;
                    var nuovaRotazione = Vector3.zero;
                    //gameObject.transform.rotation = Quaternion.Euler(nuovaRotazione);
                    gameObject.transform.rotation = Quaternion.Slerp(BehaviourManager.GetRigidBody.rotation,
                        Quaternion.Euler(nuovaRotazione), BehaviourManager.turnSmoothing);
                }

                _isClimbing = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Ladder") && other.name == "Top")
            {
                _topCheck = null;
            }

            if (other.gameObject.CompareTag("Ladder") && other.name == "Bottom")
            {
            }
        }

        private void RestoreZAxis()
        {
            var o = gameObject;
            var posizioneAttuale = o.transform.position;
            posizioneAttuale.z = _initialPosition.z;
            o.transform.position = posizioneAttuale;
        }
    }
}