using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// #1. Use consecutive common char blocks instead of a char at once
/// #2. Calculate whether a partial edit distance + Math(a.Length -1 - aIndex, b.Length - 1 - bIndex), 
/// if the sum > minEditDistance, remove the solution from the list
/// if the sum == minEditDistance, but the rest of the string is not all common chars, also remove the solution
/// #3. Try to remove certain solutions by use dynamic programing logic
/// </summary>

namespace Algorithms
{
    public static class EditDistanceV2
    {
        private static List<List<(int first, int second)>> FindAllCommonSubsequenceDynamically(string a, string b)
        {
            var swapped = false;
            if (a.Length > b.Length)
            {
                (a, b) = (b, a);
                swapped = true;
            }
            var occuranceMap = BuildOccuranceMap(b);

            var solutions = new List<List<(int first, int second)>>();
            for (int i = 0; i < a.Length; i++)
            {
                var newSolutions = new List<List<(int first, int second)>>();
                foreach (var solution in solutions)
                {
                    var indices = FindIndices(a[i], solution.Last().second);
                    foreach (var index in indices)
                    {
                        var newSolution = solution
                            .Append(GetIndexPair(i, index))
                            .ToList();
                        newSolutions.Add(newSolution);
                    }
                }
                var indicesOfCurrentChar = FindIndices(a[i]);
                foreach (var index2 in indicesOfCurrentChar)
                {
                    var newSolutionOfCurrentChar = new List<(int first, int second)>
                    {
                        GetIndexPair(i, index2)
                    };
                    newSolutions.Add(newSolutionOfCurrentChar);
                }

                solutions.AddRange(newSolutions);
            }

            return solutions;

            static Dictionary<char, List<int>> BuildOccuranceMap(string str)
            {
                return str
                    .Select((x, index) => (value: x, index))
                    .GroupBy(x => x.value)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Select(x => x.index)
                            .OrderBy(x => x)
                            .ToList()
                    );
            }

            List<int> FindIndices(char value, int? lastIndex = null)
            {
                return !occuranceMap.ContainsKey(value) ? new List<int>() :
                    occuranceMap[value]
                    .Where(i => lastIndex is null || i > lastIndex.Value)
                    .ToList();
            }

            (int first, int second) GetIndexPair(int first, int second) =>
                swapped ? (second, first) : (first, second);
        }

        public static void TestCalculateEditDistance()
        {
            //TestCalculateEditDistance("xyzabc", "abcde");
            //TestCalculateEditDistance("mabc", "ambxcde");
            //TestCalculateEditDistance("abc", "ambxcde");
            TestCalculateEditDistance("uvwxyzabc", "abmncd");
            //TestCalculateEditDistance("uvwxyzabc", "amnobcdefghi");

            static void TestCalculateEditDistance(string a, string b)
            {
                int editDistance = CalculateEditDistance(a, b);
                Console.WriteLine($"The edit distance for {a} and {b} is {editDistance}");
                Console.WriteLine(Environment.NewLine);
            }
        }

        private static int CalculateEditDistance2(string a, string b)
        {
            int maximumDistance = Math.Max(a.Length, b.Length);

            int editDistance = maximumDistance;

            var swapped = false;
            if (a.Length > b.Length)
            {
                (a, b) = (b, a);
                swapped = true;
            }
            var occuranceMap = BuildOccuranceMap(b);

            var solutions = new List<CommonSubsequence>();
            for (int i = 0; i < a.Length; i++)
            {
                var newSolutions = new List<CommonSubsequence>();
                foreach (var solution in solutions)
                {
                    var indices = FindIndices(a[i], solution.Sequence.Last().SecondStartIndex);
                    foreach (var index in indices)
                    {
                        var newSequence = solution.Sequence
                            .Append(GetCommonChar(i, index))
                            .ToList();
                        var newSolution = new CommonSubsequence(newSequence);
                        newSolutions.Add(newSolution);
                    }
                }
                var indicesOfCurrentChar = FindIndices(a[i]);
                foreach (var index2 in indicesOfCurrentChar)
                {
                    var newSequence = new List<CommonConsecutiveChars>
                    {
                        GetCommonChar(i, index2)
                    };
                    var newSolution = new CommonSubsequence(newSequence);
                    newSolutions.Add(newSolution);
                }

                solutions.AddRange(newSolutions);
            }

            static Dictionary<char, List<int>> BuildOccuranceMap(string str)
            {
                return str
                    .Select((x, index) => (value: x, index))
                    .GroupBy(x => x.value)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Select(x => x.index)
                            .OrderBy(x => x)
                            .ToList()
                    );
            }

            List<int> FindIndices(char value, int? lastIndex = null)
            {
                return !occuranceMap.ContainsKey(value) ? new List<int>() :
                    occuranceMap[value]
                    .Where(i => lastIndex is null || i > lastIndex.Value)
                    .ToList();
            }

            CommonConsecutiveChars GetCommonChar(int first, int second) =>
                swapped ? new CommonConsecutiveChars(a[first], second, first) :
                new CommonConsecutiveChars(a[first], first, second);

            return editDistance;

        }

        private static int CalculateEditDistance(string a, string b)
        {
            var commonSubsequences = FindAllCommonSubsequenceDynamically(a, b);
            int maximumDistance = Math.Max(a.Length, b.Length);
            if (!commonSubsequences.Any())
            {
                return maximumDistance;
            }

            Console.WriteLine($"We found {commonSubsequences.Count} common subsequences for {a} and {b}. These are as below:");
            foreach (var sequence in commonSubsequences)
            {
                var commonStr = new string(sequence.Select(x => a[x.first]).ToArray());
                Console.WriteLine(commonStr);
            }

            Console.WriteLine();

            int editDistance = maximumDistance;
            var editDistances = new List<int>();
            foreach (var sequence in commonSubsequences)
            {
                int editDistanceOfSeq = CalculateEditDistanceForSequence(sequence);
                editDistances.Add(editDistanceOfSeq);
                if (editDistanceOfSeq < editDistance)
                {
                    editDistance = editDistanceOfSeq;
                }
            }

            var subSequencesWithMinEditDistances = commonSubsequences
                .Where((_, i) => editDistances[i] == editDistance)
                .ToList();

            Console.WriteLine($"We have found {subSequencesWithMinEditDistances.Count} common subsequences for {a} and {b} with minimum edit distance {editDistance}. These are as below:");
            foreach (var sequence in subSequencesWithMinEditDistances)
            {
                var commonStr = new string(sequence.Select(x => a[x.first]).ToArray());
                Console.WriteLine(commonStr);
            }

            Console.WriteLine();

            return editDistance;
            int CalculateEditDistanceForSequence(List<(int first, int second)> sequence)
            {
                int editDistanceOfSeq = 0;
                int previousFirstIndex = -1, previousSecondIndex = -1;
                int firstDistance, secondDistance;
                foreach (var (firstIndex, secondIndex) in sequence)
                {
                    firstDistance = firstIndex - previousFirstIndex - 1;
                    secondDistance = secondIndex - previousSecondIndex - 1;
                    editDistanceOfSeq += Math.Max(firstDistance, secondDistance);
                    previousFirstIndex = firstIndex;
                    previousSecondIndex = secondIndex;
                }
                var (lastFirstIndex, lastSecondIndex) = sequence.Last();
                firstDistance = a.Length - lastFirstIndex - 1;
                secondDistance = b.Length - lastSecondIndex - 1;
                editDistanceOfSeq += Math.Max(firstDistance, secondDistance);
                return editDistanceOfSeq;
            }
        }
    }

    class CommonConsecutiveChars
    {
        public string String;
        public int FirstStartIndex, FirstEndIndex, SecondStartIndex, SecondEndIndex;
        public CommonConsecutiveChars(string a, int firstStart, int firstEnd, int secondStart)
        {
            String = a.Substring(firstStart, firstEnd - firstStart + 1);
            FirstStartIndex = firstStart;
            FirstEndIndex = firstEnd;
            SecondStartIndex = secondStart;
            SecondEndIndex = secondStart + String.Length;
        }
    }

    class CommonSubsequence
    {
        public List<CommonConsecutiveChars> Sequence;

        public CommonSubsequence(List<CommonConsecutiveChars> commonChars)
        {
            Sequence = commonChars;
        }

        public CommonSubsequence(CommonSubsequence previousCommonChars, CommonConsecutiveChars newCommonChar)
        {
            Sequence = previousCommonChars.Sequence.Append(newCommonChar).ToList();
            _editDistance = GetEditDistance(previousCommonChars, newCommonChar);
        }

        private int _editDistance = -1;
        int GetEditDistance()
        {
            if (_editDistance >= 0)
            {
                return _editDistance;
            }

            _editDistance = 0;
            int previousFirstIndex = -1, previousSecondIndex = -1;
            int firstDistance, secondDistance;
            foreach (var commonChars in Sequence)
            {
                int firstIndex = commonChars.FirstStartIndex;
                int secondIndex = commonChars.SecondStartIndex;
                firstDistance = firstIndex - previousFirstIndex - 1;
                secondDistance = secondIndex - previousSecondIndex - 1;
                _editDistance += Math.Max(firstDistance, secondDistance);
                previousFirstIndex = commonChars.FirstEndIndex;
                previousSecondIndex = commonChars.SecondEndIndex;
            }

            return _editDistance;
        }

        private int GetEditDistance(CommonSubsequence commonSubsequence, CommonConsecutiveChars newCommonChars)
        {
            int editDistance = commonSubsequence.GetEditDistance();
            var lastCommonChars = commonSubsequence.Sequence.Last();
            var firstDistance = newCommonChars.FirstStartIndex - lastCommonChars.FirstEndIndex - 1;
            var secondDistance = newCommonChars.SecondStartIndex - lastCommonChars.SecondEndIndex - 1;
            return editDistance + Math.Max(firstDistance, secondDistance);
        }
    }
}
