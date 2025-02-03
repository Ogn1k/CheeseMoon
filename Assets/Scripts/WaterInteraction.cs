using UnityEditor.UIElements;
using UnityEngine;

public class WaterInteraction : MonoBehaviour
{
    [SerializeField] private string _playerTag = "Player";
	public float bounce = 20;

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		print(hit.gameObject);
		if (hit.gameObject.tag == _playerTag)
		{
			hit.gameObject.GetComponent<CharacterController>().Move(bounce * Vector3.up * Time.deltaTime);

		}
		
	}
	private void OnCollisionEnter(Collision collision)
	{
		print(collision.gameObject);
	}
}
