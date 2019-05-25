using System;
using System.Collections.Generic;
using ExitGames.Client.Photon.Chat;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// This simple Chat UI demonstrate basics usages of the Chat Api
/// </summary>
/// <remarks>
/// The ChatClient basically lets you create any number of channels.
///
/// some friends are already set in the Chat demo "DemoChat-Scene", 'Joe', 'Jane' and 'Bob', simply log with them so that you can see the status changes in the Interface
///
/// Workflow:
/// Create ChatClient, Connect to a server with your AppID, Authenticate the user (apply a unique name,)
/// and subscribe to some channels.
/// Subscribe a channel before you publish to that channel!
///
///
/// Note:
/// Don't forget to call ChatClient.Service() on Update to keep the Chatclient operational.
/// </remarks>
public class ChatGui : MonoBehaviour, IChatClientListener
{

	public string[] ChannelsToJoinOnConnect; // set in inspector. Demo channels to join automatically.
	
	//public string[] FriendsList;
	
	public int HistoryLengthToFetch; // set in inspector. Up to a certain degree, previously sent messages can be fetched for context
	
	public string UserName { get; set; }
	
	private string selectedChannelName; // mainly used for GUI/input
	
	public ChatClient chatClient;
	
	public RectTransform ChatPanel;     // set in inspector (to enable/disable panel)
	public InputField InputFieldChat;   // set in inspector
	public Text CurrentChannelText;     // set in inspector
	
	public bool ShowState = true;
	public GameObject Title;
	public Text StateText; // set in inspector
    public Text UserIdText;
	
	public void Start()
	{
        selectedChannelName = PhotonNetwork.room.name;

		StateText.text  = "";
		
		if (string.IsNullOrEmpty(UserName))
		{
			UserName = "user" + Environment.TickCount%99; //made-up username
		}

        UserIdText.text = UserName;

        bool _AppIdPresent = string.IsNullOrEmpty(PhotonNetwork.PhotonServerSettings.ChatAppID);
		
		if (string.IsNullOrEmpty(PhotonNetwork.PhotonServerSettings.ChatAppID))
		{
			Debug.LogError("You need to set the chat app ID in the PhotonServerSettings file in order to continue.");
			return;
		}

        Connect();
    }
	
	public void Connect()
	{		
		this.chatClient = new ChatClient(this);
        #if !UNITY_WEBGL
        this.chatClient.UseBackgroundWorkerForSending = true;
        #endif
        this.chatClient.Connect(PhotonNetwork.PhotonServerSettings.ChatAppID, "1.0", new ExitGames.Client.Photon.Chat.AuthenticationValues(UserName));

        this.ChannelsToJoinOnConnect.SetValue(selectedChannelName, 0);
        this.chatClient.Subscribe(selectedChannelName);
        ShowChannel(selectedChannelName);

        Debug.Log("Connecting as: " + UserName);
	}

    /// <summary>To avoid that the Editor becomes unresponsive, disconnect all Photon connections in OnDestroy.</summary>
    public void OnDestroy()
    {
        if (this.chatClient != null)
        {
            this.chatClient.Disconnect();
        }
    }

    /// <summary>To avoid that the Editor becomes unresponsive, disconnect all Photon connections in OnApplicationQuit.</summary>
    public void OnApplicationQuit()
	{
		if (this.chatClient != null)
		{
			this.chatClient.Disconnect();
		}
	}
	
	public void Update()
	{
		if (this.chatClient != null)
		{
			this.chatClient.Service(); // make sure to call this regularly! it limits effort internally, so calling often is ok!
		}
		
		// check if we are missing context, which means we got kicked out to get back to the Photon Demo hub.
		if ( this.StateText == null)
		{
			Destroy(this.gameObject);
			return;
		}

        if (Input.GetKeyDown(KeyCode.Return))
        {
            EventSystem.current.SetSelectedGameObject(InputFieldChat.gameObject, null);
        }
	}
	
	
	public void OnEnterSend()
	{
		if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
		{
			SendChatMessage(this.InputFieldChat.text);
			this.InputFieldChat.text = "";
        }
	}	
	
	public int TestLength = 2048;
	private byte[] testBytes = new byte[2048];
	
	private void SendChatMessage(string inputLine)
	{
		if (string.IsNullOrEmpty(inputLine))
		{
			return;
		}
		if ("test".Equals(inputLine))
		{
			if (this.TestLength != this.testBytes.Length)
			{
				this.testBytes = new byte[this.TestLength];
			}
			
			this.chatClient.SendPrivateMessage(this.chatClient.AuthValues.UserId, testBytes, true);
		}
		
		
		bool doingPrivateChat = this.chatClient.PrivateChannels.ContainsKey(this.selectedChannelName);
		string privateChatTarget = string.Empty;
		if (doingPrivateChat)
		{
			// the channel name for a private conversation is (on the client!!) always composed of both user's IDs: "this:remote"
			// so the remote ID is simple to figure out
			
			string[] splitNames = this.selectedChannelName.Split(new char[] { ':' });
			privateChatTarget = splitNames[1];
		}
		
		
		if (inputLine[0].Equals('\\'))
		{
			string[] tokens = inputLine.Split(new char[] {' '}, 2);
            if (tokens[0].Equals("\\state"))
            {
                int newState = 0;


                List<string> messages = new List<string>();
                messages.Add("i am state " + newState);
                string[] subtokens = tokens[1].Split(new char[] { ' ', ',' });

                if (subtokens.Length > 0)
                {
                    newState = int.Parse(subtokens[0]);
                }

                if (subtokens.Length > 1)
                {
                    messages.Add(subtokens[1]);
                }

                this.chatClient.SetOnlineStatus(newState, messages.ToArray());
            }
            else if (tokens[0].Equals("\\clear"))
            {
                if (doingPrivateChat)
                {
                    this.chatClient.PrivateChannels.Remove(this.selectedChannelName);
                }
                else
                {
                    ChatChannel channel;
                    if (this.chatClient.TryGetChannel(this.selectedChannelName, doingPrivateChat, out channel))
                    {
                        channel.ClearMessages();
                    }
                }
            }
            else if (tokens[0].Equals("\\setname") && !string.IsNullOrEmpty(tokens[1]))
            {
                this.UserName = tokens[1];
                UserIdText.text = UserName;

                this.chatClient.Disconnect();
                Connect();
            }
            else
            {
                Debug.Log("The command '" + tokens[0] + "' is invalid.");
            }
		}
		else
		{
			if (doingPrivateChat)
			{
				this.chatClient.SendPrivateMessage(privateChatTarget, inputLine);
			}
			else
			{
				this.chatClient.PublishMessage(this.selectedChannelName, inputLine);
			}
		}
	}
	
	public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
	{
		if (level == ExitGames.Client.Photon.DebugLevel.ERROR)
		{
			UnityEngine.Debug.LogError(message);
		}
		else if (level == ExitGames.Client.Photon.DebugLevel.WARNING)
		{
			UnityEngine.Debug.LogWarning(message);
		}
		else
		{
			UnityEngine.Debug.Log(message);
		}
	}
	
	public void OnConnected()
	{
		if (this.ChannelsToJoinOnConnect != null && this.ChannelsToJoinOnConnect.Length > 0)
		{
			this.chatClient.Subscribe(this.ChannelsToJoinOnConnect, this.HistoryLengthToFetch);
		}
		
		UserIdText.text = "Connected as "+ this.UserName;
		
		this.chatClient.SetOnlineStatus(ChatUserStatus.Online); // You can set your online state (without a mesage).
	}
	
	public void OnDisconnected()
	{
	}
	
	public void OnChatStateChange(ChatState state)
	{
		// use OnConnected() and OnDisconnected()
		// this method might become more useful in the future, when more complex states are being used.
		
		this.StateText.text = state.ToString();
	}
	
	public void OnSubscribed(string[] channels, bool[] results)
	{
		this.chatClient.PublishMessage(selectedChannelName, "Hi there! :)"); // you don't HAVE to send a msg on join but you could.
		
		Debug.Log("OnSubscribed: " + string.Join(", ", channels));
		
		// Switch to the channel
		ShowChannel(selectedChannelName);
	}	
	
	public void OnUnsubscribed(string[] channels)
	{
		
	}
	
	public void OnGetMessages(string channelName, string[] senders, object[] messages)
	{
		if (channelName.Equals(this.selectedChannelName))
		{
			// update text
			ShowChannel(this.selectedChannelName);
		}
	}
	
	public void OnPrivateMessage(string sender, object message, string channelName)
	{
		// as the ChatClient is buffering the messages for you, this GUI doesn't need to do anything here
		// you also get messages that you sent yourself. in that case, the channelName is determinded by the target of your msg
		//this.InstantiateChannelButton(channelName);
		
		byte[] msgBytes = message as byte[];
		if (msgBytes != null)
		{
			Debug.Log("Message with byte[].Length: "+ msgBytes.Length);
		}
		if (this.selectedChannelName.Equals(channelName))
		{
			ShowChannel(channelName);
		}
	}
	
	/// <summary>
	/// New status of another user (you get updates for users set in your friends list).
	/// </summary>
	/// <param name="user">Name of the user.</param>
	/// <param name="status">New status of that user.</param>
	/// <param name="gotMessage">True if the status contains a message you should cache locally. False: This status update does not include a
	/// message (keep any you have).</param>
	/// <param name="message">Message that user set.</param>
	public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
	{
		
		Debug.LogWarning("status: " + string.Format("{0} is {1}. Msg:{2}", user, status, message));
	}

    public void OnUserSubscribed(string channel, string user)
    {
        Debug.LogFormat("OnUserSubscribed: channel=\"{0}\" userId=\"{1}\"", channel, user);
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        Debug.LogFormat("OnUserUnsubscribed: channel=\"{0}\" userId=\"{1}\"", channel, user);
    }

    public void AddMessageToSelectedChannel(string msg)
	{
		ChatChannel channel = null;
		bool found = this.chatClient.TryGetChannel(this.selectedChannelName, out channel);
		if (!found)
		{
			Debug.Log("AddMessageToSelectedChannel failed to find channel: " + this.selectedChannelName);
			return;
		}
		
		if (channel != null)
		{
			channel.Add("Bot", msg, channel.LastMsgId);
		}
	}
	
	
	
	public void ShowChannel(string channelName)
	{
		if (string.IsNullOrEmpty(channelName))
		{
			return;
		}
		
		ChatChannel channel = null;
		bool found = this.chatClient.TryGetChannel(channelName, out channel);
		if (!found)
		{
			Debug.Log("ShowChannel failed to find channel: " + channelName);
			return;
		}
		
		this.selectedChannelName = channelName;
		this.CurrentChannelText.text = channel.ToStringMessages();
		Debug.Log("ShowChannel: " + this.selectedChannelName);
		
	}
	
	public void OpenDashboard()
	{
		Application.OpenURL("https://www.photonengine.com/en/Dashboard/Chat");
	}
	
	
	
	
}