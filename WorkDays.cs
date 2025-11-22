
public static class CalculatorWorkingDays
{
    public static int CalculatorWorkDays(int year, int month)
    {
        int daysInMonth = DateTime.DaysInMonth(year,month);
        int workDays = 0;

        for(int day = 1; day <= daysInMonth; day++)
        {
            var date = new DateTime(year,month,day);

            if(date.DayOfWeek != DayOfWeek.Sunday && date.DayOfWeek != DayOfWeek.Saturday) workDays++;
        }
        return workDays;
    }
}