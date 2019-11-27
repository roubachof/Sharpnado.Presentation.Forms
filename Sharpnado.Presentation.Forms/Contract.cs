// ---------------------------------------------------------------------------------------------------
// Modified version of CodeContracts:
// Requires and Ensures only raise exceptions in DEBUG. Do nothing otherwise.
// ---------------------------------------------------------------------------------------------------
// CodeContracts
// Copyright(c) Microsoft Corporation
// All rights reserved.
// Licensed under the MIT license.
// ---------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Sharpnado.Presentation.Forms
{
    public static class Contract
    {
        private const string RequiresType = "<REQUIRES>";
        private const string EnsuresType = "<ENSURES>";

        /// <summary>
        /// Specifies a contract such that the expression <paramref name="condition"/> must be true before the enclosing method or property is invoked.
        /// </summary>
        /// <remarks>
        /// This call must happen at the beginning of a method or property before any other code.
        /// This contract is exposed to clients so must only reference members at least as visible as the enclosing method.
        /// </remarks>
        [Conditional("DEBUG")]
        public static void Requires(
            Func<bool> condition,
            string message = null,
            [CallerMemberName] string callingMember = null,
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!condition())
            {
                throw new ContractViolationException(
                    FormatMessage(RequiresType, message, callingMember, sourceLineNumber));
            }
        }

        /// <summary>
        /// Specifies a public contract such that the expression <paramref name="condition"/> will be true when the enclosing method or property returns normally.
        /// </summary>
        /// <remarks>
        /// This call must happen at the end of a method or property after any other code.
        /// This contract is exposed to clients so must only reference members at least as visible as the enclosing method.
        /// </remarks>
        [Conditional("DEBUG")]
        public static void Ensures(
            Func<bool> condition,
            string message = null,
            [CallerMemberName] string callingMember = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!condition())
            {
                throw new ContractViolationException(
                    FormatMessage(EnsuresType, message, callingMember, sourceLineNumber));
            }
        }

        /// <summary>
        /// Returns whether the <paramref name="predicate"/> returns <c>true</c>
        /// for all integers starting from <paramref name="fromInclusive"/> to <paramref name="toExclusive"/> - 1.
        /// </summary>
        /// <param name="fromInclusive">First integer to pass to <paramref name="predicate"/>.</param>
        /// <param name="toExclusive">One greater than the last integer to pass to <paramref name="predicate"/>.</param>
        /// <param name="predicate">Function that is evaluated from <paramref name="fromInclusive"/> to <paramref name="toExclusive"/> - 1.</param>
        /// <returns><c>true</c> if <paramref name="predicate"/> returns <c>true</c> for all integers
        /// starting from <paramref name="fromInclusive"/> to <paramref name="toExclusive"/> - 1.</returns>
        /// <seealso cref="System.Collections.Generic.List&lt;T&gt;.TrueForAll"/>
        public static bool ForAll(int fromInclusive, int toExclusive, Predicate<int> predicate)
        {
#if !DEBUG
            return true;
#endif
            if (fromInclusive > toExclusive)
            {
                throw new ArgumentException("fromInclusive must be less than or equal to toExclusive");
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            for (int i = fromInclusive; i < toExclusive; i++)
            {
                if (!predicate(i))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns whether the <paramref name="predicate"/> returns <c>true</c>
        /// for any integer starting from <paramref name="fromInclusive"/> to <paramref name="toExclusive"/> - 1.
        /// </summary>
        /// <param name="fromInclusive">First integer to pass to <paramref name="predicate"/>.</param>
        /// <param name="toExclusive">One greater than the last integer to pass to <paramref name="predicate"/>.</param>
        /// <param name="predicate">Function that is evaluated from <paramref name="fromInclusive"/> to <paramref name="toExclusive"/> - 1.</param>
        /// <returns><c>true</c> if <paramref name="predicate"/> returns <c>true</c> for any integer
        /// starting from <paramref name="fromInclusive"/> to <paramref name="toExclusive"/> - 1.</returns>
        /// <seealso cref="System.Collections.Generic.List&lt;T&gt;.Exists"/>
        public static bool Exists(int fromInclusive, int toExclusive, Predicate<int> predicate)
        {
#if !DEBUG
            return true;
#endif
            if (fromInclusive > toExclusive)
            {
                throw new ArgumentException("fromInclusive must be less than or equal to toExclusive");
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            for (int i = fromInclusive; i < toExclusive; i++)
            {
                if (predicate(i))
                {
                    return true;
                }
            }

            return false;
        }

        private static string FormatMessage(
            string contractType,
            string message,
            string callingMember,
            int sourceLineNumber)
        {
            StringBuilder sb = new StringBuilder($"{contractType} contract violation");
            if (callingMember != null)
            {
                sb.Append($" at {callingMember}");
            }

            if (sourceLineNumber != 0)
            {
                sb.Append($", line {sourceLineNumber}");
            }

            if (message != null)
            {
                sb.Append($": {message}");
            }

            return sb.ToString();
        }
    }

    public class ContractViolationException : Exception
    {
        public ContractViolationException(string message)
            : base(message)
        {
        }
    }
}