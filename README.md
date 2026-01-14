## A program for performing various operations on localization files of Limbus Company

### Latest version: [1.0:0 Release](https://github.com/x1bViolet/LC-Localization-Controls/releases/tag/1.0%CB%900)

### Functions list:
- **The main principle of operation**: copy files from Source directory to Destination directory and perform various operations on them as specified in the current configuration profile.
- Specify .json files format:
  - Formatting (Indented, None).
  - Indentation size.
  - Line break mode (LF, CR, CRLF).
  - UTF-8 BOM/BOM-less encoding.
- Append missing IDs and files by whitelists with Wildcard patterns. It is intended to be used as a mechanism for automatically adding new content after updates in Limbus to your localization instead of manually editing json files. In this case you need to set your workspace direcory as Source and Destination to factually overwrite files. **(However, I advise you to create a backup of your files if you have unsaved changes and trying to use this program for the first time)**
  - By default, options from previously mentioned function are selected to preserve the current file formatting when resaving json files, Therefore, there is no need to worry about unnecessary changes, such as UTF-8 BOM conversion to UTF-8 or change of Indentation size.
- Copy selected font files as Context and Title fonts for Limbus Custom Languages.
- JsonPath regex conversions. Also intended to be used as a mechanism for automatic translation of a new content after updates in Limbus via text replacements via Regular expressions instead of manually editing json files in which the content complexity is quite light and translation can be easily resolved by auto-replacements.
- Shorthands conversion: replacing the alternative form of keywords in the localization files of combat descriptions with a full set of tags of the text from TextMeshPro.
- Merged fonts conversion. Detailed information is provided in the program itself above the option in the form of a comment.
> The explanation of each function is written in the program itself as comments (Remarks) above the options.
