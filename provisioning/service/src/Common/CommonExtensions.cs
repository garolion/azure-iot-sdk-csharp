// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;

//    using Microsoft.Azure.Devices.Common.WebApi;
//#if !WINDOWS_UWP && !NETSTANDARD1_3
//    using Microsoft.Owin;
//#endif

namespace Microsoft.Azure.Devices.Common
{
    internal delegate bool TryParse<TInput, TOutput>(TInput input, bool ignoreCase, out TOutput output);

    internal static class CommonExtensionMethods
    {
        private const char ValuePairDelimiter = ';';
        private const char ValuePairSeparator = '=';

        public static string EnsureEndsWith(this string value, char suffix)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var length = value.Length;
            if (length == 0)
            {
                return suffix.ToString();
            }
            else if (value[length - 1] == suffix)
            {
                return value;
            }
            else
            {
                return value + suffix;
            }
        }

        public static string EnsureStartsWith(this string value, char prefix)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (value.Length == 0)
            {
                return prefix.ToString();
            }
            else
            {
                return value[0] == prefix ? value : prefix + value;
            }
        }

        public static string GetValueOrDefault(this IDictionary<string, string> map, string keyName)
        {
            string value;
            if (!map.TryGetValue(keyName, out value))
            {
                value = null;
            }

            return value;
        }

        public static IDictionary<string, string> ToDictionary(this string valuePairString, char kvpDelimiter, char kvpSeparator)
        {
            if (string.IsNullOrWhiteSpace(valuePairString))
            {
                throw new ArgumentException("Malformed Token");
            }

            IEnumerable<string[]> parts = valuePairString.Split(kvpDelimiter).
                Select((part) => part.Split(new char[] { kvpSeparator }, 2));

            if (parts.Any((part) => part.Length != 2))
            {
                throw new FormatException("Malformed Token");
            }

            IDictionary<string, string> map = parts.ToDictionary((kvp) => kvp[0], (kvp) => kvp[1], StringComparer.OrdinalIgnoreCase);

            return map;
        }

//#if !WINDOWS_UWP && !NETSTANDARD1_3
//        public static string GetMaskedClientIpAddress(this HttpRequestMessage requestMessage)
//        {
//            // note that this only works if we are hosted as an OWIN app
//            if (requestMessage.Properties.ContainsKey("MS_OwinContext"))
//            {
//                OwinContext owinContext = requestMessage.Properties["MS_OwinContext"] as OwinContext;
//                if (owinContext != null)
//                {
//                    string remoteIpAddress = owinContext.Request.RemoteIpAddress;

//                    string maskedRemoteIpAddress = string.Empty;

//                    IPAddress remoteIp = null;
//                    IPAddress.TryParse(remoteIpAddress, out remoteIp);

//                    if (null != remoteIp)
//                    {
//                        byte[] addressBytes = remoteIp.GetAddressBytes();
//                        if (remoteIp.AddressFamily == AddressFamily.InterNetwork)
//                        {
//                            addressBytes[addressBytes.Length - 1] = 0xFF;
//                            addressBytes[addressBytes.Length - 2] = 0xFF;
//                            maskedRemoteIpAddress = new IPAddress(addressBytes).ToString();
//                        }
//                        else if (remoteIp.AddressFamily == AddressFamily.InterNetworkV6)
//                        {
//                            addressBytes[addressBytes.Length - 1] = 0xFF;
//                            addressBytes[addressBytes.Length - 2] = 0xFF;
//                            addressBytes[addressBytes.Length - 3] = 0xFF;
//                            addressBytes[addressBytes.Length - 4] = 0xFF;
//                            maskedRemoteIpAddress = new IPAddress(addressBytes).ToString();
//                        }

//                    }

//                    return maskedRemoteIpAddress;
//                }
//            }

//            return null;
//        }
//#endif

        public static void AppendKeyValuePairIfNotEmpty(this StringBuilder builder, string name, object value)
        {
            if (value != null)
            {
                builder.Append(name);
                builder.Append(ValuePairSeparator);
                builder.Append(value);
                builder.Append(ValuePairDelimiter);
            }
        }

        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static string RemoveWhitespace(this string value)
        {
            return new string(value.Where(c => !char.IsWhiteSpace(c)).ToArray());
        }
    }
}
