using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CorpChatServer
{
    class JsonClassToServer
    {
        public string Instraction { set; get; }
        public string Message { set; get; }
        public string Login { set; get; }
        public string Password { set; get; }
        public string Sender_ID { set; get; }
        public string ID_Contact { set; get; }
        public string FIO_Find { set; get; }
        public string Count_of_Massege_for_load { set; get; }
        public string Param_Find { set; get; }
        public JsonClassToServer()
        {

        }
        public JsonClassToServer(string Instraction, string First_string, string Second_String) //GI, FC, GH
        {
            this.Instraction = Instraction;
            if (Instraction == "GI")
            {
                this.Login = First_string;
                this.Password = Second_String;
            }
            else if(Instraction == "FC")
            {
                this.Sender_ID = First_string;
                this.Param_Find = Second_String;
            }
        }
        public JsonClassToServer(string Instraction, string ID) // SI, GD
        {
            this.Instraction = Instraction;
            this.Sender_ID = ID;
        }
        public JsonClassToServer(string Instraction, string Sender_ID, string ID_Contact, string String)//GH, SM
        {
            this.Instraction = Instraction;
            this.Sender_ID = Sender_ID;
            this.ID_Contact = ID_Contact;
            if (Instraction == "GH")
            {
                this.Count_of_Massege_for_load = String;
            }
            else if (Instraction == "SM")
            {
                this.Message = String;
            }
        }
    }

}