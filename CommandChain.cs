using System.Drawing;

namespace MyIdeia
{
    public class CommandChain
    {
        private Command first = null;
        private Command last = null;

        public void Add<T>()
            where T : Command, new()
        {
            if (first == null)
                first = last = new T();
            else
            {
                last.Next = new T();
                last = last.Next;
            }
        }

        public bool Run(
            string command, 
            Rectangle sel,
            Bitmap bmp,
            Graphics g,
            params object[] parameters)
        {
            Command crr = first;
            while (crr != null)
            {
                if (crr.CommandOk(command, parameters))
                {
                    crr.Do(sel, bmp, g, parameters);
                    return true;
                }
                crr = crr.Next;
            }
            return false;
        }
    }
}