 using System;
 using Microsoft.Xna.Framework;
 using Microsoft.Xna.Framework.Graphics;

 namespace MyGame.UI;

// Holds helper code for drawing the game-over screen.
internal static class DeathScreen
{
    // Draws the game-over screen. The parameters provide drawing tools and game details.
    public static void Draw(
        // Draws shapes and text on the screen.
        SpriteBatch spriteBatch,
        // Provides the font used for the screen text.
        SpriteFont font,
        // A small image used to draw solid-colored shapes.
        Texture2D pixel,
        // The width of the game window in pixels.
        int screenWidth,
        // The height of the game window in pixels.
        int screenHeight,
        // Resizes screen elements to fit the window.
        float scale,
        // The time the player died; this parameter is available to the caller.
        float deathTime)
    {
        // Draw a see-through dark rectangle over the whole screen.
        spriteBatch.Draw(pixel, new Rectangle(0, 0, screenWidth, screenHeight), new Color(5, 0, 0, 205));

        // Get the panel's position and size.
        Rectangle panel = GetPanelBounds(screenWidth, screenHeight);
        // Draw the panel using a dark color.
        UIHelper.DrawPanel(spriteBatch, pixel, panel, new Color(20, 9, 10, 248));

        // Store the title text in a constant because it does not change.
        const string title = "YOU DIED";
        // Set the title size using the screen scale.
        float titleScale = 0.88f * scale;
        // Measure how much space the title takes up.
        Vector2 titleSize = font.MeasureString(title) * titleScale;
        // Find a position that puts the title in the center near the top of the panel.
        Vector2 titlePosition = new(panel.Center.X - titleSize.X / 2, panel.Y + 24 * scale);

        // Draw the title slightly lower and to the side to make a shadow.
        spriteBatch.DrawString(font, title, titlePosition + new Vector2(2 * scale, 3 * scale), Color.Black, 0, Vector2.Zero, titleScale, SpriteEffects.None, 0);
        // Draw the red title over the shadow.
        spriteBatch.DrawString(font, title, titlePosition, new Color(235, 45, 42), 0, Vector2.Zero, titleScale, SpriteEffects.None, 0);

        // Store the message shown below the title.
        const string subtitle = "YOUR RUN IS OVER";
        // Set the subtitle size using the screen scale.
        float subtitleScale = 0.42f * scale;
        // Measure how much space the subtitle takes up.
        Vector2 subtitleSize = font.MeasureString(subtitle) * subtitleScale;
        // Center the subtitle below the title.
        Vector2 subtitlePosition = new(panel.Center.X - subtitleSize.X / 2, titlePosition.Y + titleSize.Y + 8 * scale);
        // Draw the subtitle in light gray.
        spriteBatch.DrawString(font, subtitle, subtitlePosition, Color.LightGray, 0, Vector2.Zero, subtitleScale, SpriteEffects.None, 0);

        // Draw the button that lets the player leave the game.
        UIHelper.DrawHoverButton(spriteBatch, font, pixel, GetLeaveBounds(screenWidth, screenHeight), "LEAVE GAME", scale);
        // Draw the button that lets the player start again.
        UIHelper.DrawHoverButton(spriteBatch, font, pixel, GetRestartGameBounds(screenWidth, screenHeight), "RESTART GAME", scale);
    }

    // Returns the position and size of the leave-game button.
    public static Rectangle GetLeaveBounds(int screenWidth, int screenHeight) =>
        // Use button number 0 to get the first button's position.
        GetOptionBounds(screenWidth, screenHeight, 0);

    // Returns the position and size of the restart button.
    public static Rectangle GetRestartGameBounds(int screenWidth, int screenHeight) =>
        // Use button number 1 to get the second button's position.
        GetOptionBounds(screenWidth, screenHeight, 1);

    // Calculates a panel that is centered and fits inside the game window.
    private static Rectangle GetPanelBounds(int screenWidth, int screenHeight)
    {
        // Get the size multiplier for this window.
        float scale = UIHelper.GetScale(screenWidth, screenHeight);
        // Choose a scaled width, but do not let the panel go outside the window.
        int width = Math.Min((int)(590 * scale), Math.Max(1, screenWidth - 32));
        // Choose a scaled height, but do not let the panel go outside the window.
        int height = Math.Min((int)(480 * scale), Math.Max(1, screenHeight - 32));
        // Return a rectangle positioned in the center of the window.
        return new Rectangle((screenWidth - width) / 2, (screenHeight - height) / 2, width, height);
    }

    // Calculates the position and size of a button inside the panel.
    private static Rectangle GetOptionBounds(int screenWidth, int screenHeight, int index)
    {
        // Get the size multiplier for this window.
        float scale = UIHelper.GetScale(screenWidth, screenHeight);
        // Get the panel's position and size.
        Rectangle panel = GetPanelBounds(screenWidth, screenHeight);

        // Leave space between the panel's sides and the buttons.
        int horizontalPadding = Math.Max(14, (int)(34 * scale));
        // Make the button fill the panel, leaving the side space on both sides.
        int width = Math.Max(1, panel.Width - horizontalPadding * 2);
        // Set the button height, with a minimum size of 34 pixels.
        int height = Math.Max(34, (int)(48 * scale));
        // Set the space between buttons, with a minimum of 6 pixels.
        int gap = Math.Max(6, (int)(10 * scale));
        // Leave space between the buttons and the bottom of the panel.
        int bottomPadding = Math.Max(16, (int)(24 * scale));

        // Find the top edge of the first button so both buttons fit near the bottom.
        int top = panel.Bottom - bottomPadding - height * 2 - gap;
        // Return this button's rectangle; index moves the second button below the first.
        return new Rectangle(panel.X + horizontalPadding, top + index * (height + gap), width, height);
    }
}
