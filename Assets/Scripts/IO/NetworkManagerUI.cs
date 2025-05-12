using Unity.Netcode;
using UnityEngine;

public class NetworkManagerUI : MonoBehaviour
{
    public void OnClickStartHost()
    {
        NetworkManager.Singleton.StartHost();
        gameObject.SetActive(false);
    }

    public void OnClickStartClient()
    {
        NetworkManager.Singleton.StartClient();
        gameObject.SetActive(false);
    }
}
