using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public int canvas;
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

    public void MoveToSceneSubject(string subject) 
    {
        switch(subject)
        {
            case "maths":
                PlayerPrefs.SetString("subject", "maths");
                SceneManager.LoadScene(2);
                break;
            case "english":
                PlayerPrefs.SetString("subject", "english");
                SceneManager.LoadScene(2);
                break;
            case "compsci":
                PlayerPrefs.SetString("subject", "compsci");
                SceneManager.LoadScene(2);
                break;
            case "sci":
                PlayerPrefs.SetString("subject", "sci");
                SceneManager.LoadScene(2);
                break;
            case "history":
                PlayerPrefs.SetString("subject", "history");
                SceneManager.LoadScene(2);
                break;
        }
    }
}
