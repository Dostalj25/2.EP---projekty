string cestaKeSlovum = Path.Combine(AppContext.BaseDirectory, "slova.txt");

if (!File.Exists(cestaKeSlovum))
{
    Console.WriteLine("Soubor slova.txt nebyl nalezen.");
    return;
}

string[] slova = File.ReadAllLines(cestaKeSlovum)
    .Select(slovo => slovo.Trim().ToLowerInvariant())
    .Where(slovo => slovo.Length > 0)
    .ToArray();

if (slova.Length == 0)
{
    Console.WriteLine("Soubor slova.txt neobsahuje žádná slova.");
    return;
}

Random random = new();
Console.WriteLine("Šibenice! Hádej písmena. Napiš konec, pokud chceš skončit.");

while (true)
{
    string slovo = slova[random.Next(slova.Length)];
    HashSet<char> uhadnuta = [];
    HashSet<char> spatna = [];
    bool vyhra = false;

    while (spatna.Count < 7)
    {
        string zobrazeni = string.Join(' ', slovo.Select(pismeno => uhadnuta.Contains(pismeno) ? pismeno : '_'));
        Console.WriteLine($"\nSlovo: {zobrazeni} | Chyby: {spatna.Count}/7");

        if (slovo.All(uhadnuta.Contains))
        {
            Console.WriteLine("Správně, vyhrál/a jsi!");
            vyhra = true;
            break;
        }

        Console.Write("Písmeno: ");
        string tip = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

        if (tip == "konec")
        {
            return;
        }

        if (tip.Length != 1 || !char.IsLetter(tip[0]))
        {
            Console.WriteLine("Zadej jedno písmeno.");
        }
        else if (uhadnuta.Contains(tip[0]) || spatna.Contains(tip[0]))
        {
            Console.WriteLine("Tohle písmeno už jsi zkoušel/a.");
        }
        else if (slovo.Contains(tip[0]))
        {
            uhadnuta.Add(tip[0]);
            Console.WriteLine("Písmeno ve slově je!");
        }
        else
        {
            spatna.Add(tip[0]);
            Console.WriteLine("To písmeno ve slově není.");
        }
    }

    if (!vyhra)
    {
        Console.WriteLine($"Došly pokusy. Hledané slovo bylo: {slovo}.");
    }

    Console.Write("Další kolo? (ano/ne): ");
    if ((Console.ReadLine() ?? "").Trim().ToLowerInvariant() is not ("ano" or "a" or "jo"))
    {
        break;
    }
}
