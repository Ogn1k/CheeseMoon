using Cinemachine;
using UnityEngine;

public class InventorySwitch : MonoBehaviour
{
	public bool flag = false;
	public GameObject inventoryObj;
	public CinemachineFreeLook camera;
	Transform cameraFollow = null;

	private void Start()
	{
		cameraFollow = camera.Follow;
		LockCursor(true);
	}

	public void LockCursor(bool _lock)
	{
		if (_lock)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;

			
			camera.Follow = cameraFollow;
			camera.LookAt = cameraFollow;
		}
		else
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;

			
			camera.Follow = null;
			camera.LookAt = null;

		}
	}

	public void FlagThing()
	{
		flag = !flag;
	}
	private void Update()
	{

		if (Input.GetKeyDown(KeyCode.Escape)) flag = !flag;

		if (flag)
		{
			LockCursor(false);
			inventoryObj.SetActive(true);
		}
		else
		{
			LockCursor(true);
			inventoryObj.SetActive(false);
		}
	}
}
    