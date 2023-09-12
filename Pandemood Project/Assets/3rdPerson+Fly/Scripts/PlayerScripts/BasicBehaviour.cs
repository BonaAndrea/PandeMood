using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.PlayerScripts
{
	// This class manages which player behaviour is active or overriding, and call its local functions.
// Contains basic setup and common functions used by all the player behaviours.
	[RequireComponent(typeof(Animator))]
	public sealed class BasicBehaviour : MonoBehaviour
	{
		public Transform playerCamera;                        // Reference to the camera that focus the player.
		public float turnSmoothing = 0.06f;                   // Speed of turn when moving to match camera facing.
		private int _currentBehaviour;                         // Reference to the current player behaviour.
		private int _behaviourLocked;                          // Reference to temporary locked behaviour that forbids override.
		private Vector3 _lastDirection;                        // Last direction the player was moving.
		private ThirdPersonOrbitCamBasic _camScript;           // Reference to the third person camera script.
		private readonly bool _sprint;                                  // Boolean to determine whether or not the player activated the sprint mode.
		private readonly bool _crouch;
		private bool _isWalkingOnZ = true;
		private bool _changedFOV;                              // Boolean to store when the sprint action has changed de camera FOV.
		private int _hFloat;                                   // Animator variable related to Horizontal Axis.
		private int _vFloat;                                   // Animator variable related to Vertical Axis.
		private List<GenericBehaviour> _behaviours;            // The list containing all the enabled player behaviours.
		private List<GenericBehaviour> _overridingBehaviours;  // List of current overriding behaviours.
		private Rigidbody _rBody;                              // Reference to the player's rigidbody.
		private int _groundedBool;                             // Animator variable related to whether or not the player is on the ground.
		private Vector3 _colExtents;                           // Collider extents for ground test. 
		public bool canVertical;
		private Collider _collider;
		private Rigidbody _rigidbody;

		public BasicBehaviour(bool sprint, bool crouch)
		{
			_sprint = sprint;
			_crouch = crouch;
		}

		// Get current horizontal and vertical axes.
		public float GetH { get; private set; }
		public void SetH(float hor){ GetH = hor; } 
		public float GetV { get; private set; }

		// Get the player camera script.
		public ThirdPersonOrbitCamBasic GetCamScript { get { return _camScript; } }

		// Get the player's rigid body.
		public Rigidbody GetRigidBody { get { return _rBody; } }

		// Get the player's animator controller.
		public Animator GetAnim { get; private set; }

		// Get current default behaviour.
		public int GetDefaultBehaviour { get; private set; }

		private void Awake ()
		{
			// Set up the references.
			gameObject.GetComponent<MoveBehaviour>();
			_collider = gameObject.GetComponent<Collider>();
			_behaviours = new List<GenericBehaviour> ();
			_overridingBehaviours = new List<GenericBehaviour>();
			GetAnim = GetComponent<Animator> ();
			_hFloat = Animator.StringToHash("H");
			_vFloat = Animator.StringToHash("V");
			_camScript = playerCamera.GetComponent<ThirdPersonOrbitCamBasic> ();
			_rBody = GetComponent<Rigidbody> ();

			// Grounded verification variables.
			_groundedBool = Animator.StringToHash("Grounded");
			_colExtents = _collider.bounds.extents;

			_rigidbody = GetComponent<Rigidbody>();

			//Event subscription
			ChangeDirection.OnDirectionChanged += TurnOf90Degrees;
		}

		void TurnOf90Degrees(String corner)
		{
			_isWalkingOnZ = !_isWalkingOnZ;
		}

		void Update()
		{
			// Store the input axes.
			GetH = Input.GetAxis("Horizontal");
			GetV = Input.GetAxis("Vertical");
			switch (canVertical)
			{
				case true:
					_rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
					break;
				case false:
                    if (_isWalkingOnZ)
						_rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
					else
						_rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
					break;
			}

			// Set the input axes on the Animator Controller.
			GetAnim.SetFloat(_hFloat, GetH, 0.1f, Time.deltaTime); 
			GetAnim.SetFloat(_vFloat, GetV, 0.1f, Time.deltaTime);

			// Toggle sprint by input.
			//crouch = Input.GetButton (crouchButton);

			// Set the correct camera FOV for sprint mode.
			/*if(IsSprinting())
		{
			changedFOV = true;
			camScript.SetFOV(sprintFOV);
		}
		else if(changedFOV)
		{
			camScript.ResetFOV();
			changedFOV = false;
		}*/
			// Set the grounded test on the Animator Controller.
			GetAnim.SetBool(_groundedBool, IsGrounded());
		}

		// Call the FixedUpdate functions of the active or overriding behaviours.
		private void FixedUpdate()
		{
			// Call the active behaviour if no other is overriding.
			var isAnyBehaviourActive = false;
			if (_behaviourLocked > 0 || _overridingBehaviours.Count == 0)
			{
				foreach (var behaviour in _behaviours.Where(behaviour => behaviour.isActiveAndEnabled && _currentBehaviour == behaviour.GetBehaviourCode()))
				{
					isAnyBehaviourActive = true;
					behaviour.LocalFixedUpdate();
				}
			}
			// Call the overriding behaviours if any.
			else
			{
				foreach (var behaviour in _overridingBehaviours)
				{
					behaviour.LocalFixedUpdate();
				}
			}

			// Ensure the player will stand on ground if no behaviour is active or overriding.
			if (isAnyBehaviourActive || _overridingBehaviours.Count != 0) return;
			_rBody.useGravity = true;
			Repositioning ();
		}

		// Call the LateUpdate functions of the active or overriding behaviours.
		private void LateUpdate()
		{
			// Call the active behaviour if no other is overriding.
			if (_behaviourLocked > 0 || _overridingBehaviours.Count == 0)
			{
				foreach (var behaviour in _behaviours.Where(behaviour => behaviour.isActiveAndEnabled && _currentBehaviour == behaviour.GetBehaviourCode()))
				{
					behaviour.LocalLateUpdate();
				}
			}
			// Call the overriding behaviours if any.
			else
			{
				foreach (var behaviour in _overridingBehaviours)
				{
					behaviour.LocalLateUpdate();
				}
			}

		}

		// Put a new behaviour on the behaviours watch list.
		public void SubscribeBehaviour(GenericBehaviour behaviour)
		{
			_behaviours.Add (behaviour);
		}

		// Set the default player behaviour.
		public void RegisterDefaultBehaviour(int behaviourCode)
		{
			GetDefaultBehaviour = behaviourCode;
			_currentBehaviour = behaviourCode;
		}

		// Attempt to set a custom behaviour as the active one.
		// Always changes from default behaviour to the passed one.
		public void RegisterBehaviour(int behaviourCode)
		{
			if (_currentBehaviour == GetDefaultBehaviour)
			{
				_currentBehaviour = behaviourCode;
			}
		}

		// Attempt to deactivate a player behaviour and return to the default one.
		public void UnregisterBehaviour(int behaviourCode)
		{
			if (_currentBehaviour == behaviourCode)
			{
				_currentBehaviour = GetDefaultBehaviour;
			}
		}

		// Attempt to override any active behaviour with the behaviours on queue.
		// Use to change to one or more behaviours that must overlap the active one (ex.: aim behaviour).
		public bool OverrideWithBehaviour(GenericBehaviour behaviour)
		{
			// Behaviour is not on queue.
			if (_overridingBehaviours.Contains(behaviour)) return false;
			// No behaviour is currently being overridden.
			if (_overridingBehaviours.Count == 0)
			{
				// Call OnOverride function of the active behaviour before overrides it.
				foreach (GenericBehaviour overriddenBehaviour in _behaviours)
				{
					if (overriddenBehaviour.isActiveAndEnabled && _currentBehaviour == overriddenBehaviour.GetBehaviourCode())
					{
						overriddenBehaviour.OnOverride();
						break;
					}
				}
			}
			// Add overriding behaviour to the queue.
			_overridingBehaviours.Add(behaviour);
			return true;
		}

		// Attempt to revoke the overriding behaviour and return to the active one.
		// Called when exiting the overriding behaviour (ex.: stopped aiming).
		public bool RevokeOverridingBehaviour(GenericBehaviour behaviour)
		{
			if (_overridingBehaviours.Contains(behaviour))
			{
				_overridingBehaviours.Remove(behaviour);
				return true;
			}
			return false;
		}

		// Check if any or a specific behaviour is currently overriding the active one.
		public bool IsOverriding(GenericBehaviour behaviour = null)
		{
			if (behaviour == null)
				return _overridingBehaviours.Count > 0;
			return _overridingBehaviours.Contains(behaviour);
		}

		// Check if the active behaviour is the passed one.
		public bool IsCurrentBehaviour(int behaviourCode)
		{
			return this._currentBehaviour == behaviourCode;
		}

		// Check if any other behaviour is temporary locked.
		public bool GetTempLockStatus(int behaviourCodeIgnoreSelf = 0)
		{
			return (_behaviourLocked != 0 && _behaviourLocked != behaviourCodeIgnoreSelf);
		}

		// Atempt to lock on a specific behaviour.
		//  No other behaviour can overrhide during the temporary lock.
		// Use for temporary transitions like jumping, entering/exiting aiming mode, etc.
		public void LockTempBehaviour(int behaviourCode)
		{
			if (_behaviourLocked == 0)
			{
				_behaviourLocked = behaviourCode;
			}
		}

		// Attempt to unlock the current locked behaviour.
		// Use after a temporary transition ends.
		public void UnlockTempBehaviour(int behaviourCode)
		{
			if(_behaviourLocked == behaviourCode)
			{
				_behaviourLocked = 0;
			}
		}

		// Common functions to any behaviour:

		// Check if player is sprinting.
		public bool IsSprinting()
		{
			return _sprint && IsMoving() && CanSprint();
		}
		public bool IsCrouching()
		{
			return _crouch && CanCrouch();
		}

		// Check if player can sprint (all behaviours must allow).
		private bool CanSprint()
		{
			return _behaviours.All(behaviour => behaviour.AllowSprint()) && _overridingBehaviours.All(behaviour => behaviour.AllowSprint());
		}

		private bool CanCrouch()
		{
			return _behaviours.All(behaviour => behaviour.AllowCrouch()) && _overridingBehaviours.All(behaviour => behaviour.AllowCrouch());
		}

		// Check if the player is moving on the horizontal plane.
		public bool IsHorizontalMoving()
		{
			return GetH != 0;
		}

		// Check if the player is moving.
		public bool IsMoving()
		{
			return (GetH != 0)|| (GetV != 0);
		}

		public bool IsMovingYAxis()
		{
			return _rBody.velocity.y != 0;
		}

		// Get the last player direction of facing.
		public Vector3 GetLastDirection()
		{
			return _lastDirection;
		}

		// Set the last player direction of facing.
		public void SetLastDirection(Vector3 direction)
		{
			_lastDirection = direction;
		}

		// Put the player on a standing up position based on last direction faced.
		public void Repositioning()
		{
			if (_lastDirection == Vector3.zero) return;
			_lastDirection.y = 0;
			var targetRotation = Quaternion.LookRotation (_lastDirection);
			var newRotation = Quaternion.Slerp(_rBody.rotation, targetRotation, turnSmoothing);
			_rBody.MoveRotation (newRotation);
		}

		// Function to tell whether or not the player is on ground.
		public bool IsGrounded()
		{
			var ray = new Ray(this.transform.position + Vector3.up * (2 * _colExtents.x), Vector3.down);
			return Physics.SphereCast(ray, _colExtents.x, _colExtents.x + 0.2f);
		}

		public float SetV(float vNew)
		{
			return GetV = vNew;
		}
		// ReSharper disable Unity.PerformanceAnalysis
		public void UpdateBounds()
		{
			Debug.Log("Updating Bounds...");
			_colExtents = _collider.bounds.extents;
		}
	}

// This is the base class for all player behaviours, any custom behaviour must inherit from this.
// Contains references to local components that may differ according to the behaviour itself.
	public abstract class GenericBehaviour : MonoBehaviour
	{
		//protected Animator anim;                       // Reference to the Animator component.
		protected int SpeedFloat;                      // Speed parameter on the Animator.
		protected BasicBehaviour BehaviourManager;     // Reference to the basic behaviour manager.
		protected int BehaviourCode;                   // The code that identifies a behaviour.
		protected bool CanSprint;                      // Boolean to store if the behaviour allows the player to sprint.
		private bool _canCrouch;

		private void Awake()
		{
			// Set up the references.
			BehaviourManager = GetComponent<BasicBehaviour> ();
			SpeedFloat = Animator.StringToHash("Speed");
			CanSprint = true;
			_canCrouch = true;

			// Set the behaviour code based on the inheriting class.
			BehaviourCode = this.GetType().GetHashCode();
		}

		// Protected, virtual functions can be overridden by inheriting classes.
		// The active behaviour will control the player actions with these functions:
	
		// The local equivalent for MonoBehaviour's FixedUpdate function.
		public virtual void LocalFixedUpdate() { }
		// The local equivalent for MonoBehaviour's LateUpdate function.
		public virtual void LocalLateUpdate() { }
		// This function is called when another behaviour overrides the current one.
		public virtual void OnOverride() { }

		// Get the behaviour code.
		public int GetBehaviourCode()
		{
			return BehaviourCode;
		}

		// Check if the behaviour allows sprinting.
		public bool AllowSprint()
		{
			return CanSprint;
		}
		public bool AllowCrouch()
		{
			return _canCrouch;
		}
	}
}