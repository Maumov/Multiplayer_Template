using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Assets.Scripts.Server.Bootstrap
{
    public class LANDiscovery : MonoBehaviour
    {
        public int port = 47777;
        public float interval = 1f;
        public string broadcastMsg = "UNITY_LAN_SERVER";

        public Action<string> OnServerFound;

        UdpClient sender;
        UdpClient listener;
        float timer;
        bool listening;

        void Awake()
        {
            StartListener();
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

                Debug.Log( "[LANDiscovery] Listening on UDP " + port );
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
                return;

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
                    OnServerFound?.Invoke( ep.Address.ToString() );
                    Debug.Log( "[LANDiscovery] Server found: " + ep.Address );
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
            foreach ( var ip in Dns.GetHostEntry( Dns.GetHostName() ).AddressList )
            {
                if ( ip.AddressFamily == AddressFamily.InterNetwork )
                {
                    var b = ip.GetAddressBytes();
                    b[ 3 ] = 255;
                    list.Add( new IPAddress( b ) );
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