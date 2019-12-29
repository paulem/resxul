using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Resxul.Models
{
    public static class ProfileUtils
    {
        public static Profile ResolveVariables(this Profile profile, Variables variables)
        {
            var properties = profile.GetType().GetProperties().Where(x => x.PropertyType == typeof(string) && Attribute.IsDefined(x, typeof(DataMemberAttribute)));

            foreach (var property in properties)
            {
                if (property.GetValue(profile) is string value && !string.IsNullOrEmpty(value))
                    property.SetValue(profile, variables.ResolveVariables(Environment.ExpandEnvironmentVariables(value)));
            }

            return profile;
        }

        public static void CopyTo(this Profile profile, Profile anotherProfile)
        {
            var anotherProfileType = anotherProfile.GetType();

            var properties = profile.GetType().GetProperties().Where(x => x.PropertyType == typeof(string) && Attribute.IsDefined(x, typeof(DataMemberAttribute)));

            foreach (var property in properties)
            {
                if (property.GetValue(profile) is string value && !string.IsNullOrEmpty(value))
                    anotherProfileType.GetProperty(property.Name)?.SetValue(anotherProfile, value);
            }
        }

        public static bool IsResolved(this Profile profile)
        {
            var variableMarkers = new[] { "%", "{", "}" };

            var properties = profile.GetType().GetProperties().Where(x => x.PropertyType == typeof(string) && Attribute.IsDefined(x, typeof(DataMemberAttribute)));
            var notEmptyStrings = properties.Select(x => (string)x.GetValue(profile)).Where(x => !string.IsNullOrEmpty(x)).ToList();

            return notEmptyStrings.All(x => !variableMarkers.Any(x.Contains));
        }

        public static bool IsReady(this Profile profile)
        {
            return IsResolved(profile);
        }
    }
}
