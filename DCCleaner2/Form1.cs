using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinHttp;
using System.Text.RegularExpressions;

namespace DCCleaner2
{
    public partial class Form1 : Form
    {
        public WinHttpRequest Winhttp;
        List<string> pageno = new List<string>();
        List<string> logno = new List<string>();
        List<string> board_id = new List<string>();
        List<string> cid = new List<string>();
        int time;
        public static string cookie;
        public static string user_no; // 넘버
        public static string ci_t;
        public static string user_id; // 유저id
        int page; // 파싱할 페이지번호
        int board_count = 1;
        int onoff = 0;
        public Form1()
        {
            Winhttp = new WinHttpRequest();
            InitializeComponent();
        }

        public bool login()
        {
            Winhttp.Open("POST", "https://dcid.dcinside.com/join/mobile_login_ok.php");
            Winhttp.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            Winhttp.SetRequestHeader("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 6_0 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10A5376e Safari/8536.25");
            Winhttp.SetRequestHeader("Referer", "http://m.dcinside.com/login.php?r_url=%2Flist.php%3Fid%3Dhit&rucode=1");
            Winhttp.Send("user_id="+textBox1.Text+"&user_pw="+textBox2.Text+"&id_chk=on&mode=&id=&r_url=%252Flist.php%253Fid%253Dhit");
            Winhttp.WaitForResponse();

            Winhttp.Open("GET", "http://m.dcinside.com/list.php?id=hit");
            Winhttp.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            Winhttp.SetRequestHeader("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 6_0 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10A5376e Safari/8536.25");
            Winhttp.SetRequestHeader("Referer", "http://m.dcinside.com/login.php?r_url=%2Flist.php%3Fid%3Dhit&rucode=1");
            Winhttp.Send();
            Winhttp.WaitForResponse();
            string data = Winhttp.ResponseText;
            if (data.IndexOf("logout") != -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private static DateTime Delay(int MS)
        {
            DateTime ThisMoment = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, MS);
            DateTime AfterWards = ThisMoment.Add(duration);
            while (AfterWards >= ThisMoment)
            {
                System.Windows.Forms.Application.DoEvents();
                ThisMoment = DateTime.Now;
            }
            return DateTime.Now;
        }
        private void label4_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            login();
            if (login())
            {
                button2.Enabled = true;
                toolStripStatusLabel1.Text = "로그인 성공";
            }
            else
            {
                toolStripStatusLabel1.Text = "로그인 실패";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int count = Convert.ToInt32(textBox3.Text);
            onoff = 1;
            if (onoff == 1)
            {
                for (int i = 0; i < count; i++)
                {
                    /* board_id 파싱 */
                    Winhttp.Open("GET", "http://m.dcinside.com/gallog/list.php?g_id=" + textBox1.Text + "&g_type=G&page=1");
                    Winhttp.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                    Winhttp.SetRequestHeader("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 6_0 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10A5376e Safari/8536.25");
                    Winhttp.SetRequestHeader("Referer", "http://m.dcinside.com/gallog/home.php?g_id=" + textBox1.Text);
                    Winhttp.SetRequestHeader("X-Requested-With", "XMLHttpRequest");
                    Winhttp.Send();
                    Winhttp.WaitForResponse();
                    string datas = Winhttp.ResponseText;
                    Regex regexs = new Regex("&id=(?<board_id>\\w+)&no=");
                    MatchCollection mcs = regexs.Matches(datas);
                    foreach (Match ms in mcs)
                    {
                        board_id.Add(ms.Groups["board_id"].Value);
                        board_count++;
                        if (board_count > 10)
                            break;
                    }
                    board_count = 1;
                    /* ci_t 파싱부분 */
                    Winhttp.Open("GET", "http://gall.dcinside.com/board/view/?id=hit&no=1");
                    Winhttp.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                    Winhttp.SetRequestHeader("Referer", "http://gall.dcinside.com/board/view/?id=hit&no=1");
                    Winhttp.Send();
                    Winhttp.WaitForResponse();
                    string data = Winhttp.ResponseText;
                    string regex = "ci_t\" value=\"(.*?)\" />";
                    Match match = Regex.Match(data, regex);
                    if (match.Success)
                    {
                        ci_t = match.Groups[1].Value;
                    }
                    /* user_id 와 cid 파싱부분 */
                    Winhttp.Open("GET", "http://gallog.dcinside.com/");
                    Winhttp.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                    Winhttp.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36");
                    Winhttp.SetRequestHeader("Referer", "http://gallog.dcinside.com/");
                    Winhttp.SetRequestHeader("X-Requested-With", "XMLHttpRequest");
                    Winhttp.Send();
                    Winhttp.WaitForResponse();
                    string data2 = Winhttp.ResponseText;
                    Regex regex2 = new Regex("http://gallog.dcinside.com/(?<userid>\\w+)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    MatchCollection mc = regex2.Matches(data2);
                    foreach (Match m in mc)
                    {
                        user_id = m.Groups["userid"].Value;
                    }

                    //WinHttpRequest Winhttp = new WinHttpRequest();
                    Winhttp.Open("GET", "http://gallog.dcinside.com/inc/_mainGallog.php?gid=" + user_id);
                    Winhttp.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                    Winhttp.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.86 Safari/537.36");
                    Winhttp.SetRequestHeader("Referer", "http://gallog.dcinside.com/inc/_mainGallog.php?gid=" + user_id);
                    Winhttp.SetRequestHeader("X-Requested-With", "XMLHttpRequest");
                    Winhttp.Send();
                    Winhttp.WaitForResponse();
                    string data3 = Winhttp.ResponseText;
                    Regex regex3 = new Regex("&cid=(?<cid>\\w+)&", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    MatchCollection mc2 = regex3.Matches(data3);
                    foreach (Match m in mc2)
                    {
                        cid.Add(m.Groups["cid"].Value);
                    }
                    /* pageno 와 logno 파싱 */
                    Winhttp.Open("GET", "http://gallog.dcinside.com/inc/_mainGallog.php?page=1&gid=" + user_id);
                    Winhttp.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                    Winhttp.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
                    Winhttp.SetRequestHeader("Referer", "http://gallog.dcinside.com/inc/_mainGallog.php?page=1&gid=" + user_id);
                    Winhttp.SetRequestHeader("X-Requested-With", "XMLHttpRequest");
                    Winhttp.Send();
                    Winhttp.WaitForResponse();
                    string data4 = Winhttp.ResponseText;
                    Regex regex4 = new Regex("&pno=(?<pageno>\\w+)&logNo=(?<logno>\\w+)&", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                    MatchCollection mc3 = regex4.Matches(data4);
                    foreach (Match m in mc3)
                    {
                        pageno.Add(m.Groups["pageno"].Value);
                        logno.Add(m.Groups["logno"].Value);
                    }
                    if (pageno.Count == 0)
                    {
                        MessageBox.Show("글이 없습니다.");
                        onoff = 0;
                    }
                    else
                    {
                        Winhttp.Open("GET", "http://m.dcinside.com/gallog/list.php?g_id=" + textBox1.Text + "&g_type=G&page=1");
                        Winhttp.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                        Winhttp.SetRequestHeader("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 6_0 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10A5376e Safari/8536.25");
                        Winhttp.SetRequestHeader("Referer", "http://m.dcinside.com/gallog/home.php?g_id=" + textBox1.Text);
                        Winhttp.SetRequestHeader("X-Requested-With", "XMLHttpRequest");
                        Winhttp.Send();
                        Winhttp.WaitForResponse();

                        for (int j = 0; j < pageno.Count; j++)
                        {
                            /* 갤러리삭제 */
                            Winhttp.Open("POST", "http://gall.dcinside.com/forms/delete_submit");
                            Winhttp.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                            Winhttp.SetRequestHeader("Referer", "http://gall.dcinside.com/");
                            Winhttp.SetRequestHeader("X-Requested-With", "XMLHttpRequest");
                            Winhttp.Send("ci_t=" + ci_t + "&id="+board_id[j]+"&no=" + pageno[j] + "&key=");
                            Delay(200);
                            /* 갤로그삭제 */
                            Winhttp.Open("POST", "http://gallog.dcinside.com/inc/_deleteArticle.php");
                            Winhttp.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                            Winhttp.SetRequestHeader("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 6_0 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10A5376e Safari/8536.25");
                            Winhttp.SetRequestHeader("Referer", "http://gallog.dcinside.com");
                            Winhttp.SetRequestHeader("X-Requested-With", "XMLHttpRequest");
                            Winhttp.Send("rb=&dTp=1&gid=" + user_id + "&cid=" + cid[j] + "&page=&pno=" + pageno[j] + "&no=" + pageno[j] + "&logNo=" + logno[j] + "&id="+board_id[j]+"&nate=&con_key=aaaaaaaaaaaaaaaaaaaa");
                            Winhttp.WaitForResponse();
                            richTextBox1.AppendText(pageno[j] + " -> Del\n");
                            Delay(200);
                        }
                        richTextBox1.AppendText("--Page[" + (page + 1) + "]--\n");
                    }
                    pageno.Clear();
                    logno.Clear();
                    board_id.Clear();
                    cid.Clear();
                }
            }
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(null, null);
            }
            if (e.KeyCode == Keys.Tab)
            {
                this.Focus();
                textBox2.SelectAll();
            }
        }
    }
}
