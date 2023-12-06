using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.NetworkInformation;
using System.Collections;
using System.Net;
using System.Net.Sockets;

public enum MessageType
{
    LOGIN,           // 0  C->S 登录请求 一个字符串 用户名和密码
    LOGOUT,          // 1  C->S 登出 空串
    ALL_CARD,        // 2  S->C 所有用户和群聊 List<CardInfo>
    ERROR,           // 3  S->C C->S 字符串错误信息
    OK,              // 4  S->C 字符串
    SEND,            // 5  C->S C->C 用户发送信息 SendMessage
    RECEIVE,         // 6  S->C 用户接收到信息 SendMessage
    REQUEST_DIALOG,  // 7  C->S 请求某个聊天记录 Dialog
    DIALOG_INFO,     // 8  S->C 返回聊天记录 Dialog 其中 TalkInfo 为 TalkMessages序列化
    SEND_OK,         // 9  S->C 消息发送成功 字符串: 发送成功的消息
    REGISTER,        // 10 C->S 注册请求 一个字符串 名称和密码
    CALL_REQUEST,    // 11 C->S 用户请求和另一个用户发送通话  S->C 有用户请求通话
    CALL_DENY,       // 12 C->S S->C 拒绝通话
    CALL_ACCEPT,     // 13 C->S S->C 接受通话 
    SENDFILE,        // 14 C->C 表示接下来要发送文件
}
public enum CardType
{
    GROUP,      // 0
    USER        // 1
}

[Serializable]
public struct Message
{
    public MessageType MsgType { get; set; }
    public string MsgInfo { get; set; }
    public Message(MessageType msgType, string info)
    {
        MsgType = msgType;
        MsgInfo = info;
    }
}
[Serializable]
public struct SendMessage
{
    public CardType MsgType { get; set; }     // 是否为多对多信息
    public string SrcId { get; set; }
    public string DestId { get; set; }
    public string MsgInfo { get; set; }
    public SendMessage(CardType MsgType, string src, string destId, string msgInfo)
    {
        this.MsgType = MsgType; SrcId = src; DestId = destId; MsgInfo = msgInfo;
    }
}
[Serializable]
public class Dialog
{
    public CardType type { get; set; }
    public string src_id;
    public string dest_id;
    public string TalkInfo;
    public Dialog(CardType type, string src_id, string dest_id, string talkInfo)
    {
        this.type = type;
        this.src_id = src_id;
        this.dest_id = dest_id;
        TalkInfo = talkInfo;
    }
}
[Serializable]
public class CardInfo
{
    public CardType type;
    public string dest_id;
    public string dest_name;
    public CardInfo(CardType type, string dest_id, string dest_name)
    {
        this.type = type;
        this.dest_id = dest_id;
        this.dest_name = dest_name;
    }
}
[Serializable]
public class OneTalkMessage         // 对话中的一条消息
{
    public string sender;
    public DateTime send_time;
    public string message;
    public OneTalkMessage(string sender, DateTime send_time, string message)
    {
        this.sender = sender;
        this.send_time = send_time;
        this.message = message;
    }
    public string Format()
    {
        return $"{sender}  {send_time}\r\n{message}\r\n\r\n";
    }
}
[Serializable]
public class TalkMessages : List<OneTalkMessage>
{
    public TalkMessages() : base() { }
    public string Format()
    {
        string TalkInfo = "";
        foreach (OneTalkMessage message in this)
        {
            TalkInfo += message.Format();
        }
        return TalkInfo;
    }
}

internal class Parser
{
    public static T String2Obj<T>(string msg)
    {
        return JsonConvert.DeserializeObject<T>(msg);
    }
    public static string Obj2String<T>(T msg)
    {
        return JsonConvert.SerializeObject(msg);
    }
    public static string NewErrorMessage(string msg)
    {
        return Obj2String(new Message(MessageType.ERROR, msg));
    }
}

internal class NetworkUtil
{
    private static byte[] buffer = new byte[1024 * 1024];
    public static void SendToStream(Stream stream, string data)
    {
        byte[] message = Encoding.UTF8.GetBytes(data);
        stream.Write(message, 0, message.Length);
        Console.WriteLine("Sent: {0}", data);
    }
    public static string ReadFromStream(Stream stream)
    {
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        return Encoding.UTF8.GetString(buffer, 0, bytesRead);
    }


    public static string GetClientIP(TcpClient client)
    {
        return client.Client.RemoteEndPoint.ToString().Split(":")[0];
    }

    #region 获取可用端口
    private static int MAX_PORT_NUM = 65535;
    private static IList PortIsUsed()
    {
        // 获取本地计算机的网络连接和通信统计数据的信息
        IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();

        //返回本地计算机上的所有Tcp监听程序
        IPEndPoint[] ipsTCP = ipGlobalProperties.GetActiveTcpListeners();

        //返回本地计算机上的所有UDP监听程序
        IPEndPoint[] ipsUDP = ipGlobalProperties.GetActiveUdpListeners();

        //返回本地计算机上的Internet协议版本4(IPV4 传输控制协议(TCP)连接的信息。
        TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

        IList allPorts = new ArrayList();
        foreach (IPEndPoint ep in ipsTCP) allPorts.Add(ep.Port);
        foreach (IPEndPoint ep in ipsUDP) allPorts.Add(ep.Port);
        foreach (TcpConnectionInformation conn in tcpConnInfoArray) allPorts.Add(conn.LocalEndPoint.Port);

        return allPorts;
    }

    public static int GetAvailablePort()
    {
        int BEGIN_PORT = 5000;

        IList portUsed = PortIsUsed();
        if (portUsed.Count == MAX_PORT_NUM)
            return -1;
        for (int i = BEGIN_PORT; i < MAX_PORT_NUM; i++)
        {
            if (!portUsed.Contains(i))
                return i;
        }
        return -1;
    }
    #endregion
}
