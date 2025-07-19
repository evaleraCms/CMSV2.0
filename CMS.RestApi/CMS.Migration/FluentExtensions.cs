using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CMS.Infrastructure.Common.Extensions;
using FluentMigrator;
using FluentMigrator.Builders;
using FluentMigrator.Builders.Create.Index;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Builders.Delete;
using FluentMigrator.Builders.Execute;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;
using LinqToDB.Data;

namespace CMS.Migration
{
    public class AlterColumnRequest
    {

        public string TableName { get; set; }
        public string ColumnName { get; set; }
    }

    public class RenameRequest
    {
        public string TableName { get; set; }
        public string OldColumnName { get; set; }
        public string NewColumnName { get; set; }
    }
    public static class FluentExtensions
    {
        public static IFluentSyntax CreateTableIfNotExists(this MigrationBase self, string tableName, Func<ICreateTableWithColumnOrSchemaOrDescriptionSyntax, IFluentSyntax> constructTableFunction, string schemaName = "dbo")
        {
            if (!self.Schema.Schema(schemaName).Table(tableName).Exists())
            {
                return constructTableFunction(self.Create.Table(tableName));
            }
            else
            {
                return null;
            }
        }

        public static IFluentSyntax CreateaIndexIfNotExists(this MigrationBase self, string tableName, string idexName, Func<ICreateIndexOnColumnOrInSchemaSyntax, IFluentSyntax> constructIndexFunction, string schemaName = "dbo")
        {
            if (!self.Schema.Schema(schemaName).Table(tableName).Exists() &&
                 !self.Schema.Schema(schemaName).Table(tableName).Index(idexName).Exists())
            {
                return constructIndexFunction(self.Create.Index(idexName).OnTable(tableName));
            }
            else
            {
                return null;
            }
        }

        public static IFluentSyntax CreateForeignKeyIfNotExists(this MigrationBase self, string tableName, string foreignKeyName, string referenceTable, Dictionary<string, string> columns, string schemaName = "dbo")
        {
            if (!self.Schema.Schema(schemaName).Table(tableName).Exists() &&
                 !self.Schema.Schema(schemaName).Table(tableName).Constraint(foreignKeyName).Exists() &&
                 self.Schema.Schema(schemaName).Table(referenceTable).Exists())
            {
                bool sameId = tableName == referenceTable;
                foreach (var column in columns)
                {
                    if (!self.Schema.Schema(schemaName).Table(tableName).Column(column.Key).Exists())
                    {
                        return null;

                    }
                    if (!self.Schema.Schema(schemaName).Table(tableName).Column(column.Value).Exists())
                    {
                        return null;
                    }
                    if (sameId)
                    {
                        if (column.Key != column.Value)
                        {
                            return null;
                        }
                    }

                    return self.Create.ForeignKey(foreignKeyName)
                        .FromTable(tableName).ForeignColumns(columns.Keys.ToArray())
                        .ToTable(referenceTable).PrimaryColumns(columns.Values.ToArray());

                }
            }
            else
            {
                return null;
            }
            return null;
        }

        public static ICreateTableWithColumnOrSchemaOrDescriptionSyntax CreateTableIfNotExists(this MigrationBase self, string tableName, string schemaName = "dbo")
        {
            if (!self.Schema.Schema(schemaName).Table(tableName).Exists())
            {
                return self.Create.Table(tableName);
            }
            else
            {
                return null;
            }
        }

        public static IInSchemaSyntax DeleteTableIfExists(this FluentMigrator.Migration self, string tableName, string schemaName = "dbo")
        {
            if (self.Schema.Schema(schemaName).Table(tableName).Exists())
            {
                 self.Delete.Table(tableName);
            }
           
            return null;
            
        }
        public static IInSchemaSyntax DeleteIndexIfExists(this FluentMigrator.Migration self, string tableName, string indexName, string[] columns, string schemaName = "dbo")
        {
            
            
            if (self.Schema.Schema(schemaName).Table(tableName).Exists() && 
                self.Schema.Schema(schemaName).Table(tableName).Index(indexName).Exists())
            {
                self.Delete.Index(indexName).OnTable(tableName).OnColumns(columns);
            }
            
            return null;
            
        }
        public static IList<T> ExecuteReader<T>(this FluentMigrator.Migration self, string connectionString, string sqlStatement, Func<IDataReader, T> onRead, params DataParameter[] dataParameters)
        { 
            List<T> records = new List<T>();
            using (var dataContext = new LinqToDB.Data.DataConnection(connectionString))
            { 
                var command = new CommandInfo(dataContext, sqlStatement, dataParameters);
                using (var reader = command.ExecuteReader())
                { 
                    var dataReader = reader.Reader;
                    if (dataReader != null) 
                    { 
                        T record = onRead(dataReader);
                        records.Add(record);
                    }
                }
            
            }
            return records;
        
        }

        public static T ExecuteScaler<T>(this MigrationBase self, string connectionString, string sqlStatement, params DataParameter[] dataParameters)
        {
            T ret;
            using (var dataContext = new LinqToDB.Data.DataConnection(connectionString))
            {
                var command = new CommandInfo(dataContext, sqlStatement,dataParameters);
                ret = command.Execute<T>();
            }
            return ret;
        }

        public static DataTable ExecuteReader(this FluentMigrator.Migration self, string connectionString, string sqlStatement, params DataParameter[] dataParameters)
        {
            DataTable dt = null;
            using (var dataContext = new LinqToDB.Data.DataConnection(connectionString)) 
            {
                dataContext.CommandTimeout = 60 * 3;
                var command = new CommandInfo(dataContext, sqlStatement, dataParameters);

                using (var reader = command.ExecuteReader()) 
                {
                    var dataReader = reader.Reader;
                    int i = 0;
                    dt = new DataTable();
                    for (i = 0; i < dataReader.FieldCount; i++)
                        dt.Columns.Add(dataReader.GetName(i).Trim(), dataReader.GetFieldType(i));

                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        { 
                            DataRow dr = dt.NewRow();
                            for (int j = 0; j < dataReader.FieldCount; j++) 
                            {
                                dr[j] = dataReader[i];
                            }
                            dt.Rows.Add(dr);
                        }
                    
                    }
                }

            }
            return dt;
        
        }

        public static void ExecuteCommand(this FluentMigrator.Migration self, string connectionString, string sqlStatement, params DataParameter[] dataParameters)
        {
            using (var dataContext = new LinqToDB.Data.DataConnection(connectionString))
            {
                dataContext.CommandTimeout = 60 * 10;
                var command = new CommandInfo(dataContext,sqlStatement,dataParameters);
                command.Execute();
            
            }        
        }

        public static T GetRowValue<T>(this IDataReader dr, params string[] fieldNames)
        {
            foreach (var field in fieldNames)
            {
                int countFields = dr.FieldCount;
                for (int i = 0; i < countFields; i++) 
                { 
                    string drFieldName = dr.GetName(i);
                    object value = dr[i];
                    if ((typeof(T) == typeof(string) || !value.IsEmpty()) &&
                        string.Compare(drFieldName,field,StringComparison.OrdinalIgnoreCase)==0) 
                    {
                        Type type = typeof(T);
                        if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable))
                        {
                            type = Nullable.GetUnderlyingType(typeof(T));
                        }
                        return (T)Convert.ChangeType(value, type);
                    }                
                }               

            }
            return default(T);
        }
    }
}
   