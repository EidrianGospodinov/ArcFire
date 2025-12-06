using System;
using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Multiplayer
{
    public class Chat : MonoBehaviour, IChatClientListener
    {

        public string userName;
        public ChatClient ChatClient;
        public InputField InputField;
        public Text ChatContent;
        private bool isInputFieldToggled;
        
        [Header("necessary dependency for the command Dispatcher")]
        private CommandDispatcher dispatcher;
        private MultiPlayerLevelManager multiPlayerLevelManager;
        void Awake()
        {
            ChatClient = new ChatClient(this);
        }

        private void Start()
        {
            multiPlayerLevelManager = FindFirstObjectByType<MultiPlayerLevelManager>();
            
            
            
            
            
            dispatcher = new CommandDispatcher(multiPlayerLevelManager);
        }

        // Update is called once per frame
        void Update()
        {
            ChatClient.Service();



            if(Input.GetKeyDown(KeyCode.T) && !isInputFieldToggled)
            {
                isInputFieldToggled = true;
                InputField.Select();
                InputField.ActivateInputField();

            }

            if (Input.GetKeyDown(KeyCode.Escape) && isInputFieldToggled)
            {

                isInputFieldToggled = false;

                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);

            }


            if ((Input.GetKeyDown(KeyCode.Return))||(Input.GetKeyDown(KeyCode.KeypadEnter)) && isInputFieldToggled)
            {
                if (InputField.text.StartsWith("/"))
                {
                    var text = InputField.text.Substring(1);
                    if (text != "")
                    { 
                        HandleCommand(InputField.text.Substring(1));
                    }
                }
                else
                {
                    SetMessage();
                }
            }


        }

        private void HandleCommand(string message)
        {
            string result = dispatcher.Dispatch(message);
            CleanupInputField();
        }

        public void DebugReturn(DebugLevel level, string message)
        {
            Debug.Log($"Chat- {level} - {message}");
        }

        public void OnDisconnected()
        {
            Debug.Log($"Chat - users: {userName} has disconnected");
        }

        public void OnConnected()
        {
            Debug.Log($"Chat - {userName} has connected");
            ChatClient.Subscribe(PhotonNetwork.CurrentRoom.Name,
                creationOptions: new ChannelCreationOptions() { PublishSubscribers = true });
            
        }

        public void OnChatStateChange(ChatState state)
        {
            Debug.Log($"Chat - OnStateChange {state}");
        }

        public void OnGetMessages(string channelName, string[] senders, object[] messages)
        {
            if(ChatClient.TryGetChannel(PhotonNetwork.CurrentRoom.Name, out var currentChat))
            {
                ChatContent.text = currentChat.ToStringMessages();
            }
        }

        public void OnPrivateMessage(string sender, object message, string channelName)
        {
            
        }

        public void OnSubscribed(string[] channels, bool[] results)
        {
            Debug.Log("onSubscribe");
            for (int i = 0; i < channels.Length; i++)
            {
                if (results[i])
                {
                    Debug.Log($"Chat - subscribed to {channels[i]} channels");
                    ChatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name, " has joined the chat");
                }
            }
        }

        public void OnUnsubscribed(string[] channels)
        {
            Debug.LogError("onUNSubscribe");
            for (int i = 0; i < channels.Length; i++)
            {
                Debug.Log($"Chat - unsubscribed to {channels[i]} channels");
                ChatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name, " has left the chat");
            }
        }

        public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
        {
        }

        public void OnUserSubscribed(string channel, string user)
        {
        }

        public void OnUserUnsubscribed(string channel, string user)
        {
        }

        public void SetMessage()
        {
            if (InputField.text == "")
            {
                return;
            }

            ChatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name, InputField.text);
            CleanupInputField();
        }

        private void CleanupInputField()
        {
            InputField.text = "";
            isInputFieldToggled = false;
            
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
