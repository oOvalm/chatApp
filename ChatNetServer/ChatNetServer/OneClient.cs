using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

internal class OneClient
{
    TcpClient tcpClient;
    NetworkStream stream;
    public string user_id;
    public string user_name;
    private bool isconn;
    public OneClient(TcpClient client)
    {
        isconn = true;
        stream = client.GetStream();
        tcpClient = client;

        byte[] buffer = new byte[1024*1024*64];
        int bytesRead;
        try
        {
            while (isconn)
            {
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("Received: {0}", data);

                    ResolveMessage(Parser.String2Obj<Message>(data));
                }
            }
        }catch (IOException ex) {
            Console.WriteLine($"error: {ex}");
        }
        finally
        {
            // Close the client's connection
            client.Close();
            if (user_id != null)
            {
                Program.ClientMap.Remove(user_id);
                SqlUtil.SetUserState(user_id, false);
            }
            Console.WriteLine("Client disconnected");
        }
    }

    private void ResolveMessage(Message message)
    {
        switch (message.MsgType)
        {
            
            case MessageType.LOGIN:     // 用户登录
                Login(message.MsgInfo);
                break;
            case MessageType.SEND:      // 用户发送信息
                SendMsg(message.MsgInfo);
                break;
            case MessageType.REQUEST_DIALOG:   // 用户请求和聊天记录
                RequestDialogInfo(message.MsgInfo); 
                break;
            case MessageType.CALL_REQUEST:      // 用户请求通话
                RequestUserCall(message.MsgInfo);
                break;
            case MessageType.CALL_DENY:
                DenyUserCall(message.MsgInfo);
                break;
            case MessageType.CALL_ACCEPT:
                AcceptUserCall(message.MsgInfo);
                break;
            case MessageType.REGISTER:  // 用户注册
                RequestRegister(message.MsgInfo);
                break;
            case MessageType.LOGOUT:    // 用户登出
                isconn = false;
                break;
            default:
                Console.WriteLine($"unkown message type {message}");
                NetworkUtil.SendToStream(stream, 
                    Parser.NewErrorMessage("unkown message type"));
                break;
        }
    }

    // 如果登录成功则返回所有用户
    private void Login(string LoginMsg)
    {
        string[] strs = LoginMsg.Split(' ');
        if(strs.Length != 2) {
            NetworkUtil.SendToStream(stream, Parser.NewErrorMessage("unkown login message!"));
        }
        string user_id = strs[0];
        string user_psw = strs[1];
        int state = SqlUtil.UserState(user_id);
        if(state == -1)
        {
            NetworkUtil.SendToStream(stream, Parser.NewErrorMessage("unkown user id"));
            return;
        }
        if (state == 1)
        {
            NetworkUtil.SendToStream(stream, Parser.NewErrorMessage("the user has been logged in"));
            return;
        }
        if (!SqlUtil.PswCorrect(user_id, user_psw))
        {
            NetworkUtil.SendToStream(stream, Parser.NewErrorMessage("incorrcet password!"));
            return;
        }
        SqlUtil.SetUserState(user_id, true);
        this.user_id = user_id;
        this.user_name = SqlUtil.QueryUserName(user_id);
        if(!Program.ClientMap.ContainsKey(user_id))
            Program.ClientMap.Add(user_id, this);
        NetworkUtil.SendToStream(stream, Parser.Obj2String(new Message(MessageType.OK, "")));   // 登录成功
        NetworkUtil.SendToStream(stream, Parser.Obj2String(new Message(MessageType.ALL_CARD, Parser.Obj2String(SqlUtil.GetAllUserNGroup(user_id)))));   // 发送所有用户
    }
    
    // 一方发送数据
    private void SendMsg(string MsgInfo)
    {
        SendMessage msg = Parser.String2Obj<SendMessage>(MsgInfo);
        if (!msg.SrcId.Equals(user_id))
        {
            Console.WriteLine("ERRRORRORRRR!!!!!!   发送方和包的发送方不一致！！！\n");
            return;
        }
        // 存信息
        var noww = DateTime.Now;

        if (msg.MsgType == CardType.USER)        // 一对一
        {
            SqlUtil.NewDialogInfo_P2P(msg, noww);
            msg.MsgInfo = $"{user_name}  {noww}\r\n{msg.MsgInfo}\r\n\r\n";
            NetworkUtil.SendToStream(stream, Parser.Obj2String(new Message(MessageType.SEND_OK, msg.MsgInfo)));
            if (SqlUtil.UserState(msg.DestId) == 0) // 对方不在线
                return;
            Program.ClientMap[msg.DestId].ReceiveMsg(msg);
        }
        else    // 多对多
        {
            SqlUtil.NewDialogInfo_Group(msg, noww);
            msg.MsgInfo = $"{user_name}  {noww}\r\n{msg.MsgInfo}\r\n\r\n";
            NetworkUtil.SendToStream(stream, Parser.Obj2String(new Message(MessageType.SEND_OK, msg.MsgInfo)));
            List<string> members = SqlUtil.QueryGroupMember(msg.DestId);
            foreach(var mem in members) if(Program.ClientMap.ContainsKey(mem) && !mem.Equals(msg.SrcId)){
                Program.ClientMap[mem].ReceiveMsg(msg);
            }
        }
    }

    // 向聊天的接收方发送数据
    private void ReceiveMsg(SendMessage msg)
    {
        NetworkUtil.SendToStream(stream, Parser.Obj2String(new Message(MessageType.RECEIVE, Parser.Obj2String(msg))));
    }

    // 请求聊天记录
    private void RequestDialogInfo(string MsgInfo)
    {
        Dialog dialog = Parser.String2Obj<Dialog>(MsgInfo);
        if (dialog.type == CardType.USER)
        {
            dialog.TalkInfo = Parser.Obj2String(SqlUtil.QueryUserDialogInfo(dialog.src_id, dialog.dest_id));
            NetworkUtil.SendToStream(stream, Parser.Obj2String(new Message(MessageType.DIALOG_INFO, Parser.Obj2String(dialog))));
        }
        else if (dialog.type == CardType.GROUP)
        {
            dialog.TalkInfo = Parser.Obj2String(SqlUtil.QueryGroupDialogInfo(dialog.dest_id));
            NetworkUtil.SendToStream(stream, Parser.Obj2String(new Message(MessageType.DIALOG_INFO, Parser.Obj2String(dialog))));
        }
        else Debug.Assert(false);
    }

    // 用户注册
    private void RequestRegister(string MsgInfo)
    {
        string[] strs = MsgInfo.Split();
        if (strs.Length != 2)
        {
            NetworkUtil.SendToStream(stream, Parser.NewErrorMessage("unkown register message!"));
        }
        string user_name = strs[0], user_psw = strs[1];
        string user_id = SqlUtil.AddUser(user_name, user_psw);
        NetworkUtil.SendToStream(stream, Parser.Obj2String(new Message(MessageType.OK, user_id)));
    }

    // 请求呼叫
    private void RequestUserCall(string MsgInfo)
    {
        string dest_id = MsgInfo;
        if (SqlUtil.UserState(dest_id) == 0)
        {
            NetworkUtil.SendToStream(stream, Parser.Obj2String(new Message(MessageType.CALL_DENY, $"offline {SqlUtil.QueryUserName(dest_id)}({dest_id}) 不在线")));
            return;
        }
        OneClient otherClient = Program.ClientMap[dest_id];
        string user_ip = NetworkUtil.GetClientIP(otherClient.tcpClient);
        // 向被呼叫用户发送信息
        NetworkUtil.SendToStream(otherClient.stream, Parser.Obj2String(new Message(MessageType.CALL_REQUEST, $"{user_id} {user_ip}")));
    }

    private void DenyUserCall(string msg)
    {
        string dest_id = msg.Split(' ')[0];
        string res = msg.Substring(msg.IndexOf(' '));
        OneClient otherClient = Program.ClientMap[dest_id];
        // 向被呼叫用户转发信息
        NetworkUtil.SendToStream(otherClient.stream, Parser.Obj2String(new Message(MessageType.CALL_DENY, res)));
    }

    private void AcceptUserCall(string msg)
    {
        string[] str = msg.Split(" ");
        string dest_id = str[0];
        string dest_port = str[1];
        OneClient otherClient = Program.ClientMap[dest_id];
        string dest_ip = NetworkUtil.GetClientIP(otherClient.tcpClient);
        NetworkUtil.SendToStream(otherClient.stream, Parser.Obj2String(new Message(MessageType.CALL_ACCEPT, $"{user_id} {dest_ip} {dest_port}")));
    }
}
