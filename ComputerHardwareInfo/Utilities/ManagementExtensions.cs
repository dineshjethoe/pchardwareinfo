using System;
using System.Management;

namespace ComputerHardwareInfo.Utilities
{
    public static class WmiManagementExtensions
    {
        public static ManagementObjectSearcher CreateWmiSearcher(this ManagementScope scope, string wmiQuery)
        {
            return new ManagementObjectSearcher(scope, new ObjectQuery(wmiQuery));
        }

        public static string GetPropertyValueAsString(this ManagementObject obj, string propertyName)
        {
            var value = obj.GetPropertyValue(propertyName);
            return value?.ToString() ?? "N/A";
        }

        public static T GetPropertyValue<T>(this ManagementObject obj, string propertyName, T defaultValue)
        {
            try
            {
                var value = obj.GetPropertyValue(propertyName);
                return value == null ? defaultValue : (T)value;
            }
            catch
            {
                return defaultValue;
            }
        }

        public static void ExecuteWmiQuery(this ManagementScope scope, string wmiQuery, Action<ManagementObject> processResult)
        {
            using (var searcher = scope.CreateWmiSearcher(wmiQuery))
            {
                foreach (ManagementObject result in searcher.Get())
                {
                    processResult(result);
                }
            }
        }
    }
}
