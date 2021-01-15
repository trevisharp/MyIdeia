using System.Drawing;

namespace MyIdeia
{
    public abstract class Command
    {
        public abstract void Do(
                Rectangle sel,
                Bitmap bmp,
                Graphics g,
                params object[] parameters
            );
        
        public abstract bool CommandOk(string command, params object[] parameters);

        public Command Next { get; set; }
    }
}