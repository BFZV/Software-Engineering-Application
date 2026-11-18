using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyGame;

internal static class UpgradeWeaponMenu
{
    private const int LevelCount = 5;

    public static void Draw(
        SpriteBatch spriteBatch,
        SpriteFont font,
        Texture2D pixel,
        int screenWidth,
        int screenHeight,
        bool shotgun,
        int currentFireRateLevel,
        int currentDamageLevel,
        int fireRateLevel,
        int damageLevel,
        int cost,
        int money,
        string status)
    {
        float scale = UIHelper.GetScale(screenWidth, screenHeight);
        Rectangle panel = GetPanelBounds(screenWidth, screenHeight);
        spriteBatch.Draw(pixel, new Rectangle(0, 0, screenWidth, screenHeight), new Color(0, 0, 0, 180));
        UIHelper.DrawPanel(spriteBatch, pixel, panel, new Color(75, 78, 84, 250));

        string title = "UPGRADE WEAPON";
        float titleScale = 0.78f * scale;
        Vector2 titleSize = font.MeasureString(title) * titleScale;
        spriteBatch.DrawString(
            font,
            title,
            new Vector2(panel.Center.X - titleSize.X / 2, panel.Y + 16 * scale),
            new Color(255, 75, 35),
            0,
            Vector2.Zero,
            titleScale,
            SpriteEffects.None,
            0);

        string weaponName = shotgun ? "SHOTGUN" : "PISTOL";
        UIHelper.DrawText(
            spriteBatch,
            font,
            weaponName,
            panel.X + (int)(24 * scale),
            panel.Y + (int)(84 * scale),
            Color.White,
            0.4f * scale);

        DrawUpgradeRow(
            spriteBatch,
            font,
            pixel,
            panel,
            0,
            "Fire Rate",
            "Shoot faster.",
            fireRateLevel,
            currentFireRateLevel,
            scale,
            screenWidth,
            screenHeight);
        DrawUpgradeRow(
            spriteBatch,
            font,
            pixel,
            panel,
            1,
            "Damage",
            "Deal more damage.",
            damageLevel,
            currentDamageLevel,
            scale,
            screenWidth,
            screenHeight);

        int footerY = panel.Bottom - (int)(82 * scale);
        float detailScale = 0.34f * scale;
        string details = $"COST  ${cost:N0}     MONEY  ${money:N0}";
        UIHelper.DrawText(
            spriteBatch,
            font,
            string.IsNullOrEmpty(status) ? details : status,
            panel.X + (int)(24 * scale),
            footerY - (int)(28 * scale),
            string.IsNullOrEmpty(status) ? new Color(255, 220, 75) : Color.Red,
            detailScale);

        DrawFooterButton(spriteBatch, font, pixel, GetUndoBounds(screenWidth, screenHeight), "UNDO", scale, false);
        DrawFooterButton(spriteBatch, font, pixel, GetSaveBounds(screenWidth, screenHeight), "SAVE", scale, money < cost);
    }

    public static Rectangle GetFireRateLevelBounds(int screenWidth, int screenHeight, int level) =>
        GetLevelBounds(screenWidth, screenHeight, 0, level);

    public static Rectangle GetDamageLevelBounds(int screenWidth, int screenHeight, int level) =>
        GetLevelBounds(screenWidth, screenHeight, 1, level);

    public static Rectangle GetUndoBounds(int screenWidth, int screenHeight) =>
        GetFooterBounds(screenWidth, screenHeight, false);

    public static Rectangle GetSaveBounds(int screenWidth, int screenHeight) =>
        GetFooterBounds(screenWidth, screenHeight, true);

    private static void DrawUpgradeRow(
        SpriteBatch spriteBatch,
        SpriteFont font,
        Texture2D pixel,
        Rectangle panel,
        int row,
        string label,
        string description,
        int selectedLevel,
        int currentLevel,
        float scale,
        int screenWidth,
        int screenHeight)
    {
        int padding = Math.Max(12, (int)(24 * scale));
        int rowY = panel.Y + (int)(126 * scale) + row * (int)(142 * scale);
        float labelScale = 0.56f * scale;
        float descriptionScale = 0.36f * scale;

        UIHelper.DrawText(
            spriteBatch,
            font,
            label,
            panel.X + padding,
            rowY + (int)(18 * scale),
            new Color(255, 75, 35),
            labelScale);
        UIHelper.DrawText(
            spriteBatch,
            font,
            description,
            panel.X + padding,
            rowY + (int)(65 * scale),
            Color.White,
            descriptionScale);

        for (int level = 1; level <= LevelCount; level++)
        {
            Rectangle bounds = GetLevelBounds(screenWidth, screenHeight, row, level);
            bool selected = level == selectedLevel;
            bool locked = level < currentLevel;
            Color fill = selected
                ? new Color(70, 67, 25, 255)
                : locked
                    ? new Color(45, 47, 51, 200)
                    : new Color(45, 48, 54, 245);
            Color outline = selected ? Color.Yellow : new Color(205, 215, 225);
            spriteBatch.Draw(pixel, bounds, fill);
            UIHelper.DrawOutline(spriteBatch, pixel, bounds, outline);

            string levelText = $"Lv. {level}";
            float textScale = 0.4f * scale;
            Vector2 textSize = font.MeasureString(levelText) * textScale;
            spriteBatch.DrawString(
                font,
                levelText,
                new Vector2(bounds.Center.X - textSize.X / 2, bounds.Center.Y - textSize.Y / 2),
                selected ? Color.Yellow : locked ? Color.Gray : Color.White,
                0,
                Vector2.Zero,
                textScale,
                SpriteEffects.None,
                0);
        }
    }

    private static void DrawFooterButton(
        SpriteBatch spriteBatch,
        SpriteFont font,
        Texture2D pixel,
        Rectangle bounds,
        string label,
        float scale,
        bool disabled)
    {
        spriteBatch.Draw(pixel, bounds, disabled ? new Color(55, 55, 55, 235) : new Color(45, 48, 54, 245));
        UIHelper.DrawOutline(spriteBatch, pixel, bounds, new Color(205, 215, 225));

        float textScale = 0.58f * scale;
        Vector2 textSize = font.MeasureString(label) * textScale;
        spriteBatch.DrawString(
            font,
            label,
            new Vector2(bounds.Center.X - textSize.X / 2, bounds.Center.Y - textSize.Y / 2),
            disabled ? Color.Gray : new Color(255, 75, 35),
            0,
            Vector2.Zero,
            textScale,
            SpriteEffects.None,
            0);
    }

    private static Rectangle GetLevelBounds(int screenWidth, int screenHeight, int row, int level)
    {
        float scale = UIHelper.GetScale(screenWidth, screenHeight);
        Rectangle panel = GetPanelBounds(screenWidth, screenHeight);
        int padding = Math.Max(12, (int)(24 * scale));
        int labelWidth = (int)(panel.Width * 0.34f);
        int levelsX = panel.X + padding + labelWidth;
        int right = panel.Right - padding;
        int gap = Math.Max(4, (int)(10 * scale));
        int levelWidth = (right - levelsX - gap * (LevelCount - 1)) / LevelCount;
        int levelHeight = Math.Max(44, (int)(74 * scale));
        int levelY = panel.Y + (int)(126 * scale) + row * (int)(142 * scale) + (int)(18 * scale);
        return new Rectangle(levelsX + (level - 1) * (levelWidth + gap), levelY, levelWidth, levelHeight);
    }

    private static Rectangle GetFooterBounds(int screenWidth, int screenHeight, bool save)
    {
        float scale = UIHelper.GetScale(screenWidth, screenHeight);
        Rectangle panel = GetPanelBounds(screenWidth, screenHeight);
        int padding = Math.Max(12, (int)(24 * scale));
        int gap = Math.Max(8, (int)(24 * scale));
        int width = (panel.Width - padding * 2 - gap) / 2;
        int height = Math.Max(46, (int)(62 * scale));
        int x = save ? panel.Center.X + gap / 2 : panel.X + padding;
        int y = panel.Bottom - (int)(82 * scale);
        return new Rectangle(x, y, width, height);
    }

    private static Rectangle GetPanelBounds(int screenWidth, int screenHeight)
    {
        float scale = UIHelper.GetScale(screenWidth, screenHeight);
        int width = Math.Min(screenWidth - 32, (int)(1200 * scale));
        int height = Math.Min(screenHeight - 32, (int)(600 * scale));
        return new Rectangle(
            (screenWidth - width) / 2,
            (screenHeight - height) / 2,
            width,
            height);
    }
}
