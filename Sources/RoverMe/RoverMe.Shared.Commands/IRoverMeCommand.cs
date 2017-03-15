using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoverMe.Shared.Commands
{
    public abstract class IRoverMeCommand
    {

        #region Commands Enum

        public enum RCommand
        {
            Stop = 0,
            Foward = 1,
            Backward = 2,
            Left = 3,
            Right = 4,
            FowardRight = 5,
            FowardLeft = 6,
            BackwardRight = 7,
            BackwardLeft = 8,
            CameraRight = 9,
            CameraLeft = 10,
            SoundMessage = 11
        }

        #endregion

        #region Global Methods

        public bool CommandDispatcher(RCommand command, string[] args)
        {
            switch (command)
            {
                case RCommand.Stop:
                    return StopCommand(args);
                case RCommand.Foward:
                    return FowardCommand(args);
                case RCommand.Backward:
                    return BackwardCommand(args);
                case RCommand.Left:
                    return LeftCommand(args);
                case RCommand.Right:
                    return RightCommand(args);
                case RCommand.FowardRight:
                    return FowardRightCommand(args);
                case RCommand.FowardLeft:
                    return FowardLeftCommand(args);
                case RCommand.BackwardRight:
                    return BackwardRightCommand(args);
                case RCommand.BackwardLeft:
                    return BackwardLeftCommand(args);
                case RCommand.CameraRight:
                    return CameraRightCommand(args);
                case RCommand.CameraLeft:
                    return CameraLeftCommand(args);
                case RCommand.SoundMessage:
                    return SoundMessageCommand(args);
                default:
                    return StopCommand(args);
            }
        }

        #endregion

        #region Moving Commands

        public abstract bool StopCommand(string[] args);
        public abstract bool FowardCommand(string[] args);
        public abstract bool BackwardCommand(string[] args);
        public abstract bool LeftCommand(string[] args);
        public abstract bool RightCommand(string[] args);
        public abstract bool FowardRightCommand(string[] args);
        public abstract bool FowardLeftCommand(string[] args);
        public abstract bool BackwardRightCommand(string[] args);
        public abstract bool BackwardLeftCommand(string[] args);

        #endregion

        #region Camera Commands

        public abstract bool CameraRightCommand(string[] args);
        public abstract bool CameraLeftCommand(string[] args);

        #endregion

        #region Sound Commands

        public abstract bool SoundMessageCommand(string[] args);

        #endregion

    }
}
