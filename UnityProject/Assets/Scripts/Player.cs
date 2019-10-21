using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
public class Player : MonoBehaviour
{
	enum PlayerState
	{
		None,
		Check,
		GameOver,
		WaitReset
	}
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
	float mCheckTime = 0.0f;
	const float mCheckTimeMax = 2.0f;
	Target mCheckTarget;
	PlayerState mState = PlayerState.None;
	// キーを持っているか
	bool mHasKey = false;
	// 時間
	float mTimer = 0.0f;
	// 時間
	int mCheckCount = 0;
	// 写真の回数
	int mPhotoCount = 0;

	// ------------------------------------------------------------------------
	/// @brief 写真をとる
	// ------------------------------------------------------------------------
	void Photo()
	{
		if(!Input.GetKeyDown(KeyCode.Space))
		{
			return;
		}
		var targets = FindObjectsOfType<Target>();
		var cam = Camera.main;
		cam.targetTexture = mRenderTexture;
		foreach(var target in targets)
		{
			target.PhotoMode(true);
		}
		cam.Render();
		foreach(var target in targets)
		{
			target.PhotoMode(false);
		}
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
		mPhoto.gameObject.SetActive(true);
		++mPhotoCount;
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
		mRigidbody.velocity = velocity;
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
	void Check()
	{
		RaycastHit hitInfo;
		var cam = Camera.main.transform;
		if(!Physics.Raycast(cam.position, cam.forward, out hitInfo, 2.0f, 1 << LayerMask.NameToLayer("Target")))
		{
			mScreenMessage.text = string.Empty;
		}
		if(hitInfo.collider == null)
		{
			return;
		}
		mScreenMessage.text = hitInfo.collider.name;
		mScreenMessage.color = Color.white;
		if(!Input.GetButtonDown("Fire1"))
		{
			return;
		}
		// 調べる
		var target = hitInfo.collider.gameObject.GetComponent<Target>();
		if(target != null)
		{
			mCheckTime = mCheckTimeMax;
			mState = PlayerState.Check;
			mCheckTarget = target;
			++mCheckCount;
			return;
		}
		// キーを取得
		if(hitInfo.collider.gameObject.name == "Key")
		{
			mHasKey = true;
			Destroy(hitInfo.collider.gameObject);
		}
		// クリア
		if(hitInfo.collider.gameObject.name == "Door")
		{
			if(mHasKey)
			{
				mState = PlayerState.WaitReset;
				mScreenMessage.color = Color.black;
				mFade.CrossFadeAlpha(1.0f, 0.0f, false);
				mScreenMessage.text = MakeClearMessage();
				mFade.color = Color.white;
			}
		}
	}
	string MakeClearMessage()
	{
		return	"GAME CLEAR!!";
	}
	// ------------------------------------------------------------------------
	/// @brief 調べる中
	// ------------------------------------------------------------------------
	void CheckUpdate()
	{
		mCheckTime -= Time.deltaTime;
		if(mCheckTime <= 0.0f)
		{
			if(mCheckTarget.mGameOver != null)
			{
				mState = PlayerState.GameOver;
				mCheckTarget.mGameOver.SetActive(true);
				return;
			}
			if(mCheckTarget.mKey != null)
			{
				mCheckTarget.mKey.SetActive(true);
			}
			mState = PlayerState.None;
			Destroy(mCheckTarget.gameObject);
			return;
		}
		mCheckTarget.gameObject.transform.Translate(0.0f, Time.deltaTime, 0.0f);
	}
	// ------------------------------------------------------------------------
	/// @brief ゲームオーバー演出
	// ------------------------------------------------------------------------
	void GameOverUpdate()
	{
		if(mCheckTarget.mGameOver == null)
		{
			return;
		}
		var vec = Camera.main.transform.position - mCheckTarget.mGameOver.transform.position;
		mCheckTarget.mGameOver.transform.position += vec.normalized * Time.deltaTime * 1.5f;
		if(vec.magnitude < 0.3f)
		{
			mState = PlayerState.WaitReset;
			mScreenMessage.color = Color.white;
			mScreenMessage.text = "GAME OVER..";
			mFade.CrossFadeAlpha(1.0f, 0.0f, false);
			mFade.color = Color.black;
		}
	}
	// ------------------------------------------------------------------------
	/// @brief ターゲットを初期化
	// ------------------------------------------------------------------------
	void Awake()
	{
		var targets = FindObjectsOfType<Target>();
		var list = new List<int>();
		for(int i = 0; i < targets.Length; i++)
		{
			list.Add(i);
		}
		list = list.OrderBy(a => System.Guid.NewGuid()).ToList();
		if(list.Count < 2)
		{
			return;
		}
		targets[list[0]].mKey = GameObject.Find("Key");
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
		if(mState == PlayerState.WaitReset)
		{
			if(Input.GetKeyDown(KeyCode.Space))
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			}
			return;
		}
		if(mState == PlayerState.GameOver)
		{
			GameOverUpdate();
			return;
		}
		if(mState == PlayerState.Check)
		{
			CheckUpdate();
			return;
		}
		// カーソルロック
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			Cursor.lockState = CursorLockMode.None;
		}
		Check();
		Move();
		Rotate();
		Photo();
		mTimer += Time.deltaTime;
	}
}
