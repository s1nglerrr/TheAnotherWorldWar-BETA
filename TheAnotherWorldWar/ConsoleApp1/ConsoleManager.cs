using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    public static class ConsoleManager
    {
        public static int indx = 0;
        public static int ConsoleKeys(string header, List<string> list)
        {
            if (list.Count != 0)
            {
                string[] strs = list.ToArray();

                Console.CursorVisible = false;

                ConsoleRefresh(header, strs);

                Console.ForegroundColor = ConsoleColor.White;

                while (true)
                {
                    ConsoleKeyInfo key = ListenKays();
                    if (key.Key == ConsoleKey.UpArrow)
                    {
                        if (indx > 0)
                        {
                            indx--;
                            ConsoleRefresh(header, strs);

                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                    else if (key.Key == ConsoleKey.DownArrow)
                    {
                        if (indx <= strs.Length - 2)
                        {
                            indx++;
                            ConsoleRefresh(header, strs);

                            Console.ForegroundColor = ConsoleColor.White;
                        }
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        Console.CursorVisible = true;
                        int indx1 = indx;
                        indx = 0;
                        return indx1;
                    }
                    else if (key.Key == ConsoleKey.Escape)
                    {
                        Console.CursorVisible = true;
                        int indx1 = indx;
                        indx = 0;
                        return -1;
                    }
                }
            }
            else
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.White;
                Console.CursorVisible = false;

                ConsoleWriteLineCentered("Тут пока ничего нет.");
                while (true)
                {
                    ConsoleKeyInfo key = ListenKays();
                    if (key.Key == ConsoleKey.Enter)
                    {
                        Console.CursorVisible = true;
                        int indx1 = indx;
                        indx = 0;
                        return -1;
                    }
                    else if (key.Key == ConsoleKey.Escape)
                    {
                        Console.CursorVisible = true;
                        int indx1 = indx;
                        indx = 0;
                        return -1;
                    }
                }
            }
        }
        public static string ConsoleAsk(string str)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            ConsoleWriteLineCentered(str);

            return ListenAskKays();
        }
        public static string ListenAskKays()
        {
            StringBuilder input = new StringBuilder();
            int cursorPosition = 0;
            int inputLineTop = Console.CursorTop;

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Escape)
                {
                    return "";
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    return input.ToString();
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (input.Length > 0 && cursorPosition > 0)
                    {
                        input.Remove(cursorPosition - 1, 1);
                        cursorPosition--;

                        Console.SetCursorPosition(0, inputLineTop);

                        Console.Write(input.ToString() + " ");

                        Console.SetCursorPosition(cursorPosition, inputLineTop);
                    }
                }
                else if (key.Key == ConsoleKey.LeftArrow)
                {
                    if (cursorPosition > 0)
                    {
                        cursorPosition--;
                        Console.SetCursorPosition(cursorPosition, inputLineTop);
                    }
                }
                else if (key.Key == ConsoleKey.RightArrow)
                {
                    if (cursorPosition < input.Length)
                    {
                        cursorPosition++;
                        Console.SetCursorPosition(cursorPosition, inputLineTop);
                    }
                }
                else if (key.Key == ConsoleKey.Home)
                {
                    cursorPosition = 0;
                    Console.SetCursorPosition(cursorPosition, inputLineTop);
                }
                else if (key.Key == ConsoleKey.End)
                {
                    cursorPosition = input.Length;
                    Console.SetCursorPosition(cursorPosition, inputLineTop);
                }
                else if (key.Key == ConsoleKey.Delete)
                {
                    if (cursorPosition < input.Length)
                    {
                        input.Remove(cursorPosition, 1);
                        Console.SetCursorPosition(0, inputLineTop);
                        Console.Write(input.ToString() + " ");
                        Console.SetCursorPosition(cursorPosition, inputLineTop);
                    }
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    input.Insert(cursorPosition, key.KeyChar);
                    cursorPosition++;

                    Console.SetCursorPosition(0, inputLineTop);
                    Console.Write(input.ToString());
                    Console.SetCursorPosition(cursorPosition, inputLineTop);
                }
            }
        }
        private static void ConsoleRefresh(string header, params string[] str)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            ConsoleWriteLineCentered(header + "\n");
            for (int i = 0; i < str.Length; i++)
            {
                if (indx == i)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    ConsoleWriteLineCentered("- " + $"{str[i]}" + " -");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    ConsoleWriteLineCentered($"{str[i]}");
                }
            }
        }
        public static void ConsoleWriteLineCentered(string text)
        {
            int consoleWidth = Console.WindowWidth;
            int textLength = text.Length;

            int centeredPosition = Math.Max(0, (consoleWidth - textLength) / 2);

            if (centeredPosition < Console.BufferWidth)
            {
                Console.CursorLeft = centeredPosition;
            }
            else
            {
                Console.CursorLeft = 0;
            }

            Console.WriteLine(text);
        }
        public static void ErrorMassege(string msg)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.CursorVisible = false;

            ConsoleWriteLineCentered(msg);
            Console.ReadLine();

            Console.ResetColor();
            Console.CursorVisible = true;
        }
        public static ConsoleKeyInfo ListenKays()
        {
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                return key;
            }
        }
    }
}
