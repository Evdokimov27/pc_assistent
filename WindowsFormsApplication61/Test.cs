using HtmlAgilityPack;
using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace WindowsFormsApplication61
{
   
    public partial class Test : Form
    {
        private const int APPCOMMAND_BROWSER_BACKWARD = 1;
        private const int APPCOMMAND_BROWSER_FORWARD = 2;
        private const int APPCOMMAND_BROWSER_REFRESH = 3;
        private const int APPCOMMAND_BROWSER_STOP = 4;
        private const int APPCOMMAND_BROWSER_SEARCH = 5;
        private const int APPCOMMAND_BROWSER_FAVORITES = 6;
        private const int APPCOMMAND_BROWSER_HOME = 7;
        private const int APPCOMMAND_VOLUME_MUTE = 8;
        private const int APPCOMMAND_VOLUME_DOWN = 9;
        private const int APPCOMMAND_VOLUME_UP = 10;
        private const int APPCOMMAND_MEDIA_NEXTTRACK = 11;
        private const int APPCOMMAND_MEDIA_PREVIOUSTRACK = 12;
        private const int APPCOMMAND_MEDIA_STOP = 13;
        private const int APPCOMMAND_MEDIA_PLAY_PAUSE = 14;
        private const int APPCOMMAND_LAUNCH_MAIL = 15;
        private const int APPCOMMAND_LAUNCH_MEDIA_SELECT = 16;
        private const int APPCOMMAND_LAUNCH_APP1 = 17;
        private const int APPCOMMAND_LAUNCH_APP2 = 18;
        private const int APPCOMMAND_BASS_DOWN = 19;
        private const int APPCOMMAND_BASS_BOOST = 20;
        private const int APPCOMMAND_BASS_UP = 21;
        private const int APPCOMMAND_TREBLE_DOWN = 22;
        private const int APPCOMMAND_TREBLE_UP = 23;
        private const int APPCOMMAND_MICROPHONE_VOLUME_MUTE = 24;
        private const int APPCOMMAND_MICROPHONE_VOLUME_DOWN = 25;
        private const int APPCOMMAND_MICROPHONE_VOLUME_UP = 26;
        private const int APPCOMMAND_HELP = 27;
        private const int APPCOMMAND_FIND = 28;
        private const int APPCOMMAND_NEW = 29;
        private const int APPCOMMAND_OPEN = 30;
        private const int APPCOMMAND_CLOSE = 31;
        private const int APPCOMMAND_SAVE = 32;
        private const int APPCOMMAND_PRINT = 33;
        private const int APPCOMMAND_UNDO = 34;
        private const int APPCOMMAND_REDO = 35;
        private const int APPCOMMAND_COPY = 36;
        private const int APPCOMMAND_CUT = 37;
        private const int APPCOMMAND_PASTE = 38;
        private const int APPCOMMAND_REPLY_TO_MAIL = 39;
        private const int APPCOMMAND_FORWARD_MAIL = 40;
        private const int APPCOMMAND_SEND_MAIL = 41;
        private const int APPCOMMAND_SPELL_CHECK = 42;
        private const int APPCOMMAND_DICTATE_OR_COMMAND_CONTROL_TOGGLE = 43;
        private const int APPCOMMAND_MIC_ON_OFF_TOGGLE = 44;
        private const int APPCOMMAND_CORRECTION_LIST = 45;
        private const int APPCOMMAND_MEDIA_PLAY = 46;
        private const int APPCOMMAND_MEDIA_PAUSE = 47;
        private const int APPCOMMAND_MEDIA_RECORD = 48;
        private const int APPCOMMAND_MEDIA_FAST_FORWARD = 49;
        private const int APPCOMMAND_MEDIA_REWIND = 50;
        private const int APPCOMMAND_MEDIA_CHANNEL_UP = 51;
        private const int APPCOMMAND_MEDIA_CHANNEL_DOWN = 52;
        private const int WM_APPCOMMAND = 0x319;



        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, int lParam);
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        Tag tag = new Tag();

        int max = 0;
        int interval = 0;
        Font f = new Font("Arial", 10);
        SolidBrush b = new SolidBrush(Color.Black);
        SolidBrush w = new SolidBrush(Color.White);
        byte[] datab;
        string speach;
        int start = 0;
        int end = 0;
        int len_sig = 0;
        int len_sil = 0;
        static String ycText = "";
        public WaveInEventArgs arg;
        int pos = 0;
        int[] data;
        public string app_path;
        string json;
        App app = new App();
        Buttons buttons= new Buttons();
        ToolTip t = new ToolTip();


        public Test()
        {
            datab = new byte[48000 * 16 * 2];
            data = new int[48000 * 16];
            InitializeComponent();
            notifyIcon1.Visible = true;
            // добавляем Эвент или событие по 2му клику мышки, 
            //вызывая функцию  notifyIcon1_MouseDoubleClick
            this.notifyIcon1.MouseDoubleClick += new MouseEventHandler(notifyIcon1_MouseDoubleClick);

            // добавляем событие на изменение окна
            this.Resize += new System.EventHandler(this.Form1_Resize);
        }
        AudioRecorder r;
        private void Form1_Resize(object sender, EventArgs e)
        {
            // проверяем наше окно, и если оно было свернуто, делаем событие        
            if (WindowState == FormWindowState.Minimized)
            {
                // прячем наше окно из панели
                this.ShowInTaskbar = false;
                // делаем нашу иконку в трее активной
                notifyIcon1.Visible = true;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            t.SetToolTip(textBox2, "Чтобы указать, что любое сочетание клавиш SHIFT, CTRL и ALT должно быть удерживано, а несколько других клавиш нажимаются, заключите код этих ключей в круглые скобки.");

            r = new AudioRecorder();
            r.f = this;
            r.Start();
            if (!File.Exists("app.json"))
            {
                json = JsonConvert.SerializeObject(app.dict, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText("app.json", json);
            }
            if (!File.Exists("buttons.json"))
            {
                json = JsonConvert.SerializeObject(app.dict, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText("buttons.json", json);
            }

                json = File.ReadAllText("app.json");
             app.dict = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(json);
             
             
             json = File.ReadAllText("buttons.json");
             buttons.dict = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(json);

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            r.Stop();
        }
        public byte[] int16ToBA(int[] arri, byte[] arrb)
        {
            int len = 0;
            for (int i = 0; i < len_sig + 24000; i++) {
                int cp = i + start;
                if (cp >= 48000 * 16) cp -= 48000 * 16;
                byte[] bytes = BitConverter.GetBytes(arri[cp]);
                arrb[len++] = bytes[0];
                arrb[len++] = bytes[1];
            }
            return arrb;
        }
        public short BAToInt16(byte[] bytes, int index)
        {
            short value = BitConverter.ToInt16(bytes, index);

            return value;
        }
        public async Task<string> speech_recognition(String filename)
        {
            String iamToken = "t1.9euelZqUiszNzJyVnMyai4zLksfNyO3rnpWayZeOx8jOlp7KkZ3Mj8-di4vl8_dCTFBi-e8MdQ8o_d3z9wJ7TWL57wx1Dyj9zef1656VmpuYjcbHyM6KmcfMlsbLiouN7_0.6v9JSkOpeOFC8B7RBjrlsJJ7r_hCCPvVg89JfEKDF3Z_PFDH3fSp6FiJoJdPY0k18eQsiWBS7P5Mw175-TgvDQ"; // IAM-токен
            String folderId = "b1gn1nfvk2nna6m0ms56";// # Идентификатор каталога
            String audioFileName = filename;

            string url = "https://stt.api.cloud.yandex.net/speech/v1/stt:recognize?lang=ru-RU&folderId=" + folderId + "&format=lpcm&sampleRateHertz=48000";

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + iamToken);



            var content = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(System.IO.File.ReadAllBytes(filename));
            content.Headers.TryAddWithoutValidation("Authorization", "Bearer " + iamToken);

            fileContent.Headers.ContentDisposition =
           new ContentDispositionHeaderValue("form-data") //<- 'form-data' instead of 'attachment'
           {
               Name = "attachment", // <- included line...
               FileName = filename,
           };


            content.Add(fileContent);

            var response = await client.PostAsync(url, content);
            var contents = await response.Content.ReadAsStringAsync();
            ycText = await Task.FromResult(contents.ToString());



            return ycText;



        }
        int cnt = 0;
        private void Form1_Paint(object sender, PaintEventArgs e)
        {

            if (arg == null) return;

            for (int i = 10; i < arg.BytesRecorded / 2; i++)
            {
                cnt++;
                int y = BAToInt16(arg.Buffer, i * 2); ;
                int y1 = BAToInt16(arg.Buffer, i * 2 - 20); ;
                if (max < Math.Abs(y)) max = Math.Abs(y);
                if (cnt == 48000)
                {
                    if (interval == 0) start = pos;
                    else
                    if (max > 300)//тут нужно будет сделать больше.
                    {
                        len_sig += 48000;
                    }
                    else {//тишина


                        if (len_sig >= 48000)
                        {
                            datab = int16ToBA(data, datab);
                            WaveFormat waveformat = new WaveFormat(48000, 16, 1);

                            var reader = new RawSourceWaveStream(datab, 0, len_sig * 2, waveformat);

                            using (var convertedStream = WaveFormatConversionStream.CreatePcmStream(reader))
                            {
                                WaveFileWriter.CreateWaveFile("text.wav", convertedStream);
                                speech_recognition("text.wav");

                            }
                            len_sig = 0;

                        }
                        len_sig = 0;
                        start = pos;

                    }
                    max = 0;
                    cnt = 0;
                    interval++;
                }
                try
                {




                    data[pos] = y;
                    pos++;
                    if (pos >= 48000 * 16) pos = 0;

                }
                catch (Exception e1) { }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (ycText.Length < 2) return;
            ycText = ycText.Replace('{', ' ');
            ycText = ycText.Replace('}', ' ');
            ycText = ycText.Replace('"', ' ');
            label1.Text = ycText.Split(':')[1].Trim();
            if (speach != ycText.Split(':')[1].Trim())
            {
                speach = ycText.Split(':')[1].Trim();
                for (int i = 0; i < app.dict.Count; i++)
                {
                    for (int j = 1; j < app.dict[i.ToString()].Length; j++)
                    {
                        if (speach == app.dict[i.ToString()][j])
                        {
                            var prs = new ProcessStartInfo(app.dict[i.ToString()][0]);
                            prs.Arguments = app.dict[i.ToString()][1].ToString();
                            Process.Start(prs);
                        }

                    }
                }

                for (int i = 0; i < buttons.dict.Count; i++)
                {
                    for (int j = 1; j < buttons.dict[i.ToString()].Length; j++)
                    {
                        if (speach == buttons.dict[i.ToString()][j])
                        {
                            SendKeys.Send(buttons.dict[i.ToString()][0]);
                        }
                    }

                }
                
            }
            
        }



        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                int Start, End;
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }

            return "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process txt = new System.Diagnostics.Process();
            txt.StartInfo.FileName = "notepad.exe";
            txt.StartInfo.Arguments = "buttons.json";
            txt.Start();
            System.Diagnostics.Process txt2 = new System.Diagnostics.Process();
            txt2.StartInfo.FileName = "notepad.exe";
            txt2.StartInfo.Arguments = "app.json";
            txt2.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {


                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();
                    }
                }
                tag.music_app = filePath;
            }
        }
        HtmlAgilityPack.HtmlDocument htmlSnippet = new HtmlAgilityPack.HtmlDocument();




        private void button3_Click_1(object sender, EventArgs e)
        {  
            List<string> mass = new List<string>();
                mass.Add(app_path);
                mass.Add(textBox1.Text);
                for (int i = 0; i < listBox1.Items.Count; i++)
                {
                    mass.Add(listBox1.Items[i].ToString());
                }
                app.dict.Add(app.dict.Count.ToString(), mass.ToArray());
                
                for (int i = 0; i < listBox1.Items.Count; i++)
                {

                }

            string json = JsonConvert.SerializeObject(app.dict, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText("app.json", json);


        }

        private void button5_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {


                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();
                    }
                }
                app_path = filePath;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            listBox1.Items.Add(textBox4.Text);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            List<string> mass = new List<string>();
            mass.Add(textBox2.Text);
            for (int i = 0; i < listBox2.Items.Count; i++)
            {
                mass.Add(listBox2.Items[i].ToString());
            }
            MessageBox.Show(mass[1]);
            buttons.dict.Add(buttons.dict.Count.ToString(), mass.ToArray());

            string json = JsonConvert.SerializeObject(buttons.dict, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText("buttons.json", json);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            listBox2.Items.Add(textBox3.Text);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            json = JsonConvert.SerializeObject(app.dict, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText("app.json", json);

            json = JsonConvert.SerializeObject(app.dict, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText("buttons.json", json);
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            notifyIcon1.Visible = false;
            // возвращаем отображение окна в панели
            this.ShowInTaskbar = true;
            //разворачиваем окно
            WindowState = FormWindowState.Normal;
        }
    }

    internal class AudioRecorder
{
    public WaveIn waveSource = null;
    public Test f;
    public AudioRecorder()
    {
            f = null;
    }

    public void Start()
    {
        waveSource = new WaveIn();
        waveSource.WaveFormat = new WaveFormat(48000,16, 1);
        waveSource.DeviceNumber = 0;
            waveSource.BufferMilliseconds = 100;
       waveSource.DataAvailable += new EventHandler<WaveInEventArgs>(waveSource_DataAvailable);
        waveSource.RecordingStopped += new EventHandler<StoppedEventArgs>(waveSource_RecordingStopped);

        
        waveSource.StartRecording();
    }

    public void Stop()
    {
        waveSource.StopRecording();
    }

    private void waveSource_DataAvailable(object sender, WaveInEventArgs e)
    {

            f.arg = e;
            f.Invalidate();
    }

    private void waveSource_RecordingStopped(object sender, StoppedEventArgs e)
    {
        if (waveSource != null)
        {
            waveSource.Dispose();
            waveSource = null;
        }

       

    }
}
}
