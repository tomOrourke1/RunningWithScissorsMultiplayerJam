using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using System;
using System.Linq.Expressions;


public class TestRelay : MonoBehaviour
{


    UnityTransport transport;

    private /*async*/ void Start()
    {
        //      await UnityServices.InitializeAsync();

        //      AuthenticationService.Instance.SignedIn += () =>
        //      {
        //          Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        //      };

        //      if (!AuthenticationService.Instance.IsSignedIn)
        //      {
        //          await AuthenticationService.Instance.SignInAnonymouslyAsync();
        //      }

        transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

    }


    public async Task<string> CreateRelay()
    {
        try
        {
            var allocation = await RelayService.Instance.CreateAllocationAsync(5);

            //   RelayServerData serverData = new RelayServerData(ell, "dtls");
            //   NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);

            RelayServer server = new RelayServer(allocation.RelayServer.IpV4, allocation.RelayServer.Port);
            

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(server.IpV4, (ushort)server.Port);


            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
                );

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log("Join Code: " + joinCode);


            NetworkManager.Singleton.StartHost();


            return joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);

            return default;
        }

    }


    public async Task<int> JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log("Length: " + joinCode.Length);
            Debug.Log("Joining Relay with: " + joinCode + ".");
            JoinAllocation join = await RelayService.Instance.JoinAllocationAsync(joinCode);


            //  RelayServerData relayServerData = new RelayServerData(join, "dtls");

            // NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(join.RelayServer.IpV4, (ushort)join.RelayServer.Port);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                join.RelayServer.IpV4,
                (ushort)join.RelayServer.Port,
                join.AllocationIdBytes,
                join.Key,
                join.ConnectionData,
                join.HostConnectionData
                );


            NetworkManager.Singleton.StartClient();
            return 0;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
            return 1;
        }
    }

}



/*
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                ell.RelayServer.IpV4,
                (ushort)ell.RelayServer.Port,
                ell.AllocationIdBytes,
                ell.Key,
                ell.ConnectionData
                );
            */


/*            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                join.RelayServer.IpV4,
                (ushort)join.RelayServer.Port,
                join.AllocationIdBytes,
                join.Key,
                join.ConnectionData,
                join.ConnectionData
                );
            */