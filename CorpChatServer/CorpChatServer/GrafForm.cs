using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace CorpChatServer
{
    public partial class GrafForm : Form
    {
        Hosting hosting_first;
        public GrafForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            hosting_first = new Hosting(33023);
            hosting_first.Main_Vhod();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ////first.Abort();
            ////hosting_first.Delete();
            //hosting_first.Di
            //hosting_first = new Hosting(33023);
            //hosting_first.Main_Vhod();
            ////first.Start();
            ////first.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //first.Abort();
        }
    }
}
