using UnityEngine;
public class Target : MonoBehaviour
{
	public GameObject mKey = null;
	public GameObject mGameOver = null;
	void Start()
	{
		if(mGameOver != null)
		{
			mGameOver.SetActive(false);
			mGameOver.transform.position = transform.position;
		}
		if(mKey != null)
		{
			mKey.SetActive(true);
			mKey.transform.position = transform.position;
		}
	}
}
