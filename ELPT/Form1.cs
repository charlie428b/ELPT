using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
        private int _left = 1;//0=有道 1=纯文本
        private int _right = 1;//0=Dictionary.com 1=必应 2=Lexipedia

        private WebClient wc = new WebClient();
        //声明所需正则表达式
        private Regex regText = new Regex(@"(?<=\{""tr"":\[\{""l"":\{""i"":\["").+?(?=""\]\}\}\]\})");//来自有道的解释
        private Regex regPronounce = new Regex(@"(?<=\[\{\""L\"":\""US\"",\""V\"":\"").+?(?=\""\},)");//来自必应的音标
        private Regex regWordF = new Regex(@"(?<=\{\""wf\"":\{\""name\"":\"")(.+?)\"",\""value\"":\""(.+?)(?=\""\}\})");//来自有道的词形变化

        /// <summary>
        /// 点击查询按钮或按回车时执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                case 1://纯文本
                    labelWord.Text = ComboBox1.Items[0].ToString();
                    richTextBox1.ResetText();
                    try
                    {
                        //从必应查询美式读音
                        byte[] resultByteP = wc.DownloadData("http://dict.bing.com.cn/api/http/v2/4154AA7A1FC54ad7A84A0236AA4DCAF1/en-us/zh-cn/lexicon/?q=" + ComboBox1.Items[0] + "&format=application/json&theme={Win10}+3251FBE529D34206822990E48226D8BE");
                        string resultP = Encoding.UTF8.GetString(resultByteP);
                        if (regPronounce.IsMatch(resultP))
                        {
                            MatchCollection matches = regPronounce.Matches(resultP);
                            for (int i = 0; i < matches.Count; i++)
                            {
                                richTextBox1.Text += "[" + matches[i].Value + "]";
                            }
                        }
                        //加载并播放来自必应的发音
                        axWindowsMediaPlayer1.URL = "http://media.engkoo.com:8129/en-US/" + ComboBox1.Items[0] + ".mp3";
                        //从有道查询解释
                        byte[] resultByte = wc.DownloadData("http://dict.youdao.com/jsonapi?q=" + ComboBox1.Items[0] + "&keyfrom=deskdict.main&dogVersion=1.0&dogui=json&client=deskdict&id=075aef8658e2c89b0&vendor=unknown&in=YoudaoDictFull&appVer=6.3.67.7016&appZengqiang=1&abTest=2&le=eng&dicts=%7B%22count%22%3A11%2C%22dicts%22%3A%5B%5B%22ec%22%2C%22ce%22%2C%22cj%22%2C%22jc%22%2C%22ck%22%2C%22kc%22%2C%22cf%22%2C%22fc%22%5D%2C%5B%22pic_dict%22%5D%2C%5B%22web_trans%22%2C%22special%22%2C%22ee%22%2C%22hh%22%5D%2C%5B%22collins%22%2C%22ec21%22%2C%22ce_new%22%5D%2C");
                        string result = Encoding.UTF8.GetString(resultByte);
                        if (regText.IsMatch(result))
                        {
                            MatchCollection matches = regText.Matches(result);
                            for (int i = 0; i < matches.Count; i++)
                            {
                                richTextBox1.Text += "\n" + matches[i].Value.Replace("\\u2026", "...");
                            }
                        }
                        //从有道查询词形变化
                        if (regWordF.IsMatch(result))
                        {
                            MatchCollection matches = regWordF.Matches(result);
                            for (int i = 0; i < matches.Count; i++)
                            {
                                richTextBox1.Text += "\n" + matches[i].Groups[1].Value + "：" + matches[i].Groups[2].Value;
                            }
                        }
                    }
                    catch
                    {
                        labelWord.Text = "";
                        richTextBox1.Text = "加载失败";
                    }
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
                case 2://Lexipedia
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
            MessageBox.Show(string.Format("版本{0}\n刘持冰制作", Application.ProductVersion));
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

        /// <summary>
        /// 将右侧窗格的词典切换至综合查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonText_Click(object sender, EventArgs e)
        {
            webBrowser1.Hide();//隐藏webBrowser，显示richTextBox
            panelTextBox.Show();
            int temp = _right;//使右边不必更新
            _right = -1;
            _left = 1;//更改flags
            Search();
            _right = temp;//恢复右边的flags
        }

        /// <summary>
        /// 将右侧窗格的词典切换至有道
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonYoudao_Click(object sender, EventArgs e)
        {
            webBrowser1.Show();//隐藏webBrowser，显示richTextBox
            panelTextBox.Hide();
            int temp = _right;//使右边不必更新
            _right = -1;
            _left = 0;//更改flags
            Search();
            _right = temp;//恢复右边的flags
        }

        /// <summary>
        /// 当调整窗口大小时将splitContainer的拆分器位置设为50%处
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Resize(object sender, EventArgs e)
        {
            try
            {
                splitContainer2.SplitterDistance = Size.Width / 2;
            }
            catch { }
        }

        /// <summary>
        /// 当窗口加载时将splitContainer的拆分器位置设为50%处
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            splitContainer2.SplitterDistance = Size.Width / 2;
        }

        /// <summary>
        /// 点击菜单“隐藏右侧窗格”时隐藏或显示右侧窗格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 仅综合查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.splitContainerPanel2Collapsed)
            {
                Properties.Settings.Default.splitContainerPanel2Collapsed = false;
                splitContainer2.SplitterDistance = Size.Width / 2;
            }
            else
            {
                Properties.Settings.Default.splitContainerPanel2Collapsed = true;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }
    }
}
