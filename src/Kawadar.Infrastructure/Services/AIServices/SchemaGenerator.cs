namespace Kawadar.Infrastructure.Services.AIServices;

using System.Reflection;
using System.Text.Json.Serialization;
using Google.GenAI.Types;

public static class SchemaGenerator
{
  public static Schema FromType<T>()
  {
    // Option A: manual mapping per type (most control)
    // Option B: reflect over T's properties automatically
    return BuildSchema(typeof(T));
  }

  private static Schema BuildSchema(System.Type type)
  {
    var underlyingType = Nullable.GetUnderlyingType(type);
    if (underlyingType != null)
    {
      var schema = BuildSchema(underlyingType);
      schema.Nullable = true;
      return schema;
    }

    if (type == typeof(string)) return new Schema { Type = Google.GenAI.Types.Type.String };
    if (type == typeof(int) || type == typeof(long)) return new Schema { Type = Google.GenAI.Types.Type.Integer };
    if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return new Schema { Type = Google.GenAI.Types.Type.Number };
    if (type == typeof(bool)) return new Schema { Type = Google.GenAI.Types.Type.Boolean };

    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
    {
      var itemType = type.GetGenericArguments()[0];
      return new Schema { Type = Google.GenAI.Types.Type.Array, Items = BuildSchema(itemType) };
    }

    // Object — reflect properties
    var properties = new Dictionary<string, Schema>();
    foreach (var prop in type.GetProperties())
    {
      var jsonName = prop.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name
                     ?? ToCamelCase(prop.Name);
      properties[jsonName] = BuildSchema(prop.PropertyType);
    }

    return new Schema
    {
      Type = Google.GenAI.Types.Type.Object,
      Properties = properties
    };
  }

  private static string ToCamelCase(string name) =>
      string.IsNullOrEmpty(name) ? name : char.ToLower(name[0]) + name[1..];
}