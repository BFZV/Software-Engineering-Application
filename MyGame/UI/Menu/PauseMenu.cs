 using System;
 using Microsoft.Xna.Framework;
 using Microsoft.Xna.Framework.Graphics;
 using Microsoft.Xna.Framework.Input;

 namespace MyGame;

// Contains code for drawing the pause menu and its buttons.
internal static class PauseMenu
{
    // Draws the pause button on the game screen.
    public static void DrawButton(
        // Tool used to draw images and shapes.
        SpriteBatch spriteBatch,
        // Small image used to draw solid shapes.
        Texture2D pixel,
        // Width of the game window.
        int screenWidth,
        // Height of the game window.
        int screenHeight,
        // Size multiplier for the button and its parts.
        float scale)
    {
        // Find the pause button's position and size.
        Rectangle button = GetPauseButtonBounds(screenWidth, screenHeight);
        // Check whether the mouse pointer is over the button.
        bool hovered = button.Contains(Mouse.GetState().Position);

        // Set the width of each pause symbol line.
        int iconWidth = Math.Max(3, (int)(5 * scale));
        // Set the height of each pause symbol line.
        int iconHeight = Math.Max(12, (int)(20 * scale));
        // Set the space between the two lines.
        int iconGap = Math.Max(3, (int)(5 * scale));
        // Put the two lines in the center of the button from left to right.
        int iconX = button.Center.X - iconWidth - iconGap / 2;
        // Put the lines in the center of the button from top to bottom.
        int iconY = button.Center.Y - iconHeight / 2;
        // Use a darker red when the mouse is over the button.
        Color iconColor = hovered ? new Color(139, 0, 0) : Color.Red;
        // Draw the left line of the pause symbol.
        spriteBatch.Draw(pixel, new Rectangle(iconX, iconY, iconWidth, iconHeight), iconColor);
        // Draw the right line of the pause symbol.
        spriteBatch.Draw(pixel, new Rectangle(iconX + iconWidth + iconGap, iconY, iconWidth, iconHeight), iconColor);
    }

    // Draws the pause screen and its buttons.
    public static void Draw(
        // Tool used to draw images and text.
        SpriteBatch spriteBatch,
        // Font used for the menu text.
        SpriteFont font,
        // Small image used to draw solid shapes.
        Texture2D pixel,
        // Width of the game window.
        int screenWidth,
        // Height of the game window.
        int screenHeight,
        // Size multiplier for the screen elements.
        float scale)
    {
        // Draw a see-through dark layer over the whole screen.
        spriteBatch.Draw(pixel, new Rectangle(0, 0, screenWidth, screenHeight), new Color(0, 0, 0, 175));

        // Find the pause panel's position and size.
        Rectangle panel = GetPanelBounds(screenWidth, screenHeight);
        // Draw the panel with a dark background.
        UIHelper.DrawPanel(spriteBatch, pixel, panel, new Color(10, 16, 15, 245));

        // Store the text shown at the top of the panel.
        const string title = "PAUSED";
        // Measure the title at the size it will be drawn.
        Vector2 titleSize = font.MeasureString(title) * (0.82f * scale);
        // Center the title near the top of the panel.
        Vector2 titlePosition = new(panel.Center.X - titleSize.X / 2, panel.Y + 22 * scale);
        // Draw the title in red.
        spriteBatch.DrawString(font, title, titlePosition, new Color(255, 95, 65), 0, Vector2.Zero, 0.82f * scale, SpriteEffects.None, 0);

        // Draw the button that resumes the game.
        UIHelper.DrawHoverButton(spriteBatch, font, pixel, GetResumeBounds(screenWidth, screenHeight), "RESUME", scale);
        // Draw the button that opens Settings.
        UIHelper.DrawHoverButton(spriteBatch, font, pixel, GetSettingsBounds(screenWidth, screenHeight), "SETTINGS", scale);
        // Draw the button that leaves the game.
        UIHelper.DrawHoverButton(spriteBatch, font, pixel, GetLeaveBounds(screenWidth, screenHeight), "LEAVE GAME", scale);
    }

    // Returns the position and size of the pause button.
    public static Rectangle GetPauseButtonBounds(int screenWidth, int screenHeight)
    {
        // Get a size multiplier that fits the current window.
        float scale = UIHelper.GetScale(screenWidth, screenHeight);
        // Set the button size, with a minimum of 34 pixels.
        int size = Math.Max(34, (int)(48 * scale));
        // Set the space between the button and the screen edge.
        int margin = (int)(22 * scale);
        // Place the button near the top-right corner and return its rectangle.
        return new Rectangle(screenWidth - size - margin, margin + (int)(64 * scale), size, size);
    }

    // Returns the position and size of the Resume button.
    public static Rectangle GetResumeBounds(int screenWidth, int screenHeight) =>
        // Use button number 0 for Resume.
        GetOptionBounds(screenWidth, screenHeight, 0);

    // Returns the position and size of the Settings button.
    public static Rectangle GetSettingsBounds(int screenWidth, int screenHeight) =>
        // Use button number 1 for Settings.
        GetOptionBounds(screenWidth, screenHeight, 1);

    // Returns the position and size of the Leave Game button.
    public static Rectangle GetLeaveBounds(int screenWidth, int screenHeight) =>
        // Use button number 2 for Leave Game.
        GetOptionBounds(screenWidth, screenHeight, 2);

    // Calculates a panel that fits in the center of the game window.
    private static Rectangle GetPanelBounds(int screenWidth, int screenHeight)
    {
        // Get the size multiplier for this window.
        float scale = UIHelper.GetScale(screenWidth, screenHeight);
        // Choose a scaled width that does not go outside the window.
        int width = Math.Min((int)(470 * scale), Math.Max(1, screenWidth - 32));
        // Choose a scaled height that does not go outside the window.
        int height = Math.Min((int)(350 * scale), Math.Max(1, screenHeight - 32));
        // Return a rectangle centered in the window.
        return new Rectangle((screenWidth - width) / 2, (screenHeight - height) / 2, width, height);
    }

    // Calculates the position and size of one button in the pause menu.
    private static Rectangle GetOptionBounds(int screenWidth, int screenHeight, int index)
    {
        // Get the size multiplier for this window.
        float scale = UIHelper.GetScale(screenWidth, screenHeight);
        // Get the pause panel's position and size.
        Rectangle panel = GetPanelBounds(screenWidth, screenHeight);
        // Set the space between the buttons and the panel's sides.
        int horizontalPadding = (int)(35 * scale);
        // Set the button width based on the panel width.
        int width = Math.Max(1, panel.Width - horizontalPadding * 2);
        // Set the button height, with a minimum of 34 pixels.
        int height = Math.Max(34, (int)(52 * scale));
        // Set the space between buttons, with a minimum of 6 pixels.
        int gap = Math.Max(6, (int)(12 * scale));
        // Find the top position of the first button.
        int top = panel.Y + (int)(92 * scale);
        // Return the button's rectangle; index places each button below the one before it.
        return new Rectangle(panel.X + horizontalPadding, top + index * (height + gap), width, height);
    }
}
