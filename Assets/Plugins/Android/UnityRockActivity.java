package com.tango.unityrock;

import android.os.Bundle;
import android.util.Log;

import android.content.Intent;
import android.widget.Toast;
import com.unity3d.player.UnityPlayerActivity;
import com.unity3d.player.UnityPlayer;

public class UnityRockActivity extends UnityPlayerActivity {
    public static final String TAG = "com.tango.unityrock";
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        Log.d(TAG,"::onCreate.");
        handleIntent(getIntent());
    }

    @Override
    protected void onNewIntent(Intent i){
    	Log.v(TAG, "::onNewIntent");
    	String data = i.getDataString();
    	if(data != null){
    		Log.v(TAG, data);
        handleIntent(i);
    	}
    }

    public void unityCallback(String gift_id, String gift_type){
    	Log.d(TAG, "Gift message from friend: gift_id " +gift_id + " gift_type: " + gift_type );
    	Toast.makeText(this, gift_type, Toast.LENGTH_SHORT).show();
    }

    public void handleIntent(Intent i){
    	String data = i.getDataString();
    	Log.v(TAG, "::handleIntent");
    	if(data != null){
    		Log.v(TAG, data);
    		UnityPlayer.UnitySendMessage("GameObject", "HandleUrl", data);
    	}
    }
}
