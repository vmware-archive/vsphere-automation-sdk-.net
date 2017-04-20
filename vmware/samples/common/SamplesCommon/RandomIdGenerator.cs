/**
 * *******************************************************
 * Copyright VMware, Inc. 2015, 2016.  All Rights Reserved.
 * SPDX-License-Identifier: MIT
 * *******************************************************
 *
 * DISCLAIMER. THIS PROGRAM IS PROVIDED TO YOU "AS IS" WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, WHETHER ORAL OR WRITTEN,
 * EXPRESS OR IMPLIED. THE AUTHOR SPECIFICALLY DISCLAIMS ANY IMPLIED
 * WARRANTIES OR CONDITIONS OF MERCHANTABILITY, SATISFACTORY QUALITY,
 * NON-INFRINGEMENT AND FITNESS FOR A PARTICULAR PURPOSE.
 */
namespace vmware.samples.common
{
    using System;
    using System.Text;

    public class RandomIdGenerator
    {
        private static readonly string LATIN_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnoprstuvwxyz";
        private static readonly string DIGITS = "0123456789";
        private static readonly Random random = new Random((int)DateTime.Now.Ticks);
        private static readonly string VAPI_UUID_URI_PREFIX = "urn:uuid:";

        public static string NewGuid()
        {
            return Guid.NewGuid().ToString();
        }

        public static string GenerateRandomId(string prefix)
        {
            return prefix + Guid.NewGuid();
        }

        public static string GenerateRandomUri()
        {
            return VAPI_UUID_URI_PREFIX + Guid.NewGuid();
        }

        public static string GetRandomString(string prefix)
        {
            return prefix + GenerateRandomString(5);
        }

        public static string GenerateRandomString(int length)
        {
            if (length < 1)
            {
                throw new ArgumentOutOfRangeException(
                    "length", "Length can not be zero or negative.");
            }
            var sb = new StringBuilder();
            var charsAndDigits = LATIN_CHARS + DIGITS;

            // first character is always Latin char
            sb.Append(
                LATIN_CHARS[random.Next(LATIN_CHARS.Length)]);

            var index = 1;
            while (index++ < length)
            {
                sb.Append(charsAndDigits[
                    random.Next(charsAndDigits.Length)]);
            }
            return sb.ToString();
        }
    }
}
