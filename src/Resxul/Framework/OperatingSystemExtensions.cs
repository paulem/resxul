using System;
using System.Collections.Generic;
using System.Text;

namespace Resxul.Framework
{
    public static class OperatingSystemExtensions
    {
        public static bool IsWin8OrHigher(this OperatingSystem operatingSystem)
        {
            return operatingSystem.Version.Major > 6 || operatingSystem.Version.Major == 6 && operatingSystem.Version.Minor >= 2;
        }

        public static bool IsWin8(this OperatingSystem operatingSystem)
        {
            return operatingSystem.Version.Major == 6 && operatingSystem.Version.Minor == 2;
        }

        public static bool IsWin10Rs4OrHigher(this OperatingSystem operatingSystem)
        {
            return operatingSystem.IsWin10OrHigher() && operatingSystem.Version.Build >= 17134;
        }

        public static bool IsWin10Rs5OrHigher(this OperatingSystem operatingSystem)
        {
            return operatingSystem.IsWin10OrHigher() && operatingSystem.Version.Build >= 17763;
        }

        public static bool IsWin10Rs6OrHigher(this OperatingSystem operatingSystem)
        {
            return operatingSystem.IsWin10OrHigher() && operatingSystem.Version.Build >= 18204;
        }

        public static bool IsWin10OrHigher(this OperatingSystem operatingSystem)
        {
            return operatingSystem.Version.Major >= 10;
        }

        public static bool IsVistaOrHigher(this OperatingSystem operatingSystem)
        {
            return operatingSystem.Version.Major >= 6;
        }

        public static bool IsWin7OrHigher(this OperatingSystem operatingSystem)
        {
            return operatingSystem.Version.Major > 6 || operatingSystem.Version.Major == 6 && operatingSystem.Version.Minor >= 1;
        }

        public static bool IsWin7(this OperatingSystem operatingSystem)
        {
            return operatingSystem.Version.Major == 6 && operatingSystem.Version.Minor == 1;
        }
    }
}
