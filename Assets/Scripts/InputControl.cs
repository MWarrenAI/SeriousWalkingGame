using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UIElements;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class InputControl : MonoBehaviour
{
    //Matt's Game Manager
    /// <summary>
    /// Used to call
    /// </summary>
    [SerializeField]
    GameManager m_GameManager;

    float m_ScreenSegment;
    private const int c_ScreenDivisionCount = 6;
    private Vector2 m_PressStartPosition;
    private bool m_IsHoldingPress;
    [SerializeField] GameObject m_QuestionObject;
    [SerializeField] GameObject m_LeftAnswerPanel;
    [SerializeField] GameObject m_RightAnswerPanel;
    Touch? m_ActiveTouch;

    [RuntimeInitializeOnLoadMethodAttribute]
    static void EnableInputSystem()
    {
        EnhancedTouchSupport.Enable();
        TouchSimulation.Enable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_ActiveTouch = null;
        m_ScreenSegment = Screen.width / c_ScreenDivisionCount;
    }

    // Update is called once per frame
    void Update()
    {
        if (Touch.activeTouches.Count > 0)
        {
            m_ActiveTouch = Touch.activeTouches[0];

            if (m_ActiveTouch != null)
            {
                if (m_ActiveTouch.Value.began)
                {
                    OnTouchPress();
                }
                else if (m_ActiveTouch.Value.ended)
                {
                    OnTouchRelease();
                }
            }
        }

        if(m_IsHoldingPress)
        {
            Vector2 screenCentre = new Vector2(Screen.width / 2, Screen.height / 2);
            Vector2 currentPosition = m_ActiveTouch.Value.screenPosition;
            Vector2 boxPosition = new Vector2(currentPosition.x, screenCentre.y);

            if (m_QuestionObject != null)
            {
                m_QuestionObject.transform.position = boxPosition;
            }
            
            if(currentPosition.x < m_ScreenSegment)
            {
                m_LeftAnswerPanel?.SetActive(true);
            }
            else if(currentPosition.x > (Screen.width - m_ScreenSegment))
            {
                m_RightAnswerPanel?.SetActive(true);
            }
            else
            {
                m_LeftAnswerPanel?.SetActive(false);
                m_RightAnswerPanel?.SetActive(false);
            }
        }
    }

    void ResetQuestionBoxPosition()
    {
        if (m_QuestionObject != null)
        {
            Vector2 screenCentre = new Vector2(Screen.width / 2, Screen.height / 2);
            m_QuestionObject.transform.position = screenCentre;
        }

        m_LeftAnswerPanel?.SetActive(false);
        m_RightAnswerPanel?.SetActive(false);
    }

    void OnTouchPress()
    {
        if (m_ActiveTouch != null)
        {
            m_PressStartPosition = m_ActiveTouch.Value.screenPosition;
            Debug.Log("ON PRESS : " + m_PressStartPosition);
            m_IsHoldingPress = true;
        }
    }

    void OnTouchRelease()
    {
        if (m_PressStartPosition != null && (m_ActiveTouch != null))
        {
            Vector2 endPosition = m_ActiveTouch.Value.screenPosition;
            Vector2 delta = endPosition - m_PressStartPosition;
            Debug.Log("ON RELEASE : " + endPosition);
            Debug.Log("Mouse Delta: " + delta);
            m_PressStartPosition = Vector2.zero;
            m_IsHoldingPress = false;
            m_ActiveTouch = null;

            if (endPosition.x > (Screen.width - m_ScreenSegment))
            {
                OnRightSwipe();
            }
            else if(endPosition.x < m_ScreenSegment)
            {
                OnLeftSwipe();
            }
            else
            {
                ResetQuestionBoxPosition();
            }
        }
    }

    void OnRightSwipe()
    {
        Debug.Log("Swiped Right");
        ResetQuestionBoxPosition();

        if (m_GameManager != null)
        {
            m_GameManager.SwipeRight();
        }
    }

    void OnLeftSwipe()
    {
        Debug.Log("Swiped Left");
        ResetQuestionBoxPosition();

        if(m_GameManager != null)
        {
            m_GameManager.SwipeLeft();
        }
    }
}
