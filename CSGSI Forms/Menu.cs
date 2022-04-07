using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;
using CSGSI;
using CSGSI.Nodes;
using CSGSI_Forms.Properties;
using CSGSI_Library;
using Network;
using static CSGSI_Library.CSGSI_my;

namespace CSGSI_Forms
{
    public partial class Menu : Form, IDisposable
    {
        private GameState _gs;
        private Client client;
        private bool freezeTime;
        private string LocalPlayer = "";
        private MusicPlayer musicPlayer;
        private readonly CSGSI_my my;
        private Overlay overlay;
        private Server server;
        private Thread ServerThread;

        internal Menu(int port)
        {
            InitializeComponent();
            my = new CSGSI_my(port);
            gsl.NewGameState += OnNewGameState;
            Text = "Listening...";
            Height = 135;
            MusicPlayer.InitializeMusicPlayers(5);
        }

        public new void Dispose()
        {
            base.Dispose();
            overlay.Dispose();
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
                        CheckKill();
                    if (LocalFlashbang.Checked)
                        CheckFlashbang();
                }

                if (LocalMusic.Checked)
                    CheckEndRound();
                if (LocalFreeze.Checked)
                    CheckFreezeTime();
            }
        }

        private void CheckKill()
        {
            var headshot = false;
            Uri uri = null;

            if (_gs.Previously.Player.State.RoundKills != -1 && _gs.Player.State.RoundKills > 0)
            {
                if (Enum.TryParse(_gs.Player.State.RoundKills.ToString(), out TypeKill typekill))
                {
                    uri = my.GetUri(typekill);
                }

                if (_gs.Previously.Player.State.RoundKillHS != -1 &&
                    _gs.Player.State.RoundKillHS > _gs.Previously.Player.State.RoundKillHS)
                    headshot = true;
            }

            if (uri != null)
                my.PlayMusic(uri, headshot);
        }

        private void CheckFlashbang()
        {
            if (_gs.Previously.Player.State.Flashed == 0 || _gs.Previously.Player.State.Burning == 0)
                my.PlayMusic(my.GetUri(TypeMusic.flashbang));
        }

        private void CheckEndRound()
        {
            if (_gs.Previously.Round.Phase == RoundPhase.Live && _gs.Round.Phase == RoundPhase.Over &&
                _gs.Round.WinTeam != RoundWinTeam.Undefined)
            {
                Uri uri = null;
                var team = RoundWinTeam.Undefined;

                if (_gs.Player.Team == PlayerTeam.CT)
                    team = RoundWinTeam.CT;
                else
                    team = RoundWinTeam.T;
                if (_gs.Round.WinTeam == team)
                    uri = my.GetUri(TypeMusic.win);
                else
                    uri = my.GetUri(TypeMusic.lose);
                my.PlayMusic(uri);
            }
        }

        private void CheckFreezeTime()
        {
            if ((_gs.Previously.Map.TeamCT.TimeoutsRemaining == 1 && _gs.Map.TeamCT.TimeoutsRemaining == 0 ||
                 _gs.Previously.Map.TeamT.TimeoutsRemaining == 1 && _gs.Map.TeamT.TimeoutsRemaining == 0) &&
                !freezeTime)
            {
                freezeTime = true;
            }
            else if (_gs.Round.Phase == RoundPhase.FreezeTime && freezeTime)
            {
                freezeTime = false;
                musicPlayer = my.PlayMusic(my.GetUri(TypeMusic.freezetime));
            }
            else if (_gs.Previously.Round.Phase == RoundPhase.FreezeTime && musicPlayer != null)
            {
                musicPlayer.Stop(5000);
                musicPlayer = null;
            }
        }

        private void ReceiveNewGameState(GameState gs)
        {
            Uri uri = null;
            overlay.SetGS(gs);
            if (NetworkHealth.Checked)
                if (gs.Previously.Player.State.Health != -1)
                    if (gs.Player.State.Health > 0 && gs.Player.State.Health < 50)
                        uri = my.GetUri(TypeMusic.lowhealth);
                    else if (gs.Player.State.Health == 0)
                        uri = my.GetUri(TypeMusic.death);
            if (uri != null)
                my.PlayMusic(uri);
        }

        private void SpecialEvent(object sender, EventArgs e)
        {
            switch (sender.GetType().Name)
            {
                case "Button":
                    var button = sender as Button;
                    if (button.Name == "ConnectButton")
                    {
                        try
                        {
                            if (address.Text != string.Empty)
                            {
                                var data = address.Text.Split(':');
								string ip = data[0];
								int port = Convert.ToInt32(data[1]);

								if (client.Connect(ip, port))
									button.Enabled = false;
							}

                        }
                        catch (SocketException error)
                        {
                            MessageBox.Show(error.Message);
                        }
                    }
                    else if (button.Name == "LocalHostButton")
                    {
                        address.Text = "127.0.0.1:25565";
                    }
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
                    {
                        my.LoadMusic();
                    }

                    break;
                case "CheckBox":
                    var checkbox = sender as CheckBox;
                    if (checkbox.Checked)
                    {
                        if (overlay == null)
                            overlay = new Overlay();
                        overlay.Show();
                    }
                    else
                    {
                        overlay.Hide();
                    }

                    break;
                case "TrackBar":
                    MusicPlayer.Volume = ((TrackBar) sender).Value;
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
            var pic = sender as PictureBox;
            if ((string) pic.Tag == "unMute")
            {
                MusicPlayer.Volume = 0;
                pic.Image = Resources.Off;
                pic.Tag = "Mute";
                spVolume.Enabled = false;
            }
            else if ((string) pic.Tag == "Mute")
            {
                MusicPlayer.Volume = spVolume.Value;
                pic.Image = Resources.On;
                pic.Tag = "unMute";
                spVolume.Enabled = true;
            }
        }

        private void StartThreads(object sender, EventArgs e)
        {
            new Thread(StartClient).Start();
        }

        private void CloseApplication(object sender, FormClosedEventArgs e)
        {
            client.Close();
            Environment.Exit(0);
        }

        private void StartClient()
        {
            client = new Client();
            ConnectButton.BeginInvoke(new Action(delegate { ConnectButton.Enabled = true; }));
            client.OnRecieve += ReceiveNewGameState;
            client.OnDisconnect += () =>
            {
                ConnectButton.BeginInvoke(new Action(delegate { ConnectButton.Enabled = true; }));
            };
            Dispatcher.Run();
        }

        private void StartServer()
        {
            server = new Server();
            ServerButton.BeginInvoke(new Action(delegate
            {
                ServerButton.Enabled = false;
                StopServerButton.Enabled = true;
            }));
            server.OnRecieve += ReceiveNewGameState;
            server.OnConnect += e =>
            {
                ServerLog.BeginInvoke(new Action(delegate
                {
                    ServerLog.Text += e + " has been connected." + Environment.NewLine;
                }));
            };
            server.OnDisconnect += e =>
            {
                ServerLog.BeginInvoke(new Action(delegate
                {
                    ServerLog.Text += e + " has been disconnected." + Environment.NewLine;
                }));
            };
            Dispatcher.Run();
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
    }
}