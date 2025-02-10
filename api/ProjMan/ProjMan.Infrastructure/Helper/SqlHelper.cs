using ProjMan.Application.Attributes;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace ProjMan.Infrastructure.Helper;

public static class SqlHelper
{
    public static string GenerateWhere<T>()
    {
        var sql = string.Empty;

        PropertyInfo[] propCollection = typeof(T).GetProperties();
        foreach (PropertyInfo property in propCollection)
        {
            foreach (var attribute in property.GetCustomAttributes(true))
            {
                var fieldName = property.Name;
                if (attribute is ColumnAttribute)
                {
                    fieldName = (attribute as ColumnAttribute)?.Name ?? string.Empty;
                }

                if (attribute is SearchableAttribute)
                {
                    if (string.IsNullOrEmpty(sql))
                    {
                        sql = " AND (";
                    }
                    else
                    {
                        sql += " OR";
                    }

                    sql += string.Format(" LOWER({0}) LIKE @search", fieldName.Enquote());
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(sql)) sql += ") ";
        return sql;
    }


    public static string GenerateSort(string sortField, int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(sortField)) return string.Empty;

        string order = "ASC";
        if (sortOrder == -1)
        {
            order = "DESC";
        }

        return $"{sortField.Enquote()} {order}";
    }
}
