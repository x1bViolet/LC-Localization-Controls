### Localization export UI:
![image](https://github.com/user-attachments/assets/0d9b8306-fd7e-42f5-b23a-454cc4e7091d)
  - Function for raw localization files export with font conversion of some files using the created crutch in the form of `replacement_map.json` and `merged_font.ttf` from [LimbusFonts](https://github.com/kimght/LimbusFonts), so that Limbus has more than 1 font in total (But in text editor you will see only squares).
  - Function to convert the Shorthand (Somethind like `[Keyword:'Name']`) version of keywords into a TextMeshPro format (`<sprite name="KeywordID"><color=#hexcolor><u><link="KeywordID">Name</color></u></link>`) understandable by the game is also configured as 'Shorthand' checkbox.
    - !! Manually add new keywords to files in your folder in `⇲ Asset Directory\Keywords\Text Sources` and their color in `⇲ Asset Directory\Keywords\Keywords@DefaultColordata.T[-]` to convert them too
  - Strongly recommended to use the 'Insert missing ID' checkbox in order to avoid widespread 'Unknown' instead of the original untranslated text, it manually adds missing ID in files and missing files too (ProjectMoon did not make this thing work in one of the latest patches for Custom TL in which they mentioned it).

### Settings
1. Regex options for Shorthands
![image](https://github.com/user-attachments/assets/9eaf76f3-7dd9-4cbe-8f29-3af85623335c)
   - Shows their custom regex pattern and how they will be converted at export.
   - Better to use https://regex101.com/r/WdEwd8/1 website to write pattern, or leave it as it is, group names as `?<Some>` on current pattern is neccessary.
   - Color in `?<Color>` group being recognized automatically as `#[a-fA-F0-9]{6}` after extracting group value, so this group can have any form. In this example there are round brackets and group originally has value `(#hexcolor)`.
 
2. Font options
![image](https://github.com/user-attachments/assets/7d950243-7c50-4924-b835-3fae6939ad26)
   - Shows defined in `replacement_map.json` symbol replacements for custom font.
   - Fonts placed in `⇲ Asset Directory\Font` folder.
     - In font folder: `.json` file in folder will be selected as replacement_map, `ttf` or `otf` file as font for preview, `.toml` as config with rules for files.


### Program config
![image_2025-04-28_22-41-07](https://github.com/user-attachments/assets/93a20427-c7fc-4007-bfb3-8250097dade6)
In `⇲ Asset Directory\Parameters.T[-]` file writed all application parameters.
   - The UI language can be changed via the `Selected Language` parameter. Languages are located in the `⇲ Asset Directory\Languages` and can also be created independently: copy English folder and rewrite the `.L[-]` file, if desired, you can also change the interface font through the `Custom font : … override` parameters in this file. Name of the folder with the new language in `⇲ Asset Directory\Languages` will be taken as a parameter, name of .L[-] file can be any.
   - `Missing ID source` parameter is responsible for a third-party folder with the original localization, from which files will be taken to insert the missing IDs and files themselves, placed in `⇲ Asset Directory\Missing ID sources`.
