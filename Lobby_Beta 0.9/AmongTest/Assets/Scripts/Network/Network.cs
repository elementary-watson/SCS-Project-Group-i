﻿using Photon.Pun;
using Photon.Realtime;
using System;
//using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Network : MonoBehaviourPunCallbacks
{
    int GAMEID = 101;
    [SerializeField] string SessionID;
    [Header("LobbyRoom")]
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[10];
    [SerializeField] private TMP_Text[] playerReadyTexts = new TMP_Text[10];
    [SerializeField] private Image[] playerImageContainer = new Image[10];
    public Text statusText;
    RoomOptions myRoomOptions;

    [Header("User Interface")]
    public CameraFollow playerCamera;
    List<int> PlayerColor = new List<int>();
    public Text txtCounterPlayersInRoom;
    public Text txtCurrentRoomName;
    public Text countdown;
    [SerializeField] GameObject LobbyRoomPanel;
    [SerializeField] GameObject StartPanel;
    [SerializeField] GameObject FadeObject;
    [SerializeField] GameObject CounterObject;
    [SerializeField] Text ReferenceForDeveloper;

    //Photon
    [Header("Photon Chat")]
    //[SerializeField] private GameObject chatWindow;

    [Header("Photon")]
    private TypedLobby customLobby = new TypedLobby("customLobby", LobbyType.Default);
    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
    [SerializeField] string lobby_Room_Name;
    PhotonView photonView;
    int lobbySwitch;
    int maxPlayersOfRoom;

    [Header("Character Control")]
    public GameObject useindicator;
    [SerializeField] Main_Console_Script mainConsole_object;
    [SerializeField] Game_Info_Script gInfoScript_object;
    public Map_Control_Script msc_object;

    [Header("Game logic")]
    [SerializeField] Camera cam;
    [SerializeField] Time_Game_Script tgs_object;
    [SerializeField] private Result_Voting_Panel result_vp;
    [SerializeField] TextMeshProUGUI myPlayerRole;
    [SerializeField] GameObject Introduction_Panel_Crewmate;
    [SerializeField] GameObject Introduction_Panel_Saboteur;
    [SerializeField] GameObject Introduction_Panel;
    [SerializeField] Lobby_Timer lobbyTime_object;
    [SerializeField] Introduction_Panel introPanel_object;
    [SerializeField] GameObject Callmeeting_Panel;
    [SerializeField] CallMeeting_Script callMeeting_object;
    [SerializeField] private bool isGameOver;
    [SerializeField] private double RPC_GameStartTimestamp; //XOF Zum Loggen der Daten
    [SerializeField] string RPC_currentTimestampString; //XOF Zum Loggen der Daten
    [SerializeField] double RPC_currentTimestampDouble; //XOF Zum Loggen der Daten
    public Progressbar_Script prog_reference;
    public Multiplayer_Reference m_reference;
    [SerializeField] Gameover_Panel_Script gameOver_object;
    [SerializeField] Panel_Manager_Script pms_object;

    [Header("Spieler Variablen")]
    [SerializeField] IDictionary<int, string> listOfSuspects = new Dictionary<int, string>();
    [SerializeField] IDictionary<int, string> listOfVotekicks = new Dictionary<int, string>();
    [SerializeField] private Vector3 spawnPositions;
    GameObject spawnedPlayerObject; //Spieler Prefab
    List<string> randomColorList;
    private string playerColor;
    private string myPlayerColorFilename;
    [SerializeField] private bool isSaboteur; //Wichtige Variable
    [SerializeField] private bool isGhost; //Wichtige Variable    
    [SerializeField] float playerScorepoints; //XOF Punktzahl der spieler
    [SerializeField] int numberOfTask; //XOF Punktzahl der spieler
    [SerializeField] string myCurrentTask; //XOF meine Task
    [SerializeField] Image img_active;
    [SerializeField] Image img_inactive;
    [SerializeField] Umfrage1_Script umfrage1;
    [SerializeField] GameObject Umfrage1_Panel;

    [Header("Setup Room")]
    bool canSetSaboteur;

    private void Start()
    {
        //MessageBox.Show(Callback, "Hello World!", "Hello");
    }

    public void Awake()
    {
        // XOF PhotonNetwork.FetchServerTimestamp();
        isGameOver = false;
        lobbySwitch = 0;
        lobby_Room_Name = "LobbyRoom_A";
        isSaboteur = false;
        isGhost = false;
        for (int i = 0; i < 10; i++)
        {
            playerImageContainer[i].enabled = false;
            playerNameTexts[i].text = "";
            playerReadyTexts[i].text = "";
        }
        myRoomOptions = new RoomOptions() { MaxPlayers = 10, IsVisible = true, IsOpen = true /*,PlayerTtl = 10000, EmptyRoomTtl=60000*/ };
        maxPlayersOfRoom = 10;
        //canJoin = true;
        canSetSaboteur = true;
    }
    private void Update()
    {
        
    }

    #region getta/setta
    public int getRPC_GameStartTimestamp() { return (int)RPC_GameStartTimestamp; }
    public string getRPC_currentTimestampString() { return RPC_currentTimestampString; }
    public double getRPC_currentTimestampDouble() { return RPC_currentTimestampDouble; }
    public void setRPC_currentTimestamp(double RPC_currentTimestamp) { this.RPC_currentTimestampString = "" + RPC_currentTimestamp; 
        this.RPC_currentTimestampDouble = RPC_currentTimestamp; }
    public string getSessionID() { return SessionID; }
    public void setSessionID(string sessionID) { print(sessionID); SessionID = sessionID; }
    public int getActorId() { return PhotonNetwork.LocalPlayer.ActorNumber; }
    public int getActorsInRoom() { return PhotonNetwork.CurrentRoom.PlayerCount; }
    public string getPlayerColor() { return myPlayerColorFilename; }

    public bool getIsSaboteur(){ return isSaboteur; }

    public void setIsGameOver(bool isGameOver) {this.isGameOver = isGameOver;}    
    public bool getIsGameOver() {return isGameOver;}
    public void addSuspectToList(int gameround, string playerColor)
    {
        string value = gameround + " " + playerColor;
        // Runde pre/post Brown/Rot/Blau
        listOfSuspects.Add(gameround, value);
    }
    public void addVotekickToList(int gameround, string playerColor)
    {
        string value = gameround + " " + playerColor;
        // Runde pre/post Brown/Rot/Blau
        listOfVotekicks.Add(gameround, value);
    }
    public float getScorePoints() { return playerScorepoints; }
    public int getNumberOfTasks() { return numberOfTask; }
    public int getMaxPlayer() { return maxPlayersOfRoom; }
    #endregion

    #region Chatfunctions

    public void callChatWindowRPC() // XOF kann gelöscht werden
    {
        try {
            print("i was called");
            photonView.RPC("openChatWindow", RpcTarget.All);
        }
        catch (Exception e) 
        {
            print("Exception: " + e);
        }
    }
    #endregion

    #region Joining Process

    public void btnJoinOrCreateRoom()
    {
        statusText.text = "Connecting";
        PhotonNetwork.NickName = "Player" + UnityEngine.Random.Range(0, 1000);
        print("Nickname was created: " + PhotonNetwork.NickName);
        PhotonNetwork.ConnectUsingSettings();
    }

    public void JoinLobby()
    {
        print("1. Join Lobby was called");
        //In PUN 2 you would have to deal with the Room List in another way, since it isn't cached internally any longer. 
        //PhotonNetwork.CurrentLobby.Name = "";
        PhotonNetwork.JoinLobby(customLobby);

        print(PhotonNetwork.CountOfPlayersOnMaster);
        try
        {
            if (PhotonNetwork.InLobby) print("We are in a lobby");
            print("LobbyName: " + PhotonNetwork.CurrentLobby.ToString());
            print("LobbyName: " + PhotonNetwork.CurrentLobby.Name);
        }
        catch (Exception e)
        {
            print("ERROR: " + e);
        }
    }
    //Not working because Photon ist ein Huenson
    public override void OnJoinedLobby()
    {
        if (PhotonNetwork.InLobby) print("We are in a lobby");
    }
    public override void OnConnected()
    {
        print("2. Onconnected We are connected");
        base.OnConnected();
    }
    public override void OnConnectedToMaster()
    {
        print("OnConnectedToMaster is called");
        // PhotonNetwork.PlayerList is never empty
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            print("NICHT IM RAUM: ID: " + player.ActorNumber + "\n" + "Nickname: " + player.NickName);
        }
        statusText.text = "Joining room";

        pickLobbyRoom();
    }
    private void pickLobbyRoom()
    {
        if (lobbySwitch == 0) PhotonNetwork.JoinOrCreateRoom("LobbyRaum_A", myRoomOptions, TypedLobby.Default);
        if (lobbySwitch == 1) PhotonNetwork.JoinOrCreateRoom("LobbyRaum_B", myRoomOptions, TypedLobby.Default);
        if (lobbySwitch == 2) PhotonNetwork.JoinOrCreateRoom("LobbyRaum_C", myRoomOptions, TypedLobby.Default);
        if (lobbySwitch == 3) PhotonNetwork.JoinOrCreateRoom("LobbyRaum_D", myRoomOptions, TypedLobby.Default);
    }

    [PunRPC]
    public void RoomPlayerJoin()
    {
        int i = 0;
        for (int j = 0; j < 10; j++)
        {
            playerImageContainer[j].enabled = false;
            playerNameTexts[j].SetText("");
        }
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            print("ID: " + player.ActorNumber + "\n" + "Nickname: " + player.NickName);
            playerNameTexts[i].SetText(player.NickName);
            playerImageContainer[i].enabled = true;
            i++;
        }
    }
    [PunRPC]
    public void RefreshPlayerNumberOnJoin()
    {
        txtCounterPlayersInRoom.text = "(" + PhotonNetwork.CurrentRoom.PlayerCount + "/10)";
    }
    #endregion

    #region Inside_Room/Configuration
    int test3 = 0;
    public override void OnJoinedRoom()
    {
        print("DEBUG: I joined room");
        //try { 
        //else if (PhotonNetwork.CurrentRoom.PlayerCount == roomMaxPlayerRef+1) PhotonNetwork.LeaveRoom();
        if (PhotonNetwork.CurrentRoom.IsOpen == true)
        {
            print(System.DateTime.Now);
            umfrage1.btn_finished(getActorId());
            photonView = gameObject.GetComponent<PhotonView>();
            addPeopleTimerLobby(true);
            if (PhotonNetwork.CurrentRoom.PlayerCount > 4)
            {
                startTimerLobby();
            }
            else if (PhotonNetwork.CurrentRoom.PlayerCount == 4)
            {
                startTimerLobby();
            }
            else if (PhotonNetwork.CurrentRoom.PlayerCount < 5)
            {

            }
            PhotonNetwork.NickName = PhotonNetwork.LocalPlayer.ActorNumber + "";
            txtCurrentRoomName.text = PhotonNetwork.CurrentRoom.Name;
            print("Name of room: " + PhotonNetwork.CurrentRoom.Name +
                "\nPlayer in current room: " + PhotonNetwork.CurrentRoom.PlayerCount +
                "\nAlle RaumStatistiken PlayerInRooms: " + PhotonNetwork.CountOfPlayersInRooms +
                "\nDEBUG: (InRoom) Name of Player: " + PhotonNetwork.NickName);
            statusText.text = "Connected to Lobby: " + lobby_Room_Name;

            photonView.RPC("RefreshPlayerNumberOnJoin", RpcTarget.All);
            photonView.RPC("RoomPlayerJoin", RpcTarget.All);
            //txtCounterPlayersInRoom.text = "("+ PhotonNetwork.CurrentRoom.PlayerCount + "/10)";            

            //photonView.RPC("sendMyActorID", RpcTarget.All);

            if (PhotonNetwork.CurrentRoom.PlayerCount == maxPlayersOfRoom)
            {
                print("MaxPlayer has arrived");
                PhotonNetwork.CurrentRoom.IsOpen = false;
                initiateStartGame();
            }
        }
        else { print("ERROR: Joining Room failed"); }
    }

    public void addPeopleTimerLobby(bool isJoin)
    {
        photonView.RPC("RPC_addPeopleTimerLobby", RpcTarget.AllBuffered, isJoin);
    }
    [PunRPC]
    public void RPC_addPeopleTimerLobby(bool isJoin)
    {
        lobbyTime_object.setLobbyRoomPeople(isJoin);
    }
    public void startTimerLobby()
    {
        photonView.RPC("RPC_startTimerLobby", RpcTarget.AllBuffered);
    }
    [PunRPC]
    public void RPC_startTimerLobby()
    {
        lobbyTime_object.setup(getActorId());
    }

    public void stopTimerLobby()
    {
        photonView.RPC("RPC_stopTimerLobby", RpcTarget.AllBuffered);
    }
    [PunRPC]
    public void RPC_stopTimerLobby()
    {
        lobbyTime_object.stopTimer();
    }
    #endregion

    #region prepareGame
    public void initiateStartGame()
    {
        setSessionID();        
    }
    public void setSessionID()
    {
        photonView.RPC("RPC_setSessionID", RpcTarget.All);
    }
    int counterAllSession = 0;
    [PunRPC]
    public void RPC_setSessionID(PhotonMessageInfo info)
    {
        counterAllSession += 1;
        string lobbyId = "";
        if (lobbySwitch == 0)
            lobbyId = "0";
        else if (lobbySwitch == 1)
            lobbyId = "1";
        else if (lobbySwitch == 2)
            lobbyId = "2";
        else if (lobbySwitch == 3)
            lobbyId = "3";
        int timesstamp = (int)info.SentServerTimestamp;
        print("RPC Timestamp: " + timesstamp);
        string sessionID = GAMEID + "00" + timesstamp + "00" + lobbyId;
        print("SessionID" + sessionID);
        setSessionID(sessionID);
        if(counterAllSession == PhotonNetwork.CurrentRoom.PlayerCount) { sendMyActorID(); }
    }
    public void sendMyActorID()
    {
        photonView.RPC("RPC_sendMyActorID", RpcTarget.All, getActorId());
    }
    int countArrived = 0;
    int lastPlayer;

    public void lastAsRoomLeader(int otherActorID)
    {
        countArrived += 1;
        if (otherActorID < getActorId())
        {
            print(otherActorID + " " + getActorId());
            lastPlayer = getActorId();
        }
        else
        {
            lastPlayer = otherActorID;
        }
    }
    // Hier schickt jeder spieler seine eigene actor id im RPC
    [PunRPC]
    public void RPC_sendMyActorID(int otherActorID)
    {
        //Hier bekommt jeder von jedem die ActorID, deshalb wollen wir den letzten als Konfigurator bestimmen, weil es nur einer sein darf.
        print("RPC_sendMyActorID PlayerCount: " + PhotonNetwork.CurrentRoom.PlayerCount);
        countArrived += 1;

        lastAsRoomLeader(otherActorID);
        if (countArrived == PhotonNetwork.CurrentRoom.PlayerCount && lastPlayer == getActorId())
        {
            countArrived = 0;
            maxPlayersOfRoom = (int)PhotonNetwork.CurrentRoom.PlayerCount;
            lastPlayerSetConfiguration();
        }
    }
    public void lastPlayerSetConfiguration()
    {
        //Randomize colors
        RandomColor();
        //photonView = gameObject.GetComponent<PhotonView>();
        int i = 0;
        //setupPlayer over RPC
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            print("Id and Color: " + player.NickName + "-" + randomColorList[i]);
            photonView.RPC("RPC_setupPlayer", RpcTarget.All, player.NickName + "-" + randomColorList[i]);
            //photonView.RPC("setupPlayer", RpcTarget.OthersBuffered, player.NickName + "-" + randomColorList[i]);
            i++;
        }
    }
    private void RandomColor()
    {
        List<string> RandomColorList = new List<string> { "PlayerBlack", "PlayerBlue", "PlayerBrown", "PlayerPink", "PlayerGreen", "PlayerOrange", "PlayerPurple", "PlayerRed", "PlayerWhite", "PlayerYellow" };
        /*List<string> RandomColorList = new List<string> { "PlayerBlack", "PlayerBlack", "PlayerBlack", "PlayerBlack", "PlayerBlack", "PlayerBlack", "PlayerBlack", "PlayerBlack", "PlayerBlack", "PlayerBlack" };*/
        var count = RandomColorList.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = RandomColorList[i];
            RandomColorList[i] = RandomColorList[r];
            RandomColorList[r] = tmp;
        }
        this.randomColorList = RandomColorList;
    }
    
    [PunRPC]
    public void RPC_setupPlayer(String idAndColor)
    {
        m_reference.setNumberOfPlayer((int)PhotonNetwork.CurrentRoom.PlayerCount);
        
        maxPlayersOfRoom = (int)PhotonNetwork.CurrentRoom.PlayerCount;
        //XOF hier werden farben eingestellt und die multiplayer referenz aufgefüllt
        String[] parts = idAndColor.Split('-');
        string temp = "";
        if (parts[0] == (PhotonNetwork.NickName))
        {
            temp = parts[1];
        }
        int otherActorID = int.Parse(parts[0]);
        lastAsRoomLeader(otherActorID);        
        if (temp != null) playerColor = temp.Remove(0, 6);

        switch (playerColor)
        {
            case "Purple":
                myPlayerColorFilename = "Purple_Char";
                break;
            case "Brown":
                myPlayerColorFilename = "Brown_Char";
                break;
            case "Green":
                myPlayerColorFilename = "Green_Char";
                break;
            case "Yellow":
                myPlayerColorFilename = "Yellow_Char";
                break;
            case "Blue":
                myPlayerColorFilename = "Blue_Char";
                break;
            case "White":
                myPlayerColorFilename = "White_Char";
                break;
            case "Black":
                myPlayerColorFilename = "Black_Char";
                break;
            case "Pink":
                myPlayerColorFilename = "Pink_Char";
                break;
            case "Orange":
                myPlayerColorFilename = "Orange_Char";
                break;
            case "Red":
                myPlayerColorFilename = "Red_Char";
                break;
        }
        /*// The character prefab filename
        List<string> colorFileList = new List<string> { "Black_Char", "Blue_Char", "Brown_Char", "Pink_Char", "Green_Char", "Orange_Char", "Purple_Char", "Red_Char", "White_Char", "Yellow_Char" };
        string temp = "";
        if (playerColor != null)
            temp = playerColor.Remove(0, 6);
        int i = 0;
        foreach (string item in colorFileList)
        {
            if (item.Contains(temp))
            {
                myPlayerColorFilename = colorFileList[i];
            }
            i++;
        }*/
        m_reference.addNewPlayer(getActorId(), playerColor);
        //bool isMaxPlayer = m_reference.addPlayer(int.Parse(parts[0]), parts[1].Remove(0, 6), maxPlayersOfRoom);

        if (countArrived == PhotonNetwork.CurrentRoom.PlayerCount && lastPlayer == getActorId() && canSetSaboteur == true) // wenn alle ankommen soll nur EINER den saboteur bestimmen
        {
            countArrived = 0;
            canSetSaboteur = false;
            print("Init setSaboteur with ActorID: " + getActorId());
            setupMultiplayerGame();
        }
        print("RPC_setupPlayer: " + int.Parse(parts[0]) + " Color" + parts[1].Remove(0, 6));
    }

    public void checkConnectionState()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            string nickName = player.NickName;
        }
    }

    public void setupMultiplayerGame() //wird nur einmal vom letzten playerausgeführt
    {
        // XOF hier wird der Saboteur erstellt
        int rand = UnityEngine.Random.Range(0, PhotonNetwork.CurrentRoom.PlayerCount);
        //int i = 1;
        int saboteurID = m_reference.fullPlayerList[rand].getActorID();
        print("The Saboteur with index: " + rand + " is: " + saboteurID);

        setPlayerSpawnPosition();
        photonView.RPC("RPC_setupMultiplayerGame", RpcTarget.All, saboteurID);
    }

    [PunRPC]
    public void RPC_setupMultiplayerGame(int saboteurID) // Spielbalance und saboteur einstellen
    {        
        //diese methode absichern falls method durch playerleave gerufen wurde. Falls einer geht muss nach jeder Runde rebalanced werden.
        print("RPC_setupMultiplayerGame Saboteur ID " + saboteurID);
        if (getActorId() == saboteurID)
        {
            isSaboteur = true;
            int index = m_reference.getPlayerByActorID(getActorId());
            m_reference.fullPlayerList[index].setPlayerIsSaboteur(true);
        }
        m_reference.setSaboteurActorID(saboteurID);
        m_reference.setupGamestyle();
        //Continues in Class Countdown XOF
        CounterObject.SetActive(true);
    }

    public void RPCStartFading()
    {
        photonView.RPC("startFading", RpcTarget.All);
    }

    [PunRPC]
    public void startFading()
    {
        FadeObject.SetActive(true);
        LobbyRoomPanel.SetActive(false);
        //Invoke("RPCstartgame", 5);
        //RPCstartgame();
    }
    #endregion

    #region startgame
    public void RPCstartgame(){photonView.RPC("startGame", RpcTarget.All);}
    int test2 = 0;
    [PunRPC]
    public void startGame(PhotonMessageInfo info)
    {
        Umfrage1_Panel.SetActive(false);
        print("TimeStamp: " + info.timestamp);
        print("TimeStamp: " + info.SentServerTime);
        RPC_GameStartTimestamp =  info.SentServerTime;
        //GameMapPanel.SetActive(true);
        //LobbyRoomPanel.SetActive(false);
        if (test2 == 0) { }
            //ReferenceForDeveloper.text = RPC_GameStartTimestamp;
            print("HERE" + test2 + "  " + m_reference.getNumberOfPlayer());
        test2 += 1;
        if (test2 == m_reference.getNumberOfPlayer())
        {
            SpawnPlayer();
        }        
    }
    private void SpawnPlayer()
    {
        GameObject spawn = PhotonNetwork.Instantiate(playerColor, spawnPositions, Quaternion.identity);
        CharacterControl cc = spawn.GetComponent<CharacterControl>();
        cc.interactIcon = useindicator;
        cc.setMCSScript(msc_object);
        cc.setMultiplayerReference(m_reference);
        cc.setInteractImages(img_active, img_inactive);
        cc.setActorID(PhotonNetwork.LocalPlayer.ActorNumber);
        cc.setMainConsoleScript(mainConsole_object);
        cc.setGameInfoScript(gInfoScript_object);
        spawn.GetComponent<CharacterControl>().interactIcon = useindicator;

        playerCamera.target = spawn.transform;
        spawnedPlayerObject = spawn;
        setPhotonViewID();
        setPlayerMovement(false);
        Introduction_Panel.SetActive(true);
        introPanel_object.setup();
        if (isSaboteur)
        {
            myPlayerRole.text = "Role >> Saboteur";
            //myPlayerRole.text = timestamp;
            Introduction_Panel_Saboteur.SetActive(true);
        }
        else
        {
            Introduction_Panel_Crewmate.SetActive(true);        
        }

        //XOFXOFXOF AB HIER geht es im Introduction Panel weiter und RPC -> setIntroductionOff wird gerufen! Damit wird das Spiel gestartet
    }
    public void setPhotonViewID() // photon id weil nur damit andere player angesprochen werden können
    {
        photonView.RPC("RPC_setPhotonViewID", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, spawnedPlayerObject.GetComponent<PhotonView>().ViewID);
    }
    [PunRPC]
    public void RPC_setPhotonViewID(int actorID, int photonViewID) //Photon View bei allen sichern
    {
        print("Got ViewID of: " + actorID + " with viewID: " + photonViewID);
        m_reference.addPhotonplayer(actorID, photonViewID);
        var player = PhotonView.Find(photonViewID);
        //tgs_object.beginTimer();
    }
    public void setPlayerSpawnPosition()
    {
        List<string> setColorPosition = new List<string> { "Black", "Blue", "Brown", "Pink", "Green", "Orange", "Purple", "Red", "White", "Yellow" };
        var count = setColorPosition.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = setColorPosition[i];
            setColorPosition[i] = setColorPosition[r];
            setColorPosition[r] = tmp;
        }
        string chain = "";
        foreach (string item in setColorPosition)
        {
            chain = chain + item + "-";
        }
        photonView.RPC("sentColorPositionForMultiplayer", RpcTarget.All, chain);
    }
    [PunRPC]
    public void sentColorPositionForMultiplayer(string chain)
    {
        String[] parts = chain.Split('-');
        List<string> setColorPosition = parts.ToList();
        m_reference.setRandomColorList(setColorPosition);
        Vector3 spawnPos = m_reference.setupPositions(PhotonNetwork.LocalPlayer.ActorNumber);
        if (spawnPos == new Vector3(0, 0, 0))
            print("ERROR: Player was not found position faulty!");
        else
            spawnPositions = spawnPos;        
    }
    int test = 0;
    public void setIntroductionOff() // wird von introduction panel als vorletzten schritt aufgerufen!
    {
        photonView.RPC("RPC_setIntroductionOff", RpcTarget.All);
    }
    [PunRPC]
    public void RPC_setIntroductionOff()
    {
        test += 1;
        if(m_reference.getNumberOfPlayer() == test)
        {
            Invoke("finalyIntroductionOff", 4);
        }
    }
    public void finalyIntroductionOff()
    {
        introPanel_object.setIntroductionOff();
    }
    #endregion

    #region inGame
    public void resetPlayerPosition()
    {
        photonView.RPC("RPC_resetPlayerPosition", RpcTarget.All);
    }
    [PunRPC]
    public void RPC_resetPlayerPosition()
    {
        spawnedPlayerObject.transform.position = spawnPositions;
        spawnedPlayerObject.GetComponent<CharacterControl>().resetTask();        
    }
    public void setPlayerMovement(bool canWalk)
    {
        print(canWalk);
        Player_Movement pm_object = spawnedPlayerObject.GetComponent<Player_Movement>();
        if (canWalk) 
        {
            pm_object.enableMovementSpeed();
            spawnedPlayerObject.GetComponent<CharacterControl>().toggleInteractFunction(true);
        }
        else
        {
            pm_object.disableMovementSpeed();
            spawnedPlayerObject.GetComponent<CharacterControl>().toggleInteractFunction(false);
        }
    }
    public void getNextTask()
    {
        photonView.RPC("RPC_getNextTask", RpcTarget.All, getActorId());
    }

    [PunRPC]
    public void RPC_getNextTask(int actorId)
    {
        string nextTask = m_reference.getNextTask();
        if (actorId == getActorId())
        {
            myCurrentTask = nextTask;
            sendNextTask();
        }
    }
    public void sendNextTask()
    {
        mainConsole_object.setCurrentTask(myCurrentTask);
    }

    public void incrementTaskprogress()
    {
        print("DEBUG: IncrementProgress was called");
        photonView.RPC("incrementTaskprogressNetwork", RpcTarget.All, spawnedPlayerObject.GetComponent<CharacterControl>().getIncrementPower());

        float addScore = spawnedPlayerObject.GetComponent<CharacterControl>().getIncrementPower();

        if (isSaboteur)
            addScore = addScore*-1;
        playerScorepoints += addScore;
        numberOfTask += 1;
    }
    [PunRPC]
    public void incrementTaskprogressNetwork(float increment)
    {
        prog_reference.setTaskprogress(increment);
    }

    #endregion

    #region Voting

    public void setScoreOfRound(bool source) // Punkte geben wird von Time_Game_Script oder Progressbar_Script gerufen
    {
        //XOF RPC EINBAUEN
        // Source -> true crewmates und vice versa
        if (source)
        {
            m_reference.setCrewPoints(m_reference.getCrewPoints() + 1);
        }
        else
        {
            m_reference.setSaboteurPoints(m_reference.getSaboteurPoints() + 1);
        }
    }
    public void callMeeting(bool isCrewmate)
    {
        photonView.RPC("RPC_callMeeting", RpcTarget.All, getActorId(), isCrewmate);
    }
    [PunRPC]
    public void RPC_callMeeting(int actorID, bool isCrewmate)
    {
        print("outer" + actorID);
        if(actorID == getActorId())
        {
            print("Inner: " + getActorId());
            Callmeeting_Panel.SetActive(true);
            callMeeting_object.setup(isCrewmate);
        }
    }
    public void callSubmitVote(bool isSubmitted, string playerColor, int photonActorID, int indexPosition)
    {
        if (m_reference.getCurrentStage()==1)         
            addSuspectToList(m_reference.getGameRound(), playerColor);        
        else if(m_reference.getCurrentStage() == 3)
            addVotekickToList(m_reference.getGameRound(), playerColor); // in die votekickliste speichern

        photonView.RPC("submitAllPlayers", RpcTarget.All, isSubmitted, playerColor, photonActorID, indexPosition);
    }


    [PunRPC]
    public void submitAllPlayers(bool isSubmitted, string playerColor, int photonActorID, int indexPosition, PhotonMessageInfo info)
    {
        setRPC_currentTimestamp(info.SentServerTime);
        if (isSubmitted)
            result_vp.submitVote(getActorId(), getPlayerColor(), playerColor, photonActorID, indexPosition);
        else
            result_vp.submitVote(getActorId(), getPlayerColor(), "", 0, -1);
    }

    // XOF Ghost
    public void setPlayerToGhost(int photonViewId) // wird von resultpanel zum schluss von phase 3 gerufen
    {
        photonView.RPC("RPC_setPlayerToGhost", RpcTarget.All, photonViewId);
    }

    [PunRPC]
    public void RPC_setPlayerToGhost(int kickedPhotonViewId)
    {
        //m_reference.
        print("Photon Ghost call");
        if (isSaboteur)
        {
            setIsGameOver(true);
        }
        else if (kickedPhotonViewId == spawnedPlayerObject.GetComponent<PhotonView>().ViewID)
        {
            print(PhotonNetwork.LocalPlayer.ActorNumber + " Found " + playerColor + " with viewID: " + spawnedPlayerObject.GetComponent<PhotonView>().ViewID);
            spawnedPlayerObject.layer = 11; //set player to invisibleLayer
            spawnedPlayerObject.GetComponent<CharacterControl>().setStatusToGhost();
            isGhost = true;
            myPlayerRole.text = "Rolle >> Geist";
            int oldMask = cam.cullingMask;
            cam.cullingMask = -1; // |= ~(1 << 11);
            var allObjects = spawnedPlayerObject.GetComponentsInChildren<SpriteRenderer>(false);
            foreach (SpriteRenderer item in allObjects)
            {
                print("Renderer False Found: " + item.name);
                item.enabled = true;
            }
            var allTrueObjects = spawnedPlayerObject.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (SpriteRenderer item in allTrueObjects)
            {
                print("Renderer true Found: " + item.name);
                item.enabled = true;
            }
            spawnedPlayerObject.GetComponentInChildren<SpriteRenderer>(true).enabled = true;
        }
        else if (kickedPhotonViewId == -1)
        {
            print("Player not found");
        }

        else
        {
            print(PhotonNetwork.LocalPlayer.ActorNumber + " wurde nicht gekicked " + playerColor);
            GameObject[] playerObject = GameObject.FindGameObjectsWithTag("Player"); //PhotonView.Find()//
            int i = 0;
            foreach (GameObject item in playerObject)
            {
                print("Index: " + i);
                if (kickedPhotonViewId == item.GetComponent<PhotonView>().ViewID)
                {
                    print("ID: " + PhotonNetwork.LocalPlayer.ActorNumber + " hat den gevoteten Spieler gefunden!");
                    item.layer = 11;
                }
            }
        }
    }

    public void RPC_checkForGameOver()
    {
        photonView.RPC("checkForGameOver", RpcTarget.All, isGameOver);
    }
    int timeCheckForGameOver = 0;
    [PunRPC]
    public void checkForGameOver(bool isGameOver)
    {
        timeCheckForGameOver += 1;
        if (isGameOver)
        {
            setIsGameOver(isGameOver);
        }
        print("RESULT GAMEOVER" + timeCheckForGameOver + " " + maxPlayersOfRoom);
        if (timeCheckForGameOver == maxPlayersOfRoom)
        {
            pms_object.checkVotingResult(getIsGameOver());
        }
    }
    #endregion

    #region endGame
    bool allScoresDone = false;
    bool allTasksDone = false;
    int statisticsHelper = 0;
    public void RPC_buildStatistics( )
    {
        photonView.RPC("buildStatistics", RpcTarget.All, getActorId(), getScorePoints(), getNumberOfTasks());
    }
    [PunRPC]
    public void buildStatistics(int actorID , float score, int numOfTasks)
    {
        canSetSaboteur = true;
        statisticsHelper += 1;
        print("buildStatistics called");
        allScoresDone = m_reference.addAllPlayerScores(actorID, score, maxPlayersOfRoom);
        allTasksDone = m_reference.addAllPlayerTasks(actorID, numOfTasks, maxPlayersOfRoom);
        if(statisticsHelper == maxPlayersOfRoom)
        {
            gameOver_object.createTable();
        }
    }
    #endregion

    #region leaveLobby/LobbyJoinFail
    //XOF Funktion der Methode unbekannt. Steht in Relation mit Lobbyraum verlassen
    public void OnPhotonPlayerDisconnected()
    {
        photonView.RPC("RoomPlayerLeave", RpcTarget.All, PhotonNetwork.NickName.ToString());
        photonView.RPC("RefreshPlayerNumberOnLeave", RpcTarget.All);
        // send event, add your code here
        print("DEBUG: OnPhotonPlayerDisconnected");
        PhotonNetwork.SendAllOutgoingCommands(); // send it right now
    }
    public override void OnLeftRoom()
    {
        print("1. I left the room");
        if (lobbySwitch == 0) lobbySwitch = 1;
        else if (lobbySwitch == 1) lobbySwitch = 2;
        else if (lobbySwitch == 2) lobbySwitch = 0;
        base.OnLeftRoom();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        print("2. I Disconnected");
        base.OnDisconnected(cause);

    }
    //public override void OnPhoton
    public override void OnLeftLobby()
    {
        print("3. I left the lobby");
        base.OnLeftLobby();
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print("Joining Room failed");
        if (lobbySwitch == 0) lobbySwitch = 1;
        else if (lobbySwitch == 1) lobbySwitch = 2;
        else if (lobbySwitch == 2) lobbySwitch = 3;
        else if (lobbySwitch == 3) lobbySwitch = 4;
        pickLobbyRoom();
        base.OnJoinRoomFailed(returnCode, message);
        //PhotonNetwork.LeaveRoom();
    }

    #endregion

    #region QuitGame

    public void WebGLQuit()
    {
        this.SendQuitEvent();
    }

    void OnApplicationQuit()
    {
        print("DEBUG: Player left Application");
        this.SendQuitEvent();
    }

    void SendQuitEvent() // send event, add your code here
    {
        try
        {
            //XOF wenn saboteur leaved dann passiert noch nix
            if (m_reference.getSaboteurActorID() == getActorId())
            {
                print("QUIT: Saboteur left the game, reset?");
            }
            lobbyTime_object.setLobbyRoomPeople(false);
            if (PhotonNetwork.CurrentRoom.PlayerCount > 4)
            {
                addPeopleTimerLobby(false);
            }
            maxPlayersOfRoom -= 1; 
            photonView.RPC("RoomPlayerLeave", RpcTarget.All, PhotonNetwork.NickName.ToString());
            photonView.RPC("RefreshPlayerNumberOnLeave", RpcTarget.All);

            print("DEBUG: SendQuitEvent ausführen");
            PhotonNetwork.SendAllOutgoingCommands(); // send it right now!
        }
        catch (Exception e) { print("ERROR: " + e); }
    }

    [PunRPC]
    public void RefreshPlayerNumberOnLeave()
    {
        txtCounterPlayersInRoom.text = "(" + (PhotonNetwork.CurrentRoom.PlayerCount - 1) + "/10)";
    }

    [PunRPC]
    public void RoomPlayerLeave(string playerNickname)
    {
        m_reference.deleteplayer(PhotonNetwork.LocalPlayer.ActorNumber);
        int i = 0;
        for (int j = 0; j < 10; j++)
        {
            playerImageContainer[j].enabled = false;
            playerNameTexts[j].SetText("");
        }
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (playerNickname != player.NickName)
            {
                print("ID: " + player.ActorNumber + "\n" + "Nickname: " + player.NickName);
                playerNameTexts[i].SetText(player.NickName);
                playerImageContainer[i].enabled = true;
                i++;
            }
        }
    }
    #endregion

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        print("WE UPDATED THE LIST ROOMS");
        //base.OnRoomListUpdate(roomList);
        UpdateCachedRoomList(roomList);
    }
    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        print("UpdateCachedRoomList is called ");
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                cachedRoomList.Remove(info.Name);
            }
            else
            {
                cachedRoomList[info.Name] = info;
                print("Name of LobbyRoom: " + info.Name);
                print("Count of Player: " + info.PlayerCount);
            }
        }
    }

}