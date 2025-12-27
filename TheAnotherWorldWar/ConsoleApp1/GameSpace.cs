using System;
using System.Collections.Generic;
using System.Text;
using static ConsoleApp1.Army;
using static ConsoleApp1.City;
using static ConsoleApp1.City.BuildingSystem;
using static ConsoleApp1.City.Road;
using static ConsoleApp1.Country;
using static ConsoleApp1.MenuManger;

namespace ConsoleApp1
{
    public class GameSpace
    {
        static bool showPoliticalMap = false;
        static int mapCursorX = -1;
        static int mapCursorY = -1;

        static int cityCursorX = -1;
        static int cityCursorY = -1;

        public static void ShowGameSpace(Game game)
        {
            Map gameMap = game.Map;
            List<Country> countries = game.GameCountries;
            int mapSize = game.MapSize;

            int cursorX;
            int cursorY;

            if (mapCursorX == -1 && mapCursorY == -1)
            {
                cursorX = mapSize / 2;
                cursorY = mapSize / 2;
            }
            else
            {
                cursorX = mapCursorX;
                cursorY = mapCursorY;
            }

            var cellToCountry = new Dictionary<int, Country>();
            foreach (var country in countries)
            {
                foreach (var cell in country.CountryTerritory)
                {
                    cellToCountry[cell] = country;
                }
            }

            Console.CursorVisible = false;

            while (true)
            {
                Console.Clear();

                int consoleWidth = Console.WindowWidth;
                int mapDisplayWidth = mapSize * 2 - 1;
                int leftPadding = (consoleWidth - mapDisplayWidth) / 2;

                int prevCursorX = cursorX;
                int prevCursorY = cursorY;

                if (showPoliticalMap)
                {
                    ConsoleWriteMapWithCountries(gameMap.MetaMap, mapSize, countries);
                }
                else
                {
                    ConsoleWriteMap(gameMap.MetaMap, mapSize);
                }

                DrawCursor(cursorX, cursorY, leftPadding, gameMap.MetaMap, mapSize, cellToCountry, showPoliticalMap);

                while (true)
                {
                    ConsoleKeyInfo key = ConsoleManager.ListenKays();

                    if (key.Key == ConsoleKey.UpArrow && cursorY > 1)
                    {
                        EraseCursor(prevCursorX, prevCursorY, leftPadding, gameMap.MetaMap, mapSize, cellToCountry, showPoliticalMap);
                        cursorY--;

                        DrawCursor(cursorX, cursorY, leftPadding, gameMap.MetaMap, mapSize, cellToCountry, showPoliticalMap);

                        mapCursorX = prevCursorX = cursorX;
                        mapCursorY = prevCursorY = cursorY;
                    }
                    else if (key.Key == ConsoleKey.DownArrow && cursorY < mapSize - 2)
                    {
                        EraseCursor(prevCursorX, prevCursorY, leftPadding, gameMap.MetaMap, mapSize, cellToCountry, showPoliticalMap);
                        cursorY++;

                        DrawCursor(cursorX, cursorY, leftPadding, gameMap.MetaMap, mapSize, cellToCountry, showPoliticalMap);

                        mapCursorX = prevCursorX = cursorX;
                        mapCursorY = prevCursorY = cursorY;
                    }
                    else if (key.Key == ConsoleKey.LeftArrow && cursorX > 1)
                    {
                        EraseCursor(prevCursorX, prevCursorY, leftPadding, gameMap.MetaMap, mapSize, cellToCountry, showPoliticalMap);
                        cursorX--;

                        DrawCursor(cursorX, cursorY, leftPadding, gameMap.MetaMap, mapSize, cellToCountry, showPoliticalMap);

                        mapCursorX = prevCursorX = cursorX;
                        mapCursorY = prevCursorY = cursorY;
                    }
                    else if (key.Key == ConsoleKey.RightArrow && cursorX < mapSize - 2)
                    {
                        EraseCursor(prevCursorX, prevCursorY, leftPadding, gameMap.MetaMap, mapSize, cellToCountry, showPoliticalMap);
                        cursorX++;

                        DrawCursor(cursorX, cursorY, leftPadding, gameMap.MetaMap, mapSize, cellToCountry, showPoliticalMap);

                        mapCursorX = prevCursorX = cursorX;
                        mapCursorY = prevCursorY = cursorY;
                    }
                    else if (key.Key == ConsoleKey.Z)
                    {
                        showPoliticalMap = !showPoliticalMap;

                        EraseCursor(prevCursorX, prevCursorY, leftPadding, gameMap.MetaMap, mapSize, cellToCountry, !showPoliticalMap);

                        mapCursorX = prevCursorX = cursorX;
                        mapCursorY = prevCursorY = cursorY;

                        break;
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        int cellIndex = cursorY * mapSize + cursorX;
                        var cellChar = gameMap.MetaMap[cellIndex];

                        City cityAtPosition = FindCityAtPosition(cursorX, cursorY, countries, mapSize);

                        Country countryAtPosition = FindCountryAtPosition(cursorX, cursorY, game.GameCountries, mapSize);

                        if ((cellChar == Game.MapObjects["Capital"] || cellChar == Game.MapObjects["City"]) && cityAtPosition != null && IsCityOwnedByPlayer(game, FindCityAtPosition(cursorX, cursorY, game.GameCountries, game.MapSize)))
                        {
                            EraseCursor(prevCursorX, prevCursorY, leftPadding, gameMap.MetaMap, mapSize, cellToCountry, showPoliticalMap);

                            ShowCityMap(cityAtPosition, game);

                            Console.CursorVisible = false;
                        }
                        else if (showPoliticalMap && cellChar != Game.MapObjects["Border"] && cellChar != Game.MapObjects["Sea"] && countryAtPosition != null && !ArmyMapObjects.Values.Contains(cellChar))
                        {
                            EraseCursor(prevCursorX, prevCursorY, leftPadding, gameMap.MetaMap, mapSize, cellToCountry, showPoliticalMap);

                            Country playerCountry = game.GameCountries.FirstOrDefault(c => !c.IsBot);

                            if (countryAtPosition == playerCountry)
                            {
                                MenuManger.InGameSetMenu(MenuManger.CreateDynamicInGameMenu(MenuManger.InGameCountryInfoMenuListAction,
                                    CreateCountryInfoString(game, countryAtPosition),
                                    "Back"), game, countryAtPosition);
                            }
                            else
                            {
                                CountryInfo countryInfo = new CountryInfo
                                {
                                    SelectedCountry = countryAtPosition,
                                    PlayerCountry = playerCountry
                                };

                                MenuManger.InGameSetMenu(MenuManger.CreateDynamicInGameMenu(MenuManger.InGameCountryInfoMenuListAction,
                                    CreateCountryInfoString(game, countryAtPosition),
                                    "Politics", "Back"), game, countryInfo);
                            }

                            Console.CursorVisible = false;
                        }
                        else if (ArmyMapObjects.Values.Contains(cellChar))
                        {
                            Country playerCountry = game.GameCountries.FirstOrDefault(c => !c.IsBot);

                            ArmyDivision selectedDivision = null;
                            foreach (var country in game.GameCountries)
                            {
                                if (country.CountryArmy?.Divisions != null)
                                {
                                    foreach (var division in country.CountryArmy.Divisions)
                                    {
                                        if (division.Position == cellIndex)
                                        {
                                            selectedDivision = division;
                                            break;
                                        }
                                    }
                                }
                                if (selectedDivision != null) break;
                            }

                            if (selectedDivision != null && selectedDivision.OwnerCountry == playerCountry)
                            {
                                EraseCursor(prevCursorX, prevCursorY, leftPadding, gameMap.MetaMap, mapSize, cellToCountry, showPoliticalMap);

                                var divisionInfo = new ArmyDivisionMenuInfo
                                {
                                    Division = selectedDivision,
                                    PlayerCountry = playerCountry,
                                    CurrentGame = game
                                };

                                MenuManger.InGameSetMenu(MenuManger.CreateDynamicInGameMenu(
                                    MenuManger.InGameArmyDivisionMenuAction,
                                    $"Division: {selectedDivision.DivisionType}, Position: ({cursorX}, {cursorY})",
                                    "Move", "Disband", "Back"), game, divisionInfo);
                            }
                        }
                        else if (!showPoliticalMap && IsValidArmyPlacementTerrain(cellChar) && CanBuildSettlementAt(cellChar) && !ArmyMapObjects.Values.Contains(cellChar) && countryAtPosition != null && !countryAtPosition.IsBot)
                        {
                            var cellInfo = new LandCellInfo
                            {
                                CellIndex = cellIndex,
                                CursorX = cursorX,
                                CursorY = cursorY,
                                CountryAtPosition = countryAtPosition
                            };

                            EraseCursor(prevCursorX, prevCursorY, leftPadding, gameMap.MetaMap, mapSize, cellToCountry, showPoliticalMap);

                            MenuManger.InGameSetMenu(MenuManger.InGameLandCellActionMenu, game, cellInfo);
                        }
                        else if (cellChar == Game.MapObjects["Port"])
                        {
                            Country portOwner = FindCountryAtPosition(cursorX, cursorY, game.GameCountries, mapSize);

                            if (portOwner != null && !portOwner.IsBot)
                            {
                                Port portAtPosition = FindPortAtPosition(cursorX, cursorY, portOwner, game);

                                if (portAtPosition != null)
                                {
                                    EraseCursor(prevCursorX, prevCursorY, leftPadding, gameMap.MetaMap, mapSize, cellToCountry, showPoliticalMap);
                                    Console.CursorVisible = true;
                                    Console.Clear();
                                    Console.ResetColor();

                                    InGameSetMenu(InGamePortActionMenu, game, portAtPosition);
                                }
                            }
                        }
                        else if (cellChar == Game.MapObjects["CargoShip"] || cellChar == Game.MapObjects["CombatShip"])
                        {
                            Country playerCountry = game.GameCountries.FirstOrDefault(c => !c.IsBot);

                            Warship selectedShip = null;
                            foreach (var country in game.GameCountries)
                            {
                                if (country.CountryPorts != null)
                                {
                                    foreach (var port in country.CountryPorts)
                                    {
                                        if (port.MilitaryShips != null)
                                        {
                                            foreach (var ship in port.MilitaryShips)
                                            {
                                                if (ship.Position == cellIndex)
                                                {
                                                    selectedShip = ship;
                                                    break;
                                                }
                                            }
                                        }
                                        if (selectedShip != null) break;
                                    }
                                }
                                if (selectedShip != null) break;
                            }

                            if (selectedShip != null && selectedShip.OwnerCountry == playerCountry)
                            {
                                EraseCursor(prevCursorX, prevCursorY, leftPadding, gameMap.MetaMap, mapSize, cellToCountry, showPoliticalMap);

                                var shipInfo = new Warship.WarshipMenuInfo
                                {
                                    Ship = selectedShip,
                                    PlayerCountry = playerCountry,
                                    CurrentGame = game
                                };

                                MenuManger.InGameSetMenu(MenuManger.CreateDynamicInGameMenu(
                                    MenuManger.InGameWarshipMenuAction,
                                    $"Ship: {selectedShip.ShipType}, Position: ({cursorX}, {cursorY})",
                                    "Move", "Disband", "Back"), game, shipInfo);
                            }
                        }
                    }
                    else if (key.Key == ConsoleKey.Escape)
                    {
                        EraseCursor(prevCursorX, prevCursorY, leftPadding, gameMap.MetaMap, mapSize, cellToCountry, showPoliticalMap);
                        Console.CursorVisible = true;
                        Console.Clear();
                        Console.ResetColor();

                        MenuManger.InGameSetMenu(MenuManger.InGameMenuEscList, game);
                    }
                    else if (key.Key == ConsoleKey.Spacebar)
                    {
                        game.NextTurn();
                    }
                }
            }
        }
        private static void EraseCursor(int x, int y, int leftPadding, List<char> map, int mapSize,
                               Dictionary<int, Country> cellToCountry, bool showPoliticalMap)
        {
            try
            {
                int savedCursorLeft = Console.CursorLeft;
                int savedCursorTop = Console.CursorTop;

                int cursorScreenX = leftPadding + x * 2;
                int cursorScreenY = savedCursorTop - mapSize + y;

                Console.SetCursorPosition(cursorScreenX, cursorScreenY);

                int cellIndex = y * mapSize + x;
                char cellChar = map[cellIndex];

                if (cellChar == ArmyMapObjects["InfantryDivision"] ||
                    cellChar == ArmyMapObjects["ArmoredDivision"] ||
                    cellChar == ArmyMapObjects["MountainDivision"])
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(cellChar);
                }
                else if (cellChar == Game.MapObjects["Border"])
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(cellChar);
                }
                else if (cellChar == Game.MapObjects["Land"])
                {
                    if (showPoliticalMap)
                    {
                        if (cellToCountry.TryGetValue(cellIndex, out var country))
                        {
                            Console.ForegroundColor = country.CountryColor;
                            Console.Write(Game.MapObjects["Land"]);
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(Game.MapObjects["Land"]);
                    }
                }
                else if (cellChar == Game.MapObjects["Forest"])
                {
                    if (showPoliticalMap)
                    {
                        if (cellToCountry.TryGetValue(cellIndex, out var country))
                        {
                            Console.ForegroundColor = country.CountryColor;
                            Console.Write(Game.MapObjects["Forest"]);
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write(Game.MapObjects["Forest"]);
                    }
                }
                else if (cellChar == Game.MapObjects["Mountain"])
                {
                    if (showPoliticalMap)
                    {
                        if (cellToCountry.TryGetValue(cellIndex, out var country))
                        {
                            Console.ForegroundColor = country.CountryColor;
                            Console.Write(Game.MapObjects["Mountain"]);
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(Game.MapObjects["Mountain"]);
                    }
                }
                else if (cellChar == Game.MapObjects["Hill"])
                {
                    if (showPoliticalMap)
                    {
                        if (cellToCountry.TryGetValue(cellIndex, out var country))
                        {
                            Console.ForegroundColor = country.CountryColor;
                            Console.Write(Game.MapObjects["Hill"]);
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(Game.MapObjects["Hill"]);
                    }
                }
                else if (cellChar == Game.MapObjects["Desert"])
                {
                    if (showPoliticalMap)
                    {
                        if (cellToCountry.TryGetValue(cellIndex, out var country))
                        {
                            Console.ForegroundColor = country.CountryColor;
                            Console.Write(Game.MapObjects["Desert"]);
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write(Game.MapObjects["Desert"]);
                    }
                }
                else if (cellChar == Game.MapObjects["Capital"] || cellChar == Game.MapObjects["City"] || cellChar == Game.MapObjects["Port"])
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(cellChar);
                }
                else if (cellChar == Game.MapObjects["CargoShip"] || cellChar == Game.MapObjects["CombatShip"])
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(cellChar);
                }
                else
                {
                    if (showPoliticalMap)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(Game.MapObjects["Sea"]);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write(Game.MapObjects["Sea"]);
                    }
                }

                Console.ResetColor();

                Console.SetCursorPosition(savedCursorLeft, savedCursorTop);
            }
            catch
            {
                return;
            }
        }
        private static void DrawCursor(int x, int y, int leftPadding, List<char> map, int mapSize,
                       Dictionary<int, Country> cellToCountry, bool showPoliticalMap)
        {
            try
            {
                int savedCursorLeft = Console.CursorLeft;
                int savedCursorTop = Console.CursorTop;

                int cursorScreenX = leftPadding + x * 2;
                int cursorScreenY = savedCursorTop - mapSize + y;

                Console.SetCursorPosition(cursorScreenX, cursorScreenY);

                int cellIndex = y * mapSize + x;
                char cellChar = map[cellIndex];

                ConsoleColor cellColor = ConsoleColor.Black;

                if (cellChar == ArmyMapObjects["InfantryDivision"] ||
                    cellChar == ArmyMapObjects["ArmoredDivision"] ||
                    cellChar == ArmyMapObjects["MountainDivision"])
                {
                    cellColor = ConsoleColor.White;
                }
                else if (showPoliticalMap)
                {
                    if (cellChar == Game.MapObjects["Border"])
                    {
                        cellColor = ConsoleColor.DarkGray;
                    }
                    else if (cellChar == Game.MapObjects["Land"])
                    {
                        if (cellToCountry.TryGetValue(cellIndex, out var country))
                        {
                            cellColor = country.CountryColor;
                        }
                    }
                    else if (cellChar == Game.MapObjects["Forest"])
                    {
                        if (cellToCountry.TryGetValue(cellIndex, out var country))
                        {
                            cellColor = country.CountryColor;
                        }
                    }
                    else if (cellChar == Game.MapObjects["Mountain"] || cellChar == Game.MapObjects["Hill"])
                    {
                        if (cellToCountry.TryGetValue(cellIndex, out var country))
                        {
                            cellColor = country.CountryColor;
                        }
                    }
                    else if (cellChar == Game.MapObjects["Desert"])
                    {
                        if (cellToCountry.TryGetValue(cellIndex, out var country))
                        {
                            cellColor = country.CountryColor;
                        }
                    }
                    else if (cellChar == Game.MapObjects["Capital"] || cellChar == Game.MapObjects["City"] || cellChar == Game.MapObjects["Port"])
                    {
                        cellColor = ConsoleColor.White;
                    }
                    else if (cellChar == Game.MapObjects["CargoShip"] || cellChar == Game.MapObjects["CombatShip"])
                    {
                        cellColor = ConsoleColor.White;
                    }
                    else
                    {
                        cellColor = ConsoleColor.White;
                    }
                }
                else
                {
                    if (cellChar == Game.MapObjects["Border"])
                    {
                        cellColor = ConsoleColor.DarkGray;
                    }
                    else if (cellChar == Game.MapObjects["Land"])
                    {
                        cellColor = ConsoleColor.Green;
                    }
                    else if (cellChar == Game.MapObjects["Forest"])
                    {
                        cellColor = ConsoleColor.DarkGreen;
                    }
                    else if (cellChar == Game.MapObjects["Mountain"] || cellChar == Game.MapObjects["Hill"])
                    {
                        cellColor = ConsoleColor.DarkGray;
                    }
                    else if (cellChar == Game.MapObjects["Desert"])
                    {
                        cellColor = ConsoleColor.DarkYellow;
                    }
                    else if (cellChar == Game.MapObjects["Capital"] || cellChar == Game.MapObjects["City"] || cellChar == Game.MapObjects["Port"])
                    {
                        cellColor = ConsoleColor.White;
                    }
                    else if (cellChar == Game.MapObjects["CargoShip"] || cellChar == Game.MapObjects["CombatShip"])
                    {
                        cellColor = ConsoleColor.White;
                    }
                    else
                    {
                        cellColor = ConsoleColor.Blue;
                    }
                }

                Console.BackgroundColor = cellColor;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write(cellChar);
                Console.ResetColor();

                Console.SetCursorPosition(savedCursorLeft, savedCursorTop);
            }
            catch
            {
                return;
            }
        }
        public static void ConsoleWriteMap(List<char> map, int mapSize)
        {
            int consoleWidth = Console.WindowWidth;
            int mapDisplayWidth = mapSize * 2 - 1;
            int leftPadding = (consoleWidth - mapDisplayWidth) / 2;

            for (int y = 0; y < mapSize; y++)
            {
                Console.CursorLeft = leftPadding;

                for (int x = 0; x < mapSize; x++)
                {
                    char cell = map[y * mapSize + x];

                    if (cell == ArmyMapObjects["InfantryDivision"] ||
                        cell == ArmyMapObjects["ArmoredDivision"] ||
                        cell == ArmyMapObjects["MountainDivision"])
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (cell == Game.MapObjects["Border"])
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }
                    else if (cell == Game.MapObjects["Land"])
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    else if (cell == Game.MapObjects["Forest"])
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                    }
                    else if (cell == Game.MapObjects["Mountain"] || cell == Game.MapObjects["Hill"])
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }
                    else if (cell == Game.MapObjects["Desert"])
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                    }
                    else if (cell == Game.MapObjects["Capital"] || cell == Game.MapObjects["City"] || cell == Game.MapObjects["Port"])
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (cell == Game.MapObjects["CargoShip"] || cell == Game.MapObjects["CombatShip"])
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                    }

                    Console.Write(cell);

                    if (x < mapSize - 1)
                    {
                        Console.Write(' ');
                    }

                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }
        public static void ConsoleWriteMapWithCountries(List<char> map, int mapSize, List<Country> countries)
        {
            int consoleWidth = Console.WindowWidth;
            int mapDisplayWidth = mapSize * 2 - 1;
            int leftPadding = (consoleWidth - mapDisplayWidth) / 2;

            var cellToCountry = new Dictionary<int, Country>();
            foreach (var country in countries)
            {
                foreach (var cell in country.CountryTerritory)
                {
                    cellToCountry[cell] = country;
                }
            }

            for (int y = 0; y < mapSize; y++)
            {
                Console.CursorLeft = leftPadding;

                for (int x = 0; x < mapSize; x++)
                {
                    int index = y * mapSize + x;
                    char cell = map[index];

                    if (cell == ArmyMapObjects["InfantryDivision"] ||
                        cell == ArmyMapObjects["ArmoredDivision"] ||
                        cell == ArmyMapObjects["MountainDivision"])
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(cell);
                    }
                    else if (cell == Game.MapObjects["Border"])
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(cell);
                    }
                    else if (cell == Game.MapObjects["Land"])
                    {
                        if (cellToCountry.TryGetValue(index, out var country))
                        {
                            Console.ForegroundColor = country.CountryColor;
                            Console.Write(Game.MapObjects["Land"]);
                        }
                    }
                    else if (cell == Game.MapObjects["Forest"])
                    {
                        if (cellToCountry.TryGetValue(index, out var country))
                        {
                            Console.ForegroundColor = country.CountryColor;
                            Console.Write(Game.MapObjects["Forest"]);
                        }
                    }
                    else if (cell == Game.MapObjects["Mountain"])
                    {
                        if (cellToCountry.TryGetValue(index, out var country))
                        {
                            Console.ForegroundColor = country.CountryColor;
                            Console.Write(Game.MapObjects["Mountain"]);
                        }
                    }
                    else if (cell == Game.MapObjects["Hill"])
                    {
                        if (cellToCountry.TryGetValue(index, out var country))
                        {
                            Console.ForegroundColor = country.CountryColor;
                            Console.Write(Game.MapObjects["Hill"]);
                        }
                    }
                    else if (cell == Game.MapObjects["Desert"])
                    {
                        if (cellToCountry.TryGetValue(index, out var country))
                        {
                            Console.ForegroundColor = country.CountryColor;
                            Console.Write(Game.MapObjects["Desert"]);
                        }
                    }
                    else if (cell == Game.MapObjects["Capital"] || cell == Game.MapObjects["City"] || cell == Game.MapObjects["Port"])
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(cell);
                    }
                    else if (cell == Game.MapObjects["CargoShip"] || cell == Game.MapObjects["CombatShip"])
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(cell);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(Game.MapObjects["Sea"]);
                    }

                    if (x < mapSize - 1)
                    {
                        Console.Write(' ');
                    }

                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }
        public static void ShowGameSpaceForMovement(Game game, ArmyMovementInfo movementInfo)
        {
            Map gameMap = game.Map;
            List<Country> countries = game.GameCountries;
            int mapSize = game.MapSize;

            ArmyDivision division = movementInfo.Division;
            int divisionX = division.Position % mapSize;
            int divisionY = division.Position / mapSize;

            int cursorX = divisionX;
            int cursorY = divisionY;

            var cellToCountry = new Dictionary<int, Country>();
            foreach (var country in countries)
            {
                foreach (var cell in country.CountryTerritory)
                {
                    cellToCountry[cell] = country;
                }
            }

            Console.CursorVisible = false;

            while (true)
            {
                Console.Clear();

                int consoleWidth = Console.WindowWidth;
                int mapDisplayWidth = mapSize * 2 - 1;
                int leftPadding = (consoleWidth - mapDisplayWidth) / 2;

                int prevCursorX = cursorX;
                int prevCursorY = cursorY;

                ConsoleWriteMapWithCountries(gameMap.MetaMap, mapSize, countries);

                HighlightAvailableMovementCells(game, division, mapSize, leftPadding);

                if (prevCursorX != -1 && prevCursorY != -1)
                {
                    UpdateCellWithHighlight(game, division, prevCursorX, prevCursorY, mapSize, leftPadding, countries, cellToCountry);
                }

                DrawMovementCursor(cursorX, cursorY, leftPadding, gameMap.MetaMap, mapSize, cellToCountry, game, division);

                DisplayMovementInfo(division, cursorX, cursorY, mapSize);

                while (true)
                {
                    ConsoleKeyInfo key = ConsoleManager.ListenKays();

                    if (key.Key == ConsoleKey.UpArrow && cursorY > 1)
                    {
                        if (prevCursorX != -1 && prevCursorY != -1)
                        {
                            UpdateCellWithHighlight(game, division, prevCursorX, prevCursorY, mapSize, leftPadding, countries, cellToCountry);
                        }
                        cursorY--;
                        DrawMovementCursor(cursorX, cursorY, leftPadding, gameMap.MetaMap, mapSize, cellToCountry, game, division);
                        DisplayMovementInfo(division, cursorX, cursorY, mapSize);
                        prevCursorX = cursorX;
                        prevCursorY = cursorY;
                    }
                    else if (key.Key == ConsoleKey.DownArrow && cursorY < mapSize - 2)
                    {
                        if (prevCursorX != -1 && prevCursorY != -1)
                        {
                            UpdateCellWithHighlight(game, division, prevCursorX, prevCursorY, mapSize, leftPadding, countries, cellToCountry);
                        }
                        cursorY++;
                        DrawMovementCursor(cursorX, cursorY, leftPadding, gameMap.MetaMap, mapSize, cellToCountry, game, division);
                        DisplayMovementInfo(division, cursorX, cursorY, mapSize);
                        prevCursorX = cursorX;
                        prevCursorY = cursorY;
                    }
                    else if (key.Key == ConsoleKey.LeftArrow && cursorX > 1)
                    {
                        if (prevCursorX != -1 && prevCursorY != -1)
                        {
                            UpdateCellWithHighlight(game, division, prevCursorX, prevCursorY, mapSize, leftPadding, countries, cellToCountry);
                        }
                        cursorX--;
                        DrawMovementCursor(cursorX, cursorY, leftPadding, gameMap.MetaMap, mapSize, cellToCountry, game, division);
                        DisplayMovementInfo(division, cursorX, cursorY, mapSize);
                        prevCursorX = cursorX;
                        prevCursorY = cursorY;
                    }
                    else if (key.Key == ConsoleKey.RightArrow && cursorX < mapSize - 2)
                    {
                        if (prevCursorX != -1 && prevCursorY != -1)
                        {
                            UpdateCellWithHighlight(game, division, prevCursorX, prevCursorY, mapSize, leftPadding, countries, cellToCountry);
                        }
                        cursorX++;
                        DrawMovementCursor(cursorX, cursorY, leftPadding, gameMap.MetaMap, mapSize, cellToCountry, game, division);
                        DisplayMovementInfo(division, cursorX, cursorY, mapSize);
                        prevCursorX = cursorX;
                        prevCursorY = cursorY;
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        int targetCellIndex = cursorY * mapSize + cursorX;

                        if (CanMoveToPosition(game, division, targetCellIndex, countries, mapSize))
                        {
                            bool positionOccupied = false;
                            foreach (var country in countries)
                            {
                                if (country.CountryArmy?.Divisions != null)
                                {
                                    foreach (var otherDivision in country.CountryArmy.Divisions)
                                    {
                                        if (otherDivision.Position == targetCellIndex && otherDivision != division)
                                        {
                                            positionOccupied = true;
                                            break;
                                        }
                                    }
                                }
                                if (positionOccupied) break;
                            }

                            if (!positionOccupied)
                            {
                                MoveDivision(game, division, targetCellIndex);
                                ShowGameSpace(game);
                                return;
                            }
                            else
                            {
                                ConsoleManager.ErrorMassege("Position is occupied by another division!");
                                ShowGameSpace(game);
                                return;
                            }
                        }
                        else
                        {
                            ConsoleManager.ErrorMassege("Cannot move to this position!");
                            ShowGameSpace(game);
                            return;
                        }
                    }
                    else if (key.Key == ConsoleKey.Escape)
                    {
                        ShowGameSpace(game);
                        return;
                    }
                }
            }
        }

        private static void DrawMovementCursor(int x, int y, int leftPadding, List<char> map, int mapSize,
                               Dictionary<int, Country> cellToCountry, Game game, ArmyDivision division)
        {
            try
            {
                int savedCursorLeft = Console.CursorLeft;
                int savedCursorTop = Console.CursorTop;

                Console.SetCursorPosition(0, savedCursorTop - mapSize);


                int cursorScreenX = leftPadding + x * 2;

                int cursorScreenY = savedCursorTop - mapSize + y;

                Console.SetCursorPosition(cursorScreenX, cursorScreenY);

                int cellIndex = y * mapSize + x;
                char cellChar = map[cellIndex];

                bool canMove = CanMoveToPosition(game, division, cellIndex, game.GameCountries, mapSize);

                bool occupied = false;
                foreach (var country in game.GameCountries)
                {
                    if (country.CountryArmy?.Divisions != null)
                    {
                        foreach (var otherDivision in country.CountryArmy.Divisions)
                        {
                            if (otherDivision.Position == cellIndex && otherDivision != division)
                            {
                                occupied = true;
                                break;
                            }
                        }
                    }
                    if (occupied) break;
                }

                ConsoleColor backgroundColor;
                if (occupied || !canMove)
                {
                    backgroundColor = ConsoleColor.DarkRed;
                }
                else
                {
                    backgroundColor = ConsoleColor.DarkGreen;
                }

                ConsoleColor foregroundColor = ConsoleColor.Black;

                if (cellChar == ArmyMapObjects["InfantryDivision"] ||
                    cellChar == ArmyMapObjects["ArmoredDivision"] ||
                    cellChar == ArmyMapObjects["MountainDivision"])
                {
                    foregroundColor = ConsoleColor.White;
                }
                else
                {
                    if (cellToCountry.TryGetValue(cellIndex, out var country))
                    {
                        foregroundColor = country.CountryColor;
                    }
                    else
                    {
                        if (cellChar == Game.MapObjects["Border"])
                        {
                            foregroundColor = ConsoleColor.DarkGray;
                        }
                        else if (cellChar == Game.MapObjects["Land"])
                        {
                            foregroundColor = ConsoleColor.Green;
                        }
                        else if (cellChar == Game.MapObjects["Forest"])
                        {
                            foregroundColor = ConsoleColor.DarkGreen;
                        }
                        else if (cellChar == Game.MapObjects["Mountain"] || cellChar == Game.MapObjects["Hill"])
                        {
                            foregroundColor = ConsoleColor.DarkGray;
                        }
                        else if (cellChar == Game.MapObjects["Desert"])
                        {
                            foregroundColor = ConsoleColor.DarkYellow;
                        }
                        else if (cellChar == Game.MapObjects["Capital"])
                        {
                            foregroundColor = ConsoleColor.White;
                        }
                        else
                        {
                            foregroundColor = ConsoleColor.Black;
                        }
                    }
                }

                Console.BackgroundColor = backgroundColor;
                Console.ForegroundColor = foregroundColor;
                Console.Write(cellChar);
                Console.ResetColor();

                Console.SetCursorPosition(savedCursorLeft, savedCursorTop);
            }
            catch
            {
                return;
            }
        }

        private static void UpdateCellWithHighlight(Game game, ArmyDivision division,
                                      int x, int y, int mapSize, int leftPadding,
                                      List<Country> countries, Dictionary<int, Country> cellToCountry)
        {
            try
            {
                int savedCursorLeft = Console.CursorLeft;
                int savedCursorTop = Console.CursorTop;

                Console.SetCursorPosition(0, savedCursorTop - mapSize);

                int screenX = leftPadding + x * 2;
                int screenY = savedCursorTop - mapSize + y;

                Console.SetCursorPosition(screenX, screenY);

                int cellIndex = y * mapSize + x;
                char cellChar = game.Map.MetaMap[cellIndex];

                bool canMove = CanMoveToPosition(game, division, cellIndex, countries, mapSize);

                bool occupied = false;
                foreach (var country in countries)
                {
                    if (country.CountryArmy?.Divisions != null)
                    {
                        foreach (var otherDivision in country.CountryArmy.Divisions)
                        {
                            if (otherDivision.Position == cellIndex && otherDivision != division)
                            {
                                occupied = true;
                                break;
                            }
                        }
                    }
                    if (occupied) break;
                }

                if (canMove && !occupied)
                {
                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(cellChar);
                    Console.ResetColor();
                }
                else
                {
                    if (cellChar == ArmyMapObjects["InfantryDivision"] ||
                        cellChar == ArmyMapObjects["ArmoredDivision"] ||
                        cellChar == ArmyMapObjects["MountainDivision"])
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(cellChar);
                    }
                    else if (cellChar == Game.MapObjects["Border"])
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(cellChar);
                    }
                    else if (cellChar == Game.MapObjects["Land"])
                    {
                        if (cellToCountry.TryGetValue(cellIndex, out var country))
                        {
                            Console.ForegroundColor = country.CountryColor;
                            Console.Write(Game.MapObjects["Land"]);
                        }
                    }
                    else if (cellChar == Game.MapObjects["Forest"])
                    {
                        if (cellToCountry.TryGetValue(cellIndex, out var country))
                        {
                            Console.ForegroundColor = country.CountryColor;
                            Console.Write(Game.MapObjects["Forest"]);
                        }
                    }
                    else if (cellChar == Game.MapObjects["Mountain"] || cellChar == Game.MapObjects["Hill"])
                    {
                        if (cellToCountry.TryGetValue(cellIndex, out var country))
                        {
                            Console.ForegroundColor = country.CountryColor;
                            Console.Write(cellChar);
                        }
                    }
                    else if (cellChar == Game.MapObjects["Desert"])
                    {
                        if (cellToCountry.TryGetValue(cellIndex, out var country))
                        {
                            Console.ForegroundColor = country.CountryColor;
                            Console.Write(Game.MapObjects["Desert"]);
                        }
                    }
                    else if (cellChar == Game.MapObjects["Capital"])
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(cellChar);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(Game.MapObjects["Sea"]);
                    }
                }

                Console.ResetColor();
                Console.SetCursorPosition(savedCursorLeft, savedCursorTop);
            }
            catch
            {
                return;
            }
        }
        private static void HighlightAvailableMovementCells(Game game, ArmyDivision division,
                                                    int mapSize, int leftPadding)
        {
            int maxDistance = GetMaxMovementDistance(division.DivisionType);
            int centerX = division.Position % mapSize;
            int centerY = division.Position / mapSize;

            Country divisionOwner = division.OwnerCountry;

            int savedCursorTop = Console.CursorTop;
            int mapStartY = savedCursorTop - mapSize;

            for (int y = Math.Max(1, centerY - maxDistance); y <= Math.Min(mapSize - 2, centerY + maxDistance); y++)
            {
                for (int x = Math.Max(1, centerX - maxDistance); x <= Math.Min(mapSize - 2, centerX + maxDistance); x++)
                {
                    int distance = Math.Abs(x - centerX) + Math.Abs(y - centerY);
                    if (distance <= maxDistance && distance > 0)
                    {
                        int cellIndex = y * mapSize + x;

                        bool canMove = CanMoveToPosition(game, division, cellIndex, game.GameCountries, mapSize);

                        bool occupied = IsCellOccupiedByDivision(game, cellIndex, division);

                        Country territoryOwner = null;
                        foreach (var country in game.GameCountries)
                        {
                            if (country.CountryTerritory.Contains(cellIndex))
                            {
                                territoryOwner = country;
                                break;
                            }
                        }

                        ConsoleColor highlightColor;
                        if (occupied)
                        {
                            highlightColor = ConsoleColor.DarkRed;
                        }
                        else if (territoryOwner != null && divisionOwner.IsAtWarWith(territoryOwner))
                        {
                            highlightColor = ConsoleColor.DarkYellow;
                        }
                        else if (canMove)
                        {
                            highlightColor = ConsoleColor.DarkGreen;
                        }
                        else
                        {
                            continue;
                        }

                        int screenX = leftPadding + x * 2;
                        int screenY = mapStartY + y;

                        try
                        {
                            Console.SetCursorPosition(screenX, screenY);
                            Console.BackgroundColor = highlightColor;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write(game.Map.MetaMap[cellIndex]);
                            Console.ResetColor();
                        }
                        catch { }
                    }
                }
            }

            Console.SetCursorPosition(0, savedCursorTop);
        }
        public static void ShowCityMap(City city, Game game)
        {
            int citySize = (int)Math.Sqrt(city.CityMap.Count);

            int cursorX = citySize / 2;
            int cursorY = citySize / 2;

            if (cityCursorX == -1 && cityCursorY == -1)
            {
                cursorX = citySize / 2;
                cursorY = citySize / 2;
            }
            else
            {
                cursorX = cityCursorX;
                cursorY = cityCursorY;
            }

            Console.CursorVisible = false;

            while (true)
            {
                Console.Clear();

                int consoleWidth = Console.WindowWidth;
                int mapDisplayWidth = citySize * 2 - 1;
                int leftPadding = (consoleWidth - mapDisplayWidth) / 2;

                int prevCursorX = cursorX;
                int prevCursorY = cursorY;

                DrawCityMap(city.CityMap, citySize, leftPadding);

                Console.WriteLine();

                if (!city.IsCapital)
                {
                    ConsoleManager.ConsoleWriteLineCentered($"City: {city.СityName}");
                }
                else
                {
                    ConsoleManager.ConsoleWriteLineCentered($"Сapital: {city.СityName}");
                }

                DrawCityCursor(cursorX, cursorY, leftPadding, city.CityMap, citySize);

                while (true)
                {
                    ConsoleKeyInfo key = ConsoleManager.ListenKays();

                    if (key.Key == ConsoleKey.UpArrow && cursorY > 1)
                    {
                        EraseCityCursor(prevCursorX, prevCursorY, leftPadding, city.CityMap, citySize);
                        cursorY--;

                        DrawCityCursor(cursorX, cursorY, leftPadding, city.CityMap, citySize);

                        cityCursorX = prevCursorX = cursorX;
                        cityCursorY = prevCursorY = cursorY;
                    }
                    else if (key.Key == ConsoleKey.DownArrow && cursorY < citySize - 2)
                    {
                        EraseCityCursor(prevCursorX, prevCursorY, leftPadding, city.CityMap, citySize);
                        cursorY++;

                        DrawCityCursor(cursorX, cursorY, leftPadding, city.CityMap, citySize);

                        cityCursorX = prevCursorX = cursorX;
                        cityCursorY = prevCursorY = cursorY;
                    }
                    else if (key.Key == ConsoleKey.LeftArrow && cursorX > 1)
                    {
                        EraseCityCursor(prevCursorX, prevCursorY, leftPadding, city.CityMap, citySize);
                        cursorX--;

                        DrawCityCursor(cursorX, cursorY, leftPadding, city.CityMap, citySize);

                        cityCursorX = prevCursorX = cursorX;
                        cityCursorY = prevCursorY = cursorY;
                    }
                    else if (key.Key == ConsoleKey.RightArrow && cursorX < citySize - 2)
                    {
                        EraseCityCursor(prevCursorX, prevCursorY, leftPadding, city.CityMap, citySize);
                        cursorX++;

                        DrawCityCursor(cursorX, cursorY, leftPadding, city.CityMap, citySize);

                        cityCursorX = prevCursorX = cursorX;
                        cityCursorY = prevCursorY = cursorY;
                    }
                    else if (key.Key == ConsoleKey.Escape)
                    {
                        EraseCityCursor(prevCursorX, prevCursorY, leftPadding, city.CityMap, citySize);

                        cityCursorX = cityCursorY = -1;

                        ShowGameSpace(game);
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        int cellIndex = cursorY * citySize + cursorX;
                        char cellChar = city.CityMap[cellIndex];

                        if (cellChar == SityMapObjects["MilitaryFactory"])
                        {
                            MilitaryFactory factory = MilitaryFactory.FindMilitaryFactory(city, cellIndex);
                            if (factory != null)
                            {
                                EraseCityCursor(prevCursorX, prevCursorY, leftPadding, city.CityMap, citySize);
                                Console.Clear();
                                Console.ResetColor();

                                MenuManger.InGameSetMenu(MenuManger.CreateDynamicInGameMenu(MenuManger.InGameBuildingManipulationListAction,
                                    $"Select an action.", "Open", "Demolish"), game, factory);
                            }
                        }
                        else if (cellChar == SityMapObjects["CivilianFactory"])
                        {
                            CivilianFactory factory = CivilianFactory.FindCivilianFactory(city, cellIndex);
                            if (factory != null)
                            {
                                EraseCityCursor(prevCursorX, prevCursorY, leftPadding, city.CityMap, citySize);
                                Console.Clear();
                                Console.ResetColor();

                                MenuManger.InGameSetMenu(MenuManger.CreateDynamicInGameMenu(MenuManger.InGameBuildingManipulationListAction,
                                    $"Select an action.", "Demolish"), game, factory);
                            }
                        }
                        else if (cellChar == SityMapObjects["House"])
                        {
                            House house = House.FindHouse(city, cellIndex);
                            if (house != null)
                            {
                                EraseCityCursor(prevCursorX, prevCursorY, leftPadding, city.CityMap, citySize);
                                Console.Clear();
                                Console.ResetColor();

                                MenuManger.InGameSetMenu(MenuManger.CreateDynamicInGameMenu(MenuManger.InGameBuildingManipulationListAction,
                                    $"Select an action.", "Demolish"), game, house);
                            }
                        }
                        else if (cellChar == SityMapObjects["VerticalRoad"] ||
                                    cellChar == SityMapObjects["HorizontalRoad"] ||
                                    cellChar == SityMapObjects["Crossroad"])
                        {
                            EraseCityCursor(prevCursorX, prevCursorY, leftPadding, city.CityMap, citySize);
                            Console.Clear();
                            Console.ResetColor();

                            RoadInfo roadInfo = new RoadInfo(city, cellIndex);
                            MenuManger.InGameSetMenu(MenuManger.CreateDynamicInGameMenu(MenuManger.InGameBuildingManipulationListAction,
                                $"Select an action.", "Demolish"), game, roadInfo);
                        }
                        else if (cellChar == SityMapObjects["Land"])
                        {
                            Country cityOwner = FindCountryForCity(city, game.GameCountries);
                            if (cityOwner != null && !cityOwner.IsBot)
                            {
                                EraseCityCursor(prevCursorX, prevCursorY, leftPadding, city.CityMap, citySize);
                                Console.Clear();
                                Console.ResetColor();

                                MenuManger.InGameSetMenu(MenuManger.CreateDynamicInGameMenu(MenuManger.InGameBuildMenuListAction,
                                    $"Select building to construct (Available money: ${cityOwner.CountryMoney})",
                                    BuildableStructures), game, new BuildInfo(city, cellIndex));
                            }
                        }
                    }
                }
            }
        }
        private static void DrawCityMap(List<char> cityMap, int citySize, int leftPadding)
        {
            for (int y = 0; y < citySize; y++)
            {
                Console.CursorLeft = leftPadding;

                for (int x = 0; x < citySize; x++)
                {
                    char cell = cityMap[y * citySize + x];
                    SetCityCellColor(cell);
                    Console.Write(cell);

                    if (x < citySize - 1)
                    {
                        Console.Write(' ');
                    }

                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }
        private static void SetCityCellColor(char cell)
        {
            if (cell == SityMapObjects["Border"])
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
            }
            else if (cell == SityMapObjects["VerticalRoad"])
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
            }
            else if (cell == SityMapObjects["HorizontalRoad"])
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
            }
            else if (cell == SityMapObjects["Crossroad"])
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
            }
            else if (cell == SityMapObjects["CivilianFactory"])
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
            else if (cell == SityMapObjects["MilitaryFactory"])
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
            }
            else if (cell == SityMapObjects["House"])
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            }
            else if (cell == SityMapObjects["Tree"])
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
            }
            else if (cell == SityMapObjects["Land"])
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
        }
        private static void DrawCityCursor(int x, int y, int leftPadding, List<char> cityMap, int citySize)
        {
            try
            {
                int savedCursorLeft = Console.CursorLeft;
                int savedCursorTop = Console.CursorTop - 2;

                int cursorScreenX = leftPadding + x * 2;
                int cursorScreenY = savedCursorTop - citySize + y;

                if (cursorScreenX >= 0 && cursorScreenX < Console.WindowWidth &&
                    cursorScreenY >= 0 && cursorScreenY < Console.WindowHeight)
                {
                    Console.SetCursorPosition(cursorScreenX, cursorScreenY);

                    int cellIndex = y * citySize + x;
                    char cellChar = cityMap[cellIndex];

                    ConsoleColor cellColor = GetCityCellBackgroundColor(cellChar);

                    Console.BackgroundColor = cellColor;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(cellChar);
                    Console.ResetColor();

                    Console.SetCursorPosition(savedCursorLeft, savedCursorTop + 2);
                }
            }
            catch
            {
                return;
            }
        }
        private static void EraseCityCursor(int x, int y, int leftPadding, List<char> cityMap, int citySize)
        {
            try
            {
                int savedCursorLeft = Console.CursorLeft;
                int savedCursorTop = Console.CursorTop - 2;

                int cursorScreenX = leftPadding + x * 2;
                int cursorScreenY = savedCursorTop - citySize + y;

                if (cursorScreenX >= 0 && cursorScreenX < Console.WindowWidth &&
                    cursorScreenY >= 0 && cursorScreenY < Console.WindowHeight)
                {
                    Console.SetCursorPosition(cursorScreenX, cursorScreenY);

                    int cellIndex = y * citySize + x;
                    char cellChar = cityMap[cellIndex];

                    SetCityCellColor(cellChar);
                    Console.Write(cellChar);
                    Console.ResetColor();

                    Console.SetCursorPosition(savedCursorLeft, savedCursorTop + 2);
                }
            }
            catch
            {
                return;
            }
        }
        private static ConsoleColor GetCityCellBackgroundColor(char cellChar)
        {
            if (cellChar == SityMapObjects["Border"])
            {
                return ConsoleColor.DarkGray;
            }
            else if (cellChar == SityMapObjects["VerticalRoad"] ||
                     cellChar == SityMapObjects["HorizontalRoad"] ||
                     cellChar == SityMapObjects["Crossroad"])
            {
                return ConsoleColor.DarkGray;
            }
            else if (cellChar == SityMapObjects["CivilianFactory"])
            {
                return ConsoleColor.Cyan;
            }
            else if (cellChar == SityMapObjects["MilitaryFactory"])
            {
                return ConsoleColor.DarkRed;
            }
            else if (cellChar == SityMapObjects["House"])
            {
                return ConsoleColor.DarkYellow;
            }
            else if (cellChar == SityMapObjects["Tree"])
            {
                return ConsoleColor.DarkGreen;
            }
            else if (cellChar == SityMapObjects["Land"])
            {
                return ConsoleColor.Green;
            }

            return ConsoleColor.Black;
        }
        private static City FindCityAtPosition(int x, int y, List<Country> countries, int mapSize)
        {
            foreach (var country in countries)
            {
                foreach (var city in country.CountryСitys)
                {
                    if (city.GetX(mapSize) == x && city.GetY(mapSize) == y)
                    {
                        return city;
                    }
                }
            }
            return null;
        }
        private static bool IsCityOwnedByPlayer(Game game, City city)
        {
            foreach (var country in game.GameCountries)
            {
                if (country.CountryСitys != null && country.CountryСitys.Contains(city))
                {
                    return !country.IsBot;
                }
            }
            return false;
        }
        private static Country FindCountryAtPosition(int x, int y, List<Country> countries, int mapSize)
        {
            int cellIndex = y * mapSize + x;

            foreach (var country in countries)
            {
                if (country.CountryTerritory != null && country.CountryTerritory.Contains(cellIndex))
                {
                    return country;
                }
                if (country.CountryPorts != null)
                {
                    foreach (var port in country.CountryPorts)
                    {
                        if (port.PortPosition == cellIndex)
                        {
                            return country;
                        }
                    }
                }
            }
            return null;
        }
        public static string CreateCountryInfoString(Game game, Country country)
        {
            StringBuilder countryInfo = new StringBuilder();

            string header = $"--{country.CountryName.ToUpper()}--";
            countryInfo.AppendLine(CenterText(header));
            countryInfo.AppendLine();

            countryInfo.AppendLine(CenterText($"Ideology: {GetIdeologyName(country.CountryIdeology)}"));
            countryInfo.AppendLine(CenterText($"Color: {country.CountryColor}"));
            countryInfo.AppendLine(CenterText($"Population: {country.CountryPopulation}"));
            countryInfo.AppendLine(CenterText($"Money: ${country.CountryMoney}"));
            countryInfo.AppendLine(CenterText($"Territory: {country.CountryTerritory?.Count ?? 0} cells"));
            countryInfo.AppendLine(CenterText($"Cities: {country.CountryСitys?.Count ?? 0}"));
            countryInfo.AppendLine(CenterText($"Ports: {country.CountryPorts?.Count ?? 0}"));

            countryInfo.AppendLine();
            string warStatusHeader = "--WAR STATUS--";
            countryInfo.AppendLine(CenterText(warStatusHeader));
            countryInfo.AppendLine();

            if (country.WarWithCountries != null && country.WarWithCountries.Count > 0)
            {
                countryInfo.AppendLine(CenterText($"At war with {country.WarWithCountries.Count} countries:"));
                foreach (var enemy in country.WarWithCountries)
                {
                    countryInfo.AppendLine(CenterText($"  • {enemy.CountryName}"));
                }
            }
            else
            {
                countryInfo.AppendLine(CenterText("Not at war"));
            }

            if (country.Allies != null && country.Allies.Count > 0)
            {
                countryInfo.AppendLine();
                countryInfo.AppendLine(CenterText($"Allies: {country.Allies.Count} countries"));
                foreach (var ally in country.Allies)
                {
                    countryInfo.AppendLine(CenterText($"  • {ally.CountryName}"));
                }
            }

            countryInfo.AppendLine();

            string ammunitionHeader = "--ARMAMENT--";
            countryInfo.AppendLine(CenterText(ammunitionHeader));
            countryInfo.AppendLine();

            if (country.MilitaryAmmunitionDepot != null && country.MilitaryAmmunitionDepot.Count > 0)
            {
                foreach (var ammo in country.MilitaryAmmunitionDepot)
                {
                    countryInfo.AppendLine(CenterText($"{ammo.Key}: {ammo.Value}"));
                }
            }
            else
            {
                countryInfo.AppendLine(CenterText("No data"));
            }

            countryInfo.AppendLine();

            string industryHeader = "--INDUSTRY--";
            countryInfo.AppendLine(CenterText(industryHeader));
            countryInfo.AppendLine();

            int civilianFactories = 0;
            int militaryFactories = 0;

            if (country.CountryСitys != null)
            {
                foreach (var city in country.CountryСitys)
                {
                    civilianFactories += city.CivilianFactories?.Count ?? 0;
                    militaryFactories += city.MilitaryFactories?.Count ?? 0;
                }
            }

            countryInfo.AppendLine(CenterText($"Civilian factories: {civilianFactories}"));
            countryInfo.AppendLine(CenterText($"Military factories: {militaryFactories}"));

            return countryInfo.ToString();
        }

        private static string CenterText(string text)
        {
            int consoleWidth = Console.WindowWidth;

            string trimmedText = text.TrimEnd();
            int textLength = trimmedText.Length;

            if (textLength >= consoleWidth)
            {
                return trimmedText.Length > consoleWidth ?
                    trimmedText.Substring(0, consoleWidth - 3) + "..." :
                    trimmedText;
            }

            int leftPadding = (consoleWidth - textLength) / 2;
            return new string(' ', Math.Max(0, leftPadding)) + trimmedText;
        }

        private static string GetIdeologyName(IdeologiesEnum ideology)
        {
            return ideology switch
            {
                IdeologiesEnum.LeftDemocracy => "Left democracy",
                IdeologiesEnum.RightDemocracy => "Right democracy",
                IdeologiesEnum.Сommunism => "Communism",
                IdeologiesEnum.Fascism => "Fascism",
                IdeologiesEnum.Monarchism => "Monarchism",
                _ => "Unknown"
            };
        }
        private static bool IsValidArmyPlacementTerrain(char cellChar)
        {
            return cellChar == Game.MapObjects["Land"] ||
                   cellChar == Game.MapObjects["Desert"] ||
                   cellChar == Game.MapObjects["Forest"] ||
                   cellChar == Game.MapObjects["Mountain"] ||
                   cellChar == Game.MapObjects["Hill"];
        }

        private static void DisplayMovementInfo(ArmyDivision division, int cursorX, int cursorY, int mapSize)
        {
            int savedCursorLeft = Console.CursorLeft;
            int savedCursorTop = Console.CursorTop;

            try
            {
                Console.SetCursorPosition(0, savedCursorTop + 2);
                Console.ForegroundColor = ConsoleColor.Yellow;

                int divisionX = division.Position % mapSize;
                int divisionY = division.Position / mapSize;
                int distance = Math.Abs(cursorX - divisionX) + Math.Abs(cursorY - divisionY);
                int maxDistance = GetMaxMovementDistance(division.DivisionType);

                Console.WriteLine($"Moving {division.DivisionType}");
                Console.WriteLine($"Current position: ({divisionX}, {divisionY})");
                Console.WriteLine($"Selected position: ({cursorX}, {cursorY})");
                Console.WriteLine($"Distance: {distance}/{maxDistance} cells");
                Console.WriteLine("\nPress ENTER to confirm move");
                Console.WriteLine("Press ESC to cancel");

                Console.ResetColor();
                Console.SetCursorPosition(savedCursorLeft, savedCursorTop);
            }
            catch
            {
                Console.SetCursorPosition(savedCursorLeft, savedCursorTop);
            }
        }
        public static bool IsNearWater(List<char> map, int x, int y, int mapSize)
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
                        if (map[neighborIndex] == Game.MapObjects["Sea"])
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private static bool CanBuildSettlementAt(char cellChar)
        {
            return cellChar == Game.MapObjects["Land"] ||
                   cellChar == Game.MapObjects["Forest"] ||
                   cellChar == Game.MapObjects["Desert"] ||
                   cellChar == Game.MapObjects["Hill"];
        }
        private static Port FindPortAtPosition(int x, int y, Country country, Game game)
        {
            int cellIndex = y * game.MapSize + x;

            foreach (var port in country.CountryPorts)
            {
                if (port.PortPosition == cellIndex)
                    return port;
            }
            return null;
        }
        public static void ShowGameSpaceForShipMovement(Game game, Warship.WarshipMovementInfo movementInfo)
        {
            Map gameMap = game.Map;
            List<Country> countries = game.GameCountries;
            int mapSize = game.MapSize;

            Warship ship = movementInfo.Ship;
            int shipX = ship.Position % mapSize;
            int shipY = ship.Position / mapSize;

            int cursorX = shipX;
            int cursorY = shipY;

            var cellToCountry = new Dictionary<int, Country>();
            foreach (var country in countries)
            {
                foreach (var cell in country.CountryTerritory)
                {
                    cellToCountry[cell] = country;
                }
            }

            Console.CursorVisible = false;

            while (true)
            {
                Console.Clear();

                int consoleWidth = Console.WindowWidth;
                int mapDisplayWidth = mapSize * 2 - 1;
                int leftPadding = (consoleWidth - mapDisplayWidth) / 2;

                int prevCursorX = cursorX;
                int prevCursorY = cursorY;

                ConsoleWriteMapWithCountries(gameMap.MetaMap, mapSize, countries);

                HighlightAvailableShipMovementCells(game, ship, mapSize, leftPadding);

                if (prevCursorX != -1 && prevCursorY != -1)
                {
                    UpdateCellWithShipHighlight(game, ship, prevCursorX, prevCursorY, mapSize, leftPadding, countries, cellToCountry);
                }

                DrawShipMovementCursor(cursorX, cursorY, leftPadding, gameMap.MetaMap, mapSize, cellToCountry, game, ship);

                while (true)
                {
                    ConsoleKeyInfo key = ConsoleManager.ListenKays();

                    if (key.Key == ConsoleKey.UpArrow && cursorY > 1)
                    {
                        if (prevCursorX != -1 && prevCursorY != -1)
                        {
                            UpdateCellWithShipHighlight(game, ship, prevCursorX, prevCursorY, mapSize, leftPadding, countries, cellToCountry);
                        }
                        cursorY--;
                        DrawShipMovementCursor(cursorX, cursorY, leftPadding, gameMap.MetaMap, mapSize, cellToCountry, game, ship);
                        prevCursorX = cursorX;
                        prevCursorY = cursorY;
                    }
                    else if (key.Key == ConsoleKey.DownArrow && cursorY < mapSize - 2)
                    {
                        if (prevCursorX != -1 && prevCursorY != -1)
                        {
                            UpdateCellWithShipHighlight(game, ship, prevCursorX, prevCursorY, mapSize, leftPadding, countries, cellToCountry);
                        }
                        cursorY++;
                        DrawShipMovementCursor(cursorX, cursorY, leftPadding, gameMap.MetaMap, mapSize, cellToCountry, game, ship);
                        prevCursorX = cursorX;
                        prevCursorY = cursorY;
                    }
                    else if (key.Key == ConsoleKey.LeftArrow && cursorX > 1)
                    {
                        if (prevCursorX != -1 && prevCursorY != -1)
                        {
                            UpdateCellWithShipHighlight(game, ship, prevCursorX, prevCursorY, mapSize, leftPadding, countries, cellToCountry);
                        }
                        cursorX--;
                        DrawShipMovementCursor(cursorX, cursorY, leftPadding, gameMap.MetaMap, mapSize, cellToCountry, game, ship);
                        prevCursorX = cursorX;
                        prevCursorY = cursorY;
                    }
                    else if (key.Key == ConsoleKey.RightArrow && cursorX < mapSize - 2)
                    {
                        if (prevCursorX != -1 && prevCursorY != -1)
                        {
                            UpdateCellWithShipHighlight(game, ship, prevCursorX, prevCursorY, mapSize, leftPadding, countries, cellToCountry);
                        }
                        cursorX++;
                        DrawShipMovementCursor(cursorX, cursorY, leftPadding, gameMap.MetaMap, mapSize, cellToCountry, game, ship);
                        prevCursorX = cursorX;
                        prevCursorY = cursorY;
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        int targetCellIndex = cursorY * mapSize + cursorX;

                        if (Warship.MoveShip(game, ship, targetCellIndex))
                        {
                            ShowGameSpace(game);
                            return;
                        }
                        else
                        {
                            ShowGameSpace(game);
                            return;
                        }
                    }
                    else if (key.Key == ConsoleKey.Escape)
                    {
                        ShowGameSpace(game);
                        return;
                    }
                }
            }
        }
        private static void DrawShipMovementCursor(int x, int y, int leftPadding, List<char> map, int mapSize,
                                   Dictionary<int, Country> cellToCountry, Game game, Warship ship)
        {
            try
            {
                int savedCursorLeft = Console.CursorLeft;
                int savedCursorTop = Console.CursorTop;

                Console.SetCursorPosition(0, savedCursorTop - mapSize);

                int cursorScreenX = leftPadding + x * 2;
                int cursorScreenY = savedCursorTop - mapSize + y;

                Console.SetCursorPosition(cursorScreenX, cursorScreenY);

                int cellIndex = y * mapSize + x;
                char cellChar = map[cellIndex];

                bool canMove = Warship.CanMoveToPosition(game, ship, cellIndex, game.GameCountries, mapSize);
                bool occupied = Warship.IsCellOccupiedByShip(game, cellIndex, ship);

                ConsoleColor backgroundColor;
                if (occupied || !canMove)
                {
                    backgroundColor = ConsoleColor.DarkRed;
                }
                else
                {
                    backgroundColor = ConsoleColor.DarkBlue;
                }

                ConsoleColor foregroundColor = ConsoleColor.Black;

                if (cellChar == Game.MapObjects["CargoShip"] || cellChar == Game.MapObjects["CombatShip"] || cellChar == Game.MapObjects["Capital"] || cellChar == Game.MapObjects["City"] || cellChar == Game.MapObjects["Port"])
                {
                    foregroundColor = ConsoleColor.White;
                }
                else if (cellChar == Game.MapObjects["Sea"])
                {
                    foregroundColor = ConsoleColor.White;
                }
                else
                {
                    if (cellToCountry.TryGetValue(cellIndex, out var country))
                    {
                        foregroundColor = country.CountryColor;
                    }
                    else
                    {
                        foregroundColor = ConsoleColor.Black;
                    }
                }

                Console.BackgroundColor = backgroundColor;
                Console.ForegroundColor = foregroundColor;
                Console.Write(cellChar);
                Console.ResetColor();

                Console.SetCursorPosition(savedCursorLeft, savedCursorTop);
            }
            catch
            {
                return;
            }
        }

        private static void UpdateCellWithShipHighlight(Game game, Warship ship, int x, int y, int mapSize,
                                                      int leftPadding, List<Country> countries,
                                                      Dictionary<int, Country> cellToCountry)
        {
            try
            {
                int savedCursorLeft = Console.CursorLeft;
                int savedCursorTop = Console.CursorTop;

                Console.SetCursorPosition(0, savedCursorTop - mapSize);

                int screenX = leftPadding + x * 2;
                int screenY = savedCursorTop - mapSize + y;

                Console.SetCursorPosition(screenX, screenY);

                int cellIndex = y * mapSize + x;
                char cellChar = game.Map.MetaMap[cellIndex];

                bool canMove = Warship.CanMoveToPosition(game, ship, cellIndex, countries, mapSize);
                bool occupied = Warship.IsCellOccupiedByShip(game, cellIndex, ship);

                if (canMove && !occupied)
                {
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(cellChar);
                    Console.ResetColor();
                }
                else
                {
                    if (cellChar == Game.MapObjects["CargoShip"] || cellChar == Game.MapObjects["CombatShip"] || cellChar == Game.MapObjects["Capital"] || cellChar == Game.MapObjects["City"] || cellChar == Game.MapObjects["Port"])
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(cellChar);
                    }
                    else if (cellChar == Game.MapObjects["Sea"])
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(cellChar);
                    }
                    else
                    {
                        if (cellToCountry.TryGetValue(cellIndex, out var country))
                        {
                            Console.ForegroundColor = country.CountryColor;
                            Console.Write(cellChar);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write(Game.MapObjects["Sea"]);
                        }
                    }
                }

                Console.ResetColor();
                Console.SetCursorPosition(savedCursorLeft, savedCursorTop);
            }
            catch
            {
                return;
            }
        }

        private static void HighlightAvailableShipMovementCells(Game game, Warship ship, int mapSize, int leftPadding)
        {
            int maxDistance = Warship.GetMaxMovementDistance(ship.ShipType);
            int centerX = ship.Position % mapSize;
            int centerY = ship.Position / mapSize;

            int savedCursorTop = Console.CursorTop;
            int mapStartY = savedCursorTop - mapSize;

            for (int y = Math.Max(1, centerY - maxDistance); y <= Math.Min(mapSize - 2, centerY + maxDistance); y++)
            {
                for (int x = Math.Max(1, centerX - maxDistance); x <= Math.Min(mapSize - 2, centerX + maxDistance); x++)
                {
                    int distance = Math.Abs(x - centerX) + Math.Abs(y - centerY);
                    if (distance <= maxDistance && distance > 0)
                    {
                        int cellIndex = y * mapSize + x;

                        bool canMove = Warship.CanMoveToPosition(game, ship, cellIndex, game.GameCountries, mapSize);
                        bool occupied = Warship.IsCellOccupiedByShip(game, cellIndex, ship);

                        ConsoleColor highlightColor;
                        if (occupied)
                        {
                            highlightColor = ConsoleColor.DarkRed;
                        }
                        else if (canMove)
                        {
                            highlightColor = ConsoleColor.DarkBlue;
                        }
                        else
                        {
                            continue;
                        }

                        int screenX = leftPadding + x * 2;
                        int screenY = mapStartY + y;

                        try
                        {
                            Console.SetCursorPosition(screenX, screenY);
                            Console.BackgroundColor = highlightColor;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.Write(game.Map.MetaMap[cellIndex]);
                            Console.ResetColor();
                        }
                        catch { }
                    }
                }
            }

            Console.SetCursorPosition(0, savedCursorTop);
        }
    }
}
