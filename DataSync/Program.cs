using System.Runtime.InteropServices;
using OpenDBDiff.Abstractions.Schema;
using OpenDBDiff.Abstractions.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Generates;
using OpenDBDiff.SqlServer.Schema.Model;
using OpenDBDiff.SqlServer.Schema.Options;

namespace DataSync;
class Program
{
    private static SqlOption SqlFilter = new SqlOption();

    /// <summary>
    /// Sample
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");


        SqlFilter.Ignore.FilterColumnCollation = false;
        SqlFilter.Ignore.FilterUsers = false;
        SqlFilter.Ignore.FilterPartitionScheme = false;
        SqlFilter.Ignore.FilterXMLSchema = false;
        SqlFilter.Ignore.FilterTableLockEscalation = false;
        SqlFilter.Ignore.FilterTableFileGroup = false;
        SqlFilter.Ignore.FilterSynonyms = false;
        SqlFilter.Ignore.FilterRules = false;
        SqlFilter.Ignore.FilterRoles = false;
        SqlFilter.Ignore.FilterPartitionScheme = false;
        SqlFilter.Ignore.FilterNotForReplication = false;
        SqlFilter.Filters.Items.Add(new SqlOptionFilterItem() { ObjectType = OpenDBDiff.Abstractions.Schema.ObjectType.Schema, FilterPattern = "DBA" });
        string s1 = "";
        string s2 = "";
        string dbName1 = "";
        string dbName2 = "";
        string user1 = "";
        string user2 = "";
        string pass1 = "";
        string pass2 = "";
        var options = new CommandlineOptions
        {
            Source = $"Server={s1};Database={dbName1};User Id={user1};Password={pass1};",
            Destination = $"Server={s2};Database={dbName2};User Id={user2};Password={pass2};"
        };

        Database origin;
        Database destination;

        Generate sql = new Generate();
        sql.ConnectionString = options.Destination;
        Console.WriteLine("Reading first database...");
        sql.Options = SqlFilter;
        origin = sql.Process();

        sql.ConnectionString = options.Source;
        Console.WriteLine("Reading second database...");
        destination = sql.Process();
        Console.WriteLine("Comparing databases schemas...");


        Database originClone = (Database)origin.Clone(null);


        origin = Generate.Compare(origin, destination);
        var snc = origin.ToSqlDiff(new System.Collections.Generic.List<ISchemaBase>(), originClone, destination);

        int ROW = 1;
        // It has been categorized for ease of use in visual representation.
        var finalle = snc.list.Where(t => !string.IsNullOrEmpty(t.SQL)).GroupBy(t => t.SCHEMA).Select(t =>
        new TreeData
        {
            id = (ROW++).ToString(),
            Name = t.Key.ToString(),
            children = t.GroupBy(x => x.OBJECTTYPE).Select(x => new TreeData
            {
                id = (ROW++).ToString(),
                Name = x.Key.ToString(),
                children = x.GroupBy(xy => xy.StatusNew).Select(t => new TreeData
                {
                    id = (ROW++).ToString(),
                    Name = t.Key.ToString(),
                    children = t.Select(x => new TreeData
                    {
                        id = (ROW++).ToString(),
                        Name = $"{x.SCHEMA} - {x.OBJECTNAME} {(!string.IsNullOrEmpty(x.OBJECTSUBNAME) ? (" -> " + x.OBJECTSUBNAME) : "")}",
                        Data = x,
                        isRow = true
                    }).ToList()
                }).ToList()
            }).ToList()
        }).ToList();

        string JSON = Newtonsoft.Json.JsonConvert.SerializeObject(finalle);

        Console.ReadLine();
    }
}

public class CommandlineOptions
{
    /// <summary>
    /// Target Database Connection
    /// </summary>
    public string Destination { get; set; }
    /// <summary>
    /// Source Database Connection
    /// /// </summary>
    public string Source { get; set; }
}

/// <summary>
/// UI View MUI
/// UI sample https://stackblitz.com/edit/react-ts-4txpa7?file=package.json,App.tsx
/// </summary>
public class TreeData
{
  public string id { get; set; }
  public string Name { get; set; }
  public List<TreeData> children { get; set; }
  public SQLScript? Data { get; set; }
  public bool? isRow { get; set; }
}

 