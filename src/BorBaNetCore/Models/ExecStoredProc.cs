using Microsoft.EntityFrameworkCore;
using BorBaNetCore.DataModel;
namespace BorBaNetCore.Models
{
    public class ExecStoredProc
    {

        public void execAll()
        {
            BorBaContext ctx = new BorBaContext();
            ctx.Set<Users>().FromSql("dbo.SomeSproc @Id = {0}, @Name = {1}", 45, "Ada");
            //ctx.Database.ExecuteSqlCommand("dbo.SomeSproc @Id = {0}, @Name = {1}", 45, "Ada");
        }
    }  
}
