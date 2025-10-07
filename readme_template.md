# SolutionName

**Inhaltsverzeichnis:**

- [SolutionName](#solutionname)
  - [Einleitung](#einleitung)
  - [Datenstruktur](#datenstruktur)
  - [Backend](#backend)
    - [Authentifizierung (wird derzeit nicht implementiert)](#authentifizierung-wird-derzeit-nicht-implementiert)
  - [Frontend](#frontend)
  - [Beschreibung der Validierungen](#beschreibung-der-validierungen)

---

## Einleitung

Beschreibung des Sytems.

## Datenstruktur

Das System besteht aus den Komponente(n):

- EntityName1
- EntityName2

**Tabelle:** **EntityName1**

| Name | Type    | Nullable | MaxLength | Unique | Beschreibung |
|------|---------|----------|-----------|--------|--------------|
| LicenseNumber  | String   | No  | 12  | Yes    | Eindeutige Führerscheinnummer |
| FirstName      | String   | No  | 64  | No     | Vorname des Inhabers |
| LastName       | String   | No  | 64  | No     | Nachname des Inhabers |
| DateOfBirth    | DateTime | No  | --- | No     | Geburtsdatum des Inhabers |
| IssueDate      | DateTime | No  | --- | No     | Ausstellungsdatum |
| ValidUntil     | DateTime | No  | --- | No     | Gültig bis |
| LicenseClasses | String   | Yes | 128 | No     | Führerscheinklassen (z.B. "B, A, C") |

**Tabelle:** **EntityName2**

| Name | Type       | Nullable | MaxLength | Unique | Beschreibung |
|------|------------|----------|-----------|--------|--------------|
| DriverLicense     | IdType   | No  | --- | No     | Referenz zum Führerschein |
| LicensePlate      | String   | No  | 10  | Yes    | KFZ-Kennzeichen |
| VIN               | String   | No  | 12  | Yes    | Fahrzeug-Identifikationsnummer |
| Manufacturer      | String   | No  | 64  | No     | Fahrzeughersteller |
| ModelDesignation  | String   | No  | 64  | No     | Fahrzeugmodell |
| FirstRegistration | DateTime | No  | --- | No     | Datum der Erstzulassung |

> **Hinweise:**
>
> - Hier können Informationen für die Validierung angegebn sein.

---

## Backend

Beschreibung des Backend-Services, der die Daten für die oben genannten Entitäten bereitstellt.

### Authentifizierung (wird derzeit nicht implementiert)

Die Authentifizierung regelt den Zugriff auf die Daten. Die nachfolgende Tabelle definiert die Berechtigungen für die einzelnen Rollen.

| Entität/Rolle           | Anonym | SysAdmin | RegistrationAuthority | Police |
|-------------------------|--------|----------|-----------------------|--------|
| **EntityName1**         | ---    | CRUD     | CRUD                  | R      |
| **EntityName2**         | ---    | CRUD     | CRUD                  | R      |

**CRUD...** Create, Read, Update, Delete

## Frontend

Beschreibung der Web-Anwendung, die die Daten über den Backend-Service anzeigt und verwaltet. Die klassische Angular-Anwendung soll die folgenden Funktionalitäten bieten:

- **Anzeigen** von Daten
- **Anlegen** von neuen Entitäten
- **Ändern** von bestehenden Entitäten
- **Löschen** von bestehender Entitäten

## Beschreibung der Validierungen

> **HINWEIS:** Falls die Validierung eines Feldes nicht positiv ist, wird eine `BusinessRuleException(...)` geworfen.
