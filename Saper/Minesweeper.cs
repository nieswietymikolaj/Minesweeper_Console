using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Saper
{
    class Minesweeper
    {
        public SoundPlayer lastSong = new SoundPlayer("Music/Chiptune.wav");
        public bool musicFlag = true;
        SoundPlayer gamePlayer;

        Semaphore semaphore;
        Thread timeThread;
        Thread animationThread;

        Stopwatch stopwatch = new Stopwatch();

        public int mapSize = 0, bombAmount = 0;
        int posX = 0, posY = 0;
        static char[,] map = new char[50, 50];
        int[,] bombMap = new int[50, 50];
        static int bombsLeft = 0, playTime = 0;
        bool gameOn = false, newGame = false;
        int[] bombsList = new int[668];
        public string level = "własny";

        public Minesweeper(int mSize, int bAmount)
        {
            mapSize = mSize;
            bombAmount = bAmount;
        }

        private void Timer()
        {
            stopwatch.Start();

            while (true)
            {
                semaphore.WaitOne();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.SetCursorPosition(Console.WindowWidth / 2 + 16, 11);
                playTime = Convert.ToInt32(stopwatch.Elapsed.TotalSeconds);
                Console.WriteLine("{0}", playTime);
                semaphore.Release();
            }
        }

        private void Animation()
        {
            bool flag = true;

            while (true)
            {
                semaphore.WaitOne();
                if (flag == true)
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                Console.SetCursorPosition(Console.WindowWidth / 2 + 12, 5);
                Console.WriteLine("*");

                if (flag == true)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    flag = false;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    flag = true;
                }
                Console.SetCursorPosition(Console.WindowWidth / 2 + 30, 5);
                Console.WriteLine("*");
                Console.ResetColor();
                semaphore.Release();
                Thread.Sleep(500);
            }
        }

        public void NewGame()
        {
            stopwatch.Reset();

            semaphore = new Semaphore(1, 1);
            timeThread = new Thread(Timer);
            animationThread = new Thread(Animation);

            bombsLeft = bombAmount;
            gameOn = true;
            newGame = true;
            posX = 0;
            posY = 0;

            ConsoleKey key;

            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    bombMap[i, j] = 0;
                    map[i, j] = '#';
                }
            }

            ShowFrame();
            ShowStats();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(Console.WindowWidth / 2 + 16, 11);
            Console.WriteLine("0");
            Console.ResetColor();
            ShowMap(false);

            animationThread.Start();

            while (gameOn)
            {
                semaphore.WaitOne();
                if (IsGameWin() == true)
                {
                    GameWin();
                    break;
                }
                semaphore.Release();

                key = Console.ReadKey(true).Key;

                semaphore.WaitOne();

                switch (key)
                {
                    case ConsoleKey.UpArrow: if (posY != 0) posY--; break;
                    case ConsoleKey.LeftArrow: if (posX != 0) posX--; break;
                    case ConsoleKey.DownArrow: if (posY != mapSize - 1) posY++; break;
                    case ConsoleKey.RightArrow: if (posX != mapSize - 1) posX++; break;
                    case ConsoleKey.W: if (posY != 0) posY--; break;
                    case ConsoleKey.A: if (posX != 0) posX--; break;
                    case ConsoleKey.S: if (posY != mapSize - 1) posY++; break;
                    case ConsoleKey.D: if (posX != mapSize - 1) posX++; break;
                    case ConsoleKey.E: CheckCell(posX, posY); break;
                    case ConsoleKey.Q: FlagCell(posX, posY); break;
                    case ConsoleKey.Escape: ExitGame(); break;
                }

                if (gameOn != false)
                {
                    ShowMap(false);
                }
                semaphore.Release();
            }
        }

        private void ShowMap(bool isGameOver)
        {
            var width = Console.WindowWidth;
            var height = Console.WindowHeight;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(Console.WindowWidth / 2 + 23, 9);
            Console.WriteLine("{0}  ", bombsLeft);
            Console.ResetColor();

            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    Console.SetCursorPosition(width / 3 - (mapSize / 2 - i + 2), height / 2 - (mapSize / 2 - j));

                    if (posX == i && posY == j)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write('█');
                        Console.ResetColor();
                    }
                    else if (bombMap[i, j] == 1 && isGameOver)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                        Console.Write('*');
                        Console.ResetColor();
                    }
                    else if (map[i, j] == 'F' && !isGameOver)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                        Console.Write('F');
                        Console.ResetColor();
                    }
                    else if (map[i, j] == '#')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        Console.Write('#');
                        Console.ResetColor();
                    }
                    else if (map[i, j] == ' ')
                    {
                        Console.Write('░');
                    }
                    else
                    {
                        if (map[i, j] == '1')
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write(map[i, j]);
                            Console.ResetColor();
                        }
                        else if (map[i, j] == '2')
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(map[i, j]);
                            Console.ResetColor();
                        }
                        else if (map[i, j] == '3')
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(map[i, j]);
                            Console.ResetColor();
                        }
                        else if (map[i, j] == '4')
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.Write(map[i, j]);
                            Console.ResetColor();
                        }
                        else if (map[i, j] == '5')
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write(map[i, j]);
                            Console.ResetColor();
                        }
                        else if (map[i, j] == '6')
                        {
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.Write(map[i, j]);
                            Console.ResetColor();
                        }
                        else if (map[i, j] == '7')
                        {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.Write(map[i, j]);
                            Console.ResetColor();
                        }
                        else if (map[i, j] == '8')
                        {
                            Console.ForegroundColor = ConsoleColor.DarkMagenta;
                            Console.Write(map[i, j]);
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.Write(map[i, j]);
                        }
                    }
                }
            }
        }

        private bool IsGameWin()
        {
            int counter = 0;

            for (int i = 0; i < mapSize; i++)
            {
                for (int j = 0; j < mapSize; j++)
                {
                    if (map[i, j] == '#' || map[i, j] == 'F')
                    {
                        counter++;
                    }
                }
            }

            if (counter > bombAmount)
            {
                return false;
            }
            return true;
        }

        private void CheckCell(int posX, int posY)
        {
            if (map[posX, posY] == 'F')
            {
                return;
            }

            if (newGame)
            {
                timeThread.Start();
                InitMap(posX, posY);
                bombsLeft = bombAmount;
                playTime = 0;
                newGame = false;
            }

            if (bombMap[posX, posY] == 1)
            {
                GameLost();
                gameOn = false;
                return;
            }

            char counter = '0';

            if (map[posX, posY] != '#' && map[posX, posY] != ' ' && map[posX, posY] != 'F')
            {
                for (int i = Math.Max(0, posX - 1); i <= Math.Min(mapSize - 1, posX + 1); i++)
                {
                    for (int j = Math.Max(0, posY - 1); j <= Math.Min(mapSize - 1, posY + 1); j++)
                    {
                        if (map[i, j] == 'F')
                        {
                            counter++;
                        }
                    }
                }

                if (counter == map[posX, posY])
                {
                    for (int i = Math.Max(0, posX - 1); i <= Math.Min(mapSize - 1, posX + 1); i++)
                    {
                        for (int j = Math.Max(0, posY - 1); j <= Math.Min(mapSize - 1, posY + 1); j++)
                        {
                            if (map[i, j] == '#')
                            {
                                CheckCell(i, j);
                            }
                        }
                    }
                }
            }

            RevealField(posX, posY);
        }

        private void FlagCell(int posX, int posY)
        {
            if (map[posX, posY] == 'F')
            {
                map[posX, posY] = '#';
                bombsLeft++;
            }
            else if (map[posX, posY] == '#')
            {
                map[posX, posY] = 'F';
                bombsLeft--;
            }
            else return;
        }

        private void RevealField(int posX, int posY)
        {
            char counter = '0';

            for (int i = Math.Max(0, posX - 1); i <= Math.Min(mapSize - 1, posX + 1); i++)
            {
                for (int j = Math.Max(0, posY - 1); j <= Math.Min(mapSize - 1, posY + 1); j++)
                {
                    if (bombMap[i, j] == 1)
                    {
                        counter++;
                    }
                }
            }

            if (counter != '0')
            {
                map[posX, posY] = counter;
                return;
            }

            map[posX, posY] = ' ';

            for (int i = Math.Max(0, posX - 1); i <= Math.Min(mapSize - 1, posX + 1); i++)
            {
                for (int j = Math.Max(0, posY - 1); j <= Math.Min(mapSize - 1, posY + 1); j++)
                {
                    if (bombMap[i, j] == 0 && map[i, j] == '#')
                    {
                        RevealField(i, j);
                    }
                }
            }
        }

        private void InitMap(int posX, int posY)
        {
            Random random = new Random();

            int[] notBomb = new int[9];
            notBomb[0] = (posX * mapSize) + posY;

            for (int i = 1; i < 9; i++)
            {
                notBomb[i] = -1;
            }

            int counter = 1;

            for (int i = Math.Max(0, posX - 1); i <= Math.Min(mapSize - 1, posX + 1); i++)
            {
                for (int j = Math.Max(0, posY - 1); j <= Math.Min(mapSize - 1, posY + 1); j++)
                {
                    if (i == posX && j == posY)
                    {
                        continue;
                    }
                    notBomb[counter] = i * mapSize + j;
                    counter++;
                }
            }

            for (int i = 0; i < bombAmount; i++)
            {
                int bomb = random.Next(mapSize * mapSize);

                while (true)
                {
                    if (!Array.Exists(bombsList, element => element == bomb) && !Array.Exists(notBomb, element => element == bomb))
                    {
                        break;
                    }
                    bomb = random.Next(mapSize * mapSize);
                }

                bombsList[i] = bomb;
            }

            for (int i = 0; i < bombAmount; i++)
            {
                bombMap[bombsList[i] / mapSize, bombsList[i] % mapSize] = 1;
            }
        }

        private void GameWin()
        {
            stopwatch.Stop();
            timeThread.Abort();
            animationThread.Abort();

            posX = -1;
            posY = 0;
            ShowFrame();
            ShowStats();
            ShowMap(false);

            Console.ReadKey(true);
            AnimatedTransition(2);
            ShowInscription("ASCII/win.txt", true);
            Console.ReadKey(true);
            AnimatedTransition(0);

            Console.CursorVisible = true;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.SetCursorPosition(Console.WindowWidth / 2 - 12, 11);
            Console.Write("        GRATULACJE!       ");
            Console.SetCursorPosition(Console.WindowWidth / 2 - 12, 13);
            Console.Write("TWÓJ WYNIK TRAFI DO RANKINGU");
            Console.SetCursorPosition(Console.WindowWidth / 2 - 12, 16);
            Console.ResetColor();
            Console.Write("   NICK: >             <    ");
            Console.SetCursorPosition(Console.WindowWidth / 2 - 2, 16);

            Console.ForegroundColor = ConsoleColor.Green;

            string nick = Console.ReadLine();

            while (nick.Length == 0 || nick.Length > 13)
            {
                AnimatedTransition(0);
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.SetCursorPosition(Console.WindowWidth / 2 - 12, 19);
                Console.WriteLine(" PODANO NIEPOPRAWNY NICK! ");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.SetCursorPosition(Console.WindowWidth / 2 - 12, 11);
                Console.Write("        GRATULACJE!       ");
                Console.SetCursorPosition(Console.WindowWidth / 2 - 12, 13);
                Console.Write("TWÓJ WYNIK TRAFI DO RANKINGU");
                Console.ResetColor();
                Console.SetCursorPosition(Console.WindowWidth / 2 - 12, 16);
                Console.Write("   NICK: >             <    ");
                Console.SetCursorPosition(Console.WindowWidth / 2 - 2, 16);
                Console.ForegroundColor = ConsoleColor.Green;
                nick = Console.ReadLine();
            }
            Console.ResetColor();

            Console.CursorVisible = false;
            SaveScore(nick);

            AnimatedTransition(2);
        }

        private void GameLost()
        {
            stopwatch.Stop();
            timeThread.Abort();
            animationThread.Abort();

            posX = -1;
            posY = 0;
            ShowFrame();
            ShowStats();
            ShowMap(true);

            gameOn = false;

            Console.ReadKey(true);
            AnimatedTransition(0);
            ShowBomb();
            Console.ReadKey(true);
            AnimatedTransition(2);
            ShowInscription("ASCII/loss.txt", false);
            Console.ReadKey(true);
            AnimatedTransition(2);

            if (musicFlag == true)
            {
                lastSong.PlayLooping();
            }
        }

        private void ShowBomb()
        {
            string[] logo = File.ReadAllLines("ASCII/bomb.txt");

            for (int i = 0; i < logo.Length; i++)
            {
                if (i < 5)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                }

                Console.SetCursorPosition(Console.WindowWidth / 2 - (logo[i].Length / 2), Console.WindowHeight / 2 + i - (logo.Length / 2));
                Console.Write(logo[i]);
            }

            string[] fire =
            {
                "██    ██   ",
                "██  ██     ",
                "           ",
                "░░    ████ ",
                "           ",
                "██  ██     ",
                "██    ██   ",
                "           "
            };

            string[] fireUp =
            {
                "                  ",
                "       ██         ",
                " ██    ██    ██   ",
                "   ██      ██     ",
                "                  ",
                "██ ██  ░░  ██ ██  "
            };

            string[] fireUpClean =
{
                "                  ",
                "                  ",
                "                  ",
                "                  ",
                "                  ",
                "       ░░         "
            };

            string[] fireClean =
{
                "           ",
                "           ",
                "           ",
                "           ",
                "           ",
                "           ",
                "           "
            };

            int sleepTime = 100;

            ShowBombFire(49, 6, fire, sleepTime);
            ShowBombFire(48, 6, fire, sleepTime);
            ShowBombFire(47, 6, fire, sleepTime);
            ShowBombFire(46, 6, fire, sleepTime);
            ShowBombFire(45, 5, fire, sleepTime);
            ShowBombFire(44, 5, fire, sleepTime);
            ShowBombFire(43, 4, fire, sleepTime);
            ShowBombFire(42, 4, fire, sleepTime);
            ShowBombFire(41, 4, fire, sleepTime);
            ShowBombFire(40, 4, fire, sleepTime);
            ShowBombFire(39, 4, fire, sleepTime);
            ShowBombFire(38, 4, fire, sleepTime);
            ShowBombFire(37, 4, fire, sleepTime);
            ShowBombFire(37, 4, fireClean, 0);
            ShowBombFire(28, 3, fireUp, sleepTime);
            ShowBombFire(26, 4, fireUp, sleepTime);
            ShowBombFire(26, 5, fireUp, sleepTime);
            ShowBombFire(26, 5, fireUpClean, 500);

            gamePlayer = new SoundPlayer("Sounds/Explosion.wav");
            gamePlayer.Play();

            Console.ForegroundColor = ConsoleColor.DarkBlue;

            ShowExplosion(28, 40, 15, 22, 80);
            Thread.Sleep(50);
            ShowExplosion(23, 45, 13, 25, 160);
            Thread.Sleep(50);
            ShowExplosion(18, 50, 10, 28, 320);
            Thread.Sleep(50);
            ShowExplosion(13, 55, 7, 28, 640);
            Thread.Sleep(50);
            ShowExplosion(8, 60, 4, 28, 1000);
            Thread.Sleep(50);
            ShowExplosion(3, 69, 2, 28, 1000);
            Thread.Sleep(50);
            ShowExplosion(3, 78, 2, 28, 1000);

            Console.ResetColor();
        }

        private void ShowExplosion(int x1, int x2, int y1, int y2, int length)
        {
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                Console.SetCursorPosition(random.Next(x1, x2), random.Next(y1, y2));
                Console.WriteLine((char)random.Next(60, 100));
            }
        }

        private void ShowBombFire(int x, int y, string[] fire, int sleepTime)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;

            for (int i = 0; i < fire.Length; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write(fire[i]);
            }
            Thread.Sleep(sleepTime);
        }

        private void ShowInscription(string path, bool color)
        {
            string[] logo = File.ReadAllLines(path);

            if (color == true)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
            }

            for (int i = 0; i < logo.Length; i++)
            {
                Console.SetCursorPosition(Console.WindowWidth / 2 - (logo[i].Length / 2), Console.WindowHeight / 2 + i - (logo.Length / 2) - 1);
                Console.Write(logo[i]);
            }
        }

        private void SaveScore(string nick)
        {
            StreamWriter writer = File.AppendText("ASCII/scoreboard.txt");
            writer.WriteLine(playTime.ToString().PadLeft(4, '0') + " " + nick + " " + level.ToUpper() + " " + bombAmount + " " + mapSize);
            writer.Close();
        }

        private void ExitGame()
        {
            stopwatch.Stop();
            int choice = 0;
            int choiceMax = 2;
            ConsoleKey key;

            AnimatedTransition(0);

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.SetCursorPosition(Console.WindowWidth / 2 - 14, 14);
            Console.Write("CZY CHCESZ ZAKOŃCZYĆ ROZGRYWKĘ?");

            while (true)
            {
                string[] options = { "NIE", "TAK" };

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

                    Console.SetCursorPosition(Console.WindowWidth / 2 - (options[0].Length / 2), 17 + (i * 2));
                    Console.Write(options[i]);
                }

                key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.W: if (--choice == -1) choice = choiceMax - 1; break;
                    case ConsoleKey.S: if (++choice == choiceMax) choice = 0; break;
                    case ConsoleKey.E:
                        switch (choice)
                        {
                            case 0: AnimatedTransition(0); stopwatch.Start(); ShowFrame(); ShowStats(); ShowMap(false); return;
                            case 1: AnimatedTransition(0); gameOn = false; timeThread.Abort(); animationThread.Abort(); return;
                        }
                        break;
                }
            }
        }

        private void ShowFrame()
        {
            int widthMin = Console.WindowWidth / 3 - (mapSize / 2) - 4;
            int heightMin = Console.WindowHeight / 2 - (mapSize / 2) - 1;

            int widthMax = Console.WindowWidth / 3 + (mapSize / 2);
            int heightMax = Console.WindowHeight / 2 + (mapSize / 2) + 1;

            Console.ForegroundColor = ConsoleColor.Magenta;

            for (int x = widthMin; x < widthMax; x++)
            {
                for (int y = heightMin; y < heightMax; y++)
                {
                    Console.SetCursorPosition(x, y);

                    if (y == heightMin && x == widthMin)
                        Console.Write('╬');
                    else if (y == heightMin && x == widthMax - 1)
                        Console.Write('╬');
                    else if (y == heightMax - 1 && x == widthMin)
                        Console.Write('╬');
                    else if (y == heightMax - 1 && x == widthMax - 1)
                        Console.Write('╬');
                    else if ((x == widthMin || x == widthMax - 1) && y > heightMin)
                        Console.Write('║');
                    else if ((y == heightMin || y == heightMax - 1) && x > widthMin)
                        Console.Write('═');
                }
            }

            Console.ResetColor();
        }

        private void ShowStats()
        {
            var width = Console.WindowWidth / 2 + 6;
            var height = 6;

            Console.SetCursorPosition(width, height);
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("           SAPER          ");
            Console.SetCursorPosition(width - 1, height + 1);
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("============================");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(width, height + 3);
            Console.WriteLine("Pozostałe bomby: ");
            Console.SetCursorPosition(width, height + 5);
            Console.WriteLine("Czas gry: ");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.SetCursorPosition(width, height + 7);
            Console.WriteLine("P. trudności: {0}", level.ToUpper());
            Console.SetCursorPosition(width, height + 8);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("- rozmiar mapy: {0} x {1}  ", mapSize, mapSize);
            Console.SetCursorPosition(width, height + 9);
            Console.WriteLine("- ilość bomb: {0}         ", bombAmount);
            Console.SetCursorPosition(width, height + 11);
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Jak grać? ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.SetCursorPosition(width, height + 12);
            Console.WriteLine(" [E] - sprzawdź pole");
            Console.SetCursorPosition(width, height + 13);
            Console.WriteLine(" [Q] - oznacz pole");
            Console.SetCursorPosition(width, height + 14);
            Console.WriteLine(" [Esc] - wyjdź do menu");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.SetCursorPosition(width - 1, height + 16);
            Console.WriteLine("============================");
            Console.ResetColor();
            ShowBombs();
        }

        private void ShowBombs()
        {
            string[] bomb =
            {
                " ,-*",
                "(_)",
            };

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.SetCursorPosition(Console.WindowWidth / 2 + 9, 5);
            Console.Write(bomb[0]);
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.SetCursorPosition(Console.WindowWidth / 2 + 9, 6);
            Console.Write(bomb[1]);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.SetCursorPosition(Console.WindowWidth / 2 + 27, 5);
            Console.Write(bomb[0]);
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.SetCursorPosition(Console.WindowWidth / 2 + 27, 6);
            Console.Write(bomb[1]);
            Console.ResetColor();
        }

        public void AnimatedTransition(int sleepTime)
        {
            Console.ForegroundColor = ConsoleColor.Black;

            int pomA = 3, pomB = Console.WindowWidth - 3;

            for (int i = 1; i < Console.WindowWidth / 2; i++)
            {
                for (int j = 2; j < Console.WindowHeight - 2; j++)
                {
                    Console.SetCursorPosition(pomA, j);
                    Console.Write("░");
                    Console.SetCursorPosition(pomB, j);
                    Console.Write("░");
                }
                pomA++;
                pomB--;

                Thread.Sleep(sleepTime);
            }

            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
