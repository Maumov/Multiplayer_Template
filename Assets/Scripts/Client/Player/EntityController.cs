using Client.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityController : MonoBehaviour
{

    PlayerController playerController;

    public void Init( PlayerController owner )
    {
        playerController = owner;
    }

    public void TakeDamage( int amount )
    {
        if ( playerController != null )
        {
            playerController.TakeDamage( amount  );
        }
        
    }

}
