using System;
using System.Collections.Generic;
using System.Linq;

namespace TruthTableGenerator {

	public class ExpressionTree {

		private readonly char[] opArray = new char[] {
			'→', '∧', '∨', '¬'
		};

		private bool IsOperator(char c) {
			return opArray.FirstOrDefault(ch => ch == c) != default(char);
		}

		public void InOrder(Node node) {
			if (node == null) return;
			InOrder(node.Left);
			//Console.WriteLine($"L: {node.Left?.Data}\tR: {node.Right?.Data}\tData: {node.Data}");
			InOrder(node.Right);
		}

		public bool Solve(Node node) {
			if (node == null) return false;
			if (char.IsLetter(node.Data)) {
				var variable = Truthtable.MainVariables.FirstOrDefault(v => v.Name == node.Data.ToString());
				if (variable != default(Variable))
					return variable.Value;
			}

			if (IsOperator(node.Data)) {
				node.Operator = node.Data;
			}

			bool A = Solve(node.Left);
			bool B = Solve(node.Right);

			bool outcome = Calculate(A, B, node);

			var varA = GetVariableByName(node.Left?.Data.ToString());
			Variable varB = null;
			if (node.Right != null)
				varB = GetVariableByName(node.Right?.Data.ToString());

			//Console.WriteLine($"{node.Left.Data} {node.Data} {node.Right.Data} = {outcome}");
			var varOutput = GetVariableByName($"{node.Left?.Data}{node.Operator}{node.Right?.Data}");
			if (varOutput == null)
				Truthtable.Variables.Add(new Variable($"{node.Left.Data}{node.Operator}{node.Right?.Data}", outcome));
			else
				varOutput.Value = outcome;

			//Truthtable.Variables.Add(Calculate(varA, varB, node));
			return outcome;
		}

		public Variable GetVariableByName(string name) {
			if (name.Length == 1 && char.IsLetter(name[0])) {
				var variable = Truthtable.MainVariables.FirstOrDefault(v => v.Name == name);
				if (variable != default(Variable))
					return variable;
			} else {
				var variable = Truthtable.Variables.FirstOrDefault(v => v.Name == name);
				if (variable != default(Variable))
					return variable;
			}
			return null;
		}

		public bool Calculate(bool A, bool B, Node c) {
			return c.Operator switch {
				'→' => !A || B,
				'∧' => A && B,
				'∨' => A || B,
				'¬' => !A,
				_ => false,
			};
		}

		public Variable Calculate(Variable A, Variable B, Node c) {
			return c.Operator switch {
				'→' => A.Implication(B),
				'∧' => A.And(B),
				'∨' => A.Or(B),
				'¬' => A.Negate(),
				_ => null,
			};
		}

		public char[] GetPostfixFromExpression(string expression) {
			Dictionary<char, int> precedence = new Dictionary<char, int>() {
				{ '→', 2 },
				{ '∧', 2 },
				{ '∨', 2 },
				{ '¬', 3 },
				{ '(', 1 }
			};
			Stack<char> opStack = new Stack<char>();
			List<char> output = new List<char>();
			char[] tokens = expression.ToCharArray();
			foreach (char token in tokens) {
				if (token == ' ') continue;
				if (!IsOperator(token)) {
					output.Add(token);
				} else if (token.Equals("(")) {
					opStack.Push(token);
				} else if (token.Equals(")")) {
					char topToken = opStack.Pop();
					while (topToken != '(') {
						output.Add(topToken);
						if (opStack.Count == 0) throw new Exception("Reached end of stack while trying to find ')'");
						topToken = opStack.Pop();
					}
				} else {
					//while (opStack.Count > 0 && precedence[opStack.Peek()] >= precedence[token]) {
					//	output.Add(opStack.Pop());
					//}
					opStack.Push(token);
				}
			}

			while (opStack.Count > 0) {
				output.Add(opStack.Pop());
			}
			return output.Where(ch => ch != '(' && ch != ')').ToArray();
		}

		public Node ConstructTree(char[] postfix) {
			Stack<Node> st = new Stack<Node>();
			Node t, t1, t2;

			// Traverse through every character of
			// input expression
			for (int i = 0; i < postfix.Length; i++) {
				// If operand, simply Push into stack
				if (!IsOperator(postfix[i])) {
					t = new Node(postfix[i]);
					st.Push(t);
					if (char.IsLetter(postfix[i])) {
						var found = Truthtable.MainVariables.FirstOrDefault(v => v.Name == postfix[i].ToString());
						if (found == default(Variable)) {
							var variable = new Variable(postfix[i].ToString());
							Truthtable.MainVariables.Add(variable);
							Truthtable.Variables.Add(variable);
						}
					}
				} else // operator
				  {
					t = new Node(postfix[i]);
					if (postfix[i] == '¬') {
						t1 = st.Pop();
						t.Left = t1;
						st.Push(t);
						continue;
					}

					// Pop two top nodes
					// Store top
					t1 = st.Pop();     // Remove top
					t2 = st.Pop();

					// make them children
					t.Right = t1;
					t.Left = t2;

					// System.out.println(t1 + "" + t2);
					// Add this subexpression to stack
					st.Push(t);
				}
			}

			// only element will be root of
			// expression tree
			t = st.Peek();
			st.Pop();

			return t;
		}
	}
}