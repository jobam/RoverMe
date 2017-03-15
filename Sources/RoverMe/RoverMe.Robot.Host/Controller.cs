using RoverMe.Robot.HostApp;
using RoverMe.Shared.Commands;
using RoverMe.Shared.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace RoverMe.Robot.Host
{
    public class Controller : IRoverMeCommand
    {
        #region Attributes and Properties

        public SocketServer ServerCommands { get; set; }
        public string ListeningPort { get; set; }

        public DataReader Reader { get; private set; }
        public DataWriter Writer { get; private set; }

        public MotorControl Motorcontroller { get; set; }

        public int DefaultActionTime = 1000; // milliseconds

        #endregion

        #region Cycle

        public Controller(string port = "4242")
        {
            this.ListeningPort = port;

            //Running motor controller
            Motorcontroller = new MotorControl();
            Motorcontroller.RunFoward(DefaultActionTime * 2);
        }

        public void StartCommandServer()
        {
            ServerCommands = new SocketServer(this.ListeningPort);
            ServerCommands.ClientConnected += onClientConnected;
            ServerCommands.IncommingCommand += onCommandRecieved;

            ServerCommands.Start();
        }
        
        #endregion

        #region Event Catch

        private void onCommandRecieved(string command)
        {
            int commandId = -1;

            Int32.TryParse(command, out commandId);

            switch (commandId)
            {
                case (int)RCommand.Right:
                    RightCommand(null);
                    break;
                case (int)RCommand.Left:
                    LeftCommand(null);
                    break;
                case (int)RCommand.Foward:
                    FowardCommand(null);
                    break;
                case (int)RCommand.Backward:
                    BackwardCommand(null);
                    break;
                case (int)RCommand.Stop:
                    StopCommand(null);
                    break;
                case (int)RCommand.BackwardRight:
                    BackwardRightCommand(null);
                    break;
                case (int)RCommand.BackwardLeft:
                    BackwardLeftCommand(null);
                    break;
                case (int)RCommand.FowardRight:
                    FowardRightCommand(null);
                    break;
                case (int)RCommand.FowardLeft:
                    FowardLeftCommand(null);
                    break;
                case (int)RCommand.CameraRight:
                    CameraRightCommand(null);
                    break;
                case (int)RCommand.CameraLeft:
                    RightCommand(null);
                    break;
                case (int)RCommand.SoundMessage:
                    SoundMessageCommand(null);
                    break;
            }

            Debug.WriteLine("Command Recieved: " + command, "Info");
        }

        private async void onClientConnected(DataReader reader, DataWriter writer)
        {
            this.Reader = reader;
            this.Writer = writer;

            Debug.WriteLine("Client connected", "Info");
            await ServerCommands.StartListeningCommands();
        }

        #endregion

        #region IRoverMeCommand

        public override bool BackwardCommand(string[] args)
        {
            throw new NotImplementedException();
        }

        public override bool BackwardLeftCommand(string[] args)
        {
            throw new NotImplementedException();
        }

        public override bool BackwardRightCommand(string[] args)
        {
            throw new NotImplementedException();
        }

        public override bool CameraLeftCommand(string[] args)
        {
            throw new NotImplementedException();
        }

        public override bool CameraRightCommand(string[] args)
        {
            throw new NotImplementedException();
        }

        public override bool FowardCommand(string[] args)
        {
            try
            {
                Motorcontroller.RunFoward(DefaultActionTime);
                return true;
            }catch (Exception e)
            {
                return false;
            }
        }

        public override bool FowardLeftCommand(string[] args)
        {
            throw new NotImplementedException();
        }

        public override bool FowardRightCommand(string[] args)
        {
            throw new NotImplementedException();
        }

        public override bool LeftCommand(string[] args)
        {
            try
            {
                Motorcontroller.RunMotorLeft(DefaultActionTime);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public override bool RightCommand(string[] args)
        {
            try
            {
                Motorcontroller.RunMotorRight(DefaultActionTime);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public override bool SoundMessageCommand(string[] args)
        {
            throw new NotImplementedException();
        }

        public override bool StopCommand(string[] args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
