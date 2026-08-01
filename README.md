# **Aris**     _~\[+]\~_ 

Aris is a PDF editor and word-processor replacement tool. It uses PdfPig, PdfiumViewer.Updated and Itext7 for extraction, displaying and editing of Pdfs.

<img width="1600" height="860" alt="Aris GIF" src="https://github.com/user-attachments/assets/da7586a6-80c5-4eb8-84c2-99d6d3222598" />


# Status 

This is version 0.1.0 of Aris. 

[[Install]](https://github.com/HinacioSant/Aris#Install)

It consist of all my Initial intended capabilities fulfilling my current personal needs. For future intended features check [[Future]](https://github.com/HinacioSant/Aris#future)

Files are save in their original folder with the prefix [Edited]

# Re-Editing Files 

Attempting to Edit a already edited file can lead to errors **MAINLY** in Re-editing a word. 

_-Intended overall fix for the future-_

# Prerequisites

**OS -** [Windows]

**.Net SDK -** [Version: .Net 10.0]

# Install

See [[Prerequisites]](https://github.com/HinacioSant/Aris#Prerequisites)

**Clone -** 

```
Git clone https://github.com/HinacioSant/Aris.git
```

**Run -**

```
cd Aris

dotnet Run
```


# Dependencies

**Pdf Editing -**  [ Itext7 Version: 9.6.0 - Itext7.bouncy-castle-adapter Version: 9.6.0 ]

**Pdf Displaying -** [ PdfiumViewer.Native.x86_64.v8-xfa Version: 2018.4.8.256  - PdfiumViewer.Updated Version: 2.14.5 ]

**Pdf Word Extraction -** [ PdfPig Version: 0.1.14 ]

# Fonts 

Aris use the **Standard 14** as base fonts. To use fonts outside of that you should add custom fonts.

Custom Fonts should be added to manifest for FontHandler.

If added **CORRECTLY** Aris will display and edit the extracted words with the original font else Aris will use the default "Helvetica" font.

If any exception is raised durring font creation Aris will user the default "Helvetica" font.

### <ins>Standard 14 Fonts:<ins>
<sup>[ "Helvetica", "Helvetica-Bold", "Helvetica-Oblique", "Helvetica-BoldOblique", "Times-Roman", "Times-Bold", "Times-Italic", "Times-BoldItalic", "Courier", "Courier-Bold", "Courier-Oblique", "Courier-BoldOblique", "Symbol", "ZapfDingbats"]<sup>

# Future

Remove old content stream operatores on every edit using a port of [PdfCanvasProcessor](https://github.com/mkl-public/testarea-itext7/blob/master/src/main/java/mkl/testarea/itext7/content/PdfCanvasEditor.java) for C#.


# License

AGPL-3.0-or-later. See -- [LICENSE](https://github.com/HinacioSant/Aris/blob/main/LICENSE)