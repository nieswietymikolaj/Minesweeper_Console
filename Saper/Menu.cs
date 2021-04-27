using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Media;
using System.IO;

namespace Saper
{
    class Menu
    {
        Minesweeper minesweeper = new Minesweeper(20, 50);
        SoundPlayer player = new SoundPlayer("Music/Chiptune.wav");

        public Menu() { }

        public int MainMenu()
        {
            player.PlayLooping();
            //minesweeper.musicFlag = false;

            int choice = 0;
            int choiceMax = 5;
            ConsoleKey key;

            Console.Clear();
            ShowFrame();
            AnimatedLogo();

            while (true)
            {
                ShowLogo();

                string[] options =
                {
                    "    NOWA GRA    ",
                    "   USTAWIENIA   ",
                    "     WYNIKI     ",
                    "   INFORMACJE   ",
                    "   WYJDŹ Z GRY  "
                };

                ShowMenu(choice, options);

                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.W: if (--choice == -1) choice = choiceMax - 1; break;
                    case ConsoleKey.S: if (++choice == choiceMax) choice = 0; break;
                    case ConsoleKey.E:
                        switch (choice)
                        {
                            case 0: minesweeper.AnimatedTransition(2); minesweeper.NewGame(); break;
                            case 1: Settings(); break;
                            case 2: ShowScoreboard(GetScoreboard()); break;
                            case 3: Informations(); break;
                            case 4: minesweeper.AnimatedTransition(2); return -1;
                        }
                        break;
                    case ConsoleKey.Escape: minesweeper.AnimatedTransition(2); return -1;
                }
            }
        }

        private void ShowMenu(int choice, string[] options)
        {
            for (int i = 0; i < options.Length; i++)
            {
                if (choice == i)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }

                Console.SetCursorPosition(Console.WindowWidth / 2 - (options[0].Length / 2 - 1), 15 + (i * 2));
                Console.Write(options[i]);
            }
        }

        private void Settings()
        {
            int choice = 0;
            int choiceMax = 5;
            ConsoleKey key;

            ClearMenu();

            while (true)
            {
                string[] options =
                {
                    "POZIOM TRUDNOŚCI",
                    "WIELKOŚĆ PLANSZY",
                    "  LICZBA BOMB   ",
                    "     MUZYKA     ",
                    "     POWRÓT     "
                };

                ShowMenu(choice, options);

                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.W: if (--choice == -1) choice = choiceMax - 1; break;
                    case ConsoleKey.S: if (++choice == choiceMax) choice = 0; break;
                    case ConsoleKey.E:
                        switch (choice)
                        {
                            case 0: ChangeDifficultyLevel(); break;
                            case 1: ChangeMapSize(); break;
                            case 2: ChangeBombAmount(); break;
                            case 3: Music(); break;
                            case 4: ClearMenu(); return;
                        }
                        break;
                    case ConsoleKey.Escape: ClearMenu(); return;
                }
            }
        }

        private void ChangeDifficultyLevel()
        {
            int choice = 0;
            int choiceMax = 4;
            ConsoleKey key;

            ClearMenu();

            while (true)
            {
                string[] options =
                {
                    "  POCZĄTKUJĄCY  ",
                    "  ZAAWANSOWANY  ",
                    "    EKSPERT     ",
                    "     POWRÓT     "
                };

                ShowMenu(choice, options);

                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.W: if (--choice == -1) choice = choiceMax - 1; break;
                    case ConsoleKey.S: if (++choice == choiceMax) choice = 0; break;
                    case ConsoleKey.E:
                        switch (choice)
                        {
                            case 0: minesweeper.mapSize = 8; minesweeper.bombAmount = 10; minesweeper.level = "początkujący"; ClearMenu(); return;
                            case 1: minesweeper.mapSize = 16; minesweeper.bombAmount = 40; minesweeper.level = "zaawansowany"; ClearMenu(); return;
                            case 2: minesweeper.mapSize = 24; minesweeper.bombAmount = 99; minesweeper.level = "ekspert"; ClearMenu(); return;
                            case 3: ClearMenu(); return;
                        }
                        break;
                    case ConsoleKey.Escape: ClearMenu(); return;
                }
            }
        }

        private void ChangeMapSize()
        {
            ClearMenu();
            ConsoleKey key;

            while (true)
            {
                string navigation = "[W] " + (char)30 + "  [S] " + (char)31 + "  [E] Ustaw";

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.SetCursorPosition(Console.WindowWidth / 2 - (navigation.Length / 2), 15);
                Console.Write(navigation);

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.SetCursorPosition(Console.WindowWidth / 2 - 1, 18);
                Console.Write(minesweeper.mapSize + " ");

                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.W: if (minesweeper.mapSize <= 24 - 1) minesweeper.mapSize++; break;
                    case ConsoleKey.S: if (minesweeper.mapSize >= 9) minesweeper.mapSize--; break;
                    case ConsoleKey.E: ClearMenu(); minesweeper.level = "własny"; return;
                    case ConsoleKey.Escape: ClearMenu(); minesweeper.level = "własny"; return;
                }
            }
        }

        private void ChangeBombAmount()
        {
            ClearMenu();
            ConsoleKey key;

            while (true)
            {
                string navigation = "[W] " + (char)30 + "  [S] " + (char)31 + "  [E] Ustaw";

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.SetCursorPosition(Console.WindowWidth / 2 - (navigation.Length / 2), 15);
                Console.Write(navigation);

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.SetCursorPosition(Console.WindowWidth / 2 - 1, 18);
                Console.Write(minesweeper.bombAmount + " ");

                Console.SetCursorPosition(0, 0);
                key = Console.ReadKey(true).Key;
                Console.SetCursorPosition(0, 0);

                switch (key)
                {
                    case ConsoleKey.W: if (minesweeper.bombAmount <= minesweeper.mapSize * minesweeper.mapSize - minesweeper.mapSize - 1) minesweeper.bombAmount++; break;
                    case ConsoleKey.S: if (minesweeper.bombAmount >= 11) minesweeper.bombAmount--; break;
                    case ConsoleKey.E: ClearMenu(); minesweeper.level = "własny"; return;
                    case ConsoleKey.Escape: ClearMenu(); minesweeper.level = "własny"; return;
                }
            }
        }

        private void Music()
        {
            int choice = 0;
            int choiceMax = 3;
            ConsoleKey key;

            ClearMenu();

            while (true)
            {
                string[] options =
                {
                    "   ZMIEŃ UTWÓR  ",
                    "  WŁ/WYŁ MUZYKĘ ",
                    "     POWRÓT     "
                };

                ShowMenu(choice, options);

                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.W: if (--choice == -1) choice = choiceMax - 1; break;
                    case ConsoleKey.S: if (++choice == choiceMax) choice = 0; break;
                    case ConsoleKey.E:
                        switch (choice)
                        {
                            case 0: Songs(); break;
                            case 1:
                                if (minesweeper.musicFlag == true)
                                {
                                    player.Stop();
                                    minesweeper.musicFlag = false;
                                }
                                else
                                {
                                    player = minesweeper.lastSong;
                                    player.PlayLooping();
                                    minesweeper.musicFlag = true;
                                }
                                break;
                            case 2: ClearMenu(); return;
                        }
                        break;
                    case ConsoleKey.Escape: ClearMenu(); return;
                }
            }
        }

        private void Songs()
        {
            int choice = 0;
            int choiceMax = 3;
            ConsoleKey key;

            ClearMenu();

            while (true)
            {
                string[] options =
                {
                    "    CHIPTUNE    ",
                    "  WHAT IS LOVE  ",
                    "     POWRÓT     "
                };

                ShowMenu(choice, options);

                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.W: if (--choice == -1) choice = choiceMax - 1; break;
                    case ConsoleKey.S: if (++choice == choiceMax) choice = 0; break;
                    case ConsoleKey.E:
                        switch (choice)
                        {
                            case 0: player = new SoundPlayer("Music/Chiptune.wav"); minesweeper.lastSong = new SoundPlayer("Music/Chiptune.wav"); player.PlayLooping(); minesweeper.musicFlag = true; break;
                            case 1: player = new SoundPlayer("Music/WhatIsLove.wav"); minesweeper.lastSong = new SoundPlayer("Music/WhatIsLove.wav"); player.PlayLooping(); minesweeper.musicFlag = true; break;
                            case 2: ClearMenu(); return;
                        }
                        break;
                    case ConsoleKey.Escape: ClearMenu(); return;
                }
            }
        }

        private static List<string> GetScoreboard()
        {
            List<string> scores = new List<string>();

            StreamReader reader = new StreamReader("ASCII/scoreboard.txt");
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                scores.Add(line);
            }
            reader.Close();
            scores.Sort();

            return scores;
        }

        private void ShowScoreboard(List<string> scores)
        {
            ClearMenu();

            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.SetCursorPosition(Console.WindowWidth / 2 - 3, 12);
            Console.Write(" TOP 8 ");
            Console.SetCursorPosition(Console.WindowWidth / 2 - 25, 14);
            Console.Write(" M.  CZAS  PSEUDONIM      P.TRUDNOŚCI   BOMB  R.MAPY ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Black;

            int length;

            if (scores.Count < 8)
            {
                length = scores.Count;
            }
            else
            {
                length = 8;
            }

            for (int i = 0; i < length; i++)
            {
                string[] data = scores[i].Split(' ');
                Console.SetCursorPosition(Console.WindowWidth / 2 - 24, 16 + i);
                Console.WriteLine("{0}.  {1}  {2}  {3}  {4}  {5}x{5}", i + 1, data[0].PadRight(4), data[1].PadRight(13), data[2].PadRight(12), data[3].ToString().PadRight(4), data[4].ToString(), data[4].ToString().PadRight(3));
            }

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.SetCursorPosition(Console.WindowWidth / 2 - 20, Console.WindowHeight - 4);
            Console.Write("[naciśnij dowolny klawisz, aby powrócić]");

            Console.SetCursorPosition(0, 0);
            Console.ReadKey(true);
            ClearMenu();
        }

        private void Informations() //AUTOR: MATEUSZ PASZKO
        {
            ClearMenu();

            string[] author = File.ReadAllLines("ASCII/author.txt");

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(Console.WindowWidth / 2 - 6, 14);
            Console.Write("GRA AUTORSTWA:");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;

            for (int i = 0; i < author.Length; i++)
            {
                Console.SetCursorPosition(Console.WindowWidth / 2 - (author[i].Length / 2), 16 + i);
                Console.Write(author[i]);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(Console.WindowWidth / 2 - 2, 20);
            Console.Write((char)169 + " 2020");
            Console.ForegroundColor = ConsoleColor.DarkGray;

            Console.SetCursorPosition(Console.WindowWidth / 2 - 20, Console.WindowHeight - 4);
            Console.Write("[naciśnij dowolny klawisz, aby powrócić]");

            Console.SetCursorPosition(0, 0);
            Console.ReadKey(true);
            ClearMenu();
        }

        private void ShowLogo()
        {
            string[] logo = File.ReadAllLines("ASCII/logo.txt");

            Console.ForegroundColor = ConsoleColor.DarkYellow;

            for (int i = 0; i < logo.Length; i++)
            {
                Console.SetCursorPosition(Console.WindowWidth / 2 - (logo[i].Length / 2), 4 + i);
                Console.Write(logo[i]);
            }
        }

        private void ShowFrame()
        {
            int height = Console.WindowHeight;
            int width = Console.WindowWidth;

            Console.ForegroundColor = ConsoleColor.Green;

            for (int x = 1; x < width; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    Console.SetCursorPosition(x, y);

                    if (y == 1 && x == 1)
                        Console.Write('╔');
                    else if (y == 1 && x == width - 1)
                        Console.Write('╗');
                    else if (y == height - 2 && x == 1)
                        Console.Write('╚');
                    else if (y == height - 2 && x == width - 1)
                        Console.Write('╝');
                    else if ((x == 1 || x == width - 1) && y > 1)
                        Console.Write('║');
                    else if ((y == 1 || y == height - 2) && x > 1)
                        Console.Write('═');
                    else if (x == 2 || x == width - 2)
                        Console.Write('░');
                }
            }
        }

        private void AnimatedLogo()
        {
            string[] logo = File.ReadAllLines("ASCII/logo.txt");

            int height = Console.WindowHeight / 2;
            int width = Console.WindowWidth / 2 - (logo[0].Length / 2);

            for (int i = (logo.Length / 2); i <= 11; i++)
            {
                ShowAnimatedLogo(logo, width, height - i);
                if (i == (logo.Length / 2))
                {
                    Thread.Sleep(500);
                }
                else
                {
                    Thread.Sleep(50);
                }
            }
        }

        private void ShowAnimatedLogo(string[] logo, int wdh, int hgt)
        {
            for (int i = 0; i < logo.Length; i++)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.SetCursorPosition(wdh, hgt + i);
                Console.Write(logo[i]);
            }
            Console.SetCursorPosition(wdh, hgt + logo.Length);
            Console.Write("                                         ");
        }

        private void ClearMenu()
        {
            for (int i = 0; i < 16; i++)
            {
                Console.SetCursorPosition(Console.WindowWidth / 6, Console.WindowHeight / 2 - 3 + i);
                Console.Write("                                                        ");
            }
        }
    }
}