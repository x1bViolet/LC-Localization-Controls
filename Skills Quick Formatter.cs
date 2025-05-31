using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Translation_Devouring_Siltcurrent.Requirements;

namespace Translation_Devouring_Siltcurrent
{
    internal abstract class SkillsQuickFormatter
    {
        internal protected static string AmountTransformConditionAlt(string ID, string Amount)
        {
            return ID switch
            {
                "Combustion" => "Огня",
                "Laceration" => "Кровотечения",
                "Vibration" => "Тремора",
                "Burst" => "Разрыва",
                "Sinking" => "Утопания",
                "Breath" => "Дыхания",
                _ => $"<color=#ffffff><size=200%><b>NULL</b></size></color>"
            };
        }

        internal protected static string AmountTransformCondition(string ID, string Amount)
        {
            return ID switch
            {
                "Combustion" => Amount switch
                {
                    "1" => "Огнём",
                    _ => "Огня"
                },
                "Laceration" => Amount switch
                {
                    "1" => "Кровотечением",
                    _ => "Кровотечения"
                },
                "Vibration" => Amount switch
                {
                    "1" => "Тремором",
                    _ => "Тремора"
                },
                "Burst" => Amount switch
                {
                    "1" => "Разрывом",
                    "2" => "Разрывам",
                    "3" => "Разрывам",
                    "4" => "Разрывам",
                    _ => "Разрывами"
                },
                "Sinking" => Amount switch
                {
                    "1" => "Утопанием",
                    _ => "Утопания"
                },
                "Breath" => Amount switch
                {
                    "1" => "Дыханием",
                    _ => "Дыхания"
                },
                "Agility" => Amount switch
                {
                    "1" => "Спешкой",
                    _ => "Спешки"
                },
                "Binding" => Amount switch
                {
                    "1" => "Связыванием",
                    _ => "Связывания"
                },
                _ => $"<color=#ffffff><size=200%><b>NULL</b></size></color>"
            };
        }

        internal protected static string BuffTransformCondition(string BuffName)
        {
            return BuffName switch
            {
                "Skill Final Power" => "Итоговая сила навыка",
                "Minus Coin Power" => "Сила отрицательных монет",
                "Plus Coin Power" => "Сила положительных монет",
                "Coin Power" => "Сила монет",
                "Base Power" => "Базовая сила",
                "Skill Power" => "Сила навыка",
                "Final Power" => "Итоговая сила",
                "Clash Power" => "Сила в столкновении",
                _ => BuffName
            };
        }
        internal protected static string BuffTransformConditionAlt(string BuffName)
        {
            return BuffName switch
            {
                "Skill Final Power" => "Итоговую силу навыка",
                "Minus Coin Power" => "Силу отрицательных монет",
                "Plus Coin Power" => "Силу положительных монет",
                "Coin Power" => "Силу монет",
                "Base Power" => "Базовую силу",
                "Skill Power" => "Силу навыка",
                "Final Power" => "Итоговую силу",
                "Clash Power" => "Силу в столкновении",
                _ => BuffName
            };
        }

        internal protected static string AmountTransform(string ID, string Amount)
        {
            return ID switch
            {
                "Combustion" => Amount switch
                {
                    "1" => "Огонь",
                    _ => "Огня"
                },
                "Laceration" => Amount switch
                {
                    "1" => "Кровотечение",
                    _ => "Кровотечения"
                },
                "Vibration" => Amount switch
                {
                    "1" => "Тремор",
                    _ => "Тремора"
                },
                "Burst" => Amount switch
                {
                    "1" => "Разрыв",
                    "2" => "Разрыва",
                    "3" => "Разрыва",
                    "4" => "Разрыва",
                    _ => "Разрывов"
                },
                "Sinking" => Amount switch
                {
                    "1" => "Утопание",
                    _ => "Утопания"
                },
                "Breath" => Amount switch
                {
                    "1" => "Дыхание",
                    _ => "Дыхания"
                },
                "Agility" => Amount switch
                {
                    "1" => "Спешку",
                    _   => "Спешки"
                },
                "Binding" => Amount switch
                {
                    "1" => "Связывание",
                    _   => "Связывания"
                },
                "Vulnerable" => Amount switch
                {
                    "1" => "Уязвимость",
                    "2" => "Уязвимости",
                    "3" => "Уязвимости",
                    "4" => "Уязвимости",
                    _   => "Уязвимостей"
                },
                "Protection" => Amount switch
                {
                    "1" => "Защищённость",
                    "2" => "Защищённости",
                    "3" => "Защищённости",
                    "4" => "Защищённости",
                    _   => "Защищённостей"
                },
                "Enhancement" => Amount switch
                {
                    "1" => "Повышение силы атаки",
                    "2" => "Повышения силы атаки",
                    "3" => "Повышения силы атаки",
                    "4" => "Повышения силы атаки",
                    _   => "Повышений силы атаки"
                },
                "Reduction" => Amount switch
                {
                    "1" => "Понижение силы атаки",
                    "2" => "Понижения силы атаки",
                    "3" => "Понижения силы атаки",
                    "4" => "Понижения силы атаки",
                    _   => "Понижений силы атаки"
                },
                "Endurance" => Amount switch
                {
                    "1" => "Повышение силы защиты",
                    "2" => "Повышения силы защиты",
                    "3" => "Повышения силы защиты",
                    "4" => "Повышения силы защиты",
                    _   => "Повышений силы защиты"
                },
                "Disarming" => Amount switch
                {
                    "1" => "Понижение силы защиты",
                    "2" => "Понижения силы защиты",
                    "3" => "Понижения силы защиты",
                    "4" => "Понижения силы защиты",
                    _   => "Понижений силы защиты"
                },
                "ResultEnhancement" => Amount switch
                {
                    "1" => "Повышение силы",
                    "2" => "Повышения силы",
                    "3" => "Повышения силы",
                    "4" => "Повышения силы",
                    _   => "Повышений силы"
                },
                "ResultReduction" => Amount switch
                {
                    "1" => "Понижение силы",
                    "2" => "Понижения силы",
                    "3" => "Понижения силы",
                    "4" => "Понижения силы",
                    _   => "Понижений силы"
                },


                "PlusCoinValueUp" => Amount switch
                {
                    "1" => "Усиление положительной монеты",
                    "2" => "Усиления положительной монеты",
                    "3" => "Усиления положительной монеты",
                    "4" => "Усиления положительной монеты",
                    _ => "Усилений положительной монеты"
                },
                "PlusCoinValueDown" => Amount switch
                {
                    "1" => "Ослабление положительной монеты",
                    "2" => "Ослабления положительной монеты",
                    "3" => "Ослабления положительной монеты",
                    "4" => "Ослабления положительной монеты",
                    _   => "Ослаблений положительной монеты"
                },


                "MinusCoinValueUp" => Amount switch
                {
                    "1" => "Усиление отрицательный монеты",
                    "2" => "Усиления отрицательный монеты",
                    "3" => "Усиления отрицательный монеты",
                    "4" => "Усиления отрицательный монеты",
                    _   => "Усилений отрицательной монеты"
                },
                "MinusCoinValueDown" => Amount switch
                {
                    "1" => "Ослабление отрицательный монеты",
                    "2" => "Ослабления отрицательный монеты",
                    "3" => "Ослабления отрицательный монеты",
                    "4" => "Ослабления отрицательный монеты",
                    _   => "Ослаблений отрицательный монеты"
                },


                "SlashResistUp" => Amount switch
                {
                    "1" => "Повышение сопротивления к Рубящему урону",
                    "2" => "Повышения сопротивления к Рубящему урону",
                    "3" => "Повышения сопротивления к Рубящему урону",
                    "4" => "Повышения сопротивления к Рубящему урону",
                    _   => "Повышений сопротивления к Рубящему урону"
                },
                "SlashResistDown" => Amount switch
                {
                    "1" => "Понижение сопротивления к Рубящему урону",
                    "2" => "Понижения сопротивления к Рубящему урону",
                    "3" => "Понижения сопротивления к Рубящему урону",
                    "4" => "Понижения сопротивления к Рубящему урону",
                    _   => "Понижений сопротивления к Рубящему урону"
                },

                "PenetrateResistUp" => Amount switch
                {
                    "1" => "Повышение сопротивления к Пронзающему урону",
                    "2" => "Повышения сопротивления к Пронзающему урону",
                    "3" => "Повышения сопротивления к Пронзающему урону",
                    "4" => "Повышения сопротивления к Пронзающему урону",
                    _   => "Повышений сопротивления к Пронзающему урону"
                },
                "PenetrateResistDown" => Amount switch
                {
                    "1" => "Понижение сопротивления к Пронзающему урону",
                    "2" => "Понижения сопротивления к Пронзающему урону",
                    "3" => "Понижения сопротивления к Пронзающему урону",
                    "4" => "Понижения сопротивления к Пронзающему урону",
                    _   => "Понижений сопротивления к Пронзающему урону"
                },

                "HitResistUp" => Amount switch
                {
                    "1" => "Повышение сопротивления к Рубящему урону",
                    "2" => "Повышения сопротивления к Рубящему урону",
                    "3" => "Повышения сопротивления к Рубящему урону",
                    "4" => "Повышения сопротивления к Рубящему урону",
                    _   => "Повышений сопротивления к Рубящему урону"
                },
                "HitResistDown" => Amount switch
                {
                    "1" => "Понижение сопротивления к Дробящему урону",
                    "2" => "Понижения сопротивления к Дробящему урону",
                    "3" => "Понижения сопротивления к Дробящему урону",
                    "4" => "Понижения сопротивления к Дробящему урону",
                    _   => "Понижений сопротивления к Дробящему урону"
                },



                "SlashDamageUp" => Amount switch
                {
                    "1" => "Повышение Рубящего урона",
                    "2" => "Повышения Рубящего урона",
                    "3" => "Повышения Рубящего урона",
                    "4" => "Повышения Рубящего урона",
                    _   => "Повышений Рубящего урона"
                },
                "PenetrateDamageUp" => Amount switch
                {
                    "1" => "Повышение Дробящего урона",
                    "2" => "Повышения Дробящего урона",
                    "3" => "Повышения Дробящего урона",
                    "4" => "Повышения Дробящего урона",
                    _   => "Повышений Дробящего урона"
                },
                "HitDamageUp" => Amount switch
                {
                    "1" => "Повышение Дробящего урона",
                    "2" => "Повышения Дробящего урона",
                    "3" => "Повышения Дробящего урона",
                    "4" => "Повышения Дробящего урона",
                    _   => "Повышений Дробящего урона"
                },


                "AttackDmgUp" => Amount switch
                {
                    "1" => "Повышение урона",
                    "2" => "Повышения урона",
                    "3" => "Повышения урона",
                    "4" => "Повышения урона",
                    _   => "Повышений урона"
                },
                "AttackDmgDown" => Amount switch
                {
                    "1" => "Понижение урона",
                    "2" => "Понижения урона",
                    "3" => "Понижения урона",
                    "4" => "Понижения урона",
                    _   => "Понижений урона"
                },
                "DefenseUp" => Amount switch
                {
                    "1" => "Повышение уровня защиты",
                    "2" => "Повышения уровня защиты",
                    "3" => "Повышения уровня защиты",
                    "4" => "Повышения уровня защиты",
                    _ => "Повышений уровня защиты"
                },
                "DefenseDown" => Amount switch
                {
                    "1" => "Понижение уровня защиты",
                    "2" => "Понижения уровня защиты",
                    "3" => "Понижения уровня защиты",
                    "4" => "Понижения уровня защиты",
                    _ => "Понижений уровня защиты"
                },
                "AttackUp" => Amount switch
                {
                    "1" => "Повышение уровня атаки",
                    "2" => "Повышения уровня атаки",
                    "3" => "Повышения уровня атаки",
                    "4" => "Повышения уровня атаки",
                    _ => "Повышений уровня атаки"
                },
                "AttackDown" => Amount switch
                {
                    "1" => "Понижение уровня атаки",
                    "2" => "Понижения уровня атаки",
                    "3" => "Понижения уровня атаки",
                    "4" => "Понижения уровня атаки",
                    _ => "Понижений уровня атаки"
                },
                "Paralysis" => Amount switch
                {
                    "1" => "Паралич",
                    "2" => "Паралича",
                    "3" => "Паралича",
                    "4" => "Паралича",
                    _ => "Параличей"
                },
                _ => $"<color=#ffffff><size=200%><b>NULL</b></size></color>",
            };
        }

        internal protected static string PlaceStyleHighlightPlaceholders(string TargetSkillJsonFileText)
        {
            int Replacements = 0;
            JToken JParser = JToken.Parse(TargetSkillJsonFileText);
            foreach (JToken StringItem in JParser.SelectTokens("$.dataList[*].levelList[*].coinlist[*].coindescs[*].desc"))
            {
                if (!$"{StringItem}".Contains("<style=\"highlight\">") & !$"{StringItem}".Equals(""))
                {
                    StringItem.Replace($"{StringItem}<style=\"highlight\"></style>");
                    Replacements++;
                }
            }
            foreach (JToken StringItem in JParser.SelectTokens("$.dataList[*].levelList[*].desc"))
            {
                if (!$"{StringItem}".Contains("<style=\"highlight\">") & !$"{StringItem}".Equals(""))
                {
                    StringItem.Replace($"{StringItem}<style=\"highlight\"></style>");
                    Replacements++;
                }
            }

            if (Replacements > 0)
            {
                return JParser.ToString(Formatting.Indented).Replace("\r\n", "\n");
            }
            else
            {
                return TargetSkillJsonFileText;
            }
        }

        internal protected static string QuickConditionsAndInflictsConvert(string TargetSkillJsonFileText)
        {
            JToken JParser = JToken.Parse(TargetSkillJsonFileText);
            foreach (JToken StringItem in JParser.SelectTokens("$.dataList[*].levelList[*].coinlist[*].coindescs[*].desc"))
            {
                string CoinDesc = $"{StringItem}";


                /*lang=regex*/
                RegexTransform(ref CoinDesc, @"Inflict \[(?<ID>\w+)\]", Match =>
                {
                    string ExportString = "";
                    DictionaryWithDefault<dynamic, string> T = ExtractGroupValues(Match);

                    ExportString += $"Накладывает [{T["ID"]}:`<color=#ffffff><size=200%><b>NULL</b></size></color>`]";

                    return ExportString;
                });


                /*lang=regex*/
                RegexTransform(ref CoinDesc, @"(?<InflictCase>Inflict|inflict) (?<CountAmount>\+)?(?<Amount>\d+) \[(?<ID>\w+)\](?<AndCondition> and \+(?<Amount2>\d+) \[(?<ID2>\w+)\] Count)?( Count)?(?<NextTurn> next turn)?", Match =>
                {
                    string ExportString = "";
                    DictionaryWithDefault<dynamic, string> T = ExtractGroupValues(Match);


                    if (T["CountAmount"].Equals(""))
                    {
                        ExportString += $"{(T["InflictCase"].Equals("Inflict") ? "Наносит" : "наносит")} {T["Amount"]} <Name Placeholder>";
                    }
                    else
                    {
                        ExportString += $"Повышает счётчик <Name Placeholder> на {T["Amount"]}";
                    }

                    if (!T["AndCondition"].Equals("") & T["ID2"].Equals(T["ID"]))
                    {
                        ExportString += $" и повышает его счётчик на {T["Amount2"]}";
                    }

                    string KeywordText = AmountTransform(T["ID"], T["Amount"]);

                    ExportString = ExportString.Replace("<Name Placeholder>", !KeywordText.Equals("") ? $"[{T["ID"]}:`{KeywordText}`]" : $"[{T["ID"]}:`<color=#ffffff><size=200%><b>NULL</b></size></color>`]");

                    if (!T["NextTurn"].Equals(""))
                    {
                        ExportString += " на следующий ход";
                    }

                    //rin($"{T[0]} : {ExportString}");

                    return ExportString;
                });

                /*lang=regex*/
                RegexTransform(ref CoinDesc, @"(?<GainCase>Gain|gain) (?<CountAmount>\+)?(?<Amount>\d+) \[(?<ID>\w+)\]( Potency| Count)?(?<AndCondition> and \+(?<CountAmount2>\d+) \[(?<ID2>\w+)\] Count)?( next turn)?", Match =>
                {
                    string ExportString = "";
                    DictionaryWithDefault<dynamic, string> T = ExtractGroupValues(Match);

                    if (T["CountAmount"].Equals(""))
                    {
                        ExportString += $"{(T["GainCase"].Equals("Gain") ? "Получает" : "получает")} {T["Amount"]} <Name Placeholder>";
                    }
                    else
                    {
                        ExportString += $"{(T["GainCase"].Equals("Gain") ? "Повышает" : "повышает")} свой счётчик <Name Placeholder> на {T["Amount"]}";
                    }

                    if (!T["AndCondition"].Equals("") & T["ID2"].Equals(T["ID"]))
                    {
                        ExportString += $" и повышает его счётчик на {T["CountAmount2"]}";
                    }


                    string KeywordText = T["ID"].Equals("Charge") ? "Заряда" : AmountTransform(T["ID"], T["Amount"]);

                    ExportString = ExportString.Replace("<Name Placeholder>", $"[{T["ID"]}:`{KeywordText}`]");

                    rin($"{T[0]} : {ExportString}");

                    return ExportString;
                });

                rin(CoinDesc);

                StringItem.Replace(CoinDesc);
            }











            foreach (JToken StringItem in JParser.SelectTokens("$.dataList[*].levelList[*].desc"))
            {
                string SkillDesc = $"{StringItem}";

                /*lang=regex*/
                RegexTransform(ref SkillDesc, @"(?<Type>Heal|Lose) (?<Amount>\d+) (?<What>HP|SP)", Match =>
                {
                    string ExportString = "";
                    DictionaryWithDefault<dynamic, string> T = ExtractGroupValues(Match);

                    ExportString += $"{(T["Type"].Equals("Heal") ? "Восстанавливает" : "Треяет")} {T["Amount"]} {(T["Type"].Equals("HP") ? "ОЗ" : "СД")}";

                    return ExportString;
                });

               /*lang=regex*/
                RegexTransform(ref SkillDesc, @"If this unit's Speed is faster than the(?<ThanMainTarget> main) target's gain (?<BuffName>Skill Power|Skill Final Power|Final Power|Clash Power|Base Power|Plus Coin Power|Minus Coin Power|Coin Power) based on Speed difference \((?<BuffName>Skill Power|Skill Final Power|Final Power|Clash Power|Base Power|Plus Coin Power|Minus Coin Power|Coin Power) \+(?<BuffAmount>\d+) for every (?<BuffAmountForEveryX>\d+) Speed difference; max (?<BuffAmountMax>\d+)\)", Match =>
                {
                    string ExportString = "";
                    DictionaryWithDefault<dynamic, string> T = ExtractGroupValues(Match);

                    ExportString += $"Если своя скорость выше скорости{(!T["ThanMainTarget"].Equals("") ? " главной" : "")} цели, {BuffTransformCondition(T["BuffName"])} +{T["BuffAmount"]} за каждые {T["BuffAmountForEveryX"]} единицы разницы в скорости (До {T["BuffAmountMax"]})";

                    return ExportString;
                });



                /*lang=regex*/
                RegexTransform(ref SkillDesc, @"Inflict (\+)?(?<InflictAmount>\d+) \[(?<ID>\w+)\](?<InflictsCount> Count)?(?<AndInflictCount> and \+(?<AndInflictCountAmount>\d+) \[(?<ID>\w+)\] Count)?", Match =>
                {
                    string ExportString = "";
                    DictionaryWithDefault<dynamic, string> T = ExtractGroupValues(Match);

                    if (!T["InflictsCount"].Equals(""))
                    {
                        ExportString += $"Повышает счётчик [{T["ID"]}:`{AmountTransformConditionAlt(T["ID"], T["InflictAmount"])}`] цели на {T["InflictAmount"]}";
                        rin(ExportString);
                    }
                    else
                    {
                        ExportString += $"Наносит {T["InflictAmount"]} [{T["ID"]}:`{AmountTransformCondition(T["ID"], T["InflictAmount"])}`]";

                        if (!T["AndInflictCount"].Equals(""))
                        {
                            ExportString += $" и повышает его счётчик на {T["AndInflictCountAmount"]}";
                        }
                    }

                    return ExportString;
                });

                /*lang=regex*/
                RegexTransform(ref SkillDesc, @"Consume (?<ConsumeAmount>\d+) \[(?<ConsumeBuff>Charge|Vibration)\] Count( on self)? to gain (?<BuffName>Skill Power|Skill Final Power|Final Power|Clash Power|Base Power|Plus Coin Power|Minus Coin Power|Coin Power) (?<BuffMath>\-|\+)(?<BuffAmount>\d+)(?<ForEveryXMaximum> \((Max|max) (?<ForEveryXMaximumAmount>\d+)\))?", Match =>
                {
                    string ExportString = "";
                    DictionaryWithDefault<dynamic, string> T = ExtractGroupValues(Match);

                    ExportString += $"Тратит{(!T["ForEveryXMaximumAmount"].Equals("") ? " по" : "")} {T["ConsumeAmount"]} единиц счётчика [{T["ConsumeBuff"]}:`{(T["ConsumeBuff"].Equals("Vibration") ? "Тремора" : "Заряда")}`], чтобы {(T["BuffMath"].Equals("+") ? "повысить" : "понизить")} {BuffTransformConditionAlt(T["BuffName"])} на {T["BuffAmount"]}{(!T["ForEveryXMaximumAmount"].Equals("") ? $" (До {T["ForEveryXMaximumAmount"]})" : "")}";

                    return ExportString;
                });

                /*lang=regex*/
                RegexTransform(ref SkillDesc, @"(?<IfTargetHas>If target has (?<IfTargetHasAmount>\d+)\+ \[(?<IfTargetHasID>\w+)\](?<IsCountHere> Count)?, )?(?<BuffName>Skill Power|Skill Final Power|Final Power|Clash Power|Base Power|Plus Coin Power|Minus Coin Power|Coin Power) (?<BuffMath>\-|\+)(?<BuffAmount>\d+)(?<ForEveryX> for every (?<ForEveryXAmount>\d+) \[(?<ForEveryXID>\w+)\](?<ForEveryXCount> Count)? on (?<ForEveryXWhoOn>(the )?Self|self|(the )?target|Target)(?<ForEveryXMaximum> \((Max|max) (?<ForEveryXMaximumAmount>\d+)\)))?", Match =>
                {
                    string ExportString = "";
                    DictionaryWithDefault<dynamic, string> T = ExtractGroupValues(Match);

                    string BuffName = BuffTransformCondition(T["BuffName"]);

                    ExportString += $"{BuffName} {T["BuffMath"]}{T["BuffAmount"]}";

                    if (!T["ForEveryX"].Equals(""))
                    {
                        ExportString += $" за каждые {T["ForEveryXAmount"]}{(!T["ForEveryXCount"].Equals("") ? " единицы счётчика" : "")} [{T["ForEveryXID"]}:`{AmountTransform(T["ForEveryXID"], T["ForEveryXAmount"])}`]";
                        if (T["ForEveryXWhoOn"].ToLower().EndsWith("self"))
                        {
                            ExportString += "";
                        }
                        else if (T["ForEveryXWhoOn"].ToLower().EndsWith("target"))
                        {
                            ExportString += $"{(T["ForEveryXCount"].Equals("") ? " на" : "")} цели";
                        }
                    }

                    if (!T["IfTargetHas"].Equals(""))
                    {
                        if (T["IsCountHere"].Equals(""))
                        {
                            ExportString += $" против целей с {T["IfTargetHasAmount"]} и более [{T["IfTargetHasID"]}:`{AmountTransformCondition(T["IfTargetHasID"], T["IfTargetHasAmount"])}`]";
                        }
                        else
                        {
                            ExportString += $" против целей с счётчиком [{T["IfTargetHasID"]}:`{AmountTransformCondition(T["IfTargetHasID"], T["IfTargetHasAmount"])}`] {T["IfTargetHasAmount"]} и более";
                        }
                    }

                    if (!T["ForEveryXMaximum"].Equals(""))
                    {
                        ExportString += $" (До {T["ForEveryXMaximumAmount"]})";
                    }

                    return ExportString;
                });

                /*lang=regex*/
                RegexTransform(ref SkillDesc, @"(?<GainCase>Gain|gain) (?<CountAmount>\+)?(?<Amount>\d+) \[(?<ID>\w+)\]( Potency| Count)?(?<AndCondition> and \+(?<CountAmount2>\d+) \[(?<ID2>\w+)\] Count)?( next turn)?", Match =>
                {
                    string ExportString = "";
                    DictionaryWithDefault<dynamic, string> T = ExtractGroupValues(Match);

                    if (T["CountAmount"].Equals(""))
                    {
                        ExportString += $"{(T["GainCase"].Equals("Gain") ? "Получает" : "получает")} {T["Amount"]} <Name Placeholder>";
                    }
                    else
                    {
                        ExportString += $"{(T["GainCase"].Equals("Gain") ? "Повышает" : "повышает")} свой счётчик <Name Placeholder> на {T["Amount"]}";
                    }

                    if (!T["AndCondition"].Equals("") & T["ID2"].Equals(T["ID"]))
                    {
                        ExportString += $" и повышает его счётчик на {T["CountAmount2"]}";
                    }


                    string KeywordText = T["ID"].Equals("Charge") ? "Заряда" : AmountTransform(T["ID"], T["Amount"]);

                    ExportString = ExportString.Replace("<Name Placeholder>", $"[{T["ID"]}:`{KeywordText}`]");

                    rin($"{T[0]} : {ExportString}");

                    return ExportString;
                });

                rin(SkillDesc);

                StringItem.Replace(SkillDesc);
            }

            return JParser.ToString(Formatting.Indented).Replace("\r\n", "\n");
        }
    }
}
