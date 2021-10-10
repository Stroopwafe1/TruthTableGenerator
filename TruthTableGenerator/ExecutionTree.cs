using System;
using System.Collections.Generic;
using System.Linq;

namespace TruthTableGenerator {

	public class ExecutionTree {
		public List<string> expressions = new List<string>();
		public Dictionary<char, string> mnemonics = new Dictionary<char, string>();
		public char currentChar = 'A';

		/*
		 * Recursively work out the parentheses
		 * Place parentheses around the negation => (¬q)
		 * ((p → q) ∧ (q → r)) → (p → r)
		 * (A ∧ (q → r)) → (p → r)
		 * (A ∧ B) → (p → r)
		 * C → (p → r)
		 * C → D
		 */

		public void Parse(string expression) {
			expression = expression.Replace(" ", "");
			Stack<char> stack = new Stack<char>();
			if (!expression.Contains('(')) {
				expressions.Add(expression);
				mnemonics.Add(currentChar, expression);
				currentChar = (char)(currentChar + 1);
				return;
			} else {
				string tempExpression = expression;
				foreach (char c in expression) {
					if (char.IsWhiteSpace(c)) continue;
					if (char.IsLetter(c) && char.IsLower(c)) {
						var variable = Truthtable.GetVariableByName(c.ToString());
						if (variable == default(Variable)) {
							var newVar = new Variable(c.ToString());
							Truthtable.MainVariables.Add(newVar);
							Truthtable.Variables.Add(newVar);
						}
					}
					stack.Push(c);
					if (c == ')') {
						string buildString = "";
						char top = stack.Pop();
						while (top != '(') {
							buildString += top;
							top = stack.Pop();
						}
						buildString += top;
						char[] expressionChars = buildString.ToCharArray();
						Array.Reverse(expressionChars);
						string exp = string.Join("", expressionChars);
						expressions.Add(exp);
						mnemonics.Add(currentChar, exp);
						tempExpression = tempExpression.Replace(exp, currentChar.ToString());
						currentChar = (char)(currentChar + 1);
					}
				}
				Parse(tempExpression);
			}

			currentChar = (char)(currentChar + 1);
			return;
		}

		public void Solve() {
			foreach (string expression in expressions) {
				/* (p→q)
				 * (q→r)
				 * (p→r)
				 */

				string stripped = expression.Replace("(", "").Replace(")", "");
				if (stripped.Length < 3) continue;
				char op = GetOperator(stripped);
				string[] parts = stripped.Split(op);
				var leftVar = Truthtable.GetVariableByName(parts[0]);
				var rightVar = Truthtable.GetVariableByName(parts[1]);

				if (leftVar == default(Variable)) {
					// If it still doesn't have a variable, make one
					var newVar = new Variable(parts[0]);
					Truthtable.Variables.Add(newVar);
					leftVar = newVar;

					if (!char.IsUpper(parts[0][0]))
						Truthtable.MainVariables.Add(newVar);
				}

				if (rightVar == default(Variable)) {
					// If it still doesn't have a variable, make one
					var newVar = new Variable(parts[1]);
					Truthtable.Variables.Add(newVar);
					rightVar = newVar;
					if (!char.IsUpper(parts[1][0]))
						Truthtable.MainVariables.Add(newVar);
				}

				if (leftVar.Name.StartsWith('¬'))
					leftVar = leftVar.Negate();
				else if (rightVar.Name.StartsWith('¬'))
					rightVar = rightVar.Negate();
				string name = $"{leftVar.Name}{op}{rightVar.Name}";
				if (leftVar.Alias != default(char) && rightVar.Alias != default(char))
					name = $"{leftVar.Alias}{op}{rightVar.Alias}";

				char alias = mnemonics.FirstOrDefault(pair => pair.Value.Replace("(", "").Replace(")", "") == name).Key;

				var outputVar = Truthtable.GetVariableByName(name);
				if (outputVar == default(Variable))
					outputVar = Truthtable.GetVariableByName(alias.ToString());
				if (outputVar == default(Variable)) {
					outputVar = new Variable(name);
					Truthtable.Variables.Add(outputVar);
				}
				if (alias != default(char))
					outputVar.Alias = alias;
				switch (op) {
					case '→':
						outputVar.Value = (leftVar.Implication(rightVar)).Value;
						break;
					case '∧':
						outputVar.Value = (leftVar.And(rightVar)).Value;
						break;
					case '∨':
						outputVar.Value = (leftVar.Or(rightVar)).Value;
						break;
				}
			}
		}

		public char GetOperator(string expression) {
			if (expression.Contains('→')) return '→';
			else if (expression.Contains('∧')) return '∧';
			else if (expression.Contains('∨')) return '∨';
			else return default;
		}
	}
}