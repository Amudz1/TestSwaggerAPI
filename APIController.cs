using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

[ApiController]
[Route("TestSwagger/[controller]")]

public class APIController : ControllerBase
{
    private readonly ApplicationContext db;
    private readonly IFakeExternalService fes;
    private readonly IMemoryCache memoryCache;
    
    public APIController(ApplicationContext context, IFakeExternalService fakeExternalService, IMemoryCache cache)
    {
        db = context;
        fes = fakeExternalService;
        memoryCache = cache;
    }
  
    [HttpGet] //------ ПОЛУЧИТЬ ВСЕ ДАННЫЕ ИЗ БД ---------
    public async Task<ActionResult<IEnumerable<DateInformation>>> Get()
    {   
        try
        {
            var allDataBase =  await db.DateInformations.ToListAsync();
            return Ok(allDataBase);
        }
        catch
        {
            return BadRequest("Вероятнее всего, нет доступа к базе данных.");
        }
    }

    [HttpGet("{year:int}/{month:int}")] // ------- ПОЛУЧИТЬ ДАННЫЕ ИЗ БД\ВНЕШНИЙ СЕРВИС\КЕШИРОВАНИЕ ---------
    public async Task<ActionResult<IEnumerable<DateInformation>>> Get(int year, int month)
    {
        try
        {
            string key = $"{month}-{year}";

            if(memoryCache.TryGetValue(key, out DateInformation? cached)) return Ok(cached);

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

            // var datafes = await fes.ExternalValue(year,month);
            // var entity = new DateInformation
            // {
            //     Year = datafes.year,
            //     Month = datafes.month,
            //     Name = datafes.value,
            //     WorkDay = datafes.workDay
            // };
            
            // db.DateInformations.Add(entity);
            // await db.SaveChangesAsync();

            // memoryCache.Set(key,entity, TimeSpan.FromMinutes(10));
            // return Ok(entity);

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

    [HttpPost("{year:int}/{month:int}")] // ------- УСТАНОВИТЬ ЗНАЧЕНИЕ --------
    public async Task<ActionResult<IEnumerable<DateInformation>>> Post(int year, int month)
    {
        var validation = ValidationDate(year,month);
        if(validation != null) return validation;
        try
        {
            var createDate = await fes.ExternalValue(year,month);

            var exitDate =  await db.DateInformations
                .SingleOrDefaultAsync(x => x.Year == year && x.Month == month);
            
            if (exitDate is null)
            {
                exitDate = new DateInformation
                {
                    Year = year,
                    Month = month,
                    Name = createDate.Value,
                    WorkDay = createDate.WorkDay
                };
                db.DateInformations.Add(exitDate);
            }else
            {   
                return Conflict($"Дата {month}.{year} уже существует.");
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

    private ActionResult? ValidationDate(int year, int month)
    {
        if(year < 1900 || year > 2100) 
            return BadRequest($"Год не должен быть меньше 1900 и больше 2100 на момент {DateTime.Now}");
        if(month < 1 || month > 12)
            return BadRequest($"Месяц не может быть меньше 1 и больше 12 на момент {DateTime.Now}");
        return null;
    }
}

