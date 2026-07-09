# Autoservis

Aplikace pro komplexní správu autoservisu, vyvinuta v prostředí .NET WPF. Umožňuje evidenci zákazníků, jejich vozidel a detailní správu servisních zakázek.

## Ke stažení
Instalační soubor naleznete zde: [Stáhnout instalaci Autoservis](https://github.com/PetrMFIT/Autoservis/releases/download/v1.0.0/setup.exe)

## Současné funkce
- Správa zákazníků: Centrální evidence jména, telefonu, e-mailu, adresy a poznámek.
- Evidence vozidel: Záznamy o značce, modelu, SPZ, VIN, roku výroby, typu paliva a přiřazení k majiteli.
- Servisní zakázky: Tvorba zakázek s detailním rozpisem, sledováním stavu tachometru a statusu zakázky (Rozpracováno, Hotovo, Zaplaceno).
- Položky zakázky: Evidence použitého materiálu (kód, název, množství, jednotka, dodavatel) a provedené práce (počet hodin, sazba).
- Fotodokumentace: Moznost připojení a správy fotografii k jednotlivým zakázkám pro dokumentaci oprav.
- Export do PDF: Základní generování PDF dokumentu pro přehled zakázky.

## Budoucí plány (Roadmap)
- Přechod na hybridní architekturu: Plánuji předělat aplikaci na webovou technologii zabalenou v desktopovém prostředí (např. WebView2 / Blazor Hybrid).
- Pokročilá customizace PDF: Nastaveni vzhledu a generování zakázkových listů přímo v aplikaci podle potřeb uživatele.
- Podpora vlastního loga: Moznost nahrání vlastního loga servisu, které se bude zobrazovat v hlavičce aplikace i na všech exportovaných dokumentech.

## Technologie
- Jazyk: C#
- Framework: .NET WPF
- Databáze: SQLite s vyuzitim Entity Framework Core
- PDF Export: QuestPDF
- UI komponenty: MahApps.Metro.IconPacks

## Instalace a spuštěni
1. Stáhněte si instalační balíček z odkazu výše.
2. Spusťte soubor `.exe`
3. Aplikace si při prvním spuštění sama vytvoří databázi a potřebné složky v: `%AppData%\Roaming\AutoservisApp\`


