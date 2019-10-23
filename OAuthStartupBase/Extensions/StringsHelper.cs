using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace OAuthStartupBase.Extensions {
    public static class StringsHelper {
        /// <summary>
        /// Formats a policy's name
        /// </summary>
        /// <param name="policy">Policy's name</param>
        public static string FormatPolicyName(this string policy) {
            if (!policy.Contains("CORS"))
                return $"{policy}CORS";
            else
                return policy;
        }
        /// <summary>Converts config value to array of strings</summary>
        /// <param name="config">Config value</param>
        public static string[] ConfigStringToCollection(this string config) =>
            config.Split(",");
    }
}