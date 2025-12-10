using TestSwaggerAPI.Models;
using TestSwaggerAPI.Helpers;

namespace TestSwaggerAPI.Services;

public class FakeExternalService : IFakeExternalService
{   
    public static GetExternalValue ToGev(WorkCalendar r) =>
        new (r.Year, r.Month, r.Name, r.WorkDay);

    public Task<GetExternalValue> ExternalValue(int year, int month)
    {
        var value = $"Name on {month}.{year}";
        int workDays = CalculatorWorkingDays.CalculatorWorkDays(year,month);
        string workingDay = $"{workDays} рабочих дней";
    
        return Task.FromResult(new GetExternalValue(year, month, value, workingDay));
    }
}