using TestSwaggerAPI.Models;

namespace TestSwaggerAPI.Services;

public interface IFakeExternalService
{
    Task<GetExternalValue> ExternalValue(int year, int month);
}