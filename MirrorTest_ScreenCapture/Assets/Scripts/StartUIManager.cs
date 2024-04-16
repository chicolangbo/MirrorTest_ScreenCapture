using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartUIManager : MonoBehaviour
{
    public GameObject startUIPanel;
    public TextMeshProUGUI IpAddress;
    public TextMeshProUGUI userName;
    public Button startHostBtn;
    public Button startClientBtn;

    public NetworkManager manager;

    private void Awake()
    {
        if(!NetworkClient.isConnected && !NetworkServer.active)
        {
            StartButtons();
        }
    }

    public void StartButtons()
    {
        if(!NetworkClient.active)
        {
            startHostBtn.onClick.RemoveAllListeners();
            startHostBtn.onClick.AddListener(() => { manager.StartHost(); DeactivateStartUIPanel(); });

            startClientBtn.onClick.RemoveAllListeners();
            startClientBtn.onClick.AddListener(() => { manager.StartClient(); DeactivateStartUIPanel(); });

            if(IpAddress != null && IpAddress.text != "")
            {
                Debug.Log("ip 있음" + IpAddress.text);
                manager.networkAddress = IpAddress.text;
            }
            else
            {
                manager.networkAddress = "localhost";
                Debug.Log("ip 없음" + manager.networkAddress);
            }

            if(Transport.active is PortTransport portTransport)
            {
                if(ushort.TryParse(portTransport.Port.ToString(), out ushort port))
                {
                    portTransport.Port = port;
                }
            }
        }
    }

    public void DeactivateStartUIPanel()
    {
        startUIPanel.SetActive(false);
    }
}