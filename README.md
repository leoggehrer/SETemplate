# SETemplate  
  
Das Projekt ***SETemplate*** ist eine Vorlage fuer die Erstellung von datenzentrierten Anwendungen. Ausgehend von dieser Vorlage koennen neue Anwendungen, sogenannte Domain-Projekte, erstellt, erweitert und angepasst werden.  
  
## Inhaltsverzeichnis

- [SETemplate](#SETemplate)
  - [Inhaltsverzeichnis](#inhaltsverzeichnis)
  - [Infrastruktur](#infrastruktur)
  - [Template](#template)
  - [Entwicklerwerkzeuge](#entwicklerwerkzeuge)
  - [Verwendung der Vorlage](#verwendung-der-vorlage)
  - [System-Erstellungs-Prozess](#system-erstellungs-prozess)
    - [Vorbereitungen](#vorbereitungen)
    - [Projekterstellung](#projekterstellung)
  - [Abgleich mit dem SETemplate](#abgleich-mit-dem-SETemplate)
  - [Setzen von Preprozessor-Defines](#setzen-von-preprozessor-defines)
  - [Umsetzungsschritte](#umsetzungsschritte)
    - [Erstellen des Backend-Systems](#erstellen-des-backend-systems)
    - [Erstellen der AspMvc-Anwendung](#erstellen-der-aspmvc-anwendung)
    - [Erstellen des RESTful-Services](#erstellen-des-restful-services)
  
## Infrastruktur  
  
Zur Umsetzung des Projektes wird DotNetCore (8.0 und hoeher) als Framework, die Programmiersprache CSharp (C#) und die Entwicklungsumgebung Visual Studio 2022 Community verwendet. Alle Komponenten koennen kostenlos aus dem Internet heruntergeladen werden.  
  
In diese Dokumentation werden unterschiedliche Begriffe verwendet. In der nachfolgenden Tabelle werden die wichtigsten Begriffe zusammengefasst und erlaeutert:  

|Begriff|Bedeutung|Synonym(e)|
|---|---|---|
|**Solution**|Ist eine Zusammenstellung von verschiedenen Teilprojekten zu einer Gesamtloesung.|Gesamtloesung, Loesung, Projekt|
|**Domain Solution**|Damit ist eine Gesamtloesung gemeint, welches fuer einen bestimmten Problembereich eine Loesung darstellt.|Problemloesung, Projekt|
|**Teilprojekt**|Ist die Zusammenfassung von Klassen und/oder Algorithmen, welches eine logische Einheit fuer die Loesungen bestimmter Teilprobleme bildet.|Teilloesung, Projekteinheit, Projekt|
|**Projekttyp**|Unter Projekttyp wird die physikalische Beschaffenheit eines Projektes bezeichnet. Es gibt zwei grundlegende Typen von Projekten: <br>  - Ein wiederverwendbares Projekt (z.B.: eine Bibliothek) und <br>  - ein ausfuehrbares Projekt (z.B.: eine Konsolenanwendung, WepApi, AspMvc usw.). <br>**Als Regel gilt:**<br> Alle Programme und Algorithmen werden in den *wiederverwendbaren Projekten* implementiert. Die ausfuehrbaren Einheiten dienen nur als Startprojekte und leiten die Anfragen an die *wiederverwendbaren Komponenten* weiter.| Bibliothek, Console |
|**Library oder Bibliothek**|Kennzeichnet einen *wiederverwendbaren Projekttyp*. | Bibliothek |
|**Console**|Kennzeichnet einen *ausfuehrbaren Projekttyp*. Dieser Typ startet eine Konsole fuer die Programmausfuehrung.|Konsole|
|**Host**|Dieser Typ kennzeichnet ein *ausfuehrbares Projekt*, welches zum Starten den IIS verwendet oder im Modus `selfhosting` gestartet werden kann. | Web-Application |
|**Abhaengigkeit**|Die Abhaengikeit beschreibt die Beziehungen von Projekten untereinander. Benoetigt ein *Projekt A* Funktionalitaeten aus einem *Projekt B*, so wird eine Projektreferenz vom *Projekt B* in *Projekt A* benoetigt.| Projektreferenz, Referenz, Dependency, Projektverweis |
  
## Template  

Die Struktur vom **SETemplate** besteht aus unterschiedlichen Teilprojekten und diese sind in einer Gesamtloesung (im Kontext von Visual Studio ist das eine Solution) zusammengefasst. Eine Erlaeuterung der einzelnen Projekte, deren Typ und die Abhaengigkeit finden sie in der folgenden Tabelle:  
  
|Projekt|Beschreibung|Typ|Abhaengigkeit|
|---|---|---|---|
|**SETemplate.Common** |In diesem Projekt werden alle Hilfsfunktionen, allgemeine Erweiterungen und Schnittstellen zusammengefasst. Diese sind unabhaengig vom Problembereich und koennen auch in andere Domaen-Projekte wiederverwendet werden.| Library | keine |
|**SETemplate.Logic**|Dieses Projekt beinhaltet den vollstaendigen Datenzugriff, die gesamte Geschaeftslogik und stellt somit den zentralen Baustein des Systems dar.| Library | SETemplate.Common |
|**SETemplate.Logic.UnitTest**|In diesem Projekt befinden sich die Unit-Tests fuer die gesamte Geschaeftslogik.| MSTest | SETemplate.Common, SETemplate.Logic |
|**SETemplate.WebApi**|In diesem Projekt ist die REST-Schnittstelle implementiert. Dieses Modul stellt eine API (Aplication Programming Interface) fuer den Zugriff auf das System ueber das Netzwerk zur Verfuegung.| Host | SETemplate.Logic |
|**SETemplate.ConApp**|Dieses Projekt dient als Initial-Anwendung zum Erstellen der Datenbank, das Anlegen von Anmeldedaten falls die Authentifizierung aktiv ist und zum Importieren von bestehenden Daten. Nach der Initialisierung wird diese Anwendung kaum verwendet.| Console | SETemplate.Logic |
|**SETemplate.MVVMApp**|Diese Projekt beinhaltet die Basisfunktionen fuer eine Wpf-Anwendung (Avalonia) und kann als Vorlage fuer die Entwicklung einer einer Wpf-Anwendung mit dem SETemplate Framework verwendet werden.|Host| SETemplate.Logic |
|**SETemplate.XxxYyy**|Es folgen noch weitere Vorlagen von Client-Anwendungen wie Angular, Blazor und mobile Apps. Zum jetzigen Zeitpunkt existiert nur die AspMvc-Anwendung und Wpf-Anwendung. Die Erstellung und Beschreibung weiterer Client-Anwendungen erfolgt zu einem spaeteren Zeitpunkt.| Host | SETemplate.Logic |
  
## Entwicklerwerkzeuge  

Dem Entwickler stehen unterschiedliche Hilfsmittel fuer die Erstellung von Projekten zur Seite. Die wichtigsten Werkzeuge sind in der nachfolgenden Tabelle zusammengefasst:  
  
|Projekt|Beschreibung|Typ|Abhaengigkeit  
|---|---|---|---|
|**TemplateTools.Logic** | In diesem Modul befinden sich die Komponenten zur Code-Generierung. Eine genauere Beschreibung der Komponenten erfolgt zu einenem späteren Zeitpunk.| Library | SETemplate.Common |
|**SETemplate.CodeGenApp**|Diese Anwendung startet die Code-Generierung fuer das Projekt in dem es enthalten ist. <br>**Voraussetzung:**<br> Die Code-Generierung verwendet die Meta-Information der Logik (XY.Logic oder XY.AppLogic). Das bedeuted allerdings, dass die Bibliothek fehlerfrei ueberstezt werden kann. Nur so kann die Meta-Information aus Logik gelesen und fuer die Code-Generierung verwendet werden.|Console| TemplateTools.Logic |
|**TemplateTools.ConApp**|Diese Anwendung ist eine Zusammenfassung von unterschiedlichen Werkzeugen welche mit dem ***'SETemplate'*** verwendet werden koennen. Folgende Werkzeuge befinden sich in der Sammlung:<br>- **Copier**...kopiert alle Teilprojekte aus der Vorlage ***'SETemplate'*** in das angegebenen Zielverzeichnis und fuehrt eine Umbenennung der Komponenten durch.<br>- **Preprocessor**...dient zum Setzen der Definitionen (z.B. ACCOUNT_ON usw.) in der entspechenden Solution. <br>- **Code-Generierung**...Mit dieser Auswahl kann die Code-Generierung aktiviert werden.<br>-**Comparison**...Diese Funktion dient zum Abgleich der Vorlage ***'SETemplate'*** mit den bereits erstellten ***'Domain-Projekten'***.| Console | SETemplate.Common |
  
## Verwendung der Vorlage
  
Nachfolgend werden die einzelnen Schritte von der Vorlage ***SETemplate*** bis zum konkreten Projekt ***QTMusicStore*** erlaeutert. Das Projekt ist eine einfache Anwendung zur Demonstration von der Verwendung der Vorlage. Im Projekt ***QTMusicStore*** werden Kuenstler (Artisten) und deren produzierten Alben verwaltet. Jedes Album hat ein Genre (Rock, Pop, Klassik usw.) zugeordnet. Ein Datenmodell ist im nachfolgendem Abschnitt definiert.  

## System-Erstellungs-Prozess  
  
Wenn nun ein einfacher Service oder eine Anwendung entwickelt werden soll, dann kann die Vorlage ***SETemplate*** als Ausgangsbasis verwendet und weiterentwickelt werden. Dazu empfiehlt sich folgende Vorgangsweise:  
  
### Vorbereitungen  
  
- Erstellen eines Ordners (z.B.: Develop)  
- Herunterladen des Repositories ***SETemplate*** vom [GitHub](<https://github.com/leoggehrer/CSTemplate-SETemplate>) und in einem Ordner speichern.  
  
> **ACHTUNG:** Der Solution-Ordner von der Vorlage muss ***SETemplate*** heißen.  
  
### Projekterstellung  

Die nachfolgenden Abbildung zeigt den schematischen Erstellungs-Prozess fuer ein Domain-Projekt:  
  
![Create domain project overview](img/CreateProjectOverview.png)  
  
Als Ausgangsbasis wird die Vorlage ***SETemplate*** verwendet. Diese Vorlage wird mit Hilfe dem Hilfsprogramm ***'TemplateTools.ConApp'*** in ein Verzeichnis eigener Wahl kopiert. Bei der Erstellung des Domain-Projektes **QTMusicStore** werden die folgenden Aktionen ausgefuehrt:

- Alle Projektteile aus der Vorlage werden in das Zielverzeichnis kopiert.
- Die Namen der Projekte und Komponenten werden entsprechend angepasst.
- Alle Projekte mit dem Prefix **SETemplate** werden mit dem domainspezifischen Namen erstezt.
- Beim Kopieren der Dateien wird der Label **@BaseCode** mit dem Label **@CodeCopy** ersetzt (Diese Labels werden fur einen späteren Abgleich-Prozess verwendet).

Nachfolgend wird die Erstellung des Beispiel-Projektes **QTMusicStore**  mit dem Werkzeug **TemplateTools.ConApp** demonstriert:
  
```csharp
==============
Template Tools
==============

Force flag:    False
Solution path: C:\Users\g.gehrer\source\repos\leoggehrer\SETemplate

[ ----] -----------------------------------------------------------------
[    1] Force...............Change force flag
[    2] Path................Change solution path
[ ----] -----------------------------------------------------------------
[    3] Copier..............Copy this solution to a domain solution
[    4] Preprocessor........Setting defines for project options
[    5] CodeGenerator.......Generate code for this solution
[    6] Comparison..........Compares a project with the template
[    7] Cleanup.............Deletes the temporary directories
[-----] -----------------------------------------------------------------
[  x|X] Exit................Exits the application

Choose [n|n,n|a...all|x|X]: 3
```

Nach der Bestaetigung der Auswahl erscheint die folgende Bildschirmmaske:

```csharp
Template Copier
===============

Copy 'SETemplate' from: /Users/ggehrer/Projects/CSMacTemplate/SETemplate
Copy to 'TargetSolution':   /Users/ggehrer/Projects/CSMacTemplate/TargetSolution

[1] Change source path
[2] Change target path
[3] Change target solution name
[4] Start copy process
[x|X] Exit

Choose: 
```

Nun muessen die Parameter **Target path** und **Target solution name** eingestellt werden.

```csharp
Template Copier
===============

Copy 'SETemplate' from: /Users/ggehrer/Projects/CSMacTemplate/SETemplate
Copy to 'QTMusicStore':   /Users/ggehrer/Projects/Sample/QTMusicStore

[1] Change source path
[2] Change target path
[3] Change target solution name
[4] Start copy process
[x|X] Exit

Choose: 4
```

**Hinweis:** Die Vorlage muss in einem Ordner mit dem Namen **SETemplate** gespeichert sein.  
  
Nach der Ausfuehren der Option ***'[4] Start copy process'*** befindet sich folgende Projektstruktur im Ordner **...\QTMusicStore**:  
  
- CommonBase  
- QTMusicStore.AppLogic
- QTMusicStore.AspMvc
- QTMusicStore.CodeGenApp
- QTMusicStore.ConApp
- QTMusicStore.Logic.UnitTest
- QTMusicStore.WebApi
- QTMusicStore.WpfApp
- TemplateCodeGeneration.AppLogic
- TemplateTools.ConApp
  
In der Vorlage ***SETemplate*** sind alle Code-Teile, welche als Basis-Code in andere Projekte verwendet werden, mit einem Label ***@BaseCode*** markiert. Dieser Label wird im Zielprojekt mit dem Label ***@CodeCopy*** ersetzt. Das hat den Vorteil, dass Aenderungen in der Vorlage auf die bereits bestehenden Projekte uebertragen werden koennen (naehere Informationen dazu gibt es spaeter).  
  
> **ACHTUNG:** Im Domain-Projekt duerfen keine Aenderungen von Dateien mit dem Label ***@CodeCopy*** durchgefuehrt werden, da diesen beim naechsten Abgleich wieder ueberschrieben werden. Sollen dennoch Aenderungen vorgenommen werden, dann muss der Label ***@CodeCopy*** geaendert (z.B.: @CustomCode) oder entfernt werden. Damit wird diese Datei vom Abgleich mit der Vorlage ausgeschlossen.  
  
## Abgleich mit dem SETemplate  
  
In der Software-Entwicklung gibt es immer wieder Verbesserungen und Erweiterungen. Das betrifft die Vorlage ***SETemplate*** genauso wie alle anderen Projekte. Nun stellt sich die Frage: Wie koennen Verbesserungen und/oder Erweiterungen von der Vorlage auf die **Domain-Projekte** uebertragen werden?  
In der Vorlage sind die Quellcode-Dateien mit den Labels ***@BaseCode*** gekennzeichnet. Beim Kopieren werden diese Labels durch den Label ***@CodeCopy*** ersetzt. Mit dem Werkzeug **Comparison** im Programm **TemplateTools.ConApp** werden die Dateien mit dem Label ***@BaseCode*** und ***@CodeCopy*** abgeglichen. In der folgenden Skizze ist dieser Prozess dargestellt:  
  
![Template-Comparsion-Overview](TemplateComparsionOverview.png)  
  
Fuer den Abgleichprozess wird ebefalls die Werkzeugsammlung ***TemplateTools.ConApp*** verwendet:
  
```csharp
Template Tools
==============

Choose a tool:

[1] Copier..........Copy a quick template to a project
[2] Preprocessor....Setting defines for project options
[3] CodeGenerator...Generate code for template solutions
[4] Comparison......compares a project with the template and compares it
[x|X] Exit

Choose: 4
```
  
Nach der Bestaetigung der Auswahl erscheint die folgende Bildschirmmaske:
  
```csharp  
Template Comparison
===================

Balance label(s):
  @BaseCode       => @CodeCopy

Source: /Users/ggehrer/Projects/CSMacTemplate/SETemplate

[ +] Add path: ADD...
[ 1] Balancing for: /Users/ggehrer/Projects/Sample/QTMusicStore
[ a] Balancing for: ALL...
[x|X...Quit]: 

Choose [n|n,n|x|X]: 1
```  
  
Wird nun die Option **[1 oder a]** aktiviert, dann werden alle Dateien in der Solution **SETemplate** mit der Kennzeichnung `@BaseCopy` mit den Dateien in der Solution  **QTMusicStore** mit der Kennzeichnung `@CodeCopy` abgeglichen.  
  
## Setzen von Preprozessor-Defines  

Im Projekt ***SETemplate*** sind Preprozessor-Definitions definiert. Mit Hilfe dieser Definitions können Module und Funktionen  eingeschaltet bzw. ausgeschaltet werden. Diese *Definitions* werden natuerlich beim Kopieren in das Domain-Projekt uebernommen. Mit dem Werkzeugt **Preprocessor** koennen diese Definitionen ein.- bzw. ausgeschaltet werden. Das Programm kann ebenfalls ueber das Programm **TemplateTools.ConApp** aktiviert werden.

```csharp
Template Tools
==============

Choose a tool:

[1] Copier..........Copy a quick template to a project
[2] Preprocessor....Setting defines for project options
[3] CodeGenerator...Generate code for template solutions
[4] Comparison......compares a project with the template and compares it
[x|X] Exit

Choose: 2
```

Nach der Bestaetigung der Auswahl erscheint die folgende Bildschirmmaske:

```csharp
Template Preprocessor
=====================

Define-Values:
--------------
ACCOUNT_OFF ACCESSRULES_OFF LOGGING_OFF REVISION_OFF DBOPERATION_ON ROWVERSION_OFF GUID_OFF CREATED_OFF MODIFIED_OFF CREATEDBY_OFF MODIFIEDBY_OFF IDINT_ON IDLONG_OFF IDGUID_OFF SQLSERVER_OFF SQLITE_ON GENERATEDCODE_ON 

Set define-values 'SETemplate' from: /Users/ggehrer/Projects/Sample/QTMusicStore

[1 ] Change source path
[2 ] Set definition ACCOUNT_OFF          ==> ACCOUNT_ON
[3 ] Set definition ACCESSRULES_OFF      ==> ACCESSRULES_ON
[4 ] Set definition LOGGING_OFF          ==> LOGGING_ON
[5 ] Set definition REVISION_OFF         ==> REVISION_ON
[6 ] Set definition DBOPERATION_ON       ==> DBOPERATION_OFF
[7 ] Set definition ROWVERSION_OFF       ==> ROWVERSION_ON
[8 ] Set definition GUID_OFF             ==> GUID_ON
[9 ] Set definition CREATED_OFF          ==> CREATED_ON
[10] Set definition MODIFIED_OFF         ==> MODIFIED_ON
[11] Set definition CREATEDBY_OFF        ==> CREATEDBY_ON
[12] Set definition MODIFIEDBY_OFF       ==> MODIFIEDBY_ON
[13] Set definition IDINT_ON             ==> IDINT_OFF
[14] Set definition IDLONG_OFF           ==> IDLONG_ON
[15] Set definition IDGUID_OFF           ==> IDGUID_ON
[16] Set definition SQLSERVER_OFF        ==> SQLSERVER_ON
[17] Set definition SQLITE_ON            ==> SQLITE_OFF
[18] Set definition GENERATEDCODE_ON     ==> GENERATEDCODE_OFF
[19] Start assignment process...
[x|X] Exit

Choose [n|n,n|x|X]: 
```

Mit der entsprechenden Auswahl koennen die Module bzw. Funktionen ein.- und ausgeschaltet werden. Die folgende Tabelle bietet eine Uebersicht ueber die Module und Funktionen:

| Preprozessor-Definition | Modul/Funktion | Abhaengigkeit | Beschreibung |
|-------------------------|----------------|---------------|--------------|
| ACCOUNT_ON | Authentifizierung | keine | Aktiviert das rollenbasierte Zugriffssystem. Das bedeuted, dass ein Zugriff auf das System nur mit einem gueltigem Konto gestattet ist. <br>**Rolle:**<br>Eine Rolle wird dem Konto zugeordnet und mit dieser Rolle sind unterschiedliche Operationen moeglich. Zum Beispiel kann mit der Rolle **SysAdmin** ein weiteres Konto erstellt und bearbeitet werden. Naehere Details erfolgen in einem spaeteren Abschnitt.|
| ACCESSRULES_ON | Autorisierung | ACCOUNT_ON | Aktiviert die datengesteuerte Zugriffskontrolle. Zum Beispiel darf der Datensatz mit der Id=4711 nur mit der Rolle **DataManager** geloescht werden. Naehere Details erfolgen in einem spaeteren Abschnitt. |
| LOGGING_ON               | Logging | ACCOUNT_ON | Die Manipulation der Daten wird protokolliert. Zum Beispiel: Mit welchem Konto und Zeitpunkt sind Datensaetze erstellt bzw. geaendert worden. |
|REVISION_ON | Historie | ACCOUNT_ON | Die Daten werden zusaetzlich in einer Historie verwaltet. Aenderungen in der Vergangenheit werden aufgezeichnet. |
| DBOPERATION_ON | Datenbank | keine | Schaltet die programmgesteuerten Operationen wie Loeschen, Erstellen, Migration usw. ein. **Vorsicht:** Wenn diese Option eingeschaltet ist, dann kann mit dem **DbManager** die Datenbank geloescht werden!  |
| ROWVERSION_ON | Optimistic Concurrency | keine | Kontrolliert den parallelen Zugriff von mehreren Benutzer auf denselben Datansatz. Manche Datenbanken (z.B.: SQLite) bieten keine Unterstuetzung und daher muss diese Eigenschaft ausgeschaltet werden. |  
| GUID_ON | Spalte Guid | keine | Aktiviert fuer eine Entitaet eine zusaetzliche Identitaetseigenschaft. Diese Eigenschaft wird vom System verwaltet. |
| CREATED_ON | Erstellung | ACCOUNT_ON | Aktiviert fuer eine Entitaet eine zusaetzliche Eigenschaft fuer den Erstellungszeitpunk. Diese Eigenschaft wird vom System verwaltet. |
| MODIFIED_ON | Aenderung | ACCOUNT_ON | Aktiviert fuer eine Entitaet eine zusaetzliche Eigenschaft fuer den letzten Aenderungszeitpunkt. Diese Eigenschaft wird vom System verwaltet. |
| CREATEDBY_ON | Erstellung-Konto | ACCOUNT_ON | Aktiviert fuer eine Entitaet eine zusaetzliche Navigationseigenschaft zum Ersteller-Konto. Diese Eigenschaft wird vom System verwaltet. |
| MODIFIEDBY_ON | Aenderung | ACCOUNT_ON | Aktiviert fuer eine Entitaet eine zusaetzliche Navigationseigenschaft zum letzten Aenderungs-Konto. Diese Eigenschaft wird vom System verwaltet. |
| IDINT_ON | IdType=int | keine | Definiert den Datentyp fuer die Identitaet einer Entitaet mit dem Datentyp **int**. Die anderen Optionen werden automatisch ausgeschaltet. |
| IDLONG_ON | IdType=long | keine | Definiert den Datentyp fuer die Identitaet einer Entitaet mit dem Datentyp **long**. Die anderen Optionen werden automatisch ausgeschaltet. |
| IDGUID_ON | IdType=Guid | keine | Definiert den Datentyp fuer die Identitaet einer Entitaet mit dem Datentyp **Guid**. Die anderen Optionen werden automatisch ausgeschaltet. |
| SQLSERVER_ON | Datenbank | keine | Definiert die Verwendung der Datenbank **MSSQL-SERVER**. Die anderen Optionen werden automatisch ausgeschaltet. |
| SQLITE_ON | Datenbank | keine | Definiert die Verwendung der Datenbank **SQLite**. Die anderen Optionen werden automatisch ausgeschaltet. |
| GENERATEDCODE_ON | Code-Generierung | keine | Diese Definition zeigt an, dass fuer dieses Projekt die Code-Generierung ausgefuehrt wurde. Wenn die Funktion **Delete generation files** ausgefuehrt wurde, dann ist diese Definition wieder ausgeschaltet. Naehere Details erfolgen in einem spaeteren Abschnitt. |

## Umsetzungsschritte  
  
Nachdem nun das Domain-Projekt **QTMusicStore** erstellt wurde, werden nun folgende Schritte der Reihe nach ausgefuehrt:  
  
### Erstellen des Backend-Systems
  
- Erstellen der ***Enumeration***  
  - ...  
- Erstellen der ***Entitaeten***  
  - ...  
- Definition des ***Datenbank-Kontext***  
  - *DbSet&lt;Entity&gt;* definieren  
  - ...  
  - partielle Methode ***GetDbSet<E>()*** implementieren  
- Erstellen der ***Kontroller*** im *Logic* Projekt  
  - ***EntityController*** erstellen  
  - ...  
- Erstellen der ***Datenbank*** mit den Kommandos in der ***Package Manager Console***  
  - *add-migration InitDb*  
  - *update-database*  
- Implementierung der ***Business-Logic***  
- Erstellen des UnitTest-Projekt mit der Bezeichnung ***QTMusicStore.Logic.UnitTest***  
  - Ueberpruefen der Geschaeftslogik mit ***UnitTests***  
- Importieren von Daten (optional)  
  
### Erstellen der AspMvc-Anwendung

Mit wenigen Schritte kann fuer das *Backend* eine AspMvc Web-Anwendung, fuer die Verwaltung und Bearbeitung von Daten, erstellt werden. Die Vorlage dazu befindet sich bereit in der Solution. Zu diesem Zweck sind die Schritte der Reihe nach auszufuehren:  

- Erstellen der Models  
  - ...  
- Erstellen der Kontroller  
  - ...  
- Erstellen der Ansichten  
  - ...  

> HINWEIS: Fuer die Erstellung der Ansichten (Views) koennen auch die bereitgestellten Standard-Ansichten (im Ordner /Views/Shared) verwendet werden. Eine Anleitung zur Verwendung dieser Ansichten befindet sich im Dokument mit dem Namen *'AspMvcDefaultViews.md'*.

### Erstellen des RESTful-Services
  
Um die Funktionen im Backend anderen (heterogenen) System zur Verfuegung zu stellen, ist es sinnvoll, dass diese Funktionalitaet ueber RESTful-Services delegiert werden. Fuer die Bereitstellung sind die folgenden Schritte notwendig:

- Erstellen der Models  
  - ...  
- Erstellen der Kontroller  
  - ...  
  
Die einzelnen Schritte sind im [Github-QTMusicStore](https://github.com/leoggehrer/Sample-QTMusicStore) detailiert aufgefuehrt.  
  
**Viel Spaß beim Umsetzen der Aufgabe!**  
