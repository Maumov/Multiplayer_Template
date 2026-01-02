using UnityEngine;
using Unity.Netcode;

namespace Assets.Scripts.Server.Bootstrap
{
    public class HostBootstrap : MonoBehaviour
    {
        void Start()
        {
            Debug.Log( "[HOST] Starting as Host..." );
            NetworkManager.Singleton.StartHost();
        }
    }
}