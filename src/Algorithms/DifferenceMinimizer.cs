using System;
using System.Collections.Generic;
using System.Linq;
using MinDifferenceSolutions = System.Collections.Generic.List<(int diff, System.Collections.Generic.List<int> values)>;

namespace Algorithms;

public static class DifferenceMinimizer
{
    public static MinDifferenceSolutions FindMinDifferenceAfterModification(
        int[] ints, int k)
    {
        int max, maxIndex;
        var min = max = ints[0];
        var minIndex = maxIndex = 0;
        for (int i = 1; i < ints.Length; i++)
        {
            if (ints[i] < min)
            {
                (min, minIndex) = (ints[i], i);
            }
            if (ints[i] > max)
            {
                (max, maxIndex) = (ints[i], i);
            }
        }
        int diff = max - min;
        var solutions = new MinDifferenceSolutions();
        if (diff <= k)
        {
            solutions.Add((diff, values: ints.Select(x => x + k).ToList()));
            solutions.Add((diff, values: ints.Select(x => x - k).ToList()));
            return solutions;
        }

        if (minIndex != 0)
        {
            (ints[minIndex], ints[0]) = (ints[0], ints[minIndex]);
        }
        if (maxIndex != 1)
        {
            (ints[maxIndex], ints[1]) = (ints[1], ints[maxIndex]);
        }

        var currentSolution = (
            diff: int.MaxValue,
            values: new List<int>()
        );
        var choices = new Dictionary<int, int>();
        FindMinDifferenceAfterModificationRecursively(
            ints, k, 0,
            max, min,
            currentSolution,
            choices,
            solutions
        );

        return solutions;
    }

    public static void TestFindMinDifferenceAfterModification()
    {
        TestFindMinDifferenceAfterModification(9);
        //for (int k = 1; k < 8; k++)
        //{
        //    TestFindMinDifferenceAfterModification(k);
        //}
    }

    private static void TestFindMinDifferenceAfterModification( int k)
    {
        var ints = new[] { 1, 3, 2, 6, 7, 9 };
        var solutions = FindMinDifferenceAfterModification(ints, k);
        Console.WriteLine($"The tower heights are:");
        foreach (var i in ints)
        {
            Console.WriteLine(i);
        }
        Console.WriteLine();
        Console.WriteLine($"The {solutions.Count} solutions for the towers with k as {k} are as below:\r\n");
        int solutionIndex = 1;
        foreach (var (diff, values) in solutions)
        {
            Console.WriteLine($"The {solutionIndex++}th solution is:");
            Console.WriteLine($"The new difference is {diff}");
            values.PrintOut();
        }

        Console.WriteLine();
    }

    private static void FindMinDifferenceAfterModificationRecursively(
        int[] ints, int k, int i, 
        int min, int max,
        (int diff, List<int> values) currentSolution,
        Dictionary<int, int> choices,
        MinDifferenceSolutions solutions)
    {
        var values = currentSolution.values;
            
        if (i == ints.Length)
        {
            int diff = max - min;
            // Coming to an end
            if (diff < FindCurrentMinDiff())
            {
                solutions.Clear();
            }
            if (diff <= FindCurrentMinDiff())
            {
                solutions.Add((currentSolution.diff, values: values.ToList()));
            }
            return;
        }
        var isNewChoice = !choices.ContainsKey(ints[i]);
        if (!isNewChoice)
        {
            currentSolution.values.Add(choices[ints[i]]);
            FindMinDifferenceAfterModificationRecursively(
                ints,
                k,
                i + 1,
                min,
                max,
                currentSolution,
                choices,
                solutions
            );
            values.RemoveAt(values.Count - 1);
            return;
        }

        int newValue = ints[i] + k;
        if (i == 0)
        {
            min = max = newValue;
        }
        int newMin = min, newMax = max;
        int newDiff = max - min;

        if (newValue > max)
        {
            newMax = newValue;
            newDiff = newMax - min;
        }
        else if (newValue < min)
        {
            newMin = newValue;
            newDiff = max - newMin;
        }

        if (newDiff <= FindCurrentMinDiff())
        {
            currentSolution.diff = newDiff;
            values.Add(newValue);
            choices[ints[i]] = newValue;
            FindMinDifferenceAfterModificationRecursively(
                ints,
                k,
                i + 1,
                newMin,
                newMax,
                currentSolution,
                choices,
                solutions
            );
            values.RemoveAt(values.Count - 1);
            choices.Remove(ints[i]);
        }

        newValue = ints[i] - k;
            
        if (i == 0)
        {
            min = max = newValue;
        }

        newMin = min;
        newMax = max;

        if (newValue < min)
        {
            newMin = newValue;
            newDiff = max - newMin;
        }
        else if (newValue > max)
        {
            newMax = newValue;
            newDiff = newMax - min;
        }
        else
        {
            newDiff = max - min;
        }

        if (newDiff <= FindCurrentMinDiff())
        {
            currentSolution.diff = newDiff;
            values.Add(newValue);
            choices[ints[i]] = newValue;
                
            FindMinDifferenceAfterModificationRecursively(
                ints,
                k,
                i + 1,
                newMin,
                newMax,
                currentSolution,
                choices,
                solutions
            );
            values.RemoveAt(values.Count - 1);
            choices.Remove(ints[i]);
        }

        int FindCurrentMinDiff() => solutions.Any() ? solutions.First().diff : ints[1] - ints[0];
    }
}