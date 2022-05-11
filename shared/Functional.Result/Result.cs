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

		public async static Task Match<TOf>(this Task<Result<TOf>> asyncRes, Action<TOf> ifSuccess, Action<Errors> ifError)
		{
			var res = await asyncRes;
			res.Match<TOf>(ifSuccess, ifError);
		}
	}

	// Represents a monad with a result
	public struct Result<TOf>
	{
		internal readonly TOf? _value;

		public Errors Errors { get; }

		public bool Success => Errors.Any() == false;

		public bool Failure => Success == false;

		public Result()
		{
			Errors = new Errors();
			_value = default;
		}

		public Result(TOf of) : this()
		{
			if (of == null)
				throw new ArgumentNullException(nameof(of), "The inner value of a successful result can't be null");

			_value = of;
		}

		public Result(Errors errors) : this() => Errors = errors;

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

		public void Match<TResult>(Action<TOf> ifSuccess, Action<Errors> ifError)
		{
			if (Success)
				ifSuccess(_value ?? throw new ArgumentNullException(nameof(_value), "Inner value was null"));
			else 
				ifError(Errors);
		}

		#endregion

	}
}
