
#define KAMCORD_IPHONE

#if UNITY_IPHONE

using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using KamcordJSON;

//////////////////////////////////////////////////////////////////
/// iOS Version: 1.7.5 (2014-05-29)
//////////////////////////////////////////////////////////////////

class KamcordImplementationIOS : Kamcord.Implementation
{
    public override void SetLoggingEnabled(bool value)
    {
    }
    
	public override bool IsEnabled ()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return Kamcord_IsEnabled();
		}
		return false;
	}

	public override void WhitelistBoard(String boardName)
	{
	}
	
	public override void BlacklistBoard(String boardName)
	{
	}
	
	public override void WhitelistAllBoards()
	{
	}
	
	public override void BlacklistAllBoards()
	{
	}
	
	public override String GetBoard()
	{
		return "";
	}
	
	public override bool IsWhitelisted(String boardName)
	{
		return true;
	}

	//////////////////////////////////////////////////////////////////
	/// Share settings
	///
	public override void SetVideoTitle (string title)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_SetVideoTitle(title);
		}
	}

	public override void SetYouTubeSettings (string description,
                                          	 string tags)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_SetYouTubeSettings(description, tags);
		}
	}

	public override void SetFacebookAppID (string facebookAppID)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_SetFacebookAppID(facebookAppID);
		}
	}

	public override void SetFacebookAppIDAndShareAuth(string facebookAppID, bool useSharedAuth)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_SetFacebookAppIDAndShareAuth(facebookAppID, useSharedAuth);
		}
	}

	public override void LogoutOfSharedFacebookAuth()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_LogoutOfSharedFacebookAuth();
		}
	}

	public override void SetWeChatAppID(string weChatAppID)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_SetWeChatAppID(weChatAppID);
		}
	}

	public override void SetFacebookDescription (string facebookDescription)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_SetFacebookDescription(facebookDescription);
		}
	}
	
	public override void SetDefaultTweet (string tweet)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_SetDefaultTweet(tweet);
		}
	}
	
	public override void SetTwitterDescription (string twitterDescription)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_SetTwitterDescription(twitterDescription);
		}
	}
	
	public override void SetDefaultEmailSubject (string subject)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_SetDefaultEmailSubject(subject);
		}
	}
	
	public override void SetDefaultEmailBody (string body)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_SetDefaultEmailBody(body);
		}
	}

	public override void SetShareTargets(Kamcord.ShareTarget target1,
	                                     Kamcord.ShareTarget target2,
	                                     Kamcord.ShareTarget target3,
	                                     Kamcord.ShareTarget target4)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_SetShareTargets(target1, target2, target3, target4);
		}
	}
	
	public override void SetVideoMetadata (Dictionary <string, object> metadata)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if (metadata != null && metadata.Count > 0)
			{
				 _KamcordSetVideoMetadata(Json.Serialize(metadata));
			}	
		}
	}
	
	public override void SetMaxFreeDiskSpacePercentageUsage (double percentage)
	{
		// Constrain how much of the remaining free disk space your app will use.
		// For instance, if you'd like to use up to 90% of the remaining free disk space,
		// call this method with a value of 0.9. Values must be in the range (0, 1). 
		// 
		// By default, a percentage is not used. Instead, if free space drops below 50 MB,
		// Kamcord still stop recording.
		
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_KamcordSetMaxFreeDiskSpacePercentageUsage(percentage);
		}
	}
	
	public override string Version()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return Kamcord_Version();
		}
		return "";
	}
	
	public override void SetLevelAndScore (string level, double score)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_SetLevelAndScore(level, score);
		}
	}

	public override void SetDeveloperMetadata (Kamcord.MetadataType metadataType,
	                                           string displayKey,
	                                           string displayValue)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_SetDeveloperMetadata(metadataType, displayKey, displayValue);
		}
	}

	public override void SetDeveloperMetadataWithNumericValue (Kamcord.MetadataType metadataType,
	                                                           string displayKey,
	                                                           string displayValue,
	                                                           double numericValue)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_SetDeveloperMetadataWithNumericValue(metadataType, displayKey, displayValue, numericValue);
		}
	}

	//////////////////////////////////////////////////////////////////
	/// Video Watching
	/// 

	public override bool VideoExistsWithMetadataConstraints(Dictionary <string, object> metadata)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return Kamcord_VideoExistsWithMetadataConstraints(Json.Serialize(metadata));
		}
		return false;
	}

	public override void ShowVideoWithMetadataConstraints(Dictionary <string, object> metadata, string title)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_ShowVideoWithMetadataConstraints(Json.Serialize(metadata), title);
		}
	}

	public override void ShowVideoWithVideoID(string videoID, string title)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_ShowVideoWithVideoID(videoID, title);
		}
	}

	//////////////////////////////////////////////////////////////////
	/// Video Recording
	///
	
	public override void BeginDraw ()
	{
	}
	
	public override void EndDraw ()
	{
	}
	
	public override void StartRecording ()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_StartRecording();
		}
	}
	
	public override void StopRecording ()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_StopRecording();
		}
	}
	
	public override void Pause ()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_Pause();
		}
	}
	
	public override void Resume ()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_Resume();
		}
	}
	
	public override bool IsRecording ()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return Kamcord_IsRecording();
		}
		
		return false;
	}

	public override bool IsPaused ()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return Kamcord_IsPaused();
		}
		return false;
	}
	
	public override void Snapshot (string filename)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_KamcordSnapshot(Application.persistentDataPath + "/" + filename);
		}
	}
	
	public override void SetVideoQuality (Kamcord.VideoQuality quality)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_SetVideoQuality(quality);
		}
	}

	public override void SetUseFastRender (bool useFastRender)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_KamcordSetUseFastRender(useFastRender);
		}
	}

	public override void SetVoiceOverlayEnabled (bool enabled)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_SetVoiceOverlayEnabled(enabled);
		}
	}
	
	public override bool VoiceOverlayEnabled ()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return Kamcord_VoiceOverlayEnabled();
		}
		else
		{
			return false;
		}
	}
	
	public override void ActivateVoiceOverlay (bool activate)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_ActivateVoiceOverlay(activate);
		}
	}
	
	public override bool VoiceOverlayActivated ()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return Kamcord_VoiceOverlayActivated();
		}
		else
		{
			return false;
		}
	}
	
	public override void CaptureFrame ()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_CaptureFrame();
		}
	}

	// Enable notifications from Kamcord.
	// By default, we schedule 4 "Gameplay of the Week" notifications every week for 4 weeks.
	public override void SetNotificationsEnabled (bool notificationsEnabled)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_SetNotificationsEnabled(notificationsEnabled);
		}
	}

	public override void FireTestNotification ()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_FireTestNotification();
		}
	}
	
	//////////////////////////////////////////////////////////////////
	/// UI 
	///	
	
	// Show Kamcord view.
	public override void ShowView ()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_ShowView();
		}
	}
	
	public override void ShowWatchView ()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_ShowWatchView();
		}
	}

	//////////////////////////////////////////////////////////////////
	/// Sundry Methods
	///

	public override void SetMaximumVideoLength (uint seconds)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_SetMaximumVideoLength(seconds);
		}
	}
	
	public override uint MaximumVideoLength ()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return Kamcord_MaximumVideoLength();
		}
		else
		{
			return 0;
		}
	}
	
	public override void SetVideoFPS (uint videoFPS)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_SetVideoFPS(videoFPS);
		}
	}
	
	public override uint VideoFPS ()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return Kamcord_VideoFPS();
		}
		else
		{
			return 0;
		}
	}
	
	public override void Disable ()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_KamcordDisable();
		}
	}

	public override void UploadVideo(string title)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_KamcordUploadVideo(title);
		}
	}

	//////////////////////////////////////////////////////////////////
	/// Audio Overlay APIs - only useful in some pretty rare scenarios
	/// where you want to turn off the automatic recording of game audio
	/// and add your own background track to the recorded video.
	///
	
	public override void Init (string devKey,
						       string devSecret,
						       string appName,
						       Kamcord.VideoQuality videoQuality)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_KamcordInit(devKey, devSecret, appName, videoQuality.ToString());
		}
	}
	
	public override void SetDeviceBlacklist (bool disableiPod4G,
	                                         bool disableiPod5G,
	                                         bool disableiPhone3GS,
	                                         bool disableiPhone4,
	                                         bool disableiPhone4S,
	                                         bool disableiPhone5,
	                                         bool disableiPhone5C,
	                                         bool disableiPhone5S,
	                                         bool disableiPad1,
	                                         bool disableiPad2,
	                                         bool disableiPadMini,
	                                         bool disableiPad3,
	                                         bool disableiPad4,
	                                         bool disableiPadAir)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_SetDeviceBlacklist(
				disableiPod4G,
				disableiPod5G,
				disableiPhone3GS,
				disableiPhone4,
				disableiPhone4S,
				disableiPhone5,
				disableiPhone5C,
				disableiPhone5S,
				disableiPad1,
				disableiPad2,
				disableiPadMini,
				disableiPad3,
				disableiPad4,
				disableiPadAir);
		}
	}
	
	public override void Awake(Kamcord kamcordInstance)
	{
		Init(kamcordInstance.developerKey,
			 kamcordInstance.developerSecret,
			 kamcordInstance.appName,
			 kamcordInstance.videoQuality);
	}

	public override void SetCrossPromoIcon (string localImageFileURL)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_KamcordSetCrossPromoIcon(localImageFileURL);
		}
	}
    
	public override void SetMode(int mode)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_SetMode(mode);
		}
	}

	public override void SetAudioSettings(int sampleRate, int numChannels)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_KamcordSetAudioSettings(sampleRate, numChannels);
		}
	}

	public override void WriteAudioData(float [] data, int numSamples)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Kamcord_WriteAudioBytes(data, numSamples);
		}
	}

	public override void SetFlushOnCopy(bool flush)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_KamcordSetGLFinishAfterCopyFrame(flush);
		}
	}
    
	/* Interface to native implementation */
	
	[DllImport ("__Internal")]
	private static extern bool Kamcord_SetDeviceBlacklist(
		bool disableiPod4G,
		bool disableiPod5G,
		bool disableiPhone3GS,
		bool disableiPhone4,
		bool disableiPhone4S,
		bool disableiPhone5,
		bool disableiPhone5C,
		bool disableiPhone5S,
		bool disableiPad1,
		bool disableiPad2,
		bool disableiPadMini,
		bool disableiPad3,
		bool disableiPad4,
		bool disableiPadAir);
	
	[DllImport ("__Internal")]
	private static extern bool Kamcord_IsEnabled();
	
	[DllImport ("__Internal")]
	private static extern string Kamcord_Version();
	
	[DllImport ("__Internal")]
	private static extern void _KamcordInit(string devKey,
											string devSecret,
											string appName,
											string videoQuality);

	//////////////////////////////////////////////////////////////////
    /// Share settings
    ///
	[DllImport ("__Internal")]
	private static extern void Kamcord_SetVideoTitle(string title);
	
	[DllImport ("__Internal")]
    private static extern void Kamcord_SetYouTubeSettings(string description,
                                                          string tags);
	
	[DllImport ("__Internal")]
	private static extern void Kamcord_SetFacebookAppID(string facebookAppID);

	[DllImport ("__Internal")]
	private static extern void Kamcord_SetFacebookAppIDAndShareAuth(string facebookAppID, bool useSharedAuth);

	[DllImport ("__Internal")]
	private static extern void Kamcord_LogoutOfSharedFacebookAuth();

	[DllImport ("__Internal")]
	private static extern void Kamcord_SetWeChatAppID(string weChatAppID);

	[DllImport ("__Internal")]
	private static extern void Kamcord_SetFacebookDescription(string description);

	[DllImport ("__Internal")]
    private static extern void Kamcord_SetDefaultTweet(string tweet);
	
	[DllImport ("__Internal")]
	private static extern void Kamcord_SetTwitterDescription(string twitterDescription);
	
	[DllImport ("__Internal")]
    private static extern void Kamcord_SetDefaultEmailSubject(string subject);
	
	[DllImport ("__Internal")]
    private static extern void Kamcord_SetDefaultEmailBody(string body);

	[DllImport ("__Internal")]
    private static extern void Kamcord_SetLevelAndScore(string level,
                                                        double score);

	[DllImport ("__Internal")]
	private static extern void Kamcord_SetDeveloperMetadata(Kamcord.MetadataType metadataType,
	                                                        string displayKey,
	                                                        string displayValue);

	[DllImport ("__Internal")]
	private static extern void Kamcord_SetDeveloperMetadataWithNumericValue(Kamcord.MetadataType metadataType,
	                                                                        string displayKey,
	                                                                        string displayValue,
	                                                                        double numericValue);

	//////////////////////////////////////////////////////////////////
	/// Video Viewing
	/// 
	[DllImport ("__Internal")]
	private static extern bool Kamcord_VideoExistsWithMetadataConstraints(string jsonDictionary);

	[DllImport ("__Internal")]
	private static extern void Kamcord_ShowVideoWithMetadataConstraints(string jsonDictionary,
	                                                                    string title);

	[DllImport ("__Internal")]
	private static extern void Kamcord_ShowVideoWithVideoID(string videoID,
	                                                        string title);

	//////////////////////////////////////////////////////////////////
    /// Video recording 
    ///
	[DllImport ("__Internal")]
	private static extern void Kamcord_StartRecording();
	
	[DllImport ("__Internal")]
	private static extern void Kamcord_StopRecording();
	
	[DllImport ("__Internal")]
	private static extern void Kamcord_Pause();
	
	[DllImport ("__Internal")]
	private static extern void Kamcord_Resume();
	
	[DllImport ("__Internal")]
	private static extern bool Kamcord_IsRecording();

	[DllImport ("__Internal")]
	private static extern bool Kamcord_IsPaused();

	[DllImport ("__Internal")]
	private static extern bool _KamcordSetUseFastRender(bool useFastRender);

	[DllImport ("__Internal")]
	private static extern bool Kamcord_SetVoiceOverlayEnabled(bool enabled);
	
	[DllImport ("__Internal")]
	private static extern bool Kamcord_VoiceOverlayEnabled();
	
	[DllImport ("__Internal")]
	private static extern void Kamcord_ActivateVoiceOverlay(bool activate);
	
	[DllImport ("__Internal")]
	private static extern bool Kamcord_VoiceOverlayActivated();

	[DllImport ("__Internal")]
	private static extern void Kamcord_CaptureFrame();
	
	[DllImport ("__Internal")]
	private static extern void Kamcord_SetVideoQuality(Kamcord.VideoQuality quality);

	[DllImport ("__Internal")]
	private static extern void Kamcord_SetShareTargets(Kamcord.ShareTarget target1,
	                                                   Kamcord.ShareTarget target2,
	                                                   Kamcord.ShareTarget target3,
	                                                   Kamcord.ShareTarget target4);

	[DllImport ("__Internal")]
	private static extern void _KamcordSnapshot(string filepath);
	
	[DllImport ("__Internal")]
	private static extern void _KamcordSetVideoMetadata(string jsonMetadata);
	
	[DllImport ("__Internal")]
	private static extern void _KamcordSetMaxFreeDiskSpacePercentageUsage(double percentage);

	[DllImport ("__Internal")]
	private static extern void _KamcordSetGLFinishAfterCopyFrame(bool finish);

	//////////////////////////////////////////////////////////////////
    /// UI 
    ///
    [DllImport ("__Internal")]
    private static extern void Kamcord_ShowView();
	
	[DllImport ("__Internal")]
    private static extern void Kamcord_ShowWatchView();
    
	//////////////////////////////////////////////////////////////////
    /// Notifications
    ///
	[DllImport ("__Internal")]
	private static extern void Kamcord_SetNotificationsEnabled(bool notificationsEnabled);
	
	[DllImport ("__Internal")]
	private static extern void Kamcord_FireTestNotification();

	//////////////////////////////////////////////////////////////////
    /// Sundry Methods
    ///    
	[DllImport ("__Internal")]
    private static extern void Kamcord_SetMaximumVideoLength(uint seconds);
    
	[DllImport ("__Internal")]
    private static extern uint Kamcord_MaximumVideoLength();

	[DllImport ("__Internal")]
    private static extern void Kamcord_SetVideoFPS(uint seconds);
    
	[DllImport ("__Internal")]
    private static extern uint Kamcord_VideoFPS();

	[DllImport ("__Internal")]
	private static extern bool _KamcordUploadVideo(string title);
	
    //////////////////////////////////////////////////////////////////
    /// Audio APIs - only useful in some pretty rare situations
    ///

	[DllImport ("__Internal")]
	private static extern void _KamcordSetAudioSettings(int sampleRate, int numChannels);
	
	[DllImport ("__Internal")]
	private static extern void Kamcord_WriteAudioBytes(float [] data, int numSamples);

	// Other Kamcord methods

	[DllImport ("__Internal")]
	private static extern bool _KamcordDisable();

	[DllImport ("__Internal")]
	private static extern void _KamcordSetCrossPromoIcon(string localImagefileURL);

	[DllImport ("__Internal")]
	private static extern void Kamcord_SetMode(long mode);
}
	
#endif
