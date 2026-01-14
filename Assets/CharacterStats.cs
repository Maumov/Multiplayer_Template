using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterStats : NetworkBehaviour, IStats
{
    public NetworkVariable<int> maxHealth = new NetworkVariable<int>( 100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server );
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>( 100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server );


    public void SetStats( int _maxHealth )
    {
        maxHealth.Value = _maxHealth;   
        currentHealth.Value = maxHealth.Value;
    }

    public void UpdateHealth( int amount )
    {
        Debug.Log("Health Updated");
        currentHealth.Value += amount;
    }
}
