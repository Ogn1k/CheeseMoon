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
	public float drag = 20f;
	public float movingThreshold = 0.01f;
	public float gravity = 25f;
	public float jumpSpeed = 1.0f;
	public float turnSmoothTime = 0.1f;

	[Header("Camera settings")]
	public float lookSenseH = 0.1f;
	public float lookSenseV = 0.1f;
	public float lookLimitV = 89f;

	private PlayerLocomotionInput _playerLocomotionInput;
	private PlayerState _playerState;
	private Vector2 _cameraRotation = Vector2.zero;
	private Vector2 _playerTargetRotation = Vector2.zero;

	private float _verticalVelocity = 0f;
	private float turnSmoothVelocity;
	#endregion

	#region Startup
	private void Awake()
	{
		_playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
		_playerState = GetComponent<PlayerState>();
	}
	#endregion

	#region Update logic
	private void Update()
	{
		UpdateMovementState();
		HandleVerticalMovement();
		HandleLateralMovement();
	}

	private void UpdateMovementState()
	{
		bool isMovementInput = _playerLocomotionInput.MovementInput != Vector2.zero;
		bool isMovingLaterally = IsMovingLaterally();
		bool isSprinting = _playerLocomotionInput.SprintToggledOn && isMovingLaterally;
		bool isGrounded = IsGrounded();

		PlayerMovementState lateralState = isSprinting ? PlayerMovementState.Sprinting : 
			isMovingLaterally || isMovementInput ? PlayerMovementState.Running : PlayerMovementState.Idling;
		_playerState.SetPlayerMovementState(lateralState);

		if (!isGrounded && _characterController.velocity.y >= 0f)
		{
			_playerState.SetPlayerMovementState(PlayerMovementState.Jumping);
		}
		else if (!isGrounded && _characterController.velocity.y <= 0f) 
		{
			_playerState.SetPlayerMovementState(PlayerMovementState.Falling);
		}

	}

	private void HandleVerticalMovement()
	{
		bool isGrounded = _playerState.InGroundedState();

		if (isGrounded && _verticalVelocity < 0)
			_verticalVelocity = 0f;

		_verticalVelocity -= gravity * Time.deltaTime;

		if(_playerLocomotionInput.JumpPressed && isGrounded)
		{
			_verticalVelocity += Mathf.Sqrt(jumpSpeed * 3 * gravity);
		}
	}

	private void HandleLateralMovement()
	{
		bool isSprinting = _playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;
		bool isGrounded = _playerState.InGroundedState();

		float lateralAcceleration = isSprinting ? sprintAcceleration : runAcceleration;
		float clampLateralMagnitude = isSprinting ? sprintSpeed : runSpeed; 

		Vector3 cameraForwardXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
		Vector3 cameraRightXZ = new Vector3(_playerCamera.transform.right.x, 0f, _playerCamera.transform.right.z).normalized;
		Vector3 movementDirection = cameraRightXZ * _playerLocomotionInput.MovementInput.x + cameraForwardXZ * _playerLocomotionInput.MovementInput.y;

		Vector3 movementDelta = movementDirection * runAcceleration * Time.deltaTime;
		Vector3 newVelocity = _characterController.velocity + movementDelta;

		Vector3 currentDrag = newVelocity.normalized * drag * Time.deltaTime;
		newVelocity = (newVelocity.magnitude > drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
		newVelocity = Vector3.ClampMagnitude(newVelocity, clampLateralMagnitude);
		newVelocity.y += _verticalVelocity;

		_characterController.Move(newVelocity * Time.deltaTime);

		
	}
	#endregion

	#region lateUpdate
	private void LateUpdate()
	{
		_cameraRotation.x += lookSenseH * _playerLocomotionInput.LookInput.x;
		_cameraRotation.y = Mathf.Clamp(_cameraRotation.y - lookSenseV * _playerLocomotionInput.LookInput.y, -lookLimitV, lookLimitV);

		_playerTargetRotation.x += transform.eulerAngles.x + lookSenseH * _playerLocomotionInput.LookInput.x;
		//transform.rotation = Quaternion.Euler(0f, _playerTargetRotation.x, 0f);

		//_playerCamera.transform.rotation = Quaternion.Euler(_cameraRotation.y, _cameraRotation.x, 0f); 

		float rotationSpeed = 0.5f;
		float horizontalInput = _playerLocomotionInput.MovementInput.x;
		float verticalInput = _playerLocomotionInput.MovementInput.y;
		Vector3 inputDir = transform.forward * verticalInput + transform.right * horizontalInput; //if you search for a camera problem - here it is :)

		if (inputDir != Vector3.zero)
			transform.forward = Vector3.Slerp(transform.forward, inputDir.normalized, Time.deltaTime * lookSenseH);

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

		float targetAngle = Mathf.Atan2(_playerLocomotionInput.MovementInput.x, _playerLocomotionInput.MovementInput.y) * Mathf.Rad2Deg + _playerCamera.transform.rotation.eulerAngles.y;
		float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

		//Set the player rotation based on the look transform
		transform.rotation = Quaternion.Euler(0, smoothAngle, 0);
			//Quaternion.Euler(0, _dummyCamera.transform.rotation.eulerAngles.y, 0);
			//reset the y rotation of the look transform
		_dummyCamera.transform.localEulerAngles = new Vector3(angles.x, 0, 0);

		
	}

	private bool IsGrounded()
	{
		return _characterController.isGrounded;
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
