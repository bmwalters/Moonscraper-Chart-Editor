using UnityEngine;

public enum ButtonState {
    Pressed,
}

public struct GamePadState {
    public struct ButtonStates {
        public ButtonState A;
        public ButtonState B;
        public ButtonState X;
        public ButtonState Y;
        public ButtonState LeftShoulder;
        public ButtonState RightShoulder;
        public ButtonState LeftStick;
        public ButtonState RightStick;
        public ButtonState Start;
        public ButtonState Back;
    }

    public struct DPadStates {
        public ButtonState Up;
        public ButtonState Down;
        public ButtonState Left;
        public ButtonState Right;
    }

    public struct ThumbStickStates {
        public struct ThumbStickAxisStates {
            public float X;
            public float Y;
        }

        public ThumbStickAxisStates Left;
        public ThumbStickAxisStates Right;
    }

    public bool IsConnected;
    public ButtonStates Buttons;
    public DPadStates DPad;
    public ThumbStickStates ThumbSticks;
}

public class GamePad {
    public static GamePadState GetState(int index) {
        // TODO: real implementation (for cross-platform) using InputManager?
        GamePadState state = new GamePadState();
        return state;
    }
}