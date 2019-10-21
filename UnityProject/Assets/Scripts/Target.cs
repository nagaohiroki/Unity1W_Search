using UnityEngine;
public class Target : MonoBehaviour
{
	public enum PhotoType
	{
		Rotate,
		Hide
	}
	public GameObject mKey = null;
	public GameObject mGameOver = null;
	[SerializeField]
	PhotoType mPhotoType = PhotoType.Rotate;
	Quaternion mRotate;
	void Start()
	{
		if(mGameOver != null)
		{
			mGameOver.SetActive(false);
			mGameOver.transform.position = transform.position;
		}
		if(mKey != null)
		{
			mKey.SetActive(false);
			mKey.transform.position = transform.position;
		}
		mRotate = transform.rotation;
	}
	public void PhotoMode(bool inIsPhoto)
	{
		if(mKey == null)
		{
			return;
		}
		switch(mPhotoType)
		{
		case PhotoType.Hide:
			gameObject.SetActive(!inIsPhoto);
			break;
		case PhotoType.Rotate:
			PhotoModeRotate(inIsPhoto);
			break;
		}
	}
	void PhotoModeRotate(bool inIsPhoto)
	{
		if(!inIsPhoto)
		{
			transform.rotation = mRotate;
			return;
		}
		var rot = transform.rotation;
		transform.rotation = Quaternion.Euler(rot.eulerAngles + new Vector3(0.0f, 45.0f, 0.0f));
	}
}
