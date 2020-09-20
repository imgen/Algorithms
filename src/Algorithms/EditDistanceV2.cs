using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// #1. Use consecutive common char blocks instead of a char at once
/// #2. Calculate whether a partial edit distance + Math.Abs(a.Length -1 - aIndex - b.Length - 1 - bIndex), 
/// if the sum > minEditDistance, remove the solution from the list
/// if the sum == minEditDistance, but the rest of the string is not all common chars, also remove the solution
/// #3. Try to remove certain solutions by use dynamic programing logic
/// </summary>

namespace Algorithms
{
    public static class EditDistanceV2
    {
        public static void TestCalculateEditDistance()
        {
            //TestCalculateEditDistance("xyzabc", "abcde");
            //TestCalculateEditDistance("mabc", "ambxcde");
            //TestCalculateEditDistance("abc", "ambxcde");
            //TestCalculateEditDistance("uvwxyzabc", "abmncd");
            //TestCalculateEditDistance("uvwxyzabc", "amnobcdefghi");
            TestCalculateEditDistance("aaaaaaaaaaaaaaaaaaaaaaaXaaaaaaaaaaaaaaaaaaaaaaaaaaa", "aaaaaaXaaaaaaaaaaaaaaaaXaaaaaaaaaaaaaaaaaaaaXaaaaaaX", 
                3, 2, 13);
            TestCalculateEditDistance("abc*efghijklm---nopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ", "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLM+++NOPQRSTU*WXYZ", 
                8, 1, 16);
            TestCalculateEditDistance("aaaaaaaaaaaaaaaaaaaaaaabccccccbaaaaaaaaaaaaaaaaaaaa", "aaaaaaaaaaaaaaaaaaaaaaaaabccccccbaaaaaaaaaaaaaaaaaa", 
                2, 1, 9);
            TestCalculateEditDistance(
                "CGICNWFJZOWBDEVORLYOOUHSIPOICMSOQIUBGSDIROYOMJJSLUPVRBNOOPAHMPNGQXHCPSLEYZEYSDGF",
                "TBYHUADAJRXTDDKWMPYKNQFMGBOXJGNLOGIKTLKVUKREEEVZMTCJVUZSHNRZKGRQOXMBZYGBNDFHDVLM",
                74, 114, 1245
            );
            TestCalculateEditDistance(
                "NTBFKWGUYSLYRMMPSXNYXAZNZFJIPJDMNPZNLPEEYEONABFCZEZYVGCJBFMGWKQPTTGJDLKKQYJZYFSL",
                "PEDWJMTVXVGGCLSTOOQEACZJNOVUYXPQGIRAPHFWAESSZKHKGKUEEGVWZXVFJWLQBUBOJMHLEHGKWYTN",
                70, 8, 699
            );
            TestCalculateEditDistance(
                "RPXZTOSEPHWYBYSOOAKMOOJRSSFHENHDVHOIKVNXMFAEXXTBNPNFPTFGNUPLKDWRSUQYWAUVMNVYJZQL",
                "MFKSTCDHEPTSMYNDRSNJZOULCCIUXZDCGQZRSRYEODXKNBGBBKSPPCQHJYUSCKMJZBPUBLLXPRCQJYRV",
                72, 15, 586
            );
            TestCalculateEditDistance(
                "USJZEXTQXQYCXPMSRNGIWRHJFQZFQYSOTBEUZMWWHJBOTOUPGLMRDITCGYIUJXGTBIOAJWYXCHUWFNYP",
                "DKAXVOVHAAWFYDZXJHUUXIGQRIBQGNFHYYIYDZDTDYHGOZPRLQLUOHLKWLCPXKVDGWXYROAHSVEICUYF",
                72, 600, 3344
            );
            TestCalculateEditDistance(
                "GMPOQQULURLAFHPSVGLCGWVTGJZEARVPKRKEWEOONARMPIEMYPUJYTHKQBYDMTPXGDKJTSHOJHWIWXBL",
                "VSXFWFBANKGTNLVHZRJPHLGKMTCLSWCIQONXSGEBZESADLWHYUCFLFEJNBISZMVVLLCANHKLRSONBABF",
                74, 5, 997
            );
            TestCalculateEditDistance(
                "CFACAXPMVDBVRTXQNNALQJVGTRWFIFHUBGFQEUCYVXPABQBPKZWQVRVYIETXJTUKXIDGRRGPYCAOZNEL",
                "UJSLLVNZRJXMXDKRFZMZNQNLZENYKGAKINKZXVRZGCETREQCNCWABDXLKAEBLXRIRDVHELGADMJDMPJN",
                74, 57, 2175
            );

            static void TestCalculateEditDistance(string a, string b, 
                int expectedEditDistance,
                int expectedSolutionCount, 
                int currentTotalSolutionCount
                )
            {
                if (a.Length > b.Length)
                {
                    (a, b) = (b, a);
                }
                int editDistance = CalculateEditDistance(
                        a, b,
                        expectedEditDistance,
                        expectedSolutionCount, 
                        currentTotalSolutionCount
                    );
                Console.WriteLine($"The edit distance for {a} and {b} is {editDistance}");
                Console.WriteLine(Environment.NewLine);
            }
        }

        private static int CalculateEditDistance(string a, 
            string b,
            int expectedEditDistance,
            int expectedSolutionCount,
            int currentTotalSolutionCount)
        {
            int maximumDistance = Math.Max(a.Length, b.Length);
            int editDistance = maximumDistance;
            var commonCharsMap = new Dictionary<string, CommonConsecutiveChars>();
            var occuranceMap = BuildOccuranceMap(b);

            var solutions = new List<CommonSubsequence>();
            var newSolutions = new List<CommonSubsequence>();
            for (int i = 0; i < a.Length; i++)
            {
                RemoveImpossibleSolutionsAtElement(i);
                foreach (var solution in solutions)
                {
                    var indices = FindIndices(a[i], solution.LastCommonChars.SecondEndIndex);
                    foreach (var index in indices)
                    {
                        var commonChars = FindCommonChars(a, b, i, index);
                        if (commonChars == null)
                        {
                            continue;
                        }
                        var newSolution = solution.Append(commonChars);
                        UpdateSolutions(newSolution, newSolutions);
                    }
                }
                var indicesOfCurrentChar = FindIndices(a[i]);
                
                foreach (var index2 in indicesOfCurrentChar)
                {
                    var commonChars = FindCommonChars(a, b, i, index2);
                    if (commonChars == null)
                    {
                        continue;
                    }
                    var newSolution = new CommonSubsequence(new List<CommonConsecutiveChars> { commonChars }, a, b);
                    UpdateSolutions(newSolution, newSolutions);
                }

                var subOptimalNewSolutions = newSolutions
                    .GroupBy(x => x.LastCommonChars.FirstEndIndex + "," + x.LastCommonChars.SecondEndIndex)
                    .SelectMany(g =>
                    {
                        var minPartialEditDistance = g
                            .Min(x => x.GetPartialEditDistance());
                        return g.Where(x => x.GetPartialEditDistance() > minPartialEditDistance);
                    }).ToList();

                var validNewSolutions = newSolutions
                    .Except(subOptimalNewSolutions);
                solutions.AddRange(validNewSolutions);
                RemoveImpossibleSolutions();
                newSolutions.Clear();
            }

            if (solutions.Count > currentTotalSolutionCount)
            {
                PrintError("\r\nOops, regression in performance\r\n");
            }

            if (editDistance != expectedEditDistance)
            {
                PrintError("Oops, wrong edit distance");
            }

            Console.WriteLine($"We found {solutions.Count} common subsequences. These are as below:");
            //PrintCommonSubsequences(solutions);

            var allValidSolutions = solutions.Where(x => x.GetFullEditDistance() == editDistance)
                .ToList();
            if (allValidSolutions.Count < expectedSolutionCount)
            {
                PrintError("\r\nOops, lost some valid solutions\r\n");
            }

            Console.WriteLine($"We have found {allValidSolutions.Count} common subsequences for {a} and {b} with minimum edit distance {editDistance}. These are as below:");
            //PrintCommonSubsequences(allValidSolutions);

            return editDistance;

            static void PrintError(string error)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\r\n{error}\r\n");
                Console.ForegroundColor = ConsoleColor.Black;
            }

            static void PrintCommonSubsequences(IList<CommonSubsequence> subsequences)
            {
                foreach (var sequence in subsequences)
                {
                    Console.WriteLine(sequence.GetCommonString());
                }

                Console.WriteLine();
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

            void UpdateSolutions(CommonSubsequence newSolution, List<CommonSubsequence> solutions)
            {
                var partialEditDistance = newSolution.GetPartialEditDistance();
                var fullEditDistance = newSolution.GetFullEditDistance();
                var bestPossibleEditDistance = newSolution.GetBestPossibleEditDistance();
                if (fullEditDistance <= editDistance)
                {
                    editDistance = fullEditDistance;
                    solutions.Add(newSolution);
                }
                else if (bestPossibleEditDistance <= editDistance)
                {
                    solutions.Add(newSolution);
                }
            }

            void RemoveImpossibleSolutions()
            {
                var impossibleSolutions = solutions
                    .Where(x => x.GetBestPossibleEditDistance() > editDistance)
                    .ToList();
                foreach(var impossibleSolution in impossibleSolutions)
                {
                    solutions.Remove(impossibleSolution);
                }
            }

            // TODO: Further remove impossible ones
            void RemoveImpossibleSolutionsAtElement(int i)
            {
                var impossibleSolutions = solutions
                    .Where(x => x.GetBestPossibleEditDistanceAtIndex(i) > editDistance)
                    .ToList();
                foreach (var impossibleSolution in impossibleSolutions)
                {
                    solutions.Remove(impossibleSolution);
                }
            }

            List<int> FindIndices(char value, int? lastIndex = null)
            {
                return !occuranceMap.ContainsKey(value) ? new List<int>() :
                    occuranceMap[value]
                    .Where(i => lastIndex is null || i > lastIndex.Value)
                    .ToList();
            }

            CommonConsecutiveChars FindCommonChars(string a, string b, int firstStart, int secondStart)
            {
                var key = $"{firstStart},{secondStart}";
                if (commonCharsMap.ContainsKey(key))
                {
                    return commonCharsMap[key];
                }
                int firstEnd = firstStart;
                int secondEnd = secondStart;

                while(firstEnd < a.Length - 1 && 
                    secondEnd < b.Length - 1 && 
                    a[firstEnd + 1] == b[secondEnd + 1])
                {
                    firstEnd++;
                    secondEnd++;
                }
                var commonChars = new CommonConsecutiveChars(a, firstStart, firstEnd, secondStart, secondEnd);
                var prefix = $"{firstStart},";
                var similarCommonCharsWithSameStart = commonCharsMap.Keys
                    .Where(x => x.StartsWith(prefix))
                    .Select(x => commonCharsMap[x]);
                var anyFullCoverCurrentCommonChars = similarCommonCharsWithSameStart
                    .Any(x => x.FirstEndIndex > commonChars.FirstEndIndex && 
                              x.SecondStartIndex <= commonChars.SecondStartIndex && 
                              x.SecondEndIndex >= commonChars.SecondEndIndex
                    );
                return anyFullCoverCurrentCommonChars ? null : (commonCharsMap[key] = commonChars);
            }
        }
    }

    class CommonConsecutiveChars
    {
        public string String;
        public int FirstStartIndex, FirstEndIndex, SecondStartIndex, SecondEndIndex;
        public CommonConsecutiveChars(string a, int firstStart, int firstEnd, int secondStart, int secondEnd)
        {
            String = a.Substring(firstStart, firstEnd - firstStart + 1);
            FirstStartIndex = firstStart;
            FirstEndIndex = firstEnd;
            SecondStartIndex = secondStart;
            SecondEndIndex = secondEnd;
        }
    }

    class CommonSubsequence
    {
        public List<CommonConsecutiveChars> Sequence;
        public CommonConsecutiveChars LastCommonChars => Sequence[^1];
        public string A, B;

        private string _commonString;
        public string GetCommonString()
        {
            if (_commonString != null)
            {
                return _commonString;
            }
            return _commonString = Sequence.Aggregate(string.Empty, (str, chars) => str + chars.String);
        }

        public CommonSubsequence(List<CommonConsecutiveChars> commonChars, string a, string b)
        {
            Sequence = commonChars;
            A = a;
            B = b;
        }

        public CommonSubsequence(CommonSubsequence previousCommonChars, 
            CommonConsecutiveChars newCommonChar,
            string a, 
            string b)
        {
            Sequence = previousCommonChars.Sequence.Append(newCommonChar).ToList();
            _partialEditDistance = GetEditDistance(previousCommonChars, newCommonChar);
            A = a;
            B = b;
        }

        private int _partialEditDistance = -1;
        private int _fullEditDistance = -1;
        public int GetPartialEditDistance()
        {
            if (_partialEditDistance >= 0)
            {
                return _partialEditDistance;
            }

            _partialEditDistance = 0;
            int previousFirstIndex = -1, previousSecondIndex = -1;
            int firstDistance, secondDistance;
            foreach (var commonChars in Sequence)
            {
                int firstIndex = commonChars.FirstStartIndex;
                int secondIndex = commonChars.SecondStartIndex;
                firstDistance = firstIndex - previousFirstIndex - 1;
                secondDistance = secondIndex - previousSecondIndex - 1;
                _partialEditDistance += Math.Max(firstDistance, secondDistance);
                previousFirstIndex = commonChars.FirstEndIndex;
                previousSecondIndex = commonChars.SecondEndIndex;
            }

            return _partialEditDistance;
        }

        public int GetFullEditDistance()
        {
            if (_fullEditDistance >= 0)
            {
                return _fullEditDistance;
            }
            var lastCommonChars = LastCommonChars;
            int firstDistance = A.Length - 1 - lastCommonChars.FirstEndIndex;
            int secondDistance = B.Length - 1 - lastCommonChars.SecondEndIndex;
            _fullEditDistance = GetPartialEditDistance() + Math.Max(firstDistance, secondDistance);
            return _fullEditDistance;
        }

        private int _bestPossibleFullEditDistance = -1;
        public int GetBestPossibleEditDistance()
        {
            if (_bestPossibleFullEditDistance >= 0)
            {
                return _bestPossibleFullEditDistance;
            }
            var lastCommonChars = LastCommonChars;
            int firstDistance = A.Length - 1 - lastCommonChars.FirstEndIndex;
            int secondDistance = B.Length - 1 - lastCommonChars.SecondEndIndex;
            int bestLeftOverDistance = Math.Abs(firstDistance - secondDistance);
            _bestPossibleFullEditDistance = GetPartialEditDistance() + bestLeftOverDistance;
            return _bestPossibleFullEditDistance;
        }

        private readonly Dictionary<int, int> _bestPossibleDistanceAtIndexMap = new Dictionary<int, int>();
        public int GetBestPossibleEditDistanceAtIndex(int index)
        {
            var lastCommonChars = LastCommonChars;
            int startIndex = lastCommonChars.SecondEndIndex + 1;
            if (index <= startIndex)
            {
                return GetBestPossibleEditDistance();
            }
            if (_bestPossibleDistanceAtIndexMap.ContainsKey(index))
            {
                return _bestPossibleDistanceAtIndexMap[index];
            }
            
            var secondDistance = B.Length - 1 - lastCommonChars.SecondEndIndex;
            int distanceFromFirstEnd = 1;
            var firstDistanceAtI = A.Length - 1 - startIndex;
            for (int i = startIndex + 1; i < A.Length; i++)
            {
                var bestPossibleDistance = distanceFromFirstEnd + 
                    Math.Abs(firstDistanceAtI - secondDistance);
                _bestPossibleDistanceAtIndexMap[i] = bestPossibleDistance;
                distanceFromFirstEnd++;
                firstDistanceAtI--;
            }
            
            return _bestPossibleDistanceAtIndexMap[index];
        }

        private int GetEditDistance(CommonSubsequence commonSubsequence, CommonConsecutiveChars newCommonChars)
        {
            int editDistance = commonSubsequence.GetPartialEditDistance();
            var lastCommonChars = commonSubsequence.LastCommonChars;
            var firstDistance = newCommonChars.FirstStartIndex - lastCommonChars.FirstEndIndex - 1;
            var secondDistance = newCommonChars.SecondStartIndex - lastCommonChars.SecondEndIndex - 1;
            return editDistance + Math.Max(firstDistance, secondDistance);
        }

        public CommonSubsequence Append(CommonConsecutiveChars commonChars)
        {
            return new CommonSubsequence(this, commonChars, A, B);
        }
    }
}
