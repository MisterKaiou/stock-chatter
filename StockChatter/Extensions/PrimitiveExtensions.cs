namespace StockChatter.Extensions
{
	public static class PrimitiveExtensions
	{
		public static bool IsTrue(this bool? it) => it.HasValue && it.Value;
	}
}
