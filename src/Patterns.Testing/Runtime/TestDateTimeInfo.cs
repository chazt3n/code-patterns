using System;

using Patterns.Runtime;

namespace Patterns.Testing.Runtime
{
	/// <summary>
	///    Provides a test implementation of <see cref="IDateTimeInfo" /> that uses a constant value for "now".
	/// </summary>
	public class TestDateTimeInfo : IDateTimeInfo
	{
		private readonly DateTime _testValue;

		/// <summary>
		///    Initializes a new instance of the <see cref="TestDateTimeInfo" /> class.
		/// </summary>
		/// <param name="testValue">The test value.</param>
		public TestDateTimeInfo(DateTime testValue)
		{
			_testValue = testValue;
		}

		/// <summary>
		///    Gets the <see cref="DateTime" /> value representing "now".
		/// </summary>
		/// <returns></returns>
		public DateTime GetNow()
		{
			return _testValue;
		}
	}
}