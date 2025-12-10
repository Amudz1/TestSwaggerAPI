using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using TestSwaggerAPI.Data;
using TestSwaggerAPI.Models;
using TestSwaggerAPI.Services;
using TestSwaggerAPI.Validators;

namespace TestSwaggerAPI.Controllers;

[ApiController]
[Route("TestSwagger/[controller]")]

public class APIController : ControllerBase
{
    private readonly ApplicationContext db;
    private readonly IFakeExternalService fakeExternalService;
    private readonly IMemoryCache memoryCache;
    
    public APIController(ApplicationContext context, IFakeExternalService fakeExternalService, IMemoryCache cache)
    {
        db = context;
        this.fakeExternalService = fakeExternalService;
        memoryCache = cache;
    }
  
    [HttpGet] //------ ПОЛУЧИТЬ ВСЕ ДАННЫЕ ИЗ БД ---------
    public async Task<ActionResult<IEnumerable<WorkCalendar>>> Get()
    {   
        try
        {
            var GetDatabase =  await db.DateInformations.ToListAsync();
            return Ok(GetDatabase);
        }
        catch
        {
            return BadRequest("Вероятнее всего, нет доступа к базе данных.");
        }
    }

    [HttpGet("{year:int}/{month:int}")] // ------- ПОЛУЧИТЬ ДАННЫЕ ИЗ БД\ВНЕШНИЙ СЕРВИС\КЕШИРОВАНИЕ ---------
    public async Task<ActionResult<IEnumerable<WorkCalendar>>> Get(int year, int month)
    {
        try
        {
            string key = $"{month}-{year}";

            if(memoryCache.TryGetValue(key, out WorkCalendar? cached)) return Ok(cached);

            var datainf = await db.DateInformations
                    .FirstOrDefaultAsync(x => x.Year == year && x.Month == month);
                    
            if(datainf != null)
            {
                memoryCache.Set(key,datainf, TimeSpan.FromMinutes(10));
                return Ok(datainf);
            }
            else
            {
                return BadRequest($"Нет записи на {month}.{year}");
            }

        }
        catch(ArgumentOutOfRangeException)
        {
            return BadRequest("Некорректная дата.");
        }
        catch
        {
            return BadRequest(@"Вы сломали приложение. Ожидайте на почту досудебное письмо
                    и требуем возьместить ущерб за причинение вреда нервной системе и здоровью.");
        }
        
    }

    [HttpPost("process")] // ------- УСТАНОВИТЬ ЗНАЧЕНИЕ --------
    public async Task<ActionResult<IEnumerable<WorkCalendar>>> Post([FromQuery] DateRequest request)
    {
        if (!ModelState.IsValid)
        return BadRequest(ModelState);
        
        try
        {
            var createDate = await fakeExternalService.ExternalValue(request.Year, request.Month);

            var exitDate =  await db.DateInformations
                .SingleOrDefaultAsync(x => x.Year == request.Year && x.Month == request.Month);
            
            if (exitDate is null)
            {
                exitDate = new WorkCalendar
                {
                    Year = request.Year,
                    Month = request.Month,
                    Name = createDate.Value,
                    WorkDay = createDate.WorkDay
                };
                db.DateInformations.Add(exitDate);
            }else
            {   
                return Conflict($"Дата {request.Month}.{request.Year} уже существует.");
            }

            try
            {
                await db.SaveChangesAsync();
                return Ok(exitDate);
            }catch(DbUpdateException)
            {
               return Conflict("Данные были обновлены другим пользователем.");
            }
        }
        catch(ArgumentOutOfRangeException)
        {
            return BadRequest("Введены некорректные данные.");
        }
        catch
        {
            return BadRequest(@"Вы сломали приложение. Ожидайте на почту досудебное письмо
                    и требуем возьместить ущерб за причинение вреда нервной системе и здоровью.");
        }
    }

   
}

