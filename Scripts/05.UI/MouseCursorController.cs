using UnityEngine;
using WhereAreYouLookinAt.Enum;

public class MouseCursorController : MonoBehaviour
{
    public static MouseCursorController Instance { get; private set; }

    [Header("Cursor Texture")]
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D interactCursor;
    [SerializeField] private Texture2D impossibleCursor;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        SetCursor(CursorType.Default);
    }

    public void SetEnable(bool enable)
    {
        if (enable)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void SetCursor(CursorType cursorType)
    {
        Texture2D tex;
        Vector2 hotspot;

        switch (cursorType)
        {
            case CursorType.Interact:
                tex = interactCursor;
                hotspot = Vector2.zero;
                break;
            case CursorType.Impossible:
                tex = interactCursor;
                hotspot = Vector2.zero;
                break;
            default:
                tex = defaultCursor;
                hotspot = new Vector2(24, 20);
                break;
        }

#if UNITY_EDITOR
        Cursor.SetCursor(tex, hotspot, CursorMode.ForceSoftware);
#else
    Cursor.SetCursor(tex, hotspot, CursorMode.Auto);
#endif
    }
}
