#define DEBUG

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RabbitMQ;
using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using com.lvl6.proto;
using ProtoBuf;
using System.IO;

public class UMQNetworkManager : MonoBehaviour {
	
	static MySerializer ser = new MySerializer();
	
	//Dictionary<string, Type> classDict = new Dictionary<string, Type>();
	
	string directExchangeName = "gamemessages";
	string topicExchangeName = "chatmessages";
	string chatKey = "chat_global";
	
	string udidQueueName;
	string udidKey;
	string chatQueueName;
	
	int sessionID;
	
	static int tagNum = 1;
	
#if UNITY_ANDROID && !UNITY_EDITOR
	AndroidJavaObject javaConnection = null;
	AndroidJavaObject javaChannel = null;

	AndroidJavaObject javaChatChannel = null;
#else
	IConnection connection = null;
	IModel channel = null;
#endif

#if UNITY_EDITOR
	[SerializeField] bool forceNewUser = false;
#endif

	const int HEADER_SIZE = 12;
	
	public static UMQNetworkManager instance;

	[HideInInspector]
	public string udid = "admin2";
	
	Dictionary<int, Action<int>> actionDict = new Dictionary<int, Action<int>>();
	public static Dictionary<int, object> responseDict = new Dictionary<int, object>();
	
	[SerializeField]
	UILabel debugTextLabel;
	
	public bool ready = false;

	public int numRequestsOut
	{
		get
		{
			return requestsOut.Count;
		}
	}

	List<int> requestsOut = new List<int>();

	const float TIME_OUT = 15f;

	int attempts = 0;
	
	void Awake()
	{
		/*
		if (instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			DontDestroyOnLoad(gameObject);
			instance = this;
		}
		*/

		instance = this;

		attempts = 0;
	}
	
	// Use this for initialization
	public IEnumerator Start () {
	
		yield return new WaitForSeconds(.5f);

#if UNITY_EDITOR
		if (forceNewUser)
		{
			udid = MSUtil.timeNowMillis.ToString();
		}
		else
		{
			udid = SystemInfo.deviceUniqueIdentifier;
		}
#else
		udid = SystemInfo.deviceUniqueIdentifier;

#endif

		
		sessionID = UnityEngine.Random.Range(0, int.MaxValue);
		
		udidKey = "client_udid_" + udid;
		udidQueueName = udidKey + "_" + sessionID + "_queue";

#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaObject factoryJava = new AndroidJavaObject("com.rabbitmq.client.ConnectionFactory");
		factoryJava.Call("setHost", "staging.mobsters.lvl6.com");
		factoryJava.Call("setUsername", "lvl6client");
		factoryJava.Call("setPassword", "devclient");
		factoryJava.Call("setVirtualHost", "devmobsters");

		WriteDebug("Set connection settings...");
#else
		ConnectionFactory factory = new ConnectionFactory();
		factory.HostName = "staging.mobsters.lvl6.com";
		factory.UserName = "lvl6client";
		factory.Password = "devclient";
		factory.VirtualHost = "devmobsters";
		factory.Port = 5672;
#endif

		try{
#if UNITY_ANDROID && !UNITY_EDITOR
			javaConnection = factoryJava.Call<AndroidJavaObject>("newConnection");
#else
			connection = factory.CreateConnection();
#endif
			gameObject.SetActive(true);
		}
		catch (Exception e)
		{
			Debug.LogError("Connection exception: " + e);
			//gameObject.SetActive(false);
			if (++attempts < 5)
			{
				StartCoroutine(Start());
			}
			else
			{
				Debug.LogError("Max attempts failed.");
			}
			yield break;
		}

		Debug.Log("Created connection");
		
		try
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			javaChannel = javaConnection.Call<AndroidJavaObject>("createChannel");
			javaChatChannel = javaConnection.Call<AndroidJavaObject>("createChannel");
			WriteDebug("Created Channel");
#else
			channel = connection.CreateModel();
#endif
			gameObject.SetActive(true);
		}
		catch (Exception e)
		{
			Debug.LogError("Channel error: " + e);
			gameObject.SetActive(false);
			yield break;
		}
		
		Debug.Log("Connected");

#if UNITY_ANDROID && !UNITY_EDITOR
		javaChannel.Call<AndroidJavaObject>("exchangeDeclare", directExchangeName, "direct", true);
		javaChannel.Call<AndroidJavaObject>("exchangeDeclare", topicExchangeName, "topic", true);

		WriteDebug("Declared Exchanges");

		javaChannel.Call<AndroidJavaObject>("queueDeclare", udidQueueName, true, false, false, null);
		javaChannel.Call<AndroidJavaObject>("queueBind", udidQueueName, directExchangeName, udidKey);

		WriteDebug("Bounded with routing key: " + udidKey);

		AndroidJavaObject consumer = new AndroidJavaObject("com.rabbitmq.client.QueueingConsumer", javaChannel);
		javaChannel.Call<AndroidJavaObject>("basicConsume", udidQueueName, false, consumer);

		StartCoroutine(ConsumeJava(consumer, udidQueueName));
#else
		//Declare our exchanges
		channel.ExchangeDeclare(directExchangeName, ExchangeType.Direct, true);
		channel.ExchangeDeclare(topicExchangeName, ExchangeType.Topic, true);

		channel.QueueDeclare(udidQueueName, true, false, false, null);
		channel.QueueBind(udidQueueName, directExchangeName, udidKey);
		
		Debug.Log("Bounded with routing key: " + udidKey);
		
		QueueingBasicConsumer consumer = new QueueingBasicConsumer(channel);
		channel.BasicConsume(udidQueueName, false, consumer);
		
		StartCoroutine(Consume(consumer));
#endif

		Debug.Log("Ready");

		ready = true;
	}
	
	public void CreateUserIDQueue(MinimumUserProto user)
	{
		string userIdKey = "client_userid_" + user.userId;
		string userIdKeyQueueName = userIdKey + "_" + sessionID + "_queue";

#if UNITY_ANDROID && !UNITY_EDITOR
		javaChannel.Call<AndroidJavaObject>("queueDeclare", userIdKeyQueueName, true, false, false, null);
		javaChannel.Call<AndroidJavaObject>("queueBind", userIdKeyQueueName, directExchangeName, userIdKey);
		
		AndroidJavaObject consumer = new AndroidJavaObject("com.rabbitmq.client.QueueingConsumer", javaChannel);
		javaChannel.Call<AndroidJavaObject>("basicConsume", userIdKeyQueueName, false, consumer);

		WriteDebug("Declaring and binding to queue: " + userIdKeyQueueName);

		StartCoroutine(ConsumeJava(consumer, userIdKeyQueueName));
#else
		channel.QueueDeclare(userIdKeyQueueName, true, false, false, null);
		channel.QueueBind(userIdKeyQueueName, directExchangeName, userIdKey);

		QueueingBasicConsumer consumer = new QueueingBasicConsumer(channel);
		channel.BasicConsume (userIdKeyQueueName, false, consumer);

		StartCoroutine(Consume(consumer));
#endif	
		
		chatQueueName = udidKey + "_" + sessionID + "_chat_queue";

#if UNITY_ANDROID && !UNITY_EDITOR
		javaChatChannel.Call<AndroidJavaObject>("queueDeclare", chatQueueName, true, false, true, null);
		javaChatChannel.Call<AndroidJavaObject>("queueBind", chatQueueName, topicExchangeName, chatKey);
		
		AndroidJavaObject chatConsumer = new AndroidJavaObject("com.rabbitmq.client.QueueingConsumer", javaChatChannel);
		javaChatChannel.Call<AndroidJavaObject>("basicConsume", chatQueueName, false, chatConsumer);

		WriteDebug("Declaring and binding to queue: " + chatQueueName);

		StartCoroutine(ConsumeJava(chatConsumer, chatQueueName));
#else
		IModel chatChannel = connection.CreateModel();

		chatChannel.QueueDeclare(chatQueueName, true, false, true, null);
		chatChannel.QueueBind(chatQueueName, topicExchangeName, chatKey);
		
		QueueingBasicConsumer chatConsumer = new QueueingBasicConsumer(chatChannel);
		chatChannel.BasicConsume(chatQueueName, false, chatConsumer);
		
		StartCoroutine(ConsumeChat(chatConsumer));
#endif
		Debug.Log("Set up userID Queue");
	}

	public void CreateClanChatQueue(MinimumUserProto user, int clanId)
	{
		Debug.Log("Creating clan chat queue for clanID: " + clanId);

		string userId = "client_userid_" + user.userId;

		string clanQueueName = userId + "_" + sessionID + " _clan_queue";

#if UNITY_ANDROID && !UNITY_EDITOR

#else
		IModel clanChatChannel = connection.CreateModel();

		clanChatChannel.QueueDeclare(clanQueueName, true, false, true, null);
		clanChatChannel.QueueBind(clanQueueName, topicExchangeName, "clan_" + clanId.ToString());
		
		QueueingBasicConsumer clanConsumer = new QueueingBasicConsumer(clanChatChannel);
		clanChatChannel.BasicConsume(clanQueueName, false, clanConsumer);

		StartCoroutine(ConsumeChat(clanConsumer));	
#endif
	}
	
	public int SendRequest(System.Object request, int type, Action<int> callback = null)
	{
		MemoryStream body = new MemoryStream();
		
		if (request == null)
		{
			Debug.LogError("Bad!");
		}
		if (ser == null)
		{
			Debug.LogError("also bad!");
		}
		if (body == null)
		{
			Debug.LogError("How does that even...");
		}
		
		ser.Serialize(body, request);
		
		int size = (int)body.Length;
		byte[] message = new byte[HEADER_SIZE + (int)body.Length];
	
		message[3] = (byte)(type & 0xFF);
		message[2] = (byte)((type & 0xFF00) >> 8);
		message[1] = (byte)((type & 0xFF0000) >> 16);
		message[0] = (byte)((type & 0xFF000000) >> 24);
		
		message[7] = (byte)(tagNum & 0xFF);
		message[6] = (byte)((tagNum & 0xFF00) >> 8);
		message[5] = (byte)((tagNum & 0xFF0000) >> 16);
		message[4] = (byte)((tagNum & 0xFF000000) >> 24);
  
		message[11] = (byte)(size & 0xFF);
		message[10] = (byte)((size & 0xFF00) >> 8);
		message[9] = (byte)((size & 0xFF0000) >> 16);
		message[8] = (byte)((size & 0xFF000000) >> 24);
		
		byte[] bodyBuffer = body.GetBuffer();
		
		for (int i = 0; i < body.Length; i++) {
			message[HEADER_SIZE + i] = bodyBuffer[i];
		}
		
		//PrintByteStream(message);

#if UNITY_ANDROID && !UNITY_EDITOR
		javaChannel.Call("basicPublish", directExchangeName, "messagesFromPlayers", null, message);
#else
		IBasicProperties properties = channel.CreateBasicProperties();
		properties.SetPersistent(true);
		channel.BasicPublish(directExchangeName, "messagesFromPlayers", properties, message);
#endif


		StartCoroutine(WaitRequestTimeout(tagNum));
		
		Debug.Log("Message sent: " + tagNum + ": " + request.GetType());
		
		actionDict.Add(tagNum, callback);

		return tagNum++;
	}
	
	void PrintByteStream(byte[] msg)
	{
		string str = "";
		for (int i = 0; i < msg.Length; i++) {
			str += msg[i] + " ";
		}
		Debug.Log(str);
	}

#if UNITY_ANDROID && !UNITY_EDITOR
	IEnumerator ConsumeJava(AndroidJavaObject consumer, string queueName)
	{
		WriteDebug("Starting java consume for consumer");

		if (consumer.GetRawObject().ToInt32() == 0) Debug.Log("Problem with consumer");

		AndroidJavaObject response;
		while(true)
		{
			try
			{
				response = consumer.Call<AndroidJavaObject>("nextDelivery", 0L);
				if (response.GetRawObject().ToInt32() != 0)
				{
					ReceiveResponse(response);
				}
			}
			catch (Exception e)
			{
				string dump = e.ToString();
				//WriteDebug("Problem receiving response on " + queueName + ": " + e.ToString());
			}
			yield return new WaitForSeconds(.5f);
		}
	}
#endif
	
	IEnumerator Consume(QueueingBasicConsumer consumer)
	{
		object response;
		while(true)
		{
			response = consumer.Queue.DequeueNoWait(null);
			if (response != null)
			{
				ReceiveResponse((BasicDeliverEventArgs)response);
			}
			yield return new WaitForSeconds(.5f);
		}
	}
	
	IEnumerator ConsumeChat(QueueingBasicConsumer consumer)
	{
		object response;
		while(true)
		{
			response = consumer.Queue.DequeueNoWait(null);
			if (response != null)
			{
				ReceiveResponse((BasicDeliverEventArgs)response);
			}
			yield return new WaitForSeconds(.5f);
		}
	}
	
	void ReceiveChatResponse(BasicDeliverEventArgs response)
	{
		MemoryStream reader = new MemoryStream(response.Body);
		
		EventProtocolResponse type = (EventProtocolResponse) ((reader.ReadByte()) + (reader.ReadByte() << 8) + (reader.ReadByte() << 16) + (reader.ReadByte() << 24));
		
		int tagNum = ((reader.ReadByte()) + (reader.ReadByte() << 8) + (reader.ReadByte() << 16) + (reader.ReadByte() << 24));
		
		//Supress warning on this line.
		//Even though we're not using the value of size, 
		//the ReadByte calls progress the stream to set it up for the deserializing
#pragma warning disable 0219
		int size = ((reader.ReadByte()) + (reader.ReadByte() << 8) + (reader.ReadByte() << 16) + (reader.ReadByte() << 24));
#pragma warning restore 0219
		
		Debug.Log("Received Chat Message: " + tagNum);
		
		object proto = UMQDeserializer.Deserialize(reader, type);
		
		if (proto is ReceivedGroupChatResponseProto)
		{
			MSChatManager.instance.ReceiveGroupChatMessage(proto as ReceivedGroupChatResponseProto);
		}
		if (proto is BeginClanRaidResponseProto && MSClanEventManager.instance != null)
		{
			Debug.Log("Fallback: From other");
			MSClanEventManager.instance.DealWithBeginResponse(proto as BeginClanRaidResponseProto);
		}
		if (proto is AttackClanRaidMonsterResponseProto && MSClanEventManager.instance != null)
		{
			Debug.Log("Fallback: From other");
			MSClanEventManager.instance.DealWithAttackResponse(proto as AttackClanRaidMonsterResponseProto);
		}
	}
	
	IEnumerator WaitRequestTimeout(int tagNum)
	{
		requestsOut.Add(tagNum);
		yield return new WaitForSeconds(60);
		if (requestsOut.Contains(tagNum))
		{
			//Debug.LogWarning("Response never received for request: " + tagNum);
			MSSceneManager.instance.ReconnectPopup();
		}
	}

#if UNITY_ANDROID && !UNITY_EDITOR
	void ReceiveResponse(AndroidJavaObject response)
	{
		ReceiveResponse(response.Call<byte[]>("getBody"));
	}
#endif

	void ReceiveResponse(BasicDeliverEventArgs response)
	{
		ReceiveResponse(response.Body);
	}
	
	void ReceiveResponse(byte[] response)
	{	
		MemoryStream reader = new MemoryStream(response);
		
		EventProtocolResponse type = (EventProtocolResponse) ((reader.ReadByte()) + (reader.ReadByte() << 8) + (reader.ReadByte() << 16) + (reader.ReadByte() << 24));
		
		int tagNum = ((reader.ReadByte()) + (reader.ReadByte() << 8) + (reader.ReadByte() << 16) + (reader.ReadByte() << 24));
		
		//Supress warning on this line.
		//Even though we're not using the value of size, 
		//the ReadByte calls progress the stream to set it up for the deserializing
#pragma warning disable 0219
		int size = ((reader.ReadByte()) + (reader.ReadByte() << 8) + (reader.ReadByte() << 16) + (reader.ReadByte() << 24));
#pragma warning restore 0219
		
		object proto = UMQDeserializer.Deserialize(reader, type);
		
		Debug.Log("Received Message: " + tagNum + ": " + proto.GetType());
		
		if (proto is UpdateClientUserResponseProto)
		{
			Debug.Log("Update Client User Response");
		}
		else if (proto is PurgeClientStaticDataResponseProto)
		{
			Debug.Log("Purging static data");
			MSDataManager.instance.LoadStaticData(proto as PurgeClientStaticDataResponseProto);
		}
		else if (proto is ForceLogoutResponseProto)
		{
			ForceLogoutResponseProto logout = proto as ForceLogoutResponseProto;
			Debug.LogWarning("Force logout: " + logout.previousLoginTime);
		}
		else if (proto is ReceivedGroupChatResponseProto)
		{
			MSChatManager.instance.ReceiveGroupChatMessage(proto as ReceivedGroupChatResponseProto);
		}
		else if (proto is BeginClanRaidResponseProto && MSClanEventManager.instance != null)
		{
			Debug.Log("Fallback: From other");
			MSClanEventManager.instance.DealWithBeginResponse(proto as BeginClanRaidResponseProto);
		}
		else if (proto is AttackClanRaidMonsterResponseProto && MSClanEventManager.instance != null)
		{
			Debug.Log("Fallback: From other");
			MSClanEventManager.instance.DealWithAttackResponse(proto as AttackClanRaidMonsterResponseProto);
		}
		else
		{

			responseDict[tagNum] = proto;
			
			if (actionDict.ContainsKey(tagNum) && actionDict[tagNum] != null)
			{
				actionDict[tagNum](tagNum);
			}
			else 
			{
				if (proto is AcceptAndRejectFbInviteForSlotsResponseProto && MSResidenceManager.instance != null)
				{
					MSResidenceManager.instance.JustReceivedFriendAccept(proto as AcceptAndRejectFbInviteForSlotsResponseProto);
				}
				if (proto is InviteFbFriendsForSlotsResponseProto && MSRequestManager.instance != null)
				{
					MSRequestManager.instance.JustReceivedFriendInvite(proto as InviteFbFriendsForSlotsResponseProto);
				}
				if (proto is PrivateChatPostResponseProto && MSChatManager.instance != null)
				{
					MSChatManager.instance.ReceivePrivateChatMessage(proto as PrivateChatPostResponseProto);
				}
			}

			requestsOut.Remove(tagNum);
		}
	}

	void WriteDebug(string debug)
	{
		if (debugTextLabel != null)
		{
			debugTextLabel.text += "\n" + debug;
		}
		else
		{
			Debug.LogWarning(debug);
		}
	}

	void OnApplicationQuit()
	{
		LogoutRequestProto request = new LogoutRequestProto();
		request.sender = MSWhiteboard.localMup;
		SendRequest(request, (int)EventProtocolRequest.C_LOGOUT_EVENT);
	}
}
