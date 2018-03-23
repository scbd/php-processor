# PhpProcessor

Extract basic information from PHP pdf into Excel.

## Usage


### Convert PDF to text

#### Mac/Unix

Run in the folder where PHP PDF (`*.pdf`) are (linux/Mac users)

```
$ for file in *.pdf; do pdftotext -raw "$file" "$file.txt"; done
$ for file in *.txt; do unix2dos "$file"; done
```

Install Mac
```
$ brew install poppler dos2unix
```

linux
```
$ apt-get install poppler-utils dos2unix
```
#### Windows

```
$ for /r %i in (*.pdf) do pdftotext -raw "%i" "%i.txt"
```

For Windows you can install it from http://www.foolabs.com/xpdf/download.html

### Extract information from the PHP text (`*.txt`) into `_output.txt`

```
$ PhpProcessor.exe path/to/folder/where/php-txt/files/are
```


#### Export to Excel

1. Open the `_output.txt`
2. Select all text `CTRL+A`
3. Copy `CTRL+C`
4. Create new Excel workbook
5. Paste `CTRL+V`

Et Voila
