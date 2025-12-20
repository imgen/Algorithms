using System;

namespace Algorithms;

public static class Sorting
{
    public static int QuickSelect(int[] ints, int k)
    {
        return QuickSelect(ints, 0, ints.Length - 1, k);
    }

    private static int QuickSelect(int[] ints, int start, int end, int k)
    {
        if (start >= end)
        {
            return ints[start];
        }

        int pivot = ints[start];
        var (i, j) = (start + 1, end);
        while (i <= j)
        {
            while (i <= end && ints[i] <= pivot)
            {
                i++;
            }
            while (j > start && ints[j] > pivot)
            {
                j--;
            }
            if (i < j && ints[i] > ints[j])
            {
                (ints[i], ints[j]) = (ints[j], ints[i]);
            }
        }
        if (k == (i - start))
        {
            return pivot;
        }
        else if (k < (i - start))
        {
            return QuickSelect(ints, start + 1, i - 1, k);
        }
        else
        {
            return QuickSelect(ints, i, end, k - i);
        }
    }

    private static void QuickSort(int[] ints)
    {
        QuickSort(ints, 0, ints.Length - 1);
    }

    private static void QuickSort(int[] ints, int start, int end)
    {
        if (start >= end)
        {
            return;
        }

        int pivot = ints[start];
        var (i, j) = (start + 1, end);
        while (i <= j)
        {
            while (i <= end && ints[i] <= pivot)
            {
                i++;
            }
            while (j > start && ints[j] > pivot)
            {
                j--;
            }
            if (i < j && ints[i] > ints[j])
            {
                (ints[i], ints[j]) = (ints[j], ints[i]);
            }
        }
        if (i - 1 != start && ints[i-1] != ints[start])
        {
            (ints[i - 1], ints[start]) = (ints[start], ints[i - 1]);
        }
        QuickSort(ints, start, i-2);
        QuickSort(ints, i, end);
    }

    public static void MergeSort(int[] ints)
    {
        MergeSort(ints, 0, ints.Length - 1);
    }

    private static void MergeSort(int[] ints, int start, int end)
    {
        if (start >= end)
        {
            return;
        }

        int middle = (start + end) / 2;
        MergeSort(ints, start, middle);
        MergeSort(ints, middle + 1, end);

        var (i, j) = (middle, middle + 1);
        while (ints[i] > ints[j])
        {
            (ints[i], ints[j]) = (ints[j], ints[i]);
            // The invariant is that both parts of the array are sorted
            // after we swap ints[i] and ints[j], the invariant might be 
            // violated, so we need to do work to maintain that invariant
            while(i > 0 && ints[i] < ints[i -1])
            {
                (ints[i], ints[i - 1]) = (ints[i - 1], ints[i]);
                i--;
            }

            while (j < end && ints[j] < ints[j + 1])
            {
                (ints[j], ints[j + 1]) = (ints[j + 1], ints[j]);
                j++;
            }

            (i, j) = (middle, middle + 1);
        }
    }

    public static void TestMergeSort()
    {
        var ints = new[] { 5, 2, 1, 7, 3, 2, 6 };
        MergeSort(ints);
        ints.PrintOut();
    }

    public static void TestQuickSort()
    {
        var ints = new[] { 7, 6 };
        QuickSort(ints);
        ints.PrintOut();
        ints = new[] { 5, 2, 1, 7, 3, 2, 6 };
        QuickSort(ints);
        ints.PrintOut();

        QuickSort(ints);
        ints.PrintOut();
    }

    public static void TestQuickSelect()
    {
        var ints = new[] { 6, 7 };
        int k = 1;
        var kthSmallestNumber = QuickSelect(ints, k);
        Console.WriteLine($"The {k}th smallest number is {kthSmallestNumber}");
        ints = new[] { 5, 2, 1, 7, 3, 2, 6 };
        k = 6;
        kthSmallestNumber = QuickSelect(ints, k);
        Console.WriteLine($"The {k}th smallest number is {kthSmallestNumber}");
    }
}