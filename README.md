> Simplified version of LC Localization Controls without Shorthands or Special font parameters preview.
![image](https://github.com/user-attachments/assets/31a995ee-935a-4293-9b5d-cc92b044b4b2)
Only raw localization export to release version with missing IDs and files appending, generating overall report about export, plus additional functions:
- Shorthand-type keywords conversion to TextMeshPro by special pattern for simplified keywords spelling in raw version of localization (E.g. ```[Combustion:`Огня`]``` instead of `<sprite name="Combustion"><color=#e30000><u><link="Combustion">Огня</link></u></color>`).
  ![image](https://github.com/user-attachments/assets/6b863e04-a08b-4e42-ae63-c95e9788dcd0)
- Special fonts conversion by using symbols replacement map for them (Can work if font actually supports that, [LimbusFonts](https://github.com/kimght/LimbusFonts) as example. Means there are another font characters inside this font somewhere in unicode private use areas. In text editor being displayed as squares, but in limbus they turn into characters of a different font).
   - Usage: place in start of any property value `[font=fontname]` string that refers to the replacement_map.json file dictionary object, and all propery value symbols will be converted (Except `<tags>` or `{insertions}`, actual pattern on screenshot):
   ![image](https://github.com/user-attachments/assets/d524f02d-e6d8-4310-b5c4-6a554241002a)

Configuration file `⇲ Assets Directory\Configurazione.json`
![Image](https://github.com/user-attachments/assets/b49a7413-4988-43d1-96d0-6bb24f04e2bf)
