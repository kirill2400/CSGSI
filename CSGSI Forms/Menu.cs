using CSGSI;
using CSGSI.Nodes;
using CSGSI_Library;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static CSGSI_Library.CSGSI_my;
using Network;

namespace CSGSI_Forms
{
    public partial class Menu : Form, IDisposable
    {
        private CSGSI_my my;
        private Client client;
        private Server server;
        private Thread ServerThread;
        private ManualResetEventSlim[] signal = new ManualResetEventSlim[]
        {
            new ManualResetEventSlim(), new ManualResetEventSlim(),
            new ManualResetEventSlim(), new ManualResetEventSlim()
        };
        private GameState _gs;
        private MusicPlayer musicPlayer;
        private bool freezeTime;
        private string LocalPlayer = "";
        private Overlay overlay;
        internal Menu(int port)
        {
            InitializeComponent();
            my = new CSGSI_my(port, System.Windows.Threading.Dispatcher.CurrentDispatcher);
            gsl.NewGameState += OnNewGameState;
            Text = "Listening...";
            Height = 135;
            MusicPlayer player = null;
            for (int i = 0; i < 5; i++)
                player = new MusicPlayer();
            
        }

        private void OnNewGameState(GameState gs)
        {
            _gs = gs;
            if (LocalPlayer == string.Empty)
                LocalPlayer = gs.Player.SteamID;
            if (gs.Round.Phase != RoundPhase.Undefined)
            {
                if (gs.Player.SteamID == LocalPlayer || LocalAllPlayers.Checked)
                {
                    if (LocalKillingSpree.Checked)
                        signal[0].Set();
                    if (LocalFlashbang.Checked)
                        signal[1].Set();
                }
                if (LocalMusic.Checked)
                    signal[2].Set();
                if (LocalFreeze.Checked)
                    signal[3].Set();
            }
        }
        private void Kill()
        {
            while (true)
            {
                signal[0].Wait();
                bool headshot = false;
                Uri uri = null;

                if (_gs.Previously.Player.State.RoundKills != -1 && _gs.Player.State.RoundKills > 0)
                {
                    switch (_gs.Player.State.RoundKills)
                    {
                        case 0:
                            break;
                        case 1:
                            uri = GetUri(TypeKill.FirstBlood);
                            break;
                        case 2:
                            uri = GetUri(TypeKill.DoubleKill);
                            break;
                        case 3:
                            uri = GetUri(TypeKill.TripleKill);
                            break;
                        case 4:
                            uri = GetUri(TypeKill.UltraKill);
                            break;
                        case 5:
                            uri = GetUri(TypeKill.Rampage);
                            break;
                        default:
                            uri = GetUri(TypeKill.Other);
                            break;
                    }
                    if (_gs.Previously.Player.State.RoundKillHS != -1 && _gs.Player.State.RoundKillHS > _gs.Previously.Player.State.RoundKillHS)
                        headshot = true;
                }
                if (uri == null && headshot)
                    uri = GetUri(TypeKill.HeadShot);
                else if (uri != null)
                    PlayMusic(uri, headshot);
                signal[0].Reset();
            }
        }
        private void Flashbang()
        {
            while (true)
            {
                signal[1].Wait();
                if (_gs.Previously.Player.State.Flashed == 0 || _gs.Previously.Player.State.Burning == 0)
                    PlayMusic(GetUri(TypeMusic.FlashBang));
                signal[1].Reset();
            }
        }
        private void EndRound()
        {
            while (true)
            {
                signal[2].Wait();
                if (_gs.Previously.Round.Phase == RoundPhase.Live && _gs.Round.Phase == RoundPhase.Over && _gs.Round.WinTeam != RoundWinTeam.Undefined)
                {
                    Uri uri = null;
                    RoundWinTeam team = RoundWinTeam.Undefined;
                    
                    if (_gs.Player.Team == PlayerTeam.CT)
                        team = RoundWinTeam.CT;
                    else
                        team = RoundWinTeam.T;
                    if (_gs.Round.WinTeam == team)
                        uri = GetUri(TypeMusic.Win);
                    else
                        uri = GetUri(TypeMusic.Lose);
                    PlayMusic(uri);
                }
                signal[2].Reset();
            }
        }
        private void FreezeTime()
        {
            while (true)
            {
                signal[3].Wait();
                if (((_gs.Previously.Map.TeamCT.TimeoutsRemaining == 1 && _gs.Map.TeamCT.TimeoutsRemaining == 0) || (_gs.Previously.Map.TeamT.TimeoutsRemaining == 1 && _gs.Map.TeamT.TimeoutsRemaining == 0)) && !freezeTime)
                    freezeTime = true;
                else if (_gs.Round.Phase == RoundPhase.FreezeTime && freezeTime)
                {
                    freezeTime = false;
                    musicPlayer = PlayMusic(GetUri(TypeMusic.FreezeTime));
                }
                else if (_gs.Previously.Round.Phase == RoundPhase.FreezeTime && musicPlayer != null)
                {
                    Dispatcher.BeginInvoke(new Action(delegate
                    {
                        musicPlayer.Stop(5000);
                        musicPlayer = null;
                    }));
                }
                signal[3].Reset();
            }
        }
        private void RecieveNewGameState(GameState gs)
        {
            Uri uri = null;
            overlay.SetGS(gs);
            if (NetworkHealth.Checked)
            {
                if (gs.Previously.Player.State.Health != -1)
                    if (gs.Player.State.Health > 0 && gs.Player.State.Health < 50)
                        uri = GetUri(TypeMusic.LowHealth);
                    else if (gs.Player.State.Health == 0)
                        uri = GetUri(TypeMusic.Death);
            }
            if (uri != null)
                PlayMusic(uri);
        }

        private void SpecialEvent(object sender, EventArgs e)
        {
            switch (sender.GetType().Name)
            {
                case "Button":
                    Button button = sender as Button;
                    if (button.Name == "ConnectButton")
                    {
                        try
                        {
                            string ip; int port;
                            if (address.Text != string.Empty)
                            {
                                string[] data = address.Text.Split(':');
                                ip = data[0]; port = Convert.ToInt32(data[1]);
                            }
                            else
                                using (WebClient wc = new WebClient())
                                {
                                    string[] data = wc.DownloadString("http://docred.ml/ip.txt").Split(':');
                                    ip = data[0]; port = Convert.ToInt32(data[1]);
                                }
                            if (client.Connect(ip, port))
                                button.Enabled = false;
                        }
                        catch (SocketException error) { MessageBox.Show(error.Message); }
                    }
                    else if (button.Name == "LocalHostButton")
                        address.Text = "127.0.0.1:5001";
                    else if (button.Name == "ServerButton")
                    {
                        ServerThread = new Thread(StartServer);
                        ServerThread.Start();
                        ServerLog.Text += "Setting up server..." + Environment.NewLine;
                    }
                    else if (button.Name == "StopServerButton")
                    {
                        StopServer();
                        ServerLog.Text += "Shutdown server." + Environment.NewLine;
                    }
                    else
                        my.LoadMusic();
                    break;
                case "CheckBox":
                    CheckBox checkbox = sender as CheckBox;
                    if (checkbox.Checked)
                    {
                        if (overlay == null)
                            overlay = new Overlay();
                        overlay.Show();
                    }
                    else overlay.Hide();
                    break;
                case "TrackBar":
                    MusicPlayer.Volume = ((TrackBar)sender).Value;
                    break;
                case "PictureBox":
                    if (Height == 135)
                        Height = 375;
                    else Height = 135;
                    break;
            }
        }
        private void MuteUnMute(object sender, EventArgs e)
        {
            PictureBox pic = sender as PictureBox;
            if ((string)pic.Tag == "unMute")
            {
                MusicPlayer.Volume = 0;
                pic.Image = Properties.Resources.Off;
                pic.Tag = "Mute";
                spVolume.Enabled = false;
            }
            else if ((string)pic.Tag == "Mute")
            {
                MusicPlayer.Volume = spVolume.Value;
                pic.Image = Properties.Resources.On;
                pic.Tag = "unMute";
                spVolume.Enabled = true;
            }
        }
        private void StartThreads(object sender, EventArgs e)
        {
            new Thread(StartClient).Start();
            new Thread(Kill).Start();
            new Thread(Flashbang).Start();
            new Thread(EndRound).Start();
            new Thread(FreezeTime).Start();
        }
        private void CloseApplication(object sender, FormClosedEventArgs e)
        {
            client.Close();
            Environment.Exit(0);
        }

        private void StartClient()
        {
            client = new Client();
            ConnectButton.BeginInvoke(new Action(delegate
            {
                ConnectButton.Enabled = true;
            }));
            client.OnRecieve += RecieveNewGameState;
            client.OnDisconnect += () => {
                ConnectButton.BeginInvoke(new Action(delegate
                {
                    ConnectButton.Enabled = true;
                }));
            };
            System.Windows.Threading.Dispatcher.Run();
        }
        private void StartServer()
        {
            server = new Server();
            ServerButton.BeginInvoke(new Action(delegate
            {
                ServerButton.Enabled = false;
                StopServerButton.Enabled = true;
            }));
            server.OnRecieve += RecieveNewGameState;
            server.OnConnect += (e) => {
                ServerLog.BeginInvoke(new Action(delegate
                {
                    ServerLog.Text += e + " has been connected." + Environment.NewLine;
                }));
            };
            server.OnDisconnect += (e) => {
                ServerLog.BeginInvoke(new Action(delegate
                {
                    ServerLog.Text += e + " has been disconnected." + Environment.NewLine;
                }));
            };
            System.Windows.Threading.Dispatcher.Run();
        }
        private void StopServer()
        {
            server.Stop();
            ServerButton.BeginInvoke(new Action(delegate
            {
                ServerButton.Enabled = true;
                StopServerButton.Enabled = false;
            }));
        }

        public new void Dispose()
        {
            base.Dispose();
            overlay.Dispose();
        }
    }
}