using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>One row in the inbox. Unread = bold text + dot; read = normal text, no dot.</summary>
public class EmailListItem : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] TMP_Text senderText;
    [SerializeField] TMP_Text subjectText;
    [SerializeField] TMP_Text timeText;
    [SerializeField] GameObject unreadDot;
    [SerializeField] Image background;
    [SerializeField] Color unreadColor = new Color(1f, 1f, 1f, 1f);
    [SerializeField] Color readColor = new Color(0.88f, 0.88f, 0.9f, 1f);

    EmailMessage msg;
    EmailApp app;

    public void Bind(EmailMessage m, EmailApp owner)
    {
        msg = m;
        app = owner;
        senderText.text = m.sender;
        subjectText.text = m.subject;
        timeText.text = m.timeLabel;
        Refresh();
    }

    public void Refresh()
    {
        bool unread = !msg.read;
        if (unreadDot != null) unreadDot.SetActive(unread);
        var style = unread ? FontStyles.Bold : FontStyles.Normal;
        senderText.fontStyle = style;
        subjectText.fontStyle = style;
        if (background != null) background.color = unread ? unreadColor : readColor;
    }

    public void OnPointerClick(PointerEventData e)
    {
        if (app != null) app.OpenEmail(msg);
    }
}
