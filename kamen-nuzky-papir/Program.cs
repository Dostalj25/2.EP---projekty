string[] moznosti = ["kámen", "nůžky", "papír"];
Random random = new();

Console.WriteLine("Kámen, nůžky, papír! Napiš kámen, nůžky nebo papír. Konec: konec.");

while (true)
{
    Console.Write("> ");
    string tah = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

    if (tah == "konec")
    {
        Console.WriteLine("Díky za hru!");
        break;
    }

    if (!moznosti.Contains(tah))
    {
        Console.WriteLine("Tomu nerozumím. Zkus kámen, nůžky nebo papír.");
        continue;
    }

    string bot = moznosti[random.Next(moznosti.Length)];
    Console.WriteLine($"Ty: {tah} | Bot: {bot}");

    if (tah == bot)
    {
        Console.WriteLine("Remíza!");
    }
    else if ((tah, bot) is ("kámen", "nůžky") or ("nůžky", "papír") or ("papír", "kámen"))
    {
        Console.WriteLine("Vyhrál/a jsi!");
    }
    else
    {
        Console.WriteLine("Vyhrál bot.");
    }
}
