using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
public class NetworkConnect : MonoBehaviour
{
    [SerializeField] private short maxPlayers = 4;
    [SerializeField] private TextMeshProUGUI codeText;
    [SerializeField] private TMP_InputField inputField;
    
    private string joinCode;
    

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed In" + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void Create()
    {
        try
        {
            Debug.Log("Host - Creating an allocation.");

            // Important: Once the allocation is created, you have ten seconds to BIND
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers-1); // takes number of connections allowed as argument. you can add a region argument

            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            codeText.text = joinCode;

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();
        } 
        catch (RelayServiceException e)
        {
            Debug.LogError(e.Message);
        }
    }

    public async void Join()
    {
        try
        {
            joinCode = inputField.text;
            Debug.Log("Joining Relay with " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e.Message);
        }
    }
}
