using Microsoft.Xna.Framework.Input;

 namespace MyGame.UI;

// Reads input on the title screen.
public class TitleScreen
{
    // Remembers which keyboard keys were pressed during the previous update.
    private KeyboardState _previousKeyboardState;

    // Returns true when the player presses a new key.
    public bool Update()
    {
        // Read which keys are currently pressed.
        KeyboardState currentKeyboardState = Keyboard.GetState();
        // Get a list of all currently pressed keys.
        Keys[] pressedKeys = currentKeyboardState.GetPressedKeys();

        // Start by assuming that no new key was pressed.
        bool keyWasPressed = false;

        // Check each key that is currently pressed.
        foreach (Keys key in pressedKeys)
        {
            // Check whether this key was not pressed during the previous update.
            if (_previousKeyboardState.IsKeyUp(key))
            {
                // Remember that a new key was pressed.
                keyWasPressed = true;
                // Stop checking because one new key is enough.
                break;
            }
        }

        // Save the current keyboard state for the next update.
        _previousKeyboardState = currentKeyboardState;
        // Tell the caller whether a new key was pressed.
        return keyWasPressed;
    }
}
