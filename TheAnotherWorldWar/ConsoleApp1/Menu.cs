using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    public class Menu
    {
        public Action<int> ActionInt { get; set; } = null;
        public Action<string> ActionStr { get; set; } = null;
        public Action<int, Game, object> ActionInGame { get; set; } = null;
        public string Title { get; set; }
        public List<string> Buttons { get; set; }

        public Menu(Action<int> actionInt, Action<string> actionStr, Action<int, Game, object> actionInGame, string title, params string[] buttons)
        {
            ActionInt = actionInt;
            ActionStr = actionStr;
            ActionInGame = actionInGame;
            Title = title;
            Buttons = buttons.ToList();
        }
    }
}
