using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PlataformaEducacao.GestaoIdentidade.Api.Data
{
    public class GestaoIdentidadeContext(DbContextOptions<GestaoIdentidadeContext> options) : IdentityDbContext(options)
    { }
}
