using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    public class Army
    {
        public List<ArmyDivision> Divisions { get; set; } = new List<ArmyDivision>();

        public class ArmyDivision
        {
            public string DivisionType { get; set; }
            public int Position { get; set; }
            public Country OwnerCountry { get; set; }
            public char OriginalTerrain { get; set; }
            public bool HasMovedThisTurn { get; set; } = false;
        }

        public class ArmyMovementInfo
        {
            public ArmyDivision Division { get; set; }
            public Country PlayerCountry { get; set; }
            public Game CurrentGame { get; set; }
            public bool IsSelectingPosition { get; set; } = false;
            public bool HasMovedThisTurn { get; set; } = false;
        }

        public class ArmyDivisionMenuInfo
        {
            public ArmyDivision Division { get; set; }
            public Country PlayerCountry { get; set; }
            public Game CurrentGame { get; set; }
        }

        public static int GetMaxMovementDistance(string divisionType)
        {
            return divisionType switch
            {
                "InfantryDivision" => 1,
                "ArmoredDivision" => 3,
                "MountainDivision" => 1,
                _ => 1
            };
        }

        private static bool IsValidArmyMovementTerrain(char cellChar)
        {
            return cellChar == Game.MapObjects["Land"] ||
                   cellChar == Game.MapObjects["Forest"] ||
                   cellChar == Game.MapObjects["Desert"] ||
                   cellChar == Game.MapObjects["Mountain"] ||
                   cellChar == Game.MapObjects["Hill"] ||
                   cellChar == Game.MapObjects["Capital"] ||
                   cellChar == Game.MapObjects["City"];
        }

        public static void DisbandDivision(Game game, ArmyDivision division)
        {
            Dictionary<string, int> returnedResources = GetReturnedResources(division.DivisionType);

            foreach (var resource in returnedResources)
            {
                if (division.OwnerCountry.MilitaryAmmunitionDepot.ContainsKey(resource.Key))
                {
                    division.OwnerCountry.MilitaryAmmunitionDepot[resource.Key] += resource.Value;
                }
                else
                {
                    division.OwnerCountry.MilitaryAmmunitionDepot[resource.Key] = resource.Value;
                }
            }

            if (division.OwnerCountry.CountryArmy?.Divisions != null)
            {
                division.OwnerCountry.CountryArmy.Divisions.Remove(division);
            }

            int mapSize = (int)Math.Sqrt(game.Map.MetaMap.Count);

            char originalTerrain = GetOriginalTerrainForDisband(game, division.Position);
            game.Map.MetaMap[division.Position] = originalTerrain;
        }

        private static char GetOriginalTerrainForDisband(Game game, int position)
        {
            foreach (var country in game.GameCountries)
            {
                if (country.CountryСitys != null)
                {
                    foreach (var city in country.CountryСitys)
                    {
                        if (city.СityPosition == position)
                        {
                            return city.IsCapital ? Game.MapObjects["Capital"] : Game.MapObjects["City"];
                        }
                    }
                }
            }
            return Game.MapObjects["Land"];
        }

        private static Dictionary<string, int> GetReturnedResources(string divisionType)
        {
            return divisionType switch
            {
                "InfantryDivision" => new Dictionary<string, int> { { "Rifle", 500 }, { "Truck", 50 } },
                "ArmoredDivision" => new Dictionary<string, int> { { "Tank", 50 }, { "Truck", 100 } },
                "MountainDivision" => new Dictionary<string, int> { { "Rifle", 250 } },
                _ => new Dictionary<string, int>()
            };
        }

        public static Dictionary<string, char> ArmyMapObjects = new Dictionary<string, char>()
        {
            ["InfantryDivision"] = 'i',
            ["ArmoredDivision"] = 'a',
            ["MountainDivision"] = 'm',
        };

        public static bool MoveDivision(Game game, ArmyDivision division, int targetCellIndex)
        {
            if (division.HasMovedThisTurn)
            {
                ConsoleManager.ErrorMassege($"This division has already moved this turn!");
                return false;
            }

            int mapSize = (int)Math.Sqrt(game.Map.MetaMap.Count);
            int currentX = division.Position % mapSize;
            int currentY = division.Position / mapSize;
            int targetX = targetCellIndex % mapSize;
            int targetY = targetCellIndex / mapSize;

            int distance = Math.Abs(currentX - targetX) + Math.Abs(currentY - targetY);
            int maxDistance = GetMaxMovementDistance(division.DivisionType);

            if (distance > maxDistance || distance == 0)
                return false;

            char targetCellChar = game.Map.MetaMap[targetCellIndex];
            Country divisionOwner = division.OwnerCountry;

            if (targetCellChar == Game.MapObjects["Capital"] || targetCellChar == Game.MapObjects["City"])
            {
                bool isOwnCity = false;
                foreach (var city in divisionOwner.CountryСitys)
                {
                    if (city.СityPosition == targetCellIndex)
                    {
                        isOwnCity = true;
                        break;
                    }
                }
                if (isOwnCity)
                    return false;
            }

            foreach (var country in game.GameCountries)
            {
                if (country.CountryArmy?.Divisions != null)
                {
                    foreach (var otherDivision in country.CountryArmy.Divisions)
                    {
                        if (otherDivision.Position == targetCellIndex && otherDivision != division)
                            return false;
                    }
                }
            }

            Country targetTerritoryOwner = null;
            foreach (var country in game.GameCountries)
            {
                if (country.CountryTerritory.Contains(targetCellIndex))
                {
                    targetTerritoryOwner = country;
                    break;
                }
            }

            if (targetTerritoryOwner != null && divisionOwner.IsAtWarWith(targetTerritoryOwner))
            {
                CaptureTerritory(game, divisionOwner, targetTerritoryOwner, targetCellIndex, mapSize);
                CheckForCapitalCapture(game, divisionOwner, targetTerritoryOwner, targetCellIndex);
            }
            else if (targetTerritoryOwner != null && targetTerritoryOwner != divisionOwner)
            {
                return false;
            }

            char targetCell = game.Map.MetaMap[targetCellIndex];
            if (!IsValidArmyMovementTerrain(targetCell))
                return false;

            game.Map.MetaMap[division.Position] = division.OriginalTerrain;

            char newOriginalTerrain = game.Map.MetaMap[targetCellIndex];

            division.Position = targetCellIndex;
            division.OriginalTerrain = newOriginalTerrain;

            game.Map.MetaMap[targetCellIndex] = ArmyMapObjects[division.DivisionType];

            division.HasMovedThisTurn = true;

            return true;
        }
        private static void CaptureTerritory(Game game, Country attacker, Country defender, int cellIndex, int mapSize)
        {
            if (defender.CountryTerritory.Contains(cellIndex))
            {
                defender.CountryTerritory.Remove(cellIndex);
            }

            if (!attacker.CountryTerritory.Contains(cellIndex))
            {
                attacker.CountryTerritory.Add(cellIndex);
            }

            if (defender.CountryСitys != null)
            {
                List<City> citiesToTransfer = new List<City>();

                foreach (var city in defender.CountryСitys.ToList())
                {
                    if (city.СityPosition == cellIndex)
                    {
                        citiesToTransfer.Add(city);

                        if (city.IsCapital)
                        {
                            SurrenderCountry(game, attacker, defender);
                            return;
                        }
                    }
                }

                foreach (var city in citiesToTransfer)
                {
                    defender.CountryСitys.Remove(city);

                    if (attacker.CountryСitys == null)
                        attacker.CountryСitys = new List<City>();

                    if (!attacker.CountryСitys.Contains(city))
                        attacker.CountryСitys.Add(city);
                }
            }
        }
        private static void CheckForCapitalCapture(Game game, Country attacker, Country defender, int cellIndex)
        {
            if (defender.CountryСitys == null)
                return;

            City capturedCapital = defender.CountryСitys.FirstOrDefault(c => c.IsCapital && c.СityPosition == cellIndex);

            if (capturedCapital != null)
            {
                SurrenderCountry(game, attacker, defender);
            }
            else
            {
                foreach (City city in defender.CountryСitys)
                {
                    if (city.IsCapital)
                    {
                        int mapSize = (int)Math.Sqrt(game.Map.MetaMap.Count);
                        int cityX = city.СityPosition % mapSize;
                        int cityY = city.СityPosition / mapSize;

                        int targetX = cellIndex % mapSize;
                        int targetY = cellIndex / mapSize;

                        if (Math.Abs(cityX - targetX) <= 1 && Math.Abs(cityY - targetY) <= 1)
                        {
                            SurrenderCountry(game, attacker, defender);
                            break;
                        }
                    }
                }
            }
        }

        private static void SurrenderCountry(Game game, Country winner, Country loser)
        {
            if (loser.CountryTerritory == null || loser.CountryTerritory.Count == 0)
                return;

            City capitalCity = loser.CountryСitys?.FirstOrDefault(c => c.IsCapital);

            if (capitalCity != null)
            {
                capitalCity.IsCapital = false;


                char currentCell = game.Map.MetaMap[capitalCity.СityPosition];
                if (currentCell == Game.MapObjects["Capital"])
                {
                    game.Map.MetaMap[capitalCity.СityPosition] = Game.MapObjects["City"];
                }
            }

            if (loser.CountryTerritory != null && loser.CountryTerritory.Count > 0)
            {
                foreach (var territoryCell in loser.CountryTerritory.ToList())
                {
                    if (!winner.CountryTerritory.Contains(territoryCell))
                    {
                        winner.CountryTerritory.Add(territoryCell);
                    }
                }
                loser.CountryTerritory.Clear();
            }

            if (loser.CountryСitys != null && loser.CountryСitys.Count > 0)
            {
                foreach (var city in loser.CountryСitys.ToList())
                {
                    if (!winner.CountryСitys.Contains(city))
                    {
                        winner.CountryСitys.Add(city);
                    }
                }
                loser.CountryСitys.Clear();
            }

            int transferredMoney = loser.CountryMoney / 2;
            winner.CountryMoney += transferredMoney;
            loser.CountryMoney -= transferredMoney;

            if (loser.MilitaryAmmunitionDepot != null)
            {
                foreach (var resource in loser.MilitaryAmmunitionDepot.ToList())
                {
                    int transferredAmount = (int)(resource.Value * 0.75);

                    if (winner.MilitaryAmmunitionDepot.ContainsKey(resource.Key))
                    {
                        winner.MilitaryAmmunitionDepot[resource.Key] += transferredAmount;
                    }
                    else
                    {
                        winner.MilitaryAmmunitionDepot[resource.Key] = transferredAmount;
                    }

                    loser.MilitaryAmmunitionDepot[resource.Key] -= transferredAmount;
                }
            }

            if (loser.CountryArmy?.Divisions != null)
            {
                foreach (var division in loser.CountryArmy.Divisions.ToList())
                {
                    DisbandEnemyDivision(game, winner, division);
                }
                loser.CountryArmy.Divisions.Clear();
            }

            winner.DeclarePeace(loser);

            UpdateMapAfterSurrender(game, loser, winner);

            ConsoleManager.ErrorMassege($"{loser.CountryName} has surrendered to {winner.CountryName}!");
        }

        private static void UpdateMapAfterSurrender(Game game, Country loser, Country winner)
        {
            int mapSize = (int)Math.Sqrt(game.Map.MetaMap.Count);

            if (loser.CountryArmy?.Divisions != null)
            {
                foreach (var division in loser.CountryArmy.Divisions)
                {
                    char originalTerrain = GetOriginalTerrainForSurrender(game, division.Position);
                    game.Map.MetaMap[division.Position] = originalTerrain;
                }
            }

            if (winner.CountryСitys != null)
            {
                foreach (var city in winner.CountryСitys)
                {
                    if (city.IsCapital && !winner.CountryСitys.Any(c => c.IsCapital && c != city))
                    {
                        game.Map.MetaMap[city.СityPosition] = Game.MapObjects["Capital"];
                    }
                    else if (game.Map.MetaMap[city.СityPosition] == Game.MapObjects["Capital"])
                    {
                        game.Map.MetaMap[city.СityPosition] = Game.MapObjects["City"];
                    }
                }
            }
        }
        private static char GetOriginalTerrainForSurrender(Game game, int position)
        {
            foreach (var country in game.GameCountries)
            {
                if (country.CountryСitys != null)
                {
                    foreach (var city in country.CountryСitys)
                    {
                        if (city.СityPosition == position && city.IsCapital)
                        {
                            return Game.MapObjects["Capital"];
                        }
                    }
                }
            }

            return Game.MapObjects["Land"];
        }

        private static void DisbandEnemyDivision(Game game, Country winner, ArmyDivision division)
        {
            Dictionary<string, int> returnedResources = GetReturnedResources(division.DivisionType);

            foreach (var resource in returnedResources)
            {
                int returnedAmount = resource.Value;

                if (winner.MilitaryAmmunitionDepot.ContainsKey(resource.Key))
                {
                    winner.MilitaryAmmunitionDepot[resource.Key] += returnedAmount;
                }
                else
                {
                    winner.MilitaryAmmunitionDepot[resource.Key] = returnedAmount;
                }
            }

            char originalTerrain = GetOriginalTerrain(game, division.Position);
            game.Map.MetaMap[division.Position] = originalTerrain;
        }

        public static bool CanMoveToPosition(Game game, ArmyDivision division, int targetCellIndex,
                                     List<Country> countries, int mapSize)
        {
            Country ownerCountry = division.OwnerCountry;

            Country targetTerritoryOwner = null;
            foreach (var country in countries)
            {
                if (country.CountryTerritory.Contains(targetCellIndex))
                {
                    targetTerritoryOwner = country;
                    break;
                }
            }

            if (targetTerritoryOwner == null)
            {
                char targetCell = game.Map.MetaMap[targetCellIndex];
                if (targetCell == Game.MapObjects["Sea"] || targetCell == Game.MapObjects["Border"])
                    return false;

                return true;
            }

            char targetCellChar = game.Map.MetaMap[targetCellIndex];
            bool isEnemyCityOrCapital = (targetCellChar == Game.MapObjects["Capital"] ||
                                         targetCellChar == Game.MapObjects["City"]) &&
                                         targetTerritoryOwner != ownerCountry;

            if (targetTerritoryOwner == ownerCountry ||
                ownerCountry.IsAtWarWith(targetTerritoryOwner) ||
                isEnemyCityOrCapital)
            {
                char targetCell = game.Map.MetaMap[targetCellIndex];

                if (isEnemyCityOrCapital)
                {

                }
                else if (!IsValidArmyMovementTerrain(targetCell))
                {
                    return false;
                }

                int currentX = division.Position % mapSize;
                int currentY = division.Position / mapSize;
                int targetX = targetCellIndex % mapSize;
                int targetY = targetCellIndex / mapSize;

                int distance = Math.Abs(currentX - targetX) + Math.Abs(currentY - targetY);
                int maxDistance = GetMaxMovementDistance(division.DivisionType);

                return distance <= maxDistance && distance > 0;
            }
            return false;
        }

        public static bool IsCellOccupiedByDivision(Game game, int cellIndex, ArmyDivision excludingDivision = null)
        {
            foreach (var country in game.GameCountries)
            {
                if (country.CountryArmy?.Divisions != null)
                {
                    foreach (var division in country.CountryArmy.Divisions)
                    {
                        if (division.Position == cellIndex && division != excludingDivision)
                            return true;
                    }
                }
            }
            return false;
        }
        private static char GetOriginalTerrain(Game game, int position)
        {
            foreach (var country in game.GameCountries)
            {
                if (country.CountryArmy?.Divisions != null)
                {
                    foreach (var division in country.CountryArmy.Divisions)
                    {
                        if (division.Position == position)
                        {
                            return Game.MapObjects["Land"];
                        }
                    }
                }
            }

            char currentChar = game.Map.MetaMap[position];

            if (ArmyMapObjects.Values.Contains(currentChar))
            {
                return Game.MapObjects["Land"];
            }

            return currentChar;
        }
    }
}
