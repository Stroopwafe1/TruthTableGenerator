using System;

namespace TruthTableGenerator {

	internal class Program {

		private static void Main(string[] args) {
			//Truthtable truthtable = new Truthtable("((p → q) ∧ (q → r)) → (p → r)", 1);
			//Console.WriteLine(truthtable);
			//Console.ReadLine();
			string input;
			do {
				Console.WriteLine("Enter the expression you want to generate a truth table for... ('stop' to exit)");
				//string expression = "r → ((¬p ∨ q) ∧ (p ∨ q))";
				string expression = input = Console.ReadLine().Trim();
				if (expression.ToLower().Equals("stop") || string.IsNullOrEmpty(expression))
					break;
				expression = expression.Replace("->", "→").Replace("&", "∧").Replace('|', '∨').Replace('!', '¬');
				//string expression = "(p → q) ∧ (q → p)";
				Truthtable truthtable = new Truthtable(expression, 1);
				Console.WriteLine(truthtable);
				Console.WriteLine("\n");
			} while (input != "stop" && input != string.Empty);
		}
	}
}