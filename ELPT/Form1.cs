using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
            ////在初始化时判断是否为初次运行，若是，导入注册表
            //if (!(File.Exists(System.Environment.GetEnvironmentVariable("appdata")+"\\ELPT\\NotFirstRun.elptsetting")))
            //{
            //    System.Diagnostics.Process.Start(Application.StartupPath + "\\reg32.reg");
            //    if (Environment.Is64BitOperatingSystem)
            //    {
            //        System.Diagnostics.Process.Start(Application.StartupPath + "\\reg64.reg");
            //    }
            //    File.Create(Application.StartupPath + "\\NotFirstRun.elptsetting");
            //}
            InitializeComponent();
        }

        //flags
        private int _left = 0;//0=有道 1=必应
        private int _right = 1;//0=Dictionary.com 1=必应 2=Lexipedia

        private void Button1_Click(object sender, EventArgs e)
        {
            Search();
        }

        /// <summary>
        /// 在两个窗格中查询用户最后一次输入的词
        /// </summary>
        private void Search()
        {
            if (ComboBox1.Text != "")
            {
                //存入历史纪录并清空输入框
                ComboBox1.Items.Insert(0, ComboBox1.Text);
                ComboBox1.Text = "";
            }//若输入框内容为空则查询上次输入的内容

            if (ComboBox1.Items.Count == 0)//防止程序出错
            {
                return;
            }

            switch (_left)//在左侧窗格中查询
            {
                case -1: break;//在切换词典时执行，节省资源
                case 0://有道
                    webBrowser1.Navigate("http://dict.youdao.com/search?q=" + ComboBox1.Items[0]);
                    break;
            }

            switch (_right)//在右侧窗格中查询
            {
                case -1: break;//在切换词典时执行，节省资源
                case 0://Dictionary.com
                    webBrowser2.Navigate("http://dictionary.reference.com/browse/" + ComboBox1.Items[0]);
                    break;
                case 1://必应
                    webBrowser2.Navigate("http://cn.bing.com/dict/" + ComboBox1.Items[0]);
                    //webBrowser2.Navigate("javascript:({document.getElementById(\"target\").click();})()");
                    break;
                case 3://Lexipedia
                    webBrowser2.Navigate("http://www.lexipedia.com/english/" + ComboBox1.Items[0]);
                    break;
            }
        }

        /// <summary>
        /// 当按下Alt+C时将焦点转移至输入框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            ComboBox1.Focus();
        }

        /// <summary>
        /// 将右侧窗格的词典切换至Dictionary.com
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDcom_Click(object sender, EventArgs e)
        {
            int temp = _left;
            _left = -1;//使左侧窗格不必更新
            _right = 0;
            Search();
            _left = temp;//恢复左侧窗格的flag
        }

        /// <summary>
        /// 将右侧窗格的词典切换至必应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonBing_Click(object sender, EventArgs e)
        {
            int temp = _left;//注释见buttonDcom_Click的注释
            _left = -1;
            _right = 1;
            Search();
            _left = temp;
        }

        /// <summary>
        /// 当用户按下alt时显示工具栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuStrip1_MenuActivate(object sender, EventArgs e)
        {
            menuStrip1.Show();
        }

        /// <summary>
        /// 当工具栏失去焦点时隐藏工具栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuStrip1_MenuDeactivate(object sender, EventArgs e)
        {
            menuStrip1.Hide();
        }

        /// <summary>
        /// 显示关于信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(string.Format("版本{0}\n刘持冰制作",Application.ProductVersion));
        }

        /// <summary>
        /// 将右侧窗格的词典切换至Lexipedia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLexi_Click(object sender, EventArgs e)
        {
            int temp = _left;//注释见buttonDcom_Click的注释
            _left = -1;
            _right = 2;
            Search();
            _left = temp;
        }
    }
}
