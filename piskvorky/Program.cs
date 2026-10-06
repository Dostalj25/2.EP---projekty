int[][] vyherniKombinace =
[
    [0, 1, 2], [3, 4, 5], [6, 7, 8],
    [0, 3, 6], [1, 4, 7], [2, 5, 8],
    [0, 4, 8], [2, 4, 6]
];
Random random = new();

Console.WriteLine("Piškvorky: ty hraješ X, bot O. Vyber políčko číslem 1–9.");

while (true)
{
    char[] pole = [' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' '];
    bool konecHry = false;

    while (!konecHry)
    {
        VypisPole(pole);
        int tah;

        while (true)
        {
            Console.Write("Tvoje políčko (1–9): ");
            string vstup = Console.ReadLine() ?? "";

            if (int.TryParse(vstup, out int cislo) && cislo is >= 1 and <= 9 && pole[cislo - 1] == ' ')
            {
                tah = cislo - 1;
                break;
            }

            Console.WriteLine("Zadej číslo volného políčka od 1 do 9.");
        }

        pole[tah] = 'X';

        if (MaViteze(pole, vyherniKombinace, 'X'))
        {
            VypisPole(pole);
            Console.WriteLine("Vyhrál/a jsi!");
            konecHry = true;
            continue;
        }

        int[] volnaPolicka = Enumerable.Range(0, 9).Where(i => pole[i] == ' ').ToArray();

        if (volnaPolicka.Length == 0)
        {
            VypisPole(pole);
            Console.WriteLine("Remíza!");
            konecHry = true;
            continue;
        }

        int tahBota = volnaPolicka[random.Next(volnaPolicka.Length)];
        pole[tahBota] = 'O';
        Console.WriteLine($"Bot zvolil políčko {tahBota + 1}.");

        if (MaViteze(pole, vyherniKombinace, 'O'))
        {
            VypisPole(pole);
            Console.WriteLine("Vyhrál bot.");
            konecHry = true;
        }
    }

    Console.Write("Zahrát znovu? (ano/ne): ");
    if ((Console.ReadLine() ?? "").Trim().ToLowerInvariant() is not ("ano" or "a" or "jo"))
    {
        break;
    }
}

static void VypisPole(char[] pole)
{
    string[] bunky = pole.Select((znak, index) => znak == ' ' ? (index + 1).ToString() : znak.ToString()).ToArray();
    Console.WriteLine($"\n {bunky[0]} | {bunky[1]} | {bunky[2]}\n---+---+---\n {bunky[3]} | {bunky[4]} | {bunky[5]}\n---+---+---\n {bunky[6]} | {bunky[7]} | {bunky[8]}\n");
}

static bool MaViteze(char[] pole, int[][] kombinace, char hrac)
{
    return kombinace.Any(k => k.All(index => pole[index] == hrac));
}
