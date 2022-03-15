﻿namespace RaceControl.Extensions;

public static class PlayableContentExtensions
{
    public static string GetPreferredAudioLanguage(this IPlayableContent playableContent, string defaultAudioLanguage)
    {
        if (playableContent.ContentType != ContentType.Channel || playableContent.Name is ChannelNames.Wif or ChannelNames.Tracker or ChannelNames.Data or ChannelNames.PitLane)
        {
            return !string.IsNullOrWhiteSpace(defaultAudioLanguage) ? defaultAudioLanguage : LanguageCodes.English;
        }

        return LanguageCodes.Onboard;
    }

    public static IEnumerable<string> GetAudioLanguages(this IPlayableContent playableContent, string defaultAudioLanguage)
    {
        var languageCode = playableContent.GetPreferredAudioLanguage(defaultAudioLanguage);
        var languageCodes = new List<string> { languageCode };
        var languageCodeShort = LanguageCodes.GetTwoLetterCode(languageCode);

        if (!string.IsNullOrWhiteSpace(languageCodeShort))
        {
            languageCodes.Add(languageCodeShort);
        }

        if (languageCode != LanguageCodes.English)
        {
            languageCodes.Add(LanguageCodes.English);
            languageCodes.Add(LanguageCodes.GetTwoLetterCode(LanguageCodes.English));
        }

        return languageCodes;
    }
}