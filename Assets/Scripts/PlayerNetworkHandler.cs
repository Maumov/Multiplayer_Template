
using Unity.Netcode;

public class PlayerNetworkHandler : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        // Solo para el objeto local
        if ( IsOwner )
        {
           DontDestroyOnLoad( gameObject ); // mantiene este player al cambiar de escena
        }
    }
}