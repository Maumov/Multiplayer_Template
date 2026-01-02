using Unity.Netcode;
using UnityEngine;

namespace Client.Player
{
    public class PlayerInputHandler : NetworkBehaviour
    {
        private ClientPlayerActions actions;

        void Awake()
        {
            actions = GetComponent<ClientPlayerActions>();
        }

        void Update()
        {
            if ( Input.GetMouseButtonDown( 0 ) )
            {
                actions.RequestAttack();
            }
        }
    }
}
