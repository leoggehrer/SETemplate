# Prompt für die Generierung der HTML-Formulare mit Bootstrap

Du bist Experte für HTML-Formulare mit Bootstrap in Angular.
Für jede Entität sollst du zwei HTML-Formulare generieren:

1. **Listenansicht (List-Formular)**
2. **Bearbeitungsansicht (Edit-Formular)**

## Datumsformat

* Das vom Server gelieferte Datumsformat lautet: `{ propertyName: "2022-10-04T22:00:00.000Z" }`.
* Für Anzeige und Bearbeitung muss das Format **`dd.MM.yyyy`** verwendet werden, außer es gibt eine andere Vorgabe.
* Datumsfelder müssen im Bearbeitungsformular editierbar sein.

## Bearbeitungsmodus

* Zum Unterscheiden von Erstellen und Bearbeiten wird die Eigenschaft **`editMode`** im Bearbeitungsformular verwendet.
* Zum Erkennen des Modus wird in der Logik die Variable **`editMode`** genutzt.

### Listenansicht (List-Formular)

* Für die Listenansicht ist eine **Bootstrap-Card-Ansicht** zu verwenden.
* Die Komponenten sind bereits erstellt und befinden sich im Ordner `src/app/pages/entities`.
* Alle Komponenten sind `standalone` Komponenten.
* **Dateiname:** `entity-name-list.component.html`
* **Übersetzungen:** Ergänze die beiden Übersetzungsdateien `de.json` und `en.json` um die hinzugefügten Labels.

**Beispielstruktur:**

```html
<!--@AiCode-->
<!--HtmlBegin-->
<!--Filename:entity-name-list.component.html-->
<!--FormBegin-->
<div class="container mt-4">
    <div class="d-flex justify-content-between align-items-center mb-4 p-3" style="background-color: #f8f9fa; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1); border-radius: 4px;">
        <h3 class="mb-0 flex-grow-1">
          {{ 'ENTITYNAME_LIST.TITLE' | translate }}
        </h3>
        <a routerLink="/dashboard" class="btn btn-outline-secondary d-none d-sm-inline" title="{{ 'ENTITYNAME_LIST.BACK_TO_DASHBOARD' | translate }}" data-bs-toggle="tooltip" data-bs-placement="top">
            <i class="bi bi-arrow-left-circle"></i>
        </a>
        <a routerLink="/dashboard" class="btn btn-outline-secondary btn-lg d-inline d-sm-none" title="{{ 'ENTITYNAME_LIST.BACK_TO_DASHBOARD' | translate }}" data-bs-toggle="tooltip" data-bs-placement="top">
            <i class="bi bi-arrow-left-circle"></i>
        </a>
    </div>
    <div class="d-flex mb-3">
        <button *ngIf="canAdd" class="btn btn-primary me-2 d-none d-sm-inline" (click)="addCommand()" data-bs-toggle="tooltip" [title]="'ENTITYNAME_LIST.ADD_LIST' | translate">
            <i class="bi bi-plus"></i>
        </button>
        <button *ngIf="canAdd" class="btn btn-primary btn-lg me-2 d-inline d-sm-none" (click)="addCommand()" data-bs-toggle="tooltip" [title]="'ENTITYNAME_LIST.ADD_LIST' | translate">
            <i class="bi bi-plus"></i>
        </button>
        <input *ngIf="canSearch" type="text" class="form-control me-2" [(ngModel)]="searchTerm" [placeholder]="'ENTITYNAME_LIST.SEARCH_PLACEHOLDER' | translate" />
        <button *ngIf="canRefresh" class="btn btn-success" (click)="reloadData()" data-bs-toggle="tooltip" [title]="'ENTITYNAME_LIST.REFRESH' | translate">
            <i class="bi bi-arrow-clockwise"></i>
        </button>
    </div>
    <ul class="list-group">
        <li *ngFor="let item of dataItems" class="list-group-item d-flex flex-column flex-md-row align-items-md-center justify-content-between mb-2">
            <!-- Display the item details -->
            <div class="flex-grow-1">
                <div class="fw-bold mb-1"><i class="bi bi-list"></i> {{ item.name }}</div>
                <div class="text-muted small mb-1">{{ item.description }}</div>
                <div class="small">
                    <div class="row mb-1">
                        <div class="col-4 col-md-3 col-lg-2"><strong>{{ 'ENTITYNAME_LIST.PROPERTYNAME' | translate }}</strong></div>
                        <div class="col"><span class="fw-semibold">{{ item.propertyName }}</span></div>
                    </div>
                </div>
            </div>
            <!-- Action buttons -->
            <div class="d-flex gap-2 mt-2 mt-md-0">
                <button *ngIf="canEdit" class="btn btn-sm btn-outline-secondary d-none d-sm-inline" (click)="editCommand(item)" [title]="'ENTITYNAME_LIST.EDIT' | translate">
                    <i class="bi bi-pencil"></i>
                </button>
                <button *ngIf="canDelete" class="btn btn-sm btn-outline-danger d-none d-sm-inline" (click)="deleteCommand(item)" [title]="'ENTITYNAME_LIST.DELETE' | translate">
                    <i class="bi bi-trash"></i>
                </button>
            </div>
        </li>
    </ul>
</div>
<!--FormEnd-->
<!--HtmlEnd-->
```

### Bearbeitungsansicht (Edit-Formular)

* Für die Ansicht ist eine **Bootstrap-Card-Ansicht** zu verwenden.
* Die Komponenten sind bereits erstellt und befinden sich im Ordner `src/app/components/entities`.
* Alle Komponenten sind `standalone` Komponenten.
* **Dateiname:** `entity-name-edit.component.html`
* **Übersetzungen:** Ergänze die beiden Übersetzungsdateien `de.json` und `en.json` um die hinzugefügten Labels.
* Beispielstruktur:

```html
<!--@AiCode-->
<!--HtmlBegin-->
<!--Filename:entity-name-edit.component.html-->
<!--FormBegin-->
<div *ngIf="dataItem" class="card mt-4">
    <div class="card-header d-flex justify-content-between align-items-center">
        <h3>{{ title }}</h3>
        <button type="button" class="btn btn-sm btn-danger" [ariaLabel]="'ENTITYNAME_EDIT.CLOSE' | translate" (click)="dismiss()">
            <span aria-hidden="true">&times;</span>
        </button>
    </div>
    <div class="card-body">
        <form (ngSubmit)="submitForm()" #editForm="ngForm">
            <div class="mb-3">
                <label for="name" class="form-label">{{ 'ENTITYNAME_EDIT.NAME' | translate }}</label>
                <input id="name" class="form-control" [(ngModel)]="dataItem.name" name="name" required maxlength="100" />
            </div>
            <!-- Fields of entity -->
            <div class="mb-3">
                <label for="description" class="form-label">{{ 'ENTITYNAME_EDIT.DESCRIPTION' | translate }}</label>
                <textarea id="description" class="form-control" [(ngModel)]="dataItem.description" name="description" maxlength="500"></textarea>
            </div>
            <div class="mb-3">
                <label for="createdOn" class="form-label">{{ 'ENTITYNAME_EDIT.CREATED_ON' | translate }}</label>
                <input
                    id="createdOn"
                    type="date"
                    class="form-control"
                    [ngModel]="getDateString('createdOn')"
                    (ngModelChange)="setDateString('createdOn', $event)"
                    name="createdOn"
                    required
                />
            </div>
            <button class="btn btn-success" type="submit">{{ 'ENTITYNAME_EDIT.SAVE' | translate }}</button>
            <button class="btn btn-secondary ms-2" type="button" (click)="cancelForm()">{{ 'ENTITYNAME_EDIT.CANCEL' | translate }}</button>
        </form>
    </div>
</div>
<!--FormEnd-->
<!--HtmlEnd-->
```

### Master-Details

In manchen Fällen ist eine Master/Details Ansicht sehr hilfreich. Diese Anzeige besteht aus einer Master-Ansicht. Diese kann nicht bearbeitet werden. Die Details zu diesem Master werden unter der Master-Ansicht als 'List-group' angezeigt. Nachfolgend ist die Struktur skizziert:

```html
<!--@AiCode-->
<!--HtmlBegin-->
<!--Filename:entity-name-details.component.html-->
<!--FormBegin-->
<div class="container mt-4" *ngIf="data">
    <!-- Master: Listendaten (readonly) -->
    <div class="card mb-3">
        <div class="card-header d-flex justify-content-between align-items-center">
            <h4 class="mb-0"><i class="bi bi-list-task"></i> {{ data.propertyName }}</h4>
            <a routerLink="/tdlists" class="btn btn-outline-secondary btn-sm d-none d-sm-inline" title="{{ 'ENTITYNAME.BACK_TO_LIST' | translate }}" data-bs-toggle="tooltip" data-bs-placement="top">
                <i class="bi bi-arrow-left-circle"></i>
            </a>
            <a routerLink="/tdlists" class="btn btn-outline-secondary btn-lg d-inline d-sm-none" title="{{ 'ENTITYNAME.BACK_TO_LIST' | translate }}" data-bs-toggle="tooltip" data-bs-placement="top">
                <i class="bi bi-arrow-left-circle"></i>
            </a>
        </div>
        <div class="card-body">
            <p>{{ data.propertyName2 }}</p>
            <div class="row mb-1">
                <div class="col-4 col-md-3 col-lg-2"><strong>{{ 'ENTITYNAME.PROPERTYNAME2' | translate }}</strong></div>
                <div class="col">{{ data.propertyName2 }}</div>
            </div>
        </div>
    </div>

    <!-- Detail: Liste -->
    <div class="d-flex justify-content-between align-items-center mb-2 flex-wrap">
        <h5 class="mb-2 mb-md-0 ms-2"><i class="bi bi-card-checklist"></i> {{ 'ENTITYNAME.DETAILS' | translate }}</h5>
        <div class="mt-2 mt-md-0 me-2">
            <button class="btn btn-primary btn-sm d-none d-sm-inline" (click)="addCommand()" title="{{ 'ENTITYNAME.ADD_DETAILNAME' | translate }}" data-bs-toggle="tooltip" data-bs-placement="top">
                <i class="bi bi-plus"></i>
            </button>
            <button class="btn btn-primary btn-lg d-inline d-sm-none" (click)="addCommand()" title="{{ 'ENTITYNAME.ADD_DETAILNAME | translate }}" data-bs-toggle="tooltip" data-bs-placement="top">
                <i class="bi bi-plus"></i>
            </button>
        </div>
    </div>
    <ul class="list-group mb-3">
        <li *ngFor="let item of dataItems" class="list-group-item">
            <div class="d-flex flex-column flex-md-row justify-content-between align-items-start">
                <div class="flex-grow-1">
                    <span class="fw-semibold text-primary">{{ item.propertyName }}</span>
                    <span class="text-muted small d-block text-truncate mb-2 mt-1" style="max-width: 400px;" [title]="item.description">{{ item.propewrtyName2 }}</span>
                    <div class="row">
                        <div class="col-4 col-md-3 col-lg-2"><strong>{{ 'DETAILNAME.PROPERTYNAME' | translate }}</strong></div>
                        <div class="col">{{ item.propertyName }}</div>
                    </div>
                    <!-- Weitere Eigenschaften-->
                </div>
                <div class="mt-2 mt-md-0 ms-md-2">
                    <button class="btn btn-sm btn-outline-secondary me-1 d-none d-sm-inline" (click)="editCommand(item)" title="{{ 'ENTITYNAME.EDIT_DETAILNAME' | translate }}" data-bs-toggle="tooltip" data-bs-placement="top">
                        <i class="bi bi-pencil"></i>
                    </button>
                    <button class="btn btn-lg btn-outline-secondary me-1 d-inline d-sm-none" (click)="editCommand(item)" title="{{ 'ENTITYNAME.EDIT_DETAILNAME' | translate }}" data-bs-toggle="tooltip" data-bs-placement="top">
                        <i class="bi bi-pencil"></i>
                    </button>
                    <button class="btn btn-sm btn-outline-danger d-none d-sm-inline" (click)="deleteCommand(item)" title="{{ 'ENTITYNAME.DELETE_DETAILNAME' | translate }}" data-bs-toggle="tooltip" data-bs-placement="top">
                        <i class="bi bi-trash"></i>
                    </button>
                    <button class="btn btn-lg btn-outline-danger d-inline d-sm-none" (click)="deleteCommand(item)" title="{{ 'ENTITYNAME.DELETE_DETAILNAME' | translate }}" data-bs-toggle="tooltip" data-bs-placement="top">
                        <i class="bi bi-trash"></i>
                    </button>
                </div>
            </div>
        </li>
        <li *ngIf="dataItems.length === 0" class="list-group-item">{{ 'ENTITYNAME.NO_DETAILNAME' | translate }}</li>
    </ul>
</div>
<!--FormEnd-->
<!--HtmlEnd-->
```

### Format der Ausgabe

* Gib ausschließlich den HTML-Code aus, ohne zusätzliche Kommentare oder Erklärungen.
* Der Code muss direkt in eine **Angular-Komponente** eingefügt werden können. Die Komponente muss bereits existieren.
* Alle erforderlichen Angular-Module (z. B. **FormsModule**, **CommonModule**, **TranslateModule**) müssen im Modul der Komponente importiert sein.
* Bindings wie **`[(ngModel)]`**, **`*ngIf`**, **`*ngFor`** müssen korrekt gesetzt sein.
