using HtmlAgilityPack;
using NAudio.Wave;
using System;
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
        static  String ycText="";
        public WaveInEventArgs arg;
        int pos = 0;
        int[] data;

        
        public Test()
        {
            datab = new byte[48000 * 16 * 2];
            data = new int[48000 * 16];
            InitializeComponent();
        }
        AudioRecorder r;
        private void Form1_Load(object sender, EventArgs e)
        {
            r = new AudioRecorder();
            r.f = this;
            r.Start();

            comboBox1.Items.Add("123"); 
            
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
            String iamToken = "t1.9euelZqYz5qeiozOl8eUjZbIkM_Jze3rnpWayJmTmc6UyZqRloqam4-emMrl8_cCcFZi-e9BIWM3_N3z90IeVGL570EhYzf8.L_DaMI9pl-HLlaPKjAfRUPcxwO-7OarNR5O05XkLRg_qSJKoMp5vfu366jh9UIfezHv_nFoTyqkVKuvVoi4zAQ"; // IAM-токен
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
            ycText= await Task.FromResult(contents.ToString());

                   

            return ycText;



        }
        int cnt = 0;
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            
            if (arg == null) return;
            
            for (int i = 10; i < arg.BytesRecorded/2; i++)
            {
                cnt++;
                int y = BAToInt16(arg.Buffer, i*2); ;
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


                        if (len_sig >=48000)
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
            ycText=ycText.Replace('{', ' ');
            ycText = ycText.Replace('}', ' ');
            ycText = ycText.Replace('"', ' ');
            label1.Text = ycText.Split(':')[1].Trim();
            if (speach != ycText.Split(':')[1].Trim())
            {
                speach = ycText.Split(':')[1].Trim();
                for (int i = 0; i < tag.time.Length; i++)
                {
                    if (speach ==  "Бублик " + tag.time[i])
                    {
                        SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle, APPCOMMAND_MEDIA_PLAY << 16);
                    }
                }
                for (int i = 0; i < tag.search.Length; i++)
                {
                    if ( i < 1 && speach.Split(' ')[0] + " " + speach.Split(' ')[1] ==  "Бублик " + tag.search[i])
                    {
                          var prs = new ProcessStartInfo("opera.exe");
                          prs.Arguments = "http://google.com/search?q=" + speach.Remove(0, speach.IndexOf(' ') + speach.Split(' ')[1].Length+2).Replace(" ", "+");
                          Process.Start(prs);
                    }
                }
                for (int i = 0; i < tag.search.Length; i++)
                {
                    if ( i > 0 && speach ==  "Бублик " + tag.search[i])
                    {
                        var prs = new ProcessStartInfo("opera.exe");
                        Process.Start(prs);
                    }
                }
                for (int i = 0; i < tag.youtube.Length; i++)
                {
                    if (speach ==  "Бублик " + tag.youtube[i])
                    {
                        var prs = new ProcessStartInfo("opera.exe");
                        prs.Arguments = "https://youtube.com/";
                        Process.Start(prs);
                    }
                }
                for (int i = 0; i < tag.vk.Length; i++)
                {
                    if (speach ==  "Бублик " + tag.vk[i])
                    {
                        var prs = new ProcessStartInfo("opera.exe");
                        prs.Arguments = "https://vk.com/im?sel=275546938";
                        Process.Start(prs);
                        
                    }
                }
                for (int i = 0; i < tag.music.Length; i++)
                {
                    if (speach ==  "Бублик " + tag.music[i])
                    {
                        Process.Start(tag.music_app);
                    }
                }
                for (int i = 0; i < tag.downsound.Length; i++)
                {
                    if (speach ==  "Бублик " + tag.downsound[i])
                    {
                        for(int j=0; j < 50; j++)
                        {
                            SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle, APPCOMMAND_VOLUME_DOWN << 16);
                        }
                        for (int j = 0; j < 5; j++)
                        {
                            SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle, APPCOMMAND_VOLUME_UP << 16);
                        } 
                    }
                }
                for (int i = 0; i < tag.fiftysound.Length; i++)
                {
                    if (speach ==  "Бублик " + tag.fiftysound[i])
                    {
                        for (int j = 0; j < 50; j++)
                        {
                            SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle, APPCOMMAND_VOLUME_DOWN << 16);
                        }
                        for (int j = 0; j < 25; j++)
                        {
                            SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle, APPCOMMAND_VOLUME_UP << 16);
                        }
                    }
                }
                for (int i = 0; i < tag.upsound.Length; i++)
                {
                    if (speach ==  "Бублик " + tag.upsound[i])
                    {
                        for (int j = 0; j < 50; j++)
                        {
                            SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle, APPCOMMAND_VOLUME_UP << 16);
                        }
                    }
                }
                for (int i = 0; i < tag.play.Length; i++)
                {
                    if (speach ==  "Бублик " + tag.play[i])
                    {
                        SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle, APPCOMMAND_MEDIA_PLAY << 16);
                    }
                }
                for (int i = 0; i < tag.pause.Length; i++)
                {
                    if (speach ==  "Бублик " + tag.pause[i])
                    {
                        SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle, APPCOMMAND_MEDIA_PAUSE << 16);
                    }
                }
                for (int i = 0; i < tag.next.Length; i++)
                {
                    if (speach ==  "Бублик " + tag.next[i])
                    {
                        SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle, APPCOMMAND_MEDIA_NEXTTRACK << 16);
                    }
                }
                for (int i = 0; i < tag.previsou.Length; i++)
                {
                    if (speach ==  "Бублик " + tag.previsou[i])
                    {
                        SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle, APPCOMMAND_MEDIA_PREVIOUSTRACK << 16);
                    }
                }
                for (int i = 0; i < tag.refresh_site.Length; i++)
                {
                    if (speach ==  "Бублик " + tag.refresh_site[i])
                    {
                        SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle, APPCOMMAND_BROWSER_REFRESH << 16);
                    }
                }
                for (int i = 0; i < tag.back_site.Length; i++)
                {
                    if (speach ==  "Бублик " + tag.back_site[i])
                    {
                        SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle, APPCOMMAND_BROWSER_BACKWARD << 16);
                    }
                }
                for (int i = 0; i < tag.next_site.Length; i++)
                {
                    if (speach ==  "Бублик " + tag.next_site[i])
                    {
                        SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle, APPCOMMAND_BROWSER_FORWARD << 16);
                    }
                }
                for (int i = 0; i < tag.close.Length; i++)
                {
                    if (speach ==  "Бублик " + tag.close[i])
                    {
                        SendKeys.Send("%{F4}");
                    }
                }
                for (int i = 0; i < tag.exit.Length; i++)
                {
                    if (speach ==  "Бублик " + tag.exit[i])
                    {
                        this.Close();
                    }
                }
                for (int i = 0; i < tag.back_site.Length; i++)
                {
                    if (speach ==  "Бублик " + tag.back_site[i])
                    {
                        SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle, APPCOMMAND_BROWSER_BACKWARD << 16);
                    }
                }
                for (int i = 0; i < tag.cmd.Length; i++)
                {
                    if (speach ==  "Бублик " + tag.cmd[i])
                    {
                        Process.Start("Taskmgr.exe");
                    }
                }
                for (int i = 0; i < tag.otkat.Length; i++)
                {
                    if (speach == "Бублик " + tag.otkat[i])
                    {
                        SendKeys.Send("%{F10}");
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
            for (int j = 0; j < 0 + 1; j++)
            {
                SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle, APPCOMMAND_VOLUME_DOWN << 16);
            }
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
