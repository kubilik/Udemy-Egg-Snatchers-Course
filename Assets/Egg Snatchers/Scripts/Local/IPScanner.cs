using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IPScanner : MonoBehaviour, ILocalGameStateListener
{
    private List<Ping> pings = new List<Ping>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Scan()
    {
        // 192.168.1.4
        // 192.168.1.0 - 192.168.1.255

        pings.Clear();

        string localIp = NetworkUtilities.GetLocalIPv4();
        string subnetMask = NetworkUtilities.GetSubnetMask(localIp);

        List<string> ipAddresses = NetworkUtilities.GetIPRange(localIp, subnetMask);

        Debug.Log("Scanning Network...");

        foreach (string ipAdress in ipAddresses)
            pings.Add(new Ping(ipAdress));

        StartCoroutine(CheckPingsCoroutine());
    }

    IEnumerator CheckPingsCoroutine()
    {
        bool allDone = false;

        while (!allDone)
        {
            allDone = true;

            for (int i = pings.Count - 1; i >= 0; i--)
            {
                Ping ping = pings[i];

                if (ping.isDone)
                {
                    if (ping.time >= 0)
                    {
                        IPFound(ping.ip);
                        pings.RemoveAt(i);
                    }
                }
                else
                    allDone = false;
            }
            yield return null;
        }
    }

    private void IPFound(string ip)
    {
        Debug.Log(ip);
    }

    public void GameStateChangedCallback(LocalGameState gameState)
    {
        if (gameState != LocalGameState.Scanning)
            return;

        Scan();
    }
}
