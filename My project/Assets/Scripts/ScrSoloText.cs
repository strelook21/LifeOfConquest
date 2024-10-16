using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ScrSoloText : MonoBehaviour
{
    public TextMeshProUGUI textbox;
    public ScrSoloLogic logic;

    public void Update()
    {
        textbox.text = (logic.active) ? "Active" : "Paused";
        textbox.text += "; Step = " + logic.period + " seconds";
    }

}
