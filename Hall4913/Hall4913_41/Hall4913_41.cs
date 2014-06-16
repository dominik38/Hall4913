using System;
using Microsoft.SPOT;

using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using GTI = Gadgeteer.Interfaces;

namespace Gadgeteer.Modules.SchreiberDominik
{
    /// <summary>
    /// A Hall4913 module for Microsoft .NET Gadgeteer
    /// </summary>
    public class Hall4913 : Module
    {
        #region Private Members

        private GTI.InterruptInput input;
        private HallSensorEventHandler onDetection;
        #endregion

        #region Public Members

        /// <summary>
        /// Gets a value that indicates whether the state of the hall sensor is Detected.
        /// </summary>
        public bool IsDetected
        {
            get
            {
                return !this.input.Read();
            }
        }

        /// <summary>
        /// Represents the state of the <see cref="Hall4913"/> object.
        /// </summary>
        public enum HallSensorState
        {
            /// <summary>
            /// The state of hall sensor is Detected.
            /// </summary>
            Detected = 0,
            /// <summary>
            /// The state of hall sensor is NotDetected.
            /// </summary>
            NotDetected = 1
        }

        #endregion

        #region Constructor
        /// <summary></summary>
        /// <param name="socketNumber">The mainboard socket that has the module plugged into it.</param>
        public Hall4913(int socketNumber)
        {
            Socket socket = Socket.GetSocket(socketNumber, true, this, null);

            socket.EnsureTypeIsSupported(new char[] { 'X', 'Y' }, this);

            // These calls will throw GT.Socket.InvalidSocketException if a pin conflict or error is encountered
            input = new GTI.InterruptInput(socket, Socket.Pin.Three, GTI.GlitchFilterMode.On, GTI.ResistorMode.PullDown, GTI.InterruptMode.RisingAndFallingEdge, this);
            input.Interrupt += (_input_Interrupt);
            GTI.DigitalOutput led = new GTI.DigitalOutput(socket, GT.Socket.Pin.Four, true, this);
        }
        #endregion

        #region Events and Handlers

        /// <summary>
        /// Represents the delegate that is used to handle the <see cref="Hall4913.DetectionLost"/>
        /// and <see cref="Hall4913.Detected"/> events.
        /// </summary>
        /// <param name="sender">The <see cref="Hall4913"/> object that raised the event.</param>
        /// <param name="state">The state of the detection</param>
        public delegate void HallSensorEventHandler(Hall4913 sender, HallSensorState state);

        /// <summary>
        /// Raised when the state of <see cref="Hall4913"/> is NotDetected.
        /// </summary>
        /// <remarks>
        /// Implement this event handler and the <see cref="Detected"/> event handler
        /// when you want to provide an action associated with hall sensor activity.
        /// The state of the hall sensor is passed to the <see cref="HallSensorEventHandler"/> delegate,
        /// so you can use the same event handler for both sensor states.
        /// </remarks>
        public event HallSensorEventHandler DetectionLost;

        /// <summary>
        /// Raised when the state of <see cref="Hall4913"/> is Detected.
        /// </summary>
        /// <remarks>
        /// Implement this event handler and the <see cref="DetectionLost"/> event handler
        /// when you want to provide an action associated with hall sensor activity.
        /// Since the state of the hall sensor is passed to the <see cref="HallSensorEventHandler"/> delegate,
        /// you can use the same event handler for both sensor states.
        /// </remarks>
        public event HallSensorEventHandler Detected;

        /// <summary>
        /// Raises the <see cref="DetectionLost"/> or <see cref="Detected"/> event.
        /// </summary>
        /// <param name="sender">The <see cref="Hall4913"/> that raised the event.</param>
        /// <param name="hallSensorState">The state of the hall sensor.</param>
        protected virtual void OnHallSensorEvent(Hall4913 sender, HallSensorState hallSensorState)
        {
            if (onDetection == null)
                onDetection = OnHallSensorEvent;

            if (Program.CheckAndInvoke((hallSensorState == HallSensorState.NotDetected ? DetectionLost : Detected), onDetection, sender, hallSensorState))
            {
                switch (hallSensorState)
                {
                    case HallSensorState.NotDetected:
                        DetectionLost(sender, hallSensorState);
                        break;
                    case HallSensorState.Detected:
                        Detected(sender, hallSensorState);
                        break;
                }

            }
        }

        private void _input_Interrupt(GTI.InterruptInput input, bool value)
        {
            HallSensorState hallSensorState = input.Read() ? HallSensorState.NotDetected : HallSensorState.Detected;
            OnHallSensorEvent(this, hallSensorState);
        }

        #endregion
    }
}
