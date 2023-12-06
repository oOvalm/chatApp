using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class SqlUtil
{
    private static string myConnStr = @"server=localhost;User Id=root; password=123456;database=chatdb;Character Set=utf8";
    private static MySqlConnection myConn;         // Mysql 链接对象
    private static MySqlDataAdapter myMda;         // Mysql 数据适配器

    // 初始化数据库
    public static void Init()
    {
        myConn = new MySqlConnection(myConnStr);
        myMda = new MySqlDataAdapter();
    }

    private static DataSet SelectSql(string sqlstr)
    {
        DataSet ds = new DataSet();
        lock (myConn)
        {
            myConn.Open();
            myMda.SelectCommand = new MySqlCommand(sqlstr, myConn);
            myMda.Fill(ds, "info");
            myConn.Close();
        }
        return ds;
    }
    private static void UpdateSql(string sqlstr)
    {
        lock (myConn)
        {
            myConn.Open();
            (new MySqlCommand(sqlstr, myConn)).ExecuteNonQuery();
            myConn.Close();
        }
    }
    private static void InsertSql(string sqlstr)
    {
        lock (myConn)
        {
            myConn.Open();
            (new MySqlCommand(sqlstr, myConn)).ExecuteNonQuery();
            myConn.Close();
        }
    }

    public static int UserState(string user_id)
    {
        bool tmp;
        DataSet ds = SelectSql($"select login from user where user_id={user_id}");
        if (ds.Tables[0].Rows.Count == 0) return -1;
        tmp = (bool)ds.Tables[0].Rows[0]["login"];
        return tmp?1:0;
    }

    public static bool PswCorrect(string user_id, string user_psw)
    {
        bool tmp;
        DataSet ds = SelectSql($"select * from user where user_id={user_id} and user_psw={user_psw}");
        return ds.Tables[0].Rows.Count != 0;
    }


    public static void SetUserState(string user_id, bool state)
    {
        UpdateSql($"update user set login={state} where user_id={user_id}");
    }

    public static string QueryUserName(string user_id)
    {
        DataSet ds = SelectSql($"select user_name from user where user_id={user_id}");
        if (ds.Tables[0].Rows.Count==0)
        {
            throw new Exception($"connot find the user {user_id}");
        }
        return (string)ds.Tables[0].Rows[0]["user_name"];
    }

    // 返回所有用户
    public static List<CardInfo> GetAllUserNGroup(string src_id)
    {
        List<CardInfo> list = new List<CardInfo>();
        // 查所有群组
        DataSet ds = SelectSql($"select group_id, group_name from group_table natural join group_user where group_user.user_id={src_id}");
        foreach(DataRow row in ds.Tables[0].Rows)
        {
            list.Add(new CardInfo(CardType.GROUP, row["group_id"].ToString(), (string)row["group_name"]));
        }
        // 查所有用户
        ds = SelectSql("select user_id, user_name from user");
        foreach (DataRow row in ds.Tables[0].Rows)
        {
            list.Add(new CardInfo(CardType.USER, row["user_id"].ToString(), (string)row["user_name"]));
        }
        return list;
    }

    // 查询2个用户之间的聊天记录
    public static TalkMessages QueryUserDialogInfo(string src_user, string dest_user)
    {
        string src_name = Program.ClientMap[src_user].user_name;
        string dest_name = QueryUserName(dest_user);
        string src_dest_set = $"({src_user},{dest_user})";
        TalkMessages res = new TalkMessages();
        DataSet ds = SelectSql($"select sender, send_time, content from user_chat_record where sender in {src_dest_set} and receiver in {src_dest_set} order by send_time");
        foreach(DataRow row in ds.Tables[0].Rows)
        {
            if (row["sender"].ToString().Equals(src_user))
            {
                res.Add(new OneTalkMessage(src_name, (DateTime)row["send_time"], (string)row["content"]));
            }
            else
            {
                res.Add(new OneTalkMessage(dest_name, (DateTime)row["send_time"], (string)row["content"]));
            }
        }
        return res;
    }

    // 新对话信息 一对一
    public static void NewDialogInfo_P2P(SendMessage msg, DateTime timeStamp)
    {
        InsertSql($"insert into user_chat_record(sender, receiver, send_time, content) values (\"{msg.SrcId}\", \"{msg.DestId}\", \"{timeStamp}\", \"{msg.MsgInfo}\")");
    }


    // 插入新用户
    public static string AddUser(string username, string userpsw)
    {
        DataSet ds = SelectSql("select MAX(user_id) as mx_uid from user");
        int id = (int)ds.Tables[0].Rows[0]["mx_uid"];
        id++;
        InsertSql($"insert into user(user_id, user_name, user_psw, login) values (\"{id}\", \"{username}\", \"{userpsw}\", 0)");
        // 插入到全体成员的群组中
        InsertSql($"insert into group_user(group_id, user_id) values (\"1000000\", \"{id}\")");
        return id.ToString();
    }

    #region 群组相关
    public static TalkMessages QueryGroupDialogInfo(string group_id)
    {
        TalkMessages res = new TalkMessages();
        DataSet ds = SelectSql($"select user.user_name, send_time, content " +
            $"from group_chat_record left join user on user.user_id=group_chat_record.sender " +
            $"where group_id = {group_id} order by send_time");
        foreach(DataRow row in ds.Tables[0].Rows)
        {
            res.Add(new OneTalkMessage((string)row["user_name"], (DateTime)row["send_time"], (string)row["content"]));
        }
        return res;
    }
    public static void NewDialogInfo_Group(SendMessage msg, DateTime timeStamp)
    {
        InsertSql($"insert into group_chat_record(group_id, sender, send_time, content) values (\"{msg.DestId}\", \"{msg.SrcId}\", \"{timeStamp}\",\"{msg.MsgInfo}\")");
    }
    public static List<string> QueryGroupMember(string group_id)
    {
        List<string> group_member = new List<string>();
        DataSet ds = SelectSql($"select user_id from group_user where group_id = {group_id}");
        foreach(DataRow row in ds.Tables[0].Rows)
        {
            group_member.Add(row["user_id"].ToString());
        }
        return group_member;
    }
    #endregion
}
