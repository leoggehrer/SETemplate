# GitHub Copilot Instructions für SETemplate

## Projektübersicht

SETemplate ist ein Basis-Template für die Erstellung der Anwendung mit Code-Generierung:
- **Backend**: .NET 8.0 mit Entity Framework Core
- **Frontend**: Angular 18 mit Bootstrap und standalone Komponenten
- **Code-Generierung**: Template-gesteuerte Erstellung aller CRUD-Operationen
- **Architektur**: Clean Architecture mit strikter Trennung von manuellen und generierten Code

## Kernprinzipien

### 1. Authentifizierung und Autorisierung

Im Template sind bereits grundlegende Authentifizierungs- und Autorisierungsmechanismen implementiert. Diese sollten jedoch an die spezifischen Anforderungen der Anwendung angepasst werden. Die Authentifizierung kann über das TemplateTools-Projekt eingestellt werden.

```bash
# Ein- und Ausschalten der Authentifizierung:
dotnet run --project TemplateTools.ConApp -- AppArg=3,2,x,x
```

### 2. Code-Generierung First

**⚠️ NIEMALS manuell Controllers, Services oder CRUD-Operationen erstellen!**

```bash
# Code-Generierung ausführen:
dotnet run --project TemplateTools.ConApp -- AppArg=4,9,x,x
```

### 3. Code-Marker System

- `//@AiCode` - Generierter Code, nicht bearbeiten
- `//@GeneratedCode` - Zeigt an, dass dieser Code vom Generator generiert wurde und bei der nächsten Generierung überschrieben wird.
- `//@CustomCode` - Falls in einer generierten Datei (@GeneratedCode) eine Änderung erfolgt, dann wird der Label @GeneratedCode zu @CustomCode geändert. Damit wird verhindert, dass der Code vom Generator überschrieben wird.
- `#if GENERATEDCODE_ON` - Conditional Compilation für Features

## Entity-Entwicklung

Die Entitäten werden immer mit englischen Bezeichner benannt.

### Dateistruktur
- **Stammdaten**: `SETemplate.Logic/Entities/Data/`
- **Anwendungsdaten**: `SETemplate.Logic/Entities/App/`
- **Account**: `SETemplate.Logic/Entities/Account/`

### Entity Template

- Erstelle die Klasse mit dem Modifier *public* und *partial*.
- Die Klasse erbt von `EntityObject`.  
- Dateiname: **EntityName.cs**.  

```csharp
```

## Struktur für Validierungsklassen

- Lege eine separate *partial* Klasse für die Validierung im **gleichen Namespace** wie die Entität an.  
- Die Klasse implementiert `IValidatableEntity`.  
- Dateiname: **EntityName.Validation.cs**.  
- Erkennbare Validierungsregeln aus der Beschreibung müssen implementiert werden.

```csharp
```

## Validierungsregeln

- Keine Validierungen für Id-Felder (werden von der Datenbank verwaltet).

## Using-Regeln

- `using System` wird **nicht** explizit angegeben.

## Entity-Regeln

- Kommentar-Tags (`/// <summary>` usw.) sind für jede Entität erforderlich.  
- `SETemplate.Logic` ist fixer Bestandteil des Namespace.  
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

## Angular Komponenten

Die Generierung der Komponenten erfolgt für die Listen die sich im Ordner 'src/app/pages/entities/' befinden. Die dazugehörigen Edit Komponenten befinden sich im Ordner 'src/app/components/entities'.

### List Component Template
```html
```

### Bearbeitungsansicht (Edit-Formular)

* Für die Ansicht ist eine **Bootstrap-Card-Ansicht** zu verwenden.
* Die Komponenten sind bereits erstellt und befinden sich im Ordner `src/app/components/entities`.
* Alle Komponenten sind `standalone` Komponenten.
* **Dateiname:** `entity-name-edit.component.html`
* **Übersetzungen:** Ergänze die beiden Übersetzungsdateien `de.json` und `en.json` um die hinzugefügten Labels.
* Beispielstruktur:

```html
```

### Master-Details

In manchen Fällen ist eine Master/Details Ansicht sehr hilfreich. Diese Anzeige besteht aus einer Master-Ansicht. Diese kann nicht bearbeitet werden. Die Details zu diesem Master werden unter der Master-Ansicht als 'List-group' angezeigt. Die Generierung soll nur nach Aufforderung des Benutzers erfolgen. Nachfolgend ist die Struktur skizziert:

```html
```

## Entwicklungs-Workflow

### 1. Authentifizierung einstellen
1. Die Standard-Einstellung ist ohne Authentifizierung. 
2. Frage den Benutzer, ob Authentifizierung benötigt wird.
3. Authentifizierung ausführen: `dotnet run --project TemplateTools.ConApp -- AppArg=3,2,x,x`

### 2. Entity erstellen
1. Entity-Klasse in `Logic/Entities/{Data|App}/` erstellen
2. Validierung in separater `.Validation.cs` Datei
3. Das Entity-Modell mit dem Benutzer abklären und bestätigen lassen.

### 2. Code-Generierung
1. Code-Generierung ausführen: `dotnet run --project TemplateTools.ConApp -- AppArg=4,9,x,x`

### 3. Daten-Import
1. CSV-Datei in `ConApp/data/entityname_set.csv` erstellen
2. Einstellen, dass die CSV-Datei ins Ausgabeverzeichnis kopiert wird
3. Import-Logic in `StarterApp.Import.cs` hinzufügen
4. Console-App ausführen und Import starten

### 4. Datenbank erstellen und Import starten
1. Code-Generierung ausführen: `dotnet run --project SETemplate.ConApp -- AppArg=1,2,x`

### 5. Angular-Komponenten
1. Erstelle für alle Entitäten die List-Komponente
   - Die List-Komponenten wurden vom Generator erstellt und befinden sich im Ordner 'src/app/pages/entities/'
   - Immer mit HTML-Templates in einer separaten Datei arbeiten.
   - Immer mit CSS-Templates in einer separaten Datei arbeiten.
2. Erstelle für alle List-Komponenten das Routing in `app-routing.module.ts`.
3. Trage alle List-Komponenten in das Dashboard für die Navigation ein.
4. Erstelle für alle Entitäten die Edit-Komponente
   - Die Edit-Komponenten wurden vom Generator erstellt und befinden sich im Ordner 'src/app/components/entities/'
   - Verweise auf andere Entities als Dropdowns umsetzen.
   - Immer mit HTML-Templates in einer separaten Datei arbeiten.
   - Immer mit CSS-Templates in einer separaten Datei arbeiten.
4. Übersetzungen in `de.json` und `en.json` ergänzen

### 6. Anpassungen
- Custom Code in `//@CustomCode` Bereichen
- Separate `.Custom.cs` Dateien für erweiterte Logik
- `editMode` boolean für Create/Edit-Unterscheidung

### 7. Änderungen und Erweiterungen
- Änderungen die die Entitäten betreffen
  - Zuerst die generierten Klassen entfernen:
    1. Delete generierte Klassen: 
    `dotnet run --project TemplateTools.ConApp -- AppArg=4,7,x,x`
- Dannach starte wieder beim Workflow bei Punkt 1.

## Konventionen

### Naming
- Entities: PascalCase, Englisch
- Properties: PascalCase mit XML-Dokumentation
- Navigation Properties: Vollqualifiziert

### Validierung
- Keine Validierung für Id-Felder
- BusinessRuleException für Geschäftsregeln
- Async-Pattern mit RejectChangesAsync()

### Internationalisierung
- Alle Labels in i18n-Dateien
- Format: `ENTITYNAME_LIST.TITLE`
- Unterstützung für DE/EN

## Troubleshooting

### Häufige Probleme
- **Build-Fehler**: Code-Generierung ausführen nach Entity-Änderungen
- **Import-Fehler**: CSV-Format und Beziehungen prüfen
- **Routing**: Komponenten in `app-routing.module.ts` registrieren

### Debugging
- Generated Code über `//@AiCode` Marker identifizieren
- Custom Code in separaten Bereichen isolieren
- Console-App für Datenbank-Tests nutzen
