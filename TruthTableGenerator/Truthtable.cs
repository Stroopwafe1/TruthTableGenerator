using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TruthTableGenerator {

	public class TableRow {
		public List<bool> Output { get; set; } = new List<bool>();

		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			foreach (bool outP in Output) {
				sb.Append(outP.ToString().ToUpper()[0]);
				sb.Append('\t');
			}
			return sb.ToString().Trim();
		}
	}

	public class Truthtable {
		public static List<Variable> Variables { get; set; }
		public static List<Variable> MainVariables { get; set; }
		public List<TableRow> TableRows { get; set; }

		/**
		 *		p v (q -> r)
		 *		  / \
		 *		p	  q → r
		 *				 / \
		 *				q  r
		 * */

		public Truthtable(string expression) {
			// Parse expression into the variables and operations
			// Generate a table row for every state of the variables
			// ToString with tabs, for easy output to excel or something idk

			Variables = new List<Variable>();
			TableRows = new List<TableRow>();
			MainVariables = Variables.Where(v => v.Name.Length == 1).ToList();

			var et = new ExpressionTree();
			char[] postfix = et.GetPostfixFromExpression(expression);
			Node root = et.ConstructTree(postfix);
			et.InOrder(root);

			MainVariables.Reverse();
			for (int i = 0; i < Math.Pow(2, MainVariables.Count); i++) {
				foreach (var mv in MainVariables) {
					int value = (int)Math.Pow(2, MainVariables.IndexOf(mv));
					mv.Value = (i & value) == value;
				}
				et.Solve(root);

				TableRow tableRow = new TableRow();
				foreach (var variable in Variables) {
					//Console.WriteLine($"{variable.Name}: {variable.Value}");
					tableRow.Output.Add(variable.Value);
				}
				TableRows.Add(tableRow);
			}

			Console.WriteLine('\n');
		}

		public Truthtable(string expression, int waste) {
			Variables = new List<Variable>();
			TableRows = new List<TableRow>();
			MainVariables = Variables.Where(v => v.Name.Length == 1).ToList();

			var et = new ExecutionTree();
			et.Parse(expression);

			MainVariables.Reverse();
			
			for (int i = 0; i < Math.Pow(2, MainVariables.Count); i++) {
				foreach (var mv in MainVariables) {
					int value = (int)Math.Pow(2, MainVariables.IndexOf(mv));
					mv.Value = (i & value) == value;
				}
				et.Solve();

				var variableNames = Variables.Select(v => v.Name);
				Variables = Variables.Where(v => !v.Name.Contains('(') || (v.Name.Contains('(') && !variableNames.Contains(v.Name.Replace("(", "").Replace(")", "")))).ToList();
				TableRow tableRow = new TableRow();
				foreach (var variable in Variables) {
					//Console.WriteLine($"{variable.Name}: {variable.Value}");
					tableRow.Output.Add(variable.Value);
				}
				TableRows.Add(tableRow);
			}

			Console.WriteLine('\n');
		}

		public static Variable GetVariableByName(string name) {
			Variable variable = null;
			if (name.Length == 1 && char.IsLower(name[0])) {
				variable = MainVariables.FirstOrDefault(v => v.Name == name);
			} else if (name.Length == 1 && char.IsUpper(name[0])) {
				variable = Variables.FirstOrDefault(v => v.Alias == name[0]);
			} else {
				variable = Variables.FirstOrDefault(v => v.Name == name);
			}
			return variable;
		}

		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			foreach (var variable in Variables) {
				sb.Append(variable.Name);
				if (variable.Alias != default(char))
					sb.Append($" ({variable.Alias})");
				sb.Append('\t');
			}
			sb.AppendLine();
			foreach (var tableRow in TableRows) {
				sb.AppendLine(tableRow.ToString());
			}
			return sb.ToString().Trim();
		}
	}
}