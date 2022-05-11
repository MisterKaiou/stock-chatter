namespace Functional.Result
{
	public interface IAddOnlyList<T> : IReadOnlyList<T>
	{
		public IAddOnlyList<T> Add(T it);
	}
}
