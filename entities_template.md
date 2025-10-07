
# Prompt für die Generierung von Entitäten, Validierungsklassen und Views

Du bist ein **C#-Experte mit .NET 8.0**.
Wenn du in diesem Projekt eine Entität erstellst, dann halte dich bitte bei der Implementierung an die folgende Struktur und Konventionen:

## Struktur für Entitäten

- Erstelle die Klasse mit dem Modifier *public* und *partial*.
- Die Klasse erbt von `EntityObject`.  
- Dateiname: **EntityName.cs**.  

```csharp
//@AiCode
namespace SolutionName.Logic.Entities[.SubFolder]
{
    using System.... // Required usings

#if SQLITE_ON
    [Table("EntityNames")]
#else
    [Table("EntityNames", Schema = "app")]
#endif
    [Index(...)] // Add index attributes if required.
    public partial class EntityName : EntityObject 
    {
        #region fields
        private PropertyType _fullPropertyName = default!; // Initialize only if necessary
        #endregion fields

        #region properties
        /// <summary>
        /// Example of an auto property.
        /// </summary>
        public PropertyType AutoPropertyName { get; set; } = default!; // Initialize only if necessary

        /// <summary>
        /// Example of a full property.
        /// </summary>
        public PropertyType FullPropertyName 
        { 
            get
            {
                var result = _fullPropertyName;
                OnFullPropertyNameReading(ref result);
                return result;
            }
            set
            {
                bool handled = false;
                OnFullPropertyNameChanging(ref handled, value);
                if (!handled)
                {
                    _fullPropertyName = value;
                }
                OnFullPropertyNameChanged(_fullPropertyName);
            }
        }
        #endregion properties

        #region navigation properties
        #endregion navigation properties

        #region partial methods
        partial void OnFullPropertyNameReading(ref PropertyType value);
        partial void OnFullPropertyNameChanging(ref bool handled, PropertyType value);
        partial void OnFullPropertyNameChanged(PropertyType value);
        #endregion partial methods
    }
}
```

## Struktur für Validierungsklassen

- Lege eine separate *partial* Klasse für die Validierung im **gleichen Namespace** wie die Entität an.  
- Die Klasse implementiert `IValidatableEntity`.  
- Dateiname: **EntityName.Validation.cs**.  
- Erkennbare Validierungsregeln aus der Beschreibung müssen implementiert werden.

```csharp
//@AiCode
namespace SolutionName.Logic.Entities[.SubFolder]
{
    using System.... // Required usings
    using SolutionName.Logic.Modules.Exceptions;

    partial class EntityName : SolutionName.Logic.Contracts.IValidatableEntity 
    {
        public void Validate(SolutionName.Logic.Contracts.IContext context, EntityState entityState)
        {
            bool handled = false;
            BeforeExecuteValidation(ref handled, context, entityState);

            if (!handled)
            {
                // Implement validation logic here
                if (!IsPropertyNameValid(PropertyName))
                {
                    throw new BusinessRuleException(
                        $"The value of {nameof(PropertyName)} '{PropertyName}' is not valid.");
                }
            }
        }
        
        #region methods
        public static bool IsPropertyNameValid(PropertyType value)
        {
            // Implement validation logic here
            return true;
        }
        #endregion methods

        #region partial methods
        partial void BeforeExecuteValidation(ref bool handled, SolutionName.Logic.Contracts.IContext context, EntityState entityState);
        #endregion partial methods
    }
}
```

## Validierungsregeln

- Keine Validierungen für Id-Felder (werden von der Datenbank verwaltet).

## Struktur für Views

```csharp
//@AiCode
namespace SolutionName.Logic.Entities.Views[.SubFolder]
{
    using System.... // Required usings

    [CommonModules.Attributes.View("ViewNames")]
    public partial class ViewName : ViewObject 
    {
        #region properties
        #endregion properties

        #region navigation properties
        #endregion navigation properties
    }
}
```

## Using-Regeln

- `using System` wird **nicht** explizit angegeben.

## Entity-Regeln

- Kommentar-Tags (`/// <summary>` usw.) sind für jede Entität erforderlich.  
- `SolutionName.Logic` ist fixer Bestandteil des Namespace.  
- `[.SubFolder]` ist optional und dient der Strukturierung.

## Property-Regeln

- Primärschlüssel `Id` wird von `EntityObject` geerbt.  
- **Auto-Properties**, wenn keine zusätzliche Logik benötigt wird.  
- **Full-Properties**, wenn Lese-/Schreiblogik erforderlich ist.  
- Für Id-Felder: Typ `IdType`.  
- Bei Längenangabe: `[MaxLength(n)]`.  
- Nicht-nullable `string`: `= string.Empty`.  
- Nullable `string?`: keine Initialisierung.

## Navigation Properties-Regeln

- In der Many-Entität: `EntityNameId`.  
- Navigation Properties immer vollqualifiziert:  
  `ProjectName.Entities.EntityName EntityName`  
- **1:n**:

```csharp
  public List<Type> EntityNames { get; set; } = [];
```  

- **1:1 / n:1**:  

```csharp
  Type? EntityName { get; set; }
```

## Dokumentation

- Jede Entität und Property erhält englische XML-Kommentare.

**Beispiel:**

```csharp
/// <summary>
/// Name of the entity.
/// </summary>
public string Name { get; set; } = string.Empty;
```

## Ausgabeformat

- Jede Entität wird in einem separaten Canvas-Block mit eigenem Namespace ausgegeben:

```csharp
//@AiCode
namespace SolutionName.Logic.Entities[.SubFolder]
{
    using System.... // Required usings

    [Table("Entity1")]
    public partial class Entity1 : EntityObject 
    {
        #region properties
        #endregion properties

        #region navigation properties
        #endregion navigation properties
    }
}

//@AiCode
namespace SolutionName.Logic.Entities[.SubFolder]
{
    using System.... // Required usings

    [Table("Entity2")]
    public partial class Entity2 : EntityObject 
    {
        #region properties
        #endregion properties

        #region navigation properties
        #endregion navigation properties
    }
}
```
