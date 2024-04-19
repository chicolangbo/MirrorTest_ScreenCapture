using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogManager : MonoBehaviour
{
    public TextMeshProUGUI log;

    //public void ShowTransparencyOfSendingTexture(Color pixel)
    //{
    //    var textureTransParency = pixel.a;
    //    var additionalText = $"texture Transparency : {textureTransParency}";
    //    AddText(additionalText);
    //}

    public void AddText(string newText)
    {
        if(log.text != "")
        {
            log.text = log.text + "\n" + newText;
        }
        else
        {
            log.text = newText;
        }
    }
}
