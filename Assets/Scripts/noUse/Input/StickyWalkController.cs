using UnityEngine;

public class StickyWalkController : MonoBehaviour
{
	public PlayerController playerController;
	public CharacterController characterController;

	public float moveSpeed = 5f;
	public float rotationSpeed = 10f;
	public float jumpForce = 5f;
	public Transform cameraTransform;

	private Rigidbody rb;
	private Vector3 currentUp;
	private bool isGrounded;
	Vector3 targetUp;

	public float movingThreshold = 0.01f;
	private bool _jumpedLastFrame = false;

	public PlayerState _playerState;
	public PlayerLocomotionInput _playerLocomotionInput;
	private PlayerMovementState _lastMovementState = PlayerMovementState.Falling;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		currentUp = Vector3.up;
	}

	void Update()
	{
		UpdateMovementState();
		HandleMovement();
		HandleJump();
		AlignToSurface();
	}

	private void HandleMovement()
	{
		// Get player input
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");

		// Calculate movement direction relative to the current surface
		Vector3 forward = Vector3.Cross(cameraTransform.right, currentUp).normalized;
		Vector3 right = Vector3.Cross(currentUp, forward).normalized;
		Vector3 moveDirection = (forward * vertical + right * horizontal).normalized;

		// Move the character
		rb.linearVelocity = moveDirection * moveSpeed + Vector3.Project(rb.linearVelocity, currentUp);
	}

	public void UpdateMovementState()
	{
		_lastMovementState = _playerState.CurrentPlayerMovementState;

		bool isMovementInput = _playerLocomotionInput.MovementInput != Vector2.zero;
		bool isMovingLaterally = IsMovingLaterally();
		bool isSprinting = _playerLocomotionInput.SprintToggledOn && isMovingLaterally;

		PlayerMovementState lateralState = isSprinting ? PlayerMovementState.Sprinting :
			isMovingLaterally || isMovementInput ? PlayerMovementState.Running : PlayerMovementState.Idling;

		if (!(_playerState.CurrentPlayerMovementState == PlayerMovementState.Liquid))
		{


			_playerState.SetPlayerMovementState(lateralState);

			if ((!isGrounded || _jumpedLastFrame) && rb.linearVelocity.y >= 0f)
			{
				_playerState.SetPlayerMovementState(PlayerMovementState.Jumping);
				_jumpedLastFrame = false;
				//_characterController.stepOffset = 0f;
			}
			else if ((!isGrounded || _jumpedLastFrame) && rb.linearVelocity.y <= 0f)
			{
				_playerState.SetPlayerMovementState(PlayerMovementState.Falling);
				_jumpedLastFrame = false;
				//_characterController.stepOffset = 0f;
			}
			
				//_characterController.stepOffset = _stepOffset;
		}
	}

	private bool IsMovingLaterally()
	{
		Vector3 lateralVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

		return lateralVelocity.magnitude > movingThreshold;
	}

	private void HandleJump()
	{
		if (Input.GetButtonDown("Jump") && isGrounded)
		{
			rb.AddForce(currentUp * jumpForce, ForceMode.Impulse);
			_jumpedLastFrame = true;
		}
			
	}

	private void AlignToSurface()
	{
		// Raycast to detect the surface below the character
		RaycastHit hit;
		Debug.DrawRay(transform.position, transform.position - currentUp * 10);
		if (Physics.Raycast(transform.position, -currentUp, out hit, 1.5f))
		{
			//playerController.stickyWalk = true;
			//characterController.enabled = false;
			isGrounded = true;

			//print(hit.normal + "awd");
			targetUp = hit.normal;
			Physics.gravity = targetUp * -9.8f;

			// Smoothly rotate the character to align with the surface normal
			currentUp = Vector3.Lerp(currentUp, targetUp, Time.deltaTime * rotationSpeed);
			Quaternion targetRotation = Quaternion.FromToRotation(transform.up, currentUp) * transform.rotation;
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
		}
		else
		{

			Physics.gravity = Vector3.up * -9.8f;
			isGrounded = false;

			targetUp = Vector3.up;

			currentUp = Vector3.Lerp(currentUp, targetUp, Time.deltaTime * rotationSpeed);
			Quaternion targetRotation = Quaternion.FromToRotation(transform.up, currentUp) * transform.rotation;
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

			//playerController.stickyWalk = false;
			//characterController.enabled = true;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		// Handle transitions between surfaces
		foreach (ContactPoint contact in collision.contacts)
		{
			currentUp = contact.normal;
			break;
		}
	}
}

