using UnityEngine;
public class Target : MonoBehaviour
{
	public GameObject mKey = null;
	public GameObject mGameOver = null;
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
		if(!inIsPhoto)
		{
			transform.rotation = mRotate;
			return;
		}
		if(mKey != null)
		{
			var rot = transform.rotation;
			transform.rotation = Quaternion.Euler(rot.eulerAngles + new Vector3(0.0f, 45.0f, 0.0f));
		}
	}
}
