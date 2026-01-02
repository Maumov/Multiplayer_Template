using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class GameLauncher : MonoBehaviour
{
    [SerializeField] private string serverSceneName = "ServerScene";
    [SerializeField] private string clientSceneName = "ClientScene";

    void Start()
    {
        if ( Application.isBatchMode )
        {
            // Modo headless → servidor dedicado
            Debug.Log( "[SERVER] Starting server launcher..." );
            SceneManager.LoadScene( serverSceneName );
        }
        else
        {
            // Modo normal → cliente o host
            Debug.Log( "[CLIENT] Starting client launcher..." );
            SceneManager.LoadScene( clientSceneName );
        }
    }
}
