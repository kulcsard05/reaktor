using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    class Reaktor
    {
        private bool mukodik;
        private int homerseklet;
        private int energia;

        public bool Mukodik { get => mukodik; }
        public int Homerseklet { get => homerseklet; }
        public int Energia { get => energia; }

        public Reaktor()
        {
            mukodik = false;
            homerseklet = 40;
            energia = 0;
        }
        
        public void Beinditas()
        {
            Random rnd = new Random();

            Console.WriteLine("A reaktor beindul...");

            Thread.Sleep(rnd.Next(1000, 5000));

            mukodik = true;
            
            homerseklet = rnd.Next(40, 101);

            // energia generálás, előző értéknél nagyobb, but within a smaller range
            int maxEnergia = energia + 10 > 100 ? 100 : energia + 10; // ensure the max value doesn't exceed 100
            energia = rnd.Next(energia + 1, maxEnergia);
        }

        public void Leallitas()
        {
            Console.WriteLine("A reaktor leállítása...");

            if(mukodik == true)
            {
                if (homerseklet <= 70)
                {
                    mukodik = false;
                    Console.WriteLine("A reaktor leállt.");
                }
                else
                {
                    Console.WriteLine("A reaktor hőmérséklete túl magas, hűtse le a rendszert.");
                }
            }
        }

        public void HutovizBeengedese()
        {
            if (mukodik == true)
            {
                //fokozatosan engedjük be a hűtővizet
                Random rnd = new Random();

                while (homerseklet > 40)
                {
                    homerseklet--;

                    Console.Clear();
                    Console.WriteLine("Hűtővíz beengedése...");
                    Console.WriteLine($"Hőmérséklet: {homerseklet} fok");

                    Thread.Sleep(rnd.Next(100, 500));
                }
            }
            else
            {
                Console.WriteLine("majd lehűl magától.");
            }
        }

        public void Melegites()
        {
            Random rnd = new Random();

            if (mukodik == true)
            {
                
                homerseklet++;
                Thread.Sleep(rnd.Next(100, 500));
            }
        }

        public void Menu() {
            Console.Clear();
            Console.WriteLine($"Hőmérséklet: {homerseklet} fok | Energia: {energia} GW");

            Console.WriteLine();

            Console.WriteLine("1. Beindítás");
            Console.WriteLine("2. Leállítás");
            Console.WriteLine("3. Hűtővíz beengedése");
            Console.WriteLine("4. Kilépés");

            Console.WriteLine("Válasszon menüpontot: ");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Reaktor reaktor = new Reaktor();

            // Create a blocking collection to hold the keys that are read
            BlockingCollection<ConsoleKeyInfo> keyQueue = new BlockingCollection<ConsoleKeyInfo>();

            // Start a new task to read keys
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    var key = Console.ReadKey(true);
                    keyQueue.Add(key);
                }
            });

            // Always on display
            int prevHomerseklet = reaktor.Homerseklet;
            int prevEnergia = reaktor.Energia;
            reaktor.Menu();
            while (true)
            {
                // Reaktor info
                if (reaktor.Homerseklet != prevHomerseklet || reaktor.Energia != prevEnergia)
                {
                    prevHomerseklet = reaktor.Homerseklet;
                    prevEnergia = reaktor.Energia;

                    reaktor.Menu();
                }

                // Check if a key has been pressed
                if (keyQueue.TryTake(out ConsoleKeyInfo input, TimeSpan.FromMilliseconds(100)))
                {
                    Console.Clear();

                    switch (input.Key)
                    {
                        case ConsoleKey.D1:
                            reaktor.Beinditas();
                            break;
                        case ConsoleKey.D2:
                            reaktor.Leallitas();
                            break;
                        case ConsoleKey.D3:
                            reaktor.HutovizBeengedese();
                            break;
                        case ConsoleKey.D4:
                            Environment.Exit(0);
                            break;
                    }
                }

                // robbanás

                // melegítjük energia számítással
                reaktor.Melegites();

                if (reaktor.Homerseklet > 100)
                {
                    Console.Clear();
                    Console.WriteLine("A reaktor felrobbant!");
                    Console.WriteLine($"energia: {reaktor.Energia} GW");
                    break;
                }
            }

            Console.ReadKey();
        }
    }
}