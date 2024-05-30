using SteveRogers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MailItem : MonoBehaviour
{
    public TextMeshProUGUI txtTitle;
    public TextMeshProUGUI txtButton;
    public DataMail data;
    bool isRead = false;

    public void SetData(DataMail data, bool isRead) {
        this.data = data;
        this.isRead = isRead;

        this.txtTitle.text = data.title;
        if (isRead)
        {
            this.txtButton.text = TextMan.Get("Mail archived");
        }
        else {
            this.txtButton.text = TextMan.Get("Read");
        }
    }

    public void ShowMail() {
        PopupMailDetail._data = data;
        PopupMailDetail.isRead = isRead;
        PopupMailDetail.Show();
        PopupMailbox.Hide();
    }
}
