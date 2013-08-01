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

namespace Patterns.Runtime
{
	/// <summary>
	/// 	Defines a default implementation of the <see cref="IDateTimeInfo" /> interface.
	/// </summary>
	public class DateTimeInfo : IDateTimeInfo
	{
		/// <summary>
		/// Gets or sets the current <see cref="IDateTimeInfo"/> selector.
		/// </summary>
		/// <value>
		/// The current <see cref="IDateTimeInfo"/> selector.
		/// </value>
		public static Func<IDateTimeInfo> CurrentSelector { get; set; }

		/// <summary>
		/// Gets the current <see cref="IDateTimeInfo"/>.
		/// </summary>
		/// <value>
		/// The current <see cref="IDateTimeInfo"/>.
		/// </value>
		public static IDateTimeInfo Current
		{
			get
			{
				var selector = CurrentSelector ?? (() => new DateTimeInfo());
				return selector();
			}
		}

		#region IDateTimeInfo Members

		/// <summary>
		/// 	Gets the <see cref="DateTime" /> value representing "now".
		/// </summary>
		/// <returns> </returns>
		public DateTime GetNow()
		{
			return DateTime.Now;
		}

		#endregion
	}
}