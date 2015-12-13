using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ELPT
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private int _left = 0;//0=有道 1=必应
        private int _right = 0;//0=Dictionary.com 1=必应

        private void Button1_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void Search()
        {
            if (ComboBox1.Text == "")
            {
                ComboBox1.Text = ComboBox1.Items[0].ToString();
            }

            ComboBox1.Items.Insert(0, ComboBox1.Text);
            ComboBox1.Text = "";

            switch (_left)//在左侧窗格中查询
            {
                case 0:
                    webBrowser1.Navigate("http://dict.youdao.com/search?q=" + ComboBox1.Items[0]);
                    break;
            }

            switch (_right)//在右侧窗格中查询
            {
                case 0:
                    webBrowser2.Navigate("http://dictionary.reference.com/browse/" + ComboBox1.Items[0]);
                    break;
                case 1:
                    webBrowser2.Navigate("http://cn.bing.com/dict/" + ComboBox1.Items[0]);
                    //webBrowser2.Navigate("javascript:({document.getElementById(\"target\").click();})()");
                    break;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ComboBox1.Focus();
        }

        private void buttonDcom_Click(object sender, EventArgs e)
        {
            _left = 0;
            Search();
        }

        private void buttonBing_Click(object sender, EventArgs e)
        {
            _left = 1;
            Search();
        }
    }
}
