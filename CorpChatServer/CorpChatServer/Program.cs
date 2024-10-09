using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Data.SqlClient;
using System.Text.Json;
using System.Windows.Forms;

namespace CorpChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new GrafForm());
            }
            catch(Exception e)
            {
                string x = e.Message.ToString();
            }
        }
    }
}
