using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class EmailMessage
{
    [Tooltip("Unique id. Reading it raises the flag \"email_read:<id>\".")]
    public string id;
    public string sender;
    public string subject;
    public string timeLabel = "4:28 PM";
    [TextArea(4, 14)] public string body;
    [Tooltip("Untick for emails that arrive later via EmailApp.I.Deliver(id).")]
    public bool deliveredAtStart = true;

    [NonSerialized] public bool read;
    [NonSerialized] public bool delivered;
}

/// <summary>
/// Inbox logic. Put this on an ALWAYS-ACTIVE object (e.g. "Systems") - not on the window -
/// so the unread badge on the desktop icon is correct before the window is ever opened.
/// </summary>
public class EmailApp : MonoBehaviour
{
    public static EmailApp I { get; private set; }

    [Header("Window")]
    [SerializeField] DesktopWindow window;

    [Header("Inbox (list oldest first - the newest ends up on top)")]
    [SerializeField] List<EmailMessage> emails = new List<EmailMessage>();

    [Header("List view")]
    [SerializeField] GameObject listPanel;
    [SerializeField] Transform listContent;
    [SerializeField] EmailListItem itemPrefab;

    [Header("Read view")]
    [SerializeField] GameObject readPanel;
    [SerializeField] TMP_Text readSender;
    [SerializeField] TMP_Text readSubject;
    [SerializeField] TMP_Text readTime;
    [SerializeField] TMP_Text readBody;
    [SerializeField] Button backButton;

    [Header("Unread badge (the red circle on the desktop icon)")]
    [SerializeField] GameObject badgeRoot;
    [SerializeField] TMP_Text badgeText;

    readonly Dictionary<string, EmailListItem> items = new Dictionary<string, EmailListItem>();

    public int UnreadCount
    {
        get
        {
            int n = 0;
            foreach (var m in emails)
                if (m.delivered && !m.read) n++;
            return n;
        }
    }

    void Awake()
    {
        I = this;
        if (backButton != null) backButton.onClick.AddListener(ShowList);
        if (window != null) window.onOpened.AddListener(ShowList);   // always reopen on the inbox
    }

    void Start()
    {
        foreach (var m in emails)
            if (m.deliveredAtStart) AddToInbox(m);

        ShowList();
        UpdateBadge();
    }

    /// <summary>Delivers a not-yet-delivered email by id (e.g. Dana's final email at the end).</summary>
    public void Deliver(string id)
    {
        var m = emails.Find(e => e.id == id);
        if (m != null && !m.delivered) AddToInbox(m);
    }

    void AddToInbox(EmailMessage m)
    {
        m.delivered = true;
        var item = Instantiate(itemPrefab, listContent);
        item.transform.SetAsFirstSibling();
        item.Bind(m, this);
        items[m.id] = item;
        UpdateBadge();
        GameEvents.Raise("email_received:" + m.id);
    }

    public void OpenEmail(EmailMessage m)
    {
        readSender.text = m.sender;
        readSubject.text = m.subject;
        readTime.text = m.timeLabel;
        readBody.text = m.body;

        listPanel.SetActive(false);
        readPanel.SetActive(true);

        if (!m.read)
        {
            m.read = true;
            if (items.TryGetValue(m.id, out var item)) item.Refresh();
            UpdateBadge();
            GameEvents.Raise("email_read:" + m.id);
        }
    }

    public void ShowList()
    {
        readPanel.SetActive(false);
        listPanel.SetActive(true);
    }

    void UpdateBadge()
    {
        int n = UnreadCount;
        if (badgeRoot != null) badgeRoot.SetActive(n > 0);
        if (badgeText != null) badgeText.text = n.ToString();
    }
}
