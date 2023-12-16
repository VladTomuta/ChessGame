using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class ChessLobby : MonoBehaviour
{
    public static ChessLobby Instance { get; private set; }
    public string KEY_START_GAME { get; private set; } = "StartGameKey";

    private Lobby joinedLobby;
    private Lobby hostLobby;

    private bool gameHasStarted = false;
    

    private const float heartbeatTimerMax = 15;
    private const float lobbyUpdateTimerMax = 1.1f;
    private float lobbyUpdateTimer = 0f;

    private float heartbeatTimer;

    private TMP_Text debugText;

    private void Awake() {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        debugText = GameObject.FindGameObjectWithTag("Debug").GetComponent<TMP_Text>();

        InitializaUnityAuthentication();
    }

    private void Update() {
        HandleLobbyHeartbeat();
        HandleLobbyPolling();
    }

    private void HandleLobbyHeartbeat() {
        if (hostLobby != null) {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f) {
                heartbeatTimer = heartbeatTimerMax;

                Debug.Log("heartbeat sent to lobby");
                LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
                HeartbeatLobbyCoroutine(hostLobby.Id, 0);
            }
        }
    }

    private async void InitializaUnityAuthentication() {
        if (UnityServices.State != ServicesInitializationState.Initialized) {
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(Random.Range(0, 10000).ToString());

            await UnityServices.InitializeAsync(initializationOptions);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        PrintLobbies();

        //CheckForLobbyWithEmptySlot();

        QuickJoinLobby();

        // if (await CheckForLobbyWithEmptySlot()) {
        //     Debug.Log("A player is waiting for an opponent. Connecting with him");
        //     QuickJoinLobby();
        // } else {
        //     Debug.Log("Creating lobby");
        //     CreateLobby("MyLobby", false);
        // }
        
    }

    public async void CreateLobby(string lobbyName, bool isPrivate) {
        try {
            Debug.Log(AuthenticationService.Instance.PlayerId);
            hostLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 2, new CreateLobbyOptions {
                IsPrivate = isPrivate,
                Data = new Dictionary<string, DataObject> {
                    { KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, "0") }
                }
            });

            joinedLobby = hostLobby;

            Debug.Log("Created lobby: " + hostLobby.Id);
            debugText.text = "Created lobby: " + hostLobby.Id;


            //NetworkManager.Singleton.StartHost();

        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    public async void QuickJoinLobby() {
        try {
            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            Debug.Log("Joined lobby: " + joinedLobby.Id);
            debugText.text = "Joined lobby: " + joinedLobby.Id;

            //StartGame();
        } catch (LobbyServiceException e) {
            CreateLobby("MyLobby", false);
        }
    }

    IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);

        Debug.Log("heartbeat sent from coroutine");
        LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
        yield return delay;

    }

    public async void PrintLobbies() {
        try {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            Debug.Log("Lobbies found: " + queryResponse.Results.Count);

            foreach (Lobby lobby in queryResponse.Results) {
                Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
            }
        } catch (LobbyServiceException e) {
            Debug.Log(e);
        }
    }

    public async void CheckForLobbyWithEmptySlot()
    {
        QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions {
            Filters = new List<QueryFilter> {
                new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
            }
        };

        QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);

        if (queryResponse.Results.Count != 0) {
            Debug.Log("A player is waiting for an opponent. Connecting with him");
            QuickJoinLobby();
        } else {
            Debug.Log("Creating lobby");
            CreateLobby("MyLobby", false);
        }
    }

    public async void HandleLobbyPolling() {
        if (joinedLobby != null) {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer < 0f) {
                Debug.Log(3);
                lobbyUpdateTimer = lobbyUpdateTimerMax;

                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                joinedLobby = lobby;

                if (hostLobby != null && lobby.AvailableSlots == 0 && !gameHasStarted) {
                    StartGame();
                    gameHasStarted = true;
                }

                if (joinedLobby.Data[KEY_START_GAME].Value != "0") {
                    if (hostLobby == null) {
                        bool didItWork = await ChessRelay.Instance.StartClientWithRelay(joinedLobby.Data[KEY_START_GAME].Value);

                        if(didItWork) {
                            debugText.text = "A mers";
                        } else {
                            debugText.text = "Nu a mers";
                        }

                        StartCoroutine(WaitForConnectedStatus());
                    }

                    joinedLobby = null;
                }
            }
        }
    }

    public async void StartGame() {
        if(hostLobby != null) {
            try {
                Debug.Log("Start Game");

                string relayCode = await ChessRelay.Instance.StartHostWithRelay();

                Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions {
                    Data = new Dictionary<string, DataObject> {
                        { KEY_START_GAME, new DataObject(DataObject.VisibilityOptions.Member, relayCode) }
                    }
                });

                joinedLobby = lobby;
            } catch (LobbyServiceException e) {
                Debug.Log(e);
            }
        }
    }

    IEnumerator WaitForConnectedStatus()
    {
        Debug.Log("Am intrat in functie!");
        debugText.text = "Am intrat in functie!";
        if (NetworkManager.Singleton.IsConnectedClient.ToString() == "False") {
            Debug.Log("goood progress");
            debugText.text = "goood progress";
        }

        int i = 0;

        do {
            debugText.text = "Waiting for " + i + " seconds";
            yield return new WaitForSeconds(1f);
            i++;
        } while (NetworkManager.Singleton.IsConnectedClient.ToString() == "False");

        // Wait for 3 seconds
        //yield return new WaitForSeconds(1f);
        Debug.Log(NetworkManager.Singleton.IsConnectedClient.ToString());
        debugText.text = NetworkManager.Singleton.IsConnectedClient.ToString();

        if (NetworkManager.Singleton.IsConnectedClient.ToString() != "False") {
            Debug.Log("this is it");
            debugText.text = "this is it";
        }

        //Debug.Log(NetworkManager.Singleton.IsConnectedClient);

        // Code to execute after waiting for 3 seconds
        Debug.Log("Three seconds have passed!");
        //debugText.text = "Three seconds have passed!";
        GameObject controller = GameObject.FindGameObjectWithTag("GameController");
        controller.GetComponent<Game>().InitializePiecesServerRpc();
        GameObject.FindGameObjectWithTag("Loading").GetComponent<Text>().enabled = false;
    }
}
