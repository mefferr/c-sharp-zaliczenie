using ConsoleGame;
using System;
using System.Collections.Generic;

namespace ConsoleGame
{
    // klasa bazowa dla wszystkich objektow gry
    public abstract class GameObject
    {
        protected int x;
        protected int y;

        public GameObject(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public abstract void Update();

        public abstract void Draw();

        public int GetX()
        {
            return x;
        }

        public int GetY()
        {
            return y;
        }
    }

    // klasa Player dziedziczy po GameObject
    public class Player : GameObject
    {
        private int health;
        private int speed;

        public Player(int x, int y, int health, int speed) : base(x, y)
        {
            this.health = health;
            this.speed = speed;
        }

        public override void Update()
        {
            // aktualizacja pozycji gracza na podstawie wprowadzanych danych
            if (Console.KeyAvailable)
            {
                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.LeftArrow:
                        x -= speed;
                        break;
                    case ConsoleKey.RightArrow:
                        x += speed;
                        break;
                }

                // ograniczanie pozycji gracza do okna gry
                if (x < 0)
                {
                    x = 0;
                }
                else if (x >= Console.WindowWidth)
                {
                    x = Console.WindowWidth - 1;
                }
            }
        }

        public override void Draw()
        {
            // rysowanie gracza jako "O"
            Console.SetCursorPosition(x, y);
            Console.Write("O");
        }

        public int GetHealth()
        {
            return health;
        }

        public void SetHealth(int health)
        {
            this.health = health;
        }
    }

    // klasa Enemy dziedziczy po GameObject
    public class Enemy : GameObject
    {
        private int health;
        private int speed;

        public Enemy(int x, int y, int health, int speed) : base(x, y)
        {
            this.health = health;
            this.speed = speed;
        }

        public override void Update()
        {
            //aktualizacja pozycji i zachowania Enemy
            y += speed; // poruszanie w dół co każdą klatkę
            x += new Random().Next(-1, 2); // losowe zmienianie położenia poziomego

            // ograniczanie położenia przeciwnika do okna gry
            if (x < 0)
            {
                x = 0;
            }
            else if (x >= Console.WindowWidth - 2) // odejmowanie 2 aby uwzględnić szerokość przeciwnika
            {
                x = Console.WindowWidth - 2;
            }
        }

        public override void Draw()
        {
            // rysowanie przeciwnika jako "XXX"
            Console.SetCursorPosition(x, y + 3);
            Console.Write("XXX");
        }

        public int GetHealth()
        {
            return health;
        }

        public void SetHealth(int health)
        {
            this.health = health;
        }
    }


    // klasa Game, zarządzanie obiektami gry i pętlą gry
    public class Game
    {
        private List<GameObject> gameObjects = new List<GameObject>();
        private Player player;
        private int score;
        private bool gameOver;

        private int frameCount = 0;
        private int enemyInterval = 20; // generowanie przeciwnika co 20 klatek
        private int maxEnemies = 5; // maksymalna ilość przeciwników na ekranie

        public Game()
        {
            player = new Player(Console.WindowWidth / 2, Console.WindowHeight - 1, 3, 1);
            gameObjects.Add(player);
            gameOver = false;
        }

        public void Run()
        {
            while (!gameOver)
            {
                // aktualizowanie objektów gry
                foreach (GameObject obj in gameObjects)
                {
                    obj.Update();
                }

                // sprawdzanie kolizji
                CheckCollisions();

                // rysowanie objektów gry
                Console.Clear();
                foreach (GameObject obj in gameObjects)
                {
                    obj.Draw();
                }

                // wyświetlanie Score i Health
                Console.SetCursorPosition(0, 2);
                Console.Write($"Score: {score} Health: {player.GetHealth()}");

                // sprawdzanie warunku dla zakończenia gry
                if (player.GetHealth() <= 0)
                {
                    gameOver = true;
                }

                // generowanie nowych przeciwników
                frameCount++;
                if (frameCount >= enemyInterval && gameObjects.Count(obj => obj is Enemy) < maxEnemies)
                {
                    gameObjects.Add(new Enemy(new Random().Next(Console.WindowWidth), 0, 1, 1));
                    frameCount = 0;
                }

                System.Threading.Thread.Sleep(7);
            }

            Console.Clear();
            Console.WriteLine("Game over!");
        }

        private void CheckCollisions()
        {
            // sprawdzanie kolizji gracza z przeciwnikiem
            foreach (GameObject obj in gameObjects)
            {
                if (obj is Enemy)
                {
                    if ((obj.GetX() == player.GetX()|| obj.GetX()-1 == player.GetX() || obj.GetX() - 2 == player.GetX() 
                        || obj.GetX() - 3 == player.GetX() || obj.GetX() + 1 == player.GetX() || obj.GetX() + 2 == player.GetX() 
                        || obj.GetX() + 3 == player.GetX()) && obj.GetY() == player.GetY())
                    {
                        player.SetHealth(player.GetHealth() - 1);
                    }
                    else if (obj.GetY() >= Console.WindowHeight - 1)
                    {
                        score++;
                    }
                }
            }

            // usuwanie zniszczonych objektów
            gameObjects.RemoveAll(obj => obj is Enemy && obj.GetY() >= Console.WindowHeight);
        }

    }


}
class Program
{
    static void Main(string[] args)
    {
        Game game = new Game();
        Console.WindowHeight = 60;
        game.Run();
    }
}
