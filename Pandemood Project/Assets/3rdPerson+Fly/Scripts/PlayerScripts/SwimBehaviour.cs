using System;
using UnityEngine;
using UnityEngine.Serialization;

// FlyBehaviour inherits from GenericBehaviour. This class corresponds to the flying behaviour.
namespace Scripts.PlayerScripts
{
	public class SwimBehaviour : GenericBehaviour
	{
		//public string swimButton = "Fly";              // Default fly button.
		public float swimSpeed = 4.0f;
		public float verticalSwimSpeed = 4.0f;
		// Default flying speed.
		public float sprintFactor = 2.0f;             // How much sprinting affects fly speed.
		//public float flyMaxVerticalAngle = 60f;       // Angle to clamp camera vertical movement when flying.
		private int _swimBool;                          // Animator variable related to flying.
		private bool _swim = false;                     // Boolean to determine whether or not the player activated fly mode.
		public bool inWater;
		private CapsuleCollider _col;                  // Reference to the player capsulle collider.
		public float tempoInAcqua;
		private float _timeBreath;
		public float waterSurfacePosition = 0.0f;
		public Transform waterSurface;
		private AudioSource _swimmingAudio;
		private float _waterLevel;
		private float _charHead;
		public bool isUnderwater;
		public bool checkWater;
		private int _prevColDir;
		private float _t;
		private int _v;
		private int _swimSpeedMultiplier;
		private Animator _mAnimator;
		// Start is always called after any Awake functions.
		private void Start()
		{
			// Set up the references.
			_mAnimator = gameObject.GetComponent<Animator>();
			_swimSpeedMultiplier = Animator.StringToHash("SwimSpeed");
			_swimBool = Animator.StringToHash("Swim");
			_col = this.GetComponent<CapsuleCollider>();
			// Subscribe this behaviour on the manager.
			BehaviourManager.SubscribeBehaviour(this);
			_timeBreath = tempoInAcqua;
			inWater = false;
			_waterLevel = GameObject.Find("WaterLevel").transform.position.y;
			_charHead = GameObject.Find("CharHead").transform.position.y;
			_v = Animator.StringToHash("V");
			_mAnimator.SetFloat(_swimSpeedMultiplier, swimSpeed/2+0.5f);
		}

		// Update is used to set features regardless the active behaviour.
		private void Update()
		{
			//HsumV = Math.Abs(behaviourManager.GetH) + Math.Abs(behaviourManager.GetV);
			if (!checkWater) return;
			if ((_waterLevel + gameObject.transform.position.y) <= waterSurfacePosition)
			{//
				inWater = true;
			}
			if ((_waterLevel + gameObject.transform.position.y) >= waterSurfacePosition)
			{
				_prevColDir = _col.direction;
				_col.direction = 1;
				if (this.gameObject.GetComponent<BasicBehaviour>().IsGrounded())
				{
					inWater = false;
					checkWater = false;
				}
				else
					_col.direction = _prevColDir;
			}

			switch (inWater)
			{
				//Debug.Log(CharHead);
				case false:
					_swim = false;
					//behaviourManager.UnlockTempBehaviour(behaviourManager.GetDefaultBehaviour);

					// Obey gravity. It's the law!
					BehaviourManager.GetRigidBody.useGravity = !_swim;
					tempoInAcqua = _timeBreath;
					_col.direction = 1;
					BehaviourManager.UnregisterBehaviour(this.BehaviourCode);
					break;
				// Toggle fly by input, only if there is no overriding state or temporary transitions.
				case true when !BehaviourManager.IsOverriding() && !BehaviourManager.GetTempLockStatus(BehaviourManager.GetDefaultBehaviour):
				{
					_swim = true;

					// Force end jump transition.
					BehaviourManager.UnlockTempBehaviour(BehaviourManager.GetDefaultBehaviour);

					// Obey gravity. It's the law!
					if (inWater)
						BehaviourManager.GetRigidBody.useGravity = !_swim;

					// Player is swimming.
					if (_swim)
					{
						// Register this behaviour.
						BehaviourManager.RegisterBehaviour(this.BehaviourCode);
					}
					else
					{
						// Set collider direction to vertical.
						_col.direction = 1;
						// Set camera default offset.
						//behaviourManager.GetCamScript.ResetTargetOffsets();

						// Unregister this behaviour and set current behaviour to the default one.
						BehaviourManager.UnregisterBehaviour(this.BehaviourCode);
					}

					break;
				}
			}

			if (_swim && IsUnderwater() && tempoInAcqua > 0)
			{
				tempoInAcqua -= Time.deltaTime;
			}
			if (_swim && IsUnderwater() && tempoInAcqua <= 0)
			{
				_swim = false;
				//Game over?
				// Force end jump transition.
				BehaviourManager.UnlockTempBehaviour(BehaviourManager.GetDefaultBehaviour);

				// Obey gravity. It's the law!
				BehaviourManager.GetRigidBody.useGravity = !_swim;
				tempoInAcqua = _timeBreath;
				_col.direction = 1;
				BehaviourManager.UnregisterBehaviour(this.BehaviourCode);
			}
			// Assert this is the active behaviour
			_swim = _swim && BehaviourManager.IsCurrentBehaviour(this.BehaviourCode);

			// Set swim related variables on the Animator Controller.
			BehaviourManager.GetAnim.SetBool(_swimBool, _swim);
		}

		// This function is called when another behaviour overrides the current one.
		public override void OnOverride()
		{
			// Ensure the collider will return to vertical position when behaviour is overriden.
			_col.direction = 1;
		}

		// LocalFixedUpdate overrides the virtual function of the base class.
		public override void LocalFixedUpdate()
		{
			// Set camera limit angle related to fly mode.
			//behaviourManager.GetCamScript.SetMaxVerticalAngle(flyMaxVerticalAngle);

			// Call the swim manager.
			SwimManagement(BehaviourManager.GetH, BehaviourManager.GetV);
		}
		// Deal with the player movement when swimming.
		private void SwimManagement(float horizontal, float vertical)
		{
			// Add a force player's rigidbody according to the fly direction.
			var direction = Rotating(horizontal, vertical);
			BehaviourManager.GetRigidBody.AddForce((direction * (swimSpeed * 100 * (BehaviourManager.IsSprinting() ? sprintFactor : 1))), ForceMode.Acceleration);
		}

		// Rotate the player to match correct orientation, according to camera and key pressed.
		private Vector3 Rotating(float horizontal, float vertical)
		{
			var forward = BehaviourManager.playerCamera.TransformDirection(Vector3.forward);
			forward.y = 0.0f;
			forward = forward.normalized;
			// Calculate target direction based on camera forward and direction key.
			Vector3 right = new Vector3(forward.z, 0, -forward.x);
			// Calculate target direction based on camera forward and direction key.
			var targetDirection = forward * vertical + right * horizontal;
			if (_mAnimator.GetFloat(_v) > 0.1f && _waterLevel + gameObject.transform.position.y < waterSurfacePosition-0.1f)
			{
				targetDirection.y += verticalSwimSpeed;
			}
			else if (_mAnimator.GetFloat(_v) > 0.1f && Math.Abs(_waterLevel + gameObject.transform.position.y - (waterSurfacePosition-0.1f)) < 0.01)
			{
				targetDirection.y = waterSurfacePosition;
			}
			if (_mAnimator.GetFloat(_v) < -0.1f)
			{
				targetDirection.y -= verticalSwimSpeed;
			}
			// Rotate the player to the correct fly position.
			if (BehaviourManager.IsMoving())
			{
				//behaviourManager.GetAnim.SetFloat(direction, 0f);
				if (targetDirection != Vector3.zero && BehaviourManager.GetH!=0)
				{
					Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
					Quaternion newRotation = Quaternion.Slerp(BehaviourManager.GetRigidBody.rotation, targetRotation, BehaviourManager.turnSmoothing);

					BehaviourManager.GetRigidBody.MoveRotation(newRotation);
					BehaviourManager.SetLastDirection(targetDirection);
				}
			}

			// Player is swimming and idle?
			if (!(Mathf.Abs(horizontal) > 0.2 || Mathf.Abs(vertical) > 0.2))
			{
				// Rotate the player to stand position.
				BehaviourManager.Repositioning();
				// Set collider direction to vertical.
				_col.direction = 1;
			}

			else
			{
				// Set collider direction to horizontal.
				_col.direction = 2;
			}

			if (targetDirection == Vector3.zero)
			{
				//behaviourManager.GetAnim.SetFloat(direction, 0f, 0.3f, Time.deltaTime);
			}
			// Return the current swim direction.
			return targetDirection;
		}

		private bool IsUnderwater()
		{
			return isUnderwater = (_charHead + gameObject.transform.position.y) <= waterSurfacePosition;
		}


		private void OnTriggerStay(Collider other)
		{
			if (LayerMask.LayerToName(other.gameObject.layer) != "Water") return;
			waterSurface = other.transform;
			waterSurfacePosition = waterSurface.position.y;
			checkWater = true;


		}
	}
}
