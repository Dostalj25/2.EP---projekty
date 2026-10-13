# Terminálové hry

Konzolové hry jsou samostatné aplikace v C#. Ke spuštění je potřeba .NET 10 SDK.

- `papousek` – zopakuje napsaný řádek; prázdný řádek ukončí program.
- `kamen-nuzky-papir` – kámen, nůžky, papír proti náhodnému tahu počítače; napiš `konec` pro ukončení.
- `piskvorky` – piškvorky proti počítači; čísla 1–9 označují políčka zleva doprava a shora dolů.
- `sibenice` – hádání slova; slova jsou uložená v `sibenice/slova.txt`.
- `sibenice-gui` – samostatná okenní verze Šibenice; spusť ji příkazem `dotnet run` ve složce projektu.
- `had-monogame` – grafická hra Had vytvořená v MonoGame; pohyb šipkami nebo WASD.

## Spuštění hry Had

```sh
cd had-monogame
dotnet run
```

Hra běží v okně. Šipkami nebo WASD ovládáš hada, mezerníkem hru pozastavíš a klávesou R začneš znovu. Náraz do stěny nebo do vlastního těla ukončí hru.

## Spuštění grafické Šibenice

1. Otevři terminál v hlavní složce projektu.
2. Přejdi do složky grafické hry:

   ```sh
   cd sibenice-gui
   ```

3. Spusť aplikaci:

   ```sh
   dotnet run
   ```

Při prvním spuštění může .NET stáhnout potřebné balíčky. Hru lze spustit také z IDE výběrem projektu `sibenice-gui/SibeniceGui.csproj` a příkazem **Run/Start**.
