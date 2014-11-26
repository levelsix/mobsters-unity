using UnityEngine;
using System.Collections;
 public enum EnumTimeType { 
	Seconds = 0, 
	Minutes = 1, 
	Hours   = 2, 
	Days    = 3/*, 
	Weeks   = 4,
	Months  = 5*/
}


public class ELANNotification : MonoBehaviour {
	
	//Notification identifier
	public int ID = 0;
	public string fullClassName = "com.unity3d.player.UnityPlayerActivity"; 
	public string title = "Insert title";		
	public string message = "Insert message";   
	public EnumTimeType delayTypeTime;
	public int delay = 0; 			 		   
	public EnumTimeType repetitionTypeTime;
	public int repetition = 0;
	
	//	Advanced notification 
	public bool advancedNotification = false;
	public bool useVibration = false;
	public bool useSound = false;
	
	public bool sendOnStart = false;  // NEW (typo)
	
	public void Start(){
		
		ID = (int)(Time.time*1000) + (int)Random.Range (0,int.MaxValue/2);
		if(sendOnStart) send();
		
	}
	
	public void send(){
		ELANManager.sendParametrizedNotification(this);
	}
	
	public void cancel(){
		ELANManager.CancelLocalNotification(ID);
		
	}

	public long getDelay(){
		
		switch(delayTypeTime){ // NEW (EnumType can be used, no need for break after return)
			
		case EnumTimeType.Seconds:
			return delay;
			//break;
		case EnumTimeType.Minutes:
			return delay * 60;
		case EnumTimeType.Hours:
			return delay * 3600;
		case EnumTimeType.Days:
			return delay * 3600 * 24;
		default:
			return 0;
		}
	}
	
	public long getRepetition() { // NEW Added!
		switch(repetitionTypeTime){
			
		case EnumTimeType.Seconds:
			return repetition;
			//break;
		case EnumTimeType.Minutes:
			return repetition * 60;
		case EnumTimeType.Hours:
			return repetition * 3600;
		case EnumTimeType.Days:
			return repetition * 3600 * 24;
		default:
			return 0;
		}
	}
	
	public string toString(){
	
	string toString = "fullClassName : " + fullClassName + "\n" +
		"id : " + ID + "\n" +
		"title : " + title + "\n" +
		"message : " + message + "\n" +
		"typeTime : " + delayTypeTime + "\n" +
		"delay : " + delay + "\n" +
		"repetition : " + repetition +
		"advancedNotification : " + advancedNotification + "\n" +
		"useVibration : " + useVibration + "\n" +
		"useSound : " + useSound + "\n";
		
		//Debug.Log(toString);
		
		return toString;
		
		}
	
}