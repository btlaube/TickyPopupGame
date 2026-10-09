using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// WorkHub: a login screen, then a main screen with a button that opens the Shared Folder
/// in its own window, plus the submission tray (see SubmissionTray).
/// Flags: workapp_login (and "opened:sharedfolder" comes from the folder window's own Window Id).
/// Untick Require Login to skip the login screen while testing.
/// </summary>
public class WorkAppController : MonoBehaviour
{
    [Header("Login")]
    [SerializeField] bool requireLogin = true;
    [SerializeField] string expectedUser = "alex";
    [SerializeField] string expectedPassword = "sunflower42";
    [SerializeField] GameObject loginPanel;
    [SerializeField] TMP_InputField userField;
    [SerializeField] TMP_InputField passField;
    [SerializeField] Button loginButton;
    [SerializeField] TMP_Text loginError;

    [Header("Main screen")]
    [SerializeField] GameObject mainPanel;
    [SerializeField] Button openFolderButton;
    [SerializeField] DesktopWindow sharedFolderWindow;

    public bool LoggedIn { get; private set; }

    void Awake()
    {
        LoggedIn = !requireLogin;

        if (loginButton != null) loginButton.onClick.AddListener(TryLogin);
        if (passField != null) passField.onSubmit.AddListener(_ => TryLogin());
        if (userField != null) userField.onSubmit.AddListener(_ => TryLogin());

        if (openFolderButton == null)
            Debug.LogWarning("WorkApp: 'Open Folder Button' isn't assigned.", this);
        else
            openFolderButton.onClick.AddListener(OpenSharedFolder);

        if (loginError != null) loginError.text = "";
        Refresh();
        if (LoggedIn) GameEvents.Raise("workapp_login");
    }

    void OpenSharedFolder()
    {
        if (sharedFolderWindow == null)
        {
            Debug.LogError("WorkApp: 'Shared Folder Window' isn't assigned.", this);
            return;
        }
        sharedFolderWindow.Open();
    }

    void TryLogin()
    {
        bool userOk = string.Equals(userField.text.Trim(), expectedUser, StringComparison.OrdinalIgnoreCase);
        bool passOk = passField.text == expectedPassword;

        if (userOk && passOk)
        {
            LoggedIn = true;
            loginError.text = "";
            Refresh();
            GameEvents.Raise("workapp_login");
        }
        else
        {
            loginError.text = "Incorrect username or password.";
            passField.text = "";
        }
    }

    void Refresh()
    {
        if (loginPanel != null) loginPanel.SetActive(!LoggedIn);
        if (mainPanel != null) mainPanel.SetActive(LoggedIn);
    }
}
