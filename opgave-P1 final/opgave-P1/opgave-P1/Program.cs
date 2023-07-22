using System;
using System.Drawing;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Button = System.Windows.Forms.Button;
using TextBox = System.Windows.Forms.TextBox;

// creating the User Interface

Form scherm = new Form();
scherm.BackColor = Color.White;
scherm.ClientSize = new Size(450, 600);
scherm.Text = "Mandelbrot";

Label midden_x = new Label();
scherm.Controls.Add(midden_x);
midden_x.Location = new Point(10,6);
midden_x.Text = "Midden x";

TextBox input_x = new TextBox();
scherm.Controls.Add(input_x);
input_x.Location = new Point(110, 6);
input_x.Size = new Size(150, 20);

Label midden_y = new Label();
scherm.Controls.Add(midden_y);
midden_y.Location = new Point(10, 34);
midden_y.Text = "Midden y";

TextBox input_y = new TextBox();
scherm.Controls.Add(input_y);
input_y.Location = new Point(110, 34);
input_y.Size = new Size(150, 20);

Label schaallabel = new Label();
scherm.Controls.Add(schaallabel);
schaallabel.Location = new Point(10, 62);
schaallabel.Text = "Schaal";

TextBox input_schaal = new TextBox();
scherm.Controls.Add(input_schaal);
input_schaal.Location = new Point(110, 62);
input_schaal.Size = new Size(150, 20);

Label max_iter = new Label();
scherm.Controls.Add(max_iter);
max_iter.Location = new Point(10, 90);
max_iter.Text = "Max aantal iteraties";

TextBox input_max_iter = new TextBox();
scherm.Controls.Add(input_max_iter);
input_max_iter.Location = new Point(110, 90);
input_max_iter.Size = new Size(60, 20);

Button draw_mandelbrot = new Button();
scherm.Controls.Add(draw_mandelbrot);
draw_mandelbrot.Text = "Teken Mandelbrot";
draw_mandelbrot.Location = new Point(175, 90);
draw_mandelbrot.Size = new Size(50, 20);

PictureBox pictureBox1 = new PictureBox();

pictureBox1.Size = new Size(440, 465);
pictureBox1.Location = new Point(5, 130);
scherm.Controls.Add(pictureBox1);

Bitmap mb = new Bitmap(440, 465);

pictureBox1.Image = mb;

// defining global variables

double grootte_bitmap = mb.Width;
double zoomfactor = 1;
double verschil_middel_x = 0;
double verschil_middel_y = 0;

// function to calculate a from f(a,b)
double calculate_a(double x, double y, double a, double b, int breedte)
{
    double input_schaal_number = double.Parse(input_schaal.Text);
    // coordinaat x = verschil x met het midden van x. Daarna + de breedte van de huidige x. Disclaimer: Het gaat hier ergens mis, maar we kunnen niet vinden wat, hij zoomt wel in alleen de x en y worden net verkeerd weergegeven.
    x = zoomfactor * (verschil_middel_x * input_schaal_number + (x - (0.5 * grootte_bitmap)) * input_schaal_number + x + breedte * input_schaal_number);
    double calculateda = a * a - b * b + x;
    return calculateda;
}

// function to calculate b from f(a,b)
double calculate_b(double x, double y, double a, double b, int hoogte)
{
    double input_schaal_number = double.Parse(input_schaal.Text);
    y = zoomfactor * (verschil_middel_y * input_schaal_number + (y - (0.5 * grootte_bitmap)) * input_schaal_number + y + hoogte * input_schaal_number);
    double calculatedb = 2 * a * b + y;
    return calculatedb;
}

// function to calculate f from f(a,b)
double calculate_distance_f(double calculateda, double calculatedb)
{
    double distance = Math.Sqrt(Math.Pow(calculateda, 2) + Math.Pow(calculatedb, 2));
    return distance;
}

// function to calculate mandelgetal from f(a,b)
int calculate_mandelgetal(int breedte, int hoogte)
{
    // defining variables
    int mandelgetal = 1;
    int iterations = 0;
    double input_x_number = double.Parse(input_x.Text);
    double input_max_iter_number = double.Parse(input_max_iter.Text);
    double max_iterations = input_max_iter_number;
    double input_y_number = double.Parse(input_y.Text);
    double x = input_x_number;
    double y = input_y_number;
    double a = 0;
    double b = 0;
    double calculateda = calculate_a(x, y, a, b, breedte);
    double calculatedb = calculate_b(x, y, a, b, hoogte);
    double distance = calculate_distance_f(calculateda, calculatedb);

    // looping through the formula to get the mandelgetal
    while (distance < 2 && iterations < max_iterations)
    {
        mandelgetal++;
        iterations++;
        calculateda = calculate_a(x, y, a, b, breedte);
        calculatedb = calculate_b(x, y, a, b, hoogte);
        
        distance = calculate_distance_f(calculateda, calculatedb);
        a = calculateda;
        b = calculatedb;
    }

    // defining color scheme
    int red = (int)Math.Abs(iterations / max_iterations * 255);
    int green = (int)Math.Abs(iterations / max_iterations * 255 * 0.2);
    int blue = (int)Math.Abs(iterations / max_iterations * 255 * 0.3);

    mb.SetPixel(breedte, hoogte, Color.FromArgb(red, green, blue));

    return mandelgetal;
}

// function to draw the mandelbrot
void teken_mandelbrot(object o, EventArgs ea)
{
    int breedte = 0;
    int hoogte = 0;
    for (breedte = 0; breedte < mb.Width; breedte++)
    {
        for (hoogte = 0; hoogte < mb.Height; hoogte++)
        {
            calculate_mandelgetal(breedte, hoogte);
        }
    }
    pictureBox1.Invalidate();

}

// function to use when clicked on the bitmap to zoom in or out
void mouseclicks(Object sender, MouseEventArgs e)
{
    if (e.Button == MouseButtons.Right)
    {
        double input_schaal_number = double.Parse(input_schaal.Text);
        verschil_middel_x = e.X - (0.5 * grootte_bitmap);
        verschil_middel_y = e.Y - (0.5 * grootte_bitmap);
        zoomfactor = zoomfactor * 0.5;
        pictureBox1.MouseClick += teken_mandelbrot;
        pictureBox1.Invalidate();
    }
    else if (e.Button == MouseButtons.Left)
    {
        double input_schaal_number = double.Parse(input_schaal.Text);
        verschil_middel_x +=  e.X - (0.5 * grootte_bitmap);
        verschil_middel_y +=  e.Y - (0.5 * grootte_bitmap);
        zoomfactor = zoomfactor * 2;
        pictureBox1.MouseClick += teken_mandelbrot;
        pictureBox1.Invalidate();
    }
}

pictureBox1.MouseClick += mouseclicks;
draw_mandelbrot.Click += teken_mandelbrot;

Application.Run(scherm);

