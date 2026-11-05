 using System;
 using System.Text;
 using Microsoft.Xna.Framework;
 using Microsoft.Xna.Framework.Graphics;

 namespace MyGame;

// Holds helper methods used to draw parts of the game's interface.
internal static class UIHelper
{
    // Calculates a size multiplier based on the game window.
    public static float GetScale(int screenWidth, int screenHeight)
    {
        // Use the smaller screen ratio, then keep the result between 0.55 and 1.5.
        return Math.Clamp(Math.Min(screenWidth / 1280f, screenHeight / 720f), 0.55f, 1.5f);
    }

    // Draws a filled panel and its border.
    public static void DrawPanel(
        // Tool used to draw images and shapes.
        SpriteBatch spriteBatch,
        // Small image used to draw solid shapes.
        Texture2D pixel,
        // Position and size of the panel.
        Rectangle bounds,
        // Color used to fill the panel.
        Color color)
    {
        // Draw the panel's background.
        spriteBatch.Draw(pixel, bounds, color);
        // Draw a border around the panel.
        DrawOutline(spriteBatch, pixel, bounds, new Color(175, 185, 170));
    }

    // Draws a border around a rectangle.
    public static void DrawOutline(
        // Tool used to draw images and shapes.
        SpriteBatch spriteBatch,
        // Small image used to draw the border lines.
        Texture2D pixel,
        // Position and size of the rectangle.
        Rectangle bounds,
        // Color used for the border.
        Color color)
    {
        // Draw the top edge of the border.
        spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Y, bounds.Width, 1), color);
        // Draw the bottom edge of the border.
        spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Bottom - 1, bounds.Width, 1), color);
        // Draw the left edge of the border.
        spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Y, 1, bounds.Height), color);
        // Draw the right edge of the border.
        spriteBatch.Draw(pixel, new Rectangle(bounds.Right - 1, bounds.Y, 1, bounds.Height), color);
    }

    // Draws text at a chosen position, color, and size.
    public static void DrawText(
        // Tool used to draw text.
        SpriteBatch spriteBatch,
        // Font used for the text.
        SpriteFont font,
        // The text to draw.
        string text,
        // Horizontal position of the text.
        int x,
        // Vertical position of the text.
        int y,
        // Color of the text.
        Color color,
        // Size multiplier for the text.
        float scale)
    {
        // Draw the text without rotating or flipping it.
        spriteBatch.DrawString(font, text, new Vector2(x, y), color, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
    }

    // Draws a button and changes its colors when the mouse is over it.
    public static void DrawHoverButton(
        // Tool used to draw images and text.
        SpriteBatch spriteBatch,
        // Font used for the button label.
        SpriteFont font,
        // Small image used to draw the button.
        Texture2D pixel,
        // Position and size of the button.
        Rectangle bounds,
        // Text shown inside the button.
        string label,
        // Size multiplier for the label.
        float scale)
    {
        // Check whether the mouse pointer is over the button.
        bool hovered = bounds.Contains(Microsoft.Xna.Framework.Input.Mouse.GetState().Position);
        // Draw the button background, using a brighter color when hovered.
        spriteBatch.Draw(pixel, bounds, hovered ? new Color(75, 57, 20, 245) : new Color(24, 33, 29, 235));
        // Draw the button border, using yellow when hovered.
        DrawOutline(spriteBatch, pixel, bounds, hovered ? Color.Yellow : new Color(205, 215, 200));

        // Set the label size using the screen scale.
        float textScale = 0.48f * scale;
        // Measure the label at its chosen size.
        Vector2 labelSize = font.MeasureString(label) * textScale;
        // Center the label inside the button.
        Vector2 labelPosition = new(bounds.Center.X - labelSize.X / 2, bounds.Center.Y - labelSize.Y / 2);
        // Draw the button label.
        spriteBatch.DrawString(
            // Use this font for the label.
            font,
            // Draw the button's text.
            label,
            // Place the label at the center of the button.
            labelPosition,
            // Use yellow text when hovered and white text otherwise.
            hovered ? Color.Yellow : Color.White,
            // Do not rotate the text.
            0,
            // Start drawing from the text's top-left corner.
            Vector2.Zero,
            // Use the chosen text size.
            textScale,
            // Do not flip the text.
            SpriteEffects.None,
            // Draw at the default depth.
            0);
    }

    // Changes a whole number into Roman numeral text.
    public static string ToRomanNumeral(int number)
    {
        // List Roman numeral values from largest to smallest.
        (int Value, string Numeral)[] numerals =
        [
            (1000, "M"),
            (900, "CM"),
            (500, "D"),
            (400, "CD"),
            (100, "C"),
            (90, "XC"),
            (50, "L"),
            (40, "XL"),
            (10, "X"),
            (9, "IX"),
            (5, "V"),
            (4, "IV"),
            (1, "I")
        ];

        // Create an empty place to build the answer.
        StringBuilder result = new();
        // Check each Roman numeral value, starting with the largest.
        foreach ((int value, string numeral) in numerals)
        {
            // Keep adding this numeral while the number is at least its value.
            while (number >= value)
            {
                // Add this Roman numeral to the answer.
                result.Append(numeral);
                // Subtract its value from the number still to convert.
                number -= value;
            }
        }

        // Turn the built answer into a string and return it.
        return result.ToString();
    }
}
