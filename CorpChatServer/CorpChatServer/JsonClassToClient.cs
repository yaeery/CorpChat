using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CorpChatServer
{
    class JsonClassToClient
    {
        public string Instraction { set; get; }
        public string ID_Sender { set; get; }
        public string Result_Registration { set; get; }
        public List<string> FIO_ID_for_All_Dialogs { set; get; }
        public List<string> Message_Hystory_List { set; get; }

        public JsonClassToClient() { }
        public JsonClassToClient(string Instraction, string Result_Registration)//GI
        {
            this.Instraction = Instraction;
            if (Instraction == "GI")
            {
                this.Result_Registration = Result_Registration;
            }
        }
        public JsonClassToClient(string Instraction, List<string> List)
        {
            this.Instraction = Instraction;
            if ((Instraction == "GH")&&(Instraction == "SM/S"))
            {
                this.Message_Hystory_List = List;
            }
            else
            {
                this.FIO_ID_for_All_Dialogs = List;
            }
        }
        public JsonClassToClient(string Instraction, string ID, List<string> List)
        {
            this.Instraction = Instraction;
            this.ID_Sender = ID;
            this.Message_Hystory_List = List;
        }

        public void Clear()
        {
            Instraction = "";
            Result_Registration = "";
            FIO_ID_for_All_Dialogs.Clear();
            Message_Hystory_List.Clear();
        }
    }
}