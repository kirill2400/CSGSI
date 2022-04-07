using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Windows.Media;
using CSGSI;
using CSGSI_Forms.Properties;
using Microsoft.Win32;
using static System.Threading.Thread;
using static CSGSI_Library.CSGSI_my;

namespace CSGSI_Library
{
    internal enum TypeKill
    {
        firstblood = 1,
        doublekill,
        triplekill,
        ultrakill,
        rampage,
        headshot,
        other,
    }

    internal enum TypeMusic
    {
        flashbang,
        win,
        lose,
        lowhealth,
        death,
        freezetime
    }

    internal class CSGSI_my
    {
        private readonly Random rnd = new Random();
        internal static GameStateListener gsl;
        private Dictionary<TypeMusic, List<string>> music;
        private Dictionary<TypeKill, List<string>> ultimate;
        private bool[][] antiRandom;

        internal CSGSI_my(int port)
        {
            InstallCSGSI();
            LoadMusic();
            gsl = new GameStateListener(port);
            if (!gsl.Start())
                Environment.Exit(0);
        }

        private void InstallCSGSI()
        {
            var rg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Valve\Steam");
            var names = rg.GetValueNames();
            foreach (var valueName in names)
                if (valueName == "InstallPath")
                {
                    var file = "gamestate_integration_test.cfg";
                    var filebytes = Resources.gamestate_integration_test;
                    var data = (string) rg.GetValue(valueName);
                    if (!File.Exists(data + @"\steamapps\common\Counter-Strike Global Offensive\csgo\cfg\" + file))
                        try
                        {
                            File.WriteAllBytes(
                                data + @"\steamapps\common\Counter-Strike Global Offensive\csgo\cfg\" + file,
                                filebytes);
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
            music = new Dictionary<TypeMusic, List<string>>(6);
            antiRandom = new bool[6][];
            try
            {
                music.Add(TypeMusic.flashbang, Directory.GetFiles(TypeMusic.flashbang.ToString(), "*.mp3").ToList());
                music.Add(TypeMusic.win, Directory.GetFiles(TypeMusic.win.ToString(), "*.mp3").ToList());
                music.Add(TypeMusic.lose, Directory.GetFiles(TypeMusic.lose.ToString(), "*.mp3").Concat(Directory.GetFiles(TypeMusic.lose.ToString(), "*.wav")).ToList());
                music.Add(TypeMusic.lowhealth, Directory.GetFiles(TypeMusic.lowhealth.ToString(), "*.mp3").ToList());
                music.Add(TypeMusic.death, Directory.GetFiles(TypeMusic.death.ToString(), "*.mp3").ToList());
                music.Add(TypeMusic.freezetime, Directory.GetFiles(TypeMusic.freezetime.ToString(), "*.mp3").ToList());
            }
            catch (DirectoryNotFoundException e)
            {
                MessageBox.Show(e.Message + "\nРабота приложения будет немедленно прекращена", "Ошибка");
                Environment.Exit(0);
            }

            ultimate = new Dictionary<TypeKill, List<string>>(7);
            var dirs = Directory.GetDirectories("ultimate/");
            for (var i = 0; i < 7; i++)
                try
                {
                    var dir = dirs[i];
                    var folderName = dir.Replace("ultimate/", "");

                    if (Enum.TryParse(folderName, out TypeKill key))
                    {
                        ultimate.Add(key, Directory.GetFiles(dir, "*.mp3").Concat(Directory.GetFiles(dir, "*.wav")).ToList());
                    }
                }
                catch (DirectoryNotFoundException e)
                {
                    MessageBox.Show(e.Message + "\nРабота приложения будет немедленно прекращена", "Ошибка");
                    Environment.Exit(0);
                }
        }

        public MusicPlayer PlayMusic(Uri uri, bool headshot = false)
        {
            foreach (var player in MusicPlayer.players)
                if (!player.IsBusy)
                {
                    if (headshot)
                        player.PlayList = GetUri(TypeKill.headshot);
                    player.PlayList = uri;
                    player.Play();
                    return player;
                }

            return null;
        }

        public Uri GetUri(TypeKill type)
        {
            return new Uri(Path.Combine(Environment.CurrentDirectory,
                ultimate[type][rnd.Next(ultimate[type].Count)]));
        }

        public Uri GetUri(TypeMusic type)
        {
            var id = (int) type;
            if (!Array.Exists(antiRandom[id], item => item.Equals(false)))
                antiRandom[id] = new bool[antiRandom[id].Length];
            start:
            var mus = rnd.Next(music[type].Count);
            if (!antiRandom[id][mus])
                antiRandom[id][mus] = true;
            else goto start;
            return new Uri(Path.Combine(Environment.CurrentDirectory, music[type][mus]));
        }
    }

    internal class MusicPlayer : MediaPlayer
    {
        public static List<MusicPlayer> players = new List<MusicPlayer>(5);
        private readonly List<Uri> playlist = new List<Uri>(2);

        public static void InitializeMusicPlayers(int count)
        {
            for (var i = 0; i < count; i++)
            {
                players.Add(new MusicPlayer());
            }
        }

        public MusicPlayer()
        {
            base.Volume = 1d;
            MediaEnded += (s, e) =>
            {
                if (playlist.Count != 0) Play();
                else IsBusy = false;
            };
        }

        public new static double Volume
        {
            set
            {
                foreach (MediaPlayer player in players)
                    player.Volume = value / 100d;
            }
        }

        public bool IsBusy { get; private set; }

        public Uri PlayList
        {
            get
            {
                var temp = playlist[0];
                playlist.RemoveAt(0);
                return temp;
            }
            set => playlist.Add(value);
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
            var volume = base.Volume;
            var time = 5000 / (int) (base.Volume * 100);
            while (base.Volume > 0)
            {
                base.Volume -= 0.01d;
                Sleep(time);
            }

            Stop();
            base.Volume = volume;
        }
    }
}

namespace Network
{
    internal delegate void RecieveHandler(GameState gs);

    internal delegate void DisconnectClientHandler();

    internal delegate void ConnectServerHandler(EndPoint ipClient);

    internal delegate void DisconnectServerHandler(EndPoint ipClient);

    internal class Client
    {
        private readonly byte[] buffer = new byte[8192];
        private Socket client;

        internal Client()
        {
            CreateClient();
        }

        internal event RecieveHandler OnRecieve;
        internal event DisconnectClientHandler OnDisconnect;

        internal bool Connect(string ip, int port)
        {
            try
            {
                client.Connect(IPAddress.Parse(ip), port);
                gsl.NewGameState += SendNewGameState;
                client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, client);
            }
            catch (SocketException error)
            {
                MessageBox.Show(error.Message);
                return false;
            }

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
                var bytes = Encoding.UTF8.GetBytes(gs.JSON);
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
                var gs = new GameState(json as string);
                OnRecieve?.Invoke(gs);
            }
            catch
            {
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            var socket = (Socket) ar.AsyncState;
            try
            {
                var received = socket.EndReceive(ar);
                if (received == 1 && buffer[0] == 9)
                {
                    socket.Disconnect(false);
                    CreateClient();
                    gsl.NewGameState -= SendNewGameState;
                    OnDisconnect?.Invoke();
                }
                else
                {
                    var data = new byte[received];
                    Array.Copy(buffer, data, received);
                    socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, socket);
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
        private readonly byte[] buffer = new byte[8192];
        private Socket client;
        private Socket server;

        internal Server()
        {
            Start();
        }

        internal event RecieveHandler OnRecieve;
        internal event ConnectServerHandler OnConnect;
        internal event DisconnectServerHandler OnDisconnect;

        private void Start()
        {
            int port = 25565;
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Any, port));
            server.Listen(1);
            server.BeginAccept(AcceptCallback, null);
        }

        internal void Stop()
        {
            client.Send(new byte[] {9});
            client.Shutdown(SocketShutdown.Both);
            client.Close();
            server.Close();
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            var socket = server.EndAccept(ar);
            client = socket;
            OnConnect?.Invoke(socket.RemoteEndPoint);
            gsl.NewGameState += SendNewGameState;
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, socket);
        }

        private void SendNewGameState(GameState gs)
        {
            try
            {
                var bytes = Encoding.UTF8.GetBytes(gs.JSON);
                client.Send(bytes);
            }
            catch
            {
                if (client.Connected)
                {
                    OnDisconnect?.Invoke(client.RemoteEndPoint);
                    client.Close();
                    gsl.NewGameState -= SendNewGameState;
                    server.BeginAccept(AcceptCallback, null);
                }
                else
                {
                    server.Close();
                }
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            var socket = (Socket) ar.AsyncState;
            try
            {
                var received = socket.EndReceive(ar);
                var data = new byte[received];
                Array.Copy(buffer, data, received);
                try
                {
                    var gs = new GameState(Encoding.UTF8.GetString(data));
                    OnRecieve?.Invoke(gs);
                }
                catch
                {
                }

                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, socket);
            }
            catch
            {
                if (client.Connected)
                {
                    OnDisconnect?.Invoke(client.RemoteEndPoint);
                    client.Close();
                    gsl.NewGameState -= SendNewGameState;
                    server.BeginAccept(AcceptCallback, null);
                }
                else
                {
                    server.Close();
                }
            }
        }
    }
}