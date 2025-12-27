using System;
using System.Collections.Generic;
using System.Text;
using static ConsoleApp1.Country;
using static ConsoleApp1.City;

namespace ConsoleApp1
{
    public class MapGenerator
    {
        private List<char> Map = new List<char>();
        private int mapSize;

        public List<char> GenerateMap(int size)
        {
            mapSize = size;
            Map.Clear();

            InitializeMapWithBorders();

            GenerateLandscape();

            GenerateForests();

            GenerateMountains();

            GenerateDeserts();

            EnsureLandIsWithinBounds();

            return Map;
        }

        private void InitializeMapWithBorders()
        {
            for (int y = 0; y < mapSize; y++)
            {
                for (int x = 0; x < mapSize; x++)
                {
                    if (x == 0 || y == 0 || x == mapSize - 1 || y == mapSize - 1)
                    {
                        Map.Add(Game.MapObjects["Border"]);
                    }
                    else
                    {
                        Map.Add(Game.MapObjects["Sea"]);
                    }
                }
            }
        }
        private void GenerateLandscape()
        {

            Random random = new Random();

            float scale = (float)(random.NextDouble() * 0.1 + 0.1);

            float threshold = (float)(random.NextDouble() * 4 + 3);

            float offsetX = (float)random.NextDouble() * 100f;
            float offsetY = (float)random.NextDouble() * 100f;

            int borderBuffer = 2;
            int minX = borderBuffer;
            int maxX = mapSize - borderBuffer - 1;
            int minY = borderBuffer;
            int maxY = mapSize - borderBuffer - 1;

            for (int y = 1; y < mapSize - 1; y++)
            {
                for (int x = 1; x < mapSize - 1; x++)
                {
                    int index = y * mapSize + x;

                    if (x <= borderBuffer || y <= borderBuffer ||
                        x >= mapSize - borderBuffer - 1 || y >= mapSize - borderBuffer - 1)
                    {
                        Map[index] = Game.MapObjects["Sea"];
                        continue;
                    }

                    float noiseValue = PerlinNoise(
                        (x + offsetX) * scale,
                        (y + offsetY) * scale
                    );

                    float centerX = mapSize / 2f;
                    float centerY = mapSize / 2f;
                    float distanceToCenter = (float)Math.Sqrt(
                        Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2)
                    );
                    float maxDistance = mapSize / 2f - borderBuffer;
                    float radialFactor = 1.0f - distanceToCenter / maxDistance;

                    float adjustedThreshold = threshold * Math.Max(0.1f, radialFactor * 0.8f);

                    Map[index] = noiseValue > adjustedThreshold ? Game.MapObjects["Land"] : Game.MapObjects["Sea"];
                }
            }
        }
        private void EnsureLandIsWithinBounds()
        {
            int borderBuffer = 2;

            for (int y = 1; y < mapSize - 1; y++)
            {
                for (int x = 1; x < mapSize - 1; x++)
                {
                    int index = y * mapSize + x;

                    if (x <= borderBuffer || y <= borderBuffer ||
                        x >= mapSize - borderBuffer - 1 || y >= mapSize - borderBuffer - 1)
                    {
                        if (Map[index] == Game.MapObjects["Land"])
                        {
                            Map[index] = Game.MapObjects["Sea"];
                        }

                        for (int dy = -1; dy <= 1; dy++)
                        {
                            for (int dx = -1; dx <= 1; dx++)
                            {
                                int nx = x + dx;
                                int ny = y + dy;

                                if (nx >= 0 && nx < mapSize && ny >= 0 && ny < mapSize)
                                {
                                    int neighborIndex = ny * mapSize + nx;
                                    if (Map[neighborIndex] == Game.MapObjects["Land"])
                                    {
                                        Map[neighborIndex] = Game.MapObjects["Sea"];
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private float PerlinNoise(float x, float y)
        {
            int xi = (int)x;
            int yi = (int)y;

            float xf = x - xi;
            float yf = y - yi;

            float v1 = RandomGradient(xi, yi, xf, yf);
            float v2 = RandomGradient(xi + 1, yi, xf - 1, yf);
            float v3 = RandomGradient(xi, yi + 1, xf, yf - 1);
            float v4 = RandomGradient(xi + 1, yi + 1, xf - 1, yf - 1);

            float i1 = Interpolate(v1, v2, xf);
            float i2 = Interpolate(v3, v4, xf);

            return Interpolate(i1, i2, yf);
        }
        private float RandomGradient(int ix, int iy, float x, float y)
        {
            float random = 2920f * (float)Math.Sin(ix * 21942f + iy * 171324f + 8912f) *
                          (float)Math.Cos(ix * 23157f * iy * 217832f + 9758f);

            return (float)(random * (x + y));
        }
        private float Interpolate(float a, float b, float t)
        {
            t = t * t * (3 - 2 * t);
            return a + (b - a) * t;
        }
        public static void PlaceCapitals(List<Country> countries, List<char> geoMap, int mapSize)
        {
            var random = new Random();

            foreach (var country in countries)
            {
                if (country.CountryTerritory.Count == 0)
                {
                    continue;
                }

                if (!country.IsBot && country.CountryСitys.Any(c => c.IsCapital))
                {
                    var existingCapital = country.CountryСitys.First(c => c.IsCapital);

                    if (country.CountryTerritory.Contains(existingCapital.СityPosition))
                    {
                        continue;
                    }
                }

                int capitalPosition = FindBestCapitalPosition(country, geoMap, mapSize, random);

                var capital = CreateCity(
                    capitalPosition,
                    mapSize,
                    true,
                    GetCapitalName(country)
                );

                if (country.CountryСitys == null)
                {
                    country.CountryСitys = new List<City>();
                }

                country.CountryСitys.Add(capital);

                UpdateMapWithCapital(geoMap, capitalPosition, mapSize);
            }
        }
        private static int FindBestCapitalPosition(Country country, List<char> geoMap, int mapSize, Random random)
        {
            var suitablePositions = new List<int>();
            var goodPositions = new List<int>();

            foreach (var cell in country.CountryTerritory)
            {
                int x = cell % mapSize;
                int y = cell / mapSize;

                if (x < 2 || x >= mapSize - 2 || y < 2 || y >= mapSize - 2)
                {
                    continue;
                }

                if (geoMap[cell] != Game.MapObjects["Land"])
                {
                    continue;
                }

                bool hasGoodNeighbors = CheckNeighborsForCapital(cell, geoMap, mapSize);

                if (hasGoodNeighbors)
                {
                    goodPositions.Add(cell);
                }
                else
                {
                    suitablePositions.Add(cell);
                }
            }

            if (goodPositions.Count > 0)
            {
                return GetCentralPosition(goodPositions, mapSize, random);
            }
            else if (suitablePositions.Count > 0)
            {
                return GetCentralPosition(suitablePositions, mapSize, random);
            }
            else
            {
                return country.CountryTerritory[random.Next(country.CountryTerritory.Count)];
            }
        }
        private static bool CheckNeighborsForCapital(int cell, List<char> geoMap, int mapSize)
        {
            int x = cell % mapSize;
            int y = cell / mapSize;

            int landNeighbors = 0;
            int waterNeighbors = 0;

            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (dx == 0 && dy == 0) continue;

                    int nx = x + dx;
                    int ny = y + dy;

                    if (nx >= 0 && nx < mapSize && ny >= 0 && ny < mapSize)
                    {
                        int neighborIndex = ny * mapSize + nx;
                        char neighborCell = geoMap[neighborIndex];

                        if (neighborCell == Game.MapObjects["Land"])
                        {
                            landNeighbors++;
                        }
                        else if (neighborCell == Game.MapObjects["Sea"])
                        {
                            waterNeighbors++;
                        }
                    }
                }
            }
            return landNeighbors >= 4;
        }
        private static int GetCentralPosition(List<int> positions, int mapSize, Random random)
        {
            if (positions.Count == 0) return -1;

            int centerX = 0;
            int centerY = 0;

            foreach (var pos in positions)
            {
                centerX += pos % mapSize;
                centerY += pos / mapSize;
            }

            centerX /= positions.Count;
            centerY /= positions.Count;

            int bestPosition = positions[0];
            int minDistance = int.MaxValue;

            foreach (var pos in positions)
            {
                int x = pos % mapSize;
                int y = pos / mapSize;

                int distance = Math.Abs(x - centerX) + Math.Abs(y - centerY);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    bestPosition = pos;
                }
                else if (distance == minDistance)
                {
                    if (random.Next(2) == 0)
                    {
                        bestPosition = pos;
                    }
                }
            }

            return bestPosition;
        }
        private static string GetCapitalName(Country country)
        {
            var random = new Random(country.CountryName.GetHashCode());

            return GenerateCapitalName(random, country.CountryIdeology);
        }
        private static string GenerateCapitalName(Random random, IdeologiesEnum ideology)
        {
            List<string> prefixes = new List<string>
            {
                "Санкт-", "Нью-", "Велико", "Старо", "Ново", "Северо", "Южно", "Западно", "Восточно"
            };

            List<string> roots = new List<string>
            {
                "град", "бург", "сити", "поль", "город", "дар", "мир", "слав"
            };

            List<string> suffixes = new List<string>
            {
                "ск", "ин", "ов", "ев", "ен", "ан", "ун"
            };

            if (ideology == IdeologiesEnum.Monarchism)
            {
                roots.AddRange(new[] { "король", "император", "царь", "королев" });
            }

            string prefix = random.Next(3) == 0 ? prefixes[random.Next(prefixes.Count)] : "";
            string root = roots[random.Next(roots.Count)];
            string suffix = random.Next(2) == 0 ? suffixes[random.Next(suffixes.Count)] : "";

            string name = prefix + root + suffix;
            return char.ToUpper(name[0]) + name.Substring(1);
        }
        private static void UpdateMapWithCapital(List<char> geoMap, int position, int mapSize)
        {
            geoMap[position] = Game.MapObjects["Capital"];
        }
        public static List<char> GenerateCityMap()
        {
            int citySize = 20;
            var cityMap = new List<char>(new char[citySize * citySize]);
            var random = new Random();

            for (int i = 0; i < cityMap.Count; i++)
            {
                cityMap[i] = SityMapObjects["Land"];
            }

            for (int y = 0; y < citySize; y++)
            {
                for (int x = 0; x < citySize; x++)
                {
                    if (x == 0 || y == 0 || x == citySize - 1 || y == citySize - 1)
                    {
                        cityMap[y * citySize + x] = SityMapObjects["Border"];
                    }
                }
            }

            CreateMainRoads(cityMap, citySize, random);

            CreateDistrictRoads(cityMap, citySize, random);

            var buildingPositions = new List<(int x, int y, char type)>();

            var civilianDistrict = GetDistrictCoordinates(1, citySize / 2 - 1, 1, citySize / 2 - 1, citySize);
            var militaryDistrict = GetDistrictCoordinates(citySize / 2, citySize - 2, 1, citySize / 2 - 1, citySize);
            var residentialDistrict1 = GetDistrictCoordinates(1, citySize / 2 - 1, citySize / 2, citySize - 2, citySize);
            var residentialDistrict2 = GetDistrictCoordinates(citySize / 2, citySize - 2, citySize / 2, citySize - 2, citySize);

            AddIntersections(cityMap, citySize);

            FixRoadConnections(cityMap, citySize);

            int civilianFactories = random.Next(2, 5);
            for (int i = 0; i < civilianFactories; i++)
            {
                var pos = FindBuildingPosition(cityMap, citySize, random, civilianDistrict,
                                              SityMapObjects["CivilianFactory"]);
                if (pos.x != -1)
                {
                    buildingPositions.Add((pos.x, pos.y, SityMapObjects["CivilianFactory"]));
                    cityMap[pos.y * citySize + pos.x] = SityMapObjects["CivilianFactory"];

                    ConnectToNearestRoad(cityMap, pos.x, pos.y, citySize, random);
                }
            }

            int militaryFactories = random.Next(1, 2);
            for (int i = 0; i < militaryFactories; i++)
            {
                var pos = FindBuildingPosition(cityMap, citySize, random, militaryDistrict,
                                              SityMapObjects["MilitaryFactory"]);
                if (pos.x != -1)
                {
                    buildingPositions.Add((pos.x, pos.y, SityMapObjects["MilitaryFactory"]));
                    cityMap[pos.y * citySize + pos.x] = SityMapObjects["MilitaryFactory"];

                    ConnectToNearestRoad(cityMap, pos.x, pos.y, citySize, random);
                }
            }

            int houses = random.Next(5, 11);
            int housesInDistrict1 = houses / 2;
            int housesInDistrict2 = houses - housesInDistrict1;

            for (int i = 0; i < housesInDistrict1; i++)
            {
                var pos = FindBuildingPosition(cityMap, citySize, random, residentialDistrict1,
                                              SityMapObjects["House"]);
                if (pos.x != -1)
                {
                    buildingPositions.Add((pos.x, pos.y, SityMapObjects["House"]));
                    cityMap[pos.y * citySize + pos.x] = SityMapObjects["House"];

                    ConnectToNearestRoad(cityMap, pos.x, pos.y, citySize, random);
                }
            }

            for (int i = 0; i < housesInDistrict2; i++)
            {
                var pos = FindBuildingPosition(cityMap, citySize, random, residentialDistrict2,
                                              SityMapObjects["House"]);
                if (pos.x != -1)
                {
                    buildingPositions.Add((pos.x, pos.y, SityMapObjects["House"]));
                    cityMap[pos.y * citySize + pos.x] = SityMapObjects["House"];

                    ConnectToNearestRoad(cityMap, pos.x, pos.y, citySize, random);
                }
            }

            AddRareTreesToCity(cityMap, citySize, random);

            return cityMap;
        }
        private static void CreateMainRoads(List<char> cityMap, int citySize, Random random)
        {
            int verticalRoadsCount = random.Next(1, 3);
            int horizontalRoadsCount = random.Next(1, 3);

            var verticalRoads = new List<int>();
            var horizontalRoads = new List<int>();

            for (int i = 0; i < verticalRoadsCount; i++)
            {
                int roadX;
                do
                {
                    roadX = random.Next(2, citySize - 2);
                } while (verticalRoads.Any(x => Math.Abs(x - roadX) < 3));

                verticalRoads.Add(roadX);

                for (int y = 1; y < citySize - 1; y++)
                {
                    int index = y * citySize + roadX;
                    if (cityMap[index] == SityMapObjects["Land"])
                    {
                        cityMap[index] = SityMapObjects["VerticalRoad"];
                    }
                }
            }

            for (int i = 0; i < horizontalRoadsCount; i++)
            {
                int roadY;
                do
                {
                    roadY = random.Next(2, citySize - 2);
                } while (horizontalRoads.Any(y => Math.Abs(y - roadY) < 3));

                horizontalRoads.Add(roadY);

                for (int x = 1; x < citySize - 1; x++)
                {
                    int index = roadY * citySize + x;
                    if (cityMap[index] == SityMapObjects["Land"])
                    {
                        cityMap[index] = SityMapObjects["HorizontalRoad"];
                    }
                }
            }

            foreach (int vRoad in verticalRoads)
            {
                foreach (int hRoad in horizontalRoads)
                {
                    int index = hRoad * citySize + vRoad;
                    cityMap[index] = '+';
                }
            }
        }
        private static void CreateDistrictRoads(List<char> cityMap, int citySize, Random random)
        {
            int districtRoadsCount = random.Next(4, 8);

            for (int i = 0; i < districtRoadsCount; i++)
            {
                bool isVertical = random.Next(2) == 0;

                if (isVertical)
                {
                    int roadX = random.Next(2, citySize - 2);
                    int startY = random.Next(2, citySize - 5);
                    int endY = startY + random.Next(3, 8);

                    for (int y = startY; y < Math.Min(endY, citySize - 2); y++)
                    {
                        int index = y * citySize + roadX;
                        if (cityMap[index] == SityMapObjects["Land"])
                        {
                            cityMap[index] = SityMapObjects["VerticalRoad"];
                        }
                    }
                }
                else
                {
                    int roadY = random.Next(2, citySize - 2);
                    int startX = random.Next(2, citySize - 5);
                    int endX = startX + random.Next(3, 8);

                    for (int x = startX; x < Math.Min(endX, citySize - 2); x++)
                    {
                        int index = roadY * citySize + x;
                        if (cityMap[index] == SityMapObjects["Land"])
                        {
                            cityMap[index] = SityMapObjects["HorizontalRoad"];
                        }
                    }
                }
            }
        }
        private static List<int> GetDistrictCoordinates(int startX, int endX, int startY, int endY, int citySize)
        {
            var coordinates = new List<int>();

            for (int y = startY; y <= endY; y++)
            {
                for (int x = startX; x <= endX; x++)
                {
                    coordinates.Add(y * citySize + x);
                }
            }

            return coordinates;
        }
        private static (int x, int y) FindBuildingPosition(List<char> cityMap, int citySize, Random random,
                                                  List<int> district, char buildingType)
        {
            List<int> suitablePositions = new List<int>();

            foreach (int index in district)
            {
                int x = index % citySize;
                int y = index / citySize;

                if (cityMap[index] != SityMapObjects["Land"])
                    continue;

                bool tooClose = false;
                int minDistance = buildingType == SityMapObjects["House"] ? 1 : 2;

                for (int dy = -minDistance; dy <= minDistance; dy++)
                {
                    for (int dx = -minDistance; dx <= minDistance; dx++)
                    {
                        int nx = x + dx;
                        int ny = y + dy;

                        if (nx >= 0 && nx < citySize && ny >= 0 && ny < citySize)
                        {
                            char cell = cityMap[ny * citySize + nx];
                            if (cell == buildingType)
                            {
                                tooClose = true;
                                break;
                            }
                        }
                    }
                    if (tooClose) break;
                }

                if (!tooClose)
                {
                    suitablePositions.Add(index);
                }
            }

            if (suitablePositions.Count > 0)
            {
                int chosenIndex = suitablePositions[random.Next(suitablePositions.Count)];
                return (chosenIndex % citySize, chosenIndex / citySize);
            }

            return FindBuildingPositionFallback(cityMap, citySize, random, buildingType);
        }

        private static (int x, int y) FindBuildingPositionFallback(List<char> cityMap, int citySize, Random random, char buildingType)
        {
            List<int> possiblePositions = new List<int>();

            for (int y = 1; y < citySize - 1; y++)
            {
                for (int x = 1; x < citySize - 1; x++)
                {
                    int index = y * citySize + x;
                    if (cityMap[index] == SityMapObjects["Land"])
                    {
                        possiblePositions.Add(index);
                    }
                }
            }

            if (possiblePositions.Count > 0)
            {
                int chosenIndex = possiblePositions[random.Next(possiblePositions.Count)];
                return (chosenIndex % citySize, chosenIndex / citySize);
            }

            return (-1, -1);
        }

        private static void ConnectToNearestRoad(List<char> cityMap, int bx, int by, int citySize, Random random)
        {
            int nearestRoadX = -1, nearestRoadY = -1;
            int minDistance = int.MaxValue;

            for (int y = 1; y < citySize - 1; y++)
            {
                for (int x = 1; x < citySize - 1; x++)
                {
                    char cell = cityMap[y * citySize + x];
                    if (cell == SityMapObjects["VerticalRoad"] ||
                        cell == SityMapObjects["HorizontalRoad"] ||
                        cell == '+')
                    {
                        int distance = Math.Abs(x - bx) + Math.Abs(y - by);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            nearestRoadX = x;
                            nearestRoadY = y;
                        }
                    }
                }
            }

            if (nearestRoadX != -1 && nearestRoadY != -1)
            {
                bool horizontalFirst = random.Next(2) == 0;

                if (horizontalFirst)
                {
                    int startX = Math.Min(bx, nearestRoadX);
                    int endX = Math.Max(bx, nearestRoadX);
                    for (int x = startX; x <= endX; x++)
                    {
                        int index = by * citySize + x;
                        if (cityMap[index] == SityMapObjects["Land"])
                        {
                            cityMap[index] = SityMapObjects["HorizontalRoad"];
                        }
                    }

                    int startY = Math.Min(by, nearestRoadY);
                    int endY = Math.Max(by, nearestRoadY);
                    for (int y = startY; y <= endY; y++)
                    {
                        int index = y * citySize + nearestRoadX;
                        if (cityMap[index] == SityMapObjects["Land"])
                        {
                            cityMap[index] = SityMapObjects["VerticalRoad"];
                        }
                    }
                }
                else
                {
                    int startY = Math.Min(by, nearestRoadY);
                    int endY = Math.Max(by, nearestRoadY);
                    for (int y = startY; y <= endY; y++)
                    {
                        int index = y * citySize + bx;
                        if (cityMap[index] == SityMapObjects["Land"])
                        {
                            cityMap[index] = SityMapObjects["VerticalRoad"];
                        }
                    }

                    int startX = Math.Min(bx, nearestRoadX);
                    int endX = Math.Max(bx, nearestRoadX);
                    for (int x = startX; x <= endX; x++)
                    {
                        int index = nearestRoadY * citySize + x;
                        if (cityMap[index] == SityMapObjects["Land"])
                        {
                            cityMap[index] = SityMapObjects["HorizontalRoad"];
                        }
                    }
                }
            }
        }
        private static void AddIntersections(List<char> cityMap, int citySize)
        {
            for (int y = 1; y < citySize - 1; y++)
            {
                for (int x = 1; x < citySize - 1; x++)
                {
                    char current = cityMap[y * citySize + x];

                    if (current == SityMapObjects["VerticalRoad"] ||
                        current == SityMapObjects["HorizontalRoad"])
                    {
                        bool hasVerticalNeighbor = false;
                        bool hasHorizontalNeighbor = false;

                        if (y > 0 && (cityMap[(y - 1) * citySize + x] == SityMapObjects["VerticalRoad"] ||
                                     cityMap[(y - 1) * citySize + x] == '+'))
                            hasVerticalNeighbor = true;

                        if (y < citySize - 1 && (cityMap[(y + 1) * citySize + x] == SityMapObjects["VerticalRoad"] ||
                                                cityMap[(y + 1) * citySize + x] == '+'))
                            hasVerticalNeighbor = true;

                        if (x > 0 && (cityMap[y * citySize + (x - 1)] == SityMapObjects["HorizontalRoad"] ||
                                     cityMap[y * citySize + (x - 1)] == '+'))
                            hasHorizontalNeighbor = true;

                        if (x < citySize - 1 && (cityMap[y * citySize + x + 1] == SityMapObjects["HorizontalRoad"] ||
                                                cityMap[y * citySize + x + 1] == '+'))
                            hasHorizontalNeighbor = true;

                        if (hasVerticalNeighbor && hasHorizontalNeighbor)
                        {
                            cityMap[y * citySize + x] = '+';
                        }
                    }
                }
            }
        }
        private static void FixRoadConnections(List<char> cityMap, int citySize)
        {
            for (int y = 1; y < citySize - 1; y++)
            {
                for (int x = 1; x < citySize - 1; x++)
                {
                    char current = cityMap[y * citySize + x];

                    if (current == SityMapObjects["VerticalRoad"])
                    {
                        bool hasVerticalConnection = false;
                        bool hasHorizontalConnection = false;

                        if (y > 0 && (cityMap[(y - 1) * citySize + x] == SityMapObjects["VerticalRoad"] ||
                                     cityMap[(y - 1) * citySize + x] == '+'))
                            hasVerticalConnection = true;

                        if (y < citySize - 1 && (cityMap[(y + 1) * citySize + x] == SityMapObjects["VerticalRoad"] ||
                                                cityMap[(y + 1) * citySize + x] == '+'))
                            hasVerticalConnection = true;

                        if (x > 0 && (cityMap[y * citySize + (x - 1)] == SityMapObjects["HorizontalRoad"] ||
                                     cityMap[y * citySize + (x - 1)] == '+'))
                            hasHorizontalConnection = true;

                        if (x < citySize - 1 && (cityMap[y * citySize + x + 1] == SityMapObjects["HorizontalRoad"] ||
                                                cityMap[y * citySize + x + 1] == '+'))
                            hasHorizontalConnection = true;

                        if (hasHorizontalConnection && !hasVerticalConnection)
                        {
                            cityMap[y * citySize + x] = SityMapObjects["HorizontalRoad"];
                        }
                    }

                    else if (current == SityMapObjects["HorizontalRoad"])
                    {
                        bool hasVerticalConnection = false;
                        bool hasHorizontalConnection = false;

                        if (y > 0 && (cityMap[(y - 1) * citySize + x] == SityMapObjects["VerticalRoad"] ||
                                     cityMap[(y - 1) * citySize + x] == '+'))
                            hasVerticalConnection = true;

                        if (y < citySize - 1 && (cityMap[(y + 1) * citySize + x] == SityMapObjects["VerticalRoad"] ||
                                                cityMap[(y + 1) * citySize + x] == '+'))
                            hasVerticalConnection = true;

                        if (x > 0 && (cityMap[y * citySize + (x - 1)] == SityMapObjects["HorizontalRoad"] ||
                                     cityMap[y * citySize + (x - 1)] == '+'))
                            hasHorizontalConnection = true;

                        if (x < citySize - 1 && (cityMap[y * citySize + x + 1] == SityMapObjects["HorizontalRoad"] ||
                                                cityMap[y * citySize + x + 1] == '+'))
                            hasHorizontalConnection = true;

                        if (hasVerticalConnection && !hasHorizontalConnection)
                        {
                            cityMap[y * citySize + x] = SityMapObjects["VerticalRoad"];
                        }
                    }
                }
            }

            FixParallelRoads(cityMap, citySize);

            ConnectDisconnectedRoads(cityMap, citySize);

            AddIntersectionsAtCrossroads(cityMap, citySize);
        }
        private static void FixParallelRoads(List<char> cityMap, int citySize)
        {
            for (int y = 1; y < citySize - 1; y++)
            {
                for (int x = 1; x < citySize - 1; x++)
                {
                    char current = cityMap[y * citySize + x];

                    if (current == SityMapObjects["VerticalRoad"])
                    {
                        if (x + 1 < citySize - 1 && cityMap[y * citySize + x + 1] == SityMapObjects["VerticalRoad"])
                        {
                            int leftConnections = CountVerticalConnections(cityMap, x, y, citySize);
                            int rightConnections = CountVerticalConnections(cityMap, x + 1, y, citySize);

                            if (leftConnections <= rightConnections)
                            {
                                cityMap[y * citySize + x] = SityMapObjects["Land"];
                            }
                            else
                            {
                                cityMap[y * citySize + x + 1] = SityMapObjects["Land"];
                            }
                        }
                    }
                    else if (current == SityMapObjects["HorizontalRoad"])
                    {
                        if (y + 1 < citySize - 1 && cityMap[(y + 1) * citySize + x] == SityMapObjects["HorizontalRoad"])
                        {
                            int topConnections = CountHorizontalConnections(cityMap, x, y, citySize);
                            int bottomConnections = CountHorizontalConnections(cityMap, x, y + 1, citySize);

                            if (topConnections <= bottomConnections)
                            {
                                cityMap[y * citySize + x] = SityMapObjects["Land"];
                            }
                            else
                            {
                                cityMap[(y + 1) * citySize + x] = SityMapObjects["Land"];
                            }
                        }
                    }
                }
            }
        }
        private static int CountVerticalConnections(List<char> cityMap, int x, int y, int citySize)
        {
            int connections = 0;

            if (y > 0 && (cityMap[(y - 1) * citySize + x] == SityMapObjects["VerticalRoad"] ||
                         cityMap[(y - 1) * citySize + x] == '+'))
                connections++;

            if (y < citySize - 1 && (cityMap[(y + 1) * citySize + x] == SityMapObjects["VerticalRoad"] ||
                                    cityMap[(y + 1) * citySize + x] == '+'))
                connections++;

            if (x > 0 && (cityMap[y * citySize + (x - 1)] == SityMapObjects["HorizontalRoad"] ||
                         cityMap[y * citySize + (x - 1)] == '+'))
                connections++;

            if (x < citySize - 1 && (cityMap[y * citySize + x + 1] == SityMapObjects["HorizontalRoad"] ||
                                    cityMap[y * citySize + x + 1] == '+'))
                connections++;

            return connections;
        }
        private static int CountHorizontalConnections(List<char> cityMap, int x, int y, int citySize)
        {
            int connections = 0;

            if (x > 0 && (cityMap[y * citySize + (x - 1)] == SityMapObjects["HorizontalRoad"] ||
                         cityMap[y * citySize + (x - 1)] == '+'))
                connections++;

            if (x < citySize - 1 && (cityMap[y * citySize + x + 1] == SityMapObjects["HorizontalRoad"] ||
                                    cityMap[y * citySize + x + 1] == '+'))
                connections++;

            if (y > 0 && (cityMap[(y - 1) * citySize + x] == SityMapObjects["VerticalRoad"] ||
                         cityMap[(y - 1) * citySize + x] == '+'))
                connections++;

            if (y < citySize - 1 && (cityMap[(y + 1) * citySize + x] == SityMapObjects["VerticalRoad"] ||
                                    cityMap[(y + 1) * citySize + x] == '+'))
                connections++;

            return connections;
        }
        private static void ConnectDisconnectedRoads(List<char> cityMap, int citySize)
        {
            var visited = new bool[citySize * citySize];
            var roadGroups = new List<List<(int x, int y)>>();

            for (int y = 1; y < citySize - 1; y++)
            {
                for (int x = 1; x < citySize - 1; x++)
                {
                    char current = cityMap[y * citySize + x];
                    if ((current == SityMapObjects["VerticalRoad"] ||
                         current == SityMapObjects["HorizontalRoad"] ||
                         current == '+') && !visited[y * citySize + x])
                    {
                        var group = new List<(int x, int y)>();
                        FloodFillRoads(cityMap, x, y, citySize, visited, group);
                        roadGroups.Add(group);
                    }
                }
            }

            if (roadGroups.Count > 1)
            {
                roadGroups = roadGroups.OrderByDescending(g => g.Count).ToList();
                var mainGroup = roadGroups[0];

                for (int i = 1; i < roadGroups.Count; i++)
                {
                    ConnectToMainGroup(cityMap, citySize, mainGroup, roadGroups[i]);
                }
            }
        }
        private static void FloodFillRoads(List<char> cityMap, int startX, int startY, int citySize,
                                          bool[] visited, List<(int x, int y)> group)
        {
            var stack = new Stack<(int x, int y)>();
            stack.Push((startX, startY));

            while (stack.Count > 0)
            {
                var (x, y) = stack.Pop();
                int index = y * citySize + x;

                if (x < 1 || x >= citySize - 1 || y < 1 || y >= citySize - 1 || visited[index])
                    continue;

                char current = cityMap[index];
                if (current != SityMapObjects["VerticalRoad"] &&
                    current != SityMapObjects["HorizontalRoad"] &&
                    current != '+')
                    continue;

                visited[index] = true;
                group.Add((x, y));

                stack.Push((x + 1, y));
                stack.Push((x - 1, y));
                stack.Push((x, y + 1));
                stack.Push((x, y - 1));
            }
        }
        private static void ConnectToMainGroup(List<char> cityMap, int citySize,
                                              List<(int x, int y)> mainGroup,
                                              List<(int x, int y)> otherGroup)
        {
            (int x1, int y1) closestMain = (0, 0);
            (int x2, int y2) closestOther = (0, 0);
            int minDistance = int.MaxValue;

            foreach (var (x1, y1) in mainGroup)
            {
                foreach (var (x2, y2) in otherGroup)
                {
                    int distance = Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestMain = (x1, y1);
                        closestOther = (x2, y2);
                    }
                }
            }

            ConnectPointsWithRoad(cityMap, closestMain, closestOther, citySize);
        }
        private static void ConnectPointsWithRoad(List<char> cityMap, (int x, int y) point1,
                                                 (int x, int y) point2, int citySize)
        {
            var random = new Random();

            int startX = Math.Min(point1.x, point2.x);
            int endX = Math.Max(point1.x, point2.x);
            int midY = point1.y;

            for (int x = startX; x <= endX; x++)
            {
                int index = midY * citySize + x;
                if (cityMap[index] == SityMapObjects["Land"])
                {
                    cityMap[index] = SityMapObjects["HorizontalRoad"];
                }
            }

            int startY = Math.Min(midY, point2.y);
            int endY = Math.Max(midY, point2.y);

            for (int y = startY; y <= endY; y++)
            {
                int index = y * citySize + point2.x;
                if (cityMap[index] == SityMapObjects["Land"])
                {
                    cityMap[index] = SityMapObjects["VerticalRoad"];
                }
            }
        }
        private static void AddIntersectionsAtCrossroads(List<char> cityMap, int citySize)
        {
            for (int y = 1; y < citySize - 1; y++)
            {
                for (int x = 1; x < citySize - 1; x++)
                {
                    char current = cityMap[y * citySize + x];

                    if (current == SityMapObjects["VerticalRoad"] ||
                        current == SityMapObjects["HorizontalRoad"])
                    {
                        bool hasVerticalNeighbor = false;
                        bool hasHorizontalNeighbor = false;

                        if (y > 0 && (cityMap[(y - 1) * citySize + x] == SityMapObjects["VerticalRoad"] ||
                                     cityMap[(y - 1) * citySize + x] == '+'))
                            hasVerticalNeighbor = true;

                        if (y < citySize - 1 && (cityMap[(y + 1) * citySize + x] == SityMapObjects["VerticalRoad"] ||
                                                cityMap[(y + 1) * citySize + x] == '+'))
                            hasVerticalNeighbor = true;

                        if (x > 0 && (cityMap[y * citySize + (x - 1)] == SityMapObjects["HorizontalRoad"] ||
                                     cityMap[y * citySize + (x - 1)] == '+'))
                            hasHorizontalNeighbor = true;

                        if (x < citySize - 1 && (cityMap[y * citySize + x + 1] == SityMapObjects["HorizontalRoad"] ||
                                                cityMap[y * citySize + x + 1] == '+'))
                            hasHorizontalNeighbor = true;

                        if (hasVerticalNeighbor && hasHorizontalNeighbor)
                        {
                            cityMap[y * citySize + x] = '+';
                        }
                    }
                }
            }
        }
        private void GenerateForests()
        {
            Random random = new Random();

            int numberOfForests = random.Next(8, 12);
            float forestScale = (float)(random.NextDouble() * 0.15 + 0.05);
            float forestThreshold = (float)(random.NextDouble() * 0.3 + 0.4);

            for (int forestIndex = 0; forestIndex < numberOfForests; forestIndex++)
            {
                float offsetX = (float)random.NextDouble() * 100f;
                float offsetY = (float)random.NextDouble() * 100f;

                int centerX, centerY;
                int attempts = 0;
                do
                {
                    centerX = random.Next(2, mapSize - 2);
                    centerY = random.Next(2, mapSize - 2);
                    attempts++;

                    if (attempts > 100) break;
                }
                while (Map[centerY * mapSize + centerX] != Game.MapObjects["Land"]);

                if (attempts > 100) continue;

                int forestRadius = random.Next(8, 15);

                for (int y = Math.Max(2, centerY - forestRadius); y < Math.Min(mapSize - 2, centerY + forestRadius); y++)
                {
                    for (int x = Math.Max(2, centerX - forestRadius); x < Math.Min(mapSize - 2, centerX + forestRadius); x++)
                    {
                        float distance = (float)Math.Sqrt(Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2));
                        if (distance > forestRadius) continue;

                        int index = y * mapSize + x;
                        if (Map[index] != Game.MapObjects["Land"]) continue;

                        if (Map[index] == Game.MapObjects["Capital"]) continue;

                        float noiseValue = ForestPerlinNoise(
                            (x + offsetX) * forestScale,
                            (y + offsetY) * forestScale
                        );

                        float radialFactor = 1.0f - distance / forestRadius;
                        float adjustedThreshold = forestThreshold * (0.5f + radialFactor * 0.5f);

                        if (noiseValue > adjustedThreshold)
                        {
                            if (random.Next(100) < 20 && HasEnoughLandNeighbors(x, y))
                            {
                                CreateTreeCluster(x, y, random);
                            }
                            else
                            {
                                Map[index] = Game.MapObjects["Forest"];
                            }
                        }
                    }
                }
            }
        }
        private bool HasEnoughLandNeighbors(int x, int y)
        {
            int landCount = 0;

            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (dx == 0 && dy == 0) continue;

                    int nx = x + dx;
                    int ny = y + dy;

                    if (nx >= 0 && nx < mapSize && ny >= 0 && ny < mapSize)
                    {
                        int neighborIndex = ny * mapSize + nx;
                        if (Map[neighborIndex] == Game.MapObjects["Land"] ||
                            Map[neighborIndex] == Game.MapObjects["Forest"] ||
                            Map[neighborIndex] == Game.MapObjects["Desert"])
                        {
                            landCount++;
                        }
                    }
                }
            }

            return landCount >= 4;
        }

        private void CreateTreeCluster(int centerX, int centerY, Random random)
        {
            int clusterSize = random.Next(2, 5);

            for (int i = 0; i < clusterSize; i++)
            {
                int offsetX = random.Next(-1, 2);
                int offsetY = random.Next(-1, 2);

                int treeX = centerX + offsetX;
                int treeY = centerY + offsetY;

                if (treeX >= 2 && treeX < mapSize - 2 &&
                treeY >= 2 && treeY < mapSize - 2)
                {
                    int index = treeY * mapSize + treeX;
                    if (Map[index] == Game.MapObjects["Land"] ||
                        Map[index] == Game.MapObjects["Desert"])
                    {
                        Map[index] = Game.MapObjects["Forest"];
                    }
                }
            }
        }

        private float ForestPerlinNoise(float x, float y)
        {
            int xi = (int)x;
            int yi = (int)y;

            float xf = x - xi;
            float yf = y - yi;

            float v1 = ForestRandomGradient(xi, yi, xf, yf);
            float v2 = ForestRandomGradient(xi + 1, yi, xf - 1, yf);
            float v3 = ForestRandomGradient(xi, yi + 1, xf, yf - 1);
            float v4 = ForestRandomGradient(xi + 1, yi + 1, xf - 1, yf - 1);

            float i1 = ForestInterpolate(v1, v2, xf);
            float i2 = ForestInterpolate(v3, v4, xf);

            return ForestInterpolate(i1, i2, yf);
        }

        private float ForestRandomGradient(int ix, int iy, float x, float y)
        {
            float random = 1420f * (float)Math.Sin(ix * 3242f + iy * 73234f + 1234f) *
                          (float)Math.Cos(ix * 5432f * iy * 65432f + 4321f);

            return (float)(random * 0.01 * (x * x + y * y));
        }

        private float ForestInterpolate(float a, float b, float t)
        {
            t = t * t * t * (t * (t * 6 - 15) + 10);
            return a + (b - a) * t;
        }
        private void GenerateMountains()
        {
            Random random = new Random();

            int numberOfMountainRanges = random.Next(4, 8);
            float mountainScale = (float)(random.NextDouble() * 0.06 + 0.03);
            float mountainThreshold = (float)(random.NextDouble() * 0.3 + 0.2);

            for (int rangeIndex = 0; rangeIndex < numberOfMountainRanges; rangeIndex++)
            {
                float offsetX = (float)random.NextDouble() * 200f;
                float offsetY = (float)random.NextDouble() * 200f;

                int centerX, centerY;
                int attempts = 0;
                bool foundSuitableLocation = false;

                do
                {
                    centerX = random.Next(5, mapSize - 5);
                    centerY = random.Next(5, mapSize - 5);
                    attempts++;

                    if (attempts > 50) break;

                    int index = centerY * mapSize + centerX;
                    if (Map[index] == Game.MapObjects["Land"] && IsFarFromWater(centerX, centerY, 2))
                    {
                        if (HasEnoughLandForMountains(centerX, centerY, 4))
                        {
                            foundSuitableLocation = true;
                        }
                    }
                }
                while (!foundSuitableLocation);

                if (!foundSuitableLocation) continue;

                int rangeRadius = random.Next(8, 15);

                for (int y = Math.Max(2, centerY - rangeRadius); y < Math.Min(mapSize - 2, centerY + rangeRadius); y++)
                {
                    for (int x = Math.Max(2, centerX - rangeRadius); x < Math.Min(mapSize - 2, centerX + rangeRadius); x++)
                    {
                        float distance = (float)Math.Sqrt(Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2));
                        if (distance > rangeRadius) continue;

                        int index = y * mapSize + x;
                        if (Map[index] != Game.MapObjects["Land"] && Map[index] != Game.MapObjects["Forest"]) continue;

                        if (Map[index] == Game.MapObjects["Capital"]) continue;

                        if (BordersWater(x, y)) continue;

                        float noiseValue = MountainPerlinNoise(
                            (x + offsetX) * mountainScale,
                            (y + offsetY) * mountainScale
                        );

                        float radialFactor = 1.0f - distance / rangeRadius;
                        float adjustedThreshold = mountainThreshold * (0.7f + radialFactor * 0.3f);

                        float ridgeFactor = (float)Math.Abs(Math.Sin((x - centerX) * 0.3f) * Math.Cos((y - centerY) * 0.3f));
                        adjustedThreshold -= ridgeFactor * 0.2f;

                        if (noiseValue > adjustedThreshold)
                        {
                            if (noiseValue > adjustedThreshold + 0.1f && distance < rangeRadius * 0.7f)
                            {
                                // Вершина горы
                                Map[index] = Game.MapObjects["Mountain"];

                                CreateMountainCluster(x, y, random);
                            }
                            else if (random.Next(100) < 60)
                            {
                                Map[index] = Game.MapObjects["Hill"];
                            }
                        }
                    }
                }
            }
        }

        private bool IsFarFromWater(int x, int y, int minDistance)
        {
            for (int dy = -minDistance; dy <= minDistance; dy++)
            {
                for (int dx = -minDistance; dx <= minDistance; dx++)
                {
                    int nx = x + dx;
                    int ny = y + dy;

                    if (nx >= 0 && nx < mapSize && ny >= 0 && ny < mapSize)
                    {
                        int neighborIndex = ny * mapSize + nx;
                        if (Map[neighborIndex] == Game.MapObjects["Sea"])
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private bool HasEnoughLandForMountains(int centerX, int centerY, int radius)
        {
            int landCount = 0;
            int totalCells = 0;

            for (int y = Math.Max(0, centerY - radius); y < Math.Min(mapSize, centerY + radius); y++)
            {
                for (int x = Math.Max(0, centerX - radius); x < Math.Min(mapSize, centerX + radius); x++)
                {
                    totalCells++;
                    int index = y * mapSize + x;
                    if (Map[index] == Game.MapObjects["Land"] || Map[index] == Game.MapObjects["Forest"])
                    {
                        landCount++;
                    }
                }
            }

            return (float)landCount / totalCells > 0.7f;
        }

        private bool BordersWater(int x, int y)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (dx == 0 && dy == 0) continue;

                    int nx = x + dx;
                    int ny = y + dy;

                    if (nx >= 0 && nx < mapSize && ny >= 0 && ny < mapSize)
                    {
                        int neighborIndex = ny * mapSize + nx;
                        if (Map[neighborIndex] == Game.MapObjects["Sea"])
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void CreateMountainCluster(int centerX, int centerY, Random random)
        {
            int clusterSize = random.Next(3, 7);

            for (int i = 0; i < clusterSize; i++)
            {
                int offsetX = random.Next(-2, 3);
                int offsetY = random.Next(-2, 3);

                int mountainX = centerX + offsetX;
                int mountainY = centerY + offsetY;

                if (mountainX >= 2 && mountainX < mapSize - 2 &&
                    mountainY >= 2 && mountainY < mapSize - 2)
                {
                    int index = mountainY * mapSize + mountainX;

                    if ((Map[index] == Game.MapObjects["Land"] || Map[index] == Game.MapObjects["Forest"])
                        && !BordersWater(mountainX, mountainY))
                    {
                        if (random.Next(100) < 50)
                        {
                            Map[index] = Game.MapObjects["Mountain"];
                        }
                    }
                }
            }
        }
        private float MountainPerlinNoise(float x, float y)
        {
            int xi = (int)x;
            int yi = (int)y;

            float xf = x - xi;
            float yf = y - yi;

            float v1 = MountainRandomGradient(xi, yi, xf, yf);
            float v2 = MountainRandomGradient(xi + 1, yi, xf - 1, yf);
            float v3 = MountainRandomGradient(xi, yi + 1, xf, yf - 1);
            float v4 = MountainRandomGradient(xi + 1, yi + 1, xf - 1, yf - 1);

            float i1 = MountainInterpolate(v1, v2, xf);
            float i2 = MountainInterpolate(v3, v4, xf);

            return MountainInterpolate(i1, i2, yf);
        }

        private float MountainRandomGradient(int ix, int iy, float x, float y)
        {
            float random = 8920f * (float)Math.Sin(ix * 5242f + iy * 83234f + 7234f) *
                          (float)Math.Cos(ix * 7432f * iy * 95432f + 5321f);

            return (float)(random * 0.005 * (x * x + y * y));
        }

        private float MountainInterpolate(float a, float b, float t)
        {
            t = t * t * t * (t * (t * 6 - 15) + 10);
            return a + (b - a) * t;
        }
        private void GenerateDeserts()
        {
            Random random = new Random();

            int numberOfDeserts = random.Next(5, 8);
            float desertScale = (float)(random.NextDouble() * 0.12 + 0.04);
            float desertThreshold = (float)(random.NextDouble() * 0.25 + 0.3);

            for (int desertIndex = 0; desertIndex < numberOfDeserts; desertIndex++)
            {
                float offsetX = (float)random.NextDouble() * 150f;
                float offsetY = (float)random.NextDouble() * 150f;

                int centerX, centerY;
                int attempts = 0;
                bool foundSuitableLocation = false;

                do
                {
                    centerX = random.Next(4, mapSize - 4);
                    centerY = random.Next(4, mapSize - 4);
                    attempts++;

                    if (attempts > 100) break;

                    int index = centerY * mapSize + centerX;
                    if (Map[index] == Game.MapObjects["Land"] && IsFarFromWater(centerX, centerY, 2))
                    {
                        if (IsDesertSuitableLocation(centerX, centerY, 1))
                        {
                            foundSuitableLocation = true;
                        }
                    }
                }
                while (!foundSuitableLocation);

                if (!foundSuitableLocation) continue;

                int desertRadius = random.Next(6, 20);

                for (int y = Math.Max(2, centerY - desertRadius); y < Math.Min(mapSize - 2, centerY + desertRadius); y++)
                {
                    for (int x = Math.Max(2, centerX - desertRadius); x < Math.Min(mapSize - 2, centerX + desertRadius); x++)
                    {
                        float distance = (float)Math.Sqrt(Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2));
                        if (distance > desertRadius) continue;

                        int index = y * mapSize + x;
                        if (Map[index] != Game.MapObjects["Land"]) continue;

                        if (Map[index] == Game.MapObjects["Capital"]) continue;

                        if (BordersWater(x, y)) continue;

                        if (Map[index] == Game.MapObjects["Forest"] ||
                            Map[index] == Game.MapObjects["Mountain"] ||
                            Map[index] == Game.MapObjects["Hill"]) continue;

                        float noiseValue = DesertPerlinNoise(
                            (x + offsetX) * desertScale,
                            (y + offsetY) * desertScale
                        );

                        float radialFactor = 1.0f - distance / desertRadius;
                        float adjustedThreshold = desertThreshold * (0.6f + radialFactor * 0.4f);

                        float duneFactor = (float)(Math.Sin(x * 0.5f) * Math.Cos(y * 0.3f) * 0.3f);
                        adjustedThreshold -= duneFactor;

                        if (noiseValue > adjustedThreshold)
                        {
                            if (distance < desertRadius * 0.3f && random.Next(100) < 10)
                            {
                                CreateOasis(x, y, random);
                            }
                            else
                            {
                                Map[index] = Game.MapObjects["Desert"];
                            }
                        }
                    }
                }
            }
        }

        private bool IsDesertSuitableLocation(int x, int y, int radius)
        {
            int unsuitableCount = 0;
            int totalChecked = 0;

            for (int dy = -radius; dy <= radius; dy++)
            {
                for (int dx = -radius; dx <= radius; dx++)
                {
                    int nx = x + dx;
                    int ny = y + dy;

                    if (nx >= 0 && nx < mapSize && ny >= 0 && ny < mapSize)
                    {
                        totalChecked++;
                        int index = ny * mapSize + nx;

                        if (Map[index] == Game.MapObjects["Forest"] ||
                            Map[index] == Game.MapObjects["Mountain"] ||
                            Map[index] == Game.MapObjects["Hill"] ||
                            Map[index] == Game.MapObjects["Sea"])
                        {
                            unsuitableCount++;
                        }
                    }
                }
            }

            return (float)unsuitableCount / totalChecked < 0.2f;
        }

        private void CreateOasis(int centerX, int centerY, Random random)
        {
            int oasisSize = random.Next(1, 3);

            for (int i = 0; i < oasisSize; i++)
            {
                int offsetX = random.Next(-1, 2);
                int offsetY = random.Next(-1, 2);

                int oasisX = centerX + offsetX;
                int oasisY = centerY + offsetY;

                if (oasisX >= 2 && oasisX < mapSize - 2 &&
                    oasisY >= 2 && oasisY < mapSize - 2)
                {
                    int index = oasisY * mapSize + oasisX;
                    if (Map[index] == Game.MapObjects["Desert"])
                    {
                        Map[index] = Game.MapObjects["Land"];
                    }
                }
            }
        }

        private float DesertPerlinNoise(float x, float y)
        {
            int xi = (int)x;
            int yi = (int)y;

            float xf = x - xi;
            float yf = y - yi;

            float v1 = DesertRandomGradient(xi, yi, xf, yf);
            float v2 = DesertRandomGradient(xi + 1, yi, xf - 1, yf);
            float v3 = DesertRandomGradient(xi, yi + 1, xf, yf - 1);
            float v4 = DesertRandomGradient(xi + 1, yi + 1, xf - 1, yf - 1);

            float i1 = DesertInterpolate(v1, v2, xf);
            float i2 = DesertInterpolate(v3, v4, xf);

            return DesertInterpolate(i1, i2, yf);
        }

        private float DesertRandomGradient(int ix, int iy, float x, float y)
        {
            float random = 6820f * (float)Math.Sin(ix * 4242f + iy * 63234f + 2234f) *
                          (float)Math.Cos(ix * 6432f * iy * 75432f + 3321f);

            return (float)(random * 0.008 * (x * x + y * y));
        }

        private float DesertInterpolate(float a, float b, float t)
        {
            t = t * t * t * (t * (t * 6 - 15) + 10);
            return a + (b - a) * t;
        }
        private static void AddRareTreesToCity(List<char> cityMap, int citySize, Random random)
        {
            int numberOfTrees = random.Next(10, 21);
            int treesPlaced = 0;
            int attempts = 0;
            const int MAX_ATTEMPTS = 100;

            while (treesPlaced < numberOfTrees && attempts < MAX_ATTEMPTS)
            {
                attempts++;

                int x = random.Next(1, citySize - 1);
                int y = random.Next(1, citySize - 1);
                int index = y * citySize + x;

                if (CanPlaceTreeAt(cityMap, citySize, x, y))
                {
                    cityMap[index] = SityMapObjects["Tree"];
                    treesPlaced++;

                    if (random.Next(100) < 30 && treesPlaced < numberOfTrees)
                    {
                        CreateTreeClusterInCity(cityMap, citySize, x, y, random);
                        treesPlaced += 2;
                    }
                }
            }
        }

        private static bool CanPlaceTreeAt(List<char> cityMap, int citySize, int x, int y)
        {
            int index = y * citySize + x;
            char currentCell = cityMap[index];

            if (currentCell != SityMapObjects["Land"])
                return false;

            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    int nx = x + dx;
                    int ny = y + dy;

                    if (nx >= 0 && nx < citySize && ny >= 0 && ny < citySize)
                    {
                        char neighbor = cityMap[ny * citySize + nx];

                        if (neighbor == SityMapObjects["VerticalRoad"] ||
                            neighbor == SityMapObjects["HorizontalRoad"] ||
                            neighbor == SityMapObjects["Crossroad"] ||
                            neighbor == SityMapObjects["CivilianFactory"] ||
                            neighbor == SityMapObjects["MilitaryFactory"] ||
                            neighbor == SityMapObjects["House"] ||
                            neighbor == SityMapObjects["Tree"])
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private static void CreateTreeClusterInCity(List<char> cityMap, int citySize, int centerX, int centerY, Random random)
        {
            int clusterSize = random.Next(2, 4);

            for (int i = 0; i < clusterSize; i++)
            {
                int offsetX = random.Next(-1, 2);
                int offsetY = random.Next(-1, 2);

                if (offsetX == 0 && offsetY == 0)
                    continue;

                int treeX = centerX + offsetX;
                int treeY = centerY + offsetY;

                if (treeX >= 1 && treeX < citySize - 1 &&
                    treeY >= 1 && treeY < citySize - 1)
                {
                    if (CanPlaceTreeAt(cityMap, citySize, treeX, treeY))
                    {
                        cityMap[treeY * citySize + treeX] = SityMapObjects["Tree"];
                    }
                }
            }
        }
    }
}
