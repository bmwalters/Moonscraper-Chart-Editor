﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MSE
{
    namespace Input
    {
        public enum DeviceType
        {
            Keyboard,
            //Gamepad,
        }

        public interface IInputDevice
        {
            bool Connected { get; }
            DeviceType Type { get; }
            IInputMap GetCurrentInput();
        }
    }
}