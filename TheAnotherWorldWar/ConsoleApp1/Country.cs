using System;
using System.Collections.Generic;
using System.Text;
using static ConsoleApp1.City;

namespace ConsoleApp1
{
    public class Country
    {
        public bool IsBot { get; set; }
        public string CountryName { get; set; }
        public List<int> CountryTerritory { get; set; } = new List<int>();
        public List<City> CountryСitys { get; set; } = new List<City>();
        public List<Port> CountryPorts { get; set; } = new List<Port>();
        public ConsoleColor CountryColor { get; set; }
        public IdeologiesEnum CountryIdeology { get; set; }

        public int CountryPopulation { get; set; } = 100;
        public int CountryMoney { get; set; }

        public Army CountryArmy { get; set; }
        public List<Country> WarWithCountries { get; set; } = new List<Country>();
        public List<Country> Allies { get; set; } = new List<Country>();

        public Dictionary<string, int> MilitaryAmmunitionDepot { get; set; } = new Dictionary<string, int>()
        {
            [MilitaryFactory.MilitaryFactoryProducts[1]] = 0, // винтовка
            [MilitaryFactory.MilitaryFactoryProducts[2]] = 0, // танк
            [MilitaryFactory.MilitaryFactoryProducts[3]] = 0 // грузовик
        };

        public enum IdeologiesEnum
        {
            RightDemocracy,
            LeftDemocracy,
            Сommunism,
            Fascism,
            Monarchism
        }
        public class CountryInfo
        {
            public Country SelectedCountry { get; set; }
            public Country PlayerCountry { get; set; }
        }

        public static List<Country> GenerateCountries(List<char> MetaMap, int mapSize, int countriesCount)
        {
            var countries = new List<Country>();
            var availableLandCells = new List<int>();
            var claimedCells = new bool[MetaMap.Count];
            var random = new Random();

            for (int i = 0; i < MetaMap.Count; i++)
            {
                if (MetaMap[i] == Game.MapObjects["Land"] || MetaMap[i] == Game.MapObjects["Forest"] || MetaMap[i] == Game.MapObjects["Mountain"] || MetaMap[i] == Game.MapObjects["Hill"] || MetaMap[i] == Game.MapObjects["Desert"])
                {
                    availableLandCells.Add(i);
                }
            }

            if (availableLandCells.Count < countriesCount)
            {
                throw new ArgumentException($"Недостаточно сухопутных клеток ({availableLandCells.Count}) для {countriesCount} стран");
            }

            List<ConsoleColor> colors = Enum.GetValues(typeof(ConsoleColor)).Cast<ConsoleColor>().Where(c => c != ConsoleColor.Black && c != ConsoleColor.Gray && c != ConsoleColor.White).ToList();

            var usedNames = new HashSet<string>();

            for (int i = 0; i < countriesCount; i++)
            {
                IdeologiesEnum ideology = (IdeologiesEnum)random.Next(Enum.GetValues(typeof(IdeologiesEnum)).Length);

                string countryName;
                int attempts = 0;
                do
                {
                    countryName = GenerateCountryName(random, ideology);
                    attempts++;

                    if (attempts > 10)
                    {
                        countryName += $" {i + 1}";
                        break;
                    }
                }
                while (usedNames.Contains(countryName));

                usedNames.Add(countryName);

                Country country = new Country
                {
                    CountryName = countryName,
                    IsBot = i > 0,
                    CountryColor = colors[i % colors.Count],
                    CountryIdeology = ideology
                };

                countries.Add(country);
            }

            var startPoints = new List<int>();
            var takenCells = new HashSet<int>();

            for (int i = 0; i < countriesCount; i++)
            {
                int startPoint;
                int attempts = 0;

                do
                {
                    startPoint = availableLandCells[random.Next(availableLandCells.Count)];
                    attempts++;

                    if (attempts > 100)
                    {
                        startPoint = availableLandCells.FirstOrDefault(cell => !takenCells.Contains(cell));
                        if (startPoint == 0 && availableLandCells.Count > 0)
                            startPoint = availableLandCells[0];
                        break;
                    }
                }
                while (takenCells.Contains(startPoint) || !IsFarEnoughFromOthers(startPoint, startPoints, mapSize, 5));

                startPoints.Add(startPoint);
                takenCells.Add(startPoint);

                countries[i].CountryTerritory.Add(startPoint);
                claimedCells[startPoint] = true;
            }

            DistributeRemainingLand(availableLandCells, countries, claimedCells, mapSize, random);

            return countries;
        }

        private static bool IsFarEnoughFromOthers(int point, List<int> otherPoints, int mapSize, int minDistance)
        {
            if (otherPoints.Count == 0) return true;

            int pointX = point % mapSize;
            int pointY = point / mapSize;

            foreach (var otherPoint in otherPoints)
            {
                int otherX = otherPoint % mapSize;
                int otherY = otherPoint / mapSize;

                int distance = Math.Abs(pointX - otherX) + Math.Abs(pointY - otherY);
                if (distance < minDistance) return false;
            }

            return true;
        }

        private static void DistributeRemainingLand(List<int> availableLandCells, List<Country> countries,
                                                    bool[] claimedCells, int mapSize, Random random)
        {
            var unclaimedLand = new List<int>(availableLandCells.Where(cell => !claimedCells[cell]));

            while (unclaimedLand.Count > 0)
            {
                bool claimedAny = false;

                foreach (var country in countries)
                {
                    var newCells = new List<int>();

                    foreach (var cell in country.CountryTerritory)
                    {
                        int x = cell % mapSize;
                        int y = cell / mapSize;

                        int[] dx = { 0, 1, 0, -1 };
                        int[] dy = { -1, 0, 1, 0 };

                        for (int d = 0; d < 4; d++)
                        {
                            int nx = x + dx[d];
                            int ny = y + dy[d];
                            int neighborIndex = ny * mapSize + nx;

                            if (nx >= 0 && nx < mapSize && ny >= 0 && ny < mapSize &&
                                !claimedCells[neighborIndex] &&
                                availableLandCells.Contains(neighborIndex))
                            {
                                if (random.Next(100) < 70)
                                {
                                    newCells.Add(neighborIndex);
                                    claimedCells[neighborIndex] = true;
                                    claimedAny = true;
                                }
                            }
                        }
                    }

                    country.CountryTerritory.AddRange(newCells);
                }

                unclaimedLand = availableLandCells.Where(cell => !claimedCells[cell]).ToList();

                if (!claimedAny && unclaimedLand.Count > 0)
                {
                    foreach (var cell in unclaimedLand.ToList())
                    {
                        var closestCountry = FindClosestCountry(cell, countries, mapSize);
                        if (closestCountry != null)
                        {
                            closestCountry.CountryTerritory.Add(cell);
                            claimedCells[cell] = true;
                        }
                    }
                    break;
                }
            }
        }

        private static Country FindClosestCountry(int cell, List<Country> countries, int mapSize)
        {
            int cellX = cell % mapSize;
            int cellY = cell / mapSize;

            Country closestCountry = null;
            int minDistance = int.MaxValue;

            foreach (var country in countries)
            {
                foreach (var territoryCell in country.CountryTerritory)
                {
                    int territoryX = territoryCell % mapSize;
                    int territoryY = territoryCell / mapSize;

                    int distance = Math.Abs(cellX - territoryX) + Math.Abs(cellY - territoryY);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestCountry = country;
                    }
                }
            }

            return closestCountry;
        }
        public static string GenerateCountryName(Random random, IdeologiesEnum ideology)
        {
            List<string> firstSyllables = new List<string> { "Al", "Bel", "Vin", "Ger", "Dan", "Er", "Fran", "Gre", "Is", "Jap" };
            List<string> secondSyllables = new List<string> { "me", "ni", "ra", "to", "ve", "la", "sy", "co", "na", "ta" };
            List<string> thirdSyllables = new List<string> { "nia", "ria", "stan", "land", "burg", "gia", "thia", "via", "dia", "nya" };

            int syllableCount = random.Next(100) < 70 ? 2 : 3;

            string countryName = "";

            if (syllableCount == 2)
            {
                string firstSyllable = firstSyllables[random.Next(firstSyllables.Count)];
                string thirdSyllable = thirdSyllables[random.Next(thirdSyllables.Count)];
                countryName = Capitalize(firstSyllable + thirdSyllable);
            }
            else
            {
                string firstSyllable = firstSyllables[random.Next(firstSyllables.Count)];
                string secondSyllable = secondSyllables[random.Next(secondSyllables.Count)];
                string thirdSyllable = thirdSyllables[random.Next(thirdSyllables.Count)];
                countryName = Capitalize(firstSyllable + secondSyllable + thirdSyllable);
            }

            if (ideology == IdeologiesEnum.Monarchism)
            {
                string[] monarchPrefixes = { "Kingdom ", "Empire ", "Dominion ", "Principality ", "Sultanate ", "Emirate " };
                string prefix = monarchPrefixes[random.Next(monarchPrefixes.Length)];
                return prefix + countryName;
            }

            return countryName;
        }

        private static string Capitalize(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            return char.ToUpper(str[0]) + str.Substring(1);
        }
        public bool IsAtWarWith(Country otherCountry)
        {
            return WarWithCountries.Contains(otherCountry);
        }
        public void DeclareWar(Country targetCountry)
        {
            if (!IsAtWarWith(targetCountry))
            {
                WarWithCountries.Add(targetCountry);
                targetCountry.WarWithCountries.Add(this);

                if (Allies.Contains(targetCountry))
                {
                    Allies.Remove(targetCountry);
                    targetCountry.Allies.Remove(this);
                }
            }
        }
        public void DeclarePeace(Country targetCountry)
        {
            if (IsAtWarWith(targetCountry))
            {
                WarWithCountries.Remove(targetCountry);
                targetCountry.WarWithCountries.Remove(this);
            }
        }
    }
}
