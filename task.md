# SETemplate

## Projektbeschreibung

Kurze allgemeine Beschreibung des Projektes.

### Anforderungen

Hier die zusätzliche Anforderungen definieren.

### Technische Details

Setze die Anforderungen in folgenden Schritten um:

1. **Datenmodell erstellen**:
   - Definiere die entsprechenden Entitäten mit den entsprechenden Einschränkungen. Halte dich bei der Umsetzung der Entitäten an die Vorgaben im Dokument `entities_template.md`.
   - Entitäten für Bewegungsdaten erstelle im Unterordner `App`
   - Alle anderen Entitäten erstelle im Unterordner `Data`
2. **Entity-Modell prüfen**:
   - Lass uns gemeinsam das Entity-Modell überprüfen. Erst wenn das Entity-Modell abgenommen ist, kannst du mit den nächsten Schritten fortfahren. 
3. **CodeGenerierung ausführen**:
    - Führe die CodeGenerierung mit folgendem Befehl aus: dotnet run --project TemplateTools.ConApp -- AppArg=4,9,x,x
4. **CSV-Import erstellen**:
   - Erstelle eine Klasse, die den Import von CSV-Dateien ermöglicht. Halte dich bei der Umsetzung des Imports an die Vorgaben im Dokument `import_template.md`.
5. **Benutzeroberfläche erstellen**:
   - Erstelle eine einfache Benutzeroberfläche, die es ermöglicht, die Daten im System zu verwalten. Halte dich bei der Umsetzung der Entitäten an die Vorgaben im Dokument `forms_template.md`.
6. **Routing-Module**:
   - Trage alle Komponenten vom Ordner `src/app/pages/entities` in das Routing-Module ein.
7. **Dashboard anpassen**:
   - Passe das Dashboard an und trage die Komponenten vom Ordner `src/app/pages/entities` in das dashboard ein.
8. **README erstellen**:
   - Erstelle eine README-Datei, die das Projekt beschreibt und Anweisungen zur Installation und Nutzung enthält. Halte dich bei der Umsetzung der README an die Vorgaben im Dokument `readme_template.md`.

**Viel Erfolg bei der Umsetzung des Projekts!**
