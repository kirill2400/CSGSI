using CSGSI;
using CSGSI.Nodes;
using CSGSI_Library;
using System;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;

namespace CSGSI_Forms
{
    public partial class Menu : Form
    {
        private CSGSI_my my = new CSGSI_my();
        private int Volume
        {
            set { MusicPlayer.ChangeVolume(value); }
        }
        private Dispatcher Dispatcher;
        private MusicPlayer[] players = new MusicPlayer[5];
        public Menu()
        {
            InitializeComponent();
            Dispatcher = Dispatcher.CurrentDispatcher;
            my.gsl.NewGameState += new NewGameStateHandler(OnNewGameState);
            this.Text = "Listening...";
            for (int i = 0; i < players.Length; i++)
                players[i] = new MusicPlayer();
        }

        private void OnNewGameState(GameState gs)
        {
            Thread headshot = new Thread(Kill);
            headshot.IsBackground = true;
            headshot.Start(gs);
            Thread flashbang = new Thread(Flashbang);
            flashbang.IsBackground = true;
            flashbang.Start(gs);
            Thread endround = new Thread(EndRound);
            endround.IsBackground = true;
            endround.Start(gs);
        }
        private bool flash = false;
        private bool run = false;
        private void Kill(object gs)
        {
            GameState _gs = gs as GameState;
            int kills = _gs.Player.State.RoundKills;
            int killhs = _gs.Player.State.RoundKillHS;
            string url = null;
            if ((_gs.Round.Phase == RoundPhase.FreezeTime) || (killhs == 0 && my.killhs > 0 && kills == 0 && my.kills > 0))
            { my.kills = 0; my.killhs = 0; }
            if (killhs == 1 && kills == 1 && my.kills == 0)
            {
                my.killhs = killhs; my.kills = kills;
                url = my.ultimate["ultimate/headshot"][my.rnd.Next(my.ultimate["ultimate/headshot"].Length)];
            }
            switch (kills)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    if (my.kills != 2)
                    {
                        my.kills = 2;
                        url = GetUrl("doublekill", killhs);
                    }
                    break;
                case 3:
                    if (my.kills != 3)
                    {
                        my.kills = 3;
                        url = GetUrl("triplekill", killhs);
                    }
                    break;
                case 4:
                    if (my.kills != 4)
                    {
                        my.kills = 4;
                        url = my.ultimate["ultimate/ultrakill"][my.rnd.Next(my.ultimate["ultimate/ultrakill"].Length)];
                    }
                    break;
                case 5:
                    if (my.kills != 5)
                    {
                        my.kills = 5;
                        url = my.ultimate["ultimate/rampage"][my.rnd.Next(my.ultimate["ultimate/rampage"].Length)];
                    }
                    break;
                default:
                    if (my.kills < kills)
                    {
                        my.kills = kills;
                        url = GetUrl("other", killhs);
                    }
                    break;
            }
            if (!string.IsNullOrEmpty(url))
            {
                foreach (MusicPlayer player in players)
                    if (!player.isBusy)
                    {
                        Dispatcher.BeginInvoke(new ThreadStart(delegate {
                            player.Open(new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, url)));
                            player.Play();
                        }));
                        break;
                    }
            }
        }
        private void Flashbang(object gs)
        {
            GameState _gs = gs as GameState;
            string url = null;
            if (_gs.Player.State.Flashed == 1 && !flash)
            {
                flash = true;
                url = my.flashmusic[my.rnd.Next(my.flashmusic.Length)];
            }
            else if (_gs.Player.State.Flashed == 0 && flash)
                flash = false;
            if (!string.IsNullOrEmpty(url))
            {
                foreach (MusicPlayer player in players)
                    if (!player.isBusy)
                    {
                        Dispatcher.BeginInvoke(new ThreadStart(delegate {
                            player.Open(new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, url)));
                            player.Play();
                        }));
                        break;
                    }
            }
        }
        private void EndRound(object gs)
        {
            GameState _gs = gs as GameState;
            string url = null;
            if (_gs.Round.Phase == RoundPhase.Over && !run)
            {
                run = true;
                switch (_gs.Player.Team)
                {
                    case "CT":
                        my.playerTeam = RoundWinTeam.CT;
                        break;
                    case "T":
                        my.playerTeam = RoundWinTeam.T;
                        break;
                    default:
                        my.playerTeam = RoundWinTeam.Undefined;
                        break;
                }
                if (_gs.Round.WinTeam == my.playerTeam)
                    url = my.winmusic[my.rnd.Next(my.winmusic.Length)];
                else if (_gs.Round.WinTeam != my.playerTeam)
                    url = my.losemusic[my.rnd.Next(my.losemusic.Length)];
            }
            else if (_gs.Round.Phase == RoundPhase.FreezeTime && run)
                run = false;
            if (!string.IsNullOrEmpty(url))
            {
                foreach (MusicPlayer player in players)
                    if (!player.isBusy)
                    {
                        Dispatcher.BeginInvoke(new ThreadStart(delegate {
                            player.Open(new Uri(System.IO.Path.Combine(Environment.CurrentDirectory, url)));
                            player.Play();
                        }));
                        break;
                    }
                run = true;
            }
        }

        private void SpecialEvent(object sender, EventArgs e)
        {
            switch (sender.GetType().Name)
            {
                case "Button":
                    my.LoadMusic();
                    break;
                case "TrackBar":
                    Volume = ((TrackBar)sender).Value;
                    break;
            }
        }
        private void MuteUnMute(object sender, EventArgs e)
        {
            PictureBox pic = sender as PictureBox;
            if ((string)pic.Tag == "unMute")
            {
                Volume = 0;
                pic.Image = Properties.Resources.Off;
                pic.Tag = "Mute";
                spVolume.Enabled = false;
            }
            else if ((string)pic.Tag == "Mute")
            {
                Volume = spVolume.Value;
                pic.Image = Properties.Resources.On;
                pic.Tag = "unMute";
                spVolume.Enabled = true;
            }
        }
        private void CloseApplication(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        private string GetUrl(string kill, int killhs)
        {
            if (my.rnd.Next(100) <= 60)
                return my.ultimate["ultimate/" + kill][my.rnd.Next(0, my.ultimate["ultimate/" + kill].Length - 1)];
            else if (my.killhs < killhs)
            {
                my.killhs = killhs;
                return my.ultimate["ultimate/headshot"][my.rnd.Next(0, my.ultimate["ultimate/headshot"].Length - 1)];
            }
            return "";
        }
    }
}
