using Assets.Scripts.Server.Bootstrap;
using System.Collections.Generic;
using UnityEngine;

public class ServerListUI : MonoBehaviour
{
    public LANDiscovery discovery;
    public Transform contentParent;
    public GameObject serverEntryPrefab;

    HashSet<string> knownServers = new HashSet<string>();

    void Start()
    {
        discovery.OnServerFound += AddServer;
    }

    void AddServer( string ip )
    {
        if ( knownServers.Contains( ip ) )
            return;

        knownServers.Add( ip );

        GameObject entry = Instantiate( serverEntryPrefab, contentParent );
        entry.GetComponent<ServerEntry>().Init( ip );
    }
}
