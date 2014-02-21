using UnityEngine;
using System.Collections;

public class CBKChangeSceneButton : MonoBehaviour {
 
    public CBKValues.Scene.Scenes scene;
    
    void OnClick()
    {
        CBKValues.Scene.ChangeScene(scene);  
    }
    
}
