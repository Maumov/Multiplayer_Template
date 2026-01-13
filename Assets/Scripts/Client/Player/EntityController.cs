using Client.Player;
using Unity.Netcode;
using UnityEngine;

public class EntityController : NetworkBehaviour
{
    PlayerController playerController;
    [SerializeField] EntityWorldUI entityWorldUI;

    public override void OnNetworkSpawn()
    {
        if ( !IsOwner )
            return;

        NetworkObject playerObject = NetworkManager.Singleton.ConnectedClients[ OwnerClientId ].PlayerObject;

        playerController = playerObject.GetComponent<PlayerController>();
        playerController.SetCharacterInstance( gameObject );
        //TODO CAMBIAR EL UpdateEntityUI...
        //aqui estoy subscrito al evento de playerController y no debe hacerse asi...
        if ( playerController != null)
        {
            playerController.OnHealthChange += UpdateEntityUI;
            UpdateEntityUI();
        }
    }

    public void TakeDamage( int amount )
    {
        Debug.Log( $"Take Damage in Entity Controller" );
        if ( playerController != null )
        {
            playerController.TakeDamage( amount  );
        }
    }

    void UpdateEntityUI()
    {
        entityWorldUI.UpdateText( $"{playerController.health.Value}" );
    }
}
