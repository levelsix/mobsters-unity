using UnityEngine;
using System.Collections;

public class CBKChangeSceneButton : MonoBehaviour {
 
    public MSValues.Scene.Scenes scene;
    
    void OnClick()
    {
        MSValues.Scene.ChangeScene(scene);  
    }
    
}
