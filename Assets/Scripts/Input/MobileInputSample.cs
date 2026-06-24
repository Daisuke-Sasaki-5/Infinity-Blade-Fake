using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class MobileInputVisualizer : MonoBehaviour
{
    [SerializeField]
    private float swipeDistance = 80f;

    [SerializeField] private Player player;

    private Vector2 startPosition;
    private Vector2 currentPosition;

    private bool isTouching;
    private bool isUIMouseClick;

    private string currentState = "None";

    // í∑âüÇµä÷òA
    [SerializeField] private float holdTimeThershold = 0.2f;

    private float holdTimer;
    private bool isGuarding;

    // UIâüÇµÇΩÇ©Ç«Ç§Ç©
    private bool isUITouch;

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    void Update()
    {
#if UNITY_EDITOR

        UpdateMouseInput();

#else

        UpdateTouchInput();

#endif
    }

    void UpdateMouseInput()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            isUIMouseClick = EventSystem.current.IsPointerOverGameObject();

            if (isUIMouseClick)
            {
                return;
            }

            startPosition = Mouse.current.position.ReadValue();
            currentPosition = startPosition;

            holdTimer = 0f;
            isGuarding = false;

            isTouching = true;
        }

        if (Mouse.current.leftButton.isPressed)
        {
            if(isUIMouseClick) { return; }

            currentPosition = Mouse.current.position.ReadValue();

            holdTimer += Time.deltaTime;

            if(!isGuarding && holdTimer >= holdTimeThershold)
            {
                isGuarding = true;
                currentState = "Guard";
                player.StartGuard();
            }
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (isUIMouseClick)
            {
                isUIMouseClick = false;
                return;
            }

            if (isGuarding)
            {
                currentState = "Guard End";
                player.EndGuard();
            }
            else
            {
                currentPosition = Mouse.current.position.ReadValue();

                CheckGesture();
            }
            isTouching = false;
        }
    }

    void UpdateTouchInput()
    {
        if (Touch.activeTouches.Count == 0)
        {
            return;
        }

        Touch touch = Touch.activeTouches[0];

        if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
        {
            isUITouch = EventSystem.current.IsPointerOverGameObject(touch.touchId);

            if (isUITouch)
            {
                return;
            }

            startPosition = touch.screenPosition;
            currentPosition = startPosition;

            holdTimer = 0f;
            isGuarding = false;

            isTouching = true;
        }

        if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved)
        {
            if (isUITouch)
            {
                return;
            }
            currentPosition = touch.screenPosition;
        }

        if(touch.phase == UnityEngine.InputSystem.TouchPhase.Stationary || touch.phase == UnityEngine.InputSystem.TouchPhase.Moved)
        {
            if (isUITouch)
            {
                return;
            }

            holdTimer += Time.deltaTime;

            if(!isGuarding && holdTimer >= holdTimeThershold)
            {
                isGuarding = true;
                currentState = "Guard";
                player.StartGuard();
            }
        }

        if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended)
        {
            if (isUITouch)
            {
                isUITouch = false;
                return;
            }

            if (isGuarding)
            {
                currentState = "Guard End";
                player.EndGuard();
            }
            else
            {
                currentPosition = touch.screenPosition;

                CheckGesture();
            }
            isTouching = false;
        }
    }

    void CheckGesture()
    {
        Vector2 diff = currentPosition - startPosition;

        if (diff.magnitude < swipeDistance)
        {
            currentState = "Tap";
            return;
        }

        if(Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
        {
            currentState = diff.x > 0 ? "swipe right" : "swipe left";
        }
        else
        {
            currentState = diff.y > 0 ? "swipe up" : "swipe down";
        }

        player.Attack();
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);

        style.fontSize = 50;
        style.normal.textColor = Color.white;

        GUI.Label(new Rect(20,20,500,100),$"State : {currentState}",style);

        if (isTouching)
        {
            DrawPoint(currentPosition, 30, Color.white);

            DrawLine(
                startPosition,
                currentPosition,
                Color.yellow,
                5f
            );
        }
    }

    void DrawPoint(Vector2 position, float size, Color color)
    {
        Rect rect = new Rect(
            position.x - size / 2,
            Screen.height - position.y - size / 2,
            size,
            size
        );

        GUI.color = color;
        GUI.Box(rect, "");
    }

    void DrawLine(Vector2 start, Vector2 end, Color color, float width)
    {
        Matrix4x4 matrix = GUI.matrix;

        Vector2 guiStart = new Vector2(
            start.x,
            Screen.height - start.y
        );

        Vector2 guiEnd = new Vector2(
            end.x,
            Screen.height - end.y
        );

        float angle =
            Vector3.Angle(
                guiEnd - guiStart,
                Vector2.right
            );

        if (guiStart.y > guiEnd.y)
        {
            angle = -angle;
        }

        float length =
            (guiEnd - guiStart).magnitude;

        GUI.color = color;

        GUIUtility.RotateAroundPivot(
            angle,
            guiStart
        );

        GUI.DrawTexture(
            new Rect(
                guiStart.x,
                guiStart.y,
                length,
                width
            ),
            Texture2D.whiteTexture
        );

        GUI.matrix = matrix;
    }

    // åªç›ÇÃStateÇï‘Ç∑
    public void SetState(string state)
    {
        currentState = state;
    }
}