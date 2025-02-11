using Unity.Mathematics;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class PlayerController : MonoBehaviour
{
	#region Class vars
	[SerializeField] private CharacterController _characterController;
	[SerializeField] private Camera _playerCamera;
	[SerializeField] private Transform _dummyCamera;

	[Header("Base movement")]
	public float runAcceleration = 35f;
	public float runSpeed = 4f;
	public float sprintAcceleration = 50f;
	public float sprintSpeed = 7f;
	public float airAcceleration = 25f;
	public float drag = 20f;
	public float movingThreshold = 0.01f;
	public float gravity = 25f;
	public float jumpSpeed = 1.0f;
	public float turnSmoothTime = 0.1f;
	public float rotationSpeed = 10f;
	public Vector3 velocity = Vector3.zero;

	[Header("Camera settings")]
	public float lookSenseH = 0.1f;
	public float lookSenseV = 0.1f;
	public float lookLimitV = 89f;

	[Header("Environment")]
	[SerializeField] private LayerMask _groundLayers;

	private PlayerLocomotionInput _playerLocomotionInput;
	private PlayerState _playerState;
	private Vector2 _cameraRotation = Vector2.zero;
	private Vector2 _playerTargetRotation = Vector2.zero;

	private bool _jumpedLastFrame = false;
	private float _verticalVelocity = 0f;
	private float _antiBump;
	private float _stepOffset;

	private PlayerMovementState _lastMovementState = PlayerMovementState.Falling;

	private float turnSmoothVelocity;
	#endregion

	#region Startup
	private void Awake()
	{
		_playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
		_playerState = GetComponent<PlayerState>();

		_antiBump = sprintSpeed;
		_stepOffset = _characterController.stepOffset;
	}
	#endregion

	#region Update logic
	private void Update()
	{
		UpdateMovementState();
		HandleVerticalMovement();
		AutoJump();
		HandleLateralMovement();
		//AutoJump();
	}

	private void UpdateMovementState()
	{
		_lastMovementState = _playerState.CurrentPlayerMovementState;

		bool isMovementInput = _playerLocomotionInput.MovementInput != Vector2.zero;
		bool isMovingLaterally = IsMovingLaterally();
		bool isSprinting = _playerLocomotionInput.SprintToggledOn && isMovingLaterally;
		bool isGrounded = IsGrounded();

		PlayerMovementState lateralState = isSprinting ? PlayerMovementState.Sprinting :
			isMovingLaterally || isMovementInput ? PlayerMovementState.Running : PlayerMovementState.Idling;

		if (!(_playerState.CurrentPlayerMovementState == PlayerMovementState.Liquid))
		{


			_playerState.SetPlayerMovementState(lateralState);

			if ((!isGrounded || _jumpedLastFrame) && _characterController.velocity.y >= 0f)
			{
				_playerState.SetPlayerMovementState(PlayerMovementState.Jumping);
				_jumpedLastFrame = false;
				_characterController.stepOffset = 0f;
			}
			else if ((!isGrounded || _jumpedLastFrame) && _characterController.velocity.y <= 0f)
			{
				_playerState.SetPlayerMovementState(PlayerMovementState.Falling);
				_jumpedLastFrame = false;
				_characterController.stepOffset = 0f;
			}
			else
				_characterController.stepOffset = _stepOffset;
		}
	}

	private void HandleVerticalMovement()
	{
		bool isGrounded = _playerState.InGroundedState();

		_verticalVelocity -= gravity * Time.deltaTime;

		if (isGrounded && _verticalVelocity < 0)
			_verticalVelocity = -_antiBump;

		if (_playerLocomotionInput.JumpPressed && isGrounded)
		{
			_verticalVelocity += Mathf.Sqrt(jumpSpeed * 3 * gravity);
			_jumpedLastFrame = true;
		}

		RaycastHit hit;
		if(Physics.Raycast(transform.position, Vector3.down, out hit, 1))
		{
			Debug.DrawLine(transform.position, hit.point, Color.cyan);
			if (hit.collider.gameObject.layer == 4 && !_jumpedLastFrame)
			{
				isGrounded = true;
				_verticalVelocity += 1;
				_jumpedLastFrame = true;
			}
		}
			

		if (_playerState.IsStateGroundedState(_lastMovementState))
		{
			_verticalVelocity += _antiBump;
		}
	}

	public void AutoJump()
	{
		
	}

	private void HandleLateralMovement()
	{
		bool isSprinting = _playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;
		bool isGrounded = _playerState.InGroundedState();

		float lateralAcceleration = !isGrounded ? airAcceleration :
			isSprinting ? sprintAcceleration : runAcceleration;

		float clampLateralMagnitude = !isGrounded ? sprintSpeed :
			isSprinting ? sprintSpeed : runSpeed; 

		Vector3 cameraForwardXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
		Vector3 cameraRightXZ = new Vector3(_playerCamera.transform.right.x, 0f, _playerCamera.transform.right.z).normalized;
		Vector3 movementDirection = cameraRightXZ * _playerLocomotionInput.MovementInput.x + cameraForwardXZ * _playerLocomotionInput.MovementInput.y;

		Vector3 movementDelta = movementDirection * runAcceleration * Time.deltaTime;
		Vector3 newVelocity = _characterController.velocity + movementDelta;

		Vector3 currentDrag = newVelocity.normalized * drag * Time.deltaTime;
		newVelocity = (newVelocity.magnitude > drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
		newVelocity = Vector3.ClampMagnitude(new Vector3(newVelocity.x, 0f, newVelocity.z), clampLateralMagnitude);
		newVelocity.y += _verticalVelocity;
		newVelocity = !isGrounded ? HandleStepWalls(newVelocity) : newVelocity;


		velocity = _characterController.velocity;

		_characterController.Move(newVelocity * Time.deltaTime);

		
	}

	private Vector3 HandleStepWalls(Vector3 velocity)
	{
		Vector3 normal = CharacterControllerUtils.GetNormalWithSphereCast(_characterController, _groundLayers);
		float angle = Vector3.Angle(normal, Vector3.up);
		bool validAngle = angle <= _characterController.slopeLimit;

		if (!validAngle && _verticalVelocity < 0f)
			velocity = Vector3.ProjectOnPlane(velocity, normal);

		return velocity;
	}

	#endregion

	#region lateUpdate
	private void LateUpdate()
	{
		
		if (transform.GetComponent<InventorySwitch>().flag)
		{
			return;
		}
		_cameraRotation.x += lookSenseH * _playerLocomotionInput.LookInput.x;
		_cameraRotation.y = Mathf.Clamp(_cameraRotation.y - lookSenseV * _playerLocomotionInput.LookInput.y, -lookLimitV, lookLimitV);

		
		float horizontalInput = _playerLocomotionInput.MovementInput.x;
		float verticalInput = _playerLocomotionInput.MovementInput.y;
		//Vector3 inputDir = transform.forward * verticalInput + transform.right * horizontalInput; //if you search for a camera problem - here it is :)

		_dummyCamera.transform.rotation *= Quaternion.AngleAxis(_playerLocomotionInput.LookInput.x * lookSenseH, Vector3.up);

		_dummyCamera.transform.rotation *= Quaternion.AngleAxis(_playerLocomotionInput.LookInput.y * lookSenseH, Vector3.right);

		var angles = _dummyCamera.transform.localEulerAngles;
		angles.z = 0;

		var angle = _dummyCamera.transform.localEulerAngles.x;

		//Clamp the Up/Down rotation
		if (angle > 180 && angle < 340)
		{
			angles.x = 340;
		}
		else if (angle < 180 && angle > 40)
		{
			angles.x = 40;
		}

		_dummyCamera.transform.localEulerAngles = angles;

		Vector3 targetDirection = Vector3.zero;
		targetDirection = _playerCamera.transform.forward * verticalInput;
		targetDirection = targetDirection + _playerCamera.transform.right * horizontalInput;
		targetDirection.Normalize();
		targetDirection.y = 0;

		if(targetDirection == Vector3.zero)
			targetDirection = transform.forward;

		Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
		Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

		transform.rotation = playerRotation;
	}

	private bool IsGrounded()
	{
		if(_playerState.CurrentPlayerMovementState == PlayerMovementState.Liquid)
			return true;
		bool grounded = _playerState.InGroundedState() ? IsGroundedWhileGrounded() : IsGroundedWhileAirborne();
		return grounded;
	}

	private bool IsGroundedWhileGrounded()
	{
		Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - _characterController.radius, transform.position.z);

		bool grounded = Physics.CheckSphere(spherePosition, _characterController.radius, _groundLayers, QueryTriggerInteraction.Ignore);
		return grounded;
	}

	private bool IsGroundedWhileAirborne()
	{
		Vector3 normal = CharacterControllerUtils.GetNormalWithSphereCast(_characterController, _groundLayers);
		float angle = Vector3.Angle(normal, Vector3.up);
		bool validAngle = angle <= _characterController.slopeLimit;

		return _characterController.isGrounded && validAngle;
	}
	#endregion

	#region State checks
	private bool IsMovingLaterally()
	{
		Vector3 lateralVelocity = new Vector3(_characterController.velocity.x, 0f, _characterController.velocity.z);

		return lateralVelocity.magnitude > movingThreshold;
	}
	#endregion
}
