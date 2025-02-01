using UnityEngine;

public class InventorySwitch : MonoBehaviour
{
	bool flag = false;
	public GameObject InventoryObj;

	private void Start()
	{
		LockCursor(true);
	}

	public void LockCursor(bool _lock)
	{
		if (_lock)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		else
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
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
			InventoryObj.SetActive(true);
		}
		else
		{
			LockCursor(true);
			InventoryObj.SetActive(false);
		}
	}
}
    