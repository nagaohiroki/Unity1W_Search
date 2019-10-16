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
		angle.y = Input.GetAxis("RightHorizontal");
		var rot = transform.rotation.eulerAngles + angle * Time.deltaTime * mRotateSpeed;
		mRigidbody.MoveRotation(Quaternion.Euler(rot));
		var xangle = Vector3.zero;
		xangle.x = -Input.GetAxis("RightVertical");
		var xrot = mCamera.transform.rotation.eulerAngles + xangle * Time.deltaTime * mRotateSpeed;
		mCamera.transform.rotation = Quaternion.Euler(xrot);
	}
}
