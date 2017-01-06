using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSearchUWP.Extensions
{
    public static class ContentIndexerExtensions
    {
        public static void ToPhotoContent(this Windows.Storage.Search.IIndexableContent content, IDictionary<string, object> propertiesToSet)
        {
            var propertyKeys = new List<string>()
            {
                "System.Photo.Aperture",
                "System.Photo.ApertureDenominator",
                "System.Photo.ApertureNumerator",
                "System.Photo.Brightness",
                "System.Photo.BrightnessDenominator",
                "System.Photo.BrightnessNumerator",
                "System.Photo.CameraManufacturer",
                "System.Photo.CameraModel",
                "System.Photo.CameraSerialNumber",
                "System.Photo.Contrast",
                "System.Photo.ContrastText",
                "System.Photo.DateTaken",
                "System.Photo.DigitalZoom",
                "System.Photo.DigitalZoomDenominator",
                "System.Photo.DigitalZoomNumerator",
                "System.Photo.Event",
                "System.Photo.EXIFVersion",
                "System.Photo.ExposureBias",
                "System.Photo.ExposureBiasDenominator",
                "System.Photo.ExposureBiasNumerator",
                "System.Photo.ExposureIndex",
                "System.Photo.ExposureIndexDenominator",
                "System.Photo.ExposureIndexNumerator",
                "System.Photo.ExposureProgram",
                "System.Photo.ExposureProgramText",
                "System.Photo.ExposureTime",
                "System.Photo.ExposureTimeDenominator",
                "System.Photo.ExposureTimeNumerator",
                "System.Photo.Flash",
                "System.Photo.FlashEnergy",
                "System.Photo.FlashEnergyDenominator",
                "System.Photo.FlashEnergyNumerator",
                "System.Photo.FlashManufacturer",
                "System.Photo.FlashModel",
                "System.Photo.FlashText",
                "System.Photo.FNumber",
                "System.Photo.FNumberDenominator",
                "System.Photo.FNumberNumerator",
                "System.Photo.FocalLength",
                "System.Photo.FocalLengthDenominator",
                "System.Photo.FocalLengthInFilm",
                "System.Photo.FocalLengthNumerator",
                "System.Photo.FocalPlaneXResolution",
                "System.Photo.FocalPlaneXResolutionDenominator",
                "System.Photo.FocalPlaneXResolutionNumerator",
                "System.Photo.FocalPlaneYResolution",
                "System.Photo.FocalPlaneYResolutionDenominator",
                "System.Photo.FocalPlaneYResolutionNumerator",
                "System.Photo.GainControl",
                "System.Photo.GainControlDenominator",
                "System.Photo.GainControlNumerator",
                "System.Photo.GainControlText",
                "System.Photo.ISOSpeed",
                "System.Photo.LensManufacturer",
                "System.Photo.LensModel",
                "System.Photo.LightSource",
                "System.Photo.MakerNote",
                "System.Photo.MakerNoteOffset",
                "System.Photo.MaxAperture",
                "System.Photo.MaxApertureDenominator",
                "System.Photo.MaxApertureNumerator",
                "System.Photo.MeteringMode",
                "System.Photo.MeteringModeText",
                "System.Photo.Orientation",
                "System.Photo.OrientationText",
                "System.Photo.PeopleNames",
                "System.Photo.PhotometricInterpretation",
                "System.Photo.PhotometricInterpretationText",
                "System.Photo.ProgramMode",
                "System.Photo.ProgramModeText",
                "System.Photo.RelatedSoundFile",
                "System.Photo.Saturation",
                "System.Photo.SaturationText",
                "System.Photo.Sharpness",
                "System.Photo.SharpnessText",
                "System.Photo.ShutterSpeed",
                "System.Photo.ShutterSpeedDenominator",
                "System.Photo.ShutterSpeedNumerator",
                "System.Photo.SubjectDistance",
                "System.Photo.SubjectDistanceDenominator",
                "System.Photo.SubjectDistanceNumerator",
                "System.Photo.TagViewAggregate",
                "System.Photo.TranscodedForSync",
                "System.Photo.WhiteBalance",
                "System.Photo.WhiteBalanceText"
            };
            if (content != null && propertiesToSet != null)
            {
                foreach (var key in propertyKeys)
                {
                    if (propertiesToSet.ContainsKey(key))
                    {
                        content?.Properties.Add(key, propertiesToSet[key]);
                    }
                }
            }
        }
    }

    public enum ContentIndexerExtensionsEnum
    {
        Audio,
        Calendar,
        Communcation,
        Computer,
        Contact,
        Devices,
        Digital_Rights_Management,
        Document,
        GPS,
        Identity,
        Image,
        Journal,
        Link,
        Media,
        Message,
        Music,
        Note,
        Photo,
        PropGroup,
        PropList,
        RecordedTV,
        Search,
        Shell,
        Software,
        Sync,
        System,
        Task,
        Video,
        Volume
    }
}
