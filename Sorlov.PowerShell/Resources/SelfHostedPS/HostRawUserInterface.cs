using System;
using System.Management.Automation.Host;

namespace Sorlov.SelfHostedPS.Application
{

    public class HostRawUserInterface : PSHostRawUserInterface
    {

        
        public override KeyInfo ReadKey(ReadKeyOptions options)
        {
            return ConsoleHandler.ReadKey(options);
        }

        public override void FlushInputBuffer()
        {
            ConsoleHandler.FlushInputBuffer();
        }

        public override void SetBufferContents(Coordinates origin, BufferCell[,] contents)
        {
            ConsoleHandler.SetBufferContents(origin, contents);
        }

        public override void SetBufferContents(Rectangle rectangle, BufferCell fill)
        {
            ConsoleHandler.SetBufferContents(rectangle, fill);
        }

        public override BufferCell[,] GetBufferContents(Rectangle rectangle)
        {
            return ConsoleHandler.GetBufferContents(rectangle);
        }

        public override void ScrollBufferContents(Rectangle source, Coordinates destination, Rectangle clip, BufferCell fill)
        {
            ConsoleHandler.ScrollBufferContents(source, destination, clip, fill);
        }

        public override ConsoleColor ForegroundColor
        {
            get { return ConsoleHandler.ForegroundColor; }
            set { ConsoleHandler.ForegroundColor = value; }
        }

        public override ConsoleColor BackgroundColor
        {
            get { return ConsoleHandler.BackgroundColor; }
            set { ConsoleHandler.BackgroundColor = value; }
        }

        public override Coordinates CursorPosition
        {
            get { return ConsoleHandler.CursorPosition; }
            set { ConsoleHandler.CursorPosition = value; }
        }

        public override Coordinates WindowPosition
        {
            get { return ConsoleHandler.WindowPosition; }
            set { ConsoleHandler.WindowPosition = value; }
        }

        public override int CursorSize
        {
            get { return ConsoleHandler.CursorSize; }
            set { ConsoleHandler.CursorSize = value; }
        }

        public override Size BufferSize
        {
            get { return ConsoleHandler.BufferSize; }
            set { ConsoleHandler.BufferSize = value; }
        }

        public override Size WindowSize
        {
            get { return ConsoleHandler.WindowSize; }
            set { ConsoleHandler.WindowSize = value; }
        }

        public override Size MaxWindowSize
        {
            get { return ConsoleHandler.MaxWindowSize; }
        }

        public override Size MaxPhysicalWindowSize
        {
            get { return ConsoleHandler.MaxPhysicalWindowSize; }
        }

        public override bool KeyAvailable
        {
            get { return ConsoleHandler.KeyAvailable; }
        }

        public override string WindowTitle
        {
            get { return ConsoleHandler.WindowTitle; }
            set { ConsoleHandler.WindowTitle = value; }
        }
    }
}
