 using System;
 using Microsoft.Xna.Framework;
 using Microsoft.Xna.Framework.Graphics;

 namespace MyGame.UI;

// Defines a bar that can store values and draw itself.
public sealed class Bar
{
    // Stores the current amount; only this class can change it directly.
    public float Current { get; private set; }

    // Stores the largest amount this bar can hold; only this class can change it directly.
    public float Maximum { get; private set; }

    // Calculates how full the bar is: 0 means empty and 1 means full.
    public float Ratio => Current / Maximum;

    // Runs when a new Bar is created and receives its starting amount and limit.
    public Bar(float current, float maximum)
    {
        // Use Set to check the limit and save both starting values.
        Set(current, maximum);
    }

    // Changes the bar's current amount and maximum amount.
    public void Set(float current, float maximum)
    {
        // Check that the maximum is a valid positive number.
        if (maximum <= 0)
        {
            // Stop and report an error if the maximum is zero or less.
            throw new ArgumentOutOfRangeException(nameof(maximum), "Maximum value must be greater than zero.");
        }

        // Save the new maximum amount.
        Maximum = maximum;
        // Save the current amount, keeping it between zero and the maximum.
        Current = Math.Clamp(current, 0, maximum);
    }

    // Adds to or subtracts from the current amount.
    public void Change(float amount)
    {
        // Keep the result between zero and the maximum amount.
        Current = Math.Clamp(Current + amount, 0, Maximum);
    }

    // Draws the bar and the text or icon that goes with it.
    public void Draw(
        // The tool used to draw images and text.
        SpriteBatch spriteBatch,
        // The font used for the label.
        SpriteFont font,
        // The font used for the current and maximum values.
        SpriteFont valueFont,
        // A small image used to draw the bar's background and fill.
        Texture2D pixel,
        // The bar's position and size on the screen.
        Rectangle bounds,
        // The text shown beside the bar, unless an icon is used.
        string label,
        // The color used for the label and filled part.
        Color fillColor,
        // The size multiplier for the label text.
        float fontScale,
        // The size multiplier for the value text.
        float valueFontScale,
        // An optional image to show instead of the label text.
        Texture2D labelIcon = null)
    {
        // Measure the label and adjust its size using fontScale.
        Vector2 labelSize = font.MeasureString(label) * fontScale;
        // Make text showing the rounded current amount and maximum amount.
        string value = $"{MathF.Round(Current)} / {MathF.Round(Maximum)}";
        // Measure the values text and adjust its size using valueFontScale.
        Vector2 valueSize = valueFont.MeasureString(value) * valueFontScale;

        // Use zero height if there is no icon; otherwise make the icon taller than the label.
        int iconHeight = labelIcon is null
            // This value is used when labelIcon is null.
            ? 0
            // This value is used when an icon exists; Math.Max keeps its height at least 1.
            : Math.Max(1, (int)MathF.Ceiling(labelSize.Y * 1.5f));

        // Find the vertical position above the bar, leaving 4 pixels of space.
        int textY = bounds.Y - (int)MathF.Ceiling(Math.Max(labelIcon is null ? labelSize.Y : iconHeight, valueSize.Y)) - 4;

        // Check whether an icon was provided.
        if (labelIcon is null)
        {
            // If there is no icon, draw the label text on the left.
            spriteBatch.DrawString(font, label, new Vector2(bounds.X, textY), fillColor, 0, Vector2.Zero, fontScale, SpriteEffects.None, 0);
        }
        else
        {
            // Calculate an icon width that keeps its original shape.
            int iconWidth = Math.Max(1, (int)MathF.Ceiling(iconHeight * (float)labelIcon.Width / labelIcon.Height));
            // Draw the icon on the left in its normal colors.
            spriteBatch.Draw(labelIcon, new Rectangle(bounds.X, textY, iconWidth, iconHeight), Color.White);
        }

        // Draw the current and maximum values using several drawing settings.
        spriteBatch.DrawString(
            // Use the font chosen for the values.
            valueFont,
            // Draw the value text created above.
            value,
            // Place the text at the right edge of the bar.
            new Vector2(bounds.Right - valueSize.X, textY),
            // Use a dark gray text color.
            new Color(55, 55, 55),
            // Do not rotate the text.
            0,
            // Use the top-left corner as the text's origin.
            Vector2.Zero,
            // Use the selected value text size.
            valueFontScale,
            // Do not flip the text.
            SpriteEffects.None,
            // Draw at the default depth.
            0);

        // Draw the full bar rectangle as a white background.
        spriteBatch.Draw(pixel, bounds, Color.White);
        // Calculate how many pixels wide the filled part should be.
        int fillWidth = (int)(bounds.Width * Math.Clamp(Ratio, 0, 1));
        // Only draw the filled part if it has a width greater than zero.
        if (fillWidth > 0)
        {
            // Draw the filled part from the left side of the bar.
            spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Y, fillWidth, bounds.Height), fillColor);
        }

        // Draw a dark border around the bar.
        DrawOutline(spriteBatch, pixel, bounds, new Color(65, 65, 65));
    }

    // Draws the four thin lines that form the bar's border.
    private static void DrawOutline(SpriteBatch spriteBatch, Texture2D pixel, Rectangle bounds, Color color)
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
}
