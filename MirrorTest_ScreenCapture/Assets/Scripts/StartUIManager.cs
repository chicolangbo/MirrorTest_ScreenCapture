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
        if (!NetworkClient.active)
        {
            startHostBtn.onClick.RemoveAllListeners();
            startHostBtn.onClick.AddListener(() => { SetIpAddress(); SetPortId(); manager.StartHost(); DeactivateStartUIPanel(); });

            startClientBtn.onClick.RemoveAllListeners();
            startClientBtn.onClick.AddListener(() => { SetIpAddress(); SetPortId(); manager.StartClient(); DeactivateStartUIPanel(); });
        }
        else
        {
            Debug.Log($"Connecting to {manager.networkAddress}..");
        }
    }

    public void SetIpAddress()
    {
        Debug.Log("StartUIManager : SetIpAddress");
        if (IpAddress != null && IpAddress.text != "")
        {
            manager.networkAddress = IpAddress.text;
            Debug.Log("ip 있음" + manager.networkAddress);
        }
        else
        {
            manager.networkAddress = "localhost";
            Debug.Log("ip 없음" + manager.networkAddress);
        }
    }

    public void SetPortId()
    {
        if (Transport.active is PortTransport portTransport)
        {
            if (ushort.TryParse(portTransport.Port.ToString(), out ushort port))
            {
                portTransport.Port = port;
            }
        }
    }

    public void DeactivateStartUIPanel()
    {
        startUIPanel.SetActive(false);
    }
}