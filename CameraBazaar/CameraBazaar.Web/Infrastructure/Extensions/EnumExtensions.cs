namespace CameraBazaar.Web.Infrastructure.Extensions
{
    using CameraBazaar.Data.Models;

    public static class EnumExtensions
    {
        public static string ToDisplayName(this LightMetering lightMetering)
        {
            if (lightMetering == LightMetering.CenterWeighted)
            {
                return "Center weighted";
            }

            return lightMetering.ToString();
        }
    }
}
