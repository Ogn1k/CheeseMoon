using UnityEditor.UIElements;
using UnityEngine;

public class WaterInteraction : MonoBehaviour
{
    [SerializeField] private string _playerTag = "Player";
	public float bounce = 1;
	public float _verticalVelocity = 0f;
	public float gravity = 25f;

	private void Update()
	{
		
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag == _playerTag)
		{
			//other.gameObject.GetComponent<PlayerState>().SetPlayerMovementState(PlayerMovementState.Liquid);
			//other.gameObject.GetComponent<PlayerState>().SetPlayerMovementState(PlayerMovementState.Idling);
			//_verticalVelocity += Mathf.Sqrt(bounce * 3 * gravity)* Time.deltaTime;
			//print(_verticalVelocity);
			//CharacterController playCon = other.gameObject.GetComponent<CharacterController>();

			//playCon.Move(new Vector3(Mathf.Clamp(playCon.velocity.x, -0.5f, 0.5f), _verticalVelocity * Time.deltaTime, Mathf.Clamp(playCon.velocity.z, -0.5f, 0.5f)));
			//playCon.Move(new Vector3(0f, _verticalVelocity * Time.deltaTime * Time.deltaTime, 0f));
		}
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == _playerTag)
		{
			other.gameObject.GetComponent<PlayerState>().SetPlayerMovementState(PlayerMovementState.Liquid);
			other.gameObject.GetComponent<PlayerController>().AutoJump();
			_verticalVelocity = 0f;
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == _playerTag)
		{
			other.gameObject.GetComponent<PlayerState>().SetPlayerMovementState(PlayerMovementState.Idling);
		}
	}
}
