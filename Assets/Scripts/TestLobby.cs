using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class TestLobby : MonoBehaviour
{


    [SerializeField] TestRelay relay;


    private Lobby hostLobby;

    float heartbeatTimer;


    private string playerName;// = "Tom" + Random.Range(10, 100);


    private async void Start()
    {
        playerName = "Tom" + UnityEngine.Random.Range(10, 100);

        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {

            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("Signed in: " + AuthenticationService.Instance.PlayerId);
            };

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }


        Debug.Log("Player Name : " + playerName);
    }


    private async void Update()
    {
        await HandleLobbyHeartbeat();
    }
    private async Task HandleLobbyHeartbeat()
    {
        if (hostLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer <= 0)
            {
                float headbeatTimerMax = 15;
                heartbeatTimer = headbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }


    public async Task CreateLobby()
    {
        try
        {
            string lobbyName = "MyLobby";
            int maxPlayers = 4;


            string joinCode = await relay.CreateRelay();

            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    {"GameMode", new DataObject(DataObject.VisibilityOptions.Public, "CaptureTheFlag") },
                    {"JoinCode", new DataObject(DataObject.VisibilityOptions.Member, joinCode) }
                }
            };

            hostLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

            Debug.Log("Created Lobby! " + hostLobby.Name + " " + hostLobby.MaxPlayers + " " + hostLobby.Id + " " + hostLobby.LobbyCode);

            PrintPlayers(hostLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async Task ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryOptions = new QueryLobbiesOptions
            {
                Count = 25,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder>{
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };

            var queryResponce = await Lobbies.Instance.QueryLobbiesAsync(queryOptions);

            Debug.Log("Lobbies found: " + queryResponce.Results.Count);
            foreach (var lob in queryResponce.Results)
            {
                Debug.Log(lob.Name + " - " + lob.MaxPlayers);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }

    }

    public async Task JoinLobby()
    {
        try
        {
            var queryResponce = await Lobbies.Instance.QueryLobbiesAsync();


            await Lobbies.Instance.JoinLobbyByIdAsync(queryResponce.Results[0].Id);
            Debug.Log("Joined Lobby! " + queryResponce.Results[0].Name + " " + queryResponce.Results[0].AvailableSlots);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async Task JoinLobbyByCode(string code)
    {
        try
        {

            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()
            };

            var lob = await Lobbies.Instance.JoinLobbyByCodeAsync(code, options);
            Debug.Log("Joined Lobby with code: " + lob.LobbyCode);
            PrintPlayers(lob);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
                    {
                        {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
                    }
        };
    }


    public async Task QuickJoinLobby()
    {
        try
        {
            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions
            {
                Player = GetPlayer()
            };

            var lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
            Debug.Log("Quickly Joined lobby");
            PrintPlayers(lobby);


            await relay.JoinRelay(lobby.Data["JoinCode"].Value);

            Debug.Log("Lobby Data: " + lobby.Data["GameMode"].Value + " -- " + lobby.Data["JoinCode"].Value);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }


    private void PrintPlayers(Lobby lobby)
    {
        Debug.Log("Players in lobby " + lobby.Name);
        foreach (Player player in lobby.Players)
        {
            Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);
        }
    }



    private async void UpdateLobbyGameMode(string gamemode)
    {
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
            {
                {"GameMode", new DataObject(DataObject.VisibilityOptions.Public, gamemode)}
            }
            });

        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }


    }




    public async Task LANStartLobby()
    {
        try
        {
            string lobbyName = "MyLobby";
            int maxPlayers = 4;

            string hostName = Dns.GetHostName();

            hostName = Dns.GetHostName();
            IPHostEntry myIP = Dns.GetHostEntry(hostName);
            IPAddress[] address = myIP.AddressList;
            string ip = address[1].ToString();


            //for(int i = 0; i < address.Length; ++i)
            //{
            //    Debug.Log("Address: " + address[i]);
            //}


            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    {"IP", new DataObject(DataObject.VisibilityOptions.Public, ip) },
                }
            };

            hostLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

            Debug.Log("Created Lobby! " + hostLobby.Name + " " + hostLobby.MaxPlayers + " " + hostLobby.Id + " " + hostLobby.LobbyCode);

            PrintPlayers(hostLobby);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ip, (ushort)9000);

            NetworkManager.Singleton.StartHost();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public async Task LANJoinLobby()
    {
        try
        {
            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions
            {
                Player = GetPlayer()
            };

            var lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
            Debug.Log("Quickly Joined lobby");
            PrintPlayers(lobby);


            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(lobby.Data["IP"].Value, (ushort)9000);


            NetworkManager.Singleton.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }
    }

}
