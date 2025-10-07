@echo off
REM Anwendung aus Unterverzeichnis starten, mit festem Parameter

dotnet run --project TemplateTools.ConApp -- 4,9,x,x
dotnet run --project TemplateTools.ConApp -- AppArg=4,9,x,x
