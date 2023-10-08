using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace CzajaPoker
{
    public class Gracz
    {
        private List<Karta> rekaGracz;
        public string imiegracz { get; }

        public Gracz(string imie)
        {
            imiegracz = imie;
        }

        private void Sortowanie()
        {
            var n = rekaGracz.Count;
            for (int i = 0; i < n - 1; i++)
                for (int j = 0; j < n - i - 1; j++)
                    if (rekaGracz[j].Id > rekaGracz[j + 1].Id)
                    {
                        var tempVar = rekaGracz[j];
                        rekaGracz[j] = rekaGracz[j + 1];
                        rekaGracz[j + 1] = tempVar;
                    }
        }

        public void TaliaGracza(Talia deck)
        {
            rekaGracz = deck.DrawHand();    
            Sortowanie();
        }

        public void WymianaKart(Talia deck)
        {
            try
            {
                TaliaReka();

                Console.WriteLine(" Ile kart zmienić dla " + imiegracz + "?");
                int n = int.Parse(Console.ReadLine());

                if (n == 0)
                    return;

                Console.WriteLine(" Które chcesz wymienić ? ( Napisz ich id , np : 4 5 12");
                string zmienKarty = Console.ReadLine();

                deck.Wymienianie(ref rekaGracz, zmienKarty);
                Sortowanie();
            }
            catch (Exception)
            {
                Console.WriteLine("Coś poszło nie tak ");
            }
        }

        public void TaliaReka()
        {
            Console.WriteLine(imiegracz + "  ręka : ");
            foreach (Karta karta in rekaGracz)
            {
                Console.WriteLine(karta.ToString());
            }
            Console.WriteLine();
        }

        public int OcenaTalii()
        {
            //Straight flush
            if (rekaGracz[0].Id + 4 == rekaGracz[4].Id)
                return 8;

            //Four of a kind
            if (rekaGracz.Where(x => x.Nazwa == rekaGracz[0].Nazwa).Count() == 4 ||
                rekaGracz.Where(x => x.Nazwa == rekaGracz[1].Nazwa).Count() == 4)
            {
                return 7;
            }

            //Full house
            var fullHouse = rekaGracz.GroupBy(x => x.Nazwa)
                .Select(g => new { Value = g.Key, Count = g.Count() });

            if (fullHouse.ToList()[0].Count == 2 && fullHouse.ToList()[1].Count == 3 ||
                fullHouse.ToList()[0].Count == 3 && fullHouse.ToList()[1].Count == 2)
                return 6;

            //Flush
            if (rekaGracz.Where(x => x.Ksztalt == rekaGracz[0].Ksztalt).Count() == 5)
            {
                return 5;
            }

            //Straight
            if (rekaGracz.GroupBy(x => x.Nazwa).Select(g => new { Value = g.Key, Count = g.Count() }).ToList()[0].Count == 5)
            {
                return 4;
            }

            //Three of a kind
            var threeOfAKind = rekaGracz.GroupBy(x => x.Nazwa)
                .Select(g => new { Value = g.Key, Count = g.Count() });
            if (threeOfAKind.Where(p => p.Count == 3).Count() != 0)
            {
                return 3;
            }

            //Two pair
            int liczbaPar = rekaGracz.GroupBy(x => x.Nazwa)
                .Select(g => new { Value = g.Key, Count = g.Count() })
                .Where(p => p.Count == 2).Count();
            if (liczbaPar == 2)
            {
                return 2;
            }

            //One pair
            if (liczbaPar == 1)
            {
                return 1;
            }

            return 0;
        }
    }

    public class Talia
    {
        private List<Karta> talia = new List<Karta>();
        private List<Karta> ZamianaKart = new List<Karta>();
        private static string[] KoloryKart = new string[] { "Czerwony", "Czarny" };
        private static string[] SymbolKart = new string[] { "Pik", "Kier", "Karo", "Trefl" };
        private static string[] NazwyKart = new string[] { "9", "10", "Jopek", "Królowa", "Król", "As" };

        public Talia()
        {
            GenerujTalie();
        }

        private void SprawdzTalie()
        {
            if (talia.Count == 0)
            {
                for (int i = 0; i < ZamianaKart.Count; i++)
                {
                    talia.Add(ZamianaKart[i]);
                    ZamianaKart.Remove(ZamianaKart[i]);
                }
            }
        }

        public void GenerujTalie()
        {
            int obecnaNazwa = 0;
            int id = 1;

            //Generowanie Kier
            for (int i = 0; i < 6; i++)
            {
                talia.Add(new Karta(id, NazwyKart[obecnaNazwa], KoloryKart[0], SymbolKart[1]));
                obecnaNazwa++;
                id++;
            }

            //Generowanie Karo
            obecnaNazwa = 0;
            for (int i = 0; i < 6; i++)
            {
                talia.Add(new Karta(id, NazwyKart[obecnaNazwa], KoloryKart[0], SymbolKart[2]));
                obecnaNazwa++;
                id++;
            }

            //Generowanie Trefli
            obecnaNazwa = 0;
            for (int i = 0; i < 6; i++)
            {
                talia.Add(new Karta(id, NazwyKart[obecnaNazwa], KoloryKart[1], SymbolKart[3]));
                obecnaNazwa++;
                id++;
            }

            //Generowanie Pików
            obecnaNazwa = 0;
            for (int i = 0; i < 6; i++)
            {
                talia.Add(new Karta(id, NazwyKart[obecnaNazwa], KoloryKart[1], SymbolKart[0]));
                obecnaNazwa++;
                id++;
            }
        }

        public Karta LosowaKarta()
        {
            SprawdzTalie();
            Random random = new Random();
            return talia[random.Next(0, talia.Count)];
        }

        public List<Karta> DrawHand()
        {
            List<Karta> karty = new List<Karta>();

            Karta karta;
            for (int i = 0; i < 5; i++)
            {
                karta = LosowaKarta(); ;
                karty.Add(karta);
                talia.Remove(karta);
            }

            return karty;
        }

        public void Wymienianie(ref List<Karta> rekaGracz, string odrzuconeKarty)
        {
            Karta nowaKarta;
            foreach (var idkarta in odrzuconeKarty.Split(' '))
            {
                Karta karta = rekaGracz.Find(x => x.Id == int.Parse(idkarta));
                ZamianaKart.Add(karta);
                rekaGracz.Remove(karta);

                nowaKarta = LosowaKarta();
                rekaGracz.Add(nowaKarta);
                talia.Remove(nowaKarta);
            }
        }
    }

    public class Karta
    {
        public int Id { get; private set; }
        public string Nazwa { get; private set; }
        public string Kolor { get; private set; }
        public string Ksztalt { get; private set; }

        public Karta(int id, string nazwa, string kolor, string ksztalt)
        {
            Id = id;
            Nazwa = nazwa;
            Kolor = kolor;
            Ksztalt = ksztalt;
        }

        public override string ToString()
        {
            return "ID: " + Id + ", Nazwa: " + Nazwa + ", Kolor: " + Kolor + ", Ksztalt: " + Ksztalt;
        }
    }
    internal class Program
    {
        static string JakaTalia(int input)
        {
            switch (input)
            {
                case 1:
                    return "One pair";
                case 2:
                    return "Two pairs";
                case 3:
                    return "Three of a kind";
                case 4:
                    return "Straight";
                case 5:
                    return "Flush";
                case 6:
                    return "Full house";
                case 7:
                    return "Four of a kind";
                case 8:
                    return "Straight flush";
                default:
                    return "Nic";
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine(" Ręka pierwszego gracza : ");
            string NazwaGracz = Console.ReadLine();
            Console.WriteLine(" Ręka drugiego gracza : ");
            string NazwaGracz2 = Console.ReadLine();
            Console.WriteLine();

            Talia talia = new Talia();


            Gracz gracz = new Gracz(NazwaGracz);
            gracz.TaliaGracza(talia);

            Gracz gracz2 = new Gracz(NazwaGracz2);
            gracz2.TaliaGracza(talia);


            //Można dodać ile się chce  aby było więcej wymian lub usunąć aby było mniej

            gracz.WymianaKart(talia);
            gracz.TaliaReka();


            gracz2.WymianaKart(talia);
            gracz2.TaliaReka();


            // gracz.WymianaKart(talia);
            // gracz.TaliaReka();


            // gracz2.WymianaKart(talia);
            // gracz2.TaliaReka();



            Console.WriteLine(gracz.imiegracz + " Posiada : " + JakaTalia(gracz.OcenaTalii()));
            Console.WriteLine(gracz2.imiegracz + " Posiada : " + JakaTalia(gracz2.OcenaTalii()));

            if (gracz.OcenaTalii() > gracz2.OcenaTalii())
            {
                Console.WriteLine(gracz.imiegracz + " Wygał!");
            }
            else if (gracz.OcenaTalii() < gracz2.OcenaTalii())
            {
                Console.WriteLine(gracz2.imiegracz + " Wygrał!");
            }
            else
            {
                Console.WriteLine("Remis czy coś.");
            }
        }
    }
}