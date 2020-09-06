using System;
using System.Collections.Generic;
using System.Linq;

namespace Algorithms
{
    class LongestCommonSubsequence
    {
        private static List<List<int>> FindLongestCommonSubsequenceDynamically(int[] a, int[] b)
        {
            if (a.Length > b.Length)
            {
                (a, b) = (b, a);
            }
            var occuranceMap = BuildOccuranceMap(b);

            var solutionsWithCurrentElement = new List<List<int>>();
            var solutionsWithoutCurrentElement = new List<List<int>>();
            for (int i = 0; i < a.Length; i++)
            {
                var previousSolutions = solutionsWithCurrentElement
                    .Concat(solutionsWithoutCurrentElement)
                    .ToList();
                solutionsWithCurrentElement.Clear();
                solutionsWithoutCurrentElement.Clear();
                foreach (var solution in previousSolutions)
                {
                    var minimumIndex = FindMinimumIndex(a[i], solution.Last());
                    if (minimumIndex >= 0)
                    {
                        var newSolution = solution.Append(minimumIndex).ToList();
                        UpdateSolution(solutionsWithCurrentElement, newSolution);
                    }

                    UpdateSolution(solutionsWithoutCurrentElement, solution);
                }

                if (!solutionsWithCurrentElement.Any())
                {
                    var minimumIndex = FindMinimumIndex(a[i]);
                    if (minimumIndex >= 0)
                    {
                        solutionsWithCurrentElement.Add(new List<int> { minimumIndex });
                    }
                }
            }

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

            var maxLength = Math.Max(
                    GetMaxSequenceLength(solutionsWithCurrentElement),
                    GetMaxSequenceLength(solutionsWithoutCurrentElement)
                );

            return solutionsWithCurrentElement
                .Concat(solutionsWithoutCurrentElement)
                .Where(x => x.Count == maxLength)
                .ToList();

            static int GetMaxSequenceLength(List<List<int>> solutions) =>
                solutions.Any() ? solutions[0].Count : 0;

            static Dictionary<int, List<int>> BuildOccuranceMap(int[] ints)
            {
                return ints
                    .Select((x, index) => (value: x, index))
                    .GroupBy(x => x.value)
                    .ToDictionary(
                        x => x.Key, 
                        x => x.Select(x => x.index)
                            .OrderBy(x => x)
                            .ToList()
                    );
            }

            int FindMinimumIndex(int value, int? lastIndex = null)
            {
                if (!occuranceMap.ContainsKey(value))
                {
                    return -1;
                }

                var indices = occuranceMap[value];
                return indices.FirstOrDefault(
                        x => lastIndex == null || x > lastIndex.Value,
                        defaultValue: -1
                    );
            }
        }

        public static void TestFindLongestCommonSubsequence()
        {
            Console.WriteLine("Testing FindLongestAscendingSequence...");
            var sequenceA = new[] { 1, 3, 2, 6, 1, 7, 9 };
            var sequenceB = new[] { 2, 1, 8, 6, 9, 2, 1, 3, 7, 9 };
            var solutions = FindLongestCommonSubsequenceDynamically(sequenceA, sequenceB);
            PrintSolutions();

            void PrintSolutions(bool solutionUsesIndex = true)
            {
                Console.WriteLine($"Found {solutions.Count} solutions");
                Console.WriteLine($"The solutions for the longest common subsequence are as below:\r\n");
                int solutionIndex = 1;
                foreach (var solution in solutions)
                {
                    Console.WriteLine($"The {solutionIndex++}th solution is:");
                    var solutionWithWeight = solution
                        .Select(x => solutionUsesIndex ? sequenceB[x] : x)
                        .ToList();
                    solutionWithWeight.PrintOut();
                }
            }
        }
    }
}
