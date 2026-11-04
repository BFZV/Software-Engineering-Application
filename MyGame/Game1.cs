 using System;
 using Microsoft.Xna.Framework;
 using Microsoft.Xna.Framework.Graphics;
 using Microsoft.Xna.Framework.Input;
 using Microsoft.Xna.Framework.Media;
 using MyGame.Sprite;
 using MyGame.UI;

namespace MyGame;

public class Game1 : Microsoft.Xna.Framework.Game 
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SpriteManager spriteManager;
    private PlayerSprite player;

    //Stores the title screen, menu screen, and active game screen.
    private TitleScreen _titleScreen;
    private MenuScreen _menuScreen;
    private GameScreen _gameScreen;

    private SpriteFont _titleFont;
    private SpriteFont _titleScreenFont;

    //Stores pictures used by the title screen, shop, and interface.
    private Texture2D _titleBackground;
    private Texture2D _upgrade1Weapon;
    private Texture2D _shopWeapon;
    private Texture2D _unknownWeapon;
    private Texture2D _healthIcon;
    private Texture2D _energyIcon;
    private Texture2D _titlePixel;

    // Stores the sound used by the menu.
    private Song _menuSelectionSound;

    // Remembers which screen should currently be shown.
    private bool _showMenu;
    private bool _showGameScreen;

    //Remembers whether to return to the game after closing Settings.
    private bool _returnToGameAfterSettings;

    public GameState CurrentGameState { get; private set; } = GameState.MainMenu;

    public Game1()
    {
        //Set up the object that controls the game window.
        _graphics = new GraphicsDeviceManager(this);

        //Tell the game where its content files are stored.
        Content.RootDirectory = "Content";

        //Keep the mouse pointer visible.
        IsMouseVisible = true;
    }

    //Sets up the game before loading its content.
    protected override void Initialize()
    {
        //Get the width and height of your current screen
        _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

        //Enable fullscreen without a hardware mode switch (creates a borderless window)
        _graphics.IsFullScreen = true;
    
        //Save the changes
        _graphics.ApplyChanges();
        
        //Create the title screen.
        _titleScreen = new TitleScreen();

        spriteManager = new SpriteManager(this);
        Components.Add(spriteManager);

        spriteManager.Enabled = false;
        spriteManager.Visible = false;

        SetGameState(GameState.MainMenu);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        //Load the font used during gameplay.
        _titleFont = Content.Load<SpriteFont>("TitleFont");
        //Load the font used by the title screen and menus.
        _titleScreenFont = Content.Load<SpriteFont>("TitleDisplayFont");

        //Load the title screen background image.
        _titleBackground = Content.Load<Texture2D>("TitleBackground");

        //Load the shop images and interface icons.
        _upgrade1Weapon = Content.Load<Texture2D>("Upgrade1Weapon");
        _shopWeapon = Content.Load<Texture2D>("ShopWeapon");
        _unknownWeapon = Content.Load<Texture2D>("UnknownWeapon");
        _healthIcon = Content.Load<Texture2D>("HealthIcon");
        _energyIcon = Content.Load<Texture2D>("EnergyIcon");
        //Load the sound used by the menu.
        _menuSelectionSound = Content.Load<Song>("freesound_community-menu-selection-102220");

        //Create a 1-by-1 image that can be stretched to draw simple shapes.
        _titlePixel = new Texture2D(GraphicsDevice, 1, 1);

        //Fill that tiny image with white.
        _titlePixel.SetData(new[] { Color.White });
        
    }

    protected override void Update(GameTime gameTime)
    {
        switch (CurrentGameState)
        {
            case GameState.MainMenu:

                // Handle title screen input.
                // Start button should transition to Playing.

                break;

            case GameState.Playing:

                // Handle gameplay input.
                // Pause button should transition to Paused.

                break;

            case GameState.Paused:

                // Handle pause menu input.
                // Resume button should transition to Playing.

                break;

            case GameState.WaveTransition:

                // Handle next wave countdown or transition.

                break;

            case GameState.GameOver:

                // Handle restart or return to main menu.

                break;
        }
        
        
        // Exit if the player presses Back on a gamepad or Escape outside gameplay.
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            (Keyboard.GetState().IsKeyDown(Keys.Escape) &&
             !_showGameScreen &&
             (!_showMenu || !_menuScreen.IsSettingsOpen)))
        {
            // Close the game.
            Exit();
        }

        // Update the game screen when gameplay is active.
        if (_showGameScreen)
        {
            // Update gameplay and get any screen action the player selected.
            GameScreenResult result = _gameScreen.Update(
                // Pass the elapsed time since the previous update.
                gameTime,
                // Pass the current screen width.
                GraphicsDevice.Viewport.Width,
                // Pass the current screen height.
                GraphicsDevice.Viewport.Height,
                // Pass the method used to play a menu sound.
                PlayMenuSound);

            // Check whether the player chose to leave the game.
            if (result == GameScreenResult.LeaveGame)
            {
                // Hide gameplay and show the menu.
                SetGameState(GameState.MainMenu);
                _showMenu = false;
                _showGameScreen = false;

                // Do not return to gameplay after Settings.
                _returnToGameAfterSettings = false;

                // Create a fresh menu screen.
                _menuScreen = new MenuScreen();
            }
            // Check whether the player chose to open Settings.
            else if (result == GameScreenResult.OpenSettings)
            {
                // Open the settings page in the menu.
                _menuScreen.OpenSettings();
                // Hide gameplay and show the menu.
                
                _showMenu = true;
                _showGameScreen = false;
                SetGameState(GameState.Paused);
                // Return to gameplay when Settings is closed.
                _returnToGameAfterSettings = true;
            }
            // Check whether the player chose to restart.
            else if (result == GameScreenResult.RestartGame)
            {
                // Create a new game screen to start the game again.
                _gameScreen = new GameScreen();
                SetGameState(GameState.Playing);
                _showMenu = false;
                _showGameScreen = true;
            }

        }
        // If the menu is not showing, update the title screen.
        else if (!_showMenu)
        {
            // Check whether the player pressed a key on the title screen.
            bool keyPressed = _titleScreen.Update();
            // Open the menu if the player pressed a key.
            if (keyPressed)
            {
                // Create a menu and show it.
                _menuScreen = new MenuScreen();
                _showMenu = true;
            }
        }
        // Otherwise, update the menu because it is showing.
        else if (_showMenu)
        {
            // Remember whether Settings was open before this update.
            bool settingsWereOpen = _menuScreen.IsSettingsOpen;

            // Update the menu and find out whether an option was selected.
            MenuOption? selectedOption = _menuScreen.Update(
                // Pass the font used to draw the menu.
                _titleScreenFont,
                // Pass the current screen width.
                GraphicsDevice.Viewport.Width,
                // Pass the current screen height.
                GraphicsDevice.Viewport.Height,
                // Pass the method used to play a menu sound.
                PlayMenuSound);

            // Return to gameplay if the player just closed Settings opened from the game.
            if (_returnToGameAfterSettings && settingsWereOpen && !_menuScreen.IsSettingsOpen)
            {
                // Clear the return flag and switch back to gameplay.
                _returnToGameAfterSettings = false;
                SetGameState(GameState.Playing);
                _showMenu = false;
                _showGameScreen = true;
            }
            // Close the game if the player chose Quit.
            else if (selectedOption == MenuOption.Quit)
            {
                Exit();
            }
            // Start a new game if the player chose Start Game.
            else if (selectedOption == MenuOption.StartGame)
            {
                // Create a new game screen.
                _gameScreen = new GameScreen();

                // Hide the menu and show gameplay.
                _showMenu = false;
                _showGameScreen = true;
                SetGameState(GameState.Playing);
            }
        }

        // Synchronize gameplay with the state of GameScreen.
        if (_showGameScreen && _gameScreen != null)
        {
            if (_gameScreen.IsDead)
            {
                SetGameState(GameState.GameOver);
            }
            else if (_gameScreen.IsPaused || _gameScreen.IsShopOpen)
            {
                SetGameState(GameState.Paused);
            }
            else
            {
                SetGameState(GameState.Playing);
            }
        }
        base.Update(gameTime);
    }

    // Plays the menu selection sound if audio is turned on.
    private void PlayMenuSound()
    {
        // Check whether the player has enabled audio.
        if (_menuScreen.IsAudioEnabled)
        {
            // Make sure this sound does not repeat.
            MediaPlayer.IsRepeating = false;
            // Play the menu selection sound.
            MediaPlayer.Play(_menuSelectionSound);
        }
    }

    private void SetGameState(GameState newState)
    {
        CurrentGameState = newState;

        switch (CurrentGameState)
        {
            case GameState.MainMenu:

                spriteManager.Enabled = false;
                spriteManager.Visible = false;
                break;

            case GameState.Playing:

                spriteManager.Enabled = true;
                spriteManager.Visible = true;
                break;

            case GameState.Paused:

                spriteManager.Enabled = false;

                // Keep sprites visible behind the in-game
                // pause menu, but hide them behind Settings.
                spriteManager.Visible = _showGameScreen;
                break;

            case GameState.WaveTransition:

                spriteManager.Enabled = false;
                spriteManager.Visible = _showGameScreen;
                break;

            case GameState.GameOver:

                spriteManager.Enabled = false;
                spriteManager.Visible = false;
                break;
        }
    }
    protected override void Draw(GameTime gameTime)
    {
        // Clear the screen to black before drawing.
        GraphicsDevice.Clear(Color.Black);

        // Draw the menu if it is active.
        if (_showMenu)
        {
            // Start drawing the menu.
            _spriteBatch.Begin();

            // Draw the menu using the needed images and screen size.
            _menuScreen.Draw(
                // Pass the drawing tool.
                _spriteBatch,
                // Pass the menu font.
                _titleScreenFont,
                // Pass the background image.
                _titleBackground,
                // Pass the image used for simple shapes.
                _titlePixel,
                // Pass the screen width.
                GraphicsDevice.Viewport.Width,
                // Pass the screen height.
                GraphicsDevice.Viewport.Height);

            // Finish drawing the menu.
            _spriteBatch.End();

            // Stop here so no other screen is drawn over the menu.
            return;
        }

        // Draw gameplay if the game screen is active.
        if (_showGameScreen)
        {
            // Clear the screen to the gameplay background color.
            GraphicsDevice.Clear(new Color(28, 34, 30));

            // Start drawing gameplay.
            _spriteBatch.Begin();

            _gameScreen.DrawBackground(
                _spriteBatch,
                _titleBackground,
                GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height
            );

            _spriteBatch.End();

            // Draw player and enemies.
            // MonoGame automatically calls SpriteManager.Draw()
            base.Draw(gameTime);

            // Draw the game screen using its fonts, images, and screen size.
            _spriteBatch.Begin();
            _gameScreen.DrawUI(
                // Pass the drawing tool.
                _spriteBatch,
                // Pass the gameplay font.
                _titleFont,
                // Pass the title-screen font.
                _titleScreenFont,
                // Pass the image used for simple shapes.
                _titlePixel,
                // Pass the health icon.
                _healthIcon,
                // Pass the energy icon.
                _energyIcon,
                // Pass the first weapon upgrade image.
                _upgrade1Weapon,
                // Pass the shop weapon image.
                _shopWeapon,
                // Pass the image for an unknown weapon.
                _unknownWeapon,
                // Pass the title background image.
                _titleBackground,
                // Pass the screen width.
                GraphicsDevice.Viewport.Width,
                // Pass the screen height.
                GraphicsDevice.Viewport.Height);

            // Finish drawing gameplay.
            _spriteBatch.End();

            // Stop here so the title screen is not drawn over gameplay.
            return;
        }

        // Start drawing the title screen and smooth scaled images.
        _spriteBatch.Begin(samplerState: SamplerState.LinearClamp);

        // Store the current screen width.
        int screenWidth = GraphicsDevice.Viewport.Width;
        // Store the current screen height.
        int screenHeight = GraphicsDevice.Viewport.Height;

        // Draw the background image to cover the whole screen.
        _spriteBatch.Draw(
            // Use the title background image.
            _titleBackground,
            // Stretch the image to cover the screen.
            new Rectangle(0, 0, screenWidth, screenHeight),
            // Keep the image's original colors.
            Color.White);

        // Store the title text.
        const string title = "SURVIVAL OF THE FITTEST";
        // Store the instruction shown below the title.
        const string prompt = "PRESS ANY KEY";

        // Measure the title so it can be sized and centered.
        Vector2 titleSize = _titleScreenFont.MeasureString(title);

        // Calculate a value that makes the prompt pulse over time.
        float promptPulse = 0.5f + 0.5f * (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 3.5);

        // Use the pulse value to make the prompt fade in and out.
        float promptAlpha = 0.08f + 0.92f * promptPulse;

        // Shrink the title if needed so it fits within the screen width.
        float titleScale = Math.Min(1f, (screenWidth - 40f) / titleSize.X);

        // Measure the prompt and choose a size that fits the screen.
        Vector2 promptSize = _titleScreenFont.MeasureString(prompt);
        // Shrink the prompt if needed so it fits within the screen width.
        float promptScale = Math.Min(0.72f, (screenWidth - 40f) / promptSize.X);

        // Set the center point used to position the title.
        Vector2 titleOrigin = titleSize / 2f;
        // Set the center point used to position the prompt.
        Vector2 promptOrigin = promptSize / 2f;

        // Place the title and prompt at different heights on the screen.
        Vector2 titlePosition = new(screenWidth / 2f, screenHeight * 0.34f);
        Vector2 promptPosition = new(screenWidth / 2f, screenHeight * 0.70f);

        // Draw a dark copy of the title slightly offset to create a shadow.
        _spriteBatch.DrawString(
            // Use the title screen font.
            _titleScreenFont,
            // Draw the title text.
            title,
            // Move the shadow a little right and down.
            titlePosition + new Vector2(3, 4),
            // Make the shadow black.
            Color.Black,
            // Do not rotate the text.
            0,
            // Center the text around its position.
            titleOrigin,
            // Use the title's calculated size.
            titleScale,
            // Do not flip the text.
            SpriteEffects.None,
            // Draw at the default depth.
            0);

        // Draw the red title over its shadow.
        _spriteBatch.DrawString(
            // Use the title screen font.
            _titleScreenFont,
            // Draw the title text.
            title,
            // Place the title at its centered position.
            titlePosition,
            // Use a dark red color.
            new Color(155, 25, 18),
            // Do not rotate the text.
            0,
            // Center the text around its position.
            titleOrigin,
            // Use the title's calculated size.
            titleScale,
            // Do not flip the text.
            SpriteEffects.None,
            // Draw at the default depth.
            0);

        // Draw a dark copy of the prompt slightly offset to create a shadow.
        _spriteBatch.DrawString(
            // Use the title screen font.
            _titleScreenFont,
            // Draw the instruction text.
            prompt,
            // Move the shadow a little right and down.
            promptPosition + new Vector2(2, 3),
            // Use a dark color and make it pulse with the prompt.
            new Color(55, 30, 18) * promptAlpha,
            // Do not rotate the text.
            0,
            // Center the text around its position.
            promptOrigin,
            // Use the prompt's calculated size.
            promptScale,
            // Do not flip the text.
            SpriteEffects.None,
            // Draw at the default depth.
            0);

        // Draw the bright prompt over its shadow, using its pulsing transparency.
        _spriteBatch.DrawString(
            // Use the title screen font.
            _titleScreenFont,
            // Draw the instruction text.
            prompt,
            // Place the prompt at its centered position.
            promptPosition,
            // Use a light color and make it pulse with the prompt.
            new Color(255, 238, 210) * promptAlpha,
            // Do not rotate the text.
            0,
            // Center the text around its position.
            promptOrigin,
            // Use the prompt's calculated size.
            promptScale,
            // Do not flip the text.
            SpriteEffects.None,
            // Draw at the default depth.
            0);

        // Finish drawing the title screen.
        _spriteBatch.End();

        // Run the base game's drawing code.
        base.Draw(gameTime);
    }
}
