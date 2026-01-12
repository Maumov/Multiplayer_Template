using Client.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    PlayerController playerController;
    [SerializeField] EntityWorldUI entityWorldUI;

    public void Init( PlayerController owner )
    {
        playerController = owner;

        playerController.OnHealthChange += UpdateEntityUI;
        UpdateEntityUI();
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
