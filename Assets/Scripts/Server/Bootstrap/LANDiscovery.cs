using System.Collections.Generic;
using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Assets.Scripts.Server.Bootstrap
{
    public class LANDiscovery : MonoBehaviour
    {
        public static LANDiscovery Instance;

        // Evento para avisar cuando se encuentra un servidor
        public event Action<string> OnServerFound;

        [Header( "Broadcast Settings" )]
        public ushort broadcastPort = NetworkConstants.broadcastPort; // Puerto UDP para LAN discovery
        public float broadcastInterval = NetworkConstants.broadcastInterval; // Cada cuánto se envía broadcast (seg)
        public string broadcastMessage = NetworkConstants.broadcastMessage;

        private float timer;
        private List<string> discoveredServers = new List<string>();

        void Awake()
        {
            if ( Instance != null && Instance != this )
            {
                Destroy( gameObject );
                return;
            }
            Instance = this;
            DontDestroyOnLoad( gameObject );
        }

        void Update()
        {
            // Solo el servidor hace broadcast
            if ( NetworkManager.Singleton.IsServer )
            {
                timer += Time.deltaTime;
                if ( timer >= broadcastInterval )
                {
                    BroadcastServer();
                    timer = 0f;
                }
            }

            // Solo el cliente escucha
            if ( NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost )
            {
                ListenForServers();
            }
        }

        // -----------------------
        // Servidor: enviar broadcast
        // -----------------------
        private void BroadcastServer()
        {
            try
            {
                using ( var udp = new System.Net.Sockets.UdpClient() )
                {
                    udp.EnableBroadcast = true;
                    byte[] data = System.Text.Encoding.UTF8.GetBytes( broadcastMessage );
                    udp.Send( data, data.Length, new System.Net.IPEndPoint( System.Net.IPAddress.Broadcast, broadcastPort ) );
                    Debug.LogWarning( "[LANDiscovery] Broadcasting server! " );
                }
            }
            catch ( Exception e )
            {
                Debug.LogWarning( "[LANDiscovery] Broadcast failed: " + e.Message );
            }
        }

        // -----------------------
        // Cliente: escuchar broadcast
        // -----------------------
        private void ListenForServers()
        {
            try
            {
                using ( var udp = new System.Net.Sockets.UdpClient( broadcastPort ) )
                {
                    udp.Client.ReceiveTimeout = 1; // no bloquea
                    while ( udp.Available > 0 )
                    {
                        var remoteEP = new System.Net.IPEndPoint( System.Net.IPAddress.Any, 0 );
                        byte[] data = udp.Receive( ref remoteEP );
                        string msg = System.Text.Encoding.UTF8.GetString( data );

                        if ( msg == broadcastMessage )
                        {
                            string serverIP = remoteEP.Address.ToString();
                            if ( !discoveredServers.Contains( serverIP ) )
                            {
                                discoveredServers.Add( serverIP );
                                Debug.Log( "[LANDiscovery] Server found: " + serverIP );
                                OnServerFound?.Invoke( serverIP );
                            }
                        }
                        Debug.LogWarning( "[LANDiscovery] Listening for servers! " );
                    }
                }
            }
            catch
            {
                // Ignorar timeout
            }
        }

        public void StartClientDiscovery()
        {
            if ( !NetworkManager.Singleton.IsClient )
            {
                Debug.Log( "[LANDiscovery] Client discovery started." );
                // Nothing extra needed; Update() escuchará broadcast
            }
        }

        public void StopClientDiscovery()
        {
            discoveredServers.Clear();
            OnServerFound = null;
        }
    }
}