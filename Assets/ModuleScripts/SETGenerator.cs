using System.Collections.Generic;
using rnd = UnityEngine.Random;

public static class SETGenerator
{
    private static List<string> availableValues;
    private static string[] correctAnswers = new string[3];

    public static string[] CorrectAnswers { get { return correctAnswers; } }

    public static string[] GenerateSetList(bool stageThree = false)
    {
        List<int> correctPositions = GetCorrectAnswersAndPositions(stageThree);
        int correctValueCounter = stageThree ? 1 : 0;
        int position;

        var newList = new List<string>();
        string newValue;

        for (int i = 0; i < 9; i++)
        {
            if (correctPositions.Contains(i))
            {
                newValue = correctAnswers[correctValueCounter];
                correctValueCounter++;
            }
            else
            {
                position = rnd.Range(0, availableValues.Count);
                newValue = availableValues[position];
                availableValues.RemoveAt(position);
            }
            RemoveAllSets(newValue, newList);
            newList.Add(newValue);
        }

        return newList.ToArray();
    }

    public static string[] GenerateSetList(string stageOneValues, string stageTwoValues)
    {
        correctAnswers[0] = FindMatchingSet(stageOneValues, stageTwoValues);

        return GenerateSetList(true);
    }

    private static void GenerateEveryPossibleValue()
    {
        availableValues = new List<string>();

        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                for (int k = 0; k < 3; k++)
                    for (int l = 0; l < 3; l++)
                    {
                        availableValues.Add(i.ToString() + j.ToString() + k.ToString() + l.ToString());
                    }
    }

    private static List<int> GetCorrectAnswersAndPositions(bool stageThree = false)
    {
        int position;
        var correctPositions = new List<int>() { rnd.Range(0, 9), rnd.Range(0, 9), rnd.Range(0, 9) };

        if (correctPositions[1] == correctPositions[0]) correctPositions[1] = (correctPositions[1] + 1) % 9;
        if (correctPositions[2] == correctPositions[0]) correctPositions[2] = (correctPositions[2] + 1) % 9;
        if (correctPositions[2] == correctPositions[1]) correctPositions[2] = (correctPositions[2] + 1) % 9;

        GenerateEveryPossibleValue();

        if (!stageThree)
        {
            position = rnd.Range(0, availableValues.Count);
            correctAnswers[0] = availableValues[position];
            availableValues.RemoveAt(position);
        }
        else
        {
            availableValues.Remove(correctAnswers[0]);
            correctPositions[0] = -1;
        }

        position = rnd.Range(0, availableValues.Count);
        correctAnswers[1] = availableValues[position];
        availableValues.RemoveAt(position);

        correctAnswers[2] = FindMatchingSet(correctAnswers[0], correctAnswers[1]);
        availableValues.Remove(correctAnswers[2]);

        return correctPositions;
    }

    public static string FindMatchingSet(string value1, string value2)
    {
        string value3 = "";

        if (value1.Length != value2.Length)
        {
            throw new System.InvalidOperationException("Cannot find a set for values of different numbers of parameters. Values are " + value1 + " " + value2);
        }

        for (int i = 0; i < value1.Length; i++)
        {
            if (value1[i] == value2[i]) value3 += value1[i];
            else value3 += "012".Replace(value1[i].ToString(), string.Empty).Replace(value2[i].ToString(), string.Empty);
        }

        return value3;
    }

    private static void RemoveAllSets(string newValue, List<string> valuesSoFar)
    {
        foreach (string existingValue in valuesSoFar)
        {
            availableValues.Remove(FindMatchingSet(newValue, existingValue));
        }
    }

    public static bool FormASet(string[] values)
    {
        if (values.Length != 3) return false;

        for (int i = 0; i < 3; i++)
        {
            if ((values[i][0] + values[i][1] + values[i][2]) % 3 != 0) return false;
        }

        return true;
    }
}
