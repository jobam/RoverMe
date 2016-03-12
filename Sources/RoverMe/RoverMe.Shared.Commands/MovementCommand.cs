using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RoverMe.Shared.Commands
{
    public class MovementCommand
    {
        #region Properties

        private decimal magnitude;

        /// <summary>
        /// Represent the percentage of force applied in the movement
        /// </summary>
        public decimal Magnitude
        {
            get { return magnitude; }
            set { magnitude = value; }
        }

        private decimal angularSpeed;

        /// <summary>
        /// Represent the percentage of rotational speed applied in the movement
        /// </summary>
        public decimal AngularSpeed
        {
            get { return angularSpeed; }
            set { angularSpeed = value; }
        }

        #endregion

        #region Constructor
        public MovementCommand()
        {
        }
        public MovementCommand(decimal magnitude, decimal angularSpeed)
        {
            this.Magnitude = magnitude;
            this.AngularSpeed = angularSpeed;
        }
        #endregion

    }
}
