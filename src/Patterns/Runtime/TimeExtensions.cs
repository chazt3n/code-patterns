#region New BSD License

// Copyright (c) 2012, John Batte
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted
// provided that the following conditions are met:
// 
// Redistributions of source code must retain the above copyright notice, this list of conditions
// and the following disclaimer.
// 
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions
// and the following disclaimer in the documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.

#endregion

using System;

using Patterns.Values;

namespace Patterns.Runtime
{
	/// <summary>
	///    Provides extensions for accessing and manipulating time-oriented constructs like <see cref="DateTime" /> and
	///    <see
	///       cref="TimeSpan" />
	///    .
	/// </summary>
	public static class TimeExtensions
	{
		/// <summary>
		///    Returns a <see cref="DateTime" /> with the same <see cref="DateTime.Year" /> , <see cref="DateTime.Month" /> ,
		///    <see
		///       cref="DateTime.Day" />
		///    , <see cref="DateTime.Hour" /> , <see cref="DateTime.Minute" /> , and
		///    <see
		///       cref="DateTime.Second" />
		///    values.
		/// </summary>
		/// <param name="value"> The value. </param>
		public static DateTime AccurateToOneSecond(this DateTime value)
		{
			return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second);
		}

		/// <summary>
		///    Returns a <see cref="DateTime" /> with the same <see cref="DateTime.Year" /> , <see cref="DateTime.Month" /> , and
		///    <see
		///       cref="DateTime.Day" />
		///    values.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static DateTime AccurateToOneDay(this DateTime value)
		{
			return new DateTime(value.Year, value.Month, value.Day);
		}

		/// <summary>
		///    Returns a given DateTime as age.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public static Age ToAge(this DateTime value)
		{
			DateTime now = DateTimeInfo.Current.GetNow();

			int years = now.Year - value.Year;
			int days = now.Day - value.Day;
			int months = now.Month - value.Month;

			if (days < 0) months--;

			if (months < 0)
			{
				years--;
				months += 12;
			}

			return new Age(years, months, days);
		}
	}
}