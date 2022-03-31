using System;
using System.Text;

namespace NoobCore.Tests
{
	/// <summary>
	/// A <see cref="StringBuilder"/> like class with configurable line ending for unit tests.
	/// </summary>
	public class TestStringBuilder
	{
		/// <summary>
		/// 
		/// </summary>
		private readonly string newLine;
		/// <summary>
		/// 
		/// </summary>
		private readonly StringBuilder builder;

		public TestStringBuilder(string newLine)
		{
			this.newLine = newLine ?? throw new ArgumentNullException(nameof(newLine));
			builder = new StringBuilder();
		}

		public TestStringBuilder AppendLine(string value)
		{
			builder.Append(value).Append(newLine);
			return this;
		}

		public override string ToString() => builder.ToString();
	}
}
