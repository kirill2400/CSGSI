using System;
using System.Collections.Generic;
using System.IO;
using CSGSI;
using CSGSI.Nodes;
using Microsoft.Win32;

namespace CSGSI_Library
{
    public class CSGSI_my
    {
        public GameStateListener gsl;
        public string[] winmusic;
        public string[] losemusic;
        public string[] flashmusic;
        public Dictionary<string, string[]> ultimate;
        public Random rnd = new Random();
        public RoundWinTeam playerTeam;
        public int kills = 0;
        public int killhs = 0;

        public CSGSI_my()
        {
            InstallCSGSI();
            LoadMusic();
            gsl = new GameStateListener(3000);
            if (!gsl.Start())
                Environment.Exit(0);
        }
        public void LoadMusic()
        {
            winmusic = Directory.GetFiles(Directory.GetCurrentDirectory() + "/win", "*.mp3");
            losemusic = Directory.GetFiles(Directory.GetCurrentDirectory() + "/lose", "*.mp3");
            flashmusic = Directory.GetFiles(Directory.GetCurrentDirectory() + "/flashbang", "*.mp3");
            ultimate = new Dictionary<string, string[]>(6);
            for (int i = 0; i < 6; i++)
            {
                string dir = Directory.GetDirectories("ultimate/")[i];
                string[] x = Directory.GetFiles(dir, "*.wav");
                string[] y = Directory.GetFiles(dir, "*.mp3");
                string[] files = new string[x.Length + y.Length];
                x.CopyTo(files, 0); y.CopyTo(files, x.Length);
                ultimate.Add(dir, files);
            }
        }
        private void InstallCSGSI()
        {
            RegistryKey rg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Valve\Steam");
            string[] names = rg.GetValueNames();
            foreach (string valueName in names)
                if (valueName == "InstallPath")
                    if (!File.Exists((string)rg.GetValue(valueName) + @"\steamapps\common\Counter-Strike Global Offensive\csgo\cfg\gamestate_integration_test.cfg"))
                        File.Copy("gamestate_integration_test.cfg", (string)rg.GetValue(valueName) + @"\steamapps\common\Counter-Strike Global Offensive\csgo\cfg\gamestate_integration_test.cfg");
        }
    }
}
