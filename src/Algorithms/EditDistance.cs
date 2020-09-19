using System;
using System.Collections.Generic;
using System.Linq;

namespace Algorithms
{
    public static class EditDistance
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
                    foreach(var index in indices)
                    {
                        var newSolution = solution
                            .Append(GetIndexPair(i, index))
                            .ToList();
                        newSolutions.Add(newSolution);
                    }
                }
                var indicesOfCurrentChar = FindIndices(a[i]);
                foreach(var index2 in indicesOfCurrentChar)
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
            TestCalculateEditDistance("mabc", "ambxcde");
            //TestCalculateEditDistance("abc", "ambxcde");
            //TestCalculateEditDistance("uvwxyzabc", "abcdefghi");

            static void TestCalculateEditDistance(string a, string b)
            {
                int editDistance = CalculateEditDistance(a, b);
                Console.WriteLine($"The edit distance for {a} and {b} is {editDistance}");
                Console.WriteLine(Environment.NewLine);
            }
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
            foreach(var sequence in commonSubsequences)
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
            foreach(var sequence in subSequencesWithMinEditDistances)
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
                foreach(var (firstIndex, secondIndex) in sequence)
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
}
