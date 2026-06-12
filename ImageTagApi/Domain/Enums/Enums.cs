namespace ImageTagApi.Domain.Enums
{
    public enum FileStatus
    {
        Processing = 0,
        ReadyForTag = 1,
        Completed = 2,
        Error = 3
    }

    public enum TailWindColor
    {
        Red = 0,
        Blue = 1,
        Green = 2,
        Orange = 3,
        Violet = 4,
        Gray = 5 // タグがうまく生成できない場合使用する
    }
}
