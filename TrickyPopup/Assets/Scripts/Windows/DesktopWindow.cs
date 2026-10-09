using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// The shared window framework. Put this on the root of every window.
/// Windows must be direct children of WindowLayer and should start INACTIVE.
/// Open() shows it, raises the flag "opened:&lt;windowId&gt;", adds a taskbar button and brings it to front.
/// Header parts are auto-found if you name them Header / Title / CloseButton / MinimizeButton.
/// </summary>
public class DesktopWindow : MonoBehaviour
{
    [Header("Identity")]
    [Tooltip("Raises the flag \"opened:<windowId>\" the first time this window opens. Must be unique per window.")]
    public string windowId = "window";
    public string title = "Window";
    public Sprite taskbarIcon;

    [Header("Header (optional - auto-found by name)")]
    [SerializeField] TMP_Text titleText;
    [SerializeField] Button closeButton;
    [SerializeField] Button minimizeButton;
    [SerializeField] Image headerImage;
    [SerializeField] Color headerFocusedColor = new Color(0.20f, 0.35f, 0.75f, 1f);
    [SerializeField] Color headerUnfocusedColor = new Color(0.45f, 0.45f, 0.52f, 1f);

    [Header("Events")]
    public UnityEvent onOpened;
    public UnityEvent onClosed;

    public bool IsOpen { get; private set; }
    public bool IsMinimized { get; private set; }
    public RectTransform Rect => (RectTransform)transform;

    bool started;

    void Awake()
    {
        AutoFindHeaderParts();
        if (closeButton != null) closeButton.onClick.AddListener(Close);
        if (minimizeButton != null) minimizeButton.onClick.AddListener(Minimize);
        SetTitle(title);
        EnsureDragHandle();
    }

    void Start()
    {
        started = true;
        // A window left active in the scene but never "opened" hides itself.
        if (!IsOpen) gameObject.SetActive(false);
    }

    void OnEnable()
    {
        // If something activated this window directly (SetActive(true)) instead of calling Open(),
        // treat it as an Open() so the taskbar button and the opened flag still happen.
        if (started && !IsOpen) Open();
    }

    // ------------------------------------------------------------------ open / close

    public void Open()
    {
        if (!gameObject.scene.IsValid())
        {
            Debug.LogError($"DesktopWindow '{name}' is a PREFAB ASSET, not the copy in the scene. " +
                           "Assign the object from the Hierarchy, not from the Project window.", this);
            return;
        }

        if (IsOpen)
        {
            Focus();
            return;
        }

        IsOpen = true;
        IsMinimized = false;
        gameObject.SetActive(true);

        // Raise the flag FIRST so a taskbar/layout problem can never swallow progress.
        if (string.IsNullOrWhiteSpace(windowId) || windowId == "window")
            Debug.LogWarning($"DesktopWindow '{name}' still has the default Window Id. Give it a unique id.", this);
        GameEvents.Raise("opened:" + windowId);

        var dm = DesktopManager.I;
        if (dm == null) Debug.LogWarning("No DesktopManager found (is 'Systems' in the scene?).", this);
        else if (dm.taskbar != null) dm.taskbar.Register(this);

        Focus();
        onOpened.Invoke();
    }

    public void Close()
    {
        if (!IsOpen) return;

        IsOpen = false;
        IsMinimized = false;
        gameObject.SetActive(false);

        var dm = DesktopManager.I;
        if (dm != null)
        {
            if (dm.taskbar != null) dm.taskbar.Unregister(this);
            dm.OnWindowStateChanged();
        }
        onClosed.Invoke();
    }

    public void Minimize()
    {
        if (!IsOpen || IsMinimized) return;

        IsMinimized = true;
        gameObject.SetActive(false);
        if (DesktopManager.I != null) DesktopManager.I.OnWindowStateChanged();
    }

    public void Restore() => Focus();

    /// <summary>Brings the window to the front (and restores it if minimized).</summary>
    public void Focus()
    {
        if (!IsOpen) return;

        if (IsMinimized)
        {
            IsMinimized = false;
            gameObject.SetActive(true);
        }
        transform.SetAsLastSibling();
        if (DesktopManager.I != null) DesktopManager.I.SetFocused(this);
    }

    public void SetTitle(string newTitle)
    {
        title = newTitle;
        if (titleText != null) titleText.text = newTitle;
    }

    public void SetFocusVisual(bool focused)
    {
        if (headerImage != null)
            headerImage.color = focused ? headerFocusedColor : headerUnfocusedColor;
    }

    // ------------------------------------------------------------------ setup helpers

    void AutoFindHeaderParts()
    {
        if (headerImage == null) headerImage = FindChild<Image>("Header");
        if (closeButton == null && windowId != "popup") closeButton = FindChild<Button>("Header/CloseButton");
        if (minimizeButton == null) minimizeButton = FindChild<Button>("Header/MinimizeButton");
        if (titleText == null) titleText = FindChild<TMP_Text>("Header/Title");
    }

    T FindChild<T>(string path) where T : Component
    {
        var t = transform.Find(path);
        return t != null ? t.GetComponent<T>() : null;
    }

    void EnsureDragHandle()
    {
        if (headerImage == null)
        {
            Debug.LogWarning($"DesktopWindow '{name}': no header found, so it can't be dragged. " +
                             "Name the header object 'Header' (with an Image) or assign Header Image.", this);
            return;
        }
        headerImage.raycastTarget = true;
        if (headerImage.GetComponent<WindowDragHandle>() == null)
            headerImage.gameObject.AddComponent<WindowDragHandle>();
    }
}
