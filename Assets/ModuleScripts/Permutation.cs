using System.Collections;
using System.Collections.Generic;
using rnd = UnityEngine.Random;

public static class PermsManager
{
    private static int[] permutation;
    private static List<Cycle> cycles;

    public static string ayo = "";

    public static int[] Permutation { get { return permutation; } }
    public static List<Cycle> Cycles { get { return cycles; } }

    public static void GenerateRandomPermutationSequence()
    {
        cycles = new List<Cycle>();
        permutation = GetRandomPermutation();

        foreach (int i in permutation) ayo += i.ToString();

        SeperateIntoDisjointCycles();

    }

    private static void SeperateIntoDisjointCycles()
    {
        for (int i = 0; i < 9; i++)
        {
            if (permutation[i] != -1 && permutation[i] != i) cycles.Add(FindCycle(i));
        }
    }

    private static int[] GetRandomPermutation()
    {
        var startList = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
        var endList = new List<int>();
        int position;

        for (int i = 0; i < 9; i++)
        {
            position = rnd.Range(0, startList.Count);
            endList.Add(startList[position]);
            startList.RemoveAt(position);
        }

        return endList.ToArray();
    }

    private static Cycle FindCycle(int startPosition) // NEEDS A REWRITE
    {
        var tempCycles = new List<int>() { permutation[startPosition] };
        int lastAddedElement = permutation[startPosition];
        int nextElement;

        permutation[startPosition] = -1;

        while (permutation[lastAddedElement] != -1)
        {
            nextElement = permutation[lastAddedElement];
            tempCycles.Add(nextElement);
            permutation[lastAddedElement] = -1;
            lastAddedElement = nextElement;
        }

        return new Cycle(tempCycles.ToArray());
    }

    
}

public class Cycle
{
    public int[] _elements;

    public Cycle(int[] elements)
    {
        _elements = elements;
    }

    public int Permute(int number)
    {
        int counter = 0;
        foreach (int element in _elements)
        {
            if (element == number)
            {
                return _elements[(counter + 1) % _elements.Length];
            }
        }

        throw new System.InvalidOperationException("Element not in cycle.");
    }
}
