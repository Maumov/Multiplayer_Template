using Client.Player;
using Unity.Netcode;
using UnityEngine;

public class EntityController : NetworkBehaviour
{
    [SerializeField] EntityWorldUI entityWorldUI;

    [SerializeField] CharacterStats CharacterStats;
    
    public delegate void EntityControllerDelegate();
    public event EntityControllerDelegate OnHealthChange;
    public override void OnNetworkSpawn()
    {
        UpdateEntityUI();
    }

    public void SetInitToCharacter( int health )
    {
        CharacterStats.SetStats( health );
    }

    private void Start()
    {
        OnHealthChange += UpdateEntityUI;
    }


    #region RECEIVE DAMAGE
    void UpdateEntityUI()
    {
        entityWorldUI.UpdateText( $"{CharacterStats.currentHealth.Value}/{CharacterStats.maxHealth.Value}" );
    }
    public void TakeDamage( int damage )
    {
        Debug.Log( $"Start" );
        Debug.Log( $"Take Damage in Entity Controller" );
        damage = damage > 0 ? -damage : damage; //So is a negative value
        
        CharacterStats.UpdateHealth( damage );

        OnHealthChange?.Invoke();

        Debug.Log( $"Take Damage End" );
    }
    #endregion
}
