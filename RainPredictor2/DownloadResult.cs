using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainPredictorNeuralNetwork
{
    [DelimitedRecord(","), IgnoreFirst(1)]
    public class DownloadResult
    {
        [FieldValueDiscarded]
        public string STATION;

        [FieldValueDiscarded]
        public string STATION_NAME;

        [FieldValueDiscarded]
        public string ELEVATION;

        [FieldValueDiscarded]
        public string LATITUDE;

        [FieldValueDiscarded]
        public string LONGITUDE;

        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd HH:mm")]
        public DateTime DATE;

        [FieldValueDiscarded]
        public string REPORTTPYE;

        [FieldValueDiscarded]
        public string HOURLYSKYCONDITIONS;

        [FieldValueDiscarded]
        public string HOURLYVISIBILITY;

        [FieldValueDiscarded]
        public string HOURLYPRSENTWEATHERTYPE;

        public string HOURLYDRYBULBTEMPF;

        public string HOURLYDRYBULBTEMPC;

        public string HOURLYWETBULBTEMPF;

        public string HOURLYWETBULBTEMPC;

        public string HOURLYDewPointTempF;

        public string HOURLYDewPointTempC;

        public string HOURLYRelativeHumidity;

        [FieldValueDiscarded]
        public string HOURLYWindSpeed;

        [FieldValueDiscarded]
        public string HOURLYWindDirection;

        [FieldValueDiscarded]
        public string HOURLYWindGustSpeed;

        public string HOURLYStationPressure;

        public string HOURLYPressureTendency;

        public string HOURLYPressureChange;


        public string HOURLYSeaLevelPressure;

        public string HOURLYPrecip;

        [FieldValueDiscarded]
        public string HOURLYAltimeterSetting;

        public string DAILYMaximumDryBulbTemp;

        public string DAILYMinimumDryBulbTemp;


        public string DAILYAverageDryBulbTemp;


        public string DAILYDeptFromNormalAverageTemp;

        public string DAILYAverageRelativeHumidity;

        public string DAILYAverageDewPointTemp;

        public string DAILYAverageWetBulbTemp;

        [FieldValueDiscarded]
        public string DAILYHeatingDegreeDays;

        [FieldValueDiscarded]
        public string DAILYCoolingDegreeDays;

        [FieldValueDiscarded]
        public string DAILYSunrise;

        [FieldValueDiscarded]
        public string DAILYSunset;

        [FieldValueDiscarded]
        public string DAILYWeather;

        public string DAILYPrecip;

        [FieldValueDiscarded]
        public string DAILYSnowfall;

        [FieldValueDiscarded]
        public string DAILYSnowDepth;

        [FieldValueDiscarded]
        public string DAILYAverageStationPressure;

        public double? DAILYAverageSeaLevelPressure;

        [FieldValueDiscarded]
        public string DAILYAverageWindSpeed;

        [FieldValueDiscarded]
        public string DAILYPeakWindSpeed;

        [FieldValueDiscarded]
        public string PeakWindDirection;

        [FieldValueDiscarded]
        public string DAILYSustainedWindSpeed;

        [FieldValueDiscarded]
        public string DAILYSustainedWindDirection;

        [FieldValueDiscarded]
        public string MonthlyMaximumTemp;

        [FieldValueDiscarded]
        public string MonthlyMinimumTemp;

        [FieldValueDiscarded]
        public string MonthlyMeanTemp;

        [FieldValueDiscarded]
        public string MonthlyAverageRH;

        [FieldValueDiscarded]
        public string MonthlyDewpointTemp;

        [FieldValueDiscarded]
        public string MonthlyWetBulbTemp;

        [FieldValueDiscarded]
        public string MonthlyAvgHeatingDegreeDays;

        [FieldValueDiscarded]
        public string MonthlyAvgCoolingDegreeDays;

        [FieldValueDiscarded]
        public string MonthlyStationPressure;

        [FieldValueDiscarded]
        public string MonthlySeaLevelPressure;

        [FieldValueDiscarded]
        public string MonthlyAverageWindSpeed;

        [FieldValueDiscarded]
        public string MonthlyTotalSnowfall;

        [FieldValueDiscarded]
        public string MonthlyDeptFromNormalMaximumTemp;

        [FieldValueDiscarded]
        public string MonthlyDeptFromNormalMinimumTemp;

        [FieldValueDiscarded]
        public string MonthlyDeptFromNormalAverageTemp;

        [FieldValueDiscarded]
        public string MonthlyDeptFromNormalPrecip;

        [FieldValueDiscarded]
        public string MonthlyTotalLiquidPrecip;

        [FieldValueDiscarded]
        public string MonthlyGreatestPrecip;

        [FieldValueDiscarded]
        public string MonthlyGreatestPrecipDate;

        [FieldValueDiscarded]
        public string MonthlyGreatestSnowfall;

        [FieldValueDiscarded]
        public string MonthlyGreatestSnowfallDate;

        [FieldValueDiscarded]
        public string MonthlyGreatestSnowDepth;

        [FieldValueDiscarded]
        public string MonthlyGreatestSnowDepthDate;

        [FieldValueDiscarded]
        public string MonthlyDaysWithGT90Temp;

        [FieldValueDiscarded]
        public string MonthlyDaysWithLT32Temp;

        [FieldValueDiscarded]
        public string MonthlyDaysWithGT32Temp;

        [FieldValueDiscarded]
        public string MonthlyDaysWithLT0Temp;

        [FieldValueDiscarded]
        public string MonthlyDaysWithGT001Precip;

        [FieldValueDiscarded]
        public string MonthlyDaysWithGT010Precip;

        [FieldValueDiscarded]
        public string MonthlyDaysWithGT1Snow;

        [FieldValueDiscarded]
        public string MonthlyMaxSeaLevelPressureValue;

        [FieldValueDiscarded]
        public string MonthlyMaxSeaLevelPressureDate;

        [FieldValueDiscarded]
        public string MonthlyMaxSeaLevelPressureTime;

        [FieldValueDiscarded]
        public string MonthlyMinSeaLevelPressureValue;

        [FieldValueDiscarded]
        public string MonthlyMinSeaLevelPressureDate;

        [FieldValueDiscarded]
        public string MonthlyMinSeaLevelPressureTime;

        [FieldValueDiscarded]
        public string MonthlyTotalHeatingDegreeDays;

        [FieldValueDiscarded]
        public string MonthlyTotalCoolingDegreeDays;

        [FieldValueDiscarded]
        public string MonthlyDeptFromNormalHeatingDD;

        [FieldValueDiscarded]
        public string MonthlyDeptFromNormalCoolingDD;

        [FieldValueDiscarded]
        public string MonthlyTotalSeasonToDateHeatingDD;

        [FieldValueDiscarded]
        public string MonthlyTotalSeasonToDateCoolingDD;
    }
}
