using System.Collections.Generic;
using System.Linq;
using Domain;

namespace iBank.Services.Implementation.Shared.BuildWhereHelpers
{
    public interface IWhereTextBuilder
    {
        string AddToWhereText(string whereText, string pickName, string displayName, bool notIn, string stringToAdd);

        string AddToWhereText(string whereText, string pickName, string displayName, bool notIn, IList<string> stringsToAd, string notInText);

        string AddToWhereText(string whereText, string pickName, string displayName, bool notIn, IList<string> stringsToAd, string inText, string notInText);
    }

    public class WhereTextBuilder : IWhereTextBuilder
    {
        public string AddToWhereText(string whereText, string pickName, string displayName, bool notIn, string stringToAdd)
        {
            var op = notIn ? " <> " : " = ";

            if (string.IsNullOrEmpty(pickName))
            {
                whereText += displayName + op + stringToAdd + "; ";
            }
            else
            {
                whereText += displayName + pickName + op + stringToAdd + "; ";
            }

            return whereText;
        }

        public string AddToWhereText(string whereText, string pickName, string displayName, bool notIn, IList<string> stringsToAdd, string notInText)
        {
            //possible to specify "in list", but only pass in one value - this gets displayed differently
            if (stringsToAdd.Count() == 1)
            {
                return AddToWhereText(whereText, pickName, displayName, notIn, stringsToAdd[0]);
            }

            //if multiple values are passed in they need to be surrounded by single quotes
            var op = notIn ? " not in " : " in ";
            if (stringsToAdd.Count() == 1)
            {
                op = notIn ? " <> " : " = ";
            }

            if (string.IsNullOrEmpty(pickName))
            {
                var text = displayName + op + " ";
                for (var i = 0; i < stringsToAdd.Count; i++)
                {
                    if (i == stringsToAdd.Count - 1)
                    {
                        text += "'" + stringsToAdd[i] + "'; ";
                    }
                    else
                    {
                        text += "'" + stringsToAdd[i] + "', ";
                    }
                }
                whereText += text;
            }
            else
            {
                if (Features.WhereTextFeautureFlag.IsEnabled())
                {
                    whereText += displayName + op + pickName + "; ";
                }
                else
                {
                    whereText += displayName + notInText + pickName + "; ";
                }
            }

            return whereText;
        }

        public string AddToWhereText(string whereText, string pickName, string displayName, bool notIn, IList<string> stringsToAdd, string inText, string notInText)
        {
            //possible to specify "in list", but only pass in one value - this gets displayed differently
            if (stringsToAdd.Count() == 1)
            {
                return AddToWhereText(whereText, pickName, displayName, notIn, stringsToAdd[0]);
            }

            //if multiple values are passed in they need to be surrounded by single quotes
            var op = notIn ? notInText : inText;
            var text = "";
            if (string.IsNullOrEmpty(pickName))
            {
                for (var i = 0; i < stringsToAdd.Count; i++)
                {
                    if (i == stringsToAdd.Count - 1)
                    {
                        text += "'" + stringsToAdd[i] + "'; ";
                    }
                    else
                    {
                        text += "'" + stringsToAdd[i] + "', ";
                    }
                }
            }
            else
            {
                text += pickName + "; ";
            }
            
            return whereText += $"{displayName} {op} {text} ";
        }
    }
}
