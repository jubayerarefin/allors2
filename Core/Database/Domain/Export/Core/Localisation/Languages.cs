﻿// <copyright file="Languages.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    public partial class Languages
    {
        private Sticky<string, Language> languageByCode;

        public Sticky<string, Language> LanguageByCode => this.languageByCode ?? (this.languageByCode = new Sticky<string, Language>(this.Session, this.Meta.IsoCode));

        protected override void CoreSetup(Setup setup)
        {
            var data = new[,]
            {
                { "ab", "Abkhaz", "аҧсуа" },
                { "aa", "Afar", "Afaraf" },
                { "af", "Afrikaans", "Afrikaans" },
                { "ak", "Akan", "Akan" },
                { "sq", "Albanian", "Shqip" },
                { "am", "Amharic", "አማርኛ" },
                { "ar", "Arabic", "العربية" },
                { "an", "Aragonese", "Aragonés" },
                { "hy", "Armenian", "Հայերեն" },
                { "as", "Assamese", "অসমীয়া" },
                { "av", "Avaric", "авар мацӀ, магӀарул мацӀ" },
                { "ae", "Avestan", "avesta" },
                { "ay", "Aymara", "aymar aru" },
                { "az", "Azerbaijani", "azərbaycan dili" },
                { "bm", "Bambara", "bamanankan" },
                { "ba", "Bashkir", "башҡорт теле" },
                { "eu", "Basque", "euskara, euskera" },
                { "be", "Belarusian", "Беларуская" },
                { "bn", "Bengali", "বাংলা" },
                { "bh", "Bihari", "भोजपुरी" },
                { "bi", "Bislama", "Bislama" },
                { "bs", "Bosnian", "bosanski jezik" },
                { "br", "Breton", "brezhoneg" },
                { "bg", "Bulgarian", "български език" },
                { "my", "Burmese", "ဗမာစာ" },
                { "ca", "Catalan; Valencian", "Català" },
                { "ch", "Chamorro", "Chamoru" },
                { "ce", "Chechen", "нохчийн мотт" },
                { "ny", "Chichewa; Chewa; Nyanja", "chiCheŵa, chinyanja" },
                { "zh", "Chinese", "中文 (Zhōngwén), 汉语, 漢語" },
                { "cv", "Chuvash", "чӑваш чӗлхи" },
                { "kw", "Cornish", "Kernewek" },
                { "co", "Corsican", "corsu, lingua corsa" },
                { "cr", "Cree", "ᓀᐦᐃᔭᐍᐏᐣ" },
                { "hr", "Croatian", "hrvatski" },
                { "cs", "Czech", "česky, čeština" },
                { "da", "Danish", "dansk" },
                { "dv", "Divehi; Dhivehi; Maldivian;", "ދިވެހި" },
                { "nl", "Dutch", "Nederlands, Vlaams" },
                { "en", "English", "English" },
                { "eo", "Esperanto", "Esperanto" },
                { "et", "Estonian", "eesti, eesti keel" },
                { "ee", "Ewe", "Eʋegbe" },
                { "fo", "Faroese", "føroyskt" },
                { "fj", "Fijian", "vosa Vakaviti" },
                { "fi", "Finnish", "suomi, suomen kieli" },
                { "fr", "French", "français, langue française" },
                { "ff", "Fula; Fulah; Pulaar; Pular", "Fulfulde, Pulaar, Pular" },
                { "gl", "Galician", "Galego" },
                { "ka", "Georgian", "ქართული" },
                { "de", "German", "Deutsch" },
                { "el", "Greek, Modern", "Ελληνικά" },
                { "gn", "Guaraní", "Avañeẽ" },
                { "gu", "Gujarati", "ગુજરાતી" },
                { "ht", "Haitian; Haitian Creole", "Kreyòl ayisyen" },
                { "ha", "Hausa", "Hausa, هَوُسَ" },
                { "he", "Hebrew (modern)", "עברית" },
                { "hz", "Herero", "Otjiherero" },
                { "hi", "Hindi", "हिन्दी, हिंदी" },
                { "ho", "Hiri Motu", "Hiri Motu" },
                { "hu", "Hungarian", "Magyar" },
                { "ia", "Interlingua", "Interlingua" },
                { "id", "Indonesian", "Bahasa Indonesia" },
                { "ie", "Interlingue", "Originally called Occidental; then Interlingue after WWII" },
                { "ga", "Irish", "Gaeilge" },
                { "ig", "Igbo", "Asụsụ Igbo" },
                { "ik", "Inupiaq", "Iñupiaq, Iñupiatun" },
                { "io", "Ido", "Ido" },
                { "is", "Icelandic", "Íslenska" },
                { "it", "Italian", "Italiano" },
                { "iu", "Inuktitut", "ᐃᓄᒃᑎᑐᑦ" },
                { "ja", "Japanese", "日本語 (にほんご／にっぽんご)" },
                { "jv", "Javanese", "basa Jawa" },
                { "kl", "Kalaallisut, Greenlandic", "kalaallisut, kalaallit oqaasii" },
                { "kn", "Kannada", "ಕನ್ನಡ" },
                { "kr", "Kanuri", "Kanuri" },
                { "ks", "Kashmiri", "कश्मीरी, كشميري‎" },
                { "kk", "Kazakh", "Қазақ тілі" },
                { "km", "Khmer", "ភាសាខ្មែរ" },
                { "ki", "Kikuyu, Gikuyu", "Gĩkũyũ" },
                { "rw", "Kinyarwanda", "Ikinyarwanda" },
                { "ky", "Kirghiz, Kyrgyz", "кыргыз тили" },
                { "kv", "Komi", "коми кыв" },
                { "kg", "Kongo", "KiKongo" },
                { "ko", "Korean", "한국어 (韓國語), 조선말 (朝鮮語)" },
                { "ku", "Kurdish", "Kurdî, كوردی‎" },
                { "kj", "Kwanyama, Kuanyama", "Kuanyama" },
                { "la", "Latin", "latine, lingua latina" },
                { "lb", "Luxembourgish, Letzeburgesch", "Lëtzebuergesch" },
                { "lg", "Luganda", "Luganda" },
                { "li", "Limburgish, Limburgan, Limburger", "Limburgs" },
                { "ln", "Lingala", "Lingála" },
                { "lo", "Lao", "ພາສາລາວ" },
                { "lt", "Lithuanian", "lietuvių kalba" },
                { "lu", "Luba-Katanga", "Luba-Katanga" },
                { "lv", "Latvian", "latviešu valoda" },
                { "gv", "Manx", "Gaelg, Gailck" },
                { "mk", "Macedonian", "македонски јазик" },
                { "mg", "Malagasy", "Malagasy fiteny" },
                { "ms", "Malay", "bahasa Melayu, بهاس ملايو‎" },
                { "ml", "Malayalam", "മലയാളം" },
                { "mt", "Maltese", "Malti" },
                { "mi", "Māori", "te reo Māori" },
                { "mr", "Marathi (Marāṭhī)", "मराठी" },
                { "mh", "Marshallese", "Kajin M̧ajeļ" },
                { "mn", "Mongolian", "монгол" },
                { "na", "Nauru", "Ekakairũ Naoero" },
                { "nv", "Navajo, Navaho", "Diné bizaad, Dinékʼehǰí" },
                { "nb", "Norwegian Bokmål", "Norsk bokmål" },
                { "nd", "North Ndebele", "isiNdebele (North)" },
                { "ne", "Nepali", "नेपाली" },
                { "ng", "Ndonga", "Owambo" },
                { "nn", "Norwegian Nynorsk", "Norsk nynorsk" },
                { "no", "Norwegian", "Norsk" },
                { "ii", "Nuosu", "ꆈꌠ꒿ Nuosuhxop" },
                { "nr", "South Ndebele", "isiNdebele (South)" },
                { "oc", "Occitan", "Occitan" },
                { "oj", "Ojibwe, Ojibwa", "ᐊᓂᔑᓈᐯᒧᐎᓐ" },
                { "cu", "Old Church Slavonic, Church Slavic, Church Slavonic, Old Bulgarian, Old Slavonic", "ѩзыкъ словѣньскъ" },
                { "om", "Oromo", "Afaan Oromoo" },
                { "or", "Oriya", "ଓଡ଼ିଆ" },
                { "os", "Ossetian, Ossetic", "ирон æвзаг" },
                { "pa", "Panjabi, Punjabi", "ਪੰਜਾਬੀ, پنجابی‎" },
                { "pi", "Pāli", "पाऴि" },
                { "fa", "Persian", "فارسی" },
                { "pl", "Polish", "polski" },
                { "ps", "Pashto, Pushto", "پښتو" },
                { "pt", "Portuguese", "Português" },
                { "qu", "Quechua", "Runa Simi, Kichwa" },
                { "rm", "Romansh", "rumantsch grischun" },
                { "rn", "Kirundi", "kiRundi" },
                { "ro", "Romanian, Moldavian, Moldovan", "română" },
                { "ru", "Russian", "русский язык" },
                { "sa", "Sanskrit (Saṁskṛta)", "संस्कृतम्" },
                { "sc", "Sardinian", "sardu" },
                { "sd", "Sindhi", "सिन्धी, سنڌي، سندھی‎" },
                { "se", "Northern Sami", "Davvisámegiella" },
                { "sm", "Samoan", "gagana faa Samoa" },
                { "sg", "Sango", "yângâ tî sängö" },
                { "sr", "Serbian", "српски језик" },
                { "gd", "Scottish Gaelic; Gaelic", "Gàidhlig" },
                { "sn", "Shona", "chiShona" },
                { "si", "Sinhala, Sinhalese", "සිංහල" },
                { "sk", "Slovak", "slovenčina" },
                { "sl", "Slovene", "slovenščina" },
                { "so", "Somali", "Soomaaliga, af Soomaali" },
                { "st", "Southern Sotho", "Sesotho" },
                { "es", "Spanish; Castilian", "español, castellano" },
                { "su", "Sundanese", "Basa Sunda" },
                { "sw", "Swahili", "Kiswahili" },
                { "ss", "Swati", "SiSwati" },
                { "sv", "Swedish", "svenska" },
                { "ta", "Tamil", "தமிழ்" },
                { "te", "Telugu", "తెలుగు" },
                { "tg", "Tajik", "тоҷикӣ, toğikī, تاجیکی‎" },
                { "th", "Thai", "ไทย" },
                { "ti", "Tigrinya", "ትግርኛ" },
                { "bo", "Tibetan Standard, Tibetan, Central", "བོད་ཡིག" },
                { "tk", "Turkmen", "Türkmen, Түркмен" },
                { "tl", "Tagalog", "Wikang Tagalog" },
                { "tn", "Tswana", "Setswana" },
                { "to", "Tonga (Tonga Islands)", "faka Tonga" },
                { "tr", "Turkish", "Türkçe" },
                { "ts", "Tsonga", "Xitsonga" },
                { "tt", "Tatar", "татарча, tatarça, تاتارچا‎" },
                { "tw", "Twi", "Twi" },
                { "ty", "Tahitian", "Reo Tahiti" },
                { "ug", "Uighur, Uyghur", "Uyƣurqə, ئۇيغۇرچە‎" },
                { "uk", "Ukrainian", "українська" },
                { "ur", "Urdu", "اردو" },
                { "uz", "Uzbek", "zbek, Ўзбек, أۇزبېك‎" },
                { "ve", "Venda", "Tshivenḓa" },
                { "vi", "Vietnamese", "Tiếng Việt" },
                { "vo", "Volapük", "Volapük" },
                { "wa", "Walloon", "Walon" },
                { "cy", "Welsh", "Cymraeg" },
                { "wo", "Wolof", "Wollof" },
                { "fy", "Western Frisian", "Frysk" },
                { "xh", "Xhosa", "isiXhosa" },
                { "yi", "Yiddish", "ייִדיש" },
                { "yo", "Yoruba", "Yorùbá" },
                { "za", "Zhuang, Chuang", "Saɯ cueŋƅ, Saw cuengh" }
            };

            var count = data.Length / 3;
            for (var i = 0; i < count; i++)
            {
                new LanguageBuilder(this.Session)
                    .WithIsoCode(data[i, 0])
                    .WithName(data[i, 1])
                    .WithNativeName(data[i, 2])
                    .Build();
            }
        }

        protected override void CoreSecure(Security config)
        {
            var full = new[] { Operations.Read, Operations.Write, Operations.Execute };

            config.GrantAdministrator(this.ObjectType, full);
        }
    }
}
