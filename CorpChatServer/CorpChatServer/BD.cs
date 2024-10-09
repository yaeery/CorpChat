using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace CorpChatServer
{
    class BD
    {
        SqlConnection sqlConnection;
        public BD(string Pyt)
        {
            sqlConnection = new SqlConnection(Pyt);
        }
        public BD() { }
        public void Open_Conection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Closed)
            {
                sqlConnection.Open();
            }
        }
        public void Close_Conection()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Open)
            {
                sqlConnection.Close();
            }
        }
        public SqlConnection Get_Connection()
        {
            return sqlConnection;
        }

        //private List<string> Decoding_LP(string Msg)
        //{
        //    string Prom = "";
        //    List<string> List_Return = new List<string>();
        //    for (int j = 3; j < Msg.Length; j++)
        //    {
        //        if (Msg[j] == '#')
        //        {
        //            List_Return.Add(Prom);
        //            Prom = "";
        //            continue;
        //        }
        //        else
        //        {
        //            Prom += Msg[j];
        //        }
        //    }
        //    List_Return.Add(Prom);
        //    return List_Return;
        //}

        public string Get_Result_Access(string Login, string Password)
        {
            SqlCommand sqlCommand_Cl = new SqlCommand("select ID from Main where Login='"+ Login + "'and Password = '"+ Password + "'", sqlConnection);
            object Type = sqlCommand_Cl.ExecuteScalar();
            if ((Type == null))
            {
                return 'N'.ToString();
            }
            else
            {
                return Type.ToString();
            }
        }
        public void Get_Hystory_Message(string ID_Sender, string ID_Recipient, string Count_of_Message, List<string> List)
        {
            SqlCommand sqlCommand_Cl = new SqlCommand("select Sender, Date,Msg from Hstory where Sender = '" + ID_Sender + "' and Recipient= '"+ ID_Recipient + "'union select Sender, Date,Msg from Hstory where Sender = '" + ID_Recipient + "' and Recipient= '"+ ID_Sender + "' order by Date desc", sqlConnection);
            SqlDataReader Type = sqlCommand_Cl.ExecuteReader();
            int Count_Start = Convert.ToInt32(Count_of_Message)*20;
            int Count_End = Count_Start + 20;
            int Count = 0;
            while (Type.Read())
            {
                if ((Count >= Count_Start) && (Count < Count_End))
                {
                    if (Type[0].ToString() == ID_Sender)
                    {
                        List.Add("F");//first
                    }
                    else if (Type[0].ToString() == ID_Recipient)
                    {
                        List.Add("S");//second
                    }
                    List.Add((Convert.ToDateTime(Type[1].ToString())).ToString("H: mm dd.MM.yy"));
                    List.Add(Type[2].ToString());
                }
                Count++;
            }
            Type.Close();
        }
        public void Get_Dilogs(string ID,List<string> FIO_ID_for_All_Dialogs)
        {
            SqlCommand sqlCommand_Cl = new SqlCommand("select FIO,ID  from Main, Contacts where  ID in (select distinct Resiver from Contacts where Sender = '"+ ID+ "') and ID = Resiver and Sender = '"+ ID+ "' group by FIO, ID,LastConect order by LastConect desc", sqlConnection);
            SqlDataReader Type = sqlCommand_Cl.ExecuteReader();
            while (Type.Read())
            {
                FIO_ID_for_All_Dialogs.Add(Type[0].ToString());
                FIO_ID_for_All_Dialogs.Add(Type[1].ToString());
            }
            Type.Close();
        }
        public void Get_Find_Dilog(string ID, string FIO, List<string> FIO_ID_for_All_Dialogs)
        {
            SqlCommand sqlCommand_Cl = new SqlCommand("select FIO, ID from Main where FIO like '%"+ FIO + "%'and ID not in (select  Resiver from Contacts where Sender ='"+ ID + "') and ID <>'"+ ID + "'", sqlConnection);
            SqlDataReader Type = sqlCommand_Cl.ExecuteReader();
            while (Type.Read())
            {
                FIO_ID_for_All_Dialogs.Add(Type[0].ToString());
                FIO_ID_for_All_Dialogs.Add(Type[1].ToString());
            }
            Type.Close();
        }

        public string Insert_Messages(string ID_Sender, string ID_Recipient, string Mssage, List<string> List)
        {
            string Return_String = "";
            try
            {
                string x = DateTime.Now.ToString("H: mm dd.MM.yy");
                SqlCommand sqlCommand_Cl = new SqlCommand("Insert into Hstory values('" + ID_Sender + "','" + ID_Recipient + "','" + x + "','" + Mssage + "')", sqlConnection);
                object Itog = sqlCommand_Cl.ExecuteNonQuery();
                Return_String = "SM/S".ToString();
                List.Add("F");
                List.Add(x);
                List.Add(Mssage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Return_String = "SM/F".ToString();
            }
            return Return_String;
        }
    }
}