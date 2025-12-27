using System;
using System.Collections.Generic;
using System.Text;
using static ConsoleApp1.City.Road;

namespace ConsoleApp1
{
    public class City
    {
        public bool IsCapital { get; set; }
        public string СityName { get; set; }
        public int СityPosition { get; set; }
        public List<char> CityMap { get; set; }

        public List<CivilianFactory> CivilianFactories { get; set; }

        public List<MilitaryFactory> MilitaryFactories { get; set; }

        public List<House> Houses { get; set; }

        public static City CreateCity(int position, int mapSize, bool isCapital, string name)
        {
            City city = new City
            {
                IsCapital = isCapital,
                СityName = name,
                СityPosition = position,
                CityMap = MapGenerator.GenerateCityMap(),
                CivilianFactories = new List<CivilianFactory>(),
                MilitaryFactories = new List<MilitaryFactory>(),
                Houses = new List<House>()
            };
            CivilianFactory.InitializeCivilianFactoryInCity(city);
            MilitaryFactory.InitializeMilitaryFactoryInCity(city);
            House.InitializeHouseInCity(city);

            return city;
        }

        public class CivilianFactory
        {
            public int CivilianFactoryPosition { get; set; }
            public static City InitializeCivilianFactoryInCity(City city)
            {
                int citySize = (int)Math.Sqrt(city.CityMap.Count);
                for (int i = 0; i < city.CityMap.Count; i++)
                {
                    if (city.CityMap[i] == SityMapObjects["CivilianFactory"])
                    {
                        CivilianFactory factory = new CivilianFactory
                        {
                            CivilianFactoryPosition = i,
                        };
                        city.CivilianFactories.Add(factory);
                    }
                }
                return city;
            }
            public static City FindCityWithFactory(Game game, CivilianFactory factory)
            {
                foreach (var country in game.GameCountries)
                {
                    foreach (var city in country.CountryСitys)
                    {
                        if (city.CivilianFactories != null && city.CivilianFactories.Contains(factory))
                        {
                            return city;
                        }
                    }
                }
                return null;
            }
            public static CivilianFactory FindCivilianFactory(City city, int cellIndex)
            {
                return city.CivilianFactories.FirstOrDefault(f => f.CivilianFactoryPosition == cellIndex);
            }
            public static void DemolishMilitaryFactory(Game game, City city, CivilianFactory factory)
            {
                if (!city.CivilianFactories.Contains(factory))
                {
                    ConsoleManager.ErrorMassege("Ошибка: фабрика не найдена в этом городе!");
                    return;
                }

                int factoryPosition = factory.CivilianFactoryPosition;

                city.CivilianFactories.Remove(factory);

                if (factoryPosition >= 0 && factoryPosition < city.CityMap.Count)
                {
                    city.CityMap[factoryPosition] = SityMapObjects["Land"];
                }

                Console.Clear();
                GameSpace.ShowCityMap(city, game);
            }
        }
        public class MilitaryFactory
        {
            public int MilitaryFactoryPosition { get; set; }
            public string CurrentProduct { get; set; }

            public static List<string> MilitaryFactoryProducts = new List<string>()
            {
                "None",
                "Rifle",
                "Tank",
                "Truck"
            };

            public static Dictionary<string, int> ProductionRates = new Dictionary<string, int>()
            {
                [MilitaryFactoryProducts[0]] = 0,
                [MilitaryFactoryProducts[1]] = 100,
                [MilitaryFactoryProducts[2]] = 1,
                [MilitaryFactoryProducts[3]] = 10
            };

            public static City InitializeMilitaryFactoryInCity(City city)
            {
                int citySize = (int)Math.Sqrt(city.CityMap.Count);
                for (int i = 0; i < city.CityMap.Count; i++)
                {
                    if (city.CityMap[i] == SityMapObjects["MilitaryFactory"])
                    {
                        MilitaryFactory factory = new MilitaryFactory
                        {
                            MilitaryFactoryPosition = i,
                            CurrentProduct = MilitaryFactoryProducts.FirstOrDefault() ?? "None"
                        };
                        city.MilitaryFactories.Add(factory);
                    }
                }
                return city;
            }

            public static City FindCityWithFactory(Game game, MilitaryFactory factory)
            {
                foreach (var country in game.GameCountries)
                {
                    foreach (var city in country.CountryСitys)
                    {
                        if (city.MilitaryFactories != null && city.MilitaryFactories.Contains(factory))
                        {
                            return city;
                        }
                    }
                }
                return null;
            }

            public static MilitaryFactory FindMilitaryFactory(City city, int cellIndex)
            {
                return city.MilitaryFactories.FirstOrDefault(f => f.MilitaryFactoryPosition == cellIndex);
            }

            public static void DemolishMilitaryFactory(Game game, City city, MilitaryFactory factory)
            {
                int factoryPosition = factory.MilitaryFactoryPosition;

                city.MilitaryFactories.Remove(factory);

                if (factoryPosition >= 0 && factoryPosition < city.CityMap.Count)
                {
                    city.CityMap[factoryPosition] = SityMapObjects["Land"];
                }

                Console.Clear();
                GameSpace.ShowCityMap(city, game);
            }
        }

        public class House
        {
            public int HousePosition { get; set; }

            public static City InitializeHouseInCity(City city)
            {
                int citySize = (int)Math.Sqrt(city.CityMap.Count);
                for (int i = 0; i < city.CityMap.Count; i++)
                {
                    if (city.CityMap[i] == SityMapObjects["House"])
                    {
                        House house = new House
                        {
                            HousePosition = i,
                        };
                        city.Houses.Add(house);
                    }
                }
                return city;
            }

            public static City FindCityWithHouse(Game game, House house)
            {
                foreach (var country in game.GameCountries)
                {
                    foreach (var city in country.CountryСitys)
                    {
                        if (city.Houses != null && city.Houses.Contains(house))
                        {
                            return city;
                        }
                    }
                }
                return null;
            }

            public static House FindHouse(City city, int cellIndex)
            {
                return city.Houses.FirstOrDefault(f => f.HousePosition == cellIndex);
            }

            public static void DemolishHouse(Game game, City city, House house)
            {
                int housePosition = house.HousePosition;

                city.Houses.Remove(house);

                if (housePosition >= 0 && housePosition < city.CityMap.Count)
                {
                    city.CityMap[housePosition] = SityMapObjects["Land"];
                }

                Console.Clear();
                GameSpace.ShowCityMap(city, game);
            }
        }

        public static class Road
        {
            public static bool IsRoadCell(char cell)
            {
                return cell == SityMapObjects["VerticalRoad"] ||
                       cell == SityMapObjects["HorizontalRoad"] ||
                       cell == SityMapObjects["Crossroad"];
            }

            public static string GetRoadType(char cell)
            {
                return cell switch
                {
                    '|' => "VerticalRoad",
                    '─' => "HorizontalRoad",
                    '+' => "Crossroad",
                    _ => "NotARoad"
                };
            }

            public static void DemolishRoad(Game game, City city, int position)
            {
                char originalRoadType = city.CityMap[position];

                city.CityMap[position] = SityMapObjects["Land"];

                FixRoadConnectionsAfterDemolition(city, position);

                Console.Clear();
                GameSpace.ShowCityMap(city, game);
            }

            private static void FixRoadConnectionsAfterDemolition(City city, int demolishedPosition)
            {
                int citySize = (int)Math.Sqrt(city.CityMap.Count);
                int x = demolishedPosition % citySize;
                int y = demolishedPosition / citySize;
            }
            public class RoadInfo
            {
                public City City { get; set; }
                public int Position { get; set; }

                public RoadInfo(City city, int position)
                {
                    City = city;
                    Position = position;
                }
            }
        }

        public static Dictionary<string, char> SityMapObjects = new Dictionary<string, char>()
        {
            ["Border"] = '#',
            ["Land"] = '0',
            ["Tree"] = 'T',

            ["VerticalRoad"] = '|',
            ["HorizontalRoad"] = '─',
            ["Crossroad"] = '+',

            ["CivilianFactory"] = 'C',
            ["MilitaryFactory"] = 'M',

            ["House"] = 'H',
        };

        public int GetX(int mapSize) => СityPosition % mapSize;
        public int GetY(int mapSize) => СityPosition / mapSize;
        public static class BuildingSystem
        {
            public static Dictionary<string, int> BuildingCosts = new Dictionary<string, int>()
            {
                ["MilitaryFactory"] = 1000,
                ["CivilianFactory"] = 500,
                ["House"] = 250,
                ["Road"] = 50
            };

            public static List<string> BuildableStructures = new List<string>()
            {
                "MilitaryFactory",
                "CivilianFactory",
                "House",
                "Road"
            };

            public static List<string> RoadTypes = new List<string>()
            {
                "VerticalRoad",
                "HorizontalRoad",
                "Crossroad"
            };

            public static bool CanBuildAtPosition(City city, int position, Game game, string buildingType)
            {
                int citySize = (int)Math.Sqrt(city.CityMap.Count);
                int x = position % citySize;
                int y = position / citySize;

                if (x <= 0 || x >= citySize - 1 || y <= 0 || y >= citySize - 1)
                    return false;

                if (city.CityMap[position] != SityMapObjects["Land"])
                    return false;

                if (buildingType == "Road")
                {
                    return CanBuildRoadAt(city, position);
                }

                return HasRoadNearby(city, x, y, citySize);
            }

            private static bool HasRoadNearby(City city, int x, int y, int citySize)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        if (dx == 0 && dy == 0) continue;

                        int nx = x + dx;
                        int ny = y + dy;

                        if (nx >= 0 && nx < citySize && ny >= 0 && ny < citySize)
                        {
                            int neighborIndex = ny * citySize + nx;
                            char neighborCell = city.CityMap[neighborIndex];

                            if (neighborCell == SityMapObjects["VerticalRoad"] ||
                                neighborCell == SityMapObjects["HorizontalRoad"] ||
                                neighborCell == SityMapObjects["Crossroad"])
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }

            private static bool CanBuildRoadAt(City city, int position)
            {
                int citySize = (int)Math.Sqrt(city.CityMap.Count);
                int x = position % citySize;
                int y = position / citySize;

                char currentCell = city.CityMap[position];
                return currentCell == SityMapObjects["Land"] ||
                       currentCell == SityMapObjects["VerticalRoad"] ||
                       currentCell == SityMapObjects["HorizontalRoad"] ||
                       currentCell == SityMapObjects["Crossroad"];
            }

            public static bool BuildStructure(City city, int position, Game game, string buildingType, Country playerCountry, string roadType = null)
            {
                int cost = BuildingCosts[buildingType];
                if (playerCountry.CountryMoney < cost)
                {
                    ConsoleManager.ErrorMassege($"Not enough money! Need: ${cost}, Have: ${playerCountry.CountryMoney}");
                    return false;
                }

                if (!CanBuildAtPosition(city, position, game, buildingType))
                {
                    ConsoleManager.ErrorMassege("Cannot build here! Check requirements.");
                    return false;
                }

                playerCountry.CountryMoney -= cost;

                if (buildingType == "Road")
                {
                    return BuildRoad(city, position, roadType);
                }
                else
                {
                    return BuildBuilding(city, position, buildingType);
                }
            }

            private static bool BuildBuilding(City city, int position, string buildingType)
            {
                char buildingChar = buildingType switch
                {
                    "MilitaryFactory" => SityMapObjects["MilitaryFactory"],
                    "CivilianFactory" => SityMapObjects["CivilianFactory"],
                    "House" => SityMapObjects["House"],
                    _ => SityMapObjects["Land"]
                };

                city.CityMap[position] = buildingChar;

                switch (buildingType)
                {
                    case "MilitaryFactory":
                        var militaryFactory = new MilitaryFactory
                        {
                            MilitaryFactoryPosition = position,
                            CurrentProduct = MilitaryFactory.MilitaryFactoryProducts.FirstOrDefault() ?? "None"
                        };
                        city.MilitaryFactories.Add(militaryFactory);
                        break;

                    case "CivilianFactory":
                        var civilianFactory = new CivilianFactory
                        {
                            CivilianFactoryPosition = position
                        };
                        city.CivilianFactories.Add(civilianFactory);
                        break;

                    case "House":
                        var house = new House
                        {
                            HousePosition = position
                        };
                        city.Houses.Add(house);
                        break;
                }

                if (buildingType != "Road")
                {
                    UpdateRoadConnections(city, position);
                }

                return true;
            }
            private static bool BuildRoad(City city, int position, string roadType)
            {
                char roadChar = roadType switch
                {
                    "VerticalRoad" => SityMapObjects["VerticalRoad"],
                    "HorizontalRoad" => SityMapObjects["HorizontalRoad"],
                    "Crossroad" => SityMapObjects["Crossroad"],
                    _ => SityMapObjects["HorizontalRoad"]
                };

                city.CityMap[position] = roadChar;

                UpdateRoadConnections(city, position);

                return true;
            }

            private static void UpdateRoadConnections(City city, int position)
            {
                int citySize = (int)Math.Sqrt(city.CityMap.Count);
                int x = position % citySize;
                int y = position / citySize;

                FixRoadTypeAtPosition(city, position);

                int[] dx = { 0, 1, 0, -1 };
                int[] dy = { -1, 0, 1, 0 };

                for (int i = 0; i < 4; i++)
                {
                    int nx = x + dx[i];
                    int ny = y + dy[i];

                    if (nx >= 0 && nx < citySize && ny >= 0 && ny < citySize)
                    {
                        int neighborIndex = ny * citySize + nx;
                        if (IsRoadCell(city.CityMap[neighborIndex]))
                        {
                            FixRoadTypeAtPosition(city, neighborIndex);
                        }
                    }
                }
            }
            private static void FixRoadTypeAtPosition(City city, int position)
            {
                int citySize = (int)Math.Sqrt(city.CityMap.Count);
                int x = position % citySize;
                int y = position / citySize;
                char current = city.CityMap[position];

                if (!IsRoadCell(current))
                    return;

                bool hasVertical = false;
                bool hasHorizontal = false;

                if (y > 0 && (city.CityMap[(y - 1) * citySize + x] == SityMapObjects["VerticalRoad"] ||
                             city.CityMap[(y - 1) * citySize + x] == SityMapObjects["Crossroad"]))
                    hasVertical = true;
                if (y < citySize - 1 && (city.CityMap[(y + 1) * citySize + x] == SityMapObjects["VerticalRoad"] ||
                                        city.CityMap[(y + 1) * citySize + x] == SityMapObjects["Crossroad"]))
                    hasVertical = true;

                if (x > 0 && (city.CityMap[y * citySize + (x - 1)] == SityMapObjects["HorizontalRoad"] ||
                             city.CityMap[y * citySize + (x - 1)] == SityMapObjects["Crossroad"]))
                    hasHorizontal = true;
                if (x < citySize - 1 && (city.CityMap[y * citySize + x + 1] == SityMapObjects["HorizontalRoad"] ||
                                        city.CityMap[y * citySize + x + 1] == SityMapObjects["Crossroad"]))
                    hasHorizontal = true;

                if (hasVertical && hasHorizontal)
                    city.CityMap[position] = SityMapObjects["Crossroad"];
                else if (hasVertical)
                    city.CityMap[position] = SityMapObjects["VerticalRoad"];
                else if (hasHorizontal)
                    city.CityMap[position] = SityMapObjects["HorizontalRoad"];
                else if (current == SityMapObjects["Crossroad"])
                    city.CityMap[position] = SityMapObjects["HorizontalRoad"];
            }
            public class BuildInfo
            {
                public City City { get; set; }
                public int Position { get; set; }

                public BuildInfo(City city, int position)
                {
                    City = city;
                    Position = position;
                }
            }
        }
        public static Country FindCountryForCity(City city, List<Country> countries)
        {
            foreach (var country in countries)
            {
                if (country.CountryСitys != null && country.CountryСitys.Contains(city))
                {
                    return country;
                }
            }
            return null;
        }
    }
}
