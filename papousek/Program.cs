Console.WriteLine("Papoušek opakuje, co napíšeš. Konec ukončíš prázdným řádkem.");

while (true)
{
    Console.Write("> ");
    string? text = Console.ReadLine();

    if (string.IsNullOrEmpty(text))
    {
        break;
    }

    Console.WriteLine(text);
}
