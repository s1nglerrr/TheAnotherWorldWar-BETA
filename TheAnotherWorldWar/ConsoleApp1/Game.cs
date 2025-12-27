using System;
using System.Collections.Generic;
using System.Text;
using static ConsoleApp1.Army;
using static ConsoleApp1.City;
using static ConsoleApp1.Country;

namespace ConsoleApp1
{
    public class Game
    {
        public string GameName { get; set; }
        public int CountriesCount { get; set; }
        public string PlayerСountryName { get; set; }
        public string PlayerCapitalName { get; set; }
        public IdeologiesEnum PlayerСountryIdeology { get; set; }
        public ConsoleColor PlayerСountryСolor { get; set; }
        public int MapSize { get; set; }
        public Map Map { get; set; }
        public List<Country> GameCountries { get; set; }
        public ArmyMovementInfo ActiveMovementInfo { get; set; }
        public Warship.WarshipMovementInfo ActiveWarshipMovementInfo { get; set; }

        public static Dictionary<string, char> MapObjects = new Dictionary<string, char>()
        {
            ["Border"] = '#',

            ["Land"] = '0',
            ["Sea"] = '~',
            ["Forest"] = 'T',
            ["Desert"] = '@',
            ["Mountain"] = 'M',
            ["Hill"] = '^',

            ["Capital"] = 'X',
            ["City"] = 'x',
            ["Port"] = 'P',

            ["CargoShip"] = 'b',
            ["CombatShip"] = 's'
        };

        public Game(GameSetup gameSetup)
        {
            GameName = gameSetup.SetupGameName;
            CountriesCount = gameSetup.SetupCountriesCount;
            PlayerСountryName = gameSetup.SetupPlayerСountryName;
            PlayerCapitalName = gameSetup.SetupPlayerCapitalName;
            PlayerСountryIdeology = gameSetup.SetupPlayerСountryIdeology;
            PlayerСountryСolor = gameSetup.SetupPlayerСountryСolor;
            MapSize = gameSetup.SetupMapSize;
        }

        public void InitializeGame()
        {
            Map map = new Map();
            MapGenerator generator = new MapGenerator();
            map.MetaMap = generator.GenerateMap(MapSize);

            Map = map;
            GameCountries = GenerateCountries(Map.MetaMap, MapSize, CountriesCount);

            MapGenerator.PlaceCapitals(GameCountries, Map.MetaMap, MapSize);

            GameCountries[0].IsBot = false;
            GameCountries[0].CountryName = PlayerСountryName;
            GameCountries[0].CountryColor = PlayerСountryСolor;
            GameCountries[0].CountryIdeology = PlayerСountryIdeology;
            GameCountries[0].CountryСitys.First(c => c.IsCapital).СityName = PlayerCapitalName;

            GameSpace.ShowGameSpace(this);
        }

        public void NextTurn()
        {
            CountMoneyOfEachCountry();
            CountPopulationOfEachCountry();
            ProduceMilitaryEquipment();
            ResetDivisionMovementFlags();
            ResetShipMovementFlags();
            CollectCivilianShipIncome();

            GameSpace.ShowGameSpace(this);
        }
        private void ResetShipMovementFlags()
        {
            foreach (var country in GameCountries)
            {
                if (country.CountryPorts != null)
                {
                    foreach (var port in country.CountryPorts)
                    {
                        if (port.MilitaryShips != null)
                        {
                            foreach (var ship in port.MilitaryShips)
                            {
                                ship.HasMovedThisTurn = false;
                            }
                        }
                    }
                }
            }
        }
        private void CollectCivilianShipIncome()
        {
            foreach (var country in GameCountries)
            {
                if (country.CountryPorts != null)
                {
                    foreach (var port in country.CountryPorts)
                    {
                        int income = port.GetCivilianShipIncome();
                        country.CountryMoney += income;
                    }
                }
            }
        }

        private void CountMoneyOfEachCountry()
        {
            int IncomeFromCivilianFactories = 10;

            foreach (var country in GameCountries)
            {
                int totalCivilianFactories = 0;

                if (country.CountryСitys != null)
                {
                    foreach (var city in country.CountryСitys)
                    {
                        if (city.CivilianFactories != null)
                        {
                            totalCivilianFactories += city.CivilianFactories.Count;
                        }
                    }
                }

                int moneyIncome = totalCivilianFactories * IncomeFromCivilianFactories;

                country.CountryMoney += moneyIncome;
            }
        }

        private void ProduceMilitaryEquipment()
        {
            foreach (Country country in GameCountries)
            {
                if (country.CountryСitys == null || country.MilitaryAmmunitionDepot == null)
                    continue;

                foreach (City city in country.CountryСitys)
                {
                    if (city.MilitaryFactories == null)
                        continue;

                    foreach (MilitaryFactory factory in city.MilitaryFactories)
                    {
                        string product = factory.CurrentProduct;

                        if (product == "None")
                            continue;

                        if (MilitaryFactory.ProductionRates.TryGetValue(product, out int productionRate))
                        {
                            if (country.MilitaryAmmunitionDepot.ContainsKey(product))
                            {
                                country.MilitaryAmmunitionDepot[product] += productionRate;
                            }
                            else
                            {
                                country.MilitaryAmmunitionDepot[product] = productionRate;
                            }
                        }
                    }
                }
            }
        }
        private void CountPopulationOfEachCountry()
        {
            int IncomeFromPopulation = 10;

            foreach (var country in GameCountries)
            {
                int totalHouses = 0;

                if (country.CountryСitys != null)
                {
                    foreach (var city in country.CountryСitys)
                    {
                        if (city.CivilianFactories != null)
                        {
                            totalHouses += city.Houses.Count;
                        }
                    }
                }

                int PopulationIncome = totalHouses * IncomeFromPopulation;

                country.CountryPopulation += PopulationIncome;
            }
        }
        private void ResetDivisionMovementFlags()
        {
            foreach (var country in GameCountries)
            {
                if (country.CountryArmy?.Divisions != null)
                {
                    foreach (var division in country.CountryArmy.Divisions)
                    {
                        division.HasMovedThisTurn = false;
                    }
                }
            }
        }
    }
}
