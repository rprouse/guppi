using Guppi.Domain.Entities.Covid;

namespace Guppi.Domain.Common
{
    public static class CountriesExtensions
    {
        public static Country GetCountry(this string country) =>
            country?.ToLower() switch
            {
                "canada" => Country.CAN,
                "brazil" => Country.BRA,
                "germany" => Country.DEU,
                "spain" => Country.ESP,
                "france" => Country.FRA,
                "uk" => Country.GBR,
                "great britain" => Country.GBR,
                "india" => Country.IND,
                "italy" => Country.ITA,
                "mexico" => Country.MEX,
                "usa" => Country.USA,
                "united states" => Country.USA,
                _ => Country.Unknown
            };
    }
}
