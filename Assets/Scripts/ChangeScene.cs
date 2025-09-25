using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethodAttribute]
    static void EnableInputSystem()
    {
        EnhancedTouchSupport.Enable();
        TouchSimulation.Enable();
    }

    public void MoveToScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }
}
