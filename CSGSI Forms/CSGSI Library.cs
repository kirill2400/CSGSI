using CSGSI;
using CSGSI_Forms.Properties;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using static System.Threading.Thread;
using System.Windows.Forms;
using System.Windows.Media;
using System.Net.Sockets;
using System.Text;
using static CSGSI_Library.CSGSI_my;
using System.Net;

namespace CSGSI_Library
{
    internal enum TypeKill { DoubleKill, FirstBlood, HeadShot, Other, Rampage, TripleKill, UltraKill }
    internal enum TypeMusic { FlashBang, Win, Lose, LowHealth, Death, FreezeTime }

    internal class CSGSI_my
    {
        static private Random rnd = new Random();
        static internal System.Windows.Threading.Dispatcher Dispatcher;
        static private string[][] music;
        static private string[][] ultimate;
        static private bool[][] antiRandom;
        static internal GameStateListener gsl;

        internal CSGSI_my(int port, System.Windows.Threading.Dispatcher dispatcher)
        {
            InstallCSGSI();
            LoadMusic();
            Dispatcher = dispatcher;
            gsl = new GameStateListener(port);
            if (!gsl.Start())
                Environment.Exit(0);
        }
        private void InstallCSGSI()
        {
            RegistryKey rg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Valve\Steam");
            string[] names = rg.GetValueNames();
            foreach (string valueName in names)
                if (valueName == "InstallPath")
                {
                    string file = "gamestate_integration_test.cfg";
                    byte[] filebytes = Resources.gamestate_integration_test;
                    string data = (string)rg.GetValue(valueName);
                    if (!File.Exists(data + @"\steamapps\common\Counter-Strike Global Offensive\csgo\cfg\" + file))
                        try
                        {
                            File.WriteAllBytes(data + @"\steamapps\common\Counter-Strike Global Offensive\csgo\cfg\" + file, filebytes);
                        }
                        catch (FileNotFoundException e)
                        {
                            MessageBox.Show(e.Message + "\nРабота приложения будет немедленно прекращена", "Ошибка");
                            Environment.Exit(0);
                        }
                }
        }
        internal void LoadMusic()
        {
            music = new string[6][];
            antiRandom = new bool[6][];
            try
            {
                music[0] = Directory.GetFiles("flashbang", "*.mp3");
                antiRandom[0] = new bool[music[0].Length];
                music[1] = Directory.GetFiles("win", "*.mp3");
                antiRandom[1] = new bool[music[1].Length];
                string[] x = Directory.GetFiles("lose", "*.wav");
                string[] y = Directory.GetFiles("lose", "*.mp3");
                music[2] = new string[x.Length + y.Length];
                x.CopyTo(music[2], 0); y.CopyTo(music[2], x.Length);
                antiRandom[2] = new bool[music[2].Length];
                music[3] = Directory.GetFiles("health", "*.mp3");
                antiRandom[3] = new bool[music[3].Length];
                music[4] = Directory.GetFiles("death", "*.mp3");
                antiRandom[4] = new bool[music[4].Length];
                music[5] = Directory.GetFiles("freeze", "*.mp3");
                antiRandom[5] = new bool[music[5].Length];
            }
            catch (DirectoryNotFoundException e)
            {
                MessageBox.Show(e.Message + "\nРабота приложения будет немедленно прекращена", "Ошибка");
                Environment.Exit(0);
            }
            ultimate = new string[7][];
            for (int i = 0; i < 7; i++)
            {
                try
                {
                    string dir = Directory.GetDirectories("ultimate/")[i];
                    string[] x = Directory.GetFiles(dir, "*.wav");
                    string[] y = Directory.GetFiles(dir, "*.mp3");
                    string[] files = new string[x.Length + y.Length];
                    x.CopyTo(files, 0); y.CopyTo(files, x.Length);
                    ultimate[i] = files;
                }
                catch (DirectoryNotFoundException e)
                {
                    MessageBox.Show(e.Message + "\nРабота приложения будет немедленно прекращена", "Ошибка");
                    Environment.Exit(0);
                }
            }
        }

        static public MusicPlayer PlayMusic(Uri uri, bool headshot = false)
        {
            foreach (MusicPlayer player in MusicPlayer.players)
                if (!player.IsBusy)
                {
                    Dispatcher.BeginInvoke(new Action(delegate
                    {
                        if (headshot)
                            player.PlayList = GetUri(TypeKill.HeadShot);
                        player.PlayList = uri;
                        player.Play();
                    }));
                    return player;
                }
            return null;
        }
        static public Uri GetUri(TypeKill type)
        {
            return new Uri(Path.Combine(Environment.CurrentDirectory, ultimate[(int)type][rnd.Next(ultimate[(int)type].Length)]));
        }
        static public Uri GetUri(TypeMusic type)
        {
            int id = (int)type;
            if (!Array.Exists(antiRandom[id], item => item.Equals(false)))
                antiRandom[id] = new bool[antiRandom[id].Length];
            start:
            int mus = rnd.Next(music[id].Length);
            if (!antiRandom[id][mus])
                antiRandom[id][mus] = true;
            else goto start;
            return new Uri(Path.Combine(Environment.CurrentDirectory, music[id][mus]));
        }
    }

    internal class MusicPlayer : MediaPlayer
    {
        static public List<MusicPlayer> players = new List<MusicPlayer>(5);
        static public new double Volume
        {
            set
            {
                foreach (MediaPlayer player in players)
                    player.Volume = value / 100d;
            }
        }
        public bool IsBusy
        {
            get;
            private set;
        }
        private List<Uri> playlist = new List<Uri>(2);

        public Uri PlayList
        {
            get
            {
                Uri temp = playlist[0];
                playlist.RemoveAt(0);
                return temp;
            }
            set
            {
                playlist.Add(value);
            }
        }
        public MusicPlayer()
        {
            base.Volume = 1d;
            MediaEnded += (s, e) => { if (playlist.Count != 0) Play(); else IsBusy = false; };
            players.Add(this);
        }
        public new void Play()
        {
            IsBusy = true;
            Open(PlayList);
            base.Play();
        }
        public new void Stop()
        {
            IsBusy = false;
            base.Stop();
        }
        public void Stop(int ms)
        {
            Sleep(ms);
            double volume = base.Volume;
            int time = 5000 / (int)(base.Volume * 100);
            while (base.Volume > 0)
            {
                base.Volume -= 0.01d;
                Sleep(time);
            }
            this.Stop();
            base.Volume = volume;
        }
    }
}

namespace Network
{
    delegate void RecieveHandler(GameState gs);
    delegate void DisconnectClientHandler();
    delegate void ConnectServerHandler(EndPoint ipClient);
    delegate void DisconnectServerHandler(EndPoint ipClient);

    internal class Client
    {
        private Socket client;
        private byte[] buffer = new byte[8192];
        internal event RecieveHandler OnRecieve;
        internal event DisconnectClientHandler OnDisconnect;

        internal Client()
        {
            CreateClient();
        }

        internal bool Connect(string ip, int port)
        {
            try
            {
                client.Connect(IPAddress.Parse(ip), port);
                gsl.NewGameState += SendNewGameState;
                client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), client);
            }
            catch (SocketException error) { MessageBox.Show(error.Message); return false; }
            return true;
        }
        internal void Close()
        {
            client.Close();
        }

        private void SendNewGameState(GameState gs)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(gs.JSON);
                client.Send(bytes);
            }
            catch
            {
                if (client.Connected)
                {
                    client.Disconnect(false);
                    CreateClient();
                    gsl.NewGameState -= SendNewGameState;
                    OnDisconnect?.Invoke();
                }
            }
        }
        private void RecieveNewGameState(object json)
        {
            try
            {
                GameState gs = new GameState(json as string);
                OnRecieve?.Invoke(gs);
            }
            catch { }
        }
        private void ReceiveCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            try
            {
                int received = socket.EndReceive(ar);
                if (received == 1 && buffer[0] == 9)
                {
                    socket.Disconnect(false);
                    CreateClient();
                    gsl.NewGameState -= SendNewGameState;
                    OnDisconnect?.Invoke();
                }
                else
                {
                    byte[] data = new byte[received];
                    Array.Copy(buffer, data, received);
                    socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
                    RecieveNewGameState(Encoding.UTF8.GetString(data));
                }
            }
            catch
            {
                if (!socket.Connected)
                {
                    socket.Disconnect(false);
                    CreateClient();
                    gsl.NewGameState -= SendNewGameState;
                    OnDisconnect?.Invoke();
                }
            }
        }
        private void CreateClient()
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
    }
    internal class Server
    {
        private byte[] buffer = new byte[8192];
        private Socket server;
        private Socket client;
        internal event RecieveHandler OnRecieve;
        internal event ConnectServerHandler OnConnect;
        internal event DisconnectServerHandler OnDisconnect;

        internal Server()
        {
            Start();
        }

        private void Start()
        {
            int port;
            using (WebClient wc = new WebClient())
            {
                port = Convert.ToInt32(wc.DownloadString("http://docred.ml/ip.txt").Split(':')[1]);
            }
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Any, port));
            server.Listen(1);
            server.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }
        internal void Stop()
        {
            client.Send(new byte[] { 9 });
            client.Shutdown(SocketShutdown.Both);
            client.Close();
            server.Close();
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            Socket socket = server.EndAccept(ar);
            client = socket;
            OnConnect?.Invoke(socket.RemoteEndPoint);
            gsl.NewGameState += SendNewGameState;
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
        }
        private void SendNewGameState(GameState gs)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(gs.JSON);
                client.Send(bytes);
            }
            catch
            {
                if (client.Connected)
                {
                    OnDisconnect?.Invoke(client.RemoteEndPoint);
                    client.Close();
                    gsl.NewGameState -= SendNewGameState;
                    server.BeginAccept(new AsyncCallback(AcceptCallback), null);
                }
                else server.Close();
            }
        }
        private void ReceiveCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            try
            {
                int received = socket.EndReceive(ar);
                byte[] data = new byte[received];
                Array.Copy(buffer, data, received);
                try
                {
                    GameState gs = new GameState(Encoding.UTF8.GetString(data));
                    OnRecieve?.Invoke(gs);
                }
                catch { }
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            }
            catch
            {
                if (client.Connected)
                {
                    OnDisconnect?.Invoke(client.RemoteEndPoint);
                    client.Close();
                    gsl.NewGameState -= SendNewGameState;
                    server.BeginAccept(new AsyncCallback(AcceptCallback), null);
                }
                else server.Close();
            }
        }
    }
}