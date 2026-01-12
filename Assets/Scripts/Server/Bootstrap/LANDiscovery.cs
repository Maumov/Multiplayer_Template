using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Net.NetworkInformation;

namespace Assets.Scripts.Server.Bootstrap
{
    public class LANDiscovery : MonoBehaviour
    {
        public int port = 47777;
        public float interval = 1f;
        public string broadcastMsg = "UNITY_LAN_SERVER";
        string localIP;
        public Action<string> OnServerFound;

        UdpClient sender;
        UdpClient listener;
        float timer;
        bool listening;
        private List<string> pendingServers = new List<string>();

        void Awake()
        {
            localIP = GetLocalIPv4();
            StartListener();
        }
        string GetLocalIPv4()
        {
            foreach ( var ip in Dns.GetHostEntry( Dns.GetHostName() ).AddressList )
            {
                if ( ip.AddressFamily == AddressFamily.InterNetwork )
                    return ip.ToString();
            }
            return "";
        }
        void StartListener()
        {
            try
            {
                listener = new UdpClient();

                listener.Client.SetSocketOption( SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true );
                listener.ExclusiveAddressUse = false;
                listener.Client.Bind( new IPEndPoint( IPAddress.Any, port ) );

                listener.BeginReceive( OnReceive, null );
                listening = true;

                //Debug.Log( "[LANDiscovery] Listening on UDP " + port );
            }
            catch ( Exception e )
            {
                Debug.LogError( "[LANDiscovery] Listener failed: " + e.Message );
            }
        }

        /*
        void Start()
        {
            listener = new UdpClient( port );
            listener.BeginReceive( OnReceive, null );
        }
        */
        void Update()
        {
            if ( !NetworkManager.Singleton.IsServer )
            {
                if ( pendingServers.Count > 0 )
                {
                    foreach ( var ip in pendingServers )
                    {
                        OnServerFound?.Invoke( ip ); // ahora corre en thread principal
                    }
                    pendingServers.Clear();
                }
            }
            else
            {
                if ( sender == null )
                {
                    sender = new UdpClient();
                    sender.EnableBroadcast = true;
                }

                timer += Time.deltaTime;
                if ( timer >= interval )
                {
                    Broadcast();
                    timer = 0;
                }
            }
        }

        void Broadcast()
        {
            byte[] data = Encoding.UTF8.GetBytes( broadcastMsg );

            foreach ( var ip in GetBroadcastIPs() )
                sender.Send( data, data.Length, new IPEndPoint( ip, port ) );
        }

        void OnReceive( IAsyncResult ar )
        {
            if ( !listening )
                return;

            try
            {
                IPEndPoint ep = new IPEndPoint( IPAddress.Any, 0 );
                byte[] data = listener.EndReceive( ar, ref ep );

                string msg = Encoding.UTF8.GetString( data );
                if ( msg == broadcastMsg )
                {
                    string ip = ep.Address.ToString();
                    /*
                    if ( ip == localIP )
                        return; // ignora self
                    */
                    pendingServers.Add( ip ); // solo agrega a la lista
                    //Debug.Log( "[LANDiscovery] Server found: " + ep.Address );
                }
            }
            catch ( SocketException )
            {
                // ignorar errores UDP normales
            }
            finally
            {
                listener.BeginReceive( OnReceive, null );
            }
        }

        IPAddress[] GetBroadcastIPs()
        {
            var list = new System.Collections.Generic.List<IPAddress>();

            foreach ( NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces() )
            {
                if ( ni.OperationalStatus != OperationalStatus.Up )
                    continue;

                foreach ( UnicastIPAddressInformation ua in ni.GetIPProperties().UnicastAddresses )
                {
                    if ( ua.Address.AddressFamily != AddressFamily.InterNetwork )
                        continue;

                    byte[] ip = ua.Address.GetAddressBytes();
                    byte[] mask = ua.IPv4Mask.GetAddressBytes();

                    byte[] broadcast = new byte[ 4 ];
                    for ( int i = 0 ; i < 4 ; i++ )
                        broadcast[ i ] = ( byte ) ( ip[ i ] | ( ~mask[ i ] ) );

                    list.Add( new IPAddress( broadcast ) );
                }
            }

            return list.ToArray();
        }

        void OnDestroy()
        {
            listening = false;
            listener?.Close();
            sender?.Close();
        }
    }
}