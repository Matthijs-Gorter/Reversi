using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

internal class Reversi : Form
{
    private const int gridGrootte = 6;
    private const int plaatjeGrootte = gridGrootte * 80; // zodat elk vakje 80x80 pixels is
    private const int margin = 50;
    private const int breedte = plaatjeGrootte + 2 * margin;
    private const int hoogte = 800;
    
    /// <summary>
    /// null = leeg, true = rood, false = blauw
    /// </summary>
    private bool?[,] grid = new bool?[gridGrootte, gridGrootte];
    private bool roodAanDeBeurt = true; // true = rood, false = blauw
    private void klikOpGrid(int x, int y)
    {
        Debug.WriteLine($"{x}, {y}");
        if (grid[x, y] != null)
        {
            MessageBox.Show("Dit vakje is al bezet", "Oeps!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!stapIsCorrect(x, y))
        {
            MessageBox.Show("Deze zet is niet correct", "Oeps!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        nieuweZet(x, y);
        renderBallen();
    }
    private void nieuweZet(int x, int y)
    {
        grid[x,y] = roodAanDeBeurt;
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (!(dx == 0 && dy == 0))
                {
                    ikWordtIngesloten(x, y, dx, dy);
                }
            }
        }
        roodAanDeBeurt = !roodAanDeBeurt;

    }
    private bool stapIsCorrect(int x, int y)
    {
        return true;
    }

    private bool ikWordtIngesloten(int x, int y, int dx, int dy)
    {
        int nx = x + dx;
        int ny = y + dy;
        if (nx < 0 || nx >= gridGrootte || ny < 0 || ny >= gridGrootte)
        {
            return false; 
        }
        else if (grid[nx, ny] == null)
        {
            return false;
        }
        else if (grid[nx, ny] == roodAanDeBeurt)
        {
            return true;
        }
        else if (ikWordtIngesloten(nx, ny, dx, dy) == true)
        {
            grid[nx, ny] = roodAanDeBeurt;
            return true;
        }
        else
        {
            return false;
        }
    }

    

    private void renderBallen()
    {
        Debug.Write("\t Grid: \n ---------");
        for (int y = 0; y < gridGrootte; y++)
        {
            Debug.WriteLine("");
            for (int x = 0; x < gridGrootte; x++)
            {
                
                Debug.Write(grid[x, y] == null ? ' ' : grid[x, y] == true ? 'R' : 'B');
            }
        }
        Debug.WriteLine("\n ---------");
    }

    public Reversi()
    {
        this.Text = "Reversi";
        this.BackColor = Color.White;
        this.ClientSize = new Size(breedte, hoogte);
        Debug.WriteLine($"Het grid: {grid}");
        Bitmap plaatje = new Bitmap(plaatjeGrootte, plaatjeGrootte);
        Graphics g = Graphics.FromImage(plaatje);

        Pen pen = new Pen(Color.Black, 2);

        // rood en blauw in het midden van het grid zetten
        grid[gridGrootte / 2 - 1, gridGrootte / 2 - 1] = true;
        grid[gridGrootte / 2 - 1, gridGrootte / 2] = false;
        grid[gridGrootte / 2, gridGrootte / 2 - 1] = false;
        grid[gridGrootte / 2, gridGrootte / 2] = true;



        Button nieuwSpel = new Button();
        nieuwSpel.Text = "Nieuw Spel";
        nieuwSpel.Size = new Size(100, 50);
        nieuwSpel.Location = new Point(50, 50);
        nieuwSpel.Click += (sender, e) =>
        {
            MessageBox.Show("Nieuw Spel gestart!");
        };
        this.Controls.Add(nieuwSpel);

        Button help = new Button();
        help.Text = "Help";
        help.Size = new Size(100, 50);
        help.Location = new Point(250, 50);
        help.Click += (sender, e) =>
        {
            MessageBox.Show("Help informatie!");
        };
        this.Controls.Add(help);


        for (int i = 0; i <= gridGrootte; i++)
        {
            g.DrawLine(pen, i * (plaatjeGrootte / gridGrootte), 0, i * (plaatjeGrootte / gridGrootte), plaatjeGrootte);
            g.DrawLine(pen, 0, i * (plaatjeGrootte / gridGrootte), plaatjeGrootte, i * (plaatjeGrootte / gridGrootte));

        }

        g.Dispose();
        pen.Dispose();

        PictureBox box = new PictureBox();
        box.Image = plaatje;
        box.Size = new Size(plaatjeGrootte, plaatjeGrootte);
        box.Location = new Point(margin, hoogte - plaatjeGrootte - margin);
        box.MouseClick += (sender, e) =>
        {
            int x = e.X / (plaatjeGrootte / gridGrootte);
            int y = e.Y / (plaatjeGrootte / gridGrootte);
            klikOpGrid(x, y);
        };
        this.Controls.Add(box);

    }
}
