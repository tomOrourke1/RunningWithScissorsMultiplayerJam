using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] Button hostBtn;
    [SerializeField] Button clientBtn;

    [Space]
    [SerializeField] Button lanBtn;
    [SerializeField] Button lanConnectBtn;

    [Space]
    [Header("Relay for connections")]
    [SerializeField] TestRelay relay;
    [SerializeField] TestLobby lobby;

    private void Awake()
    {
        hostBtn.onClick.AddListener(async () =>
        {
            await lobby.CreateLobby();

        });
        clientBtn.onClick.AddListener(async () =>
        {
            await lobby.QuickJoinLobby();
        });


        lanBtn.onClick.AddListener(async () =>
        {
            await lobby.LANStartLobby();
        }); 
        lanConnectBtn.onClick.AddListener(async () =>
        {
            await lobby.LANJoinLobby();
        });




    }



}
