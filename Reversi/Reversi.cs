using System;
using System.Drawing;
using System.Windows.Forms;

internal class Reversi : Form
{
    private int width = 600;
    private int height = 800;
    private int boardSize = 6;
    private int plaatjeGrootte = 500;

    public Reversi()
    {
        this.Text = "Reversi";
        this.BackColor = Color.White;
        this.ClientSize = new Size(width, height);

        Bitmap plaatje = new Bitmap(plaatjeGrootte, plaatjeGrootte);
        Graphics g = Graphics.FromImage(plaatje);

        Pen pen = new Pen(Color.Black, 2);


        Button nieuwSpel = new Button();
        nieuwSpel.Text = "Nieuw Spel";
        nieuwSpel.Size = new Size(100, 50);
        nieuwSpel.Location = new Point(50, 50);
        nieuwSpel.Click += (sender, e) =>
        {
            MessageBox.Show("Nieuw Spel gestart!");
        };

        this.Controls.Add(nieuwSpel);


        for (int i  = 0; i <= boardSize; i++) {
            g.DrawLine(pen, i * (plaatjeGrootte / boardSize), 0, i * (plaatjeGrootte / boardSize), plaatjeGrootte);
            g.DrawLine(pen, 0, i * (plaatjeGrootte / boardSize), plaatjeGrootte, i * (plaatjeGrootte / boardSize));

        }

        g.Dispose();
        pen.Dispose();

        PictureBox box = new PictureBox();
        box.Image = plaatje;
        box.Size = new Size(plaatjeGrootte, plaatjeGrootte);
        box.Location = new Point(50, 250);
        box.MouseClick += (sender, e) =>
        {
            int x = e.X / (plaatjeGrootte / boardSize);
            int y = e.Y / (plaatjeGrootte / boardSize);
            MessageBox.Show($"Clicked on cell: ({x}, {y})");
        };
        this.Controls.Add(box);

    }
}
