using System.Text;

namespace TruthTableGenerator {

	public class Variable {
		public string Name { get; set; }
		public char Alias { get; set; }
		public bool Value { get; set; }

		public Variable(string name, bool value = false) {
			Name = name;
			Value = value;
		}

		public Variable Negate() {
			return new Variable("¬" + Name, !Value);
		}

		public Variable Or(Variable other) {
			return new Variable($"{Name}∨{other.Name}", Value || other.Value);
		}

		public Variable And(Variable other) {
			return new Variable($"{Name}∧{other.Name}", Value && other.Value);
		}

		public Variable Implication(Variable other) {
			return new Variable($"{Name}→{other.Name}", !Value || other.Value);
		}

		public Variable Equivalence(Variable other) {
			return new Variable($"{Name}↔{other.Name}", Value == other.Value);
		}

		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			sb.Append(Name);
			if (Alias != default(char))
				sb.Append($" ({Alias})");
			sb.Append(": " + Value);
			return sb.ToString();
		}
	}
}