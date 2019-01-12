using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MikuMikuLibrary.IO.Sections
{
    public static class SectionRegistry
    {
        private static readonly Dictionary<Type, SectionInfo> sSectionInfosBySectionType = new Dictionary<Type, SectionInfo>();
        private static readonly Dictionary<string, SectionInfo> sSectionInfosBySignature = new Dictionary<string, SectionInfo>();

        public static IEnumerable<SectionInfo> SectionInfos => sSectionInfosBySectionType.Values;
        public static IReadOnlyDictionary<Type, SectionInfo> SectionInfosBySectionType => sSectionInfosBySectionType;
        public static IReadOnlyDictionary<string, SectionInfo> SectionInfosBySignature => sSectionInfosBySignature;

        public static SectionInfo GetOrRegisterSectionInfo( Type sectionType )
        {
            if ( !SectionInfosBySectionType.TryGetValue( sectionType, out var sectionInfo ) )
            {
                sectionInfo = new SectionInfo( sectionType );
                sSectionInfosBySectionType[ sectionType ] = sectionInfo;
                sSectionInfosBySignature[ sectionInfo.Signature ] = sectionInfo;
            }

            return sectionInfo;
        }

        public static SectionInfo Register<T>() where T : ISection => GetOrRegisterSectionInfo( typeof( T ) );

        static SectionRegistry()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var types = assembly.GetTypes().Where(
                x => typeof( Section<> ).IsAssignableFrom( x ) && x.IsClass && !x.IsAbstract );

            foreach ( var type in types )
                GetOrRegisterSectionInfo( type );
        }
    }
}
