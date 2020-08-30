using System;
using System.Collections.Generic;
using System.Linq;

namespace Algorithms
{
    static class LongestAsendingSequence
    {
        public static List<List<int>> FindLongestAscendingSequence(int[] a)
        {
            var solutions = new List<List<int>>();
            FindLongestAscendingSequence(a, 0, new List<int>(), solutions);
            return solutions;
        }

        public static List<List<int>> FindLongestAscendingSequenceDynamically(int[] a)
        {
            var solutionsWithCurrentElement = new List<List<int>>();
            var solutionsWithoutCurrentElement = new List<List<int>>();
            FindLongestAscendingSequenceDynamically(a, 
                a.Length - 1, 
                solutionsWithCurrentElement, 
                solutionsWithoutCurrentElement
            );
            var allSolutions = solutionsWithCurrentElement
                    .Concat(solutionsWithoutCurrentElement)
                    .ToList();

            var maxSolutionCount = allSolutions.Select(x => x.Count).Max();
            var allValidSolutions = allSolutions.Where(x => x.Count == maxSolutionCount).ToList();
            return allValidSolutions;
        }

        public static void TestFindLongestAscendingSequence()
        {
            Console.WriteLine("Testing FindLongestAscendingSequence...");
            var sequence = new[] { 1, 3, 2, 6, 1, 7, 9 };
            var solutions = FindLongestAscendingSequence(sequence);
            PrintSolutions();
            sequence = new[] { 7, 6, 6, 5, 4, 3, 2, 1 };
            solutions = FindLongestAscendingSequence(sequence);
            PrintSolutions();

            Console.WriteLine("Testing FindLongestAscendingSequenceDynamically...");
            sequence = new[] { 1, 3, 2, 6, 1, 7, 9 };
            solutions = FindLongestAscendingSequenceDynamically(sequence);
            PrintSolutions(false);
            sequence = new[] { 7, 6, 6, 5, 4, 3, 2, 1 };
            solutions = FindLongestAscendingSequenceDynamically(sequence);
            PrintSolutions(false);

            void PrintSolutions(bool solutionUsesIndex = true)
            {
                Console.WriteLine($"Found {solutions.Count} solutions");
                Console.WriteLine($"The solutions for the longest ascending sequence are as below:\r\n");
                int solutionIndex = 1;
                foreach (var solution in solutions)
                {
                    Console.WriteLine($"The {solutionIndex++}th solution is:");
                    var solutionWithWeight = solution.Select(x => solutionUsesIndex? sequence[x] : x).ToList();
                    solutionWithWeight.PrintOut();
                }
            }
        }

        private static void FindLongestAscendingSequence(int[] a,
            int index,
            List<int> currentSolution,
            List<List<int>> solutions)
        {
            if (index >= a.Length)
            {
                return;
            }

            if (a.Length - index + currentSolution.Count < GetCurrentLongestSequence())
            {
                // Even if we can take every element include current element,
                // the longest sequence won't be this current solution, 
                // so simply return
                return;
            }
            if (!currentSolution.Any() || a[index] > a[currentSolution.Last()])
            {
                // Can take current element, just take it
                currentSolution.Add(index);
                // Move on to next element
                FindLongestAscendingSequence(a, index + 1, currentSolution, solutions);
                UpdateSolutions();
                // Restore solution by removing current element
                currentSolution.RemoveAt(currentSolution.Count - 1);
            }

            if (a.Length - index - 1 + currentSolution.Count < GetCurrentLongestSequence())
            {
                // If we don't take current element but take every element after it, still it 
                // won't be a valid solution, so simply return
                return;
            }

            // Find the solution which we will not take current element
            FindLongestAscendingSequence(a, index + 1, currentSolution, solutions);

            int GetCurrentLongestSequence()
            {
                return solutions.Any() ? solutions.First().Count : 0;
            }

            void UpdateSolutions()
            {
                if (currentSolution.Count > GetCurrentLongestSequence())
                {
                    // If we found a new longest sequence which is longer, clear previous solutions
                    // as they become invalid
                    solutions.Clear();
                }
                if (currentSolution.Count >= GetCurrentLongestSequence())
                {
                    // Found a valid solution
                    solutions.Add(currentSolution.ToList());
                }
            }
        }

        private static void FindLongestAscendingSequenceDynamically(int[] a,
            int index,
            List<List<int>> solutionsWithCurrentElement,
            List<List<int>> solutionsWithoutCurrentElement)
        {
            if (index < 0)
            {
                return;
            }

            var previousSolutions = solutionsWithCurrentElement
                .Concat(solutionsWithoutCurrentElement).ToList();
            solutionsWithCurrentElement.Clear();
            solutionsWithoutCurrentElement.Clear();
            foreach (var solution in previousSolutions)
            {
                if (a[index] < solution.First()) // Can take this element, take it, form a new solution
                {
                    var newSolution = solution.Prepend(a[index]).ToList();
                    UpdateSolution(solutionsWithCurrentElement, newSolution);
                }

                UpdateSolution(solutionsWithoutCurrentElement, solution);
            }

            if (!solutionsWithCurrentElement.Any())
            {
                solutionsWithCurrentElement.Add(new List<int> { a[index] });
            }

            FindLongestAscendingSequenceDynamically(
                    a,
                    index - 1,
                    solutionsWithCurrentElement,
                    solutionsWithoutCurrentElement
                );

            static void UpdateSolution(List<List<int>> solutions, List<int> solution)
            {
                if (solution.Count > GetMaxSequenceLength(solutions))
                {
                    solutions.Clear();
                }
                if (solution.Count >= GetMaxSequenceLength(solutions))
                {
                    solutions.Add(solution);
                }
            }

            static int GetMaxSequenceLength(List<List<int>> solutions) =>
                solutions.Any() ? solutions[0].Count : 0;
        }
    }
}
