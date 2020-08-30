using System;
using System.Collections.Generic;
using System.Linq;

namespace Algorithms
{
    class Knapsack
    {
        public static List<List<int>> LoadKnapsack(int[] weights, int target)
        {
            var solutions = new List<List<int>>();
            LoadKnapsack(weights, target, 0, new List<int>(), solutions);
            return solutions;
        }

        public static void TestLoadKnapsack()
        {
            var weights = new[] { 1, 3, 2, 6, 1, 7, 9 };
            var target = 7;
            var solutions = LoadKnapsack(weights, target);

            Console.WriteLine($"The solutions for the weights with target of {target} are as below:\r\n");
            int solutionIndex = 1;
            foreach(var solution in solutions)
            {
                Console.WriteLine($"The {solutionIndex++}th solution is:");
                var solutionWithWeight = solution.Select(x => weights[x]).ToList();
                solutionWithWeight.PrintOut();
            }
        }

        private static void LoadKnapsack(
            int[] weights, 
            int target, 
            int currentWeightIndex,
            List<int> currentSolution, 
            List<List<int>> solutions)
        {
            if (currentWeightIndex >= weights.Length)
            {
                return;
            }

            // Don't take current weight, then move on to next weight
            LoadKnapsack(weights, target, currentWeightIndex + 1, currentSolution, solutions);

            // Take the current weight
            currentSolution.Add(currentWeightIndex);
            var currentWeight = weights[currentWeightIndex];
            if (currentWeight == target)// Found a solution
            {
                solutions.Add(currentSolution.ToList());
            }
            else if (currentWeight < target) // Can still put more in, move on to next weight
            {
                LoadKnapsack(weights, target - currentWeight, currentWeightIndex + 1, currentSolution, solutions);
            }
            // Restore the current solution
            currentSolution.RemoveAt(currentSolution.Count - 1);
        }
    }
}
