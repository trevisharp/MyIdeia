using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace MyIdeia
{
    public partial class FrmMain : Form
    {
        private void register()
        {
            chain.Add<OrganicFilter>();
        }

        int lines = 5;

        CommandChain chain = new CommandChain();
        Bitmap source;
        Graphics g;

        private bool run(string command)
        {
            if (testcall(command))
            {
                draw();
            }
            else
                return false;
            return true;
        }

        private bool testcall(string command)
        {
            Regex functioncall = new Regex("\\w+?\\((.*?(,)( )*)*(.*?( )*)?\\)");
            Match match = functioncall.Match(command);
            if (match.Success)
            {
                string commname = string.Concat(
                    match.Value.TakeWhile(c => c != '(')
                );
                object[] parameters = string.Concat(
                    match.Value.SkipWhile(c => c != '(').Skip(1)
                    .Reverse().Skip(1).Reverse()
                ).Split(',').Select(s => s.Trim())
                .Select<string, object>(s =>
                {
                    if (s.Length > 2 && s[0] == '\"' && s[1] == '\"')
                        return s.Substring(1, s.Length - 1);
                    if (int.TryParse(s, out int i))
                        return i;
                    return s;
                }).ToArray();
                return chain.Run(commname, selec ?? 
                    new Rectangle(0, 0, source.Width, source.Height),
                    source, g, parameters);
            }
            else
                return false;
        }

        Rectangle? selec = null;
        List<Rectangle> selections = new List<Rectangle>();
        Bitmap bmp;
        Graphics bmpg;
        FlowLayoutPanel pn;
        PictureBox pb;
        TextBox tb;
        Timer timer;

        public FrmMain()
        {
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.KeyPreview = true;
            this.Load += load;

            this.pn = new FlowLayoutPanel();
            this.pn.Dock = DockStyle.Fill;
            this.pn.BackColor = Color.AliceBlue;
            this.Controls.Add(pn);

            this.pb = new PictureBox();
            this.pn.Controls.Add(pb);

            this.tb = new TextBox();
            this.tb.KeyDown += onkeydown;
            this.tb.Multiline = true;
            this.pn.Controls.Add(tb);

            this.timer = new Timer();
            timer.Interval = 100;
            timer.Tick += tick;
        }

        private void load(object sender, EventArgs e)
        {
            register();

            pb.Width = pn.Width - 20;
            tb.Width = pn.Width - 20;
            tb.Height *= lines;
            pb.Height = pn.Height - 30 - tb.Height;

            bmp = new Bitmap(pb.Width, pb.Height);
            source = new Bitmap(pb.Width, pb.Height);
            bmpg = Graphics.FromImage(bmp);
            g = Graphics.FromImage(source);
            bmpg.Clear(Color.White);
            pb.Image = bmp;

            timer.Start();
        }

        private void tick(object sender, EventArgs e)
        {

        }

        private void draw()
        {
            bmpg.DrawImage(source, Point.Empty);

            pb.Image = bmp;
        }

        private void onkeydown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Application.Exit();
                    break;
                case Keys.Enter:
                    if (run(tb.Text))
                    {
                        tb.Text = "";
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                    }
                    break;
            }
        }
    }
}