using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    public class Warship
    {
        public string ShipType { get; set; }
        public int Position { get; set; }
        public Country OwnerCountry { get; set; }
        public bool HasMovedThisTurn { get; set; } = false;

        public static Dictionary<string, char> WarshipMapObjects = new Dictionary<string, char>()
        {
            ["Cargo"] = 'b',
            ["Combat"] = 's'
        };

        public static int GetMaxMovementDistance(string shipType)
        {
            return shipType switch
            {
                "Cargo" => 2,
                "Combat" => 4,
                _ => 2
            };
        }

        private static bool IsValidShipMovementTerrain(char cellChar)
        {
            return cellChar == Game.MapObjects["Sea"];
        }

        public static bool CanMoveToPosition(Game game, Warship ship, int targetCellIndex,
                                     List<Country> countries, int mapSize)
        {
            if (ship.HasMovedThisTurn)
            {
                Console.Clear();
                return false;
            }

            int currentX = ship.Position % mapSize;
            int currentY = ship.Position / mapSize;
            int targetX = targetCellIndex % mapSize;
            int targetY = targetCellIndex / mapSize;

            int distance = Math.Abs(currentX - targetX) + Math.Abs(currentY - targetY);
            int maxDistance = GetMaxMovementDistance(ship.ShipType);

            if (distance > maxDistance || distance == 0)
                return false;

            char targetCellChar = game.Map.MetaMap[targetCellIndex];

            if (!IsValidShipMovementTerrain(targetCellChar))
                return false;

            foreach (var country in countries)
            {
                if (country.CountryPorts != null)
                {
                    foreach (var port in country.CountryPorts)
                    {
                        if (port.MilitaryShips != null)
                        {
                            foreach (var otherShip in port.MilitaryShips)
                            {
                                if (otherShip.Position == targetCellIndex && otherShip != ship)
                                    return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        public static bool MoveShip(Game game, Warship ship, int targetCellIndex)
        {
            if (ship.HasMovedThisTurn)
            {
                Console.Clear();
                return false;
            }

            int mapSize = (int)Math.Sqrt(game.Map.MetaMap.Count);
            int currentX = ship.Position % mapSize;
            int currentY = ship.Position / mapSize;
            int targetX = targetCellIndex % mapSize;
            int targetY = targetCellIndex / mapSize;

            int distance = Math.Abs(currentX - targetX) + Math.Abs(currentY - targetY);
            int maxDistance = GetMaxMovementDistance(ship.ShipType);

            if (distance > maxDistance || distance == 0)
                return false;

            char targetCellChar = game.Map.MetaMap[targetCellIndex];

            if (!IsValidShipMovementTerrain(targetCellChar))
                return false;

            foreach (var country in game.GameCountries)
            {
                if (country.CountryPorts != null)
                {
                    foreach (var port in country.CountryPorts)
                    {
                        if (port.MilitaryShips != null)
                        {
                            foreach (var otherShip in port.MilitaryShips)
                            {
                                if (otherShip.Position == targetCellIndex && otherShip != ship)
                                    return false;
                            }
                        }
                    }
                }
            }

            game.Map.MetaMap[ship.Position] = Game.MapObjects["Sea"];

            ship.Position = targetCellIndex;
            game.Map.MetaMap[targetCellIndex] = WarshipMapObjects[ship.ShipType];

            ship.HasMovedThisTurn = true;

            return true;
        }

        public static bool IsCellOccupiedByShip(Game game, int cellIndex, Warship excludingShip = null)
        {
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
                                if (ship.Position == cellIndex && ship != excludingShip)
                                    return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        public class WarshipMovementInfo
        {
            public Warship Ship { get; set; }
            public Country PlayerCountry { get; set; }
            public Game CurrentGame { get; set; }
            public bool IsSelectingPosition { get; set; } = false;
        }
        public class WarshipMenuInfo
        {
            public Warship Ship { get; set; }
            public Country PlayerCountry { get; set; }
            public Game CurrentGame { get; set; }
        }
    }
}
