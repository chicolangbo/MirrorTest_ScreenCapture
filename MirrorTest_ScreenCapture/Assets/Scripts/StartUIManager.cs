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

    private void Update()
    {
        if (NetworkClient.isConnected && !NetworkClient.ready)
        {
            // client ready
            NetworkClient.Ready();
            //if (NetworkClient.localPlayer == null)
            //    NetworkClient.AddPlayer();
        }

        if(IpAddress.text.Length >= 14)
        {
            manager.networkAddress = IpAddress.text.Trim();
            startClientBtn.interactable = true;
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
        Debug.Log(IpAddress.text.Length);
        if (IpAddress != null && IpAddress.text.Length != 1)
        {
            manager.networkAddress = IpAddress.text;
            Debug.Log("ip ����" + manager.networkAddress + ".");
        }
        else
        {
            Debug.Log("ip ����" + manager.networkAddress);
        }
    }

    public void SetPortId()
    {
        if (Transport.active is PortTransport portTransport)
        {
            if (ushort.TryParse(portTransport.Port.ToString(), out ushort port))
            {
                portTransport.Port = port;
                Debug.Log("SetPortID");
            }
        }
    }

    public void DeactivateStartUIPanel()
    {
        startUIPanel.SetActive(false);
    }
}