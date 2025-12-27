using System;
using System.Collections.Generic;
using System.Text;
using static ConsoleApp1.Army;
using static ConsoleApp1.City;
using static ConsoleApp1.City.BuildingSystem;

namespace ConsoleApp1
{
    public static class MenuManger
    {
        static GameSetup gameSetup = new GameSetup("NewGame", 10, "MyCountry", "MyCapital", Country.IdeologiesEnum.LeftDemocracy, ConsoleColor.Blue, 50);

        public static Menu MainMenu = new Menu(MainMenuAction, null, null, "--The Another World War--", "New Game", "Load Game", "Exit");
        public static Menu GameCreatingMenu => new Menu(GameCreatingMenuAction, null, null, "--CREATE A GAME--", $"Game name: {gameSetup.SetupGameName}", "Country creator", "Game settings", "Start");
        public static Menu GameSettingsMenu => new Menu(GameSettingsMenuAction, null, null, "--GAME SETTINGS--", $"Countries count: {gameSetup.SetupCountriesCount}", $"Map size: {gameSetup.SetupMapSize}");
        public static Menu CreateCountryMenu => new Menu(CreateCountryMenuAction, null, null, "--CREATE YOUR COUNTRY--", $"Сountry name: {gameSetup.SetupPlayerСountryName}", $"Capital name: {gameSetup.SetupPlayerCapitalName}", $"Сountry ideology: {gameSetup.SetupPlayerСountryIdeology.ToString()}", $"Сountry color: {gameSetup.SetupPlayerСountryСolor.ToString()}");

        public static Menu GameNameMenuAsk => new Menu(null, GameNameMenuAskAction, null, $"Choose the name of your game (Now it`s {gameSetup.SetupGameName}).");
        public static Menu CountriesCountMenuAsk => new Menu(null, CountriesCountMenuAskAction, null, $"Choose how many countries are in the game (Now it`s {gameSetup.SetupCountriesCount}).");
        public static Menu MapSizeMenuAsk => new Menu(null, MapSizeMenuAskAction, null, $"Choose the size of a game map (Now it`s {gameSetup.SetupMapSize}).");
        public static Menu СountryNameMenuAsk => new Menu(null, СountryNameMenuAskAction, null, $"Choose the name of your country (Now it`s {gameSetup.SetupPlayerСountryName}).");
        public static Menu CapitalNameMenuAsk => new Menu(null, CapitalNameMenuAskAction, null, $"Choose the name of capital in your country (Now it`s {gameSetup.SetupPlayerCapitalName}).");
        public static Menu СountryIdeologyMenuList => new Menu(СountryIdeologyMenuListAction, null, null, $"Choose the ideology of your country (Now it`s {gameSetup.SetupPlayerСountryIdeology}).", "Left democracy", "Right democracy", "Сommunism", "Fascism", "Monarchism");
        public static Menu СountryСolorMenuList => new Menu(СountryСolorMenuListAction, null, null, $"Choose the color of your country (Now it`s {gameSetup.SetupPlayerСountryСolor}).", "DarkBlue", "DarkGreen", "DarkCyan", "DarkRed", "DarkMagenta", "DarkYellow", "DarkGray", "Blue", "Green", "Cyan", "Red", "Magenta", "Yellow");

        public static Menu InGameMenuEscList = new Menu(null, null, InGameMenuEscListAction, $"--PAUSE--", "Continue", "Exit To Menu");

        public static Menu InGameMenuNextTurnQuestList = new Menu(null, null, InGameMenuNextTurnQuestListAction, $"Are you sure you want to make the next turn?", "Yes", "No");
        public static Menu InGameBuildSettlementMenuLogic => new Menu(null, null, InGameBuildSettlementMenuAction, "--BUILD SETTLEMENT--", "Build City ($10000)", "Build Port ($5000)", "Back");
        public static Menu InGameLandCellActionMenu => new Menu(null, null, InGameLandCellActionMenuAction, "What do you want to do?", "Place an army", "Build a settlement", "Back");
        public static Menu InGamePortActionMenu => new Menu(null, null, InGamePortActionMenuAction, "--PORT MANAGEMENT--", "Build Civilian Ship ($100)", "Build Military Ship", "Back");
        public static Menu InGameMilitaryShipTypeMenu => new Menu(null, null, InGameMilitaryShipTypeMenuAction, "Select military ship type:", $"Cargo Ship ($500) - Available: {{0}}/2", $"Combat Ship ($1000) - Available: {{0}}/5", "Back");
        public static Menu InGameWarshipMenuLogic => new Menu(null, null, InGameWarshipMenuAction, "--SHIP MANAGEMENT--", "Move", "Disband", "Back");
        public static Menu InGameWarshipDisbandConfirmationMenu => new Menu(null, null, InGameWarshipDisbandConfirmationAction, $"Are you sure you want to disband this ship? You will recover 50% of resources.", "Yes, disband", "No, cancel");

        public static void SetMenu(Menu menu)
        {
            int ans = ConsoleManager.ConsoleKeys(menu.Title, menu.Buttons);
            menu.ActionInt?.Invoke(ans);
        }
        public static void SetMenuAsk(Menu menu)
        {
            string ans = ConsoleManager.ConsoleAsk(menu.Title);
            menu.ActionStr?.Invoke(ans);
        }
        public static void InGameSetMenu(Menu menu, Game game, object additionalData = null)
        {
            int ans = ConsoleManager.ConsoleKeys(menu.Title, menu.Buttons);
            menu.ActionInGame?.Invoke(ans, game, additionalData);
        }

        public static Menu CreateDynamicInGameMenu(Action<int, Game, object> action, string title, List<string> buttons)
        {
            Menu InGameMenu = new Menu(
                null,
                null,
                action,
                title,
                buttons.ToArray());
            return InGameMenu;
        }

        public static Menu CreateDynamicInGameMenu(Action<int, Game, object> action, string title, params string[] buttons)
        {
            Menu InGameMenu = new Menu(
                null,
                null,
                action,
                title,
                buttons.ToArray());
            return InGameMenu;
        }

        public static void MainMenuAction(int ans)
        {
            gameSetup = new GameSetup("NewGame", 10, "MyCountry", "MyCapital", Country.IdeologiesEnum.LeftDemocracy, ConsoleColor.Blue, 50);
            if (ans == 0)
            {
                SetMenu(GameCreatingMenu);
            }
            else if (ans == 1)
            {

            }
            else if (ans == 2)
            {
                Environment.Exit(0);
            }
            else if (ans == -1)
            {
                SetMenu(MainMenu);
            }
        }
        public static void GameCreatingMenuAction(int ans)
        {
            if (ans == 0)
            {
                SetMenuAsk(GameNameMenuAsk);
            }
            else if (ans == 1)
            {
                SetMenu(CreateCountryMenu);
            }
            else if (ans == 2)
            {
                SetMenu(GameSettingsMenu);
            }
            else if (ans == 3)
            {
                Game game = new Game(gameSetup);
                game.InitializeGame();
            }
            else if (ans == -1)
            {
                SetMenu(MainMenu);
            }
        }
        public static void GameSettingsMenuAction(int ans)
        {
            if (ans == 0)
            {
                SetMenuAsk(CountriesCountMenuAsk);
            }
            else if (ans == 1)
            {
                SetMenuAsk(MapSizeMenuAsk);
            }
            else if (ans == -1)
            {
                SetMenu(GameCreatingMenu);
            }
        }
        public static void CreateCountryMenuAction(int ans)
        {
            if (ans == 0)
            {
                SetMenuAsk(СountryNameMenuAsk);
            }
            else if (ans == 1)
            {
                SetMenuAsk(CapitalNameMenuAsk);
            }
            else if (ans == 2)
            {
                SetMenu(СountryIdeologyMenuList);
            }
            else if (ans == 3)
            {
                SetMenu(СountryСolorMenuList);
            }
            else if (ans == -1)
            {
                SetMenu(GameCreatingMenu);
            }
        }

        public static void GameNameMenuAskAction(string ans)
        {
            if (ans == "")
            {
                SetMenu(GameCreatingMenu);
            }
            else
            {
                gameSetup.SetupGameName = ans;
                SetMenu(GameCreatingMenu);
            }
        }

        public static void CountriesCountMenuAskAction(string ans)
        {
            if (ans == "")
            {
                SetMenu(GameSettingsMenu);
            }
            else
            {
                int ansInt = 10;
                try
                {
                    ansInt = int.Parse(ans);
                }
                catch
                {
                    ConsoleManager.ErrorMassege("Please enter a NUMBER!");
                    SetMenuAsk(CountriesCountMenuAsk);
                }
                gameSetup.SetupCountriesCount = ansInt;
                SetMenu(GameSettingsMenu);
            }
        }
        public static void MapSizeMenuAskAction(string ans)
        {
            if (ans == "")
            {
                SetMenu(GameSettingsMenu);
            }
            else
            {
                int ansInt = 50;
                try
                {
                    ansInt = int.Parse(ans);
                }
                catch
                {
                    ConsoleManager.ErrorMassege("Please enter a NUMBER!");
                    SetMenuAsk(MapSizeMenuAsk);
                }
                gameSetup.SetupMapSize = ansInt;
                SetMenu(GameSettingsMenu);
            }
        }
        public static void СountryNameMenuAskAction(string ans)
        {
            if (ans == "")
            {
                SetMenu(CreateCountryMenu);
            }
            else
            {
                gameSetup.SetupPlayerСountryName = ans;
                SetMenu(CreateCountryMenu);
            }
        }
        public static void CapitalNameMenuAskAction(string ans)
        {
            if (ans == "")
            {
                SetMenu(CreateCountryMenu);
            }
            else
            {
                gameSetup.SetupPlayerCapitalName = ans;
                SetMenu(CreateCountryMenu);
            }
        }
        public static void СountryIdeologyMenuListAction(int ans)
        {
            if (ans == 0)
            {
                gameSetup.SetupPlayerСountryIdeology = Country.IdeologiesEnum.LeftDemocracy;
                SetMenu(CreateCountryMenu);
            }
            else if (ans == 1)
            {
                gameSetup.SetupPlayerСountryIdeology = Country.IdeologiesEnum.RightDemocracy;
                SetMenu(CreateCountryMenu);
            }
            else if (ans == 2)
            {
                gameSetup.SetupPlayerСountryIdeology = Country.IdeologiesEnum.Сommunism;
                SetMenu(CreateCountryMenu);
            }
            else if (ans == 3)
            {
                gameSetup.SetupPlayerСountryIdeology = Country.IdeologiesEnum.Fascism;
                SetMenu(CreateCountryMenu);
            }
            else if (ans == 4)
            {
                gameSetup.SetupPlayerСountryIdeology = Country.IdeologiesEnum.Monarchism;
                SetMenu(CreateCountryMenu);
            }
            else if (ans == -1)
            {
                SetMenu(CreateCountryMenu);
            }
        }
        public static void СountryСolorMenuListAction(int ans)
        {
            if (ans == 0)
            {
                gameSetup.SetupPlayerСountryСolor = ConsoleColor.DarkBlue;
                SetMenu(CreateCountryMenu);
            }
            else if (ans == 1)
            {
                gameSetup.SetupPlayerСountryСolor = ConsoleColor.DarkGreen;
                SetMenu(CreateCountryMenu);
            }
            else if (ans == 2)
            {
                gameSetup.SetupPlayerСountryСolor = ConsoleColor.DarkCyan;
                SetMenu(CreateCountryMenu);
            }
            else if (ans == 3)
            {
                gameSetup.SetupPlayerСountryСolor = ConsoleColor.DarkRed;
                SetMenu(CreateCountryMenu);
            }
            else if (ans == 4)
            {
                gameSetup.SetupPlayerСountryСolor = ConsoleColor.DarkMagenta;
                SetMenu(CreateCountryMenu);
            }
            else if (ans == 5)
            {
                gameSetup.SetupPlayerСountryСolor = ConsoleColor.DarkYellow;
                SetMenu(CreateCountryMenu);
            }
            else if (ans == 6)
            {
                gameSetup.SetupPlayerСountryСolor = ConsoleColor.DarkGray;
                SetMenu(CreateCountryMenu);
            }
            else if (ans == 7)
            {
                gameSetup.SetupPlayerСountryСolor = ConsoleColor.Blue;
                SetMenu(CreateCountryMenu);
            }
            else if (ans == 8)
            {
                gameSetup.SetupPlayerСountryСolor = ConsoleColor.Green;
                SetMenu(CreateCountryMenu);
            }
            else if (ans == 9)
            {
                gameSetup.SetupPlayerСountryСolor = ConsoleColor.Cyan;
                SetMenu(CreateCountryMenu);
            }
            else if (ans == 10)
            {
                gameSetup.SetupPlayerСountryСolor = ConsoleColor.Red;
                SetMenu(CreateCountryMenu);
            }
            else if (ans == 11)
            {
                gameSetup.SetupPlayerСountryСolor = ConsoleColor.Magenta;
                SetMenu(CreateCountryMenu);
            }
            else if (ans == 12)
            {
                gameSetup.SetupPlayerСountryСolor = ConsoleColor.Yellow;
                SetMenu(CreateCountryMenu);
            }
            else if (ans == -1)
            {
                SetMenu(CreateCountryMenu);
            }
        }
        public static void InGameMenuEscListAction(int ans, Game currentGame, object additionalData)
        {
            if (ans == 0)
            {
                GameSpace.ShowGameSpace(currentGame);
            }
            else if (ans == 1)
            {
                SetMenu(MainMenu);
            }
            else if (ans == -1)
            {
                GameSpace.ShowGameSpace(currentGame);
            }
        }
        public static void InGameMenuNextTurnQuestListAction(int ans, Game currentGame, object additionalData)
        {
            if (ans == 0)
            {
                currentGame.NextTurn();
            }
            else if (ans == 1)
            {
                GameSpace.ShowGameSpace(currentGame);
            }
            else if (ans == -1)
            {
                GameSpace.ShowGameSpace(currentGame);
            }
        }
        public static void InGameBuildingManipulationListAction(int ans, Game currentGame, object additionalData)
        {
            if (additionalData is MilitaryFactory militaryfactory)
            {
                City city = MilitaryFactory.FindCityWithFactory(currentGame, militaryfactory);

                if (ans == 0)
                {
                    InGameSetMenu(CreateDynamicInGameMenu(InGameMilitaryFactoryProductsListAction,
                        $"Select a product that will be manufactured at this factory (Now it's {militaryfactory.CurrentProduct}).",
                        MilitaryFactory.MilitaryFactoryProducts), currentGame, militaryfactory);
                }
                else if (ans == 1)
                {
                    InGameSetMenu(CreateDynamicInGameMenu(InGameConfirmDemolitionAction,
                        $"Are you sure you want to demolish this military factory? This action is irreversible.",
                        "Yes", "No"), currentGame, militaryfactory);
                }
                else if (ans == -1)
                {
                    Console.Clear();
                    GameSpace.ShowCityMap(city, currentGame);
                }
            }
            else if (additionalData is CivilianFactory civilianfactory)
            {
                City city = CivilianFactory.FindCityWithFactory(currentGame, civilianfactory);

                if (ans == 0)
                {
                    InGameSetMenu(CreateDynamicInGameMenu(InGameConfirmDemolitionAction,
                        $"Are you sure you want to demolish this civilian factory? This action is irreversible.",
                        "Yes", "No"), currentGame, civilianfactory);
                }
                else if (ans == -1)
                {
                    Console.Clear();
                    GameSpace.ShowCityMap(city, currentGame);
                }
            }
            else if (additionalData is House house)
            {
                City city = City.House.FindCityWithHouse(currentGame, house);

                if (ans == 0)
                {
                    InGameSetMenu(CreateDynamicInGameMenu(InGameConfirmDemolitionAction,
                        $"Are you sure you want to demolish this house? This action is irreversible.",
                        "Yes", "No"), currentGame, house);
                }
                else if (ans == -1)
                {
                    Console.Clear();
                    GameSpace.ShowCityMap(city, currentGame);
                }
            }
            else if (additionalData is Road.RoadInfo roadInfo)
            {
                City city = roadInfo.City;
                int roadPosition = roadInfo.Position;

                if (ans == 0)
                {
                    InGameSetMenu(CreateDynamicInGameMenu(InGameConfirmDemolitionAction,
                        $"Are you sure you want to demolish this road? This action is irreversible.",
                        "Yes", "No"), currentGame, roadInfo);
                }
                else if (ans == -1)
                {
                    Console.Clear();
                    GameSpace.ShowCityMap(city, currentGame);
                }
            }
        }
        public static void InGameConfirmDemolitionAction(int ans, Game currentGame, object additionalData)
        {
            if (additionalData is MilitaryFactory militaryfactory)
            {
                City city = MilitaryFactory.FindCityWithFactory(currentGame, militaryfactory);

                if (ans == 0)
                {
                    MilitaryFactory.DemolishMilitaryFactory(currentGame, city, militaryfactory);
                }
                else if (ans == 1 || ans == -1)
                {
                    Console.Clear();
                    GameSpace.ShowCityMap(city, currentGame);
                }
            }
            else if (additionalData is CivilianFactory civilianfactory)
            {
                City city = CivilianFactory.FindCityWithFactory(currentGame, civilianfactory);

                if (ans == 0)
                {
                    CivilianFactory.DemolishMilitaryFactory(currentGame, city, civilianfactory);
                }
                else if (ans == 1 || ans == -1)
                {
                    Console.Clear();
                    GameSpace.ShowCityMap(city, currentGame);
                }
            }
            else if (additionalData is House house)
            {
                City city = House.FindCityWithHouse(currentGame, house);

                if (ans == 0)
                {
                    House.DemolishHouse(currentGame, city, house);
                }
                else if (ans == 1 || ans == -1)
                {
                    Console.Clear();
                    GameSpace.ShowCityMap(city, currentGame);
                }
            }
            else if (additionalData is Road.RoadInfo roadInfo)
            {
                City city = roadInfo.City;

                if (ans == 0)
                {
                    Road.DemolishRoad(currentGame, city, roadInfo.Position);
                }
                else if (ans == 1 || ans == -1)
                {
                    Console.Clear();
                    GameSpace.ShowCityMap(city, currentGame);
                }
            }
        }
        public static void InGameMilitaryFactoryProductsListAction(int ans, Game currentGame, object additionalData)
        {
            if (additionalData is MilitaryFactory factory)
            {
                City city = MilitaryFactory.FindCityWithFactory(currentGame, factory);

                if (ans >= 0 && ans < MilitaryFactory.MilitaryFactoryProducts.Count)
                {
                    factory.CurrentProduct = MilitaryFactory.MilitaryFactoryProducts[ans];

                    Console.Clear();
                    GameSpace.ShowCityMap(city, currentGame);
                }
                else if (ans == -1)
                {
                    Console.Clear();
                    GameSpace.ShowCityMap(city, currentGame);
                }
            }
        }
        public static void InGameBuildMenuListAction(int ans, Game currentGame, object additionalData)
        {
            if (additionalData is BuildInfo buildInfo)
            {
                City city = buildInfo.City;
                int position = buildInfo.Position;

                if (ans == 0)
                {
                    InGameTryBuildStructureLogic(currentGame, city, position, "MilitaryFactory");
                }
                else if (ans == 1)
                {
                    InGameTryBuildStructureLogic(currentGame, city, position, "CivilianFactory");
                }
                else if (ans == 2)
                {
                    InGameTryBuildStructureLogic(currentGame, city, position, "House");
                }
                else if (ans == 3)
                {
                    InGameSetMenu(CreateDynamicInGameMenu(InGameRoadTypeMenuAction,
                        $"Select road type (Cost: ${BuildingCosts["Road"]})",
                        RoadTypes), currentGame, new BuildInfo(city, position));
                }
                else if (ans == -1)
                {
                    Console.Clear();
                    GameSpace.ShowCityMap(city, currentGame);
                }
            }
        }
        public static void InGameRoadTypeMenuAction(int ans, Game currentGame, object additionalData)
        {
            if (additionalData is BuildInfo buildInfo)
            {
                City city = buildInfo.City;
                int position = buildInfo.Position;

                if (ans >= 0 && ans < RoadTypes.Count)
                {
                    string roadType = RoadTypes[ans];
                    InGameTryBuildStructureLogic(currentGame, city, position, "Road", roadType);
                }
                else if (ans == -1)
                {
                    Console.Clear();
                    GameSpace.ShowCityMap(city, currentGame);
                }
            }
        }
        private static void InGameTryBuildStructureLogic(Game currentGame, City city, int position, string structureType, string roadType = null)
        {
            Country playerCountry = currentGame.GameCountries.FirstOrDefault(c => !c.IsBot);

            if (playerCountry == null)
            {
                ConsoleManager.ErrorMassege("Player country not found!");
                return;
            }

            bool success = BuildStructure(city, position, currentGame, structureType, playerCountry, roadType);

            if (success)
            {
                Console.Clear();
                GameSpace.ShowCityMap(city, currentGame);
            }
            else
            {
                Console.Clear();
                if (structureType == "Road")
                {
                    InGameSetMenu(CreateDynamicInGameMenu(InGameRoadTypeMenuAction,
                        $"Select road type (Cost: ${BuildingCosts["Road"]})",
                        RoadTypes), currentGame, new BuildInfo(city, position));
                }
                else
                {
                    InGameSetMenu(CreateDynamicInGameMenu(InGameBuildMenuListAction,
                        $"Select building to construct (Available money: ${playerCountry.CountryMoney})",
                        BuildableStructures), currentGame, new BuildInfo(city, position));
                }
            }
        }
        public static void InGameCountryPoliticsMenuListAction(int ans, Game currentGame, object additionalData)
        {
            if (additionalData is Country.CountryInfo countryInfo)
            {
                Country selectedCountry = countryInfo.SelectedCountry;
                Country playerCountry = countryInfo.PlayerCountry;

                if (ans == 0)
                {
                    if (playerCountry.IsAtWarWith(selectedCountry))
                    {
                        MenuManger.InGameSetMenu(CreateDynamicInGameMenu(InGameDeclarePeaceConfirmationAction,
                            $"Are you sure you want to declare peace with {selectedCountry.CountryName}?",
                            "Yes, declare peace", "No, continue war"), currentGame, countryInfo);
                    }
                    else
                    {
                        MenuManger.InGameSetMenu(CreateDynamicInGameMenu(InGameDeclareWarConfirmationAction,
                            $"Are you sure you want to declare war on {selectedCountry.CountryName}?",
                            "Yes, declare war", "No, cancel"), currentGame, countryInfo);
                    }
                }
                else if (ans == 1 || ans == -1)
                {
                    MenuManger.InGameSetMenu(CreateDynamicInGameMenu(InGameCountryInfoMenuListAction,
                        GameSpace.CreateCountryInfoString(currentGame, selectedCountry),
                        "Politics", "Back"), currentGame, countryInfo);
                }
            }
        }

        public static void InGameDeclareWarConfirmationAction(int ans, Game currentGame, object additionalData)
        {
            if (additionalData is Country.CountryInfo countryInfo)
            {
                Country selectedCountry = countryInfo.SelectedCountry;
                Country playerCountry = countryInfo.PlayerCountry;

                if (ans == 0)
                {
                    playerCountry.DeclareWar(selectedCountry);

                    MenuManger.InGameSetMenu(CreateDynamicInGameMenu(InGameCountryInfoMenuListAction,
                        GameSpace.CreateCountryInfoString(currentGame, selectedCountry),
                        "Politics", "Back"), currentGame, countryInfo);
                }
                else if (ans == 1 || ans == -1)
                {
                    MenuManger.InGameSetMenu(CreateDynamicInGameMenu(InGameCountryPoliticsMenuListAction,
                        $"Political actions with {selectedCountry.CountryName}",
                        playerCountry.IsAtWarWith(selectedCountry) ? "Declare peace" : "Declare war",
                        "Back"), currentGame, countryInfo);
                }
            }
        }

        public static void InGameDeclarePeaceConfirmationAction(int ans, Game currentGame, object additionalData)
        {
            if (additionalData is Country.CountryInfo countryInfo)
            {
                Country selectedCountry = countryInfo.SelectedCountry;
                Country playerCountry = countryInfo.PlayerCountry;

                if (ans == 0)
                {
                    playerCountry.DeclarePeace(selectedCountry);

                    MenuManger.InGameSetMenu(CreateDynamicInGameMenu(InGameCountryInfoMenuListAction,
                        GameSpace.CreateCountryInfoString(currentGame, selectedCountry),
                        "Politics", "Back"), currentGame, countryInfo);
                }
                else if (ans == 1 || ans == -1)
                {
                    MenuManger.InGameSetMenu(CreateDynamicInGameMenu(InGameCountryPoliticsMenuListAction,
                        $"Political actions with {selectedCountry.CountryName}",
                        playerCountry.IsAtWarWith(selectedCountry) ? "Declare peace" : "Declare war",
                        "Back"), currentGame, countryInfo);
                }
            }
        }

        public static void InGameCountryInfoMenuListAction(int ans, Game currentGame, object additionalData)
        {
            if (additionalData is Country.CountryInfo countryInfo)
            {
                Country selectedCountry = countryInfo.SelectedCountry;
                Country playerCountry = currentGame.GameCountries.FirstOrDefault(c => !c.IsBot);

                if (selectedCountry == playerCountry)
                {
                    if (ans == 0 || ans == -1)
                    {
                        GameSpace.ShowGameSpace(currentGame);
                    }
                }
                else
                {
                    if (ans == 0)
                    {
                        MenuManger.InGameSetMenu(CreateDynamicInGameMenu(InGameCountryPoliticsMenuListAction,
                            $"Political actions with {selectedCountry.CountryName}",
                            playerCountry.IsAtWarWith(selectedCountry) ? "Declare peace" : "Declare war",
                            "Back"), currentGame, countryInfo);
                    }
                    else if (ans == 1 || ans == -1)
                    {
                        GameSpace.ShowGameSpace(currentGame);
                    }
                }
            }
            else if (additionalData is Country country)
            {
                Country playerCountry = currentGame.GameCountries.FirstOrDefault(c => !c.IsBot);

                Country.CountryInfo countryInfo1 = new Country.CountryInfo
                {
                    SelectedCountry = country,
                    PlayerCountry = playerCountry
                };

                if (country == playerCountry)
                {
                    if (ans == 0 || ans == -1)
                    {
                        GameSpace.ShowGameSpace(currentGame);
                    }
                }
                else
                {
                    if (ans == 0)
                    {
                        MenuManger.InGameSetMenu(CreateDynamicInGameMenu(InGameCountryPoliticsMenuListAction,
                            $"Political actions with {country.CountryName}",
                            playerCountry.IsAtWarWith(country) ? "Declare peace" : "Declare war",
                            "Back"), currentGame, countryInfo1);
                    }
                    else if (ans == 1 || ans == -1)
                    {
                        GameSpace.ShowGameSpace(currentGame);
                    }
                }
            }
        }
        public static Menu InGameArmyPlacementMenuLogic => new Menu(null, null, InGameArmyPlacementMenuAction,
        "--Army Placement--",
        "Create Army",
        "Back");

        public static Menu InGameArmyTypeSelectionMenuLogic => new Menu(null, null, InGameArmyTypeSelectionMenuAction,
            "Select army type.",
            "Infantry Division (500 Rifles, 50 Trucks)",
            "Armored Division (50 Tanks, 100 Trucks)",
            "Mountain Division (250 Rifles)");

        public static void InGameArmyPlacementMenuAction(int ans, Game currentGame, object additionalData)
        {
            if (additionalData is Army.ArmyDivision divisionInfo)
            {
                if (ans == 0)
                {
                    MenuManger.InGameSetMenu(InGameArmyTypeSelectionMenuLogic, currentGame, divisionInfo);
                }
                else if (ans == 1 || ans == -1)
                {
                    GameSpace.ShowGameSpace(currentGame);
                }
            }
        }
        public static void InGameArmyTypeSelectionMenuAction(int ans, Game currentGame, object additionalData)
        {
            if (additionalData is Army.ArmyDivision divisionInfo)
            {
                Country playerCountry = divisionInfo.OwnerCountry;
                int cellIndex = divisionInfo.Position;

                if (ans == 0)
                {
                    InGameCreateArmyDivisionLogic(currentGame, playerCountry, cellIndex, "InfantryDivision",
                        new Dictionary<string, int> { { "Rifle", 500 }, { "Truck", 50 } });
                }
                else if (ans == 1)
                {
                    InGameCreateArmyDivisionLogic(currentGame, playerCountry, cellIndex, "ArmoredDivision",
                        new Dictionary<string, int> { { "Tank", 50 }, { "Truck", 100 } });
                }
                else if (ans == 2)
                {
                    InGameCreateArmyDivisionLogic(currentGame, playerCountry, cellIndex, "MountainDivision",
                        new Dictionary<string, int> { { "Rifle", 250 } });
                }
                else if (ans == -1)
                {
                    InGameShowArmyPlacementMenuLogic(currentGame, cellIndex);
                }
            }
        }
        private static void InGameCreateArmyDivisionLogic(Game currentGame, Country country, int cellIndex,
                                          string divisionType, Dictionary<string, int> requiredResources)
        {
            foreach (var resource in requiredResources)
            {
                if (!country.MilitaryAmmunitionDepot.ContainsKey(resource.Key) ||
                    country.MilitaryAmmunitionDepot[resource.Key] < resource.Value)
                {
                    string resourceName = resource.Key;
                    int available = country.MilitaryAmmunitionDepot.ContainsKey(resource.Key)
                        ? country.MilitaryAmmunitionDepot[resource.Key]
                        : 0;

                    ConsoleManager.ErrorMassege(
                        $"Not enough {resourceName}! Required: {resource.Value}, Available: {available}");

                    GameSpace.ShowGameSpace(currentGame);
                    return;
                }
            }

            foreach (var resource in requiredResources)
            {
                country.MilitaryAmmunitionDepot[resource.Key] -= resource.Value;
            }

            ArmyDivision newDivision = new ArmyDivision
            {
                DivisionType = divisionType,
                Position = cellIndex,
                OwnerCountry = country,
                OriginalTerrain = currentGame.Map.MetaMap[cellIndex]
            };

            if (country.CountryArmy == null)
            {
                country.CountryArmy = new Army();
            }

            if (country.CountryArmy.Divisions == null)
            {
                country.CountryArmy.Divisions = new List<Army.ArmyDivision>();
            }

            country.CountryArmy.Divisions.Add(newDivision);

            currentGame.Map.MetaMap[cellIndex] = Army.ArmyMapObjects[divisionType];

            GameSpace.ShowGameSpace(currentGame);
        }

        public static void InGameShowArmyPlacementMenuLogic(Game game, int cellIndex)
        {
            Country playerCountry = game.GameCountries.FirstOrDefault(c => !c.IsBot);
            if (playerCountry == null) return;

            var divisionInfo = new Army.ArmyDivision
            {
                OwnerCountry = playerCountry,
                Position = cellIndex
            };

            InGameSetMenu(InGameArmyPlacementMenuLogic, game, divisionInfo);
        }
        public static Menu InGameArmyDivisionMenuLogic => new Menu(null, null, InGameArmyDivisionMenuAction,
            "--Division Management--",
            "Move",
            "Disband",
            "Back");

        public static void InGameArmyDivisionMenuAction(int ans, Game currentGame, object additionalData)
        {
            if (additionalData is Army.ArmyDivisionMenuInfo divisionInfo)
            {
                if (ans == 0)
                {
                    var movementInfo = new Army.ArmyMovementInfo
                    {
                        Division = divisionInfo.Division,
                        PlayerCountry = divisionInfo.PlayerCountry,
                        CurrentGame = currentGame,
                        IsSelectingPosition = true
                    };

                    currentGame.ActiveMovementInfo = movementInfo;

                    GameSpace.ShowGameSpaceForMovement(currentGame, movementInfo);
                }
                else if (ans == 1)
                {
                    MenuManger.InGameSetMenu(CreateDynamicInGameMenu(
                        InGameArmyDisbandConfirmationAction,
                        $"Are you sure you want to disband this {divisionInfo.Division.DivisionType}? You will recover 100% of resources.",
                        "Yes, disband", "No, cancel"), currentGame, divisionInfo);
                }
                else if (ans == 2 || ans == -1)
                {
                    GameSpace.ShowGameSpace(currentGame);
                }
            }
        }

        public static void InGameArmyDisbandConfirmationAction(int ans, Game currentGame, object additionalData)
        {
            if (additionalData is Army.ArmyDivisionMenuInfo divisionInfo)
            {
                if (ans == 0)
                {
                    Army.DisbandDivision(currentGame, divisionInfo.Division);
                    GameSpace.ShowGameSpace(currentGame);
                }
                else if (ans == 1 || ans == -1)
                {
                    MenuManger.InGameSetMenu(InGameArmyDivisionMenuLogic, currentGame, additionalData);
                }
            }
        }

        public static void InGameBuildSettlementMenuAction(int ans, Game currentGame, object additionalData)
        {
            if (additionalData is SettlementBuildInfo buildInfo)
            {
                Country playerCountry = currentGame.GameCountries.FirstOrDefault(c => !c.IsBot);

                if (ans == 0)
                {
                    if (playerCountry.CountryMoney >= 10000)
                    {
                        InGameSetMenuAskForSettlement(currentGame, buildInfo, "city");
                    }
                    else
                    {
                        ConsoleManager.ErrorMassege($"Not enough money! Need: $10000, Have: ${playerCountry.CountryMoney}");
                        GameSpace.ShowGameSpace(currentGame);
                    }
                }
                else if (ans == 1)
                {
                    if (playerCountry.CountryMoney >= 5000)
                    {
                        int mapSize = currentGame.MapSize;
                        int x = buildInfo.CellIndex % mapSize;
                        int y = buildInfo.CellIndex / mapSize;

                        bool nearWater = IsNearWater(currentGame.Map.MetaMap, x, y, mapSize);

                        if (nearWater)
                        {
                            BuildPort(currentGame, buildInfo.CellIndex, playerCountry);
                            GameSpace.ShowGameSpace(currentGame);
                        }
                        else
                        {
                            ConsoleManager.ErrorMassege("Port can only be built adjacent to water!");
                            GameSpace.ShowGameSpace(currentGame);
                        }
                    }
                    else
                    {
                        ConsoleManager.ErrorMassege($"Not enough money! Need: $5000, Have: ${playerCountry.CountryMoney}");
                        GameSpace.ShowGameSpace(currentGame);
                    }
                }
                else if (ans == 2 || ans == -1)
                {
                    GameSpace.ShowGameSpace(currentGame);
                }
            }
        }

        private static bool IsNearWater(List<char> map, int x, int y, int mapSize)
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

        public static void InGameSetMenuAskForSettlement(Game game, SettlementBuildInfo buildInfo, string settlementType)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.CursorVisible = true;

            ConsoleManager.ConsoleWriteLineCentered("Enter name for your new city:");

            string cityName = "";
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter)
                {
                    if (!string.IsNullOrWhiteSpace(cityName))
                    {
                        BuildCity(game, buildInfo.CellIndex, cityName);
                        GameSpace.ShowGameSpace(game);
                        break;
                    }
                }
                else if (key.Key == ConsoleKey.Escape)
                {
                    GameSpace.ShowGameSpace(game);
                    break;
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (cityName.Length > 0)
                    {
                        cityName = cityName.Substring(0, cityName.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    cityName += key.KeyChar;
                    Console.Write(key.KeyChar);
                }
            }
        }

        private static void BuildCity(Game game, int cellIndex, string cityName)
        {
            Country playerCountry = game.GameCountries.FirstOrDefault(c => !c.IsBot);

            if (playerCountry == null || playerCountry.CountryMoney < 10000)
                return;

            var cityMap = GenerateEmptyCityMap();

            City newCity = City.CreateCity(cellIndex, game.MapSize, false, cityName);
            newCity.CityMap = cityMap;

            if (playerCountry.CountryСitys == null)
                playerCountry.CountryСitys = new List<City>();

            playerCountry.CountryСitys.Add(newCity);

            playerCountry.CountryMoney -= 10000;

            game.Map.MetaMap[cellIndex] = Game.MapObjects["City"];

            if (!playerCountry.CountryTerritory.Contains(cellIndex))
                playerCountry.CountryTerritory.Add(cellIndex);
        }

        private static void BuildPort(Game game, int cellIndex, Country playerCountry)
        {
            Port newPort = new Port
            {
                PortPosition = cellIndex
            };

            if (playerCountry.CountryPorts == null)
                playerCountry.CountryPorts = new List<Port>();

            playerCountry.CountryPorts.Add(newPort);

            playerCountry.CountryMoney -= 5000;

            game.Map.MetaMap[cellIndex] = Game.MapObjects["Port"];

            if (!playerCountry.CountryTerritory.Contains(cellIndex))
                playerCountry.CountryTerritory.Add(cellIndex);
        }

        private static List<char> GenerateEmptyCityMap()
        {
            int citySize = 20;
            var cityMap = new List<char>(new char[citySize * citySize]);

            for (int i = 0; i < cityMap.Count; i++)
            {
                int x = i % citySize;
                int y = i / citySize;

                if (x == 0 || y == 0 || x == citySize - 1 || y == citySize - 1)
                {
                    cityMap[i] = City.SityMapObjects["Border"];
                }
                else
                {
                    cityMap[i] = City.SityMapObjects["Land"];
                }
            }

            return cityMap;
        }
        public class SettlementBuildInfo
        {
            public int CellIndex { get; set; }
            public bool CanBuildPort { get; set; }
        }
        public static void InGameLandCellActionMenuAction(int ans, Game currentGame, object additionalData)
        {
            if (additionalData is LandCellInfo cellInfo)
            {
                if (ans == 0)
                {
                    Country playerCountry = currentGame.GameCountries.FirstOrDefault(c => !c.IsBot);
                    if (playerCountry != null && cellInfo.CountryAtPosition == playerCountry)
                    {
                        bool hasArmy = false;
                        foreach (var armyDivision in playerCountry.CountryArmy?.Divisions ?? new List<Army.ArmyDivision>())
                        {
                            if (armyDivision.Position == cellInfo.CellIndex)
                            {
                                hasArmy = true;
                                break;
                            }
                        }
                        if (!hasArmy)
                        {
                            MenuManger.InGameShowArmyPlacementMenuLogic(currentGame, cellInfo.CellIndex);
                        }
                        else
                        {
                            ConsoleManager.ErrorMassege("There is already an army at this position!");
                            GameSpace.ShowGameSpace(currentGame);
                        }
                    }
                }
                else if (ans == 1)
                {
                    bool canBuildPort = GameSpace.IsNearWater(currentGame.Map.MetaMap,
                        cellInfo.CursorX, cellInfo.CursorY, currentGame.MapSize);

                    var buildInfo = new SettlementBuildInfo
                    {
                        CellIndex = cellInfo.CellIndex,
                        CanBuildPort = canBuildPort
                    };

                    if (canBuildPort)
                    {
                        MenuManger.InGameSetMenu(MenuManger.InGameBuildSettlementMenuLogic, currentGame, buildInfo);
                    }
                    else
                    {
                        MenuManger.InGameSetMenu(MenuManger.CreateDynamicInGameMenu(
                            (ans2, g, data) =>
                            {
                                if (data is SettlementBuildInfo info)
                                {
                                    if (ans2 == 0)
                                    {
                                        MenuManger.InGameSetMenuAskForSettlement(g, info, "city");
                                    }
                                    else if (ans2 == 1 || ans2 == -1)
                                    {
                                        GameSpace.ShowGameSpace(g);
                                    }
                                }
                            }, "--BUILD SETTLEMENT--",
                            "Build City ($10000)",
                            "Back"), currentGame, buildInfo);
                    }
                }
                else if (ans == 2 || ans == -1)
                {
                    GameSpace.ShowGameSpace(currentGame);
                }
            }
        }
        public class LandCellInfo
        {
            public int CellIndex { get; set; }
            public int CursorX { get; set; }
            public int CursorY { get; set; }
            public Country CountryAtPosition { get; set; }
        }
        public static void InGamePortActionMenuAction(int ans, Game currentGame, object additionalData)
        {
            if (additionalData is Port port)
            {
                Country playerCountry = currentGame.GameCountries.FirstOrDefault(c => !c.IsBot);

                if (ans == 0)
                {
                    if (playerCountry.CountryMoney >= 100)
                    {
                        port.CivilianShips++;
                        playerCountry.CountryMoney -= 100;
                    }
                    else
                    {
                        ConsoleManager.ErrorMassege($"Not enough money! Need: $100, Have: ${playerCountry.CountryMoney}");
                    }
                    GameSpace.ShowGameSpace(currentGame);
                }
                else if (ans == 1)
                {
                    int cargoAvailable = port.CanBuildCargoShip() ? 1 : 0;
                    int combatAvailable = port.CanBuildCombatShip() ? 1 : 0;

                    string cargoText = string.Format($"Cargo Ship ($500) - Available: {{0}}/2",
                        port.MilitaryShips?.Count(s => s.ShipType == "Cargo") ?? 0);
                    string combatText = string.Format($"Combat Ship ($1000) - Available: {{0}}/5",
                        port.MilitaryShips?.Count(s => s.ShipType == "Combat") ?? 0);

                    var militaryShipMenu = new Menu(null, null, InGameMilitaryShipTypeMenuAction,
                        "Select military ship type:",
                        cargoText,
                        combatText,
                        "Back");

                    InGameSetMenu(militaryShipMenu, currentGame, port);
                }
                else if (ans == 2 || ans == -1)
                {
                    GameSpace.ShowGameSpace(currentGame);
                }
            }
        }

        public static void InGameMilitaryShipTypeMenuAction(int ans, Game currentGame, object additionalData)
        {
            if (additionalData is Port port)
            {
                Country playerCountry = currentGame.GameCountries.FirstOrDefault(c => !c.IsBot);

                if (ans == 0)
                {
                    if (!port.CanBuildCargoShip())
                    {
                        ConsoleManager.ErrorMassege("Cannot build more cargo ships! Maximum is 2.");
                        GameSpace.ShowGameSpace(currentGame);
                        return;
                    }

                    if (playerCountry.CountryMoney >= 500)
                    {
                        TryPlaceWarship(currentGame, port, playerCountry, "Cargo", 500);
                    }
                    else
                    {
                        ConsoleManager.ErrorMassege($"Not enough money! Need: $500, Have: ${playerCountry.CountryMoney}");
                        GameSpace.ShowGameSpace(currentGame);
                    }
                }
                else if (ans == 1)
                {
                    if (!port.CanBuildCombatShip())
                    {
                        ConsoleManager.ErrorMassege("Cannot build more combat ships! Maximum is 5.");
                        GameSpace.ShowGameSpace(currentGame);
                        return;
                    }

                    if (playerCountry.CountryMoney >= 1000)
                    {
                        TryPlaceWarship(currentGame, port, playerCountry, "Combat", 1000);
                    }
                    else
                    {
                        ConsoleManager.ErrorMassege($"Not enough money! Need: $1000, Have: ${playerCountry.CountryMoney}");
                        GameSpace.ShowGameSpace(currentGame);
                    }
                }
                else if (ans == 2 || ans == -1)
                {
                    Console.Clear();
                    InGameSetMenu(InGamePortActionMenu, currentGame, port);
                }
            }
        }
        private static void TryPlaceWarship(Game game, Port port, Country country, string shipType, int cost)
        {
            int mapSize = game.MapSize;
            int portX = port.PortPosition % mapSize;
            int portY = port.PortPosition / mapSize;

            List<(int x, int y)> possiblePositions = new List<(int x, int y)>();

            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (dx == 0 && dy == 0) continue;

                    int nx = portX + dx;
                    int ny = portY + dy;

                    if (nx >= 0 && nx < mapSize && ny >= 0 && ny < mapSize)
                    {
                        int cellIndex = ny * mapSize + nx;
                        char cellChar = game.Map.MetaMap[cellIndex];

                        if (cellChar == Game.MapObjects["Sea"])
                        {
                            bool occupied = false;
                            foreach (var existingPort in country.CountryPorts)
                            {
                                foreach (var ship in existingPort.MilitaryShips)
                                {
                                    if (ship.Position == cellIndex)
                                    {
                                        occupied = true;
                                        break;
                                    }
                                }
                                if (occupied) break;
                            }

                            if (!occupied)
                            {
                                possiblePositions.Add((nx, ny));
                            }
                        }
                    }
                }
            }

            if (possiblePositions.Count > 0)
            {
                var position = possiblePositions[0];
                int cellIndex = position.y * mapSize + position.x;

                Warship newShip = new Warship
                {
                    ShipType = shipType,
                    Position = cellIndex,
                    OwnerCountry = country
                };

                if (port.MilitaryShips == null)
                    port.MilitaryShips = new List<Warship>();

                port.MilitaryShips.Add(newShip);

                country.CountryMoney -= cost;

                game.Map.MetaMap[cellIndex] = Warship.WarshipMapObjects[shipType];

                GameSpace.ShowGameSpace(game);
            }
            else
            {
                ConsoleManager.ErrorMassege("No available water space near the port!");
                GameSpace.ShowGameSpace(game);
            }
        }
        public static void InGameWarshipMenuAction(int ans, Game currentGame, object additionalData)
        {
            if (additionalData is Warship.WarshipMenuInfo shipInfo)
            {
                if (shipInfo.Ship.HasMovedThisTurn)
                {
                    ConsoleManager.ErrorMassege($"This ship has already moved this turn!");
                    GameSpace.ShowGameSpace(currentGame);
                    return;
                }
                if (ans == 0)
                {
                    var movementInfo = new Warship.WarshipMovementInfo
                    {
                        Ship = shipInfo.Ship,
                        PlayerCountry = shipInfo.PlayerCountry,
                        CurrentGame = currentGame,
                        IsSelectingPosition = true
                    };

                    currentGame.ActiveWarshipMovementInfo = movementInfo;

                    GameSpace.ShowGameSpaceForShipMovement(currentGame, movementInfo);
                }
                else if (ans == 1)
                {
                    MenuManger.InGameSetMenu(CreateDynamicInGameMenu(
                        InGameWarshipDisbandConfirmationAction,
                        $"Are you sure you want to disband this {shipInfo.Ship.ShipType}?",
                        "Yes, disband", "No, cancel"), currentGame, shipInfo);
                }
                else if (ans == 2 || ans == -1)
                {
                    GameSpace.ShowGameSpace(currentGame);
                }
            }
        }

        public static void InGameWarshipDisbandConfirmationAction(int ans, Game currentGame, object additionalData)
        {
            if (additionalData is Warship.WarshipMenuInfo shipInfo)
            {
                if (ans == 0)
                {
                    DisbandWarship(currentGame, shipInfo.Ship);
                    GameSpace.ShowGameSpace(currentGame);
                }
                else if (ans == 1 || ans == -1)
                {
                    MenuManger.InGameSetMenu(InGameWarshipMenuLogic, currentGame, additionalData);
                }
            }
        }

        private static void DisbandWarship(Game game, Warship ship)
        {
            Port owningPort = null;
            Country ownerCountry = null;

            foreach (var country in game.GameCountries)
            {
                if (country.CountryPorts != null)
                {
                    foreach (var port in country.CountryPorts)
                    {
                        if (port.MilitaryShips != null && port.MilitaryShips.Contains(ship))
                        {
                            owningPort = port;
                            ownerCountry = country;
                            break;
                        }
                    }
                }
                if (owningPort != null) break;
            }

            if (owningPort != null && ownerCountry != null)
            {
                owningPort.MilitaryShips.Remove(ship);

                game.Map.MetaMap[ship.Position] = Game.MapObjects["Sea"];
            }
        }
    }
}
