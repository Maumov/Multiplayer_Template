using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class ConnectedClientsUI : MonoBehaviour
{
    [SerializeField] GameObject clientEntryPrefab;
    [SerializeField] Transform contentParent;

    Dictionary<ulong, GameObject> entries = new();

    void Start()
    {
        // Solo el host muestra esta UI
        if ( !NetworkManager.Singleton.IsServer )
        {
            gameObject.SetActive( false );
            return;
        }

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

        // Host ya existe
        AddClient( NetworkManager.Singleton.LocalClientId );
    }

    void OnDestroy()
    {
        if ( NetworkManager.Singleton == null )
            return;

        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    void OnClientConnected( ulong clientId )
    {
        AddClient( clientId );
    }

    void OnClientDisconnected( ulong clientId )
    {
        if ( !entries.TryGetValue( clientId, out var go ) )
            return;

        Destroy( go );
        entries.Remove( clientId );
    }

    void AddClient( ulong clientId )
    {
        if ( entries.ContainsKey( clientId ) )
            return;

        GameObject entry = Instantiate( clientEntryPrefab, contentParent );
        entry.GetComponent<ClientEntryUI>().Init( clientId, clientId == NetworkManager.ServerClientId );

        entries[ clientId ] = entry;
    }
}