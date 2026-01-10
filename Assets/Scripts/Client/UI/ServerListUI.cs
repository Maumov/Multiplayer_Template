using Assets.Scripts.Server.Bootstrap;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class ServerListUI : MonoBehaviour
{
    public LANDiscovery discovery;
    public Transform contentParent;
    public GameObject serverEntryPrefab;

    List<string> knownServers = new List<string>();

    void Start()
    {
        discovery.OnServerFound += AddServer;
    }

    void AddServer( string ip )
    {
        if ( knownServers.Contains( ip ) )
            return;

        Debug.Log( "Prefab: " + serverEntryPrefab );
        Debug.Log( "ContentParent: " + contentParent );
        knownServers.Add( ip );
        try
        {
            GameObject entry = Instantiate( serverEntryPrefab, contentParent );
            //entry.transform.localScale = Vector3.one;
            //entry.SetActive( true );
            entry.GetComponent<ServerEntry>()?.Init( ip );
        }
        catch ( Exception e )
        {
            Debug.LogError( "Failed to instantiate server entry: " + e );
        }
        Debug.Log( "Adding server entry for IP: " + ip );
    }

    private void OnDisable()
    {
        discovery.OnServerFound -= AddServer;
    }
}
