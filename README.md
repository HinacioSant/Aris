# **Aris**     _\~\[+]\~\_ 

Aris is a Pdf editor application. It uses PdfPig, PdfiumViewer and Itext7 for extraction, displaying and editing of Pdfs.


# Re-Editing Files -

Attempting to Edit a already edited file can lead to errors MAINLY in Re-editing a word. 

_-Intended overall fix for the future-_

# Fonts -

Aris use the **Standard 14** as base fonts. To use fonts outside of that you should add custom fonts.

Custom Fonts should be added to manifest for FontHandler.

If added **CORRECTLY** Aris will display and edit the extracted words with the original font else Aris will use the default "Helvetica" font.

If any exception is raised durring font creation Aris will user the default "Helvetica" font.

### <ins>Standard 14 Fonts:<ins>
<sup>[ "Helvetica", "Helvetica-Bold", "Helvetica-Oblique", "Helvetica-BoldOblique", "Times-Roman", "Times-Bold", "Times-Italic", "Times-BoldItalic", "Courier", "Courier-Bold", "Courier-Oblique", "Courier-BoldOblique", "Symbol", "ZapfDingbats"]<sup>