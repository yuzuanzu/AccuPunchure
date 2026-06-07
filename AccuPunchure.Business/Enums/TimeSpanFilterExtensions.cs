namespace AccuPunchure.Business.Enums;

public static class TimeSpanFilterExtensions
{
    public static string Label(this TimeSpanFilter filter) => filter switch
    {
        TimeSpanFilter.OneDay   => "1 Day",
        TimeSpanFilter.OneWeek  => "1 Week",
        TimeSpanFilter.TwoWeeks => "2 Weeks",
        TimeSpanFilter.OneMonth => "1 Month",
        TimeSpanFilter.OneYear  => "1 Year",
        _                       => filter.ToString()
    };
}
