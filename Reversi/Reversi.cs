using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

internal class Reversi : Form
{
    const int gridGrootte = 6;
    const int plaatjeGrootte = gridGrootte * 80; // zodat elk vakje 80x80 pixels is
    const int margin = 50;
    const int breedte = plaatjeGrootte + 2 * margin;
    const int hoogte = 800;

    // mogelijke statussen van een vakje
    enum VakStatus
    {
        leeg,
        rood,
        blauw,
        legaleZet
    }

    VakStatus[,] grid = new VakStatus[gridGrootte, gridGrootte];


    // maakt een knop met sjabloon
    Button maakKnop(string tekst, Point locatie, Color kleur)
    {
        Button knop = new Button();
        knop.Text = tekst;
        knop.Size = new Size(120, 45);
        knop.Location = locatie;
        knop.Font = new Font("Arial", 11, FontStyle.Bold);
        knop.BackColor = kleur;
        knop.ForeColor = Color.White;
        knop.Cursor = Cursors.Hand;

        return knop;
    }
    // maakt een label met sjabloon
    Label maakLabel(Label label, string tekst, Point locatie, Color kleur, Font font)
    {
        label.Text = tekst;
        label.Font = font;
        label.ForeColor = kleur;
        label.Size = new Size(120, 40);
        label.Location = locatie;
        label.TextAlign = ContentAlignment.MiddleCenter;
        label.BorderStyle = BorderStyle.FixedSingle;
        return label;
    }
    // speler die aan de beurt is, labels worden meteen aangepast bij wisselen
    VakStatus _speler = VakStatus.rood;
    VakStatus speler
    {
        get { return _speler; }
        set
        {
            _speler = value;
            updateLabels();
        }
    }


    // labels:
    Font dikFont = new Font("Arial", 16, FontStyle.Bold);
    Font normaalFont = new Font("Arial", 14);
    Label roodLabel = new Label();
    Label blauwLabel = new Label();
    // score teller
    Label roodTellerLabel = new Label(); 
    Label blauwTellerLabel = new Label();

    PictureBox box;

    // wordt aangeroepen als er op een vakje van het grid geklikt wordt
    void klikOpGrid(int x, int y)
    {
        Debug.WriteLine($"{x}, {y}");

        // een bezet vakje mag niet gekozen worden
        if (grid[x, y] == VakStatus.rood || grid[x, y] == VakStatus.blauw)
        {
            MessageBox.Show("Dit vakje is al bezet", "Oeps!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }


        nieuweZet(x, y);
        renderBallen();
    }

    // doet een zet: controleert of hij mag, draait stenen om en wisselt van speler
    void nieuweZet(int x, int y)
    {
        // zet de bal op het grid in de goede kleur
        grid[x, y] = speler;

        // zet is ongeldig als er niks wordt ingesloten, dan weer terugzetten
        if (!stapIsCorrect(x, y))
        {
            MessageBox.Show("Deze zet is niet correct", "Oeps!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            grid[x, y] = VakStatus.leeg;
            return;
        }

        // loop door alle richtingen
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (!(dx == 0 && dy == 0))
                {
                    // kijk of er stenen worden ingesloten en draai ze om
                    kanInSluiten(x, y, dx, dy, 0, true);
                }
            }
        }

        // andere speler is aan de beurt
        speler = speler == VakStatus.rood ? VakStatus.blauw : VakStatus.rood;

    }

    // geeft true als er in minstens één richting stenen worden ingesloten
    bool stapIsCorrect(int x, int y)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (!(dx == 0 && dy == 0))
                {
                    if (kanInSluiten(x, y, dx, dy))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    void updateLabels()
    {

        // maakt de naam van de speler die aan de beurt is dik
        if (speler == VakStatus.rood)
        {
            roodLabel.Font = dikFont;
            blauwLabel.Font = normaalFont;
        }
        else
        {
            roodLabel.Font = normaalFont;
            blauwLabel.Font = dikFont;
        }

        // update de score tellers
        roodTellerLabel.Text = tel(VakStatus.rood).ToString();
        blauwTellerLabel.Text = tel(VakStatus.blauw).ToString();

    }

    // kijkt in een richting (dx, dy) of er stenen van de tegenstander worden ingesloten
    // door een eigen steen. Bij omdraaien = true worden die stenen ook omgedraaid.
    bool kanInSluiten(int x, int y, int dx, int dy, int aantalInGeslotenVakjes = 0, bool omdraaien = false)
    {
        // volgende vakje in deze richting
        int nx = x + dx;
        int ny = y + dy;

        // buiten het grid
        if (nx < 0 || nx >= gridGrootte || ny < 0 || ny >= gridGrootte)
        {
            return false;
        }
        // leeg vakje
        else if (grid[nx, ny] == VakStatus.leeg || grid[nx, ny] == VakStatus.legaleZet)
        {
            return false;
        }
        // eigen steen 
        else if (grid[nx, ny] == speler)
        {
            return aantalInGeslotenVakjes > 0;
        }
        // steen van de tegenstander
        // roep functie opnieuw aan een stap verder in dezelfde richting om te kijken of het word ingesloten
        else if (kanInSluiten(nx, ny, dx, dy, aantalInGeslotenVakjes + 1, omdraaien) == true)
        {
            // deze steen wordt ingesloten dus draai hem om als omdraaien = true
            if (omdraaien) grid[nx, ny] = speler;
            return true;
        }
        else // steen van de tegenstander maar er wordt niks ingesloten
        {
            return false;
        }
    }

    // print het grid in de debug-output, berekent de legale zetten en tekent opnieuw
    void renderBallen()
    {
        //Debug.Write("\t Grid: \n ---------");

        //for (int y = 0; y < gridGrootte; y++)
        //{
        //    Debug.WriteLine("");

        //    for (int x = 0; x < gridGrootte; x++)
        //    {
        //        Debug.Write(grid[x, y] == VakStatus.leeg ? ' ' : grid[x, y] == VakStatus.rood ? 'R' : 'B');
        //    }
        //}

        //Debug.WriteLine("\n ---------");
        if (!berekenLegaleZetten())
        {
            eindigSpel();
        }
        box.Invalidate();
    }

    // markeert alle vakjes waar de huidige speler mag zetten en returnt true als er minstens één legale zet is
    bool berekenLegaleZetten()
    {
        bool legaleZet = false;

        // loop door de hele grid
        for (int x = 0; x < gridGrootte; x++)
        {
            for (int y = 0; y < gridGrootte; y++)
            {
                if (grid[x, y] == VakStatus.leeg || grid[x, y] == VakStatus.legaleZet)
                {
                    if (stapIsCorrect(x, y))
                    {
                        // markeer als legale zet wanneer stap correct is
                        grid[x, y] = VakStatus.legaleZet;
                        legaleZet = true;
                    } else
                    {
                        // haal de markering weg als het geen legale zet is
                        grid[x, y] = VakStatus.leeg;
                    }
                }
            }
        }
        return legaleZet;
    }

    int tel(VakStatus kleur)
    {
        int teller = 0;
        // loop door de hele grid
        for (int x = 0; x < gridGrootte; x++)
        {
            for (int y = 0; y < gridGrootte; y++)
            {
                if (grid[x, y] == kleur)
                {
                    teller++;
                }
            }
        }
        return teller;
    }

    // eindigt spel en toont een bericht met de winnaar
    void eindigSpel()
    {
        int rood = tel(VakStatus.rood);
        int blauw = tel(VakStatus.blauw);
        // kijkt wie er gewonnen heeft en toont een bericht
        string winnaar;
        if (rood > blauw)
        {
            winnaar = "Rood wint!";
        }
        else if (blauw > rood)
        {
            winnaar = "Blauw wint!";
        }
        else
        {
            winnaar = "Gelijkspel!";
        }
        MessageBox.Show($"Einde van het spel!\nRood: {rood}\nBlauw: {blauw}\n{winnaar}", "Einde van het spel", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    // begin een nieuw spel met een leeg grid en de beginstenen
    void nieuwSpel()
    {
        // grid leegmaken
        grid = new VakStatus[gridGrootte, gridGrootte];

        // rood en blauw in het midden van het grid zetten
        grid[gridGrootte / 2 - 1, gridGrootte / 2 - 1] = VakStatus.rood;
        grid[gridGrootte / 2 - 1, gridGrootte / 2] = VakStatus.blauw;
        grid[gridGrootte / 2, gridGrootte / 2 - 1] = VakStatus.blauw;
        grid[gridGrootte / 2, gridGrootte / 2] = VakStatus.rood;

        speler = VakStatus.rood;
        renderBallen();
    }

    public Reversi()
    {
        this.Text = "Reversi";
        this.BackColor = Color.White;
        this.ClientSize = new Size(breedte, hoogte);

        Debug.WriteLine($"Het grid: {grid}");

        // plaatje met de lijnen van het grid
        Bitmap plaatje = new Bitmap(plaatjeGrootte, plaatjeGrootte);
        Graphics g = Graphics.FromImage(plaatje);
        Pen pen = new Pen(Color.Black, 2);


        // knoppen maken
        Button nieuwSpelKnop = maakKnop("Nieuw Spel", new Point(50, 600), Color.SeaGreen);
        nieuwSpelKnop.Click += (sender, e) =>
        {
            nieuwSpel();
        };
        this.Controls.Add(nieuwSpelKnop);

        Button help = maakKnop("Spelregels", new Point(410, 600), Color.Purple);
        help.Click += (sender, e) =>
        {
            if (MessageBox.Show("Wil je de volledige spelregels openen in je browser?", "Spelregels", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                Process.Start(new ProcessStartInfo("https://brainking.com/nl/GameRules?tp=10") { UseShellExecute = true });
            }
        };
        this.Controls.Add(help);

        // labels voor de spelers
        maakLabel(roodLabel, "Rood", new Point(50, 20), Color.Red, dikFont);
        this.Controls.Add(roodLabel);

        maakLabel(blauwLabel, "Blauw", new Point(410, 20), Color.Blue, normaalFont);
        this.Controls.Add(blauwLabel);

        maakLabel(roodTellerLabel, "0", new Point(50, 60), Color.Red, dikFont);
        this.Controls.Add(roodTellerLabel);
        
        maakLabel(blauwTellerLabel, "0", new Point(410, 60), Color.Blue, dikFont);
        this.Controls.Add(blauwTellerLabel);

        // horizontale en verticale lijnen tekenen
        for (int i = 0; i <= gridGrootte; i++)
        {
            g.DrawLine(pen, i * (plaatjeGrootte / gridGrootte), 0,
                i * (plaatjeGrootte / gridGrootte), plaatjeGrootte);

            g.DrawLine(pen, 0, i * (plaatjeGrootte / gridGrootte),
                plaatjeGrootte, i * (plaatjeGrootte / gridGrootte));
        }

        g.Dispose();
        pen.Dispose();

        box = new PictureBox();
        box.Image = plaatje;
        box.Size = new Size(plaatjeGrootte, plaatjeGrootte);
        box.Location = new Point(margin, 100);

        // muispositie omrekenen naar vakje in het grid
        box.MouseClick += (sender, e) =>
        {
            // c# rond automisch af naar beneden bij integer deling
            int x = e.X / (plaatjeGrootte / gridGrootte);
            int y = e.Y / (plaatjeGrootte / gridGrootte);

            klikOpGrid(x, y);
        };

        // stenen en legale zetten tekenen
        box.Paint += (sender, e) =>
        {
            int vak = plaatjeGrootte / gridGrootte;

            for (int x = 0; x < gridGrootte; x++)
            {
                for (int y = 0; y < gridGrootte; y++)
                {


                Brush kwast;
                switch (grid[x, y])
                {
                    case VakStatus.rood:
                        kwast = Brushes.Red;
                        break;
                    case VakStatus.blauw:
                        kwast = Brushes.Blue;
                        break;
                    case VakStatus.legaleZet:
                        kwast = (speler == VakStatus.rood ? Brushes.LightCoral : Brushes.LightBlue);
                        break;
                    default: // bij een leeg vakje, geen kwast gebruiken
                        continue;
                }
                e.Graphics.FillEllipse(kwast, x * vak + 5, y * vak + 5, vak - 10, vak - 10);

                }
            }
        };

        this.Controls.Add(box);
        nieuwSpel();
    }
}