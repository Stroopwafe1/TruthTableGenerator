namespace TruthTableGenerator {

	public class Node {
		public Node Left { get; set; }
		public char Operator { get; set; }
		public Node Right { get; set; }
		public bool Value { get; set; }
		public char Data { get; set; }

		public Node(char data) {
			Data = data;
			Left = Right = null;
		}
	}
}