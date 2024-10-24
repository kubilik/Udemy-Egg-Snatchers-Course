using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IPScanner : MonoBehaviour, ILocalGameStateListener
{
    [Header(" Elements ")]
    [SerializeField] private IPButton ipButtonPrefab;
    [SerializeField] private Transform ipButtonsParent;

    [Header(" Settings ")]
    private List<Ping> pings = new List<Ping>();
    private float timer;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Scan()
    {
        // 192.168.1.4
        // 192.168.1.0 - 192.168.1.255

        StopAllCoroutines();

        timer = 0;

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

            timer += Time.deltaTime;

            if (timer >= 15)
            {
                allDone = true;
                Debug.Log("Timeout");
            }

            yield return null;
        }

        Debug.Log("Scan Completed.");
    }

    private void IPFound(string ip)
    {
        for (int i = 0; i < ipButtonsParent.childCount; i++)
        {
            IPButton childButton = ipButtonsParent.GetChild(i).GetComponent<IPButton>();

            if (childButton.IP == ip)
                return;
        }

        IPButton button = Instantiate(ipButtonPrefab, ipButtonsParent);
        button.Configure(ip);
    }

    public void GameStateChangedCallback(LocalGameState gameState)
    {
        if (gameState != LocalGameState.Scanning)
            return;

        Scan();
    }
}
