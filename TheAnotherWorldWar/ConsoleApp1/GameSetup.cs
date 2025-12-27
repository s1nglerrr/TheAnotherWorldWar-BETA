using System;
using System.Collections.Generic;
using System.Text;
using static ConsoleApp1.Country;

namespace ConsoleApp1
{
    public class GameSetup
    {
        public string SetupGameName { get; set; }
        public int SetupCountriesCount { get; set; }
        public string SetupPlayerСountryName { get; set; }
        public string SetupPlayerCapitalName { get; set; }
        public IdeologiesEnum SetupPlayerСountryIdeology { get; set; }
        public ConsoleColor SetupPlayerСountryСolor { get; set; }
        public int SetupMapSize { get; set; }

        public GameSetup(string setupGameName, int setupCountriesCount, string setupPlayerСountryName, string setupPlayerCapitalName, IdeologiesEnum setupСountryIdeology, ConsoleColor setupСountryСolor, int setupMapSize)
        {
            SetupGameName = setupGameName;
            SetupCountriesCount = setupCountriesCount;
            SetupPlayerСountryName = setupPlayerСountryName;
            SetupPlayerCapitalName = setupPlayerCapitalName;
            SetupPlayerСountryIdeology = setupСountryIdeology;
            SetupPlayerСountryСolor = setupСountryСolor;
            SetupMapSize = setupMapSize;
        }
    }
}
