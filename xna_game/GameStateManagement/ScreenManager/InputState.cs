#region File Description
//-----------------------------------------------------------------------------
// InputState.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// Helper for reading input from keyboard and gamepad. This class tracks both
    /// the current and previous state of both input devices, and implements query
    /// properties for high level input actions such as "move up through the menu"
    /// or "pause the game".
    /// </summary>
    public class InputState
    {
        #region Fields

        public const int MaxInputs = 4;

        public readonly KeyboardState[] CurrentKeyboardStates;
        public readonly GamePadState[] CurrentGamePadStates;
        public static MouseState CurrentMouseStates;

        public readonly KeyboardState[] LastKeyboardStates;
        public readonly GamePadState[] LastGamePadStates;
        public static MouseState LastMouseStates;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a new input state.
        /// </summary>
        public InputState()
        {
            CurrentKeyboardStates = new KeyboardState[MaxInputs];
            CurrentGamePadStates = new GamePadState[MaxInputs];
            CurrentMouseStates = new MouseState();

            LastKeyboardStates = new KeyboardState[MaxInputs];
            LastGamePadStates = new GamePadState[MaxInputs];
            LastMouseStates = new MouseState();
        }


        #endregion

        #region Properties


        /// <summary>
        /// Checks for a "menu up" input action, from any player,
        /// on either keyboard or gamepad.
        /// </summary>
        public bool MenuUp
        {
            get
            {
                return IsNewKeyPress(Keys.Up) ||
                       IsNewButtonPress(Buttons.DPadUp) ||
                       IsNewButtonPress(Buttons.LeftThumbstickUp);
            }
        }


        /// <summary>
        /// Checks for a "menu down" input action, from any player,
        /// on either keyboard or gamepad.
        /// </summary>
        public bool MenuDown
        {
            get
            {
                return IsNewKeyPress(Keys.Down) ||
                       IsNewButtonPress(Buttons.DPadDown) ||
                       IsNewButtonPress(Buttons.LeftThumbstickDown);
            }
        }


        /// <summary>
        /// Checks for a "menu select" input action, from any player,
        /// on either keyboard or gamepad.
        /// </summary>
        public bool MenuSelect
        {
            get
            {
                return IsNewKeyPress(Keys.Space) ||
                       IsNewKeyPress(Keys.Enter) ||
                       IsNewButtonPress(Buttons.A) ||
                       IsNewButtonPress(Buttons.Start);
            }
        }


        /// <summary>
        /// Checks for a "menu cancel" input action, from any player,
        /// on either keyboard or gamepad.
        /// </summary>
        public bool MenuCancel
        {
            get
            {
                return IsNewKeyPress(Keys.Escape) ||
                       IsNewButtonPress(Buttons.B) ||
                       IsNewButtonPress(Buttons.Back);
            }
        }


        /// <summary>
        /// Checks for a "pause the game" input action, from any player,
        /// on either keyboard or gamepad.
        /// </summary>
        public bool PauseGame
        {
            get
            {
                return IsNewKeyPress(Keys.Escape) ||
                       IsNewButtonPress(Buttons.Back) ||
                       IsNewButtonPress(Buttons.Start);
            }
        }

        //Check if 'W' key is pressed (to move forward)
        public bool ShipUp
        {
            get
            {
                return IsNewKeyPress(Keys.W) || IsKeyHeld(Keys.W) ||
                       IsNewKeyPress(Keys.Up) || IsKeyHeld(Keys.Up);
            }
        }

        //Check if 'S' key is pressed (to propell backward)
        public bool ShipDown
        {
            get
            {
                return IsNewKeyPress(Keys.S) || IsKeyHeld(Keys.S) ||
                       IsNewKeyPress(Keys.Down) || IsKeyHeld(Keys.Down);
            }
        }

        //Check if 'A' key is pressed (Turn the ship anti-clockwise)
        public bool ShipLeft
        {
            get
            {
                return IsNewKeyPress(Keys.A) || IsKeyHeld(Keys.A) ||
                       IsNewKeyPress(Keys.Left) || IsKeyHeld(Keys.Left);
            }
        }

        //Check if 'D' Key is pressed (Turn the ship clockwise)
        public bool ShipRight
        {
            get
            {
                return IsNewKeyPress(Keys.D) || IsKeyHeld(Keys.D)||
                       IsNewKeyPress(Keys.Right) || IsKeyHeld(Keys.Right);
            }
        }

        //Check if 'Space' is hit (For firinging)
        public bool ShipFire
        {
            get
            {
                return IsNewKeyPress(Keys.Space) || IsKeyHeld(Keys.Space) || isLeftMouseClick();
            }
        }

        public bool ShipCharging
        {
            get
            {
                return isLeftMouseHeld();
            }
        }

        //Check if 'Right mouse' is hit
        public bool ShipBarrelRoll
        {
            get
            {
                return isRightMouseClick();
            }
        }

        public bool ShipBomb
        {
            get
            {
                return isMiddleMouseClick();
            }
        }
        #endregion

        #region Methods


        /// <summary>
        /// Reads the latest state of the keyboard and gamepad.
        /// </summary>
        public void Update()
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                LastKeyboardStates[i] = CurrentKeyboardStates[i];
                LastGamePadStates[i] = CurrentGamePadStates[i];

                CurrentKeyboardStates[i] = Keyboard.GetState((PlayerIndex)i);
                CurrentGamePadStates[i] = GamePad.GetState((PlayerIndex)i); 
            }
            LastMouseStates = CurrentMouseStates;
            CurrentMouseStates = Mouse.GetState();
            //test();
        }

        //A dummy function to test the functionality of mouse inputs//
        /*private void test()
        {
            if (isRightMouseClick())
                System.Console.WriteLine("Barrel Roll!!!");
            if (isLeftMouseClick() || isLeftMouseHeld())
                System.Console.WriteLine("Ship Firing!!!!");
            return;
        }*/

        #region Input Checking
        /// <summary>
        /// Helper for checking if a key was newly pressed during this update,
        /// by any player.
        /// </summary>
        public bool IsNewKeyPress(Keys key)
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                if (IsNewKeyPress(key, (PlayerIndex)i))
                    return true;
            }

            return false;
        }


        /// <summary>
        /// Helper for checking if a key was newly pressed during this update,
        /// by the specified player.
        /// </summary>
        public bool IsNewKeyPress(Keys key, PlayerIndex playerIndex)
        {
            return (CurrentKeyboardStates[(int)playerIndex].IsKeyDown(key) &&
                    LastKeyboardStates[(int)playerIndex].IsKeyUp(key));
        }

        //Function to check whether a key has been held for all updates//
        public bool IsKeyHeld(Keys key)
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                if (IsKeyHeld(key, (PlayerIndex)i))
                    return true;
            }

            return false;
        }

        //Function to check whether a key has been held for a specific player//
        public bool IsKeyHeld(Keys key, PlayerIndex playerIndex)
        {
            return (CurrentKeyboardStates[(int)playerIndex].IsKeyDown(key) &&
                 LastKeyboardStates[(int)playerIndex].IsKeyDown(key));
        }

        /// <summary>
        /// Helper for checking if a button was newly pressed during this update,
        /// by any player.
        /// </summary>
        public bool IsNewButtonPress(Buttons button)
        {
            for (int i = 0; i < MaxInputs; i++)
            {
                if (IsNewButtonPress(button, (PlayerIndex)i))
                    return true;
            }

            return false;
        }


        /// <summary>
        /// Helper for checking if a button was newly pressed during this update,
        /// by the specified player.
        /// </summary>
        public bool IsNewButtonPress(Buttons button, PlayerIndex playerIndex)
        {
            return (CurrentGamePadStates[(int)playerIndex].IsButtonDown(button) &&
                    LastGamePadStates[(int)playerIndex].IsButtonUp(button));
        }

        //Mouse Press and Held checks//
        public bool isRightMouseClick()
        {
            return (CurrentMouseStates.RightButton == ButtonState.Pressed &&
                         LastMouseStates.RightButton == ButtonState.Released);
        }

        public bool isLeftMouseClick()
        {
            return (CurrentMouseStates.LeftButton == ButtonState.Pressed &&
                     LastMouseStates.LeftButton == ButtonState.Released);
        }

        public bool isMiddleMouseClick()
        {
            return (CurrentMouseStates.MiddleButton == ButtonState.Pressed &&
                     LastMouseStates.MiddleButton == ButtonState.Released);
        }

        public bool isLeftMouseHeld()
        {
            return (CurrentMouseStates.LeftButton == ButtonState.Pressed &&
                       LastMouseStates.LeftButton == ButtonState.Pressed);
        }

        #endregion
        /// <summary>
        /// Checks for a "menu select" input action from the specified player.
        /// </summary>
        public bool IsMenuSelect(PlayerIndex playerIndex)
        {
            return IsNewKeyPress(Keys.Space, playerIndex) ||
                   IsNewKeyPress(Keys.Enter, playerIndex) ||
                   IsNewButtonPress(Buttons.A, playerIndex) ||
                   IsNewButtonPress(Buttons.Start, playerIndex);
        }


        /// <summary>
        /// Checks for a "menu cancel" input action from the specified player.
        /// </summary>
        public bool IsMenuCancel(PlayerIndex playerIndex)
        {
            return IsNewKeyPress(Keys.Escape, playerIndex) ||
                   IsNewButtonPress(Buttons.B, playerIndex) ||
                   IsNewButtonPress(Buttons.Back, playerIndex);
        }


        #endregion
    }
}
