namespace Functional.Result
{
	public static class Result
	{
		public static Result<Unit> CreateSuccess() => new();
		public static Result<T> CreateSuccess<T>(T of) => new(of);
		public static Result<Unit> CreateError(IEnumerable<string> errors) => new(new Errors(errors));
		public static Result<T> CreateError<T>(IEnumerable<string> errors) => new(new Errors(errors));

		public static async Task<Result<TMapped>> Map<TOf, TMapped>(this Task<Result<TOf>> asyncRes, Func<TOf, TMapped> mapper)
		{
			var res = await asyncRes;

			return res.Map(mapper);
		}

		public async static Task<TResult> Match<TOf, TResult>(this Task<Result<TOf>> asyncRes, Func<TOf, TResult> ifSuccess, Func<Errors, TResult> ifError)
		{
			var res = await asyncRes;

			return res.Match(ifSuccess, ifError);
		}

	}

	// Represents a monad with an actual result
	public struct Result<TOf>
	{
		internal readonly TOf? _value = default;

		public Errors Errors { get; } = new Errors();

		public bool Success => Errors.Any() == false;

		public bool Failure => Success == false;


		public Result(TOf of)
		{
			if (of == null)
				throw new ArgumentNullException(nameof(of), "The inner value of a successful result can't be null");

			_value = of;
		}

		public Result(Errors errors) => Errors = errors;

		#region Helpers

		public Result<TMapped> Map<TMapped>(Func<TOf, TMapped> mapper)
		{
			if (Success)
				return Result.CreateSuccess(mapper(_value ?? throw new ArgumentNullException(nameof(_value), "Inner value was null")));

			return Result.CreateError<TMapped>(Errors);
		}

		public TResult Match<TResult>(Func<TOf, TResult> ifSuccess, Func<Errors, TResult> ifError)
		{
			if (Success)
				return ifSuccess(_value ?? throw new ArgumentNullException(nameof(_value), "Inner value was null"));

			return ifError(Errors);
		}

		#endregion

	}
}
