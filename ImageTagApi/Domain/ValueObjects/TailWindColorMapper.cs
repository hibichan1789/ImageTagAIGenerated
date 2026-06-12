using ImageTagApi.Domain.Enums;

namespace ImageTagApi.Domain.ValueObjects
{
    public static class TailWindColorMapper
    {
        public static TailWindColor ToEnum(string color)
        {
            return color switch
            {
                "bg-red-700" => TailWindColor.Red,
                "bg-blue-700" => TailWindColor.Blue,
                "bg-green-700" => TailWindColor.Green,
                "bg-orange-800" => TailWindColor.Orange,
                "bg-violet-700" => TailWindColor.Violet,
                _ => TailWindColor.Gray
            };
        }

        public static string ToCss(TailWindColor color)
        {
            return color switch
            {
                TailWindColor.Red => "bg-red-700",
                TailWindColor.Blue => "bg-blue-700",
                TailWindColor.Green => "bg-green-700",
                TailWindColor.Orange => "bg-orange-800",
                TailWindColor.Violet => "bg-violet-700",
                _ => "bg-gray-700"
            };
        }
    }
}
