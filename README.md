# Dokumentace k aplikaci

## Přehled
Tato aplikace je navržena pro kontrolu souborů v adresáři. Obsahuje kontrolu počtu souborů a velikosti souborů v zadaném adresáři. Aplikace využívá ASP.NET Core a poskytuje API endpoint pro kontrolu změn v adresáři.

## Složky a soubory
- `Controllers/FileCheckerController.cs`: Obsahuje kontroler, který zpracovává HTTP požadavky a volá služby pro kontrolu souborů.
- `DAL/FileCheckerService.cs`: Obsahuje implementaci služby pro kontrolu souborů.
- `Const.cs`: Obsahuje konstanty používané v aplikaci.
- `TestProject1/FileCheckerServiceTests.cs`: Obsahuje unit testy pro `FileCheckerService`.

## Třídy a metody

### `FileCheckerController`
Kontroler, který zpracovává HTTP požadavky a volá služby pro kontrolu souborů.

#### Metody
- `CheckFileChanges(string dataFolderPath)`: Kontroluje změny v zadaném adresáři a vrací JSON objekt s informacemi o změnách.

### `FileCheckerService`
Služba pro kontrolu souborů v adresáři.

#### Metody
- `CheckNumberOfFilesPerFolder(string dataFolderPath)`: Kontroluje počet souborů v adresáři a vrací JSON objekt s chybovou zprávou, pokud je počet souborů překročen.
- `CheckFilesSize(string dataFolderPath)`: Kontroluje velikost souborů v adresáři a vrací JSON objekt s chybovou zprávou, pokud některý soubor překračuje maximální povolenou velikost.

### `Const`
Třída obsahující konstanty používané v aplikaci.

#### Konstanty
- `MaxFileCount`: Maximální počet souborů v adresáři.
- `MaxFileSize`: Maximální velikost souboru v bajtech.
- `CacheKey`: Klíč pro cache.

### `FileCheckerServiceTests`
Unit testy pro `FileCheckerService`.

#### Testy
- `CheckNumberOfFilesPerFolder_ReturnsNull_WhenFileCountIsWithinLimit()`: Testuje, že metoda `CheckNumberOfFilesPerFolder` vrací `null`, když je počet souborů v limitu.
- `CheckNumberOfFilesPerFolder_ReturnsErrorJson_WhenFileCountExceedsLimit()`: Testuje, že metoda `CheckNumberOfFilesPerFolder` vrací JSON objekt s chybovou zprávou, když je počet souborů překročen.
- `CheckFilesSize_ReturnsNull_WhenAllFilesAreWithinSizeLimit()`: Testuje, že metoda `CheckFilesSize` vrací `null`, když jsou všechny soubory v limitu velikosti.
- `CheckFilesSize_ReturnsErrorJson_WhenFilesExceedSizeLimit()`: Testuje, že metoda `CheckFilesSize` vrací JSON objekt s chybovou zprávou, když některý soubor překračuje maximální povolenou velikost.

## Závěr
Tato aplikace poskytuje základní funkce pro kontrolu souborů v adresáři, včetně kontroly počtu souborů a velikosti souborů. Aplikace je testována pomocí unit testů, které zajišťují správnou funkčnost metod pro kontrolu souborů.

# Možná vylepšení

- Konfigurace přenést do appsettings.json
- Asynchroní operace - pro zvýšení výkonu a škálovatelnosti aplikace
- Validace vstupů - pro kontrolu správnosti vstupních dat
- Bezpečnost - aplikace neřeší práva uživatelů na složkách a souborech
- Podpora multijazyčnosti
- Přidání stylů na front-end
- Použití jiného cache systému (třeba lazy cache)
- Upravení logování - přidat tabulku se změnami, které proběhly od spuštění aplikace. Aktuálně se zapomenou.
