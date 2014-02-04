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
	
#if UNITY_ANDROID
	AndroidJavaObject javaConnection = null;
	AndroidJavaObject javaChannel = null;
#else
	IConnection connection = null;
	IModel channel = null;
#endif

	const int HEADER_SIZE = 12;
	
	public static UMQNetworkManager instance;
	
	public static string udid = "admin2";
	
	public static Dictionary<int, Action<int>> actionDict = new Dictionary<int, Action<int>>();
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
	
	void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			DontDestroyOnLoad(gameObject);
			instance = this;
		}
	}
	
	// Use this for initialization
	void Start () {
	
		udid = SystemInfo.deviceUniqueIdentifier;
		
		sessionID = UnityEngine.Random.Range(0, int.MaxValue);
		
		udidKey = "client_udid_" + udid;
		udidQueueName = udidKey + "_" + sessionID + "_queue";

#if UNITY_ANDROID
		AndroidJavaObject factoryJava = new AndroidJavaObject("com.rabbitmq.client.ConnectionFactory");
		factoryJava.Call("setHost", "robot.lvl6.com");
		factoryJava.Call("setUsername", "lvl6client");
		factoryJava.Call("setPassword", "devclient");
		factoryJava.Call("setVirtualHost", "devmobsters");
#else
		ConnectionFactory factory = new ConnectionFactory();
		factory.HostName = "robot.lvl6.com";
		factory.UserName = "lvl6client";
		factory.Password = "devclient";
		factory.VirtualHost = "devmobsters";
#endif

		try{
#if UNITY_ANDROID
			javaConnection = factoryJava.Call<AndroidJavaObject>("newConnection");
#else
			connection = factory.CreateConnection();
#endif
			gameObject.SetActive(true);
		}
		catch (Exception e)
		{
			Debug.LogError("Connection exception: " + e);
			gameObject.SetActive(false);
			return;
		}

		
		try
		{
#if UNITY_ANDROID
			javaChannel = javaConnection.Call<AndroidJavaObject>("createChannel");
#else
			channel = connection.CreateModel();
#endif
			gameObject.SetActive(true);
		}
		catch (Exception e)
		{
			Debug.LogError("Channel error: " + e);
			gameObject.SetActive(false);
			return;
		}
		
		WriteDebug("Connected");

#if UNITY_ANDROID
		javaChannel.Call<AndroidJavaObject>("exchangeDeclare", directExchangeName, "direct", true);
		javaChannel.Call<AndroidJavaObject>("exchangeDeclare", topicExchangeName, "topic", true);

		javaChannel.Call("queueDeclare", udidQueueName, true, false, false, null);
		javaChannel.Call("queueBind", udidQueueName, directExchangeName, udidKey);

		AndroidJavaObject consumer = new AndroidJavaObject("com.rabbitmq.client.QueueingConsumer", javaChannel);
		javaChannel.Call("basicConsume", udidQueueName, false, consumer);

#else
		//Declare our exchanges
		channel.ExchangeDeclare(directExchangeName, ExchangeType.Direct, true);
		channel.ExchangeDeclare(topicExchangeName, ExchangeType.Topic, true);
		
		channel.QueueDeclare(udidQueueName, true, false, false, null);
		channel.QueueBind(udidQueueName, directExchangeName, udidKey);
		
		WriteDebug("Bounded with routing key: " + udidKey);
		
		QueueingBasicConsumer consumer = new QueueingBasicConsumer(channel);
		channel.BasicConsume(udidQueueName, false, consumer);

#endif
		StartCoroutine(Consume(consumer));
		
		ready = true;
	}
	
	public void CreateUserIDQueue(MinimumUserProto user)
	{
		string userIdKey = "client_userid_" + user.userId;
		string userIdKeyQueueName = userIdKey + "_" + sessionID + "_queue";

#if UNITY_ANDROID
		javaChannel.Call("queueDeclare", userIdKeyQueueName, true, false, false, null);
		javaChannel.Call("queueBind", userIdKeyQueueName, directExchangeName, userIdKey);
		
		AndroidJavaObject consumer = new AndroidJavaObject("QueueingConsumer", javaChannel);
		javaChannel.Call("basicConsume", userIdKeyQueueName, false, consumer);

#else
		channel.QueueDeclare(userIdKeyQueueName, true, false, false, null);
		channel.QueueBind(userIdKeyQueueName, directExchangeName, userIdKey);

		QueueingBasicConsumer consumer = new QueueingBasicConsumer(channel);
		channel.BasicConsume (userIdKeyQueueName, false, consumer);
#endif	
		StartCoroutine(Consume(consumer));
		
		chatQueueName = udidKey + "_" + sessionID + "_chat_queue";

#if UNITY_ANDROID
		//AndroidJavaObject chatChannel = javaConnection.Call<AndroidJavaObject>("createModel");

		Debug.LogWarning("Chat channel not implemented for Android yet");
#else
		IModel chatChannel = connection.CreateModel();

		chatChannel.QueueDeclare(chatQueueName, true, false, true, null);
		chatChannel.QueueBind(chatQueueName, topicExchangeName, chatKey);
		
		QueueingBasicConsumer chatConsumer = new QueueingBasicConsumer(chatChannel);
		chatChannel.BasicConsume(chatQueueName, false, chatConsumer);
		
		StartCoroutine(ConsumeChat(chatConsumer));
#endif

		WriteDebug("Set up userID Queue");
	}

	public void CreateClanChatQueue(MinimumUserProto user, int clanId)
	{
		Debug.Log("Creating clan chat queue for clanID: " + clanId);

		string userId = "client_userid_" + user.userId;

		string clanQueueName = userId + "_" + sessionID + " _clan_queue";

#if UNITY_ANDROID

#else
		IModel clanChatChannel = connection.CreateModel();

		clanChatChannel.QueueDeclare(clanQueueName, true, false, true, null);
		clanChatChannel.QueueBind(clanQueueName, topicExchangeName, "clan_" + clanId.ToString());
		
		QueueingBasicConsumer clanConsumer = new QueueingBasicConsumer(clanChatChannel);
		clanChatChannel.BasicConsume(clanQueueName, false, clanConsumer);

		StartCoroutine(ConsumeChat(clanConsumer));	
#endif
	}
	
	public int SendRequest(System.Object request, int type, Action<int> callback)
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

#if UNITY_ANDROID
		javaChannel.Call("basicPublish", directExchangeName, "messagesFromPlayers", null, message);
#else
		IBasicProperties properties = channel.CreateBasicProperties();
		properties.SetPersistent(true);
		channel.BasicPublish(directExchangeName, "messagesFromPlayers", properties, message);
#endif


		StartCoroutine(WaitRequestTimeout(tagNum));
		
		WriteDebug("Message sent: " + tagNum + ": " + request.GetType());
		
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

	IEnumerator Consume(AndroidJavaObject consumer)
	{
		AndroidJavaObject response;
		while(true)
		{
			response = consumer.Call<AndroidJavaObject>("nextDelivery");
			if (response != null)
			{
				ReceiveResponse(response);
			}
			yield return new WaitForSeconds(.5f);
		}
	}
	
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
				ReceiveChatResponse((BasicDeliverEventArgs)response);
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
		
		WriteDebug("Received Chat Message: " + tagNum);
		
		object proto = UMQDeserializer.Deserialize(reader, type);
		
		if (proto is ReceivedGroupChatResponseProto)
		{
			CBKChatManager.instance.ReceiveGroupChatMessage(proto as ReceivedGroupChatResponseProto);
		}
		
	}

	IEnumerator WaitRequestTimeout(int tagNum)
	{
		requestsOut.Add(tagNum);
		yield return new WaitForSeconds(30);
		if (requestsOut.Contains(tagNum))
		{
			Debug.LogWarning("Response never received for request: " + tagNum);
		}
	}

	void ReceiveResponse(AndroidJavaObject response)
	{
		ReceiveResponse(response.Call<byte[]>("getBody"));
	}

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
		
		if (proto is UpdateClientUserResponseProto || proto is PurgeClientStaticDataResponseProto)
		{
			Debug.Log("Update Client User Response");
		}
		else
		{

			responseDict[tagNum] = proto;
			
			if (actionDict[tagNum] != null)
			{
				actionDict[tagNum](tagNum);
			}

			requestsOut.Remove(tagNum);
		}
	}
	
	public void WriteDebug(string str)
	{
#if DEBUG
		//debugTextLabel.text += "\n" + str;
		Debug.Log(str);
#endif
	}
}
