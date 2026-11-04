 using System;
 using Microsoft.Xna.Framework;
 using Microsoft.Xna.Framework.Graphics;
 using Microsoft.Xna.Framework.Input;

 namespace MyGame.UI;

// Lists the menu choices the player can select.
public enum MenuOption
{
    // Start playing.
    StartGame,
    // Open the settings.
    Settings,
    // Close the game.
    Quit
}

// Handles drawing and input for the game's menu.
public class MenuScreen
{
    // Stores the text shown for each menu choice.
    private static readonly string[] OptionLabels =
    [
        "START GAME",
        "SETTINGS",
        "QUIT"
    ];

    // Remembers which keyboard keys were pressed in the previous update.
    private KeyboardState _previousKeyboardState;
    // Remembers the mouse buttons and position from the previous update.
    private MouseState _previousMouseState;

    // Stores which menu choice is selected with the keyboard.
    private int _selectedIndex;
    // Stores which menu choice the mouse is over; -1 means none.
    private int _hoveredIndex = -1;
    // Remembers whether the keyboard or mouse is controlling the menu.
    private bool _keyboardNavigationActive;
    // Stores which setting is selected with the keyboard.
    private int _selectedSettingIndex;
    // Stores which setting the mouse is over; -1 means none.
    private int _hoveredSettingIndex = -1;
    // Starts with keyboard navigation enabled in Settings.
    private bool _settingsKeyboardNavigationActive = true;
    // Remembers whether the Settings screen is open.
    private bool _settingsOpen;
    // Sound effects are on when the menu is first created.
    private bool _audioEnabled = true;
    // Music is on when the menu is first created.
    private bool _songEnabled = true;

    // Lets other code check whether Settings is open.
    public bool IsSettingsOpen => _settingsOpen;
    // Lets other code check whether sound effects are on.
    public bool IsAudioEnabled => _audioEnabled;

    // Creates the menu and records the current input.
    public MenuScreen()
    {
        // Save the current keyboard state.
        _previousKeyboardState = Keyboard.GetState();
        // Save the current mouse state.
        _previousMouseState = Mouse.GetState();
    }

    // Opens Settings and selects the first setting.
    public void OpenSettings()
    {
        // Show the Settings screen.
        _settingsOpen = true;
        // Select the first setting.
        _selectedSettingIndex = 0;
        // Save the current keyboard state to avoid reading old key presses.
        _previousKeyboardState = Keyboard.GetState();
        // Save the current mouse state to avoid reading an old click.
        _previousMouseState = Mouse.GetState();
    }

    // Updates menu input and returns the option the player selected, if any.
    public MenuOption? Update(
        // Font used to measure menu items.
        SpriteFont font,
        // Width of the game window.
        int screenWidth,
        // Height of the game window.
        int screenHeight,
        // Method used to play the menu sound.
        Action playMenuSound)
    {
        // Read the current keyboard buttons.
        KeyboardState keyboardState = Keyboard.GetState();
        // Read the current mouse buttons and position.
        MouseState mouseState = Mouse.GetState();

        // Update Settings instead of the main menu when Settings is open.
        if (_settingsOpen)
        {
            // Handle keyboard and mouse input for Settings.
            UpdateSettings(font, screenWidth, screenHeight, keyboardState, mouseState, playMenuSound);
            // Save keyboard input for the next update.
            _previousKeyboardState = keyboardState;
            // Save mouse input for the next update.
            _previousMouseState = mouseState;
            // No main-menu option was selected.
            return null;
        }

        // Check whether Up or W was just pressed.
        bool movedUp = WasPressed(keyboardState, Keys.Up) || WasPressed(keyboardState, Keys.W);
        // Check whether Down or S was just pressed.
        bool movedDown = WasPressed(keyboardState, Keys.Down) || WasPressed(keyboardState, Keys.S);

        // Move the selection up when an up key is pressed.
        if (movedUp)
        {
            // Move to the previous item, wrapping around to the bottom if needed.
            _selectedIndex = (_selectedIndex + OptionLabels.Length - 1) % OptionLabels.Length;
        }
        // Otherwise, move down when a down key is pressed.
        else if (movedDown)
        {
            // Move to the next item, wrapping around to the top if needed.
            _selectedIndex = (_selectedIndex + 1) % OptionLabels.Length;
        }

        // Turn on keyboard selection after the player uses an arrow or WASD key.
        if (movedUp || movedDown)
        {
            _keyboardNavigationActive = true;
        }

        // Switch to mouse selection when the mouse moves.
        if (mouseState.Position != _previousMouseState.Position)
        {
            _keyboardNavigationActive = false;
        }

        // Start by assuming the mouse is not over a menu item.
        _hoveredIndex = -1;

        // Check each menu item to see whether the mouse is over it.
        for (int i = 0; i < OptionLabels.Length; i++)
        {
            // Check whether this item's rectangle contains the mouse position.
            if (GetOptionBounds(font, screenWidth, screenHeight, i).Contains(mouseState.Position))
            {
                // Remember which item the mouse is over.
                _hoveredIndex = i;
                // Select the item under the mouse.
                _selectedIndex = i;
                // Stop checking because the item was found.
                break;
            }
        }

        // Start with no selected menu option.
        MenuOption? selectedOption = null;

        // Select the current item when Enter is just pressed.
        if (WasPressed(keyboardState, Keys.Enter))
        {
            // Play a sound for the selection.
            playMenuSound();
            // Get the selected option or open Settings.
            selectedOption = SelectMenuOption();
        }
        // Otherwise, check whether the left mouse button was just clicked.
        else if (mouseState.LeftButton == ButtonState.Pressed &&
                 _previousMouseState.LeftButton == ButtonState.Released)
        {
            // Check each menu item to find which one was clicked.
            for (int i = 0; i < OptionLabels.Length; i++)
            {
                // Check whether the mouse click is inside this menu item.
                if (GetOptionBounds(font, screenWidth, screenHeight, i).Contains(mouseState.Position))
                {
                    // Select the clicked item.
                    _selectedIndex = i;
                    // Play a sound for the selection.
                    playMenuSound();
                    // Get the selected option or open Settings.
                    selectedOption = SelectMenuOption();
                    // Stop checking after finding the clicked item.
                    break;
                }
            }
        }

        // Save the keyboard state for the next update.
        _previousKeyboardState = keyboardState;
        // Save the mouse state for the next update.
        _previousMouseState = mouseState;

        // Return the chosen option, or null if nothing was chosen.
        return selectedOption;
    }

    // Draws the background and either the main menu or Settings.
    public void Draw(
        // Tool used to draw images and text.
        SpriteBatch spriteBatch,
        // Font used for the menu text.
        SpriteFont font,
        // Background image behind the menu.
        Texture2D background,
        // Small image used to draw solid shapes.
        Texture2D pixel,
        // Width of the game window.
        int screenWidth,
        // Height of the game window.
        int screenHeight)
    {
        // Draw the background image to cover the game window.
        spriteBatch.Draw(background, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
        // Draw a see-through dark layer over the background.
        spriteBatch.Draw(pixel, new Rectangle(0, 0, screenWidth, screenHeight), new Color(0, 0, 0, 105));

        // Draw Settings instead of the main menu when it is open.
        if (_settingsOpen)
        {
            // Draw the Settings screen.
            DrawSettings(spriteBatch, font, pixel, screenWidth, screenHeight);
            // Stop here so the main menu is not drawn over Settings.
            return;
        }

        // Store the menu title.
        string title = "SURVIVAL OF THE FITTEST";
        // Measure the title to help position and size it.
        Vector2 titleSize = font.MeasureString(title);
        // Shrink the title if necessary so it fits the screen.
        float titleScale = Math.Min(0.9f, (screenWidth - 32f) / titleSize.X);
        // Place the title at the center, near the top of the screen.
        Vector2 titlePosition = new(screenWidth / 2f, screenHeight * 0.23f);

        // Draw a black copy of the title slightly offset as a shadow.
        spriteBatch.DrawString(font, title, titlePosition + new Vector2(3, 4), Color.Black, 0, titleSize / 2f, titleScale, SpriteEffects.None, 0);

        // Draw the colored title over its shadow.
        spriteBatch.DrawString(font, title, titlePosition, new Color(220, 55, 35), 0, titleSize / 2f, titleScale, SpriteEffects.None, 0);

        // Draw each menu choice.
        for (int i = 0; i < OptionLabels.Length; i++)
        {
            // Find the position and size of this menu button.
            Rectangle button = GetOptionBounds(font, screenWidth, screenHeight, i);

            // Highlight the button if the mouse is over it or keyboard selection is on it.
            bool highlighted = i == _hoveredIndex ||
                _keyboardNavigationActive && i == _selectedIndex;
            // Choose a background color based on whether the button is highlighted.
            Color fillColor = highlighted ? new Color(75, 57, 20, 235) : new Color(15, 20, 18, 215);
            // Choose a border color based on whether the button is highlighted.
            Color borderColor = highlighted ? new Color(255, 205, 45) : new Color(205, 215, 200);

            // Draw the button's background.
            spriteBatch.Draw(pixel, button, fillColor);
            // Draw the button's border, making it thicker when highlighted.
            DrawOutline(spriteBatch, pixel, button, borderColor, highlighted ? 3 : 2);

            // Measure this button's label at its display size.
            Vector2 labelSize = font.MeasureString(OptionLabels[i]) * 0.62f;
            // Center the label inside the button.
            Vector2 labelPosition = new(
                button.Center.X - labelSize.X / 2,
                button.Center.Y - labelSize.Y / 2);

            // Use yellow text for a highlighted button and white otherwise.
            Color textColor = highlighted ? Color.Yellow : Color.White;

            // Draw the button label.
            spriteBatch.DrawString(
                // Use the menu font.
                font,
                // Use this menu item's text.
                OptionLabels[i],
                // Place the text in the center of the button.
                labelPosition,
                // Use the selected text color.
                textColor,
                // Do not rotate the text.
                0,
                // Start drawing from the text's top-left corner.
                Vector2.Zero,
                // Draw the text at 62 percent of its normal size.
                0.62f,
                // Do not flip the text.
                SpriteEffects.None,
                // Draw at the default depth.
                0);
        }
    }

    // Returns the selected option or opens Settings.
    private MenuOption? SelectMenuOption()
    {
        // Convert the selected item's number into a menu option.
        MenuOption option = (MenuOption)_selectedIndex;

        // Open Settings instead of returning it to the caller.
        if (option == MenuOption.Settings)
        {
            // Show the Settings screen.
            _settingsOpen = true;
            // Select the first setting.
            _selectedSettingIndex = 0;

            // No main-menu option needs to be returned.
            return null;
        }

        // Return Start Game or Quit to the caller.
        return option;
    }

    // Handles keyboard and mouse input while Settings is open.
    private void UpdateSettings(
        // Font used to measure menu text.
        SpriteFont font,
        // Width of the game window.
        int screenWidth,
        // Height of the game window.
        int screenHeight,
        // Current keyboard buttons.
        KeyboardState keyboardState,
        // Current mouse buttons and position.
        MouseState mouseState,
        // Method used to play a sound when a choice is made.
        Action playMenuSound)
    {
        // Close Settings when Escape or Back is just pressed.
        if (WasPressed(keyboardState, Keys.Escape) ||
            WasPressed(keyboardState, Keys.Back))
        {
            // Hide the Settings screen.
            _settingsOpen = false;
            // Stop handling Settings input for this update.
            return;
        }

        // Check whether Up or W was just pressed.
        bool movedUp = WasPressed(keyboardState, Keys.Up) || WasPressed(keyboardState, Keys.W);
        // Check whether Down or S was just pressed.
        bool movedDown = WasPressed(keyboardState, Keys.Down) || WasPressed(keyboardState, Keys.S);

        // Change the selected setting when an up key is pressed.
        if (movedUp)
        {
            // Switch between setting 0 and setting 1.
            _selectedSettingIndex = (_selectedSettingIndex + 1) % 2;
        }
        // Change the selected setting when a down key is pressed.
        else if (movedDown)
        {
            // Switch between setting 0 and setting 1.
            _selectedSettingIndex = (_selectedSettingIndex + 1) % 2;
        }

        // Use keyboard highlighting after an arrow or W/S key is pressed.
        if (movedUp || movedDown)
        {
            _settingsKeyboardNavigationActive = true;
        }

        // Start by assuming the mouse is not over a setting.
        _hoveredSettingIndex = -1;

        // Use mouse highlighting when the mouse moves.
        if (mouseState.Position != _previousMouseState.Position)
        {
            _settingsKeyboardNavigationActive = false;
        }

        // Toggle the selected setting when Left, Right, or Enter is just pressed.
        if (WasPressed(keyboardState, Keys.Left) ||
            WasPressed(keyboardState, Keys.Right) ||
            WasPressed(keyboardState, Keys.Enter))
        {
            // Turn the selected setting on or off.
            ToggleSetting(_selectedSettingIndex);
            // Play a sound for the setting change.
            playMenuSound();
        }

        // Check both settings to see whether the mouse is over one.
        for (int i = 0; i < 2; i++)
        {
            // Check whether the mouse is inside this setting's button.
            if (GetSettingBounds(screenWidth, screenHeight, i).Contains(mouseState.Position))
            {
                // Remember which setting the mouse is over.
                _hoveredSettingIndex = i;
                // Select the setting under the mouse.
                _selectedSettingIndex = i;

                // Check whether the left mouse button was just clicked.
                if (mouseState.LeftButton == ButtonState.Pressed &&
                    _previousMouseState.LeftButton == ButtonState.Released)
                {
                    // Turn this setting on or off.
                    ToggleSetting(i);
                    // Play a sound for the setting change.
                    playMenuSound();
                }
            }
        }

        // Check whether the mouse is over the Back button.
        if (GetBackBounds(screenWidth, screenHeight).Contains(mouseState.Position))
        {
            // Check whether the left mouse button was just clicked.
            if (mouseState.LeftButton == ButtonState.Pressed &&
                _previousMouseState.LeftButton == ButtonState.Released)
            {
                // Close Settings.
                _settingsOpen = false;
                // Play a sound for the click.
                playMenuSound();
            }
        }
    }

    // Draws the Settings panel, choices, and Back button.
    private void DrawSettings(
        // Tool used to draw images and text.
        SpriteBatch spriteBatch,
        // Font used for Settings text.
        SpriteFont font,
        // Small image used to draw solid shapes.
        Texture2D pixel,
        // Width of the game window.
        int screenWidth,
        // Height of the game window.
        int screenHeight)
    {
        // Choose a size multiplier based on the window, with a minimum and maximum.
        float scale = Math.Clamp(Math.Min(screenWidth / 1280f, screenHeight / 720f), 0.55f, 1.2f);

        // Find the position and size of the Settings panel.
        Rectangle panel = GetSettingsPanelBounds(screenWidth, screenHeight, scale);
        // Draw the panel with a dark background.
        spriteBatch.Draw(pixel, panel, new Color(10, 16, 15, 240));

        // Draw a light border around the panel.
        DrawOutline(spriteBatch, pixel, panel, new Color(190, 205, 185), 2);

        // Store the Settings title.
        string title = "SETTINGS";
        // Measure the title at the size it will be drawn.
        Vector2 titleSize = font.MeasureString(title) * (0.82f * scale);
        // Center the title near the top of the panel.
        Vector2 titlePosition = new(
            panel.Center.X - titleSize.X / 2,
            panel.Y + (int)(24 * scale));

        // Draw a small black shadow behind the title.
        spriteBatch.DrawString(font, title, titlePosition + new Vector2(2, 3), Color.Black, 0, Vector2.Zero, 0.82f * scale, SpriteEffects.None, 0);
        // Draw the red title over its shadow.
        spriteBatch.DrawString(font, title, titlePosition, new Color(255, 95, 65), 0, Vector2.Zero, 0.82f * scale, SpriteEffects.None, 0);

        // Store the names of the two settings.
        string[] labels = ["SOUND EFFECTS", "MUSIC"];
        // Store short explanations for the settings.
        string[] descriptions = ["Game sounds and feedback", "Background music"];
        // Store whether each setting is currently on.
        bool[] values = [_audioEnabled, _songEnabled];

        // Draw each setting row.
        for (int i = 0; i < labels.Length; i++)
        {
            // Find the position and size of this setting row.
            Rectangle bounds = GetSettingBounds(screenWidth, screenHeight, i);

            // Highlight the row under the mouse or selected by the keyboard.
            bool highlighted = i == _hoveredSettingIndex ||
                _settingsKeyboardNavigationActive && i == _selectedSettingIndex;

            // Draw the row with a different color when it is highlighted.
            spriteBatch.Draw(
                // Use the small image to draw the row background.
                pixel,
                // Use this row's position and size.
                bounds,
                // Choose a brighter color for a highlighted row.
                highlighted ? new Color(68, 54, 22, 245) : new Color(24, 33, 29, 235));

            // Draw the row border, making it thicker when highlighted.
            DrawOutline(spriteBatch, pixel, bounds, highlighted ? Color.Yellow : new Color(125, 145, 130), highlighted ? 3 : 2);

            // Choose the setting label's text size.
            float labelScale = 0.48f * scale;
            // Place the label near the left side of the row.
            Vector2 labelPosition = new(
                bounds.X + (int)(18 * scale),
                bounds.Y + (int)(10 * scale));

            // Draw the setting name.
            spriteBatch.DrawString(font, labels[i], labelPosition, highlighted ? Color.Yellow : Color.White, 0, Vector2.Zero, labelScale, SpriteEffects.None, 0);
            // Draw the setting's explanation below its name.
            spriteBatch.DrawString(font, descriptions[i], labelPosition + new Vector2(0, 29 * scale), new Color(190, 198, 188), 0, Vector2.Zero, 0.28f * scale, SpriteEffects.None, 0);

            // Find the position and size of this setting's on/off button.
            Rectangle toggle = GetSettingToggleBounds(bounds, scale);

            // Use green for On and red for Off.
            Color toggleColor = values[i] ? new Color(40, 135, 70) : new Color(105, 48, 42);
            // Draw the on/off button.
            spriteBatch.Draw(pixel, toggle, toggleColor);

            // Draw a border whose color matches the current On or Off state.
            DrawOutline(spriteBatch, pixel, toggle, values[i] ? new Color(145, 235, 160) : new Color(235, 150, 130), 2);

            // Show ON or OFF based on the current setting value.
            string valueText = values[i] ? "ON" : "OFF";

            // Measure the ON or OFF text at its display size.
            Vector2 valueSize = font.MeasureString(valueText) * (0.42f * scale);
            // Center the ON or OFF text inside its button.
            Vector2 valuePosition = new(
                toggle.Center.X - valueSize.X / 2,
                toggle.Center.Y - valueSize.Y / 2);

            // Draw the ON or OFF text.
            spriteBatch.DrawString(
                // Use the Settings font.
                font,
                // Draw the current value.
                valueText,
                // Place it in the center of the button.
                valuePosition,
                // Use white text.
                Color.White,
                // Do not rotate the text.
                0,
                // Start drawing from the text's top-left corner.
                Vector2.Zero,
                // Use the selected text size.
                0.42f * scale,
                // Do not flip the text.
                SpriteEffects.None,
                // Draw at the default depth.
                0);
        }

        // Find the position and size of the Back button.
        Rectangle backBounds = GetBackBounds(screenWidth, screenHeight);

        // Check whether the mouse is over the Back button.
        bool backHovered = backBounds.Contains(Mouse.GetState().Position);

        // Draw the Back button, using a brighter color when hovered.
        spriteBatch.Draw(pixel, backBounds, backHovered ? new Color(78, 57, 20, 245) : new Color(30, 38, 33, 240));

        // Draw the Back button's border, making it thicker when hovered.
        DrawOutline(spriteBatch, pixel, backBounds, backHovered ? Color.Yellow : new Color(190, 205, 185), backHovered ? 3 : 2);

        // Store the text shown on the Back button.
        string backLabel = "BACK TO MENU";
        // Measure the Back button text.
        Vector2 backLabelSize = font.MeasureString(backLabel) * (0.42f * scale);
        // Center the text inside the button.
        Vector2 backPosition = new(
            backBounds.Center.X - backLabelSize.X / 2,
            backBounds.Center.Y - backLabelSize.Y / 2);

        // Draw the Back button text, highlighting it when the mouse is over it.
        spriteBatch.DrawString(font, backLabel, backPosition, backHovered ? Color.Yellow : Color.White, 0, Vector2.Zero, 0.42f * scale, SpriteEffects.None, 0);
    }

    // Turns the selected setting on or off.
    private void ToggleSetting(int index)
    {
        // Setting number 0 controls sound effects.
        if (index == 0)
        {
            // Change sound effects from on to off, or off to on.
            _audioEnabled = !_audioEnabled;
        }
        // The other setting controls music.
        else
        {
            // Change music from on to off, or off to on.
            _songEnabled = !_songEnabled;
        }
    }

    // Checks whether a key was pressed now but was not pressed last update.
    private bool WasPressed(KeyboardState currentKeyboardState, Keys key)
    {
        // This is true only on the first update when the key is down.
        return currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key);
    }

    // Calculates where one main-menu button should appear.
    private static Rectangle GetOptionBounds(
        // Font used to get the size of the menu labels.
        SpriteFont font,
        // Width of the game window.
        int screenWidth,
        // Height of the game window.
        int screenHeight,
        // Number of the menu item to position.
        int index)
    {
        // Choose a size multiplier that fits the game window.
        float scale = Math.Clamp(Math.Min(screenWidth / 1280f, screenHeight / 720f), 0.6f, 1.2f);

        // Set the button width, making sure it fits inside the window.
        int width = Math.Min((int)(420 * scale), screenWidth - 32);
        // Set the button height.
        int height = (int)(66 * scale);
        // Set the space between buttons.
        int gap = (int)(20 * scale);

        // Calculate the height of all buttons and the gaps between them.
        int totalHeight = OptionLabels.Length * height + (OptionLabels.Length - 1) * gap;

        // Find the top position so the group of buttons is centered.
        int top = (screenHeight - totalHeight) / 2 + (int)(45 * scale);

        // Return this button's position and size.
        return new Rectangle(
            // Put the button in the center from left to right.
            (screenWidth - width) / 2,
            // Move down for each button after the first.
            top + index * (height + gap),
            // Set the button width.
            width,
            // Set the button height.
            height);
    }

    // Draws a border around a rectangle.
    private static void DrawOutline(SpriteBatch spriteBatch, Texture2D pixel, Rectangle bounds, Color color, int thickness)
    {
        // Draw the top edge of the border.
        spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Y, bounds.Width, thickness), color);

        // Draw the bottom edge of the border.
        spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Bottom - thickness, bounds.Width, thickness), color);

        // Draw the left edge of the border.
        spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Y, thickness, bounds.Height), color);

        // Draw the right edge of the border.
        spriteBatch.Draw(pixel, new Rectangle(bounds.Right - thickness, bounds.Y, thickness, bounds.Height), color);
    }

    // Calculates where one Settings row should appear.
    private static Rectangle GetSettingBounds(
        // Width of the game window.
        int screenWidth,
        // Height of the game window.
        int screenHeight,
        // Number of the setting row to position.
        int index)
    {
        // Choose a size multiplier that fits the game window.
        float scale = Math.Clamp(Math.Min(screenWidth / 1280f, screenHeight / 720f), 0.55f, 1.2f);

        // Get the Settings panel's position and size.
        Rectangle panel = GetSettingsPanelBounds(screenWidth, screenHeight, scale);

        // Set the space between each row and the panel's sides.
        int horizontalPadding = (int)(28 * scale);
        // Set each row's height, with a minimum height.
        int rowHeight = Math.Max(44, (int)(74 * scale));
        // Set the space between rows, with a minimum gap.
        int rowGap = Math.Max(8, (int)(14 * scale));

        // Set the top position of the first row inside the panel.
        int rowTop = panel.Y + (int)(112 * scale);

        // Return this row's position and size.
        return new Rectangle(
            // Place the row inside the panel's left edge.
            panel.X + horizontalPadding,
            // Move each next row down by its height and the gap.
            rowTop + index * (rowHeight + rowGap),
            // Make the row fit inside the panel's side padding.
            Math.Max(1, panel.Width - horizontalPadding * 2),
            // Set the row's height.
            rowHeight);
    }

    // Calculates the position and size of the Settings panel.
    private static Rectangle GetSettingsPanelBounds(int screenWidth, int screenHeight, float scale)
    {
        // Set the panel width but do not let it exceed the window.
        int width = Math.Min((int)(720 * scale), Math.Max(1, screenWidth - 32));
        // Set the panel height but do not let it exceed the window.
        int height = Math.Min((int)(470 * scale), Math.Max(1, screenHeight - 32));

        // Return a rectangle centered in the game window.
        return new Rectangle(
            // Center the panel from left to right.
            (screenWidth - width) / 2,
            // Center the panel from top to bottom.
            (screenHeight - height) / 2,
            // Set the panel width.
            width,
            // Set the panel height.
            height);
    }

    // Calculates where the On/Off button should appear inside a Settings row.
    private static Rectangle GetSettingToggleBounds(Rectangle rowBounds, float scale)
    {
        // Set the button width.
        int width = (int)(94 * scale);
        // Set the button height, keeping it inside the row.
        int height = Math.Min((int)(44 * scale), rowBounds.Height - 8);

        // Return a rectangle near the right side and vertically centered in the row.
        return new Rectangle(
            // Place the button near the right edge.
            rowBounds.Right - (int)(14 * scale) - width,
            // Center the button from top to bottom.
            rowBounds.Center.Y - height / 2,
            // Set the button width.
            width,
            // Set the button height.
            height);
    }

    // Calculates where the Back button should appear.
    private static Rectangle GetBackBounds(int screenWidth, int screenHeight)
    {
        // Choose a size multiplier that fits the game window.
        float scale = Math.Clamp(Math.Min(screenWidth / 1280f, screenHeight / 720f), 0.55f, 1.2f);

        // Get the Settings panel's position and size.
        Rectangle panel = GetSettingsPanelBounds(screenWidth, screenHeight, scale);

        // Set the button width, keeping it inside the panel.
        int width = Math.Min((int)(230 * scale), panel.Width - (int)(40 * scale));
        // Set the button height, with a minimum size.
        int height = Math.Max(36, (int)(52 * scale));

        // Return the button's position and size near the bottom of the panel.
        return new Rectangle(
            // Center the button from left to right.
            panel.Center.X - width / 2,
            // Place the button above the panel's bottom edge.
            panel.Bottom - height - (int)(24 * scale),
            // Set the button width.
            width,
            // Set the button height.
            height);
    }
}
