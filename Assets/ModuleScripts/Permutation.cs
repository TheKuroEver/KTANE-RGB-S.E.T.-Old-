using System.Collections;
using System.Collections.Generic;
using System.Linq;
using rnd = UnityEngine.Random;

public static class PermsManager
{
    private static int[] permutation;
    private static List<Cycle> cycles;
    public static List<Cycle> Cycles { get { return cycles; } }

    public static void GenerateRandomPermutationSequence()
    {
        cycles = new List<Cycle>();
        permutation = GetRandomPermutation();

        SeperateIntoDisjointCycles();
        SplitIntoSmallerCycles();
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

    private static void SplitIntoSmallerCycles()
    {
        var newCycles = new List<Cycle>();

        for (int i = 0; i < cycles.Count; i++)
        {
            while (cycles[i].Order > 3)
            {
                newCycles.Add(new Cycle(cycles[i].Elements[0], cycles[i].Elements[1], cycles[i].Elements[2]));
                cycles[i] = new Cycle(cycles[i].Elements.Where((_, index) => index >= 2).ToArray());
            }

            newCycles.Add(cycles[i]);
        }

        cycles = newCycles;
    }
}

public class Cycle
{
    private int[] _elements;

    public int[] Elements { get { return _elements; } }
    public int Order { get { return _elements.Length; } }

    public Cycle(params int[] elements)
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
            counter++;
        }

        throw new System.InvalidOperationException("Element not in cycle.");
    }

    public bool Contains(int position)
    {
        return _elements.Contains(position);
    }
}
