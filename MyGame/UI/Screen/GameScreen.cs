 using System;
 using Microsoft.Xna.Framework;
 using Microsoft.Xna.Framework.Graphics;
 using Microsoft.Xna.Framework.Input;

 namespace MyGame.UI;

// Lists the possible results from updating the game screen.
public enum GameScreenResult
{
    // Keep playing on the current screen.
    Continue,
    // Return to the main menu.
    LeaveGame,
    // Open the settings screen.
    OpenSettings,
    // Start the game over.
    RestartGame
}

// Controls the main gameplay screen and its menus.
public class GameScreen
{
    // Remembers the keyboard buttons from the previous update.
    private KeyboardState _previousKeyboardState;
    // Remembers the mouse buttons from the previous update.
    private MouseState _previousMouseState;

    // Remembers whether the shop is open.
    private bool _shopOpen;
    // Remembers whether the shotgun was purchased.
    private bool _shotgunPurchased;
    // Creates a health bar, starting full at 100.
    private readonly Bar _health = new(100, 100);
    // Creates an energy bar, starting full at 100.
    private readonly Bar _energy = new(100, 100);
    // Stores weapon upgrade levels for the shop.
    private int _pistolUpgradeLevel = 1;
    private int _shotgunUpgradeLevel = 1;
    // Remembers whether the game is paused.
    private bool _paused;
    // Remembers whether the player has lost.
    private bool _dead;
    // Tracks how long the death screen has been shown.
    private float _deathTime;

    // Gives other code the current health value.
    public float Health => _health.Current;
    // Gives other code the maximum health value.
    public float MaximumHealth => _health.Maximum;
    // Gives other code the current energy value.
    public float Energy => _energy.Current;
    // Gives other code the maximum energy value.
    public float MaximumEnergy => _energy.Maximum;
    // Stores the available money; only this class can change it directly.
    public int Money { get; private set; }
    public bool IsPaused => _paused;
    public bool IsShopOpen => _shopOpen;
    public bool IsDead => _dead;
    public GameScreen()
    {
        // Remember the keyboard state when this screen is created.
        _previousKeyboardState = Keyboard.GetState();
        // Remember the mouse state when this screen is created.
        _previousMouseState = Mouse.GetState();
    }

    // Updates game menus and returns an action if the screen changes.
    public GameScreenResult Update(
        // Provides the time passed since the previous game update.
        GameTime gameTime,
        // Provides the width of the game window.
        int screenWidth,
        // Provides the height of the game window.
        int screenHeight,
        // Provides a method to play the menu sound.
        Action playMenuSound)
    {
        // Read the current keyboard buttons.
        KeyboardState keyboardState = Keyboard.GetState();
        // Read the current mouse buttons and position.
        MouseState mouseState = Mouse.GetState();
        // Get the time passed, but do not allow one update to use more than 0.1 seconds.
        float elapsedSeconds = Math.Min((float)gameTime.ElapsedGameTime.TotalSeconds, 0.1f);

        // True only on the first update after Escape is pressed.
        bool escapePressed = keyboardState.IsKeyDown(Keys.Escape) &&
            _previousKeyboardState.IsKeyUp(Keys.Escape);

        // Handle clicks on the death screen.
        if (_dead)
        {
            // Increase the time shown on the death screen.
            _deathTime += elapsedSeconds;
            // Check whether the player clicked the mouse.
            if (IsMouseClicked(mouseState))
            {
                // Check whether the click was on the leave button.
                if (DeathScreen.GetLeaveBounds(screenWidth, screenHeight).Contains(mouseState.Position))
                {
                    // Play a sound for the button click.
                    playMenuSound();
                    // Tell the main game to return to the menu.
                    return GameScreenResult.LeaveGame;
                }

                // Check whether the click was on the restart button.
                if (DeathScreen.GetRestartGameBounds(screenWidth, screenHeight).Contains(mouseState.Position))
                {
                    // Play a sound for the button click.
                    playMenuSound();
                    // Tell the main game to restart.
                    return GameScreenResult.RestartGame;
                }
            }
        }
        // Handle input while the pause menu is open.
        else if (_paused)
        {
            // Resume if Escape is pressed or the Resume button is clicked.
            if (escapePressed || IsMouseClicked(mouseState) &&
                PauseMenu.GetResumeBounds(screenWidth, screenHeight).Contains(mouseState.Position))
            {
                // Play a sound only when the player clicked the button.
                if (!escapePressed)
                {
                    playMenuSound();
                }

                // Close the pause menu.
                _paused = false;
            }
            // Check whether the Settings button was clicked.
            else if (IsMouseClicked(mouseState) &&
                     PauseMenu.GetSettingsBounds(screenWidth, screenHeight).Contains(mouseState.Position))
            {
                // Play the button sound.
                playMenuSound();
                // Save the current input so it is not read again on the next screen.
                SaveInputState(keyboardState, mouseState);
                // Ask the main game to open Settings.
                return GameScreenResult.OpenSettings;
            }
            // Check whether the Leave button was clicked.
            else if (IsMouseClicked(mouseState) &&
                     PauseMenu.GetLeaveBounds(screenWidth, screenHeight).Contains(mouseState.Position))
            {
                // Play the button sound.
                playMenuSound();
                // Save the current input before changing screens.
                SaveInputState(keyboardState, mouseState);
                // Ask the main game to return to the menu.
                return GameScreenResult.LeaveGame;
            }
        }
        // Handle input while the shop is open.
        else if (_shopOpen)
        {
            // Check whether the shotgun purchase button was clicked.
            if (IsMouseClicked(mouseState) &&
                ShopMenu.GetShotgunPurchaseBounds(screenWidth, screenHeight).Contains(mouseState.Position))
            {
                // Buy the shotgun only if it has not been bought and there is enough money.
                if (!_shotgunPurchased && Money >= 5000)
                {
                    // Subtract the shotgun's price.
                    Money -= 5000;
                    // Remember that the shotgun is now owned.
                    _shotgunPurchased = true;
                    // Play the purchase sound.
                    playMenuSound();
                }
            }
            // Check whether the pistol upgrade button was clicked.
            else if (IsMouseClicked(mouseState) &&
                ShopMenu.GetPistolUpgradeBounds(screenWidth, screenHeight).Contains(mouseState.Position))
            {
                // Upgrade the pistol only if there is enough money and it is below the level cap.
                if (Money >= 1000 && _pistolUpgradeLevel < ShopMenu.MaximumUpgradeLevel)
                {
                    // Subtract the pistol upgrade's price.
                    Money -= 1000;
                    // Increase the pistol upgrade level.
                    _pistolUpgradeLevel++;
                    // Play the purchase sound.
                    playMenuSound();
                }
            }
            // Check whether the shotgun is owned and its upgrade button was clicked.
            else if (_shotgunPurchased &&
                IsMouseClicked(mouseState) &&
                ShopMenu.GetShotgunUpgradeBounds(screenWidth, screenHeight).Contains(mouseState.Position))
            {
                // Upgrade the shotgun only if there is enough money and it is below the level cap.
                if (Money >= 2000 && _shotgunUpgradeLevel < ShopMenu.MaximumUpgradeLevel)
                {
                    // Subtract the shotgun upgrade's price.
                    Money -= 2000;
                    // Increase the shotgun upgrade level.
                    _shotgunUpgradeLevel++;
                    // Play the purchase sound.
                    playMenuSound();
                }
            }
            // Close the shop with Escape.
            else if (escapePressed)
            {
                _shopOpen = false;
            }
        }
        // Handle regular gameplay when no menu is open.
        else
        {
            // Pause if Escape is pressed or the pause button is clicked.
            if (escapePressed || IsMouseClicked(mouseState) &&
                PauseMenu.GetPauseButtonBounds(screenWidth, screenHeight).Contains(mouseState.Position))
            {
                // Play a sound only if the pause button was clicked.
                if (!escapePressed)
                {
                    playMenuSound();
                }

                // Open the pause menu.
                _paused = true;
                // Save input so the same key or click does not affect the pause menu.
                SaveInputState(keyboardState, mouseState);
                // Stay on the game screen.
                return GameScreenResult.Continue;
            }

            // Check whether the shop button was clicked.
            if (IsMouseClicked(mouseState) &&
                ShopMenu.GetShopButtonBounds(screenWidth, screenHeight).Contains(mouseState.Position))
            {
                // Play the button sound.
                playMenuSound();
                // Open the shop.
                _shopOpen = true;
            }

            // Check whether health has run out.
            if (!_shopOpen)
            {
                if (Health <= 0)
                {
                    _dead = true;
                    _deathTime = 0;
                }
            }
        }

        // Save the current input for comparison during the next update.
        SaveInputState(keyboardState, mouseState);
        // Tell the main game to keep showing this screen.
        return GameScreenResult.Continue;
    }//end of Update()

    // Sets the player's health and its maximum value.
    public void SetHealth(float current, float maximum)
    {
        // Let the health bar check and store the values.
        _health.Set(current, maximum);
    }

    // Sets the player's energy and its maximum value.
    public void SetEnergy(float current, float maximum)
    {
        // Let the energy bar validate and store these values.
        _energy.Set(current, maximum);
    }

    // Adds money to the player's total.
    public void AddMoney(int amount)
    {
        // Add the amount, catch integer overflow, and do not allow money below zero.
        Money = Math.Max(0, checked(Money + amount));
    }

    public void DrawBackground(SpriteBatch spriteBatch, Texture2D titleBackground,
    int screenWidth, int screenHeight)
    {
        spriteBatch.Draw(
            titleBackground,
            new Rectangle(0, 0, screenWidth, screenHeight),
            Color.White
        );
    }

    // Draws gameplay, the status bars, and any open menu.
    public void DrawUI(
        // Tool used to draw images and text.
        SpriteBatch spriteBatch,
        // Font used for larger labels.
        SpriteFont font,
        // Font used for smaller display text.
        SpriteFont displayFont,
        // Small image stretched to draw simple shapes.
        Texture2D pixel,
        // Image shown beside the health bar.
        Texture2D healthIcon,
        // Image shown beside the energy bar.
        Texture2D energyIcon,
        // Image used for the first weapon upgrade.
        Texture2D upgrade1Weapon,
        // Image used in the shop.
        Texture2D shopWeapon,
        // Image used when a weapon is unknown.
        Texture2D unknownWeapon,
        // Background image used by the game and menus.
        Texture2D titleBackground,
        // Width of the game window.
        int screenWidth,
        // Height of the game window.
        int screenHeight)
    {
        // Find a size multiplier that fits the current window.
        float scale = UIHelper.GetScale(screenWidth, screenHeight);
        // Set the space between the edge of the screen and the status bars.
        int margin = (int)(22 * scale);
        // Set the space between the two status bars.
        int barGap = (int)(18 * scale);
        // Choose a bar width that fits two bars on the screen.
        int barWidth = Math.Min((int)(240 * scale), (screenWidth - margin * 2 - barGap) / 2);
        // Set the bar height, keeping it at least 12 pixels.
        int barHeight = Math.Max(12, (int)(24 * scale));
        // Set the vertical position of the bars.
        int barY = margin + (int)(34 * scale);
        // Set the horizontal position of the left bar.
        int leftX = margin;
        // Set the horizontal position of the right bar.
        int rightX = leftX + barWidth + barGap;
        // Set the text size for the bar labels.
        float fontScale = 0.58f * scale;
        // Set a smaller text size for the displayed values.
        float displayFontScale = fontScale * (2f / 3f);

        // Draw the health bar.
        _health.Draw(
            // Pass the drawing tool.
            spriteBatch,
            // Pass the label font.
            font,
            // Pass the value font.
            displayFont,
            // Pass the image used for the bar.
            pixel,
            // Set the bar's position and size.
            new Rectangle(leftX, barY, barWidth, barHeight),
            // Set the label text.
            "HEALTH",
            // Set the bar's red fill color.
            new Color(235, 35, 35),
            // Pass the font sizes.
            fontScale,
            displayFontScale,
            // Show the health icon.
            healthIcon);
        // Draw the energy bar.
        _energy.Draw(
            // Pass the drawing tool.
            spriteBatch,
            // Pass the label font.
            font,
            // Pass the value font.
            displayFont,
            // Pass the image used for the bar.
            pixel,
            // Set the energy bar's position and size.
            new Rectangle(rightX, barY, barWidth, barHeight),
            // Set the label text.
            "ENERGY",
            // Set the bar's green fill color.
            new Color(75, 220, 35),
            // Pass the font sizes.
            fontScale,
            displayFontScale,
            // Show the energy icon.
            energyIcon);
        // Set the vertical position below the two bars for money text.
        int moneyY = barY + barHeight + (int)(14 * scale);
        // Draw the player's money amount.
        UIHelper.DrawText(spriteBatch, displayFont, $"MONEY   ${Money:N0}", leftX, moneyY, new Color(255, 220, 75), displayFontScale);
        // Draw the shop menu only while it is open.
        if (_shopOpen)
        {
            // Draw the shop with its images, player upgrades, and money amount.
            ShopMenu.Draw(
                // Pass the drawing tool.
                spriteBatch,
                // Pass the menu font.
                displayFont,
                // Pass the image used for simple shapes.
                pixel,
                // Pass the background image.
                titleBackground,
                // Pass the weapon images.
                upgrade1Weapon,
                shopWeapon,
                unknownWeapon,
                // Pass the screen size and scale.
                screenWidth,
                screenHeight,
                scale,
                // Tell the shop whether the shotgun was purchased.
                _shotgunPurchased,
                // Pass the player's money and upgrade levels.
                Money,
                _pistolUpgradeLevel,
                _shotgunUpgradeLevel);
        }

        // Draw the pause button when the shop is closed.
        if (!_shopOpen)
        {
            PauseMenu.DrawButton(spriteBatch, pixel, screenWidth, screenHeight, scale);
        }
        // Draw the pause menu when the game is paused.
        if (_paused)
        {
            PauseMenu.Draw(spriteBatch, displayFont, pixel, screenWidth, screenHeight, scale);
        }
        // Draw the death screen after the player loses.
        if (_dead)
        {
            DeathScreen.Draw(spriteBatch, displayFont, pixel, screenWidth, screenHeight, scale, _deathTime);
        }
    }

    // Returns true only when the left mouse button was just pressed.
    private bool IsMouseClicked(MouseState mouseState)
    {
        // Compare the current button state with the state from the previous update.
        return mouseState.LeftButton == ButtonState.Pressed &&
            _previousMouseState.LeftButton == ButtonState.Released;
    }

    // Saves keyboard and mouse states for the next update.
    private void SaveInputState(KeyboardState keyboardState, MouseState mouseState)
    {
        // Remember which keyboard buttons are currently pressed.
        _previousKeyboardState = keyboardState;
        // Remember which mouse buttons are currently pressed.
        _previousMouseState = mouseState;
    }
}
