using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public Transform cameraTransform;

	public float speed;
	public float rotateSpeed;
	public float rotateLerp;

	private Vector2 rotation;

	private void FixedUpdate()
	{
		// Turning
#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		if (Input.GetKeyUp(KeyCode.LeftShift))
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
		if (Input.GetKey(KeyCode.LeftShift))
		{
#endif
			rotation.x -= Input.GetAxis("Mouse Y") * rotateSpeed;
			rotation.y += Input.GetAxis("Mouse X") * rotateSpeed;

			rotation.x = Mathf.Clamp(rotation.x, -90, 90);

			if (rotation.y > 360)
				rotation.y = 0;
			if (rotation.y < 0)
				rotation.y = 360;
#if UNITY_EDITOR
		}
#endif

		cameraTransform.localRotation = Quaternion.Slerp(cameraTransform.localRotation, Quaternion.Euler(new Vector3 { x = rotation.x }), rotateLerp);
		transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(new Vector3 { y = rotation.y }), rotateLerp);

		// Movement
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");

		transform.position += transform.forward * vertical * speed + transform.right * horizontal * speed;
	}
}