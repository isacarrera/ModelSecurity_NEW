using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Context;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class TruncateDbData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TruncateDbData> _logger;

        public TruncateDbData(ApplicationDbContext context, ILogger<TruncateDbData> logger)
        {
            _context = context;
            _logger = logger;
        }

        // En FormModuleData.cs
        public async Task<bool> TruncateDatabaseAsync()
        {
            try
            {
                // Deshabilitar todas las restricciones de clave foránea
                string disableConstraintsQuery = @"
            EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'";

                // Truncar todas las tablas (incluyendo las relacionadas)
                string truncateQuery = @"
            EXEC sp_MSforeachtable 'DELETE FROM ?'";

                // Restablecer los contadores de identidad
                string resetIdentityQuery = @"
            EXEC sp_MSforeachtable 'DBCC CHECKIDENT(''?'', RESEED, 0)'";

                // Habilitar nuevamente las restricciones
                string enableConstraintsQuery = @"
            EXEC sp_MSforeachtable 'ALTER TABLE ? CHECK CONSTRAINT ALL'";

                //await _context.ExecuteNonQueryAsync(disableConstraintsQuery);
                //await _context.ExecuteNonQueryAsync(truncateQuery);
                //await _context.ExecuteNonQueryAsync(resetIdentityQuery);
                //await _context.ExecuteNonQueryAsync(enableConstraintsQuery);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al truncar la base de datos");
                return false;
            }
        }
    }
}
