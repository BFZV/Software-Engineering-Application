using System;

namespace MyGame;

public class WaveManager
{
    public int CurrentWave { get; private set; } = 1;

    public int EnemiesInCurrentWave => 5 + (CurrentWave - 1) * 2;

    public void Reset()
    {
        CurrentWave = 1;
    }

    public void AdvanceWave()
    {
        CurrentWave++;
    }

    public string GetRomanNumeral()
    {
        return ToRoman(CurrentWave);
    }

    private static string ToRoman(int value)
    {
        if (value <= 0)
            return "I";

        string[] numerals =
        {
            "I", "IV", "V", "IX", "X", "XL", "L", "XC", "C", "CD", "D", "CM", "M"
        };

        int[] values = { 1, 4, 5, 9, 10, 40, 50, 90, 100, 400, 500, 900, 1000 };

        string result = string.Empty;

        for (int i = values.Length - 1; i >= 0; i--)
        {
            while (value >= values[i])
            {
                result += numerals[i];
                value -= values[i];
            }
        }

        return result;
    }
}
