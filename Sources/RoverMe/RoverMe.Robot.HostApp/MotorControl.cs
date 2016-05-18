using System;
using Windows.Foundation;
using System.Diagnostics;
using Windows.Devices.Gpio;
using System.Threading;
using Windows.System.Threading;

namespace RoverMe.Robot.HostApp
{
    /// <summary>
    /// **** Motor Control Class ****
    /// Handles pulse timings to motors of robot
    /// </summary>
    class MotorControl
    {
        private static IAsyncAction workItemThread;
        private static ulong ticksPerMs;
        private static Stopwatch stopwatch;

        const int LEFT_PWM_PIN = 4;
        const int RIGHT_PWM_PIN = 6;
        const int SENSOR_PIN = 13;
        const int ACT_LED_PIN = 47; // rpi2-its-pin47, rpi-its-pin16
        private static GpioController gpioController = null;
        private static GpioPin leftPwmPin = null;
        private static GpioPin rightPwmPin = null;
        private static GpioPin sensorPin = null;
        private static GpioPin statusLedPin = null;

        private enum MotorIds { Left, Right };
        public enum PulseMs { stop = -1, ms1 = 0, ms2 = 2 } // values selected for thread-safety
        public static PulseMs waitTimeLeft = PulseMs.stop;
        public static PulseMs waitTimeRight = PulseMs.stop;

        public static int speedValue = 10000;

        private const int MaxDebs = 10;
        private static int[] debounceValues;
        private static int[] debounceCounts;
        private static int[] debounceLast;
        private static bool GpioInitialized = false;

        static long msLastCheckTime;
        static bool isBlockSensed = false;
        static bool lastIsBlockSensed = false;

        public static void MotorsInit()
        {
            DebounceInit();
            GpioInit();
            stopwatch = new Stopwatch();
            ticksPerMs = (ulong)(Stopwatch.Frequency) / 1000;

            workItemThread = Windows.System.Threading.ThreadPool.RunAsync(
                 (source) =>
                 {
                     // setup, ensure pins initialized
                     ManualResetEvent mre = new ManualResetEvent(false);
                     mre.WaitOne(1000);

                     // settle period - to dismiss transient startup conditions, as things are starting up
                     for (int x = 0; x < 10; ++x)
                     {
                         mre.WaitOne(100);
                         isBlockSensed = DebounceValue((int)sensorPin.Read(), 0, 2) == 0;
                         lastIsBlockSensed = isBlockSensed;
                     }
                 }, WorkItemPriority.High);

        }

        /// <summary>
        /// Generate a single motor pulse wave for given servo motor (High for 1 to 2ms, duration for 20ms).
        ///   motor value denotes which moter to send pulse to.
        /// </summary>
        /// <param name="motor"></param>
        private static void PulseMotor(MotorIds motor)
        {

            // Begin pulse (setup for simple Single Speed)
            ulong pulseTicks = ticksPerMs;
            if (motor == MotorIds.Left)
            {
                if (waitTimeLeft == PulseMs.stop) return;
                if (waitTimeLeft == PulseMs.ms2) pulseTicks = ticksPerMs * 2;
                leftPwmPin.Write(GpioPinValue.High);
            }
            else
            {
                if (waitTimeRight == PulseMs.stop) return;
                if (waitTimeRight == PulseMs.ms2) pulseTicks = ticksPerMs * 2;
                rightPwmPin.Write(GpioPinValue.High);
            }

            // Timing
            ulong delta;
            ulong starttick = (ulong)(stopwatch.ElapsedTicks);
            while (true)
            {
                delta = (ulong)(stopwatch.ElapsedTicks) - starttick;
                if (delta > (50 * ticksPerMs)) break;
                if (delta > pulseTicks) break;
            }

            // End of pulse
            if (motor == MotorIds.Left) leftPwmPin.Write(GpioPinValue.Low);
            else rightPwmPin.Write(GpioPinValue.Low);
        }

        public static void MoveMotorsForTime(uint ms)
        {
            if (!GpioInitialized) return;

            ManualResetEvent mre = new ManualResetEvent(false);
            ulong stick = (ulong)stopwatch.ElapsedTicks;
            while (true)
            {
                ulong delta = (ulong)(stopwatch.ElapsedTicks) - stick;
                if (delta > (ms * ticksPerMs)) break;  // stop motion after given time

                PulseMotor(MotorIds.Left);
                mre.WaitOne(2);
                PulseMotor(MotorIds.Right);
                mre.WaitOne(2);
            }
        }

        private static void GpioInit()
        {
            try
            {
                gpioController = GpioController.GetDefault();
                if (null != gpioController)
                {
                    leftPwmPin = gpioController.OpenPin(LEFT_PWM_PIN);
                    leftPwmPin.SetDriveMode(GpioPinDriveMode.Output);

                    rightPwmPin = gpioController.OpenPin(RIGHT_PWM_PIN);
                    rightPwmPin.SetDriveMode(GpioPinDriveMode.Output);

                    statusLedPin = gpioController.OpenPin(ACT_LED_PIN);
                    statusLedPin.SetDriveMode(GpioPinDriveMode.Output);
                    statusLedPin.Write(GpioPinValue.Low);

                    sensorPin = gpioController.OpenPin(SENSOR_PIN);
                    sensorPin.SetDriveMode(GpioPinDriveMode.Input);
                    sensorPin.ValueChanged += (s, e) =>
                    {
                        GpioPinValue pinValue = sensorPin.Read();
                        statusLedPin.Write(pinValue);
                        isBlockSensed = (e.Edge == GpioPinEdge.RisingEdge);
                    };

                    GpioInitialized = true;
                }
            } 
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR: GpioInit failed - " + ex.Message);
            }
        }

        private static void DebounceInit()
        {
            debounceValues = new int[MaxDebs];
            debounceCounts = new int[MaxDebs];
            debounceLast = new int[MaxDebs];
        }

        /// <summary>
        /// DebounceValue - returns a smoothened, un-rippled, value from a run of given, possibly transient, pin values.  
        ///   curValue = raw pin input value
        ///   ix = an index for a unique pin, or purpose, to locate in array
        ///   run = the maximum number of values, which signify the signal value is un-rippled or solid
        /// </summary>
        /// <param name="curValue"></param>
        /// <param name="ix"></param>
        /// <param name="run"></param>
        /// <returns></returns>
        private static int DebounceValue(int curValue, int ix, int run)
        {
            if (ix < 0 || ix > debounceValues.Length) return 0;
            if (curValue == debounceValues[ix])
            {
                debounceCounts[ix] = 0;
                return curValue;
            }

            if (curValue == debounceLast[ix]) debounceCounts[ix] += 1;
            else debounceCounts[ix] = 0;

            if (debounceCounts[ix] >= run)
            {
                debounceCounts[ix] = run;
                debounceValues[ix] = curValue;
            }
            debounceLast[ix] = curValue;
            return debounceValues[ix];
        }

    }
}
