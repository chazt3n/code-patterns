namespace Patterns.Values
{
	/// <summary>
	///     Model describing the age of a DateTime.
	/// </summary>
	public struct Age
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="Age" /> struct.
		/// </summary>
		/// <param name="years">The years.</param>
		/// <param name="months">The months.</param>
		/// <param name="days">The days.</param>
		public Age(int years, int months, int days) : this()
		{
			Years = years;
			Months = months;
			Days = days;
		}

		/// <summary>
		///     Gets the days.
		/// </summary>
		/// <value>
		///     The days.
		/// </value>
		public int Days { get; private set; }

		/// <summary>
		///     Gets the months.
		/// </summary>
		/// <value>
		///     The months.
		/// </value>
		public int Months { get; private set; }

		/// <summary>
		///     Gets the years.
		/// </summary>
		/// <value>
		///     The years.
		/// </value>
		public int Years { get; private set; }
	}
}