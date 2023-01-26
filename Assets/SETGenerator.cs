using System.Collections;
using System.Collections.Generic;
using rnd = UnityEngine.Random;

public static class SETGenerator
{
    private static List<SETState> possibleValues;

    public static SETState[] GenerateRandomSetValues()
    {
        var setList = new SETState[9];

        possibleValues = GetAllPossibleValues();

        for (int i = 0; i < 9; i++)
        {
            setList[i] = possibleValues[rnd.Range(0, possibleValues.Length)];
            possibleValues.Remove(setList[i]);
        }

        return setList;
    }

    private static List<SETState> GetAllPossibleValues()
    {
        var possibleValues = new List<SETState>();

        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                for (int k = 0; k < 3; k++)
                    for (int l = 0; l < 3; l++)
                    {
                        possibleValues.Add(new SETState(new int[] { i, j, k, l }));
                    }

        return possibleValues;
    }

    //private list<SETState>() FindSETCombinations(params SETState[] states)
    //{
    //    foreach (SETState state in states)
    //    {

    //    }
    //}

    public class SETState
    {
        public int[] Values;

        public SETState(int[] values)
        {
            Values = values;
        }

        public static bool operator ==(SETState state1, SETState state2)
        {
            for (int i = 0; i < 4; i++)
            {
                if (state1.Values[i] != state2.Values[i]) return false;
            }

            return true;
        }

        public static bool operator !=(SETState state1, SETState state2)
        {
            return !(state1 == state2);
        }
    }
}
