 using System;
 using Microsoft.Xna.Framework;
 using Microsoft.Xna.Framework.Graphics;
 using Microsoft.Xna.Framework.Input;

namespace MyGame;

// Contains code for drawing the shop screen and its buttons.
internal static class ShopMenu
{
    // Sets the highest level allowed for weapon upgrades.
    public const int MaximumUpgradeLevel = 5;

    // Draws the shop screen using the game images, screen size, and player information.
    public static void Draw(
        // Tool used to draw images and text.
        SpriteBatch spriteBatch,
        // Font used for shop text.
        SpriteFont font,
        // Small image used to draw solid shapes.
        Texture2D pixel,
        // Background image behind the shop.
        Texture2D titleBackground,
        // Image for a weapon upgrade.
        Texture2D upgrade1Weapon,
        // Image for the shotgun.
        Texture2D shopWeapon,
        // Image shown for locked or unknown items.
        Texture2D unknownWeapon,
        // Rifle image revealed after wave five.
        Texture2D waveFiveWeapon,
        // The current wave number, used to reveal locked shop items.
        int currentWave,
        // Width of the game window.
        int screenWidth,
        // Height of the game window.
        int screenHeight,
        // Size multiplier for the shop.
        float scale,
        // Tells whether the shotgun has been purchased.
        bool shotgunPurchased,
        // The player's current amount of money.
        int money,
        // The current pistol upgrade level.
        int pistolUpgradeLevel,
        // The current shotgun upgrade level.
        int shotgunUpgradeLevel)
    {
        // Draw the background across the whole window.
        spriteBatch.Draw(titleBackground, new Rectangle(0, 0, screenWidth, screenHeight), Color.White);
        // Add a see-through dark layer over the background.
        spriteBatch.Draw(pixel, new Rectangle(0, 0, screenWidth, screenHeight), new Color(0, 0, 0, 145));

        // Find the shop panel's position and size.
        Rectangle panel = GetPanelBounds(screenWidth, screenHeight);
        // Draw the panel background.
        UIHelper.DrawPanel(spriteBatch, pixel, panel, new Color(90, 90, 90, 235));

        // Set the space between the panel edge and its contents.
        int padding = Math.Max(8, (int)(18 * scale));
        // Set the space between the two columns.
        int gap = Math.Max(6, (int)(12 * scale));
        // Calculate the width of each column.
        int columnWidth = (panel.Width - padding * 2 - gap) / 2;
        // Set the header height, with a minimum size.
        int headerHeight = Math.Max(28, (int)(54 * scale));
        // Set the height of the area that shows the money balance.
        int balanceHeight = Math.Max(20, (int)(28 * scale));
        // Find the left edge of the first column.
        int leftX = panel.X + padding;
        // Find the left edge of the second column.
        int rightX = leftX + columnWidth + gap;
        // Draw the player's money near the top of the panel.
        UIHelper.DrawText(
            // Pass the drawing tool.
            spriteBatch,
            // Pass the shop font.
            font,
            // Show the money with commas for large numbers.
            $"MONEY   ${money:N0}",
            // Place the text at the left side.
            leftX,
            // Place the text near the top of the panel.
            panel.Y + padding / 2,
            // Use a gold color.
            new Color(255, 220, 75),
            // Set the text size.
            0.45f * scale);
        // Find the top edge of the shop contents.
        int contentTop = panel.Y + padding + balanceHeight;
        // Create the rectangle for the Shop heading.
        Rectangle shopHeader = new(leftX, contentTop, columnWidth, headerHeight);
        // Create the rectangle for the Upgrades heading.
        Rectangle upgradeHeader = new(rightX, contentTop, columnWidth, headerHeight);
        // Draw the Shop heading.
        DrawCard(spriteBatch, font, pixel, shopHeader, "SHOP", scale, true);
        // Draw the Upgrades heading.
        DrawCard(spriteBatch, font, pixel, upgradeHeader, "UPGRADES", scale, true);
        // Find the area that can be clicked to buy the shotgun.
        Rectangle shotgunPurchaseBounds = GetShotgunPurchaseBounds(screenWidth, screenHeight);
        // Check whether the shotgun is not owned and the mouse is over its purchase area.
        bool shotgunHovered = !shotgunPurchased &&
            shotgunPurchaseBounds.Contains(Mouse.GetState().Position);
        // Draw a vertical line between the two columns.
        spriteBatch.Draw(
            // Use the small image to draw the line.
            pixel,
            // Set the line's position, width, and height.
            new Rectangle(
                leftX + columnWidth + gap / 2,
                contentTop,
                Math.Max(1, (int)(2 * scale)),
                panel.Bottom - padding - contentTop),
            // Use a light gray color.
            new Color(205, 215, 200));

        // Set the space between item rows.
        int rowGap = Math.Max(6, (int)(10 * scale));
        // Calculate each row's height.
        int rowHeight = (panel.Height - padding * 2 - balanceHeight - headerHeight - rowGap * 3) / 3;
        // Choose a size for item pictures that fits in each row.
        int slotSize = Math.Min(rowHeight - (int)(8 * scale), (int)(96 * scale));
        // Draw the three item rows.
        for (int i = 0; i < 3; i++)
        {
            // Find the top edge of this row.
            int rowY = contentTop + headerHeight + rowGap + i * (rowHeight + rowGap);
            // Set the height of the text area beside the item picture.
            int infoHeight = Math.Max(1, (int)(slotSize * 0.4f));
            // Center the text area vertically beside the item picture.
            int infoY = rowY + (slotSize - infoHeight) / 2 + (int)(slotSize * 0.12f);
            // Create the rectangle for the item picture.
            Rectangle itemSlot = new(leftX, rowY, slotSize, slotSize);
            // Get the rectangle for this item's text.
            Rectangle itemInfo = GetInfoBounds(screenWidth, screenHeight, i, false);
            // Draw the item picture's background box without text.
            DrawCard(spriteBatch, font, pixel, itemSlot, string.Empty, scale, false, true);
            // Set the space between the picture and its border.
            int shopIconPadding = Math.Max(2, (int)(5 * scale));
            // Create a slightly smaller rectangle for the item image.
            Rectangle shopIconBounds = new(
                itemSlot.X + shopIconPadding,
                itemSlot.Y + shopIconPadding,
                itemSlot.Width - shopIconPadding * 2,
                itemSlot.Height - shopIconPadding * 2);
            Texture2D itemTexture = i switch
            {
                0 => shopWeapon,
                1 when currentWave >= 6 => waveFiveWeapon,
                _ => unknownWeapon
            };
            if (i == 1 && currentWave >= 6)
            {
                float weaponScale = Math.Min(
                    shopIconBounds.Width / (float)itemTexture.Width,
                    shopIconBounds.Height / (float)itemTexture.Height) * 1.1f;
                int weaponWidth = (int)(itemTexture.Width * weaponScale);
                int weaponHeight = (int)(itemTexture.Height * weaponScale);
                Rectangle weaponBounds = new(
                    shopIconBounds.Center.X - weaponWidth / 2,
                    shopIconBounds.Center.Y - weaponHeight / 2,
                    weaponWidth,
                    weaponHeight);
                spriteBatch.Draw(itemTexture, weaponBounds, Color.White);
            }
            else
            {
                // Draw the shotgun picture for the first item, or the unknown-item picture for locked items.
                spriteBatch.Draw(itemTexture, shopIconBounds, Color.White);
            }

            // Draw the text and optional background for this item.
            DrawCard(
                // Pass the drawing tool.
                spriteBatch,
                // Pass the shop font.
                font,
                // Pass the image used for boxes.
                pixel,
                // Set the text area position and size.
                itemInfo,
                // Choose the label based on the item and whether the shotgun was purchased.
                i switch
                {
                    // Show this message if the shotgun is already owned.
                    0 when shotgunPurchased => "Item Purchased",
                    // Show the price for an unpurchased shotgun.
                    0 => "Buy : $5000",
                    // Show the AR M4 price once it is unlocked.
                    1 when currentWave >= 6 => "Buy : $10,000",
                    // Show that the second item is not available yet.
                    1 => "will be unlock after wave 5",
                    // Show that the third item is not available yet.
                    2 => "will be unlock after wave 10",
                    // Use a default label for any other item.
                    _ => $"ITEM {i + 1}"
                },
                // Pass the shop text size.
                scale,
                // These item rows are not headers.
                false,
                // Draw boxes around the shotgun and unlocked AR M4 purchase options.
                drawBox: i == 0 || i == 1 && currentWave >= 6,
                // Use black text for items that are still locked.
                blackText: i > 0 && !(i == 1 && currentWave >= 6),
                // Use yellow text for the shotgun and unlocked AR M4 prices.
                yellowText: i == 0 || i == 1 && currentWave >= 6,
                // Use green for the box when the shotgun was purchased.
                boxColor: i == 0 && shotgunPurchased ? new Color(144, 238, 144) : null,
                // Highlight the shotgun item when the mouse is over it.
                isHovered: i == 0 && shotgunHovered);

            // Draw the weapon name above its item information.
            if (i == 0 || i == 1 && currentWave >= 6)
            {
                string weaponLabel = i == 0 ? "Shotgun" : "AR M4";
                // Set the name's text size.
                float weaponLabelScale = 0.43f * scale;
                // Measure the name at the chosen size.
                Vector2 weaponLabelSize = font.MeasureString(weaponLabel) * weaponLabelScale;
                // Center the name above the item information, with AR M4 slightly higher.
                float labelOffset = i == 1 ? 6 * scale : 2 * scale;
                Vector2 weaponLabelPosition = new(
                    itemInfo.Center.X - weaponLabelSize.X / 2,
                    itemInfo.Y - weaponLabelSize.Y - labelOffset);
                // Draw the weapon name in red.
                spriteBatch.DrawString(
                    // Use the shop font.
                    font,
                    // Draw the weapon name.
                    weaponLabel,
                    // Use the centered position calculated above.
                    weaponLabelPosition,
                    // Use red text.
                    Color.Red,
                    // Do not rotate the text.
                    0,
                    // Start drawing from the text's top-left corner.
                    Vector2.Zero,
                    // Use the chosen name size.
                    weaponLabelScale,
                    // Do not flip the text.
                    SpriteEffects.None,
                    // Draw at the default depth.
                    0);
            }

            // Create the picture slot for this upgrade.
            Rectangle upgradeSlot = new(rightX, rowY, slotSize, slotSize);
            // Get the area used for the upgrade's text.
            Rectangle upgradeInfo = GetInfoBounds(screenWidth, screenHeight, i, true);
            // Show the pistol upgrade and show the shotgun upgrade only after it is purchased.
            bool drawUpgrade = i == 0 || (i == 1 && shotgunPurchased);
            // Draw upgrade details only when this upgrade is available to show.
            if (drawUpgrade)
            {
                // Draw the upgrade picture's background box without text.
                DrawCard(spriteBatch, font, pixel, upgradeSlot, string.Empty, scale, false, true);
                // Set the space between the picture and its border.
                int iconPadding = Math.Max(2, (int)(5 * scale));
                // Create a slightly smaller rectangle for the upgrade image.
                Rectangle iconBounds = new(
                    upgradeSlot.X + iconPadding,
                    upgradeSlot.Y + iconPadding,
                    upgradeSlot.Width - iconPadding * 2,
                    upgradeSlot.Height - iconPadding * 2);
                // Draw the pistol upgrade image or the shotgun image.
                spriteBatch.Draw(i == 0 ? upgrade1Weapon : shopWeapon, iconBounds, Color.White);
                // Choose the upgrade level for the pistol or shotgun.
                int upgradeLevel = i == 0 ? pistolUpgradeLevel : shotgunUpgradeLevel;
                // Choose the maximum level for the selected weapon.
                int maximumUpgradeLevel = MaximumUpgradeLevel;
                // Set the price of this upgrade.
                int upgradePrice = i == 0 ? 1000 : 2000;
                // Get the clickable area for this weapon's upgrade.
                Rectangle upgradeBounds = i == 0
                    ? GetPistolUpgradeBounds(screenWidth, screenHeight)
                    : GetShotgunUpgradeBounds(screenWidth, screenHeight);
                // Draw the upgrade price or show that the weapon is at its maximum level.
                DrawCard(
                    // Pass the drawing tool.
                    spriteBatch,
                    // Pass the shop font.
                    font,
                    // Pass the image used for boxes.
                    pixel,
                    // Set the text area position and size.
                    upgradeInfo,
                    // Show MAX LEVEL or the upgrade price.
                    upgradeLevel >= maximumUpgradeLevel
                        ? "MAX LEVEL"
                        : $"Upgrade : ${upgradePrice}",
                    // Pass the shop text size.
                    scale,
                    // This is not a header.
                    false,
                    // Use yellow text for upgrade information.
                    yellowText: true,
                    // Highlight the upgrade if it can be bought and the mouse is over it.
                    isHovered: upgradeLevel < maximumUpgradeLevel &&
                        upgradeBounds.Contains(Mouse.GetState().Position));

                // Choose the name of the weapon being upgraded.
                string weaponLabel = i == 0 ? "Pistol" : "Shotgun";
                // Create text showing the current and maximum upgrade levels.
                string weaponLevel = $"Lvl {upgradeLevel}./Lvl {maximumUpgradeLevel}.";
                // Set the weapon name's text size.
                float weaponScale = 0.43f * scale;
                // Set the level text size.
                float levelScale = 0.3f * scale;
                // Measure the weapon name at its chosen size.
                Vector2 weaponSize = font.MeasureString(weaponLabel) * weaponScale;
                // Measure the level text at its chosen size.
                Vector2 levelSize = font.MeasureString(weaponLevel) * levelScale;
                // Set the space between the name and level.
                float labelGap = 4 * scale;
                // Calculate the total width of both labels and the gap.
                float labelsWidth = weaponSize.X + labelGap + levelSize.X;
                // Find the left edge that centers both labels above the information area.
                float labelsX = upgradeInfo.Center.X - labelsWidth / 2;
                // Find the vertical position above the information area.
                float labelsY = upgradeInfo.Y - weaponSize.Y - 2 * scale;
                // Draw the weapon name in red.
                spriteBatch.DrawString(
                    // Use the shop font.
                    font,
                    // Draw the weapon name.
                    weaponLabel,
                    // Place it at the position calculated above.
                    new Vector2(labelsX, labelsY),
                    // Use red text.
                    Color.Red,
                    // Do not rotate the text.
                    0,
                    // Start drawing from the text's top-left corner.
                    Vector2.Zero,
                    // Use the chosen weapon name size.
                    weaponScale,
                    // Do not flip the text.
                    SpriteEffects.None,
                    // Draw at the default depth.
                    0);
                // Draw the current and maximum levels beside the weapon name.
                spriteBatch.DrawString(
                    font,
                    weaponLevel,
                    new Vector2(labelsX + weaponSize.X + labelGap, labelsY + (weaponSize.Y - levelSize.Y) / 2),
                    Color.Red,
                    0,
                    Vector2.Zero,
                    levelScale,
                    SpriteEffects.None,
                    0);
            }
        }

        Rectangle nextWaveBounds = GetNextWaveBounds(screenWidth, screenHeight);
        bool nextWaveHovered = nextWaveBounds.Contains(Mouse.GetState().Position);
        spriteBatch.Draw(
            pixel,
            nextWaveBounds,
            nextWaveHovered ? new Color(120, 100, 25, 235) : new Color(75, 75, 75, 235));
        UIHelper.DrawOutline(
            spriteBatch,
            pixel,
            nextWaveBounds,
            nextWaveHovered ? Color.Yellow : new Color(205, 215, 200));

        string nextWaveLabel = "Next Wave";
        float nextWaveTextScale = 0.45f * scale;
        Vector2 nextWaveTextSize = font.MeasureString(nextWaveLabel) * nextWaveTextScale;
        Vector2 nextWaveTextPosition = new(
            nextWaveBounds.Center.X - nextWaveTextSize.X / 2,
            nextWaveBounds.Center.Y - nextWaveTextSize.Y / 2);
        spriteBatch.DrawString(
            font,
            nextWaveLabel,
            nextWaveTextPosition,
            Color.Red,
            0,
            Vector2.Zero,
            nextWaveTextScale,
            SpriteEffects.None,
            0);
    }

    public static Rectangle GetNextWaveBounds(int screenWidth, int screenHeight)
    {
        float scale = UIHelper.GetScale(screenWidth, screenHeight);
        Rectangle panel = GetPanelBounds(screenWidth, screenHeight);
        int padding = Math.Max(8, (int)(18 * scale));
        int width = Math.Max(1, (int)(190 * scale));
        int height = Math.Max(30, (int)(42 * scale));

        return new Rectangle(
            panel.Center.X - width / 2,
            Math.Min(panel.Bottom + padding, screenHeight - height - padding),
            width,
            height);
    }

    // Returns the position and size of the Shop button.
    public static Rectangle GetShopButtonBounds(int screenWidth, int screenHeight)
    {
        // Get a size multiplier that fits the current window.
        float scale = UIHelper.GetScale(screenWidth, screenHeight);
        // Set the button width.
        int width = (int)(112 * scale);
        // Set the button height.
        int height = (int)(48 * scale);
        // Set the space between the button and the screen edges.
        int margin = (int)(22 * scale);
        // Put the button in the bottom-right corner and return its rectangle.
        return new Rectangle(screenWidth - width - margin, screenHeight - height - margin, width, height);
    }

    // Returns the clickable area for buying the shotgun.
    public static Rectangle GetShotgunPurchaseBounds(int screenWidth, int screenHeight)
    {
        // The shotgun purchase information area is also its clickable area.
        return GetInfoBounds(screenWidth, screenHeight, 0, false);
    }

    // Returns the clickable area for upgrading the pistol.
    public static Rectangle GetPistolUpgradeBounds(int screenWidth, int screenHeight) =>
        // Use the first row in the upgrades column.
        GetInfoBounds(screenWidth, screenHeight, 0, true);

    // Returns the clickable area for upgrading the shotgun.
    public static Rectangle GetShotgunUpgradeBounds(int screenWidth, int screenHeight) =>
        // Use the second row in the upgrades column.
        GetInfoBounds(screenWidth, screenHeight, 1, true);

    // Calculates where an item's information and clickable area should appear.
    private static Rectangle GetInfoBounds(int screenWidth, int screenHeight, int index, bool upgrades)
    {
        // Get a size multiplier that fits the current window.
        float scale = UIHelper.GetScale(screenWidth, screenHeight);
        // Get the shop panel's position and size.
        Rectangle panel = GetPanelBounds(screenWidth, screenHeight);
        // Set the space between the panel edge and its contents.
        int padding = Math.Max(8, (int)(18 * scale));
        // Set the space between the two columns.
        int gap = Math.Max(6, (int)(12 * scale));
        // Calculate the width of each column.
        int columnWidth = (panel.Width - padding * 2 - gap) / 2;
        // Set the height of the column headings.
        int headerHeight = Math.Max(28, (int)(54 * scale));
        // Set the height of the area that shows the money balance.
        int balanceHeight = Math.Max(20, (int)(28 * scale));
        // Set the space between item rows.
        int rowGap = Math.Max(6, (int)(10 * scale));
        // Calculate each item row's height.
        int rowHeight = (panel.Height - padding * 2 - balanceHeight - headerHeight - rowGap * 3) / 3;
        // Choose an image slot size that fits inside a row.
        int slotSize = Math.Min(rowHeight - (int)(8 * scale), (int)(96 * scale));
        // Find the top edge of the shop contents.
        int contentTop = panel.Y + padding + balanceHeight;
        // Find the top edge of the selected item row.
        int rowY = contentTop + headerHeight + rowGap + index * (rowHeight + rowGap);
        // Set the height of the item's information area.
        int infoHeight = Math.Max(1, (int)(slotSize * 0.4f));
        // Center the information area vertically beside the image.
        int infoY = rowY + (slotSize - infoHeight) / 2 + (int)(slotSize * 0.12f);
        // Use the right column for upgrades and the left column for shop items.
        int columnX = upgrades
            // This position is used for upgrade information.
            ? panel.X + padding + columnWidth + gap
            // This position is used for shop item information.
            : panel.X + padding;
        // Return the information area's position and size.
        return new Rectangle(
            // Place it beside the image slot.
            columnX + slotSize + gap / 2,
            // Use the vertical position calculated above.
            infoY,
            // Use the remaining width in the column.
            Math.Max(1, columnWidth - slotSize - gap / 2),
            // Use the calculated information height.
            infoHeight);
    }

    // Draws a card, its optional background, and its optional label.
    private static void DrawCard(
        // Tool used to draw images and text.
        SpriteBatch spriteBatch,
        // Font used for card text.
        SpriteFont font,
        // Small image used to draw solid shapes.
        Texture2D pixel,
        // Position and size of the card.
        Rectangle bounds,
        // Text shown inside the card.
        string label,
        // Size multiplier for the card.
        float scale,
        // Tells whether this card is a heading.
        bool isHeader,
        // Tells whether this card is an image slot.
        bool isWeaponSlot = false,
        // Tells whether to draw the card's background and border.
        bool drawBox = true,
        // Tells whether the label should be black.
        bool blackText = false,
        // Tells whether the label should be yellow.
        bool yellowText = false,
        // Optional background color for the card.
        Color? boxColor = null,
        // Tells whether the mouse is over the card.
        bool isHovered = false)
    {
        // Draw the card background and border when requested.
        if (drawBox)
        {
            // Draw the background using a color chosen from the card state.
            spriteBatch.Draw(
                // Use the small image to draw the background.
                pixel,
                // Use this card's position and size.
                bounds,
                // Use a hover color, a provided color, or a default color.
                isHovered
                    ? new Color(120, 100, 25, 235)
                    : boxColor ?? (isWeaponSlot
                        ? Color.White
                        : new Color(75, 75, 75, 235)));
            // Draw a border around the card.
            UIHelper.DrawOutline(
                // Pass the drawing tool.
                spriteBatch,
                // Pass the image used for the border.
                pixel,
                // Use this card's position and size.
                bounds,
                // Make the border yellow when the mouse is over the card.
                isHovered ? Color.Yellow : new Color(205, 215, 200));
        }

        // Do not draw text if the label is empty.
        if (label.Length == 0)
        {
            return;
        }

        // Choose a larger text size for headings and a smaller size for other cards.
        float textScale = (isHeader ? 0.52f : 0.36f) * scale;
        // Measure the label at its chosen size.
        Vector2 textSize = font.MeasureString(label) * textScale;
        // Center the label inside the card.
        Vector2 position = new(bounds.Center.X - textSize.X / 2, bounds.Center.Y - textSize.Y / 2);
        // Draw the card label.
        spriteBatch.DrawString(
            // Use the card font.
            font,
            // Draw this card's label.
            label,
            // Use the centered position.
            position,
            // Choose yellow, black, or red text.
            yellowText ? Color.Yellow : blackText ? Color.Black : Color.Red,
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

    // Calculates a panel that fits in the center of the game window.
    private static Rectangle GetPanelBounds(int screenWidth, int screenHeight)
    {
        // Get a size multiplier that fits the current window.
        float scale = UIHelper.GetScale(screenWidth, screenHeight);
        // Set the panel width but leave space at the edges of the screen.
        int width = Math.Min(screenWidth - 32, (int)(860 * scale));
        // Set the panel height but leave space at the top and bottom.
        int height = Math.Min(screenHeight - 32, (int)(500 * scale));
        // Return a rectangle centered in the game window.
        return new Rectangle((screenWidth - width) / 2, (screenHeight - height) / 2, width, height);
    }
}
