using SqlSugar;

namespace FytSoa.Core
{
    public class MySugarTable : SugarTable
    {
        public MySugarTable(string tableName) : base(tableName.ToLower())
        {
        }
    }
}
