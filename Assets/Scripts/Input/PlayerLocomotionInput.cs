using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-2)]
public class PlayerLocomotionInput : MonoBehaviour, PlayerControls.IPlayerLocomotionMapActions
{
	[SerializeField] private bool holdToSprint = true;

	public bool SprintToggledOn {  get; private set; }
    public PlayerControls PlayerControls {  get; private set; }
    public Vector2 MovementInput { get; private set; }
	public Vector2 LookInput { get; private set; }
	public bool JumpPressed { get; private set; }
	public bool IsWalking { get; private set; }

	private void OnEnable()
	{
		PlayerControls = new PlayerControls();
		PlayerControls.Enable();

		PlayerControls.PlayerLocomotionMap.Enable();
		PlayerControls.PlayerLocomotionMap.SetCallbacks(this);
	}

	private void OnDisable()
	{
		PlayerControls.PlayerLocomotionMap.Disable();
		PlayerControls.PlayerLocomotionMap.RemoveCallbacks(this);
	}

	private void LateUpdate()
	{
		JumpPressed = false;
	}

	public void OnMovement(InputAction.CallbackContext context)
	{
		MovementInput = context.ReadValue<Vector2>(); 
	}

	public void OnLook(InputAction.CallbackContext context)
	{
		LookInput = context.ReadValue<Vector2>();
		//print(LookInput);
	}

	public void OnToggleSprint(InputAction.CallbackContext context)
	{
		if(context.performed)
		{
			SprintToggledOn = holdToSprint || !SprintToggledOn;
		}
		else if(context.canceled) 
		{
			SprintToggledOn = !holdToSprint && SprintToggledOn;
		}
	}

	public void OnJump(InputAction.CallbackContext context)
	{
		if (!context.performed)
			return;
		JumpPressed = true;
	}
}
