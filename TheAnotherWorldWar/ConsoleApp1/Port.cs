using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    public class Port
    {
        public int PortPosition { get; set; }
        public List<Warship> MilitaryShips { get; set; } = new List<Warship>();
        public int CivilianShips { get; set; } = 0;

        public bool CanBuildCargoShip()
        {
            int cargoCount = MilitaryShips?.Count(s => s.ShipType == "Cargo") ?? 0;
            return cargoCount < 2;
        }

        public bool CanBuildCombatShip()
        {
            int combatCount = MilitaryShips?.Count(s => s.ShipType == "Combat") ?? 0;
            return combatCount < 5;
        }

        public int GetCivilianShipIncome()
        {
            return CivilianShips * 10;
        }
    }
}
