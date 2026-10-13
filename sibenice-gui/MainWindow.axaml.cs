using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Controls.Shapes;

namespace SibeniceGui;

public partial class MainWindow : Window
{
    private const int MaxChyby = 7;
    private const string CeskaAbeceda = "aábcčdďeéěfghiíjklmnňoóprřsštťuúůvyýzž";
    private readonly string[] _slova;
    private readonly HashSet<char> _uhadnuta = [];
    private readonly HashSet<char> _spatna = [];
    private readonly List<Button> _tlacitka = [];
    private readonly Random _random = new();
    private string _slovo = "";
    private bool _konecHry;

    public MainWindow()
    {
        InitializeComponent();
        string cestaKeSlovum = System.IO.Path.Combine(AppContext.BaseDirectory, "slova.txt");
        _slova = File.ReadAllLines(cestaKeSlovum)
            .Select(slovo => slovo.Trim().ToLowerInvariant())
            .Where(slovo => slovo.Length > 0)
            .ToArray();

        if (_slova.Length == 0)
        {
            MessageText.Text = "Soubor slova.txt neobsahuje žádná slova.";
            NewGameButton.IsEnabled = false;
            return;
        }

        VytvorKlavesnici();
        NovaHra();
    }

    private void VytvorKlavesnici()
    {
        foreach (char pismeno in CeskaAbeceda)
        {
            var tlacitko = new Button
            {
                Content = pismeno.ToString().ToUpperInvariant(),
                Width = 42,
                Height = 42,
                Margin = new Avalonia.Thickness(3),
                Background = Brush.Parse("#F4F1EA"),
                Foreground = Brush.Parse("#242922"),
                FontWeight = FontWeight.SemiBold
            };
            tlacitko.Click += (_, _) => ZkusPismeno(pismeno);
            _tlacitka.Add(tlacitko);
            KeyboardPanel.Children.Add(tlacitko);
        }
    }

    private void NewGame_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => NovaHra();

    private void NovaHra()
    {
        _slovo = _slova[_random.Next(_slova.Length)];
        _uhadnuta.Clear();
        _spatna.Clear();
        _konecHry = false;
        MessageText.Text = "Vyber písmeno a zkus své štěstí.";
        MessageText.Foreground = Brush.Parse("#555A50");
        foreach (Button tlacitko in _tlacitka)
        {
            tlacitko.IsEnabled = true;
            tlacitko.Opacity = 1;
        }
        AktualizujObrazovku();
    }

    private void ZkusPismeno(char pismeno)
    {
        if (_konecHry || _uhadnuta.Contains(pismeno) || _spatna.Contains(pismeno))
        {
            return;
        }

        if (_slovo.Contains(pismeno))
        {
            _uhadnuta.Add(pismeno);
            MessageText.Text = "Správně, tohle písmeno ve slově je!";
        }
        else
        {
            _spatna.Add(pismeno);
            MessageText.Text = "Tohle písmeno ve slově není.";
        }

        if (_slovo.All(_uhadnuta.Contains))
        {
            _konecHry = true;
            MessageText.Text = "Výborně, uhodl/a jsi slovo!";
            MessageText.Foreground = Brush.Parse("#36704A");
        }
        else if (_spatna.Count == MaxChyby)
        {
            _konecHry = true;
            MessageText.Text = $"Došly pokusy. Hledané slovo bylo {_slovo.ToUpperInvariant()}.";
            MessageText.Foreground = Brush.Parse("#B64A35");
        }

        AktualizujObrazovku();
    }

    private void AktualizujObrazovku()
    {
        WordText.Text = string.Join(" ", _slovo.Select(pismeno =>
            _uhadnuta.Contains(pismeno) || _konecHry && _spatna.Count == MaxChyby ? char.ToUpperInvariant(pismeno) : '_'));
        LivesText.Text = $"Zbývá {MaxChyby - _spatna.Count} pokusů";
        foreach (Button tlacitko in _tlacitka)
        {
            char pismeno = char.ToLowerInvariant(tlacitko.Content!.ToString()![0]);
            if (_uhadnuta.Contains(pismeno) || _spatna.Contains(pismeno))
            {
                tlacitko.IsEnabled = false;
                tlacitko.Opacity = 0.45;
            }
            else if (_konecHry)
            {
                tlacitko.IsEnabled = false;
                tlacitko.Opacity = 0.45;
            }
        }
        VykresliSibenici(_spatna.Count);
    }

    private void VykresliSibenici(int chyby)
    {
        HangmanCanvas.Children.Clear();
        var inkoust = Brush.Parse("#393F36");
        void Linka(double x1, double y1, double x2, double y2) => HangmanCanvas.Children.Add(new Line
        {
            StartPoint = new Avalonia.Point(x1, y1), EndPoint = new Avalonia.Point(x2, y2),
            Stroke = inkoust, StrokeThickness = 6, StrokeLineCap = PenLineCap.Round
        });

        Linka(30, 225, 205, 225);
        Linka(70, 225, 70, 25);
        Linka(70, 25, 185, 25);
        Linka(185, 25, 185, 55);
        if (chyby >= 1)
        {
            HangmanCanvas.Children.Add(new Ellipse
            {
                Width = 46, Height = 46, Stroke = inkoust, StrokeThickness = 5,
                [Canvas.LeftProperty] = 162d, [Canvas.TopProperty] = 52d
            });
        }
        if (chyby >= 2) Linka(185, 100, 185, 165);
        if (chyby >= 3) Linka(185, 115, 150, 145);
        if (chyby >= 4) Linka(185, 115, 220, 145);
        if (chyby >= 5) Linka(185, 165, 153, 205);
        if (chyby >= 6) Linka(185, 165, 217, 205);
        if (chyby >= 7)
        {
            Linka(174, 69, 181, 76);
            Linka(181, 69, 174, 76);
            Linka(190, 69, 197, 76);
            Linka(197, 69, 190, 76);
        }
    }
}
