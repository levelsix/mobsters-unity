using UnityEngine;
using System.Collections;
using com.lvl6.proto;
using System.Collections.Generic;

[System.Serializable]
public class MSTimer 
{
	long _startTime = 0;
	public long startTime { get { return _startTime;}}

	long _length = 0;
	public long length { get { return _length;}}

	public long finishTime 
	{
		get
		{
			return startTime + length - helpTime - speedUpTime;
		}
	}

	public long timeLeft { get { return finishTime - MSUtil.timeNowMillis;}}

	public bool done { get { return timeLeft <= 0;}}

	public float progress { get { return 1 -((float)timeLeft) / length; }}

	#region Help

	ClanHelpProto currActiveHelp;

	StartupResponseProto.StartupConstants.ClanHelpConstants helpConstants;

	public int helpCount
	{
		get
		{
			if (currActiveHelp == null || !currActiveHelp.userDataUuid.Equals(uuid))
			{
				if (_staticDataId > 0)
				{
					currActiveHelp = MSClanManager.instance.GetClanHelp (timerType, staticDataId, uuid);
				}
				else
				{
					currActiveHelp = MSClanManager.instance.GetClanHelp(timerType, uuid);
				}
			}

			if (currActiveHelp == null) return 0;
			return Mathf.Min(currActiveHelp.helperUuids.Count, currActiveHelp.maxHelpers);
		}
	}

	public long helpTime
	{
		get
		{
			if (helpConstants == null || helpConstants.helpType != timerType)
			{
				helpConstants = MSWhiteboard.constants.clanHelpConstants.Find(x=>x.helpType == timerType);
			}
			if (helpConstants == null) return 0;
			return helpConstants.amountRemovedPerHelp * helpCount;
		}
	}

	#endregion

	#region Speedup Items
	//TODO: All of this!

	long _speedUpTime = 0;
	public long speedUpTime
	{
		get
		{
			return _speedUpTime;
		}
	}

	#endregion

	string _uuid;
	public string uuid { get { return _uuid;}}

	int _staticDataId;
	public int staticDataId { get { return _staticDataId;}}

	public GameActionType timerType;

	public MSTimer (GameActionType type = GameActionType.NO_HELP, string uuid = "", int staticDataId = 0, long startTime = 0, long length = 0)
	{
		timerType = type;
		_uuid = uuid;
		_staticDataId = staticDataId;
		Set(startTime, length);
	}

	public void Set (long length)
	{
		Set (MSUtil.timeNowMillis, length);
	}

	public void Set (long startTime, long length)
	{
		_startTime = startTime;
		_length = length;
		currActiveHelp = null;
	}

	public void AlterStartTime (long startTime)
	{
		_startTime = startTime;
	}

}
