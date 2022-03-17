using System.Collections;

namespace Functional.Result
{
	public record Errors : IEnumerable<string>, IAddOnlyList<string>
	{
		private readonly List<string> _errors = new List<string>();

		public string this[int index] => ((IReadOnlyList<string>)_errors)[index];

		public int Count => ((IReadOnlyCollection<string>)_errors).Count;

		public Errors() { }

		public Errors(IEnumerable<string> errors)
		{
			_errors.AddRange(errors);
		}

		public IAddOnlyList<string> Add(string it)
		{
			_errors.Add(it);
			return this;
		}

		public IEnumerator<string> GetEnumerator()
		{
			return ((IEnumerable<string>)_errors).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)_errors).GetEnumerator();
		}
	}
}
