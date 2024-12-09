using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace SmoothGL.Content.Internal;

public static class JsonElementExtension
{
    private const BindingFlags PropertyBindingFlags = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance;

    public static void Populate(this JsonElement jsonElement, object target)
    {
        try
        {
            if (target is IEnumerable<object> enumerableTarget && jsonElement.ValueKind == JsonValueKind.Array)
            {
                jsonElement.PopulateArray(enumerableTarget);
            }
            else if (jsonElement.ValueKind == JsonValueKind.Object)
            {
                jsonElement.PopulateObject(target);
            }
        }
        catch (Exception exception)
        {
            Debug.WriteLine($"Populate failed: {exception.Message}");
        }
    }

    public static void PopulateArray(this JsonElement jsonArrayElement, IEnumerable<object> enumerableTarget)
    {
        var jsonArrayItemsWithTargetItems = jsonArrayElement.EnumerateArray()
            .ZipWithRemainder(enumerableTarget, out var jsonArrayRemainder, out var targetRemainder);

        foreach (var (jsonArrayItem, targetItem) in jsonArrayItemsWithTargetItems)
            jsonArrayItem.Populate(targetItem);

        if (enumerableTarget is IList listTarget)
        {
            foreach (var targetItem in targetRemainder)
                listTarget.Remove(targetItem);

            var itemType = listTarget.GetItemType();
            if (itemType != null)
            {
                foreach (var jsonArrayItem in jsonArrayRemainder)
                    listTarget.Add(jsonArrayItem.Deserialize(itemType, CommonJsonSerializerOptions.CaseInsensitive));
            }
        }
    }

    private static void PopulateObject(this JsonElement jsonObjectElement, object target)
    {
        foreach (var property in jsonObjectElement.EnumerateObject())
            SetProperty(target, property);
    }

    private static void SetProperty(object target, JsonProperty property)
    {
        var propertyInfo = target.GetType().GetProperty(property.Name, PropertyBindingFlags);
        if (propertyInfo != null)
        {
            if (property.Value.ValueKind == JsonValueKind.Object && propertyInfo.CanRead)
            {
                var propertyValue = propertyInfo.GetValue(target);
                if (propertyValue != null)
                    property.Value.Populate(propertyValue);
            }
            else if (propertyInfo.CanWrite)
            {
                var deserializedValue =
                    property.Value.Deserialize(propertyInfo.PropertyType, CommonJsonSerializerOptions.CaseInsensitive);

                propertyInfo.SetValue(target, deserializedValue);
            }
        }
    }
}