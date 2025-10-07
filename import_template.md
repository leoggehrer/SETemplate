# Prompt für die Generierung des CSV-Import

Du bist ein **C#-Experte mit .NET 8.0**.
Wenn du in diesem Projekt einen CSV-Importer erstellst, dann halte dich bitte bei der Implementierung an die folgende Struktur und Konventionen:

## Anforderungen

1. **Namespace:** Der Namespace `SolutionName.ConApp.Apps` ist verpflichtend und darf nicht geändert werden.
2. **Dateiname:** Der Import-Code wird in der Datei `StarterApp.Import.cs` implementiert.
3. **Dateistruktur:** Alle CSV-Dateien werden im Unterordner `data` gespeichert.
4. **CSV-Datei:** Jede Entität hat eine eigene CSV-Datei, benannt nach der Entität, z.B. `entityName_set.csv`.
   - Die erste Zeile der CSV-Datei enthält die Spaltennamen.
   - Die Datenzeilen folgen dem Format: `Property1;Property2;...;PropertyN`.
   - Kommentare in der CSV-Datei beginnen mit `#` und werden ignoriert.
   - Alle CSV-Dateien werden automatisch in das Ausführungsverzeichnis kopiert.
5. **Code-Struktur:**  
   - Die Import-Logik wird als **partial-Methode** in der Klasse `StarterApp` implementiert.  
   - **Jede Entität** erhält ihren eigenen Import-Block in der Datei `Program.Import.cs`.  
6. **Fehlerbehandlung:**  
   - Alle Importvorgänge sind asynchron (`async/await`).  
   - Fehler werden per `try/catch` behandelt, mit Rollback über `RejectChangesAsync()`.  

## Beispielcode: Grundgerüst

```csharp
//@AiCode
#if GENERATEDCODE_ON
namespace SolutionName.ConApp.Apps
{
    partial class StarterApp
    {
        partial void AfterCreateMenuItems(ref int menuIdx, List<MenuItem> menuItems)
        {
            menuItems.Add(new()
            {
                Key = "----",
                Text = new string('-', 65),
                Action = (self) => { },
                ForegroundColor = ConsoleColor.DarkGreen,
            });

            menuItems.Add(new()
            {
                Key = $"{++menuIdx}",
                Text = ToLabelText($"{nameof(ImportData).ToCamelCaseSplit()}", "Started the import of the csv-data"),
                Action = (self) =>
                {
#if DEBUG && DEVELOP_ON
                    ImportData();
#endif
                },
#if DEBUG && DEVELOP_ON
                ForegroundColor = ConsoleApplication.ForegroundColor,
#else
                ForegroundColor = ConsoleColor.Red,
#endif
            });
        }

        private static void ImportData()
        {
            Task.Run(async () =>
            {
                try
                {
                    await ImportDataAsync();
                }
                catch (Exception ex)
                {
                    var saveColor = ForegroundColor;
                    PrintLine();
                    ForegroundColor = ConsoleColor.Red;
                    PrintLine($"Error during data import: {ex.Message}");
                    ForegroundColor = saveColor;
                    PrintLine();
                    ReadLine("Continue with the Enter key...");
                }
            }).Wait();
        }

        private static async Task ImportDataAsync()
        {
            Logic.Contracts.IContext context = CreateContext();
            var filePath = Path.Combine(AppContext.BaseDirectory, "data", "entityName_set.csv");

            foreach (var line in File.ReadLines(filePath).Skip(1).Where(l => !l.StartsWith('#')))
            {
                var parts = line.Split(';');
                var entity = new Logic.Entities.EntityName
                {
                    PropertyName = parts[0],
                    // ... weitere Properties
                };
                try
                {
                    await context.EntityNameSet.AddAsync(entity);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    await context.RejectChangesAsync();
                    Console.WriteLine($"Error in line: {ex.Message}");
                }
            }
        }
    }
}
#endif
```

## Ziel

- Aus dem gegebenen Kontext werden die CSV-Dateien erstellt.  
- Für jede Entität wird der Import-Code nach obigem Muster generiert.  
