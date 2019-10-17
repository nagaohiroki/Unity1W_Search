using UnityEngine;
public class Player : MonoBehaviour
{
	[SerializeField]
	float mMoveSpeed = 0.0f;
	[SerializeField]
	float mRotateSpeed = 0.0f;
	[SerializeField]
	Rigidbody mRigidbody = null;
	[SerializeField]
	Transform mCamera = null;
	void Update()
	{
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
		if(angle == Vector3.zero)
		{
			angle.x = -Input.GetAxis("Mouse Y");
			angle.y = Input.GetAxis("Mouse X");
		}
		var rot = transform.rotation.eulerAngles + new Vector3(0.0f, angle.y, 0.0f) * Time.deltaTime * mRotateSpeed;
		mRigidbody.MoveRotation(Quaternion.Euler(rot));
		var xrot = mCamera.transform.rotation.eulerAngles + new Vector3(angle.x, 0.0f, 0.0f) * Time.deltaTime * mRotateSpeed;
		float upAngle = 40.0f;
		float downAngle = 40.0f;
		if(xrot.x > 180.0f && xrot.x < 360.0f - upAngle)
		{
			xrot.x = 360.0f - upAngle;
		}
		if(xrot.x < 180.0f && xrot.x > downAngle)
		{
			xrot.x = downAngle;
		}
		mCamera.transform.rotation = Quaternion.Euler(xrot);
	}
}
