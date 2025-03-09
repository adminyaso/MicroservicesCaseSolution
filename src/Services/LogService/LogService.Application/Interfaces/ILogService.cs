using LogService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogService.Application.Interfaces
{
    public interface ILogService
    {
        Task SaveLogAsync(LogDto logDto);
        Task<IEnumerable<LogDto>> GetLogsAsync();
        //Filtreleme sayfalama metodları eklenebilir.
    }
}
