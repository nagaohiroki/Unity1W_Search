using UnityEngine;
public class Player : MonoBehaviour
{
	[SerializeField]
	float mMoveSpeed = 0.0f;
	[SerializeField]
	float mRotateSpeed = 0.0f;
	void Update()
	{
		var vec = Vector3.zero;
		vec.x = Input.GetAxis("LeftHorizontal");
		vec.z = Input.GetAxis("LeftVertical");
		var rigid = GetComponent<Rigidbody>();
		var forw = transform.rotation * vec * Time.deltaTime * mMoveSpeed;
		rigid.MovePosition(transform.position + forw);
	}
}
