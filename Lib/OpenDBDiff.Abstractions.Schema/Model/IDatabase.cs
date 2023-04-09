using System.Collections.Generic;

namespace OpenDBDiff.Abstractions.Schema.Model
{
    public interface IDatabase : ISchemaBase
    {
        bool IsCaseSensitive { get; }
        SqlAction ActionMessage { get; }
        IOption Options { get; }

        //new SQLScriptList ToSqlDiff(ICollection<ISchemaBase> selectedSchemas);


        SQLScriptList ToSqlDiff(ICollection<ISchemaBase> schemas, IDatabase origin, IDatabase destination);

       // new SQLScriptList ToSqlDiff(ICollection<ISchemaBase> schemas, IDatabase origin, IDatabase destination);
        ISchemaBase Find(string objectFullName);
    }
}
