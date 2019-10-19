using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
	[SerializeField]
	float mMoveSpeed = 0.0f;
	[SerializeField]
	float mRotateSpeed = 0.0f;
	[SerializeField]
	float mMinAngleX = 0.0f;
	[SerializeField]
	float mMaxAngleX = 0.0f;
	[SerializeField]
	Rigidbody mRigidbody = null;
	[SerializeField]
	Text mScreenMessage = null;
	[SerializeField]
	RawImage mPhoto = null;
	[SerializeField]
	Image mFade = null;
	[SerializeField]
	RenderTexture mRenderTexture = null;
	float mAngleX = 0.0f;
	// ------------------------------------------------------------------------
	/// @brief 写真をとる
	// ------------------------------------------------------------------------
	void Snap()
	{
		if(!Input.GetKeyDown(KeyCode.Space))
		{
			return;
		}
		var cam = Camera.main;
		cam.targetTexture = mRenderTexture;
		cam.Render();
		var w = cam.targetTexture.width;
		var h = cam.targetTexture.height;
		Texture2D tex = new Texture2D(w, h);
		RenderTexture.active = cam.targetTexture;
		tex.ReadPixels(new Rect(0, 0, w, h), 0, 0);
		tex.Apply();
		mPhoto.texture = tex;
		cam.targetTexture = null;
		RenderTexture.active = null;
		mFade.color = Color.white;
		mFade.CrossFadeAlpha(1.0f, 0.0f, false);
		mFade.CrossFadeAlpha(0.0f, 1.0f, false);
	}
	// ------------------------------------------------------------------------
	/// @brief 移動
	// ------------------------------------------------------------------------
	void Move()
	{
		var move = Vector3.zero;
		move.x = Input.GetAxis("LeftHorizontal");
		move.z = -Input.GetAxis("LeftVertical");
		var velocity = transform.rotation * move * Time.deltaTime * mMoveSpeed;
		mRigidbody.MovePosition(transform.position + velocity);
	}
	// ------------------------------------------------------------------------
	/// @brief 旋回
	// ------------------------------------------------------------------------
	void Rotate()
	{
		var angle = Vector3.zero;
		angle.x = -Input.GetAxis("RightVertical");
		angle.y = Input.GetAxis("RightHorizontal");
		if(angle == Vector3.zero && Cursor.lockState == CursorLockMode.Locked)
		{
			angle.x = -Input.GetAxis("Mouse Y");
			angle.y = Input.GetAxis("Mouse X");
		}
		float speed = Time.deltaTime * mRotateSpeed;
		var roty = transform.rotation.eulerAngles + Vector3.up * angle.y * speed;
		mRigidbody.MoveRotation(Quaternion.Euler(roty));
		var cam = Camera.main.transform;
		mAngleX += angle.x * speed;
		mAngleX = Mathf.Clamp(mAngleX, mMinAngleX, mMaxAngleX);
		cam.localEulerAngles = Vector3.right * mAngleX;
	}
	// ------------------------------------------------------------------------
	/// @brief 調べる
	// ------------------------------------------------------------------------
	bool Check()
	{
		RaycastHit hitInfo;
		var cam = Camera.main.transform;
		if(!Physics.Raycast(cam.position, cam.forward, out hitInfo, 2.0f, 1 << LayerMask.NameToLayer("Target")))
		{
			mScreenMessage.text = string.Empty;
			return false;
		}
		mScreenMessage.text = hitInfo.collider.name;
		if(Input.GetButton("Fire1"))
		{
			hitInfo.collider.transform.Translate(Vector3.up * Time.deltaTime);
			return true;
		}
		return false;
	}
	// ------------------------------------------------------------------------
	/// @brief 初回更新
	// ------------------------------------------------------------------------
	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
	}
	// ------------------------------------------------------------------------
	/// @brief
	///
	/// @param hasFocus
	// ------------------------------------------------------------------------
	void OnApplicationFocus(bool hasFocus)
	{
		Cursor.lockState = hasFocus ? CursorLockMode.Locked : CursorLockMode.None;
	}
	// ------------------------------------------------------------------------
	/// @brief 更新
	// ------------------------------------------------------------------------
	void Update()
	{
		// カーソルロック
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			Cursor.lockState = CursorLockMode.None;
		}
		if(Check())
		{
			return;
		}
		Move();
		Rotate();
		Snap();
	}
}
