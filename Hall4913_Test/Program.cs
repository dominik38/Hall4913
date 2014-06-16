using Gadgeteer.Modules.SchreiberDominik;
using Microsoft.SPOT;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using System;

namespace Hall4913_Test
{
    public partial class Program
    {
        void ProgramStarted()
        {
            Debug.Print("Program Started");
            Hall4913 hall4913 = new Hall4913(3);
            hall4913.LEDMode = Hall4913.LEDModes.OnWhileDetected;
            hall4913.MagnetDetected += HallOnDetected;
            hall4913.MagnetDetectionLost += HallOnDetectionLost;
        }

        private void HallOnDetectionLost(Hall4913 sender, Hall4913.HallSensorState state)
        {
            Debug.Print("Detection Lost");
        }

        private void HallOnDetected(Hall4913 sender, Hall4913.HallSensorState state)
        {
            Debug.Print("Magnet Detected");
        }
    }
}
