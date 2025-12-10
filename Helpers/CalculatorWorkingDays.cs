namespace TestSwaggerAPI.Helpers;

public static class CalculatorWorkingDays
{
    public static int CalculatorWorkDays(int year, int month)
    {
        return Enumerable.Range(1, DateTime.DaysInMonth(year, month))
                     .Select(day => new DateTime(year, month, day))
                     .Count(d => d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday);
    }
}