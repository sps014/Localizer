using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Localizer.Core.Model;
using Localizer.Core.Resx;
using Localizer.Helper;

namespace Localizer.ViewModels;

public record ResxEntityViewModel
{
    public required string Key { get; set; }
    public Dictionary<string, string?> CultureValues { get; set; } = new();
    public Dictionary<string, string?> CultureComments { get; set; } = new();

    public const string NeutralKeyName = "ntrKey";

    public static string GetVmKeyName(string key)
    {
        return key==string.Empty ? NeutralKeyName : key;
    }
    public static string GetKeyNameFromVm(string vmkey)
    {
        return vmkey == NeutralKeyName ? string.Empty:vmkey;
    }

    [JsonIgnore]
    public ResxEntity ResxEntity { get; init; }
    public Dictionary<string, string?> SnapshotCultureValues { get; internal set; } = new();
    public Dictionary<string, string?> SnapshotCultureComments { get; internal set; } = new();

    public ResxEntityViewModel(ResxEntity entity)
    {
        ResxEntity = entity;
        foreach(var culture in MainWindowViewModel.Instance!.ResxManager.Tree.Cultures)
        {
            var keyName = GetVmKeyName(culture);
            CultureValues.Add(keyName, entity.GetValue(culture));
            CultureComments.Add(keyName, entity.GetComment(culture));
        }
    }

    public void UpdateValue(string value,string culture)
    {
        ResxEntity.SetValue(value, culture);
    }

    public void UpdateComment(string comment,string culture)
    {
        ResxEntity.SetComment(comment, culture);
    }

    public bool IsDiffWithSnapshot()
    {
        //no snapshot loaded then its a new entry         
        if(!SnapshotLoader.IsSnapshotLoaded)
            return true;

        if(SnapshotCultureValues.Count!=CultureValues.Count)
            return true;

        if(!SnapshotCultureValues.Keys.OrderBy(x=>x).SequenceEqual(CultureValues.Keys.OrderBy(x=>x)))
            return true;

        foreach(var culture in SnapshotCultureValues.Keys)
        {
            var snapVal = SnapshotCultureValues[culture]??string.Empty;

            if (!snapVal.Equals(CultureValues[culture]??string.Empty))
                return true;
        }

        //same thing for comment 
        if (SnapshotCultureComments.Count != CultureComments.Count)
            return true;

        if (!SnapshotCultureComments.Keys.OrderBy(x => x).SequenceEqual(CultureComments.Keys.OrderBy(x=>x)))
            return true;

        foreach (var culture in SnapshotCultureComments.Keys)
        {
            var snapVal = SnapshotCultureComments[culture] ?? string.Empty;

            if (!snapVal.Equals(CultureComments[culture]??string.Empty))
                return true;
        }

        return false;
    }



    public void FindDiffAndUpdateOnResxManager()
    {
        if(Key != ResxEntity.Key)
        {
            ResxEntity.AddUpdateOrDeleteKey(KeyChangeOperationType.Update, Key);
        }
        else
            foreach(var newCulture in CultureValues.Keys)
            {
                var culture = GetKeyNameFromVm(newCulture);

                var originalVal = ResxEntity.GetValue(culture);

                if(originalVal != CultureValues[newCulture])
                    UpdateValue(CultureValues[newCulture]!, culture);

                var originalComment = ResxEntity.GetComment(culture);

                if (originalComment != CultureComments[newCulture])
                    UpdateComment(CultureComments[newCulture]!, culture);

            }


    }
}