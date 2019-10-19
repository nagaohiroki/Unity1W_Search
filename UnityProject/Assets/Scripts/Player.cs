using UnityEngine;
public class Player : MonoBehaviour
{
	[SerializeField]
	float mMoveSpeed = 0.0f;
	[SerializeField]
	float mRotateSpeed = 0.0f;
	[SerializeField]
	float mUpAngle = 0.0f;
	[SerializeField]
	float mDownAngle = 0.0f;
	[SerializeField]
	Rigidbody mRigidbody = null;
	[SerializeField]
	Transform mCamera = null;
	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
	}
	void OnApplicationFocus(bool hasFocus)
	{
		Cursor.lockState = hasFocus ? CursorLockMode.Locked : CursorLockMode.None;
	}
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			Cursor.lockState = CursorLockMode.None;
		}
		// 移動
		var move = Vector3.zero;
		move.x = Input.GetAxis("LeftHorizontal");
		move.z = -Input.GetAxis("LeftVertical");
		var velocity = transform.rotation * move * Time.deltaTime * mMoveSpeed;
		mRigidbody.MovePosition(transform.position + velocity);
		// 旋回
		var angle = Vector3.zero;
		angle.x = -Input.GetAxis("RightVertical");
		angle.y = Input.GetAxis("RightHorizontal");
		if(angle == Vector3.zero && Cursor.lockState == CursorLockMode.Locked)
		{
			angle.x = -Input.GetAxis("Mouse Y");
			angle.y = Input.GetAxis("Mouse X");
		}
		var rot = transform.rotation.eulerAngles + new Vector3(0.0f, angle.y, 0.0f) * Time.deltaTime * mRotateSpeed;
		mRigidbody.MoveRotation(Quaternion.Euler(rot));
		var xrot = mCamera.transform.rotation.eulerAngles + new Vector3(angle.x, 0.0f, 0.0f) * Time.deltaTime * mRotateSpeed;
		if(xrot.x > 180.0f && xrot.x < 360.0f - mUpAngle)
		{
			xrot.x = 360.0f - mUpAngle;
		}
		if(xrot.x < 180.0f && xrot.x > mDownAngle)
		{
			xrot.x = mDownAngle;
		}
		mCamera.transform.rotation = Quaternion.Euler(xrot);
	}
}
