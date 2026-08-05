#       Aris ~[+]~

Aris is a WPF MVVM PDF editor that uses OCR-based text replacement.

In here I'll explain Project [Structure](https://github.com/HinacioSant/Aris#Structure), file [Responsibilities](https://github.com/HinacioSant/Aris#Responsibilities), important [Classes](https://github.com/HinacioSant/Aris#Classes) and [Data](https://github.com/HinacioSant/Aris#Data) flow.

# Structure

| Folder | Description |
| --- | --- |
| Fonts/ | Personalized fonts(outside the standard 14) for editing and displaying |
| Helpers/ | Helper holds all the UI text based animation |
| Models/ | Models hold the all records and models including result for error handling  |
| Pages/ | WPF pages |
| Services/ | Error handling, Font management, Page sizing, Json handling, Nav Services, Output path generation, [Text Extraction, Extraction filtering and word duplication handling and PDF word replacing logic], PDF Loading  |
| Themes/ | UI Styling |

<sub>"I'm aware of the need to decople certain files and reorganisation"<sub>



# Responsibilities

| File | Description |
| --- | --- |
| MainWindow | Base UI and navgation |
| Pages/Home | File upload  |
| Pages/Viewer | UI of PDF Loading, displaying and editing |
| Services | Heavy operation like OCR, PDF manipulation, Extracted text filtering |

