namespace Sorlov.PowerShell.Dto
{
    public class WindowInformation
    {
        private int x;
        public int X { get { return x; } internal set { x = value; } }

        private int y;
        public int Y { get { return y; } internal set { y = value; } }

        private int height;
        public int Height { get { return height; } internal set { height = value; } }

        private int width;
        public int Width { get { return width; } internal set { width = value; } }

        private WindowState windowState = WindowState.Normal;
        public WindowState State { get { return windowState; } internal set { windowState = value; } }
    }

    public enum WindowState
    {
        Minimized = 1,
        Maximized = 2,
        Normal = 0
    }

    public enum WindowFlash
    {
        Infinite = 1,
        Stop = 0,
        Quick = 2,
        Long = 3
    }

}
