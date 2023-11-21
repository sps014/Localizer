namespace Localizer;

internal record TableColumnInfo
{
    public bool IsVisible { get; set; }
    public string Culture { get; set; }
    public bool IsComment { get; set; }

    public string DisplayValue
    {
        get
        {
            var c = Culture==string.Empty ? "Neutral" : Culture;
            if (IsComment)
            {
                return $"Comment ({c})";
            }

            return c;
        }
    }

    public TableColumnInfo(string culture,bool isComment,bool isVisible)
    {
        Culture = culture;
        IsComment = isComment;
        IsVisible = isVisible;
    }
}