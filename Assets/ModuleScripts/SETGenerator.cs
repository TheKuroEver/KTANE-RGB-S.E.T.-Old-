using System.Collections.Generic;
using rnd = UnityEngine.Random;

public static class SETGenerator
{
    private static List<string> availableValues;
    private static string[] correctAnswers = new string[3];

    public static string[] CorrectAnswers { get { return correctAnswers; } }

    public static string[] GenerateSetList()
    {
        List<int> correctPositions = GetCorrectAnswersAndPositions();
        int correctValueCounter = 0;
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

    private static List<int> GetCorrectAnswersAndPositions()
    {
        int position;
        var correctPositions = new List<int>() { rnd.Range(0, 9), rnd.Range(0, 9), rnd.Range(0, 9) };

        if (correctPositions[1] == correctPositions[0]) correctPositions[1] = (correctPositions[1] + 1) % 9;
        if (correctPositions[2] == correctPositions[0]) correctPositions[2] = (correctPositions[2] + 1) % 9;
        if (correctPositions[2] == correctPositions[1]) correctPositions[2] = (correctPositions[2] + 1) % 9;

        GenerateEveryPossibleValue();

        position = rnd.Range(0, availableValues.Count);
        correctAnswers[0] = availableValues[position];
        availableValues.RemoveAt(position);

        position = rnd.Range(0, availableValues.Count);
        correctAnswers[1] = availableValues[position];
        availableValues.RemoveAt(position);

        correctAnswers[2] = FindMatchingSet(correctAnswers[0], correctAnswers[1]);
        availableValues.Remove(correctAnswers[2]);

        return correctPositions;
    }

    private static string FindMatchingSet(string value1, string value2)
    {
        string value3 = "";

        for (int i = 0; i < 4; i++)
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
}
