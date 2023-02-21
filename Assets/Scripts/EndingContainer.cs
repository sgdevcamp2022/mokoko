using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndingContainer : MonoBehaviour
{
    public TMP_Text teamName;
    public TMP_Text clearTime;

    public void Setter(string TeamName, string ClearTime, TextAlignmentOptions align = TextAlignmentOptions.Left) {
        teamName.text = TeamName;
        clearTime.text = ClearTime;
        teamName.alignment = align;
        clearTime.alignment = align;

    }
}