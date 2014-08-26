using UnityEngine;
using System.Collections;

public class MSHireFriendInvite : MonoBehaviour {

	[SerializeField]
	MSCheckBox checkBox;

	[SerializeField]
	UITexture image;

	[SerializeField]
	UILabel name;

	public bool checkMarked
	{
		get
		{
			return checkBox.checkMarked;
		}
		set
		{
			checkBox.checkMarked = value;
		}

	}

	public MSFacebookFriend friendInfo;

	public void Init(MSFacebookFriend info, UIScrollView scrollView)
	{
		name.text = info.name;
		friendInfo = info;
		GetComponent<UIDragScrollView>().scrollView = scrollView;
		MSFacebookManager.instance.RunLoadPhotoForUser(info.id, this.image);
	}
}
