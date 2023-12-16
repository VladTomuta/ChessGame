using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class ChessRelay : MonoBehaviour
{ 

    public static ChessRelay Instance { get; private set; }
    
    private TMP_Text debugText;

    void Awake() {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        debugText = GameObject.FindGameObjectWithTag("Debug").GetComponent<TMP_Text>();
    }

    // private async void Start()
    // {
    //     await UnityServices.InitializeAsync();

    //     AuthenticationService.Instance.SignedIn += () => {
    //         Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
    //     };

    //     await AuthenticationService.Instance.SignInAnonymouslyAsync();

    //     //CreateRelay();
    // }

    public async Task<string> CreateRelay() {
        try {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("Relay join code is " + joinCode);
            debugText.text = "Relay join code is " + joinCode;

            return joinCode;

            // NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
            //     allocation.RelayServer.IpV4,
            //     (ushort)allocation.RelayServer.Port,
            //     allocation.AllocationIdBytes,
            //     allocation.Key,
            //     allocation.ConnectionData
            // );

        } catch (RelayServiceException e) {
            Debug.Log(e);
            return null;
        }
        
    }

    public async Task<string> StartHostWithRelay(int maxConnections=1)
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
        var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        Debug.Log("Relay join code is " + joinCode);
        debugText.text = "Relay join code is " + joinCode;
        return NetworkManager.Singleton.StartHost() ? joinCode : null;
    }

    public async Task<bool> StartClientWithRelay(string joinCode)
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        Debug.Log("Joining relay with code: " + joinCode);
        debugText.text = "Joining relay with code: " + joinCode;
        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
        return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
    }

    public async void JoinRelay(string joinCode) {
        try {
            Debug.Log("Joining relay with code: " + joinCode);
            debugText.text = "Joining relay with code: " + joinCode;

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            // NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
            //     joinAllocation.RelayServer.IpV4,
            //     (ushort)joinAllocation.RelayServer.Port,
            //     joinAllocation.AllocationIdBytes,
            //     joinAllocation.Key,
            //     joinAllocation.ConnectionData,
            //     joinAllocation.HostConnectionData                
            // );

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

        } catch (RelayServiceException e) {
            Debug.Log(e);
        }
        
    }

}
